using System;

namespace StrCompare
{
    /// <summary>
    /// もともと Form1.cs の Compare / SampleCompare に実装されていた、文字列比較の
    /// 挙動を確認するロジックをテストできる形に切り出したもの。コードはそのまま
    /// 移しただけで書き換えていない。MessageBox.Showは呼び出し元(Form1)に残し、
    /// 結果文字列を組み立てて返す部分だけを切り出した。
    /// </summary>
    internal static class Logic
    {
        public static String Compare(String source, String target)
        {
            String result = "";

            // 大文字・小文字は区別される（完全一致）
            Boolean isMatch = source.Equals(target);
            result += "大文字小文字区別する（完全一致） =" + isMatch.ToString() + Environment.NewLine;

            // 大文字・小文字を区別しない（それ以外は完全一致）
            isMatch = source.Equals(target, StringComparison.OrdinalIgnoreCase);
            result += "大文字小文字区別しない（完全一致） =" + isMatch.ToString() + Environment.NewLine;

            // 大文字・小文字を区別しない（前方一致で比較）
            isMatch = source.StartsWith(target, StringComparison.OrdinalIgnoreCase);
            result += "大文字小文字区別しない（前方一致） =" + isMatch.ToString() + Environment.NewLine;

            return result;
        }

        public static String SampleCompare()
        {
            String source = "sampleString";
            String result = "";

            // 大文字・小文字は区別される（完全一致）
            Boolean isMatch = source.Equals("sampleString");
            result += "[" + source + "][" + "sampleString" + "]" + Environment.NewLine;
            result += "大文字小文字区別する（完全一致） =" + isMatch.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字は区別される（完全一致）
            isMatch = source.Equals("sampleSTRING");
            result += "[" + source + "][" + "sampleSTRING" + "]" + Environment.NewLine;
            result += "大文字小文字区別する（完全一致） =" + isMatch.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字を区別しない（それ以外は完全一致）
            isMatch = source.Equals("sampleSTRING", StringComparison.OrdinalIgnoreCase);
            result += "[" + source + "][" + "sampleSTRING" + "]" + Environment.NewLine;
            result += "大文字小文字区別しない（完全一致） =" + isMatch.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字を区別しない（前方一致で比較）
            isMatch = source.StartsWith("SAMPLE", StringComparison.OrdinalIgnoreCase);
            result += "[" + source + "][" + "SAMPLE" + "]" + Environment.NewLine;
            result += "大文字小文字区別しない（前方一致） =" + isMatch.ToString() + Environment.NewLine + Environment.NewLine;

            return result;
        }
    }
}
