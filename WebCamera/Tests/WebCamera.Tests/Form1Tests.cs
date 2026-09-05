using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebCamera.Tests
{
    /// <summary>
    /// Form1（Webカメラ映像をPictureBoxに表示するツール）のテスト。
    ///
    /// ⚠️ button1_Click は実際のカメラデバイス(VideoCapture(0))を開こうとする。
    /// カメラが無い環境ではMessageBox.Showを呼んでこのFormをClose()してしまい、
    /// カメラがある環境ではCI/テスト実行機の物理カメラを実際に掴んでしまう。
    /// backgroundWorker1_DoWork も同様に実カメラへの継続アクセスを行う。
    /// どちらも安全にテストできないため、この2つはテスト対象から除外し、
    /// フォームの生成とFormClosing(ワーカー未起動時の安全な終了)のみを検証する。
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
            }
        }

        [TestMethod]
        public void ワーカー未起動ならFormClosingはハングせず終了する()
        {
            using (var form = new Form1())
            {
                // backgroundWorker1.IsBusy が false のままなら
                // while (IsBusy) Application.DoEvents(); のループには入らず即座に戻る
                FormReflection.InvokeHandler(form, "Form1_FormClosing", form, new FormClosingEventArgs(CloseReason.UserClosing, false));
            }
        }
    }
}
