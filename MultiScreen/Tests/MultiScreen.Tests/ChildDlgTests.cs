using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MouseTrainingWithMultiScreen.Tests
{
    /// <summary>
    /// ChildDlg（ポップアップされる子ダイアログ）のテスト。
    /// </summary>
    [TestClass]
    public class ChildDlgTests
    {
        [TestMethod]
        public void コンストラクタでボタンのテキストが設定される()
        {
            using (var dlg = new ChildDlg("押してね"))
            {
                Control button = FormReflection.GetControl(dlg, "ChildDlg_button");
                Assert.AreEqual("押してね", button.Text);
            }
        }

        [TestMethod]
        public void ボタンクリックでダイアログが閉じる()
        {
            var dlg = new ChildDlg("Close");

            FormReflection.InvokeHandler(dlg, "buttonAllPopup_Click", dlg);

            // 表示前(ハンドル未生成)のFormをCloseすると、FormClosingを発火せず
            // 直接Disposeされる(WinFormsの既定動作)。Disposeされたことを確認する。
            Assert.IsTrue(dlg.IsDisposed);
        }
    }
}
