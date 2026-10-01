using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace FileArranger
{
    /// <summary>
    /// もともと Form1.cs のイベントハンドラの隣に private メソッドとして埋め込まれていた
    /// 純粋なロジックを、テストできる形に切り出したもの。
    ///
    /// コードは Form1.cs にあったものをそのまま移しただけで、中身の書き換えはしていない。
    /// 呼び出し側（Form1.cs）も、このクラスのメソッドを呼ぶよう書き換えただけで、
    /// 渡す値・受け取る値・呼ぶ順序は変えていない。
    /// </summary>
    internal static class Logic
    {
        /// <summary>
        /// ファイル名の連番部分にゼロ埋めが必要な桁数を返す。
        /// 例えば連番が1桁・2桁のときは2桁（"01","02"..."09"）にそろえる。
        /// </summary>
        public static int GetPaddingDigits(long number, Boolean isZeroDigitForZero = false)
        {
            const int PaddingMinDigits = 2;
            int paddingDigits = 0;

            if (isZeroDigitForZero && number == 0)
            {
                // 数値が「0」のときは、桁数も「0」とする
            }
            else if (number.ToString().Length <= PaddingMinDigits)
            {
                paddingDigits = PaddingMinDigits;
            }
            return paddingDigits;
        }

        /// <summary>連番に加算数を足し、必要な桁数までゼロ埋めした文字列にする。</summary>
        public static String ToPaddedNumberString(long srcNumber, int addCount = 0)
        {
            long destNumber = srcNumber + addCount;

            int paddingDigits = GetPaddingDigits(destNumber);
            return destNumber.ToString().PadLeft(paddingDigits, '0');
        }

        /// <summary>全角の数字・英字・スペースを半角に変換する。</summary>
        public static String ChangeWide2Narrow(String srcString)
        {
            Regex re = new Regex("[０-９Ａ-Ｚａ-ｚ　]");
            return re.Replace(srcString, ToNarrow);
        }

        private static String ToNarrow(Match m)
        {
            // Memo: 参照設定に「Microsoft.VisualBasic」が必要
            return Strings.StrConv(m.Value, VbStrConv.Narrow);
        }

        /// <summary>
        /// リストの選択項目の中から、区切り文字より前の部分が一致するものを数える。
        /// 一致が無ければ、新規追加時の初期値として 1 を返す。
        /// </summary>
        public static int GetAddCount(ListView lv, String fileName, String trimName, Boolean isReverse = false)
        {
            const int TargetSubItemIdx = 0;

            int count = 0;
            String searchName = "";

            int trimIdx = isReverse ? fileName.LastIndexOf(trimName) : fileName.IndexOf(trimName);
            if (0 <= trimIdx)
            {
                searchName = fileName.Substring(0, trimIdx);
            }

            for (int i = 0; i < lv.SelectedItems.Count; i++)
            {
                int idx = lv.SelectedItems[i].Index;
                String srcFileName = lv.Items[idx].SubItems[TargetSubItemIdx].Text;

                if (srcFileName.IndexOf(searchName) != -1)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                // 今回新規追加時の初期値
                count = 1;
            }
            return count;
        }

        /// <summary>
        /// 新しく追加された項目のうち、既存の一覧に既に含まれているものを取り除く。
        ///
        /// ⚠️ delimiter 引数は元の実装から使われていなかった（呼び出し側は値を渡しているが
        /// 中では参照されていない）。挙動を変えないため、そのまま残してある。
        /// </summary>
        public static void DeleteDuplicate(String[] existingArray, ref String[] newArray, String delimiter)
        {
            // 以前は走査中のリストから自分自身の要素を削除しながらインデックスを
            // 巻き戻す(i--)という紛らわしい書き方をしていた。走査対象(newArray)と
            // 結果(result)を分けることでインデックス操作を無くした(挙動は変えていない)。
            var result = new List<String>();
            foreach (String newItem in newArray)
            {
                Boolean isDuplicate = false;
                foreach (String existingItem in existingArray)
                {
                    if (existingItem.IndexOf(newItem) != -1)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    result.Add(newItem);
                }
            }

            newArray = result.ToArray();
        }
    }
}
