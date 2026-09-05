using System;
using StandardTemplate;

namespace TrimFileData
{
    /// <summary>
    /// もともと Form1.cs の GetSearchData / GetHitWord に実装されていた、
    /// 検索ワードリストとリファレンスデータから該当行を抽出するロジックを
    /// テストできる形に切り出したもの。コードはそのまま移しただけで書き換えていない。
    /// Form のコントロール参照(checkBox_OrdinalCase.Checked 等)は、呼び出し元
    /// (Form1)で読み取った値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static String GetSearchData(String[] SourceArray, String[] ReferList, Boolean ordinalCase, Boolean firstWordOnly, String searchCommonWord)
        {
            StringComparison CmpOpt = ordinalCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            StcUtils util = new StcUtils();

            String Result = "";
            for (int i = 0; i < SourceArray.Length; i++)
            {
                Result += "◆" + SourceArray[i] + Environment.NewLine;

                String[] SourceList = SourceArray[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                String Candidate = GetHitWord(SourceList, ReferList, CmpOpt, firstWordOnly, searchCommonWord);
                Result += util.TrimDuplication(Candidate, Environment.NewLine);
                Result += Environment.NewLine + Environment.NewLine;
            }

            return Result;
        }

        public static String GetHitWord(String[] SourceList, String[] ReferList, StringComparison CmpOpt, Boolean firstWordOnly, String searchCommonWord)
        {
            String Result = "";

            for (int j = 0; j < SourceList.Length; j++)
            {
                for (int k = 0; k < ReferList.Length; k++)
                {
                    if (searchCommonWord != "" &&
                        ReferList[k].IndexOf(searchCommonWord, CmpOpt) == -1)
                    {
                        continue;
                    }

                    if (ReferList[k].IndexOf(SourceList[j], CmpOpt) != -1)
                    {
                        //Fileから抽出
                        Result += ReferList[k] + Environment.NewLine;

                        // 最初に見つかった項目のみ抽出
                        if (firstWordOnly)
                        {
                            break;
                        }
                    }
                }
            }

            return Result;
        }
    }
}
