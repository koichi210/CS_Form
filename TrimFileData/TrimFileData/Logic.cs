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
        public static String GetSearchData(String[] searchWordLines, String[] referLines, Boolean ordinalCase, Boolean firstWordOnly, String searchCommonWord)
        {
            StringComparison comparison = ordinalCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            StcUtils util = new StcUtils();

            // String +=はループの度に文字列全体をコピーし直すため、行数が多いと遅くなる。
            // StringBuilderに溜めてから最後に1回だけToString()する。
            StringBuilder resultBuilder = new StringBuilder();
            for (int i = 0; i < searchWordLines.Length; i++)
            {
                resultBuilder.Append("◆").Append(searchWordLines[i]).Append(Environment.NewLine);

                String[] searchWords = searchWordLines[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                String hitLines = GetHitLines(searchWords, referLines, comparison, firstWordOnly, searchCommonWord);
                resultBuilder.Append(util.TrimDuplication(hitLines, Environment.NewLine));
                resultBuilder.Append(Environment.NewLine).Append(Environment.NewLine);
            }

            return resultBuilder.ToString();
        }

        public static String GetHitLines(String[] searchWords, String[] referLines, StringComparison comparison, Boolean firstWordOnly, String searchCommonWord)
        {
            // searchCommonWordによる絞り込みはsearchWordsのどの単語(j)でも結果が変わらないため、
            // 以前は単語数(j)×参照行数(k)回、毎回同じIndexOf判定を繰り返していた。
            // 単語ループに入る前に1回だけreferLinesを絞り込んでおけば、絞り込み自体はO(k)で済む。
            Boolean hasCommonWord = searchCommonWord != "";
            List<String> filteredLines = new List<String>(referLines.Length);
            foreach (String line in referLines)
            {
                if (!hasCommonWord || line.IndexOf(searchCommonWord, comparison) != -1)
                {
                    filteredLines.Add(line);
                }
            }

            StringBuilder resultBuilder = new StringBuilder();

            for (int j = 0; j < searchWords.Length; j++)
            {
                for (int k = 0; k < filteredLines.Count; k++)
                {
                    if (filteredLines[k].IndexOf(searchWords[j], comparison) != -1)
                    {
                        //Fileから抽出
                        resultBuilder.Append(filteredLines[k]).Append(Environment.NewLine);

                        // 最初に見つかった項目のみ抽出
                        if (firstWordOnly)
                        {
                            break;
                        }
                    }
                }
            }

            return resultBuilder.ToString();
        }
    }
}
