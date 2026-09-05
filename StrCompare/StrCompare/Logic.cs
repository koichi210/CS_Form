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
        public static String Compare(String Source, String Target)
        {
            String ResultStr = "";

            // 大文字・小文字は区別される（完全一致）
            Boolean ret = Source.Equals(Target);
            ResultStr += "大文字小文字区別する（完全一致） =" + ret.ToString() + Environment.NewLine;

            // 大文字・小文字を区別しない（それ以外は完全一致）
            ret = Source.Equals(Target, StringComparison.OrdinalIgnoreCase);
            ResultStr += "大文字小文字区別しない（完全一致） =" + ret.ToString() + Environment.NewLine;

            // 大文字・小文字を区別しない（前方一致で比較）
            ret = Source.StartsWith(Target, StringComparison.OrdinalIgnoreCase);
            ResultStr += "大文字小文字区別しない（前方一致） =" + ret.ToString() + Environment.NewLine;

            return ResultStr;
        }

        public static String SampleCompare()
        {
            String Source = "sampleString";
            String ResultStr = "";

            // 大文字・小文字は区別される（完全一致）
            Boolean ret = Source.Equals("sampleString");
            ResultStr += "[" + Source + "][" + "sampleString" + "]" + Environment.NewLine;
            ResultStr += "大文字小文字区別する（完全一致） =" + ret.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字は区別される（完全一致）
            ret = Source.Equals("sampleSTRING");
            ResultStr += "[" + Source + "][" + "sampleSTRING" + "]" + Environment.NewLine;
            ResultStr += "大文字小文字区別する（完全一致） =" + ret.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字を区別しない（それ以外は完全一致）
            ret = Source.Equals("sampleSTRING", StringComparison.OrdinalIgnoreCase);
            ResultStr += "[" + Source + "][" + "sampleSTRING" + "]" + Environment.NewLine;
            ResultStr += "大文字小文字区別しない（完全一致） =" + ret.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字を区別しない（前方一致で比較）
            ret = Source.StartsWith("SAMPLE", StringComparison.OrdinalIgnoreCase);
            ResultStr += "[" + Source + "][" + "SAMPLE" + "]" + Environment.NewLine;
            ResultStr += "大文字小文字区別しない（前方一致） =" + ret.ToString() + Environment.NewLine + Environment.NewLine;

            return ResultStr;
        }
    }
}
