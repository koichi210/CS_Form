using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContextHelp.Tests
{
    /// <summary>
    /// Form1（HelpProviderによるポップアップヘルプのサンプル）のテスト。
    ///
    /// ⚠️ button1_Click は常にMessageBox.Showを呼ぶため、テスト対象から除外する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void Form1_Loadで各コントロールにヘルプ文字列が設定される()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "Form1_Load", form);

                var popupHelp = (HelpProvider)FormReflection.GetField(form, "popupHelp");
                Control label1 = FormReflection.GetControl(form, "label1");
                Control button1 = FormReflection.GetControl(form, "button1");
                Control checkBox1 = FormReflection.GetControl(form, "checkBox1");

                Assert.AreEqual("ラベルのポップアップヘルプだよ", popupHelp.GetHelpString(label1));
                Assert.AreEqual("ボタンのポップアップヘルプだよ", popupHelp.GetHelpString(button1));
                Assert.AreEqual("チェックボックスのポップアップヘルプだよ", popupHelp.GetHelpString(checkBox1));
            }
        }
    }
}
