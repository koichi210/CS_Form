using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace ToyingData
{
    /// <summary>
    /// もともと Form1.cs に private メソッドとして埋め込まれていた、全角→半角変換の
    /// ロジックを、テストできる形に切り出したもの。
    ///
    /// TryGetRegexPattern(旧 GetRegesStr) は4つのチェックボックスの状態を直接参照していたのを、bool引数に
    /// 置き換えた。MessageBox を出す判断（対象が1つも選ばれていない）は Form1 側に残し、
    /// ここには含めていない。
    /// </summary>
    internal static class Logic
    {
        /// <summary>選ばれた変換対象から、正規表現の文字クラスを組み立てる。1つも選ばれていなければfalse。</summary>
        public static Boolean TryGetRegexPattern(Boolean number, Boolean alphaLarge, Boolean alphaSmall, Boolean space, out String regexPattern)
        {
            Boolean isSuccess = false;

            regexPattern = "[";
            if (number)
            {
                regexPattern += "０-９";
                isSuccess = true;
            }

            if (alphaLarge)
            {
                regexPattern += "Ａ-Ｚ";
                isSuccess = true;
            }

            if (alphaSmall)
            {
                regexPattern += "ａ-ｚ";
                isSuccess = true;
            }

            if (space)
            {
                regexPattern += "　";
                isSuccess = true;
            }
            regexPattern += "]";

            return isSuccess;
        }

        /// <summary>指定した正規表現の文字クラスに一致する文字を、全角→半角へ変換する。</summary>
        public static String[] ApplyWide2Narrow(String[] lines, String regexPattern)
        {
            Regex re = new Regex(regexPattern);
            return lines.Select(str => re.Replace(str, ToNarrow)).ToArray();
        }

        private static String ToNarrow(Match m)
        {
            // Memo: 参照設定に「Microsoft.VisualBasic」が必要
            return Strings.StrConv(m.Value, VbStrConv.Narrow);
        }
    }
}
