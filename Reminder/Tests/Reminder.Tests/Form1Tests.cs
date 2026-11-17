using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reminder.Tests
{
    /// <summary>
    /// Form1（構想メモ段階のプレースホルダープロジェクト。ハンドラは未実装）
    /// のテスト。
    ///
    /// 画面のロジックは「毎月 第N 〇曜日」の繰り返し設定(次回の表示)だけで、
    /// 日付計算はMonthlyNthWeekdayに切り出してMonthlyNthWeekdayTestsで確かめている。
    /// ここではフォームの生成(繰り返し欄の初期化を含む)で例外が出ないことを確認する。
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
