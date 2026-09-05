using System;
using System.Collections.Generic;

namespace ToyingFile
{
    /// <summary>
    /// もともと Form1.cs の FunctionDeleteString に埋め込まれていた、ファイル内容から
    /// 指定文字列を含む行を処理するロジックを、テストできる形に切り出したもの。
    ///
    /// コードはそのまま移しただけで書き換えていない。ファイルの読み書き(fio.LoadFile/
    /// SaveFile)は Form1 側に残し、ここには文字列だけを渡す・返す形にした。
    ///
    /// ⚠️ 元の実装の仕様として、exactMatch(大文字小文字区別)は「削除対象の行かどうかを
    /// 判定する IndexOf」にしか効いていない。実際に削除・置換する String.Replace は
    /// 常に大文字小文字を区別する（.NET Framework の String.Replace(string,string) に
    /// 大文字小文字を無視するオーバーロードが無いため）。この非対称な挙動もそのまま残した。
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
                            list.RemoveAt(j);
                            list.Insert(j, "");
                        }
                        else
                        {
                            // 文字だけ削除ならReplace
                            list[j] = list[j].Replace(DeleteArray[k], "");
                        }
                    }
                }
            }

            return String.Join(Environment.NewLine, list.ToArray());
        }
    }
}
