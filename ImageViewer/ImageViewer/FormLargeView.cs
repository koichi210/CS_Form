using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImageViewer
{
    // 一覧で選んだ画像を大きく表示する画面(WPF版ImageViewerの下の大きな表示に当たるもの)。
    // 全体表示(ウィンドウに収める)/倍率指定、ガンマ補正、動画の指定位置のコマの表示、表示中の画像のPNG保存ができる
    partial class FormLargeView : Form
    {
        // 元の画像(ガンマ補正をかける前)と、表示している画像(補正後。補正しないときは元の画像と同じもの)
        private Bitmap _original;
        private Bitmap _display;

        private String _filePath;

        // PNGで保存するときの既定のファイル名(拡張子なし)
        private String _saveBaseName;

        // 動画のコマの取り出しの世代(取り出し中に別のファイルを選んだら、古い結果は捨てる)
        private int _frameGeneration;

        // ガンマのつまみを動かしている間に毎回計算し直すと重いので、止まってから少し後に1回だけ補正する
        private readonly Timer _gammaTimer = new Timer { Interval = 150 };

        public FormLargeView()
        {
            InitializeComponent();
            InitializeToolTips();
            _gammaTimer.Tick += (s, e) =>
            {
                _gammaTimer.Stop();
                UpdateDisplay();
            };
            UpdateZoomControls();
        }

        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(checkBox_Fit, "オンなら画像全体がウィンドウに収まるように縮小/拡大する。オフなら右の倍率で表示する(スクロールできる)");
            toolTip.SetToolTip(numericUpDown_Zoom, "「全体表示」がオフのときの倍率(10〜800%)");
            toolTip.SetToolTip(trackBar_Gamma, "明るさ(ガンマ)。1.0がそのまま、大きくすると中間の暗い所が明るく、小さくすると暗くなる(0.2〜3.0)");
            toolTip.SetToolTip(button_GammaReset, "ガンマを1.0(補正なし)に戻す");
            toolTip.SetToolTip(button_SavePng, "表示中の画像(動画のコマ・ガンマ補正の結果を含む。倍率は含まない)をPNGファイルに保存する");
            toolTip.SetToolTip(textBox_VideoPosition, "動画のどこを表示するか。「秒」なら 12.5 や 1:23.5(分:秒)、「コマ」なら1始まりのコマ番号(1が先頭)。Enterキーで表示");
            toolTip.SetToolTip(textBox_Fps, "「コマ」で指定したときに時間へ直すための1秒あたりのコマ数(例: 29.97)。PATHにffmpeg.exeがあるときはffmpegがコマ番号どおりに取り出すので使わない");
            toolTip.SetToolTip(button_ShowFrame, "表示中の動画の、指定した位置のコマを表示する");
        }

        // 指定したファイルを表示する。画像はそのまま、動画は指定した位置のコマを取り出して表示する
        public void ShowFile(String filePath)
        {
            _filePath = filePath;
            this.Text = "拡大表示 - " + Path.GetFileName(filePath);

            if (Preview.IsVideoFile(filePath))
            {
                ShowVideoFrame();
                return;
            }

            _frameGeneration++;
            Bitmap bitmap = LoadBitmap(filePath);
            label_Status.Text = (bitmap == null) ? "画像を読み込めませんでした" : bitmap.Width + "×" + bitmap.Height;
            SetOriginal(bitmap, Path.GetFileNameWithoutExtension(filePath));
        }

        // 画像を読む。Image.FromFileのままだとファイルを握り続けるので、コピーを作ってすぐ閉じる
        private static Bitmap LoadBitmap(String filePath)
        {
            try
            {
                using (Image image = Image.FromFile(filePath))
                {
                    return new Bitmap(image);
                }
            }
            catch (Exception ex) when (ex is OutOfMemoryException || ex is IOException
                || ex is UnauthorizedAccessException || ex is ArgumentException)
            {
                return null;
            }
        }

        private void ShowVideoFrame()
        {
            if (!Preview.IsVideoFile(_filePath))
            {
                label_Status.Text = "動画ファイルを表示しているときだけ使えます";
                return;
            }

            VideoPosition position;
            String error;
            if (!VideoPosition.TryParse(textBox_VideoPosition.Text, radioButton_Frame.Checked, textBox_Fps.Text, out position, out error))
            {
                label_Status.Text = error;
                return;
            }

            int generation = ++_frameGeneration;
            String videoPath = _filePath;
            label_Status.Text = "コマを取り出しています...";
            VideoFrameGrabber.Grab(videoPath, position, (image, grabError) =>
            {
                if (IsDisposed || generation != _frameGeneration)
                {
                    return;
                }
                if (image == null)
                {
                    label_Status.Text = grabError;
                    return;
                }
                label_Status.Text = position.IsFrame
                    ? position.FrameNumber + "コマ目(" + position.Time.TotalSeconds.ToString("0.000") + "秒付近)"
                    : position.Time.TotalSeconds.ToString("0.000") + "秒のコマ";
                SetOriginal(ToBitmap(image), Path.GetFileNameWithoutExtension(videoPath) + "_" + position.ToFileNameSuffix());
            });
        }

        // WPFの画像(VideoFrameGrabberの結果)をWinFormsのBitmapにする(PNGにしてから読み直す)
        private static Bitmap ToBitmap(System.Windows.Media.Imaging.BitmapSource source)
        {
            System.Windows.Media.Imaging.PngBitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(source));
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Position = 0;
                using (Bitmap loaded = new Bitmap(stream))
                {
                    // ストリームから作ったBitmapはストリームを閉じると使えなくなるので、コピーを持つ
                    return new Bitmap(loaded);
                }
            }
        }

        private void SetOriginal(Bitmap bitmap, String saveBaseName)
        {
            Bitmap old = _original;
            _original = bitmap;
            _saveBaseName = saveBaseName;
            UpdateDisplay();
            if (old != null && old != _original)
            {
                old.Dispose();
            }
        }

        // 元の画像にガンマ補正をかけて表示し直す
        private void UpdateDisplay()
        {
            double gamma = trackBar_Gamma.Value / 10.0;
            label_GammaValue.Text = gamma.ToString("0.0");

            Bitmap oldDisplay = _display;
            _display = (_original == null || GammaCorrection.IsIdentity(gamma)) ? _original : ApplyGamma(_original, gamma);
            pictureBox_Image.Image = _display;
            if (oldDisplay != null && oldDisplay != _display && oldDisplay != _original)
            {
                oldDisplay.Dispose();
            }
            UpdateZoomControls();
        }

        // ガンマ補正した新しい画像を作る。補正の計算はGammaCorrection(テスト済み)
        public static Bitmap ApplyGamma(Bitmap source, double gamma)
        {
            Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height));
            }

            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
            BitmapData data = result.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                // Format32bppArgbはメモリ上で B,G,R,A の順に並ぶ
                byte[] pixels = new byte[Math.Abs(data.Stride) * result.Height];
                Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
                GammaCorrection.ApplyToBgra(pixels, GammaCorrection.BuildTable(gamma));
                Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            }
            finally
            {
                result.UnlockBits(data);
            }
            return result;
        }

        // 全体表示ならウィンドウに収め、そうでなければ倍率どおりの大きさにしてスクロールで見る
        private void UpdateZoomControls()
        {
            numericUpDown_Zoom.Enabled = !checkBox_Fit.Checked;
            if (checkBox_Fit.Checked || _display == null)
            {
                pictureBox_Image.Dock = DockStyle.Fill;
                pictureBox_Image.SizeMode = PictureBoxSizeMode.Zoom;
                return;
            }

            double scale = (double)numericUpDown_Zoom.Value / 100.0;
            pictureBox_Image.Dock = DockStyle.None;
            pictureBox_Image.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox_Image.Location = new Point(0, 0);
            pictureBox_Image.Size = new Size(Math.Max(1, (int)(_display.Width * scale)), Math.Max(1, (int)(_display.Height * scale)));
        }

        private void checkBox_Fit_CheckedChanged(object sender, EventArgs e)
        {
            UpdateZoomControls();
        }

        private void numericUpDown_Zoom_ValueChanged(object sender, EventArgs e)
        {
            UpdateZoomControls();
        }

        private void trackBar_Gamma_ValueChanged(object sender, EventArgs e)
        {
            label_GammaValue.Text = (trackBar_Gamma.Value / 10.0).ToString("0.0");
            _gammaTimer.Stop();
            _gammaTimer.Start();
        }

        private void button_GammaReset_Click(object sender, EventArgs e)
        {
            trackBar_Gamma.Value = 10;
        }

        private void button_ShowFrame_Click(object sender, EventArgs e)
        {
            ShowVideoFrame();
        }

        private void textBox_VideoPosition_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ShowVideoFrame();
            }
        }

        private void button_SavePng_Click(object sender, EventArgs e)
        {
            if (_display == null)
            {
                MessageBox.Show(this, "保存する画像がありません");
                return;
            }

            // 既定の名前は「元のファイル名.png」、動画のコマなら「元のファイル名_12.500s.png」「元のファイル名_f120.png」
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG画像 (*.png)|*.png";
                dialog.FileName = (String.IsNullOrEmpty(_saveBaseName) ? "image" : _saveBaseName) + ".png";
                String folder = String.IsNullOrEmpty(_filePath) ? null : Path.GetDirectoryName(_filePath);
                if (folder != null && Directory.Exists(folder))
                {
                    dialog.InitialDirectory = folder;
                }
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    _display.Save(dialog.FileName, ImageFormat.Png);
                    label_Status.Text = "保存しました: " + Path.GetFileName(dialog.FileName);
                }
                catch (Exception ex) when (ex is ExternalException || ex is IOException || ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(this, "保存できませんでした" + Environment.NewLine + ex.Message);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gammaTimer.Dispose();
            _frameGeneration++;
            pictureBox_Image.Image = null;
            if (_display != null && _display != _original)
            {
                _display.Dispose();
            }
            if (_original != null)
            {
                _original.Dispose();
            }
            _display = null;
            _original = null;
            base.OnFormClosed(e);
        }
    }
}
