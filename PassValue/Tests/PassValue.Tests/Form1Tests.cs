using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PassValue.Tests
{
    /// <summary>
    /// Form1（子ダイアログから値を受け取るサンプル）のテスト。
    ///
    /// ⚠️ button_Click_PopupWindow は FormSub.ShowDialog() を呼び、誰かが閉じる
    /// までモーダル表示でブロックするため、テストがハングしてしまう。この
    /// ハンドラはテスト対象から除外する。FormSub側のロジックはFormSubTestsで
    /// 個別に検証している。
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
    }
}
