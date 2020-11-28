﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using StandardTemplate;

namespace StaticAnalysisViewer
{
    public partial class Form1 : Form
    {
        private StcFileInputOutput fio = new StcFileInputOutput();
        private DataBase DB = new DataBase();
        private SaveRestore sr = new SaveRestore();
        private StcUtils util = new StcUtils();     // ツール系

        // 表示用
        private static readonly string ST_RANK_UP   = "↑";
        private static readonly string ST_RANK_DOWN = "↓";
        private static readonly string ST_RANK_PEND = "－";
        private static readonly string ST_RANK_NEW  = "New!";

        // Default値
        private readonly String SettingFileName = @"StaticAnalysisViewer.xml";
        private static readonly int    DEF_CATEGORY_SORT_IDX = 3;

		// Categoryのインデックス値
        private static readonly int CATEGORY_IDX_FNAME = 0;
        private static readonly int CATEGORY_IDX_CNT_LINE = 1;
        private static readonly int CATEGORY_IDX_CNT_CODE = 2;
        private static readonly int CATEGORY_IDX_CYCLOMATIC = 3;

        // ヘルプ
        public String HelpLink = "";

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.StaticAnalysisViewer;

            // カレントディレクトリ移動
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            sr.RegistItem(this);
            sr.LoadProc(SettingFileName, this);
            util.UpdateProfileList(ref comboBox_Profile);
        }

        // Csvを読み込んで配列に追加
        private void Button_AddData_Click(object sender, EventArgs e)
        {
            // コンボボックス初期化
            Combo_SortCategory.Items.Clear();
            Combo_RankingWeekly.Items.Clear();

            // Csvデータを読み込んでデータベースへ追加
            if (! CreateDataBase())
            {
                return;
            }

            // データをソート
            if (! SortExecute())
            {
                return;
            }

            // 最後に追加したデータを選択状態にする
            Combo_RankingWeekly.SelectedIndex = Combo_RankingWeekly.Items.Count - 1;

            // 結果表示
            ShowResult();
        }

        // 並び替え要求
        private void Button_Sort_Click(object sender, EventArgs e)
        {
            SortExecute();
        }

        // ランキングを生成要求
        private void Button_RankShow_Click(object sender, EventArgs e)
        {
            ShowResult();
        }

        private void TextBox_SortResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.A & e.Control == true)
            {
                TextBox_Ranking.SelectAll();
            }
        }

        private void LoadDataPathKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.A & e.Control == true)
            {
                TextBox_LoadDataList.SelectAll();
            }
        }

        private void TextBox_TopRankingNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                ShowResult();
            }
        }

        private void TextBox_TopRankingNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            //押されたキーが 数値でない場合は、イベントをキャンセルする
            e.Handled = util.IsKeyPressNumber(e);
        }

        private void Combo_RankingWeekly_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowResult();
        }

        // Csvデータを読み込んでデータベースへ追加
        private bool CreateDataBase()
        {
            // 最初に取り込むcsvからCategoryを作成する
            bool IsCategoryAlreadySet = false;

            //プログレスバーの初期化
            ProgressBar_LoadStatus.Maximum = TextBox_LoadDataList.Lines.Length;
            ProgressBar_LoadStatus.Minimum = 0;
            ProgressBar_LoadStatus.Value = 0;

            DB.Initialize();
            for (int i = ProgressBar_LoadStatus.Minimum; i < ProgressBar_LoadStatus.Maximum; i++, ProgressBar_LoadStatus.Value++)
            {
                string Data = fio.GetFileData(TextBox_LoadDataList.Lines[i]);
                if (Data.Equals(""))
                {
                    //ファイルパスが無効だったら次へ
                    continue;
                }

                // ランキングに表示するラベルを生成
                string Label = CreateLabelName(TextBox_LoadDataList.Lines[i]);

                // DBにデータを設定
                DB.CreateArray(Data, Label);
                Combo_RankingWeekly.Items.Add(Label);

                // Categoryコンボボックスを設定
                if (! IsCategoryAlreadySet)
                {
                    string[] Category = DB.GetCategory();
                    Combo_SortCategory.Items.AddRange(Category);
                    Combo_SortCategory.SelectedIndex = DEF_CATEGORY_SORT_IDX;
                    IsCategoryAlreadySet = true;
                }
            }

            if (Combo_RankingWeekly.Items.Count <= 0)
            {
                MessageBox.Show("読み込めるファイルがありません。",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ラベル名を生成
        private string CreateLabelName(string FilePath)
        {
            // ファイルがおいてある直上のフォルダ名取得
            var DirPath = Path.GetDirectoryName(FilePath);
            var DirArray = DirPath.Split('\\');
            return DirArray[DirArray.Length - 1];
        }

        // 並び替え要求
        private bool SortExecute()
        {
            int ArrayNum = DB.GetArrayNum();
            if (ArrayNum == 0)
            {
                MessageBox.Show("並び替えるデータがありません。",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            // データベースの登録されているすべてを並べ替え
            for (int i = 0; i < ArrayNum; i++)
            {
                DB.SortData(i, Combo_SortCategory.SelectedIndex);
            }

            return true;
        }

        private int CreateCountNumTotal(DataBase_T array)
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

        private string CreateRankingString(int PreArrayIdx, DataBase_T array, int TopRunkingNum)
        {
            // ランキングのヘッダ
            string Result = string.Format("{0,4}\t{1,8}\t{2,-15}\t{3,8}\t{4,10}  {5,8}" + System.Environment.NewLine + System.Environment.NewLine,
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
                string PreRunk = CreatePreRankingString(i, PreRunkNum);

                Result += string.Format("{0,4}\t{1,-8}\t{2,-15}\t{3,8}\t{4,10}  {5,8}" + System.Environment.NewLine,
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

        private string CreatePreRankingString(int CurRunkNum, int PreRunkNum)
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

        // ランキングを生成
        private bool RankShowExecute()
        {
            if (Combo_RankingWeekly.SelectedIndex == -1)
            {
                MessageBox.Show("表示するデータがありません。",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            // ひとつ前の配列Idx
            int PreArrayIdx = Combo_RankingWeekly.SelectedIndex - 1;

            // 表示対象の配列取得
            DataBase_T array = DB.GetData(Combo_RankingWeekly.SelectedIndex);

            //表示するランキング数を取得
            int TopRunkingNum = int.Parse(TextBox_TopRankingNum.Text);

            // ランキング文字列生成＆表示
            TextBox_Ranking.Text = CreateRankingString(PreArrayIdx, array, TopRunkingNum);

            // 「行数の合計」の文字列生成＆表示
            TextBox_CountLineTotal.Text = CreateCountNumTotal(array).ToString();
            
            // 「ファイル数の合計」の文字列生成＆表示
            TextBox_FileNumTotal.Text = array.ColumnNum.ToString();

            return true;
        }

        // 結果表示
        private void ShowResult()
        {
            // ランキングを表示
            if ( ! RankShowExecute())
            {
                return;
            }

            // グラフ生成
            CreateRankingGraphics();
        }

        private void CreateRankingGraphics()
        {
            DataBase_T array = DB.GetData(Combo_RankingWeekly.SelectedIndex);
            int TopRunkingNum = int.Parse(TextBox_TopRankingNum.Text);
            int CategoryIdx = Combo_SortCategory.SelectedIndex;

            // 表示を消す
            Chart_Result.Series.Clear();
            Chart_Result.Legends.Clear();
            Chart_Result.Controls.Clear();

            Chart chart = new Chart();
            chart.Width = 150;
            chart.Height = 150;

            Series series = new Series();
            series.ChartType = SeriesChartType.Pie;
            series["PieStartAngle"] = "270";

            int LoopMax = System.Math.Min(TopRunkingNum, array.ColumnNum);
            for (int i = 0; i < LoopMax; i++)
            {
                // Rowが短い場合はカラ行
                if (array.Data[i].Length < DB.GetRowNum())
                {
                    continue;
                }

                int YValues = 0;
                if (array.Data[i][CategoryIdx] != String.Empty)
                {
                    YValues = int.Parse(array.Data[i][CategoryIdx]);
                }

                DataPoint point = new DataPoint();
                point.XValue = 0;
                point.YValues = new double[] { YValues };
                series.Points.Add(point);

                // TODO：凡例を表示
                //string[] Path = array.Data[i][CATEGORY_NAME_IDX].Split('\\');
                //series.Name = Path[Path.Length - 1];
                //chart.Name = Path[Path.Length - 1];
            }
            chart.Series.Add(series);

            ChartArea area = new ChartArea();
            area.AxisX.IsLabelAutoFit = true;
            area.AxisY.IsLabelAutoFit = true;
            chart.ChartAreas.Add(area);
            chart.Name = "Ranking";
            Chart_Result.Controls.Add(chart);

            Chart_Result.Visible = true;
        }

        private void button_Help_Click(object sender, EventArgs e)
        {
            if (HelpLink == String.Empty)
            {
                MessageBox.Show("ヘルプリンクを設定してください");
                return;
            }
            System.Diagnostics.Process.Start(HelpLink);
        }

        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + comboBox_Profile.Text;
            sr.LoadProc(LoadFileName, this);
        }

        private void button_ProfileLoad_Click(object sender, EventArgs e)
        {
            String LoadFileName = fio.SelectLoadFileName(SettingFileName);
            if (sr.LoadProc(LoadFileName, this))
            {
                comboBox_Profile.Text = Path.GetFileName(LoadFileName);
            }
        }

        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(comboBox_Profile.Text);
            if (sr.SaveSetting(SaveFileName, this))
            {
                util.UpdateProfileList(ref comboBox_Profile, Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました。" + Environment.NewLine + SaveFileName);
            }
        }
    }

    public partial class DataBase
    {
        public readonly int UNKNOWN_IDX = -1;

        // TODO:最大数を可変にしたい
        private const uint MaxDbNum = 10000;

        private DataBase_T[] DataArray = new DataBase_T[MaxDbNum];
        private string[] Category;
        private int CategoryIdx = 0;
        private int ArrayNum = 0;       // 配列の登録数
        private int RowMaxNum = 0;      // 行の最大数（制約：一意とする）

        // 並べ替えメソッド
        private int CompareArray(string[] x, string[] y)
        {
            // そもそも比較するための情報がそろっていない場合
            if (CategoryIdx > x.Length || x[CategoryIdx] == "" || !Char.IsDigit(x[CategoryIdx], 0))
            {
                return 1;
            }
            else if (CategoryIdx > y.Length || y[CategoryIdx] == "" || !Char.IsDigit(y[CategoryIdx], 0))
            {
                return -1;
            }

            if (int.Parse(x[CategoryIdx]) < int.Parse(y[CategoryIdx]))
            {
                return 1;
            }
            else if (int.Parse(x[CategoryIdx]) > int.Parse(y[CategoryIdx]))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        // 初期化
        public void Initialize()
        {
            //DataArray = null;
            Category = null;
            CategoryIdx = 0;
            ArrayNum = 0;
            RowMaxNum = 0;
        }

        // データ配列生成
        public void CreateArray(string Data, string Label)
        {
            DataArray[ArrayNum].Label = Label;

            // 行ごとに抽出
            var Rows = Data.Split('\n');
            int Length = Rows.Length - 1;
            DataArray[ArrayNum].Data = new string[Length][];

            if (Category == null)
            {
                Category = Rows[0].Split(',');
            }

            //セルごとに抽出
            for (int i = 0, idx = 1; i < Length; i++, idx++)
            {
                //if (Rows[idx] == String.Empty)
                //{
                //    continue;
                //}
                DataArray[ArrayNum].Data[i] = Rows[idx].Split(',');
            }

            // 列数を設定
            DataArray[ArrayNum].ColumnNum = Length;

            // 制約：すべて同一のフォーマットを読むこと。読み込むファイルごとにRowが変わらないこと
            // 行数の最大値を更新
            if (RowMaxNum == 0)
            {
                RowMaxNum = DataArray[ArrayNum].Data[0].Length;
            }

            // 配列全体数を更新
            ArrayNum++;
        }

        // データを並び替える
        public void SortData(int ArrayIdx, int Idx)
        {
            // 並び替え基準を記憶
            CategoryIdx = Idx;

            // 並び替え
            System.Array.Sort(DataArray[ArrayIdx].Data, CompareArray);
        }

        // 未使用
        private void Swap(ref string[] a, ref string[] b)
        {
            string[] c = a;
            a = b;
            b = c;
        }

        // データ配列取得
        public DataBase_T GetData(int Idx)
        {
            return DataArray[Idx];
        }

        // データのインデックス取得
        public int GetIdx(int ArrayIdx, int SerchIdx, string Name)
        {
            if (ArrayIdx >= 0)
            {
                for (int i = 0; i < DataArray[ArrayIdx].ColumnNum; i++)
                {
                    if (DataArray[ArrayIdx].Data[i].Length > 1 &&
                        DataArray[ArrayIdx].Data[i][SerchIdx].IndexOf(Name) >= 0)
                    {
                        return i;
                    }
                }
            }
            return UNKNOWN_IDX;
        }

        // カテゴリ文字列を取得
        public string[] GetCategory()
        {
            return Category;
        }

        // 配列数取得
        public int GetArrayNum()
        {
            return ArrayNum;
        }

        // 行数取得
        public int GetRowNum()
        {
            return RowMaxNum;
        }

        // 列数取得
        public int GetColumnNum(int ArrayIdx)
        {
            return DataArray[ArrayIdx].ColumnNum;
        }
    }

    public struct DataBase_T
    {
        public string Label;    // 表示するラベル名
        public int ColumnNum;   // 列の最大数
        public string[][] Data; // データ配列
    }
}

