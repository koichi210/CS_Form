using System;
using System.Collections.Generic;
using System.Text;
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

            // String +=はループの度に文字列全体をコピーし直すため、行数が多いと遅くなる。
            // StringBuilderに溜めてから最後に1回だけToString()する。
            StringBuilder ResultBuilder = new StringBuilder();
            for (int i = 0; i < SourceArray.Length; i++)
            {
                ResultBuilder.Append("◆").Append(SourceArray[i]).Append(Environment.NewLine);

                String[] SourceList = SourceArray[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                String Candidate = GetHitWord(SourceList, ReferList, CmpOpt, firstWordOnly, searchCommonWord);
                ResultBuilder.Append(util.TrimDuplication(Candidate, Environment.NewLine));
                ResultBuilder.Append(Environment.NewLine).Append(Environment.NewLine);
            }

            return ResultBuilder.ToString();
        }

        public static String GetHitWord(String[] SourceList, String[] ReferList, StringComparison CmpOpt, Boolean firstWordOnly, String searchCommonWord)
        {
            // searchCommonWordによる絞り込みはSourceListのどの単語(j)でも結果が変わらないため、
            // 以前は単語数(j)×参照行数(k)回、毎回同じIndexOf判定を繰り返していた。
            // 単語ループに入る前に1回だけReferListを絞り込んでおけば、絞り込み自体はO(k)で済む。
            Boolean HasCommonWord = searchCommonWord != "";
            List<String> FilteredRefer = new List<String>(ReferList.Length);
            foreach (String line in ReferList)
            {
                if (!HasCommonWord || line.IndexOf(searchCommonWord, CmpOpt) != -1)
                {
                    FilteredRefer.Add(line);
                }
            }

            StringBuilder ResultBuilder = new StringBuilder();

            for (int j = 0; j < SourceList.Length; j++)
            {
                for (int k = 0; k < FilteredRefer.Count; k++)
                {
                    if (FilteredRefer[k].IndexOf(SourceList[j], CmpOpt) != -1)
                    {
                        //Fileから抽出
                        ResultBuilder.Append(FilteredRefer[k]).Append(Environment.NewLine);

                        // 最初に見つかった項目のみ抽出
                        if (firstWordOnly)
                        {
                            break;
                        }
                    }
                }
            }

            return ResultBuilder.ToString();
        }
    }
}
