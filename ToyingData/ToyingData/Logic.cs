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
    /// GetRegesStr は4つのチェックボックスの状態を直接参照していたのを、bool引数に
    /// 置き換えた。MessageBox を出す判断（対象が1つも選ばれていない）は Form1 側に残し、
    /// ここには含めていない。
    /// </summary>
    internal static class Logic
    {
        /// <summary>選ばれた変換対象から、正規表現の文字クラスを組み立てる。1つも選ばれていなければfalse。</summary>
        public static Boolean GetRegesStr(Boolean number, Boolean alphaLarge, Boolean alphaSmall, Boolean space, out String RegesStr)
        {
            Boolean IsSuccess = false;

            RegesStr = "[";
            if (number)
            {
                RegesStr += "０-９";
                IsSuccess = true;
            }

            if (alphaLarge)
            {
                RegesStr += "Ａ-Ｚ";
                IsSuccess = true;
            }

            if (alphaSmall)
            {
                RegesStr += "ａ-ｚ";
                IsSuccess = true;
            }

            if (space)
            {
                RegesStr += "　";
                IsSuccess = true;
            }
            RegesStr += "]";

            return IsSuccess;
        }

        /// <summary>指定した正規表現の文字クラスに一致する文字を、全角→半角へ変換する。</summary>
        public static String[] ApplyWide2Narrow(String[] StrArray, String RegesStr)
        {
            Regex re = new Regex(RegesStr);
            return StrArray.Select(str => re.Replace(str, myReplacer)).ToArray();
        }

        private static String myReplacer(Match m)
        {
            // Memo: 参照設定に「Microsoft.VisualBasic」が必要
            return Strings.StrConv(m.Value, VbStrConv.Narrow);
        }
    }
}
