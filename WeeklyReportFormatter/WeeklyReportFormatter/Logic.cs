using System;

namespace WeeklyReportFormatter
{
    /// <summary>
    /// もともと Form1.cs の button_ThisWeekChange_Click / button_NextWeekChange_Click /
    /// button_PerforceChange_Click に埋め込まれていた、週報のテキスト整形ロジックを
    /// テストできる形に切り出したもの。コードはそのまま移しただけで書き換えていない。
    /// textBox_UserName.Text などのコントロール参照は、呼び出し元(Form1)で読み取った
    /// 値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        private const int NextWeekLinesPerItem = 3;
        private const int PerforceLinesPerItem = 2;

        public static String FormatThisWeek(String beforeText, String userName)
        {
            String result = "";

            String[] lines = beforeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                String line = lines[i];
                line = line.TrimEnd();
                line = line.Replace("\t", "");                        // タブ ⇒ スペース
                line = "\t" + line;                                   // 先頭にタブ挿入
                line = line.Replace(userName + " ", "(") + ")";       // ユーザー名削除
                line = line.Replace(".0)", ")");                      // ストーリーポイントの".0"が邪魔
                line += Environment.NewLine;                             // 終端に改行挿入

                result += line;
            }

            return result;
        }

        public static String FormatNextWeek(String beforeText, String userName)
        {
            String result = "";

            String[] lines = beforeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                String line = lines[i];
                line = line.TrimEnd();

                switch (i % NextWeekLinesPerItem)
                {
                    case 0:
                        line = "\t" + line;   // 先頭にタブ挿入
                        break;

                    case 1:
                        line = " " + line;   // 課題Noと課題名の間にスペース
                        break;

                    case 2:
                        int nameIndex = line.IndexOf(userName);   // ユーザー名の先頭
                        nameIndex += userName.Length;                // ユーザー名の終端
                        line = " (" + line.Substring(nameIndex) + ")" + Environment.NewLine;  // ストーリーポイント
                        break;

                    default:
                        break;
                }

                result += line;
            }

            return result;
        }

        public static String FormatPerforce(String beforeText)
        {
            String[] lines = beforeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            String result = "";
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimStart();
                lines[i] = lines[i].TrimEnd();

                switch (i % PerforceLinesPerItem)
                {
                    case 0:
                        result += lines[i] + " ";     // ProjectID
                        break;

                    case 1:
                        result += lines[i];           // Summary
                        break;

                    default:
                        break;
                }
            }

            return result;
        }
    }
}
