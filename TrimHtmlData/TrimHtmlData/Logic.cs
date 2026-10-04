using System;
using System.Text;
using StandardTemplate;

namespace TrimHtmlData
{
    /// <summary>
    /// もともと Form1.cs の GetTrimLine / GetSearchString に実装されていた、
    /// HTMLソースから検索ワードにヒットする行(とその前後指定行数)を抽出するロジックを
    /// テストできる形に切り出したもの。checkBox_FirstWordOnly.Checked などのコントロール参照は、呼び出し元(Form1)で
    /// 読み取った値を引数として渡す形に変えた。
    ///
    /// trimLineNum が2以上でヒット行が末尾付近だと lines[i + j] が配列範囲外になっていたが、
    /// 末尾でクランプするよう修正済み(取得できる行数が足りない場合は、ある分だけ返す)。
    /// </summary>
    internal static class Logic
    {
        public static int GetTrimLine(string trimLineNumText)
        {
            StcUtils util = new StcUtils();
            int trimLineNum = util.GetInteger(trimLineNumText);
            if (trimLineNum == 0)
            {
                trimLineNum = 1;
            }

            return trimLineNum;
        }

        public static string GetSearchString(string htmlSource, string searchWord, int trimLineNum, StringComparison comparison, bool firstWordOnly)
        {
            string[] lines = htmlSource.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // String +=はヒット行数が多いほど文字列全体のコピーが積み上がって遅くなるため、
            // StringBuilderに置き換える(ロジック・境界チェックの挙動は変えていない)。
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf(searchWord, comparison) != -1)
                {
                    // Hitした行を含む指定行数分取得(末尾を超えないようクランプする)
                    for (int j = 0; j < trimLineNum && (i + j) < lines.Length; j++)
                    {
                        result.Append(lines[i + j]).Append(Environment.NewLine);
                    }

                    // 最初に見つかったワードのみ
                    if (firstWordOnly)
                    {
                        break;
                    }

                    // 次のワードとの境界
                    result.Append(Environment.NewLine);
                }
            }

            return result.Append(Environment.NewLine).ToString();
        }
    }
}
