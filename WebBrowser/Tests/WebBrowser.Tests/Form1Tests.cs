using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebBrowser.Tests
{
    /// <summary>
    /// Form1（WebBrowserコントロールでURLを表示するサンプル）のテスト。
    ///
    /// ⚠️ button_Go_Click は実際に埋め込みInternet Explorer(WebBrowserコントロール)
    /// でページ遷移を行う(実ネットワークアクセス)ため、有効なURLを指定した場合の
    /// 挙動はテストしない。button_Test_Clickは常にMessageBox.Showを呼ぶ。
    /// 安全にテストできるのは、textBox_Urlが空の場合に何もせず(ナビゲーションも
    /// MessageBoxも発生させず)早期リターンする分岐のみ。
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
        public void URLが空なら何もせず早期リターンする()
        {
            using (var form = new Form1())
            {
                var textBoxUrl = FormReflection.GetControl(form, "textBox_Url");
                textBoxUrl.Text = "";

                // ナビゲーションもMessageBoxも発生せず正常に戻ることを確認する
                FormReflection.InvokeHandler(form, "button_Go_Click", form);
            }
        }
    }
}
