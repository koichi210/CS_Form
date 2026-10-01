using System;

namespace StaticAnalysisViewer
{
    /// <summary>
    /// もともと Form1.cs に private メソッドとして埋め込まれていた、ランキング文字列の
    /// 組み立てロジックを、テストできる形に切り出したもの。
    ///
    /// コードは元のファイルにあったものをそのまま移しただけで、中身の書き換えはしていない。
    /// これらは Form のコントロールには一切触れておらず、Form1 の private フィールド
    /// だった DataBase(DB) だけに依存していたので、呼び出し側で明示的に渡す形にした。
    /// </summary>
    internal static class Logic
    {
        private static readonly string ST_RANK_UP = "↑";
        private static readonly string ST_RANK_DOWN = "↓";
        private static readonly string ST_RANK_PEND = "－";
        private static readonly string ST_RANK_NEW = "New!";

        private static readonly int CATEGORY_IDX_FNAME = 0;
        private static readonly int CATEGORY_IDX_CNT_LINE = 1;
        private static readonly int CATEGORY_IDX_CNT_CODE = 2;
        private static readonly int CATEGORY_IDX_CYCLOMATIC = 3;

        /// <summary>ファイルパスから、ランキングに表示するラベル（直上のフォルダ名）を作る。</summary>
        public static string CreateLabelName(string filePath)
        {
            var dirPath = System.IO.Path.GetDirectoryName(filePath);
            var dirArray = dirPath.Split('\\');
            return dirArray[dirArray.Length - 1];
        }

        public static int CreateCountNumTotal(DataBase db, DataBase_T array)
        {
            int countLineTotal = 0;
            for (int i = 0; i < array.RowNum; i++)
            {
                // Rowが短い場合はカラ行
                if (array.Data[i].Length < db.GetColumnNum())
                {
                    continue;
                }

                countLineTotal += int.Parse(array.Data[i][CATEGORY_IDX_CNT_LINE]);
            }
            return countLineTotal;
        }

        public static string CreateRankingString(DataBase db, int preArrayIdx, DataBase_T array, int topRankingNum)
        {
            // ランキングのヘッダ
            string result = string.Format("{0,4}\t{1,8}\t{2,-15}\t{3,8}\t{4,10}  {5,8}" + Environment.NewLine + Environment.NewLine,
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

                string[] path = array.Data[i][CATEGORY_IDX_FNAME].Split('\\');

                int preRankNum = db.GetIdx(preArrayIdx, CATEGORY_IDX_FNAME, array.Data[i][CATEGORY_IDX_FNAME]);
                string preRank = CreatePreRankingString(db, i, preRankNum);

                result += string.Format("{0,4}\t{1,-8}\t{2,-15}\t{3,8}\t{4,10}  {5,8}" + Environment.NewLine,
                            i + 1,                                      // Idx
                            preRank,                                    // New!
                            path[path.Length - 1].Replace("\"", ""),    // FileName
                            array.Data[i][CATEGORY_IDX_CNT_LINE],       // CountLine
                            array.Data[i][CATEGORY_IDX_CNT_CODE],       // CountCode
                            array.Data[i][CATEGORY_IDX_CYCLOMATIC]      // Cyclomatic
                            );
            }

            return result;
        }

        public static string CreatePreRankingString(DataBase db, int curRankNum, int preRankNum)
        {
            // 前回のランキングを取得し、ランキング変動文字列を生成
            if (preRankNum == db.UNKNOWN_IDX)
            {
                return ST_RANK_NEW;
            }

            string preRankSign = ST_RANK_PEND;
            if (preRankNum > curRankNum)
            {
                preRankSign = ST_RANK_UP;
            }
            else if (preRankNum < curRankNum)
            {
                preRankSign = ST_RANK_DOWN;
            }
            return string.Format("{0}({1,2})", preRankSign, preRankNum + 1);  // 順位は1相対なので、"+1"する
        }
    }
}
