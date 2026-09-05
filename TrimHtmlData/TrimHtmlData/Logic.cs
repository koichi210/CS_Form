using System;
using StandardTemplate;

namespace TrimHtmlData
{
    /// <summary>
    /// もともと Form1.cs の GetTrimLine / GetSearchString に実装されていた、
    /// HTMLソースから検索ワードにヒットする行(とその前後指定行数)を抽出するロジックを
    /// テストできる形に切り出したもの。コードはそのまま移しただけで書き換えていない。
    /// checkBox_FirstWordOnly.Checked などのコントロール参照は、呼び出し元(Form1)で
    /// 読み取った値を引数として渡す形に変えた。
    ///
    /// ⚠️ GetSearchString には元の実装由来の潜在バグがそのまま残っている: TrimLineNum が
    /// 2以上かつヒット行が末尾付近だと StringArray[i + j] が配列範囲外になり得る
    /// (IndexOutOfRangeException)。これは既存の挙動なので直さず、テストでは
    /// この状況を引き起こさない入力を使う。
    /// </summary>
    internal static class Logic
    {
        public static int GetTrimLine(String trimLineNumText)
        {
            StcUtils util = new StcUtils();
            int TrimLineNum = util.GetInteger(trimLineNumText);
            if (TrimLineNum == 0)
            {
                TrimLineNum = 1;
            }

            return TrimLineNum;
        }

        public static String GetSearchString(String SourceList, String SearchWord, int TrimLineNum, StringComparison CmpOpt, Boolean firstWordOnly)
        {
            String[] StringArray = SourceList.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            String ResultString = "";

            for (int i = 0; i < StringArray.Length; i++)
            {
                if (StringArray[i].IndexOf(SearchWord, CmpOpt) != -1)
                {
                    // Hitした行を含む指定行数分取得
                    for (int j = 0; j < TrimLineNum; j++)
                    {
                        ResultString += StringArray[i + j] + Environment.NewLine;
                    }

                    // 最初に見つかったワードのみ
                    if (firstWordOnly)
                    {
                        break;
                    }

                    // 次のワードとの境界
                    ResultString += Environment.NewLine;
                }
            }

            return ResultString + Environment.NewLine;
        }
    }
}
