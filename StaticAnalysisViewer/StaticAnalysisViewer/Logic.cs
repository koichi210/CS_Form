using System;
using System.Text;

namespace StaticAnalysisViewer
{
    /// <summary>
    /// もともと Form1.cs に private メソッドとして埋め込まれていた、ランキング文字列の
    /// 組み立てロジックを、テストできる形に切り出したもの。
    ///
    /// これらは Form のコントロールには一切触れておらず、Form1 の private フィールド
    /// だった DataBase(DB) だけに依存していたので、呼び出し側で明示的に渡す形にした。
    /// </summary>
    internal static class Logic
    {
        private const string _rankUp = "↑";
        private const string _rankDown = "↓";
        private const string _rankPend = "－";
        private const string _rankNew = "New!";

        private const int _categoryIdxFileName = 0;
        private const int _categoryIdxCountLine = 1;
        private const int _categoryIdxCountCode = 2;
        private const int _categoryIdxCyclomatic = 3;

        /// <summary>ファイルパスから、ランキングに表示するラベル（直上のフォルダ名）を作る。</summary>
        public static string CreateLabelName(string filePath)
        {
            var dirPath = System.IO.Path.GetDirectoryName(filePath);
            var dirArray = dirPath.Split('\\');
            return dirArray[dirArray.Length - 1];
        }

        public static int CreateCountNumTotal(DataBase db, DataBaseEntry array)
        {
            int countLineTotal = 0;
            for (int i = 0; i < array.RowNum; i++)
            {
                // Rowが短い場合はカラ行
                if (array.Data[i].Length < db.GetColumnNum())
                {
                    continue;
                }

                countLineTotal += int.Parse(array.Data[i][_categoryIdxCountLine]);
            }
            return countLineTotal;
        }

        public static string CreateRankingString(DataBase db, int preArrayIdx, DataBaseEntry array, int topRankingNum)
        {
            // ランキングのヘッダ
            var result = new StringBuilder();
            result.AppendFormat("{0,4}\t{1,8}\t{2,-15}\t{3,8}\t{4,10}  {5,8}" + Environment.NewLine + Environment.NewLine,
                                "Rank",
                                "LastWeek",
                                "FileName",
                                "CountLine",
                                "MaxCycMod",
                                "MaxCycStrict");

            int loopMax = System.Math.Min(topRankingNum, array.RowNum);
            for (int i = 0; i < loopMax; i++)
            {
                // Rowが短い場合はカラ行
                if (array.Data[i].Length < db.GetColumnNum())
                {
                    continue;
                }

                string[] path = array.Data[i][_categoryIdxFileName].Split('\\');

                int preRankNum = db.GetIdx(preArrayIdx, _categoryIdxFileName, array.Data[i][_categoryIdxFileName]);
                string preRank = CreatePreRankingString(db, i, preRankNum);

                result.AppendFormat("{0,4}\t{1,-8}\t{2,-15}\t{3,8}\t{4,10}  {5,8}" + Environment.NewLine,
                            i + 1,                                      // Idx
                            preRank,                                    // New!
                            path[path.Length - 1].Replace("\"", ""),    // FileName
                            array.Data[i][_categoryIdxCountLine],       // CountLine
                            array.Data[i][_categoryIdxCountCode],       // CountCode
                            array.Data[i][_categoryIdxCyclomatic]      // Cyclomatic
                            );
            }

            return result.ToString();
        }

        public static string CreatePreRankingString(DataBase db, int curRankNum, int preRankNum)
        {
            // 前回のランキングを取得し、ランキング変動文字列を生成
            if (preRankNum == db.UnknownIdx)
            {
                return _rankNew;
            }

            string preRankSign = preRankNum > curRankNum ? _rankUp
                               : preRankNum < curRankNum ? _rankDown
                               : _rankPend;
            return string.Format("{0}({1,2})", preRankSign, preRankNum + 1);  // 順位は1相対なので、"+1"する
        }
    }
}
