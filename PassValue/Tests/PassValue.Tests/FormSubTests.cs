using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PassValue.Tests
{
    /// <summary>
    /// FormSub（値入力用のモーダルダイアログ）のテスト。
    /// </summary>
    [TestClass]
    public class FormSubTests
    {
        [TestMethod]
        public void 設定ボタンでtextBox1の内容がvalueプロパティに反映される()
        {
            using (var fs = new FormSub())
            {
                var textBox1 = (TextBox)FormReflection.GetControl(fs, "textBox1");
                textBox1.Text = "入力された値";

                FormReflection.InvokeHandler(fs, "button_Click_Setting", fs);

                Assert.AreEqual("入力された値", fs.value);
            }
        }

        [TestMethod]
        public void OKボタンでDialogResultがOKになり閉じる()
        {
            var fs = new FormSub();

            FormReflection.InvokeHandler(fs, "buttonOk_Click", fs);

            Assert.AreEqual(DialogResult.OK, fs.DialogResult);
            // 表示前(ハンドル未生成)のFormをCloseすると直接Disposeされる
            Assert.IsTrue(fs.IsDisposed);
        }

        [TestMethod]
        public void キャンセルボタンでDialogResultがCancelになり閉じる()
        {
            var fs = new FormSub();

            FormReflection.InvokeHandler(fs, "buttonCancel_Click", fs);

            Assert.AreEqual(DialogResult.Cancel, fs.DialogResult);
            Assert.IsTrue(fs.IsDisposed);
        }
    }
}
