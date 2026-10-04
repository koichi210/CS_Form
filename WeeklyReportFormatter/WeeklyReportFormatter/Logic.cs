using System;
using System.Text;

namespace WeeklyReportFormatter
{
    /// <summary>
    /// もともと Form1.cs の button_ThisWeekChange_Click / button_NextWeekChange_Click /
    /// button_PerforceChange_Click に埋め込まれていた、週報のテキスト整形ロジックを
    /// テストできる形に切り出したもの。
    /// textBox_UserName.Text などのコントロール参照は、呼び出し元(Form1)で読み取った
    /// 値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        private const int _nextWeekLinesPerItem = 3;
        private const int _perforceLinesPerItem = 2;

        public static string FormatThisWeek(string beforeText, string userName)
        {
            // String +=は行数が多いほど文字列全体のコピーが積み上がるため、StringBuilderに溜める
            StringBuilder result = new StringBuilder();

            foreach (string rawLine in SplitLines(beforeText))
            {
                string line = rawLine.TrimEnd();
                line = line.Replace("\t", "");                        // タブ削除
                line = "\t" + line;                                   // 先頭にタブ挿入
                line = line.Replace(userName + " ", "(") + ")";       // ユーザー名削除
                line = line.Replace(".0)", ")");                      // ストーリーポイントの".0"が邪魔

                result.Append(line).Append(Environment.NewLine);      // 終端に改行挿入
            }

            return result.ToString();
        }

        public static string FormatNextWeek(string beforeText, string userName)
        {
            StringBuilder result = new StringBuilder();

            string[] lines = SplitLines(beforeText);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd();

                switch (i % _nextWeekLinesPerItem)
                {
                    case 0:
                        result.Append('\t').Append(line);   // 先頭にタブ挿入
                        break;

                    case 1:
                        result.Append(' ').Append(line);    // 課題Noと課題名の間にスペース
                        break;

                    case 2:
                        int nameEndIndex = line.IndexOf(userName) + userName.Length;   // ユーザー名の終端
                        result.Append(" (").Append(line.Substring(nameEndIndex)).Append(')').Append(Environment.NewLine);  // ストーリーポイント
                        break;
                }
            }

            return result.ToString();
        }

        public static string FormatPerforce(string beforeText)
        {
            StringBuilder result = new StringBuilder();

            string[] lines = SplitLines(beforeText);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (i % _perforceLinesPerItem == 0)
                {
                    result.Append(line).Append(' ');     // ProjectID
                }
                else
                {
                    result.Append(line);                 // Summary
                }
            }

            return result.ToString();
        }

        private static string[] SplitLines(string text)
        {
            return text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
