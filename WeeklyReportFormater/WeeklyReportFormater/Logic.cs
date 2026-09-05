using System;

namespace WeeklyReportFormater
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
        private const int NextWeekSetNum = 3;
        private const int PerforceSetNum = 2;

        public static String FormatThisWeek(String BeforeText, String UserName)
        {
            String Result = "";

            String[] Line = BeforeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Line.Length; i++)
            {
                String NewLine = Line[i];
                NewLine = NewLine.TrimEnd();
                NewLine = NewLine.Replace("\t", "");                        // タブ ⇒ スペース
                NewLine = "\t" + NewLine;                                   // 先頭にタブ挿入
                NewLine = NewLine.Replace(UserName + " ", "(") + ")";       // ユーザー名削除
                NewLine = NewLine.Replace(".0)", ")");                      // ストーリーポイントの".0"が邪魔
                NewLine += Environment.NewLine;                             // 終端に改行挿入

                Result += NewLine;
            }

            return Result;
        }

        public static String FormatNextWeek(String BeforeText, String UserName)
        {
            String Result = "";

            String[] Line = BeforeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Line.Length; i++)
            {
                String NewLine = Line[i];
                NewLine = NewLine.TrimEnd();

                switch (i % NextWeekSetNum)
                {
                    case 0:
                        NewLine = "\t" + NewLine;   // 先頭にタブ挿入
                        break;

                    case 1:
                        NewLine = " " + NewLine;   // 課題Noと課題名の間にスペース
                        break;

                    case 2:
                        int NameIdx = NewLine.IndexOf(UserName);   // ユーザー名の先頭
                        NameIdx += UserName.Length;                // ユーザー名の終端
                        NewLine = " (" + NewLine.Substring(NameIdx) + ")" + Environment.NewLine;  // ストーリーポイント
                        break;

                    default:
                        break;
                }

                Result += NewLine;
            }

            return Result;
        }

        public static String FormatPerforce(String BeforeText)
        {
            String[] Line = BeforeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            String NewLine = "";
            for (int i = 0; i < Line.Length; i++)
            {
                Line[i] = Line[i].TrimStart();
                Line[i] = Line[i].TrimEnd();

                switch (i % PerforceSetNum)
                {
                    case 0:
                        NewLine += Line[i] + " ";     // ProjectID
                        break;

                    case 1:
                        NewLine += Line[i];           // Summary
                        break;

                    default:
                        break;
                }
            }

            return NewLine;
        }
    }
}
