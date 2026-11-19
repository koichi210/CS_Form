using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using StandardTemplate;

namespace VisualStudioBuilder
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private const String _defaultSettingFileName = "VisualStudioBuilder.json";

        // 設定ファイルはJSONが基本。旧XML(VisualStudioBuilder.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String _legacySettingFileName = @"VisualStudioBuilder.xml";
        private static readonly String[] _profileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\VisualStudioBuilder\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String _appName = "VisualStudioBuilder";
        private String _userDataFolder =StandardTemplate.UserDataLocation.GetUserDataFolder(_appName);

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
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(_sr, filePath) : _sr.SaveXmlFile(filePath);
        }

        private const String _buildListDisable = "×";

        private const String _headerBuild = "ビルド";
        private const String _headerSolution = "ソリューションファイル";
        private const String _headerProjectPath = "プロジェクトフルパス";

        private const int _buildEnableIdx = 0;
        private const int _solutionNameIdx = 1;
        private const int _projectPathIdx = 2;

        private readonly StcFileInputOutput _fio = new StcFileInputOutput();

        // ビルド1回分の入力値。画面の値は開始時点でここへ写し、バックグラウンドの処理(BuildWorker_DoWork)は
        // これだけを見る(別スレッドから画面のコントロールに触らないため)
        internal class BuildWorkerInfo
        {
            public String VisualStudioExePath { get; set; }
            public String BuildOption { get; set; }
            public String LogDirectory { get; set; }
            public List<BuildTarget> Targets { get; set; }
            public Boolean IsDetectError { get; set; }
            public String DetectBuildErrorWord { get; set; }
            public Boolean IsExclude { get; set; }
            public String IgnoreExecuteFile { get; set; }
            public Boolean IsDeleteDirectory { get; set; }
            public String DeleteTargetsText { get; set; }
        }

        public Form1()
        {
            InitializeComponent();

            InitializeCommonSettings(Properties.Resources.VisualStudioBuilder);

            // DataGridViewの初期設定
            InitializeDataGridView();

            InitializePlaceholders();
            InitializeToolTips();

            // Designer.cs側でEnabled=falseになっている(未実装だった頃の名残)。
            // 削除処理を実装したので有効化する。Designer.csは自動生成で上書きされるため
            // ここで切り替える
            checkBox_DeleteDirectory.Enabled = true;

            // ビルド中の経過(何をしているか)をビルド結果欄へ出すため、BuildWorkerから途中経過を受け取る
            BuildWorker.WorkerReportsProgress = true;
            BuildWorker.ProgressChanged += BuildWorker_ProgressChanged;

            _sr.RegisterItem(this);
            LoadUserData();
        }

        // 保存先フォルダの既定設定とプロファイル一覧を読み込む(起動時と、保存先変更の直後に使う)
        private void LoadUserData()
        {
            // 表は読み込み側(RowCount=1)では1行目のセルの中身が残るため、先に空にする
            // (新しい保存先に設定ファイルが無い場合でも、起動直後と同じ空の1行になる)
            dataGridView.Rows.Clear();
            dataGridView.RowCount = 1;

            String defaultJsonPath = Path.Combine(_userDataFolder, _defaultSettingFileName);
            String defaultXmlPath = Path.Combine(_userDataFolder, _legacySettingFileName);
            JsonSaveRestore.LoadWithMigration(_sr, defaultJsonPath, defaultXmlPath,
                path => _sr.LoadProc(path, this));
            UpdateBuildGui();
            UpdateOutputGui();
            UpdateRowCount();
            _util.UpdateProfileList(comboBox_Profile, _profileExtensions, _defaultSettingFileName, _userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 削除対象の欄(textBox_DeleteDirectoryName)は複数行にした(Windowsの仕様で表示されない)ので、
        // 代わりに下のラベル(label_DeleteTargetHint)に書き方を出している
        // (既定値は[[SaveRestore.cs]]のRegisterCtrlに合わせた)
        private void InitializePlaceholders()
        {
            textBox_VisualStudioExePath.PlaceholderText = @"例: C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe";
            textBox_BuildOption.PlaceholderText = "例: /rebuild release";
            textBox_LogDirectory.PlaceholderText = @"例: C:\Work\BuildLog";
            textBox_DetectBuildErrorWord.PlaceholderText = "例: error";
            textBox_ExcludeWord.PlaceholderText = "例: LNK1168";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている。
        // グリッドの列はInitializeDataGridViewで作るので、その後に呼ぶこと
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(comboBox_Profile, "保存済みのプロファイル。選ぶとその内容を読み込む");
            toolTip.SetToolTip(textBox_VisualStudioExePath, "ビルドに使うdevenv.exeのパス。ビルド実行の前にファイルがあるか確かめる");
            toolTip.SetToolTip(textBox_BuildOption, "devenvにそのまま渡す引数(ビルドの種類と構成名)");
            toolTip.SetToolTip(button_Build, "ビルド欄が○のソリューションを順にビルドし、結果を「ビルド結果」タブに出す。" +
                "開始時にログ出力先フォルダを中身ごと削除する。見つからないソリューションは飛ばして「存在しないリスト」に出す");
            toolTip.SetToolTip(checkBox_DeleteDirectory, "オンならビルド完了後、ビルド対象の各プロジェクトフォルダ以下(サブフォルダ含む)から、下の欄に書いた名前と一致するフォルダ・ファイルを探して削除する");
            toolTip.SetToolTip(textBox_DeleteDirectoryName, "1行に1つ。フォルダ名(obj など)やファイルのパターン(*.ncb など。*と?が使える)を書く。" +
                "どの行もフォルダとファイルの両方に当てる。\\ / : を含む行や、*・*.* のように全部に当たる行は使わない");

            toolTip.SetToolTip(textBox_LogDirectory, "ソリューションごとのビルドログの出力先。ログ名は「プロジェクトのパス＋ソリューション名」の\\と:を_にしたもの" +
                "(例: D__Work_Foo_Foo.log)。ビルド開始時に中身ごと削除して作り直す。空欄ならログを出さない。Enterキーでフォルダを開く");
            toolTip.SetToolTip(checkBox_DetectBuildError, "ビルド後に各ログを検知ワードで調べ、成功/失敗の一覧を「ビルド結果」タブに出す");
            toolTip.SetToolTip(textBox_DetectBuildErrorWord, "この文字列を含むログをビルド失敗と判定する");
            toolTip.SetToolTip(checkBox_IsExclude, "オンなら下の検知ワードを含むログを成功扱いにせず、「実行ファイルの上書きに失敗」として別枠で表示する");
            toolTip.SetToolTip(textBox_ExcludeWord, "この文字列を含むログを、実行中のexeを差し替えられなかったものとして別枠に分ける");

            toolTip.SetToolTip(button_AddAllSolution, "指定フォルダ配下(サブフォルダ含む)の.slnを全て登録する。今の一覧は消える");
            toolTip.SetToolTip(label_RowCount, "表に登録されている行の数と、そのうちビルド欄が○の行の数");
            toolTip.SetToolTip(textBox_BuildLog, "ビルドの経過と結果(存在しないリスト・成功/失敗の一覧)。Ctrl+Aで全選択");
            dataGridView.Columns[_buildEnableIdx].ToolTipText = "○の行だけビルドする";
            dataGridView.Columns[_solutionNameIdx].ToolTipText = "ソリューションファイル名(.sln込み)。ダブルクリックで開く。空欄のままパスを入れると「フォルダ名.sln」が入る";
            dataGridView.Columns[_projectPathIdx].ToolTipText = "ソリューションファイルがあるフォルダ。ダブルクリックでフォルダを開く";
        }

        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String loadFileName = Path.Combine(_userDataFolder, comboBox_Profile.Text);
            LoadProfile(loadFileName);
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(_util, _fio, comboBox_Profile, _profileExtensions, SaveProfile, _userDataFolder);
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
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, _appName),
                    newFolder =>
                    {
                        // 再起動なしで新しい保存先のプロファイルを読み込み直す
                        _userDataFolder = newFolder;
                        LoadUserData();
                    });
                return;
            }

            base.WndProc(ref m);
        }

        private void button_RemoveRow_Click(object sender, EventArgs e)
        {
            if (dataGridView.RowCount > 1)
            {
                dataGridView.Rows.RemoveAt(dataGridView.CurrentRow.Index);
            }
        }

        // 表の各行を写し取る(バックグラウンドの処理へ渡すため)
        private List<BuildTarget> CollectBuildTargets()
        {
            List<BuildTarget> targets = new List<BuildTarget>();
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                targets.Add(new BuildTarget
                {
                    BuildEnable = GetCellData(i, _buildEnableIdx),
                    SolutionName = GetCellData(i, _solutionNameIdx),
                    ProjectPath = GetCellData(i, _projectPathIdx),
                });
            }
            return targets;
        }

        private void button_Build_Click(object sender, EventArgs e)
        {
            // ビルド中に押されると、実行中のビルドのログフォルダを消してしまうため先に弾く
            // (ビルド中はボタンを無効にしてあるが、念のため)
            if (BuildWorker.IsBusy)
            {
                return;
            }

            // devenv.exeが無いとバッチは「指定されたパスが見つかりません」で全部失敗するのに、
            // 以前は「ビルド完了」と出ていた。始める前に確かめて、無ければ止める
            String exeError = Logic.ValidateVisualStudioExePath(textBox_VisualStudioExePath.Text, File.Exists);
            if (exeError != null)
            {
                textBox_BuildLog.Text = "[エラー] " + exeError + Environment.NewLine;
                MessageBox.Show(this, exeError, "VisualStudioBuilder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 以前はList<object>へ順番に詰めてDoWork側で位置で取り出していたが、
            // 並び順がずれると気づきにくいため、専用クラスをそのまま渡す。
            // ソリューションの存在チェック・ログフォルダの作り直し・バッチ作成は、表が長いと時間がかかるので
            // バックグラウンド(BuildWorker_DoWork)で行う
            BuildWorkerInfo bwi = new BuildWorkerInfo
            {
                VisualStudioExePath = textBox_VisualStudioExePath.Text.Trim().Trim('"'),
                BuildOption = textBox_BuildOption.Text,
                LogDirectory = textBox_LogDirectory.Text,
                Targets = CollectBuildTargets(),
                IsDetectError = checkBox_DetectBuildError.Checked,
                DetectBuildErrorWord = textBox_DetectBuildErrorWord.Text,
                IsExclude = checkBox_IsExclude.Checked,
                IgnoreExecuteFile = textBox_ExcludeWord.Text,
                IsDeleteDirectory = checkBox_DeleteDirectory.Checked,
                DeleteTargetsText = textBox_DeleteDirectoryName.Text,
            };

            textBox_BuildLog.Clear();
            tabControl1.SelectedTab = tabPage4;
            SetBuilding(true);
            BuildWorker.RunWorkerAsync(bwi);
        }

        // ビルド中は「ビルド実行」を押せなくし、ボタンの文字で実行中であることを示す
        private void SetBuilding(Boolean isBuilding)
        {
            button_Build.Enabled = !isBuilding;
            button_Build.Text = isBuilding ? "ビルド中..." : "ビルド実行";
        }

        // ビルド結果欄の末尾に1行足す
        private void AppendBuildLog(String text)
        {
            textBox_BuildLog.AppendText(text + Environment.NewLine);
        }

        private void BuildWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AppendBuildLog((String)e.UserState);
        }

        private void BuildWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない
            // (経過はReportProgressでUIスレッドへ渡し、BuildWorker_ProgressChangedで結果欄に出す)
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            BuildWorkerInfo bwi = (BuildWorkerInfo)e.Argument;
            StringBuilder result = new StringBuilder();

            // ソリューションの存在チェック。以前は見つからないものがあると「処理を継続しますか？」の
            // ポップアップで止まっていた。今は見つからないものを飛ばして「存在しないリスト」に出し、残りをビルドする
            worker.ReportProgress(0, "ソリューションの存在を確認しています...");
            List<String> missingList = Logic.FindMissingSolutions(bwi.Targets, File.Exists);
            result.Append("[存在しないリスト]").Append(Environment.NewLine);
            foreach (String missing in missingList)
            {
                result.Append(missing).Append(Environment.NewLine);
            }
            result.Append(Environment.NewLine);

            List<BuildTarget> buildTargets = new List<BuildTarget>();
            foreach (BuildTarget target in bwi.Targets)
            {
                if (target.IsEnabled && !target.IsBlank
                    && File.Exists(Logic.GetSolutionPathName(target.ProjectPath, target.SolutionName)))
                {
                    buildTargets.Add(target);
                }
            }

            // ビルドログを削除して作り直す(空欄ならログを出さない)
            Boolean isExportLog = bwi.LogDirectory != String.Empty;
            if (isExportLog)
            {
                worker.ReportProgress(0, "ログ出力先を作り直しています: " + bwi.LogDirectory);
                _fio.DeleteDirectoryAndFile(bwi.LogDirectory);
                Directory.CreateDirectory(bwi.LogDirectory);
            }

            if (buildTargets.Count == 0)
            {
                result.Append("ビルドできるソリューションがありません(ビルド欄が○で、ファイルがあるものが0件)").Append(Environment.NewLine);
                e.Result = result.ToString();
                return;
            }

            // バッチ作成・ビルド実行
            StringBuilder script = new StringBuilder(Logic.CreateScriptHeader(bwi.VisualStudioExePath, bwi.BuildOption));
            foreach (BuildTarget target in buildTargets)
            {
                script.Append(Logic.CreateBuildScript(target.BuildEnable, target.SolutionName, target.ProjectPath, bwi.LogDirectory, isExportLog));
            }

            worker.ReportProgress(0, "ビルドしています(" + buildTargets.Count + " 件)...");
            String batchFile = _fio.CreateTempFile("Bat");
            _fio.CreateFile(batchFile, script.ToString());
            _util.ExecutePathWithWait(batchFile);

            // フォルダ・ファイル削除。ビルド全体(全ソリューション)が終わった後に、
            // ビルド対象だった各ソリューションのプロジェクトフォルダ以下を再帰的に探して消す
            // (objフォルダ等、ビルド中はまだ使われているため各ソリューションのビルド直後ではなく
            // 全ビルド完了後にまとめて行う)。判定・削除処理自体はLogic側(テスト可能)に置く
            if (bwi.IsDeleteDirectory)
            {
                String[] invalidTargets;
                String[] deleteTargets = Logic.ParseDeleteTargets(bwi.DeleteTargetsText, out invalidTargets);
                foreach (String invalidTarget in invalidTargets)
                {
                    worker.ReportProgress(0, "[削除しない] パスや全部に当たるパターンは使えません: " + invalidTarget);
                }

                if (deleteTargets.Length != 0)
                {
                    worker.ReportProgress(0, "ビルド後の削除をしています: " + String.Join(", ", deleteTargets));
                    int deletedCount = 0;
                    foreach (BuildTarget target in buildTargets)
                    {
                        deletedCount += Logic.DeleteTargetsByPatterns(target.ProjectPath, deleteTargets);
                    }
                    worker.ReportProgress(0, "削除したフォルダ・ファイル: " + deletedCount + " 件");
                }
            }

            // ビルドエラー検出。振り分けの判定はLogic側(テスト可能)に置き、
            // ログファイルの中身を見る部分だけここから渡す
            if (bwi.IsDetectError)
            {
                String[] targetArray = buildTargets
                    .ConvertAll(target => Logic.GetLogPathName(bwi.LogDirectory, target.ProjectPath, target.SolutionName))
                    .ToArray();
                result.Append(Logic.ClassifyBuildResult(targetArray, bwi.DetectBuildErrorWord,
                                                        bwi.IsExclude, bwi.IgnoreExecuteFile,
                                                        _fio.DetectFileData));
            }
            else
            {
                result.Append("ビルド完了(" + buildTargets.Count + " 件)").Append(Environment.NewLine);
            }
            e.Result = result.ToString();
        }

        // 結果は以前はメッセージボックスに出していたが、ソリューションが多いと画面からはみ出して読めなかった。
        // 「ビルド結果」タブのテキスト欄に出す(スクロール・コピーできる)
        private void BuildWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetBuilding(false);
            AppendBuildLog("");
            if (e.Cancelled)
            {
                // この場合にはe.Resultにはアクセスできない
                AppendBuildLog("キャンセルされました");
            }
            else if (e.Error != null)
            {
                // この場合にはe.Resultにはアクセスできない
                AppendBuildLog("処理が中断されました" + Environment.NewLine + e.Error.Message);
            }
            else
            {
                // 処理結果の表示
                AppendBuildLog((String)e.Result);
            }
            tabControl1.SelectedTab = tabPage4;
        }

        private void textBox_BuildLog_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_BuildLog, e);
        }

        private void InitializeDataGridView()
        {
            // ComboBoxを除く項目数
            dataGridView.ColumnCount = 2;

            // 左端(行ヘッダー)に1始まりの行番号を出す(何件登録されているか・存在しないリストのNo.と突き合わせられるように)。
            // 番号はdataGridView_RowPostPaintで描く
            dataGridView.RowHeadersVisible = true;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.RowHeadersWidth = 44;
            dataGridView.RowPostPaint += dataGridView_RowPostPaint;
            dataGridView.RowsAdded += (s, e) => UpdateRowCount();
            dataGridView.RowsRemoved += (s, e) => UpdateRowCount();
            dataGridView.CellValueChanged += (s, e) => UpdateRowCount();

            // Memo：下記を設定しない場合、最終行（☆がついている行）は削除できないので注意
            // 最下部プロパティを非表示
            dataGridView.AllowUserToAddRows = false;

            // ComboBoxのリスト作成
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            column.Items.Add(Logic.BuildListEnable);
            column.Items.Add(_buildListDisable);
            dataGridView.Columns.Insert(dataGridView.Columns[_buildEnableIdx].Index, column);

            // ヘッダ作成
            dataGridView.Columns[_buildEnableIdx].HeaderText = _headerBuild;
            dataGridView.Columns[_solutionNameIdx].HeaderText = _headerSolution;
            dataGridView.Columns[_projectPathIdx].HeaderText = _headerProjectPath;

            // 幅設定
            SetDataGridViewColumnWidth();
        }

        private String GetCellData(int rowIndex, int columnIndex)
        {
            return dataGridView.Rows[rowIndex].Cells[columnIndex].Value?.ToString() ?? "";
        }

        // 表の左端に行番号(1始まり)を描く
        private void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle bounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dataGridView.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dataGridView.RowHeadersDefaultCellStyle.Font,
                bounds, dataGridView.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        // 表の上に登録数(とビルド対象の数)を出す
        private void UpdateRowCount()
        {
            int enabledCount = 0;
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                if (GetCellData(i, _buildEnableIdx) == Logic.BuildListEnable)
                {
                    enabledCount++;
                }
            }
            label_RowCount.Text = Logic.FormatRowCount(dataGridView.RowCount, enabledCount);
        }

        private void button_AddRow_Click(object sender, EventArgs e)
        {
            // 選択されたRowの一つ下に追加
            dataGridView.Rows.Insert(dataGridView.CurrentRow.Index+1);
        }

        private void SetDataGridViewColumnWidth()
        {
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView.Columns[_buildEnableIdx].Width = 80;
            dataGridView.Columns[_solutionNameIdx].Width = 120;

            // Memo：AutoSizeしてもDataGridViewの全体サイズは取得できなかった
            //int AllColumnWidth = dataGridView.Columns[0].Width * dataGridView.ColumnCount;
            //dataGridView.Columns[2].Width = AllColumnWidth - dataGridView.Columns[0].Width - dataGridView.Columns[1].Width;
        }

        private void textBox_LogDirectory_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(textBox_LogDirectory.Text, e);
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < _buildEnableIdx || e.RowIndex < 0)
            {
                return;
            }

            DataGridView dgv = (DataGridView)sender;
            if (dgv[e.ColumnIndex, e.RowIndex].GetType() == typeof(DataGridViewComboBoxCell))
            {
                dgv.BeginEdit(false);
                var editingControl = (DataGridViewComboBoxEditingControl)dgv.EditingControl;
                editingControl.DroppedDown = true;
            }
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            // キーの種類を見ておらず、どのキーを押してもクリップボードを上書きしていた
            if (!e.Control || e.KeyCode != Keys.C)
            {
                return;
            }

            // 自前でコピー
            _util.SetClipboardData(dataGridView.GetClipboardContent());

            // Memo：DataGridViewの機能でコピー
            //dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        }

        private void checkBox_UpdateGUI(object sender, EventArgs e)
        {
            UpdateOutputGui();
        }

        private void UpdateOutputGui()
        {
            Boolean detectErrorEnable = checkBox_DetectBuildError.Checked;
            Boolean excludeEnable = detectErrorEnable && checkBox_IsExclude.Checked;

            label_DetectBuildErrorWord.Enabled = detectErrorEnable;
            textBox_DetectBuildErrorWord.Enabled = detectErrorEnable;
            checkBox_IsExclude.Enabled = detectErrorEnable;
            label_ExcludeWord.Enabled = excludeEnable;
            textBox_ExcludeWord.Enabled = excludeEnable;
        }

        private void checkBox_DeleteDirectory_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBuildGui();
        }

        private void UpdateBuildGui()
        {
            textBox_DeleteDirectoryName.Enabled = checkBox_DeleteDirectory.Checked;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < _buildEnableIdx || e.RowIndex < 0)
            {
                return;
            }

            String execPath = "";
            switch(e.ColumnIndex)
            {
                case _solutionNameIdx :

                    execPath = Logic.GetSolutionPathName(
                        GetCellData(e.RowIndex, _projectPathIdx),
                        GetCellData(e.RowIndex, _solutionNameIdx));
                    break;

                case _projectPathIdx:
                    execPath = GetCellData(e.RowIndex, _projectPathIdx);
                    break;
                
                case _buildEnableIdx:
                    // no break
                default:
                    return;
            }

            _util.ExecutePath(execPath);
        }

        private void button_AddAllSolution_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show(
                "既存リストをクリアし、ソリューションを一括で登録しますか？",
                "ソリューション一括登録",
                MessageBoxButtons.YesNo))
            {
                MessageBox.Show("処理を中断しました");
                return;
            }

            FormSelectDirectory dir = new FormSelectDirectory();
            DialogResult dr = dir.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                MessageBox.Show("処理を中断しました");
                return;
            }

            String rootDirectory = dir.DirectoryPath;
            if (!Directory.Exists(rootDirectory))
            {
                MessageBox.Show("ディレクトリパスが存在しません" + Environment.NewLine + rootDirectory);
                return;
            }

            // ソリューション追加
            AddAllSolutions(rootDirectory);
        }

        private void AddAllSolutions(String rootDirectory)
        {
            // リスト削除
            dataGridView.Rows.Clear();

            // リストアップ
            String[] solutionPaths = Directory.GetFiles(rootDirectory, "*" + Logic.SlnExtension, SearchOption.AllDirectories);

            // リスト登録
            dataGridView.Rows.Add(solutionPaths.Length);
            for (int i = 0; i < solutionPaths.Length; i++)
            {
                dataGridView.Rows[i].Cells[_buildEnableIdx].Value = Logic.BuildListEnable;
                dataGridView.Rows[i].Cells[_solutionNameIdx].Value = Path.GetFileName(solutionPaths[i]);
                dataGridView.Rows[i].Cells[_projectPathIdx].Value = Path.GetDirectoryName(solutionPaths[i]);
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != _projectPathIdx || e.RowIndex < 0)
            {
                return;
            }

            if ( dataGridView.Rows[e.RowIndex].Cells[_projectPathIdx].Value == null )
            {
                // プロジェクトパスがblankだったら何もしない。
                return;
            }

            if (dataGridView.Rows[e.RowIndex].Cells[_solutionNameIdx].Value == null)
            {
                // ソリューション名が設定されていなかったら、自動設定
                String solutionName = Path.GetFileName((String)dataGridView.Rows[e.RowIndex].Cells[_projectPathIdx].Value) + Logic.SlnExtension;
                dataGridView.Rows[e.RowIndex].Cells[_solutionNameIdx].Value = solutionName;
            }

            if (dataGridView.Rows[e.RowIndex].Cells[_buildEnableIdx].Value == null)
            {
                // ビルド設定がされていなかったら、自動設定
                dataGridView.Rows[e.RowIndex].Cells[_buildEnableIdx].Value = Logic.BuildListEnable;
            }
        }
    }
}
