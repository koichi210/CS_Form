using System;
using System.Text;

namespace StrCompare
{
    /// <summary>
    /// もともと Form1.cs の Compare / SampleCompare に実装されていた、文字列比較の
    /// 挙動を確認するロジックをテストできる形に切り出したもの。MessageBox.Showは
    /// 呼び出し元(Form1)に残し、結果文字列を組み立てて返す部分だけを切り出した。
    /// </summary>
    internal static class Logic
    {
        private const string _caseSensitiveExactLabel = "大文字小文字区別する（完全一致）";
        private const string _ignoreCaseExactLabel = "大文字小文字区別しない（完全一致）";
        private const string _ignoreCasePrefixLabel = "大文字小文字区別しない（前方一致）";

        public static string Compare(string source, string target)
        {
            StringBuilder result = new StringBuilder();

            // 大文字・小文字は区別される（完全一致）
            AppendResult(result, _caseSensitiveExactLabel, source.Equals(target));

            // 大文字・小文字を区別しない（それ以外は完全一致）
            AppendResult(result, _ignoreCaseExactLabel, source.Equals(target, StringComparison.OrdinalIgnoreCase));

            // 大文字・小文字を区別しない（前方一致で比較）
            AppendResult(result, _ignoreCasePrefixLabel, source.StartsWith(target, StringComparison.OrdinalIgnoreCase));

            return result.ToString();
        }

        public static string SampleCompare()
        {
            const string source = "sampleString";
            StringBuilder result = new StringBuilder();

            // 大文字・小文字は区別される（完全一致）
            AppendSampleResult(result, source, "sampleString", _caseSensitiveExactLabel, source.Equals("sampleString"));

            // 大文字・小文字は区別される（完全一致）
            AppendSampleResult(result, source, "sampleSTRING", _caseSensitiveExactLabel, source.Equals("sampleSTRING"));

            // 大文字・小文字を区別しない（それ以外は完全一致）
            AppendSampleResult(result, source, "sampleSTRING", _ignoreCaseExactLabel, source.Equals("sampleSTRING", StringComparison.OrdinalIgnoreCase));

            // 大文字・小文字を区別しない（前方一致で比較）
            AppendSampleResult(result, source, "SAMPLE", _ignoreCasePrefixLabel, source.StartsWith("SAMPLE", StringComparison.OrdinalIgnoreCase));

            return result.ToString();
        }

        private static void AppendResult(StringBuilder result, string label, bool isMatch)
        {
            result.Append(label).Append(" =").Append(isMatch).AppendLine();
        }

        private static void AppendSampleResult(StringBuilder result, string source, string target, string label, bool isMatch)
        {
            result.Append('[').Append(source).Append("][").Append(target).Append(']').AppendLine();
            AppendResult(result, label, isMatch);
            result.AppendLine();
        }
    }
}
