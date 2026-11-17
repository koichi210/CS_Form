using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reminder.Tests
{
    /// <summary>
    /// MonthlyNthWeekday(「毎月 第N 〇曜日」の日付計算)のテスト。
    /// 2026年11月は1日が日曜、12月は1日が火曜、2027年1月は1日が金曜、2024年2月は29日(木)まである。
    /// </summary>
    [TestClass]
    public class MonthlyNthWeekdayTests
    {
        [TestMethod]
        public void 第1から第4の曜日の日付()
        {
            Assert.AreEqual(new DateTime(2026, 11, 3), MonthlyNthWeekday.GetDate(2026, 11, WeekOfMonth.First, DayOfWeek.Tuesday));
            Assert.AreEqual(new DateTime(2026, 11, 10), MonthlyNthWeekday.GetDate(2026, 11, WeekOfMonth.Second, DayOfWeek.Tuesday));
            Assert.AreEqual(new DateTime(2026, 11, 17), MonthlyNthWeekday.GetDate(2026, 11, WeekOfMonth.Third, DayOfWeek.Tuesday));
            Assert.AreEqual(new DateTime(2026, 11, 24), MonthlyNthWeekday.GetDate(2026, 11, WeekOfMonth.Fourth, DayOfWeek.Tuesday));
        }

        [TestMethod]
        public void 月の1日がその曜日なら第1は1日()
        {
            Assert.AreEqual(new DateTime(2026, 11, 1), MonthlyNthWeekday.GetDate(2026, 11, WeekOfMonth.First, DayOfWeek.Sunday));
            Assert.AreEqual(new DateTime(2026, 12, 1), MonthlyNthWeekday.GetDate(2026, 12, WeekOfMonth.First, DayOfWeek.Tuesday));
        }

        [TestMethod]
        public void 最終はその月の最後の曜日で第4のことも第5のこともある()
        {
            // 2026年11月の火曜は4回(30日は月曜) → 最終=第4
            Assert.AreEqual(new DateTime(2026, 11, 24), MonthlyNthWeekday.GetDate(2026, 11, WeekOfMonth.Last, DayOfWeek.Tuesday));
            // 2026年7月の金曜は5回 → 最終=第5(31日)、第4は24日
            Assert.AreEqual(new DateTime(2026, 7, 31), MonthlyNthWeekday.GetDate(2026, 7, WeekOfMonth.Last, DayOfWeek.Friday));
            Assert.AreEqual(new DateTime(2026, 7, 24), MonthlyNthWeekday.GetDate(2026, 7, WeekOfMonth.Fourth, DayOfWeek.Friday));
        }

        [TestMethod]
        public void 最終は月末当日がその曜日なら月末()
        {
            Assert.AreEqual(new DateTime(2026, 12, 31), MonthlyNthWeekday.GetDate(2026, 12, WeekOfMonth.Last, DayOfWeek.Thursday));
            Assert.AreEqual(new DateTime(2026, 11, 29), MonthlyNthWeekday.GetDate(2026, 11, WeekOfMonth.Last, DayOfWeek.Sunday));
        }

        [TestMethod]
        public void うるう年の2月の最終()
        {
            Assert.AreEqual(new DateTime(2024, 2, 29), MonthlyNthWeekday.GetDate(2024, 2, WeekOfMonth.Last, DayOfWeek.Thursday));
            Assert.AreEqual(new DateTime(2024, 2, 22), MonthlyNthWeekday.GetDate(2024, 2, WeekOfMonth.Fourth, DayOfWeek.Thursday));
            // 2026年2月は28日(土)まで
            Assert.AreEqual(new DateTime(2026, 2, 28), MonthlyNthWeekday.GetDate(2026, 2, WeekOfMonth.Last, DayOfWeek.Saturday));
            Assert.AreEqual(new DateTime(2026, 2, 28), MonthlyNthWeekday.GetDate(2026, 2, WeekOfMonth.Fourth, DayOfWeek.Saturday));
        }

        [TestMethod]
        public void 次回はその月の分がまだなら今月()
        {
            DateTime next = MonthlyNthWeekday.GetNextOccurrence(
                new DateTime(2026, 11, 3, 8, 0, 0), WeekOfMonth.First, DayOfWeek.Tuesday, new TimeSpan(9, 0, 0));

            Assert.AreEqual(new DateTime(2026, 11, 3, 9, 0, 0), next);
        }

        [TestMethod]
        public void 次回は基準の日時ちょうどならその回()
        {
            DateTime next = MonthlyNthWeekday.GetNextOccurrence(
                new DateTime(2026, 11, 3, 9, 0, 0), WeekOfMonth.First, DayOfWeek.Tuesday, new TimeSpan(9, 0, 0));

            Assert.AreEqual(new DateTime(2026, 11, 3, 9, 0, 0), next);
        }

        [TestMethod]
        public void 次回はその月の分を過ぎていたら翌月()
        {
            // 当日でも時刻を過ぎていたら翌月
            DateTime sameDay = MonthlyNthWeekday.GetNextOccurrence(
                new DateTime(2026, 11, 3, 9, 0, 1), WeekOfMonth.First, DayOfWeek.Tuesday, new TimeSpan(9, 0, 0));
            Assert.AreEqual(new DateTime(2026, 12, 1, 9, 0, 0), sameDay);

            DateTime later = MonthlyNthWeekday.GetNextOccurrence(
                new DateTime(2026, 11, 18, 10, 0, 0), WeekOfMonth.First, DayOfWeek.Tuesday, new TimeSpan(9, 0, 0));
            Assert.AreEqual(new DateTime(2026, 12, 1, 9, 0, 0), later);
        }

        [TestMethod]
        public void 次回の最終を過ぎていたら翌月の最終()
        {
            DateTime next = MonthlyNthWeekday.GetNextOccurrence(
                new DateTime(2026, 11, 25), WeekOfMonth.Last, DayOfWeek.Tuesday, new TimeSpan(18, 30, 0));

            Assert.AreEqual(new DateTime(2026, 12, 29, 18, 30, 0), next);
        }

        [TestMethod]
        public void 次回は年をまたいで翌年1月()
        {
            DateTime next = MonthlyNthWeekday.GetNextOccurrence(
                new DateTime(2026, 12, 31), WeekOfMonth.First, DayOfWeek.Friday, new TimeSpan(7, 0, 0));

            Assert.AreEqual(new DateTime(2027, 1, 1, 7, 0, 0), next);
        }

        [TestMethod]
        public void 範囲外の週や時刻は例外()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => MonthlyNthWeekday.GetDate(2026, 11, (WeekOfMonth)0, DayOfWeek.Tuesday));
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => MonthlyNthWeekday.GetDate(2026, 11, (WeekOfMonth)6, DayOfWeek.Tuesday));
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => MonthlyNthWeekday.GetNextOccurrence(new DateTime(2026, 11, 1), WeekOfMonth.First, DayOfWeek.Tuesday, TimeSpan.FromHours(24)));
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => MonthlyNthWeekday.GetNextOccurrence(new DateTime(2026, 11, 1), WeekOfMonth.First, DayOfWeek.Tuesday, TimeSpan.FromMinutes(-1)));
        }

        [TestMethod]
        public void 表示名()
        {
            Assert.AreEqual("第1", MonthlyNthWeekday.GetWeekName(WeekOfMonth.First));
            Assert.AreEqual("第4", MonthlyNthWeekday.GetWeekName(WeekOfMonth.Fourth));
            Assert.AreEqual("最終", MonthlyNthWeekday.GetWeekName(WeekOfMonth.Last));
            Assert.AreEqual("日曜日", MonthlyNthWeekday.GetDayOfWeekName(DayOfWeek.Sunday));
            Assert.AreEqual("火曜日", MonthlyNthWeekday.GetDayOfWeekName(DayOfWeek.Tuesday));
            Assert.AreEqual("2026/12/01(火) 09:05", MonthlyNthWeekday.FormatDateTime(new DateTime(2026, 12, 1, 9, 5, 30)));
        }

        [TestMethod]
        public void 画面の繰り返し欄の並びと週の値が一致する()
        {
            // Form1は「しない」の次に第1〜第4・最終を並べ、選択位置をそのままWeekOfMonthに変換している
            Array values = Enum.GetValues(typeof(WeekOfMonth));
            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(i + 1, (int)values.GetValue(i));
            }
        }
    }
}
