using System;
using System.Globalization;

namespace Reminder
{
    // 「毎月 第N 〇曜日」の第N。最終は、その月の最後の〇曜日(第4のこともあれば第5のこともある)
    public enum WeekOfMonth
    {
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Last = 5,
    }

    /// <summary>
    /// 「毎月 第1火曜」「毎月 最終金曜」のように、毎月決まった週・曜日で繰り返すリマインドの日付計算。
    /// 画面(Form1)から切り離して、テストで日付を確かめられるようにしてある。
    /// </summary>
    public static class MonthlyNthWeekday
    {
        // 曜日の表示名(DayOfWeekの順: 日曜=0)。PCの言語設定に左右されないよう固定で持つ
        private static readonly String[] _dayOfWeekNames = { "日", "月", "火", "水", "木", "金", "土" };
        private const int _daysInWeek = 7;

        /// <summary>指定した年月の「第N 〇曜日」の日付(時刻は0:00)。</summary>
        public static DateTime GetDate(int year, int month, WeekOfMonth week, DayOfWeek dayOfWeek)
        {
            ValidateWeek(week);

            if (week == WeekOfMonth.Last)
            {
                // 月末から遡って最初の〇曜日
                DateTime lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                int back = ((int)lastDay.DayOfWeek - (int)dayOfWeek + _daysInWeek) % _daysInWeek;
                return lastDay.AddDays(-back);
            }

            // 1日から進んで最初の〇曜日 + (N-1)週
            DateTime firstDay = new DateTime(year, month, 1);
            int forward = ((int)dayOfWeek - (int)firstDay.DayOfWeek + _daysInWeek) % _daysInWeek;
            return firstDay.AddDays(forward + ((int)week - 1) * _daysInWeek);
        }

        /// <summary>
        /// from 以降(from ちょうども含む)で最初に来る「毎月 第N 〇曜日 の timeOfDay」の日時。
        /// その月の分が from より前なら翌月の分を返す。
        /// </summary>
        public static DateTime GetNextOccurrence(DateTime from, WeekOfMonth week, DayOfWeek dayOfWeek, TimeSpan timeOfDay)
        {
            ValidateWeek(week);
            if (timeOfDay < TimeSpan.Zero || timeOfDay >= TimeSpan.FromDays(1))
            {
                throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay, "時刻は0:00〜23:59:59の範囲で指定してください");
            }

            DateTime month = new DateTime(from.Year, from.Month, 1);
            DateTime candidate = GetDate(month.Year, month.Month, week, dayOfWeek).Add(timeOfDay);
            if (candidate >= from)
            {
                return candidate;
            }

            DateTime nextMonth = month.AddMonths(1);
            return GetDate(nextMonth.Year, nextMonth.Month, week, dayOfWeek).Add(timeOfDay);
        }

        /// <summary>画面に出す曜日名。例: 火曜日</summary>
        public static String GetDayOfWeekName(DayOfWeek dayOfWeek)
        {
            return _dayOfWeekNames[(int)dayOfWeek] + "曜日";
        }

        /// <summary>画面に出す第N。例: 第1、最終</summary>
        public static String GetWeekName(WeekOfMonth week)
        {
            ValidateWeek(week);
            return week == WeekOfMonth.Last ? "最終" : "第" + (int)week;
        }

        /// <summary>画面に出す日時。例: 2026/12/01(火) 09:00</summary>
        public static String FormatDateTime(DateTime dateTime)
        {
            // 「/」「:」はPCの地域設定の区切り文字に置き換わるので、地域設定に左右されない書式で出す
            return String.Format(CultureInfo.InvariantCulture, "{0:yyyy/MM/dd}({1}) {0:HH:mm}",
                dateTime, _dayOfWeekNames[(int)dateTime.DayOfWeek]);
        }

        private static void ValidateWeek(WeekOfMonth week)
        {
            if (week < WeekOfMonth.First || week > WeekOfMonth.Last)
            {
                throw new ArgumentOutOfRangeException(nameof(week), week, "第1〜第4・最終のどれかを指定してください");
            }
        }
    }
}
