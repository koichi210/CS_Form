using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mailer
{
    /// <summary>
    /// もともと Form1.cs に private メソッドとして埋め込まれていた、メール件名・本文の
    /// プレースホルダ置換ロジックを、テストできる形に切り出したもの。
    ///
    /// 置換結果(書式)は切り出し前と同じ。
    /// </summary>
    internal static class Logic
    {
        /// <summary>件名・本文の中の %%usersday%% / %%today%% / %%dayofweek%% 等をすべて置換する。</summary>
        public static String GetReplaceDay(String srcText, DateTime userDate)
        {
            var newText = GetUsersDay(srcText, userDate);
            newText = GetDateText(newText);
            newText = GetDayOfWeek(newText, userDate);
            return newText;
        }

        public static String GetDayOfWeek(String srcText, DateTime dt)
        {
            String destText = srcText.Replace("%%dayofweek%%", dt.ToString("ddd"));
            destText = destText.Replace("%%DAYOFWEEK%%", dt.ToString("dddd"));
            return destText;
        }

        public static String GetUsersDay(String srcText, DateTime userDate)
        {
            String destText = ReplaceDay(userDate, srcText, "%%USERSDAY%%");
            destText = ReplaceDay(userDate, destText, "%%usersday%%", false);
            return destText;
        }

        /// <summary>DateTime.Now を基準に %%today%% / %%tomorrow%% / %%weekend%% を置換する。</summary>
        public static String GetDateText(String srcText)
        {
            DateTime today = DateTime.Now;
            String destText = ReplaceDay(today, srcText, "%%TODAY%%");
            destText = ReplaceDay(today, destText, "%%today%%", false);

            var tomorrow = today.AddDays(1);
            destText = ReplaceDay(tomorrow, destText, "%%TOMORROW%%");
            destText = ReplaceDay(tomorrow, destText, "%%tomorrow%%", false);

            DateTime friday = GetWeekendFriday(today);
            destText = ReplaceDay(friday, destText, "%%WEEKEND%%");
            destText = ReplaceDay(friday, destText, "%%weekend%%", false);
            return destText;
        }

        /// <summary>
        /// %%weekend%% の日付。次の金曜日(金曜は当日)。土・日も次の金曜日になる。
        /// 以前は「5 - 曜日」で求めていたので、土曜は前日の金曜日になっていた。
        /// </summary>
        public static DateTime GetWeekendFriday(DateTime today)
        {
            return today.AddDays(((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7);
        }

        public static String ReplaceDay(DateTime dt, String srcText, String keyName, bool includeYear = true)
        {
            String dateString = dt.Month + "/" + dt.Day;
            if (includeYear)
            {
                dateString = dt.Year + "/" + dateString;
            }

            return srcText.Replace(keyName, dateString);
        }

        /// <summary>
        /// URLのパラメータ値(to/cc/bcc/su/body)としてそのまま埋め込めるようにエスケープする。
        /// 以前は空白→"+"・改行→"%0D%0A"だけを置き換えていたため、"&"(Q&Aなど)で値が途中で切れる、
        /// "#"以降が丸ごと消える、"+"(user+tag@...)が空白になる、"%"で文字化けする、といった崩れ方をしていた。
        /// </summary>
        public static String EscapeUrlParam(String value)
        {
            // 改行("\r\n")は以前と同じ"%0D%0A"になる。
            // .NET FrameworkのUri.EscapeDataStringは約65,520文字を超えるとUriFormatExceptionになるので、区切って変換する
            // (長すぎてブラウザに渡せないかどうかは、URL全体の長さで IsUrlTooLong が判定する)
            if (value.Length <= _escapeChunkLength)
            {
                return Uri.EscapeDataString(value);
            }

            var escaped = new StringBuilder();
            for (int start = 0; start < value.Length;)
            {
                int length = Math.Min(_escapeChunkLength, value.Length - start);
                // サロゲートペアの途中で区切らない
                if (start + length < value.Length && Char.IsHighSurrogate(value[start + length - 1]))
                {
                    length--;
                }
                escaped.Append(Uri.EscapeDataString(value.Substring(start, length)));
                start += length;
            }
            return escaped.ToString();
        }

        private const int _escapeChunkLength = 30000;

        /// <summary>
        /// ブラウザに渡せるURLの長さの上限。ブラウザはコマンドライン引数で起動するので、
        /// コマンドライン全体の上限(32,767文字)からブラウザのパスの分を残した長さにする。
        /// </summary>
        public const int MaxUrlLength = 32000;

        /// <summary>URLが長すぎてブラウザに渡せないか(本文が長すぎるときなど)。</summary>
        public static Boolean IsUrlTooLong(String url)
        {
            return url.Length > MaxUrlLength;
        }

        /// <summary>メール作成する日数分のオフセット一覧を作る。reverse指定で降順にする。</summary>
        public static List<int> GetLoopList(int createNum, bool reverse)
        {
            var dayOffsets = Enumerable.Range(0, Math.Max(0, createNum)).ToList();
            if (reverse)
            {
                dayOffsets.Reverse();
            }
            return dayOffsets;
        }
    }
}
