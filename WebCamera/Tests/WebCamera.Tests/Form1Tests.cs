using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebCamera.Tests
{
    /// <summary>
    /// Form1（Webカメラ映像をPictureBoxに表示するツール）のテスト。
    ///
    /// ⚠️ button_StartStop_Click / backgroundWorker_Capture_DoWork は実際のカメラデバイス
    /// (VideoCapture(0))を開こうとする。カメラがある環境ではテスト実行機の物理カメラを
    /// 実際に掴んでしまい、無い環境では RunWorkerCompleted で MessageBox を出して止まる。
    /// どちらも安全にテストできないため、この2つはテスト対象から除外する。
    /// 代わりに、カメラに触れない部分（画面構成・フレーム受け取り時の差し替えと破棄・
    /// ワーカー未起動時の終了処理）を検証する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void コンストラクタで例外なく生成できる()
        {
            using (var form = new Form1())
            {
                Assert.IsNotNull(form);
                Assert.AreEqual("WebCamera", form.Text);
            }
        }

        [TestMethod]
        public void 初期状態はStart表示でSnapshotは無効()
        {
            using (var form = new Form1())
            {
                Button startStop = (Button)FormReflection.GetControl(form, "button_StartStop");
                Button snapshot = (Button)FormReflection.GetControl(form, "button_Snapshot");

                Assert.AreEqual("Start", startStop.Text);
                Assert.IsTrue(startStop.Enabled);
                Assert.AreEqual("Snapshot", snapshot.Text);
                Assert.IsFalse(snapshot.Enabled, "映像が無いうちは保存できない");
            }
        }

        [TestMethod]
        public void プレビューは縦横比を保って表示する()
        {
            using (var form = new Form1())
            {
                PictureBox preview = (PictureBox)FormReflection.GetControl(form, "pictureBox_Preview");
                Assert.AreEqual(PictureBoxSizeMode.Zoom, preview.SizeMode);
            }
        }

        [TestMethod]
        public void TabIndexは左上から右上_左下_右下の順()
        {
            using (var form = new Form1())
            {
                Assert.AreEqual(0, FormReflection.GetControl(form, "pictureBox_Preview").TabIndex);
                Assert.AreEqual(1, FormReflection.GetControl(form, "button_StartStop").TabIndex);
                Assert.AreEqual(2, FormReflection.GetControl(form, "button_Snapshot").TabIndex);
            }
        }

        [TestMethod]
        public void フレームを受け取るとプレビューに表示されSnapshotが有効になる()
        {
            using (var form = new Form1())
            {
                PictureBox preview = (PictureBox)FormReflection.GetControl(form, "pictureBox_Preview");
                Button snapshot = (Button)FormReflection.GetControl(form, "button_Snapshot");
                var bmp = new Bitmap(4, 3);

                FormReflection.InvokeHandler(form, "backgroundWorker_Capture_ProgressChanged", form, new ProgressChangedEventArgs(0, bmp));

                Assert.AreSame(bmp, preview.Image);
                Assert.IsTrue(snapshot.Enabled);
            }
        }

        [TestMethod]
        public void 次のフレームで差し替えると古いBitmapは破棄される()
        {
            using (var form = new Form1())
            {
                PictureBox preview = (PictureBox)FormReflection.GetControl(form, "pictureBox_Preview");
                var first = new Bitmap(4, 3);
                var second = new Bitmap(4, 3);

                FormReflection.InvokeHandler(form, "backgroundWorker_Capture_ProgressChanged", form, new ProgressChangedEventArgs(0, first));
                FormReflection.InvokeHandler(form, "backgroundWorker_Capture_ProgressChanged", form, new ProgressChangedEventArgs(0, second));

                Assert.AreSame(second, preview.Image);
                AssertDisposed(first);
                Assert.AreEqual(4, second.Width, "表示中のBitmapは生きている");
            }
        }

        [TestMethod]
        public void フォームを閉じると表示中のBitmapも破棄される()
        {
            var form = new Form1();
            var bmp = new Bitmap(4, 3);
            FormReflection.InvokeHandler(form, "backgroundWorker_Capture_ProgressChanged", form, new ProgressChangedEventArgs(0, bmp));

            FormReflection.InvokeHandler(form, "Form1_FormClosed", form, new FormClosedEventArgs(CloseReason.UserClosing));
            form.Dispose();

            AssertDisposed(bmp);
        }

        [TestMethod]
        public void ワーカー未起動ならFormClosingは閉じるのを止めない()
        {
            using (var form = new Form1())
            {
                var args = new FormClosingEventArgs(CloseReason.UserClosing, false);
                FormReflection.InvokeHandler(form, "Form1_FormClosing", form, args);
                Assert.IsFalse(args.Cancel);
            }
        }

        private static void AssertDisposed(Bitmap bmp)
        {
            // 破棄済みの Bitmap はプロパティ参照で ArgumentException を投げる
            try
            {
                int unused = bmp.Width;
                Assert.Fail("Bitmap が破棄されていない");
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
