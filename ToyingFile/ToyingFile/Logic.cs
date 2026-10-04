using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToyingFile
{
    /// <summary>
    /// もともと Form1.cs の FunctionDeleteString(現 DeleteStringFromFiles) に埋め込まれていた、ファイル内容から
    /// 指定文字列を含む行を処理するロジックを、テストできる形に切り出したもの。
    ///
    /// ファイルの読み書き(fio.LoadFile/
    /// SaveFile)は Form1 側に残し、ここには文字列だけを渡す・返す形にした。
    ///
    /// caseSensitive(大文字小文字を区別するか)は、以前は「削除対象の行かどうかを判定する IndexOf」
    /// にしか効いておらず、実際に削除する String.Replace は常に大文字小文字を区別していた
    /// (.NET Framework の String.Replace に大文字小文字を無視するオーバーロードが無いため)。
    /// 区別しない場合は Regex.Replace を使うことで、判定と削除の挙動を揃えている。
    /// </summary>
    internal static class Logic
    {
        /// <summary>
        /// ファイル内容(改行区切り)から、deleteStrings のいずれかを含む行を処理する。
        /// deleteWholeLine=true なら行ごと空行にする。false なら該当文字列だけ削除する。
        /// </summary>
        public static string DeleteStringFromContent(string fileData, string[] deleteStrings, bool caseSensitive, bool deleteWholeLine)
        {
            StringComparison comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            // 大文字小文字を区別しない削除用の正規表現は、行ごとに作り直さず最初に1回だけ作る
            Regex[] ignoreCaseRegexes = (caseSensitive || deleteWholeLine)
                ? null
                : deleteStrings.Select(s => new Regex(Regex.Escape(s), RegexOptions.IgnoreCase)).ToArray();

            string[] lines = fileData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int j = 0; j < lines.Length; j++)
            {
                for (int k = 0; k < deleteStrings.Length && lines[j] != string.Empty; k++)
                {
                    //削除対象の行か判別
                    if (lines[j].IndexOf(deleteStrings[k], comparison) == -1)
                    {
                        continue;
                    }

                    if (deleteWholeLine)
                    {
                        // 一行削除&空行追加
                        lines[j] = "";
                    }
                    else if (caseSensitive)
                    {
                        // 文字だけ削除ならReplace
                        lines[j] = lines[j].Replace(deleteStrings[k], "");
                    }
                    else
                    {
                        lines[j] = ignoreCaseRegexes[k].Replace(lines[j], "");
                    }
                }
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
