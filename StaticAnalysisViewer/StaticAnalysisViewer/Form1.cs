using System;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using StandardTemplate;

namespace StaticAnalysisViewer
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private readonly StcFileInputOutput _fio = new StcFileInputOutput();
        private readonly DataBase _db = new DataBase();

        // Default値
        private const String _settingFileName = @"StaticAnalysisViewer.json";

        // 設定ファイルはJSONが基本。旧XML(StaticAnalysisViewer.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String _legacySettingFileName = @"StaticAnalysisViewer.xml";
        private static readonly String[] _profileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\StaticAnalysisViewer\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String _appName = "StaticAnalysisViewer";
        private readonly String _userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(_appName);

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private Boolean LoadProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(_sr, filePath) : _sr.LoadProc(filePath, this);
        }

        private Boolean SaveProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(_sr, filePath) : _sr.SaveSetting(filePath, this);
        }

        private const int _defaultCategorySortIdx = 3;

        // ヘルプ
        public String HelpLink { get; set; } = "";

        public Form1()
        {
            InitializeComponent();
            InitializeCommonSettings(Properties.Resources.StaticAnalysisViewer);

            InitializePlaceholders();
            InitializeToolTips();

            _sr.RegisterItem(this);
            String defaultJsonPath = Path.Combine(_userDataFolder, _settingFileName);
            String defaultXmlPath = Path.Combine(_userDataFolder, _legacySettingFileName);
            JsonSaveRestore.LoadWithMigration(_sr, defaultJsonPath, defaultXmlPath,
                path => _sr.LoadProc(path, this));
            _util.UpdateProfileList(comboBox_Profile, _profileExtensions, "", _userDataFolder);
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
                DataFolderMenu.ChangeDataFolder(_appName, _userDataFolder,
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, _appName));
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
            if (e.KeyCode == Keys.A && e.Control)
            {
                TextBox_Ranking.SelectAll();
            }
        }

        private void LoadDataPathKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                TextBox_LoadDataList.SelectAll();
            }
        }

        private void TextBox_TopRankingNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowResult();
            }
        }

        private void TextBox_TopRankingNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            //押されたキーが 数値でない場合は、イベントをキャンセルする
            e.Handled = _util.IsNotNumberKey(e);
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

            // Linesは呼ぶたびに配列を作り直すので1回だけ取得する
            string[] dataPaths = TextBox_LoadDataList.Lines;

            //プログレスバーの初期化
            ProgressBar_LoadStatus.Maximum = dataPaths.Length;
            ProgressBar_LoadStatus.Minimum = 0;
            ProgressBar_LoadStatus.Value = 0;

            _db.Initialize();
            for (int i = ProgressBar_LoadStatus.Minimum; i < ProgressBar_LoadStatus.Maximum; i++, ProgressBar_LoadStatus.Value++)
            {
                string data = _fio.LoadFile(dataPaths[i]);
                if (data == "")
                {
                    //ファイルパスが無効だったら次へ
                    continue;
                }

                // ランキングに表示するラベルを生成
                string label = Logic.CreateLabelName(dataPaths[i]);

                // DBにデータを設定
                _db.CreateArray(data, label);
                Combo_RankingWeekly.Items.Add(label);

                // Categoryコンボボックスを設定
                if (! isCategoryAlreadySet)
                {
                    string[] categories = _db.GetCategories();
                    Combo_SortCategory.Items.AddRange(categories);
                    Combo_SortCategory.SelectedIndex = _defaultCategorySortIdx;
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
            int arrayNum = _db.GetArrayNum();
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
                _db.SortData(i, Combo_SortCategory.SelectedIndex);
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
            DataBaseEntry array = _db.GetData(Combo_RankingWeekly.SelectedIndex);

            //表示するランキング数を取得
            int topRankingNum = int.Parse(TextBox_TopRankingNum.Text);

            // ランキング文字列生成＆表示
            TextBox_Ranking.Text = Logic.CreateRankingString(_db, preArrayIdx, array, topRankingNum);

            // 「行数の合計」の文字列生成＆表示
            TextBox_CountLineTotal.Text = Logic.CreateCountNumTotal(_db, array).ToString();
            
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
            DataBaseEntry array = _db.GetData(Combo_RankingWeekly.SelectedIndex);
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
                if (array.Data[i].Length < _db.GetColumnNum())
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
            String loadFileName = Path.Combine(_userDataFolder, comboBox_Profile.Text);
            LoadProfile(loadFileName);
        }

        private void button_ProfileLoad_Click(object sender, EventArgs e)
        {
            String loadFileName = _fio.SelectLoadFileName(_settingFileName, _userDataFolder);
            if (LoadProfile(loadFileName))
            {
                comboBox_Profile.Text = Path.GetFileName(loadFileName);
            }
        }

        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(_util, _fio, comboBox_Profile, _profileExtensions, SaveProfile,
                _userDataFolder, "設定値を保存しました。");
        }
    }
}

