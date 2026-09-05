using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VariableArgument.Tests
{
    /// <summary>
    /// Form1（C#にはsprintfの可変引数相当が無いことを示すサンプル）のテスト。
    ///
    /// ⚠️ button_Exceute_C_Click は常にMessageBox.Showを呼ぶため、テスト対象から
    /// 除外する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void CSボタンで入力文字列のdが置換される()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Input", "Santa_%d.raw");
                FormReflection.SetText(form, "textBox_Replace_Digit", "3");

                FormReflection.InvokeHandler(form, "button_Exceute_CS_Click", form);

                string output = FormReflection.GetControl(form, "textBox_Output").Text;
                Assert.AreEqual("Santa_3.raw", output);
            }
        }

        [TestMethod]
        public void CSボタンは複数箇所のdをまとめて置換する()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Input", "%d_%d");
                FormReflection.SetText(form, "textBox_Replace_Digit", "7");

                FormReflection.InvokeHandler(form, "button_Exceute_CS_Click", form);

                string output = FormReflection.GetControl(form, "textBox_Output").Text;
                Assert.AreEqual("7_7", output);
            }
        }
    }
}
