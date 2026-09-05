using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WeeklyReportFormater.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、週報テキストの整形ロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void FormatThisWeekはユーザー名を括弧に変換しストーリーポイントの小数点0を削る()
        {
            string before = "Yamada 3.0";

            string result = Logic.FormatThisWeek(before, "Yamada");

            Assert.AreEqual("\t(3)" + Environment.NewLine, result);
        }

        [TestMethod]
        public void FormatThisWeekはタブを削除してから先頭にタブを付け直す()
        {
            string before = "Yamada\tFix login bug";

            string result = Logic.FormatThisWeek(before, "Yamada");

            // タブを削除すると "Yamada" と "Fix" の間の区切りが消えるため、
            // 続く "Yamada " 置換の対象にはならない（元の挙動どおり）
            Assert.AreEqual("\tYamadaFix login bug)" + Environment.NewLine, result);
        }

        [TestMethod]
        public void FormatThisWeekは複数行をまとめて処理できる()
        {
            string before = "Yamada 1.0" + Environment.NewLine + "Yamada 2.0";

            string result = Logic.FormatThisWeek(before, "Yamada");

            string expected = "\t(1)" + Environment.NewLine + "\t(2)" + Environment.NewLine;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FormatNextWeekは3行1組で課題番号タイトルポイントを整形する()
        {
            string before = "12345" + Environment.NewLine + "Fix login bug" + Environment.NewLine + "Yamada 2.0";

            string result = Logic.FormatNextWeek(before, "Yamada");

            string expected = "\t12345" + " Fix login bug" + " ( 2.0)" + Environment.NewLine;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FormatNextWeekは複数組をまとめて処理できる()
        {
            string before = "111" + Environment.NewLine + "Task A" + Environment.NewLine + "Yamada 1.0" +
                             Environment.NewLine + "222" + Environment.NewLine + "Task B" + Environment.NewLine + "Yamada 2.0";

            string result = Logic.FormatNextWeek(before, "Yamada");

            StringAssert.Contains(result, "\t111");
            StringAssert.Contains(result, "\t222");
            StringAssert.Contains(result, " Task A");
            StringAssert.Contains(result, " Task B");
        }

        [TestMethod]
        public void FormatPerforceはProjectIDとSummaryを1行にまとめる()
        {
            string before = "PROJ-1" + Environment.NewLine + "  Fix login bug  ";

            string result = Logic.FormatPerforce(before);

            Assert.AreEqual("PROJ-1 Fix login bug", result);
        }

        [TestMethod]
        public void FormatPerforceは複数組をまとめて処理できる()
        {
            string before = "PROJ-1" + Environment.NewLine + "Summary1" + Environment.NewLine + "PROJ-2" + Environment.NewLine + "Summary2";

            string result = Logic.FormatPerforce(before);

            Assert.AreEqual("PROJ-1 Summary1PROJ-2 Summary2", result);
        }

        [TestMethod]
        public void FormatPerforceは前後の空白をトリムする()
        {
            string before = "   PROJ-1   " + Environment.NewLine + "   Summary   ";

            string result = Logic.FormatPerforce(before);

            Assert.AreEqual("PROJ-1 Summary", result);
        }
    }
}
