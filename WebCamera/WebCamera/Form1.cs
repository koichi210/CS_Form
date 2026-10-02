using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;       // Bitmap変換に必要

namespace WebCamera
{
    /// <summary>
    /// Webカメラのライブ映像を表示し、静止画(PNG)を保存するツール。
    ///
    /// 【スレッドの役割分担】
    /// - カメラを開く・フレームを読む・Bitmapへ変換する … BackgroundWorker(ワーカースレッド)
    /// - pictureBox_Preview への表示・古いBitmapの破棄 … UIスレッド(ProgressChanged)
    /// ワーカーはフレーム毎に「独立した」Bitmapを作ってUIへ渡し、以後そのBitmapには触らない。
    /// 以前の実装は Mat のバッファを直接参照する Bitmap を共有していたため、
    /// ワーカーが書き込み中のバッファをUIが描画してしまう危険があった。
    ///
    /// 【カメラの寿命】
    /// VideoCapture は DoWork の中だけで生成・破棄する(using)。
    /// 停止もフォームを閉じるときも、ワーカーにキャンセルを要求して終了を待つだけで
    /// カメラは必ずワーカー自身が解放するので、UIスレッドとの取り合いが起きない。
    /// </summary>
    public partial class Form1 : Form
    {
        // UIがまだ表示していない(ProgressChangedが処理されていない)フレーム数。
        // UIの描画がカメラのフレームレートに追いつかないとき、未処理のBitmapが
        // メッセージキューに溜まり続けてメモリを食い潰さないよう、ワーカー側で間引く。
        private int pendingFrames;
        private const int MaxPendingFrames = 2;

        // フォームを閉じようとしたときにワーカーが動いていたら、いったん閉じるのを取り消し、
        // ワーカーの終了(RunWorkerCompleted)を待ってから改めて閉じる。そのための目印。
        private bool closeRequested;

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();
            UpdateButtons();
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(button_StartStop, "PCの1台目のカメラの映像表示を開始/停止する");
            toolTip.SetToolTip(button_Snapshot, "押した瞬間に表示中の映像をPNGで保存する。停止後も最後に表示した映像を保存できる");
        }

        private void button_StartStop_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_Capture.IsBusy)
            {
                // 停止要求。カメラの解放はワーカー自身が行い、RunWorkerCompleted で停止状態に戻る
                backgroundWorker_Capture.CancelAsync();
                button_StartStop.Enabled = false;
                return;
            }

            //画像取得スレッド開始(カメラを開くのもワーカー側。開くのに数秒かかってもUIは固まらない)
            backgroundWorker_Capture.RunWorkerAsync();
            UpdateButtons();
        }

        private void backgroundWorker_Capture_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            //カメラ画像取得用のVideoCapture作成
            using (VideoCapture capture = new VideoCapture(0))
            using (Mat frame = new Mat())
            {
                if (!capture.IsOpened())
                {
                    e.Result = "カメラが見つかりませんでした";
                    return;
                }

                while (!worker.CancellationPending)
                {
                    //画像取得
                    if (!capture.Read(frame) || frame.Empty())
                    {
                        // カメラが抜かれた等で取得できなくなったら、空回りせずに終了する
                        e.Result = "カメラから画像を取得できなくなりました";
                        return;
                    }

                    // UIが前のフレームをまだ表示できていなければ、このフレームは捨てる
                    if (Volatile.Read(ref pendingFrames) >= MaxPendingFrames)
                    {
                        continue;
                    }

                    // frame のバッファは次の Read で上書きされるので、独立した Bitmap にコピーしてから渡す。
                    // 渡した Bitmap の所有権はUI側に移り、破棄もUI側が行う
                    Bitmap bmp = BitmapConverter.ToBitmap(frame);
                    Interlocked.Increment(ref pendingFrames);
                    worker.ReportProgress(0, bmp);
                }

                e.Cancel = true;
            }
        }

        private void backgroundWorker_Capture_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Interlocked.Decrement(ref pendingFrames);

            Bitmap bmp = e.UserState as Bitmap;
            if (bmp == null)
            {
                return;
            }

            // 閉じている最中に届いたフレームは表示せず、リークしないよう破棄だけする
            if (closeRequested || IsDisposed || Disposing)
            {
                bmp.Dispose();
                return;
            }

            //描画(差し替えた古いBitmapは、もう誰も参照していないので破棄する)
            Image old = pictureBox_Preview.Image;
            pictureBox_Preview.Image = bmp;
            if (old != null)
            {
                old.Dispose();
            }

            UpdateButtons();
        }

        private void backgroundWorker_Capture_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pendingFrames = 0;

            if (closeRequested)
            {
                // FormClosing で保留していた終了を、ワーカーが止まった今あらためて実行する
                Close();
                return;
            }

            UpdateButtons();

            if (e.Error != null)
            {
                MessageBox.Show(this, "カメラの処理中にエラーが発生しました" + Environment.NewLine + e.Error.Message,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!e.Cancelled && e.Result != null)
            {
                MessageBox.Show(this, e.Result.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_Snapshot_Click(object sender, EventArgs e)
        {
            if (pictureBox_Preview.Image == null)
            {
                return;
            }

            // ダイアログを開いている間も映像は更新され、表示中の Bitmap は差し替え時に破棄される。
            // 押した瞬間の映像を保存したいので、先に複製しておく
            using (Bitmap snapshot = new Bitmap(pictureBox_Preview.Image))
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.FileName = SnapshotFile.CreateDefaultFileName(DateTime.Now);
                dialog.InitialDirectory = SnapshotFile.GetInitialDirectory();
                dialog.Filter = SnapshotFile.DialogFilter;
                dialog.DefaultExt = SnapshotFile.Extension;
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    snapshot.Save(dialog.FileName, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "画像を保存できませんでした" + Environment.NewLine + ex.Message,
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 実行状態・映像の有無に合わせて、ボタンの表示と有効/無効を揃える。
        /// </summary>
        private void UpdateButtons()
        {
            bool running = backgroundWorker_Capture.IsBusy;
            button_StartStop.Text = running ? "Stop" : "Start";
            button_StartStop.Enabled = !running || !backgroundWorker_Capture.CancellationPending;
            // 停止後も最後に表示したフレームは残るので、それも保存できるようにしておく
            button_Snapshot.Enabled = pictureBox_Preview.Image != null;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!backgroundWorker_Capture.IsBusy)
            {
                return;
            }

            // DoEvents で回して待つ代わりに、いったん閉じるのを取り消してワーカーに停止を要求する。
            // ワーカーがカメラを解放して終わると RunWorkerCompleted から Close() し直す
            e.Cancel = true;
            closeRequested = true;
            backgroundWorker_Capture.CancelAsync();
            button_StartStop.Enabled = false;
            button_Snapshot.Enabled = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 最後に表示していた Bitmap を解放する
            Image old = pictureBox_Preview.Image;
            pictureBox_Preview.Image = null;
            if (old != null)
            {
                old.Dispose();
            }
        }
    }
}
