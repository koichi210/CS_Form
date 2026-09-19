using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ToyingFile
{
    /// <summary>
    /// もともと Form1.cs の FunctionDeleteString に埋め込まれていた、ファイル内容から
    /// 指定文字列を含む行を処理するロジックを、テストできる形に切り出したもの。
    ///
    /// コードはそのまま移しただけで書き換えていない。ファイルの読み書き(fio.LoadFile/
    /// SaveFile)は Form1 側に残し、ここには文字列だけを渡す・返す形にした。
    ///
    /// exactMatch(大文字小文字を区別するか)は、以前は「削除対象の行かどうかを判定する IndexOf」
    /// にしか効いておらず、実際に削除する String.Replace は常に大文字小文字を区別していた
    /// (.NET Framework の String.Replace に大文字小文字を無視するオーバーロードが無いため)。
    /// 区別しない場合は Regex.Replace を使うことで、判定と削除の挙動を揃えている。
    /// </summary>
    internal static class Logic
    {
        /// <summary>
        /// ファイル内容(改行区切り)から、DeleteArray のいずれかを含む行を処理する。
        /// deleteWholeLine=true なら行ごと空行にする。false なら該当文字列だけ削除する。
        /// </summary>
        public static String DeleteStringFromContent(String FileData, String[] DeleteArray, Boolean exactMatch, Boolean deleteWholeLine)
        {
            StringComparison CmpOpt = exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            String[] FileDataArray = FileData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var list = new List<String>();
            list.AddRange(FileDataArray);

            for (int j = 0; j < list.Count; j++)
            {
                if (list[j] == String.Empty)
                {
                    continue;
                }

                for (int k = 0; k < DeleteArray.Length; k++)
                {
                    //削除対象の行か判別
                    if (list[j].IndexOf(DeleteArray[k], CmpOpt) != -1)
                    {
                        if (deleteWholeLine)
                        {
                            // 一行削除&空行追加
                            list[j] = "";
                        }
                        else if (exactMatch)
                        {
                            // 文字だけ削除ならReplace
                            list[j] = list[j].Replace(DeleteArray[k], "");
                        }
                        else
                        {
                            list[j] = Regex.Replace(list[j], Regex.Escape(DeleteArray[k]), "", RegexOptions.IgnoreCase);
                        }
                    }
                }
            }

            return String.Join(Environment.NewLine, list.ToArray());
        }
    }
}
