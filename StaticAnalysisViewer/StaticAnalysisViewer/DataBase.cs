using System;
using System.Collections.Generic;

namespace StaticAnalysisViewer
{
    // 読み込んだ集計CSV(1ファイル=1週分)を保持し、並べ替え・検索する
    // (以前はForm1.cs内に同居していたものを独立したファイルへ移した)
    public class DataBase
    {
        public int UnknownIdx { get; } = -1;

        // 以前は10000件固定の配列で、超えると配列外で落ちていたのでListにした
        private readonly List<DataBaseEntry> _dataArray = new List<DataBaseEntry>();
        private string[] _categories;
        private int _categoryIdx = 0;
        private int _columnNum = 0;      // 列数（最初に読んだファイルの1行目の列数。制約：全ファイル同一とする）

        // 並べ替えメソッド(値の大きい順。比較できない行は後ろへ)
        private int CompareArray(string[] x, string[] y)
        {
            Boolean isXComparable = TryGetCategoryValue(x, out int xValue);
            Boolean isYComparable = TryGetCategoryValue(y, out int yValue);

            // 両方とも比較不能なときに1と-1を返し分けていたため、x>yとy>xが同時に成立して
            // Array.Sortが「矛盾した結果を返します」で落ちることがあった
            if (!isXComparable && !isYComparable)
            {
                return 0;
            }
            if (!isXComparable)
            {
                return 1;
            }
            if (!isYComparable)
            {
                return -1;
            }

            return yValue.CompareTo(xValue);
        }

        // 比較対象の列が範囲内にあり、数値として読める場合だけtrue
        private Boolean TryGetCategoryValue(string[] values, out int value)
        {
            value = 0;
            return _categoryIdx < values.Length && int.TryParse(values[_categoryIdx], out value);
        }

        // 初期化
        public void Initialize()
        {
            _dataArray.Clear();
            _categories = null;
            _categoryIdx = 0;
            _columnNum = 0;
        }

        // データ配列生成
        public void CreateArray(string data, string label)
        {
            DataBaseEntry entry = new DataBaseEntry();
            entry.Label = label;

            // 行ごとに抽出(1行目は見出し行)
            var rows = data.Split('\n');
            int length = rows.Length - 1;
            entry.Data = new string[length][];

            if (_categories == null)
            {
                _categories = rows[0].Split(',');
            }

            //セルごとに抽出
            for (int i = 0; i < length; i++)
            {
                entry.Data[i] = rows[i + 1].Split(',');
            }

            // 行数を設定
            entry.RowNum = length;

            // 制約：すべて同一のフォーマットを読むこと。読み込むファイルごとに列数が変わらないこと
            // 列数を記憶(最初の1回だけ)
            if (_columnNum == 0)
            {
                _columnNum = entry.Data[0].Length;
            }

            _dataArray.Add(entry);
        }

        // データを並び替える
        public void SortData(int arrayIdx, int categoryIdx)
        {
            // 並び替え基準を記憶
            _categoryIdx = categoryIdx;

            // 並び替え
            Array.Sort(_dataArray[arrayIdx].Data, CompareArray);
        }

        // データ配列取得
        public DataBaseEntry GetData(int arrayIdx)
        {
            return _dataArray[arrayIdx];
        }

        // データのインデックス取得
        public int GetIdx(int arrayIdx, int searchIdx, string name)
        {
            if (arrayIdx < 0)
            {
                return UnknownIdx;
            }

            DataBaseEntry entry = _dataArray[arrayIdx];
            for (int i = 0; i < entry.RowNum; i++)
            {
                if (entry.Data[i].Length > 1 && entry.Data[i][searchIdx].IndexOf(name) >= 0)
                {
                    return i;
                }
            }
            return UnknownIdx;
        }

        // カテゴリ文字列を取得
        public string[] GetCategories()
        {
            return _categories;
        }

        // 配列数取得
        public int GetArrayNum()
        {
            return _dataArray.Count;
        }

        // 列数取得
        public int GetColumnNum()
        {
            return _columnNum;
        }

        // 行数取得(指定したファイルのデータ行数)
        public int GetRowNum(int arrayIdx)
        {
            return _dataArray[arrayIdx].RowNum;
        }
    }

    // 読み込んだ集計CSV 1ファイル分のデータ
    public struct DataBaseEntry
    {
        public string Label;    // 表示するラベル名
        public int RowNum;      // データ行数(ヘッダ行を除く)
        public string[][] Data; // データ配列
    }
}
