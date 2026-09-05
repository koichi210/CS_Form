using System;

namespace DigitalClock
{
    /// <summary>
    /// もともと Form1.cs の UpdateTime に埋め込まれていた、時刻を "HH:mm:ss" 形式の
    /// 文字列に整形するロジックをテストできる形に切り出したもの。コードはそのまま
    /// 移しただけで書き換えていない。DateTime.Now の取得(現在時刻依存)は Form1 側に
    /// 残し、ここには DateTime を受け取って文字列に変換する部分だけを渡した。
    /// </summary>
    internal static class Logic
    {
        public static String FormatTime(DateTime d)
        {
            return String.Format("{0:00}:{1:00}:{2:00}", d.Hour, d.Minute, d.Second);
        }
    }
}
