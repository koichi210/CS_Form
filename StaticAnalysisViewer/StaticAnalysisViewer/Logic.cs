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
        public static string CreateLabelName(string FilePath)
        {
            var DirPath = System.IO.Path.GetDirectoryName(FilePath);
            var DirArray = DirPath.Split('\\');
            return DirArray[DirArray.Length - 1];
        }

        public static int CreateCountNumTotal(DataBase DB, DataBase_T array)
        {
            int CountLineTotal = 0;
            for (int i = 0; i < array.ColumnNum; i++)
            {
                // Rowが短い場合はカラ行
                if (array.Data[i].Length < DB.GetRowNum())
                {
                    continue;
                }

                CountLineTotal += int.Parse(array.Data[i][CATEGORY_IDX_CNT_LINE]);
            }
            return CountLineTotal;
        }

        public static string CreateRankingString(DataBase DB, int PreArrayIdx, DataBase_T array, int TopRunkingNum)
        {
            // ランキングのヘッダ
            string Result = string.Format("{0,4}\t{1,8}\t{2,-15}\t{3,8}\t{4,10}  {5,8}" + Environment.NewLine + Environment.NewLine,
                                "Rank",
                                "LastWeek",
                                "FileName",
                                "CountLine",
                                "MaxCycMod",
                                "MaxCycStrict");

            int LoopMax = System.Math.Min(TopRunkingNum, array.ColumnNum);
            for (int i = 0; i < LoopMax; i++)
            {
                // Rowが短い場合はカラ行
                if (array.Data[i].Length < DB.GetRowNum())
                {
                    continue;
                }

                string[] Path = array.Data[i][CATEGORY_IDX_FNAME].Split('\\');

                int PreRunkNum = DB.GetIdx(PreArrayIdx, CATEGORY_IDX_FNAME, array.Data[i][CATEGORY_IDX_FNAME]);
                string PreRunk = CreatePreRankingString(DB, i, PreRunkNum);

                Result += string.Format("{0,4}\t{1,-8}\t{2,-15}\t{3,8}\t{4,10}  {5,8}" + Environment.NewLine,
                            i + 1,                                      // Idx
                            PreRunk,                                    // New!
                            Path[Path.Length - 1].Replace("\"", ""),    // FileName
                            array.Data[i][CATEGORY_IDX_CNT_LINE],       // CountLine
                            array.Data[i][CATEGORY_IDX_CNT_CODE],       // CountCode
                            array.Data[i][CATEGORY_IDX_CYCLOMATIC]      // Cyclomatic
                            );
            }

            return Result;
        }

        public static string CreatePreRankingString(DataBase DB, int CurRunkNum, int PreRunkNum)
        {
            // 前回のランキングを取得し、ランキング変動文字列を生成
            if (PreRunkNum == DB.UNKNOWN_IDX)
            {
                return ST_RANK_NEW;
            }
            else
            {
                string PreRunkSign = ST_RANK_PEND;
                if (PreRunkNum > CurRunkNum)
                {
                    PreRunkSign = ST_RANK_UP;
                }
                else if (PreRunkNum < CurRunkNum)
                {
                    PreRunkSign = ST_RANK_DOWN;
                }
                return string.Format("{0}({1,2})", PreRunkSign, PreRunkNum + 1);  // 順位は1相対なので、"+1"する
            }
        }
    }
}
