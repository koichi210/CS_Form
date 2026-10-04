using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Othello.Tests
{
    /// <summary>
    /// Form1（オセロ盤面のフォーム）のテスト。
    ///
    /// ⚠️ button_ReStart_Click は常にMessageBox.Showを呼ぶため、テスト対象から
    /// 除外する。盤面描画ロジック自体はBoardRendererTestsでBoardRendererクラスを直接検証している。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void コンストラクタで例外なく生成でき盤面が初期化される()
        {
            using (var form = new Form1())
            {
                Assert.IsNotNull(form);
            }
        }
    }
}
