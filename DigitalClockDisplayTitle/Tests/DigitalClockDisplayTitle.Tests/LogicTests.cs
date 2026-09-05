using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DigitalClockDisplayTitle.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、時刻を "HH:mm:ss" 形式に整形するロジック）の
    /// テスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void 時刻を2桁ゼロ埋めで整形する()
        {
            DateTime d = new DateTime(2024, 1, 1, 9, 5, 3);

            string result = Logic.FormatTime(d);

            Assert.AreEqual("09:05:03", result);
        }

        [TestMethod]
        public void 既に2桁の時刻はそのまま整形する()
        {
            DateTime d = new DateTime(2024, 1, 1, 23, 59, 59);

            string result = Logic.FormatTime(d);

            Assert.AreEqual("23:59:59", result);
        }

        [TestMethod]
        public void 深夜0時も正しく整形する()
        {
            DateTime d = new DateTime(2024, 1, 1, 0, 0, 0);

            string result = Logic.FormatTime(d);

            Assert.AreEqual("00:00:00", result);
        }
    }
}
