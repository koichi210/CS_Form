using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reminder.Tests
{
    /// <summary>
    /// Form1（構想メモ段階のプレースホルダープロジェクト。ハンドラは未実装）
    /// のテスト。
    ///
    /// 現時点でこのプロジェクトにはInitializeComponent以外の実装が無く、
    /// 抽出できるロジックも一切存在しない。将来ロジックが実装された際にすぐ
    /// 気づけるよう、フォームの生成のみを確認するスモークテストを置いておく。
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
