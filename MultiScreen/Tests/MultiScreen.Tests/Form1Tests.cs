using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MouseTrainingWithMultiScreen.Tests
{
    /// <summary>
    /// Form1（ランダムな位置にダイアログをポップアップさせるマウストレーニング）の
    /// テスト。
    ///
    /// ⚠️ buttonSequencePopup_Click は ShowDialog() でモーダル表示するため、
    /// 誰かが閉じるまでテストがハングしてしまう。このハンドラはテスト対象から
    /// 除外し、Show()(モードレス)を使う buttonAllPopup_Click のみを対象にする。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void 全ポップアップボタンで指定数のダイアログが生成される()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_DlgNum", "3");

                FormReflection.InvokeHandler(form, "buttonAllPopup_Click", form);

                Assert.AreEqual(3, form.OwnedForms.Length);

                foreach (var owned in form.OwnedForms)
                {
                    owned.Close();
                }
            }
        }

        [TestMethod]
        public void 生成されるダイアログのボタン名がtextBox_ButtonNameになる()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_DlgNum", "1");
                FormReflection.SetText(form, "textBox_ButtonName", "テストボタン");

                FormReflection.InvokeHandler(form, "buttonAllPopup_Click", form);

                Assert.AreEqual(1, form.OwnedForms.Length);
                var childDlg = (ChildDlg)form.OwnedForms[0];
                Control button = FormReflection.GetControl(childDlg, "ChildDlg_button");
                Assert.AreEqual("テストボタン", button.Text);

                childDlg.Close();
            }
        }

        [TestMethod]
        public void ダイアログ生成数が0なら何も生成されない()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_DlgNum", "0");

                FormReflection.InvokeHandler(form, "buttonAllPopup_Click", form);

                Assert.AreEqual(0, form.OwnedForms.Length);
            }
        }
    }
}
