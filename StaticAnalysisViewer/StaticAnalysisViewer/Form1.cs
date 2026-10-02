using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using StandardTemplate;

namespace StaticAnalysisViewer
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private StcFileInputOutput fio = new StcFileInputOutput();
        private DataBase DB = new DataBase();

        // Default値
        private readonly String SettingFileName = @"StaticAnalysisViewer.json";

        // 設定ファイルはJSONが基本。旧XML(StaticAnalysisViewer.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"StaticAnalysisViewer.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\StaticAnalysisViewer\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String AppName = "StaticAnalysisViewer";
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(AppName);

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private Boolean LoadProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(sr, filePath) : sr.LoadProc(filePath, this);
        }

        private Boolean SaveProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(sr, filePath) : sr.SaveSetting(filePath, this);
        }

        private static readonly int    DEF_CATEGORY_SORT_IDX = 3;

        // ヘルプ
        public String HelpLink { get; set; } = "";

        public Form1()
        {
            InitializeComponent();
            InitializeCommonSettings(Properties.Resources.StaticAnalysisViewer);

            InitializePlaceholders();
            InitializeToolTips();

            sr.RegisterItem(this);
            String defaultJsonPath = Path.Combine(userDataFolder, SettingFileName);
            String defaultXmlPath = Path.Combine(userDataFolder, LegacySettingFileName);
            JsonSaveRestore.LoadWithMigration(sr, defaultJsonPath, defaultXmlPath,
                path => sr.LoadProc(path, this));
            util.UpdateProfileList(comboBox_Profile, ProfileExtensions, "", userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(TextBox_LoadDataList・TextBox_Ranking、Windowsの仕様で表示されない)と、
        // ReadOnlyの集計結果欄(TextBox_CountLineTotal・TextBox_FileNumTotal)は対象外
        private void InitializePlaceholders()
        {
            TextBox_TopRankingNum.PlaceholderText = "例: 10";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(TextBox_LoadDataList, "集計CSVのパスを1行に1つ、古い週から順に書く。各CSVの1行目は見出し行。表示名は各ファイルの親フォルダ名になる");
            toolTip.SetToolTip(Button_LoadData, "一覧のCSVを全て読み込み直し、最後のファイルのランキングを表示する");
            toolTip.SetToolTip(Combo_SortCategory, "並び替えに使う列(CSVの見出し)。値の大きい順に並べる");
            toolTip.SetToolTip(Button_Sort, "選んだ列で全データを並べ替える。画面の表示は「表示」ボタンで更新する");
            toolTip.SetToolTip(Combo_RankingWeekly, "表示するデータ。1つ前に読み込んだファイルを前週として順位の変動を出す");
            toolTip.SetToolTip(TextBox_TopRankingNum, "ランキングとグラフに出す上位の件数(数字のみ)。Enterキーで表示を更新する");
            toolTip.SetToolTip(TextBox_Ranking, "LastWeek列: ↑↓－は前週からの順位変動と前週の順位、New!は前週のデータに無いファイル");
        }

        // *******************************************************************************
        // データ保存先フォルダの変更(システムメニューから呼び出す)
        // ([[Cheetos/Form1.cs]]の同名機能と同じ考え方)
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DataFolderMenu.AppendToSystemMenu(this);
        }

        protected override void WndProc(ref Message m)
        {
            if (DataFolderMenu.IsChangeDataFolderCommand(m))
            {
                DataFolderMenu.ChangeDataFolder(AppName, userDataFolder,
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, AppName));
                return;
            }

            base.WndProc(ref m);
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
            e.Handled = util.IsNotNumberKey(e);
        }

        private void Combo_RankingWeekly_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowResult();
        }

        // Csvデータを読み込んでデータベースへ追加
        private bool CreateDataBase()
        {
            // 最初に取り込むcsvからCategoryを作成する
            bool isCategoryAlreadySet = false;

            //プログレスバーの初期化
            ProgressBar_LoadStatus.Maximum = TextBox_LoadDataList.Lines.Length;
            ProgressBar_LoadStatus.Minimum = 0;
            ProgressBar_LoadStatus.Value = 0;

            DB.Initialize();
            for (int i = ProgressBar_LoadStatus.Minimum; i < ProgressBar_LoadStatus.Maximum; i++, ProgressBar_LoadStatus.Value++)
            {
                string data = fio.LoadFile(TextBox_LoadDataList.Lines[i]);
                if (data.Equals(""))
                {
                    //ファイルパスが無効だったら次へ
                    continue;
                }

                // ランキングに表示するラベルを生成
                string label = Logic.CreateLabelName(TextBox_LoadDataList.Lines[i]);

                // DBにデータを設定
                DB.CreateArray(data, label);
                Combo_RankingWeekly.Items.Add(label);

                // Categoryコンボボックスを設定
                if (! isCategoryAlreadySet)
                {
                    string[] categories = DB.GetCategories();
                    Combo_SortCategory.Items.AddRange(categories);
                    Combo_SortCategory.SelectedIndex = DEF_CATEGORY_SORT_IDX;
                    isCategoryAlreadySet = true;
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

        // 並び替え要求
        private bool SortExecute()
        {
            int arrayNum = DB.GetArrayNum();
            if (arrayNum == 0)
            {
                MessageBox.Show("並び替えるデータがありません。",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            // データベースの登録されているすべてを並べ替え
            for (int i = 0; i < arrayNum; i++)
            {
                DB.SortData(i, Combo_SortCategory.SelectedIndex);
            }

            return true;
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
            int preArrayIdx = Combo_RankingWeekly.SelectedIndex - 1;

            // 表示対象の配列取得
            DataBase_T array = DB.GetData(Combo_RankingWeekly.SelectedIndex);

            //表示するランキング数を取得
            int topRankingNum = int.Parse(TextBox_TopRankingNum.Text);

            // ランキング文字列生成＆表示
            TextBox_Ranking.Text = Logic.CreateRankingString(DB, preArrayIdx, array, topRankingNum);

            // 「行数の合計」の文字列生成＆表示
            TextBox_CountLineTotal.Text = Logic.CreateCountNumTotal(DB, array).ToString();
            
            // 「ファイル数の合計」の文字列生成＆表示
            TextBox_FileNumTotal.Text = array.RowNum.ToString();

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
            int topRankingNum = int.Parse(TextBox_TopRankingNum.Text);
            int categoryIdx = Combo_SortCategory.SelectedIndex;

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

            int loopMax = System.Math.Min(topRankingNum, array.RowNum);
            for (int i = 0; i < loopMax; i++)
            {
                // Rowが短い場合はカラ行
                if (array.Data[i].Length < DB.GetColumnNum())
                {
                    continue;
                }

                int yValue = 0;
                if (array.Data[i][categoryIdx] != String.Empty)
                {
                    yValue = int.Parse(array.Data[i][categoryIdx]);
                }

                DataPoint point = new DataPoint();
                point.XValue = 0;
                point.YValues = new double[] { yValue };
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
            String loadFileName = Path.Combine(userDataFolder, comboBox_Profile.Text);
            LoadProfile(loadFileName);
        }

        private void button_ProfileLoad_Click(object sender, EventArgs e)
        {
            String loadFileName = fio.SelectLoadFileName(SettingFileName, userDataFolder);
            if (LoadProfile(loadFileName))
            {
                comboBox_Profile.Text = Path.GetFileName(loadFileName);
            }
        }

        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_Profile, ProfileExtensions, SaveProfile,
                userDataFolder, "設定値を保存しました。");
        }
    }

    public partial class DataBase
    {
        public int UNKNOWN_IDX { get; } = -1;

        // 以前は10000件固定の配列で、超えると配列外で落ちていたのでListにした
        private List<DataBase_T> DataArray = new List<DataBase_T>();
        private string[] Categories;
        private int CategoryIdx = 0;
        private int ColumnNum = 0;      // 列数（最初に読んだファイルの1行目の列数。制約：全ファイル同一とする）

        // 並べ替えメソッド(値の大きい順。比較できない行は後ろへ)
        private int CompareArray(string[] x, string[] y)
        {
            int xValue;
            int yValue;
            Boolean isXComparable = TryGetCategoryValue(x, out xValue);
            Boolean isYComparable = TryGetCategoryValue(y, out yValue);

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
            return CategoryIdx < values.Length && int.TryParse(values[CategoryIdx], out value);
        }

        // 初期化
        public void Initialize()
        {
            DataArray.Clear();
            Categories = null;
            CategoryIdx = 0;
            ColumnNum = 0;
        }

        // データ配列生成
        public void CreateArray(string data, string label)
        {
            DataBase_T entry = new DataBase_T();
            entry.Label = label;

            // 行ごとに抽出
            var rows = data.Split('\n');
            int length = rows.Length - 1;
            entry.Data = new string[length][];

            if (Categories == null)
            {
                Categories = rows[0].Split(',');
            }

            //セルごとに抽出
            for (int i = 0, idx = 1; i < length; i++, idx++)
            {
                entry.Data[i] = rows[idx].Split(',');
            }

            // 行数を設定
            entry.RowNum = length;

            // 制約：すべて同一のフォーマットを読むこと。読み込むファイルごとに列数が変わらないこと
            // 列数を記憶(最初の1回だけ)
            if (ColumnNum == 0)
            {
                ColumnNum = entry.Data[0].Length;
            }

            DataArray.Add(entry);
        }

        // データを並び替える
        public void SortData(int arrayIdx, int categoryIdx)
        {
            // 並び替え基準を記憶
            CategoryIdx = categoryIdx;

            // 並び替え
            System.Array.Sort(DataArray[arrayIdx].Data, CompareArray);
        }

        // データ配列取得
        public DataBase_T GetData(int arrayIdx)
        {
            return DataArray[arrayIdx];
        }

        // データのインデックス取得
        public int GetIdx(int arrayIdx, int searchIdx, string name)
        {
            if (arrayIdx >= 0)
            {
                for (int i = 0; i < DataArray[arrayIdx].RowNum; i++)
                {
                    if (DataArray[arrayIdx].Data[i].Length > 1 &&
                        DataArray[arrayIdx].Data[i][searchIdx].IndexOf(name) >= 0)
                    {
                        return i;
                    }
                }
            }
            return UNKNOWN_IDX;
        }

        // カテゴリ文字列を取得
        public string[] GetCategories()
        {
            return Categories;
        }

        // 配列数取得
        public int GetArrayNum()
        {
            return DataArray.Count;
        }

        // 列数取得
        public int GetColumnNum()
        {
            return ColumnNum;
        }

        // 行数取得(指定したファイルのデータ行数)
        public int GetRowNum(int arrayIdx)
        {
            return DataArray[arrayIdx].RowNum;
        }
    }

    public struct DataBase_T
    {
        public string Label;    // 表示するラベル名
        public int RowNum;      // データ行数(ヘッダ行を除く)
        public string[][] Data; // データ配列
    }
}

