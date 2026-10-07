using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public class BuildWorkerInfo
        {
            public String BuildScript { get; set; }
            public Boolean IsDetectError { get; set; }
            public String DetectBuildErrorWord { get; set; }
            public Boolean IsExclude { get; set; }
            public String IgnoreExecuteFile { get; set; }
            public Boolean IsDeleteDirectory { get; set; }
            public String DeleteDirectoryName { get; set; }
            public String[] ProjectPathList { get; set; }
            public String DetectTargetLogList { get; set; }
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

            _sr.RegisterItem(this);
            String defaultJsonPath = Path.Combine(_userDataFolder, _defaultSettingFileName);
            String defaultXmlPath = Path.Combine(_userDataFolder, _legacySettingFileName);
            JsonSaveRestore.LoadWithMigration(_sr, defaultJsonPath, defaultXmlPath,
                path => _sr.LoadProc(path, this));
            UpdateBuildGui();
            UpdateOutputGui();
            _util.UpdateProfileList(comboBox_Profile, _profileExtensions, _defaultSettingFileName, _userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnly・Multilineの欄は無いので全欄が対象
        // (既定値は[[SaveRestore.cs]]のRegisterCtrlに合わせた)
        private void InitializePlaceholders()
        {
            textBox_VisualStudioExePath.PlaceholderText = "例: " + Logic.DefaultMsBuildPath;
            textBox_BuildOption.PlaceholderText = "例: " + Logic.DefaultBuildOption;
            textBox_DeleteDirectoryName.PlaceholderText = "例: obj";
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
            toolTip.SetToolTip(textBox_VisualStudioExePath, "ビルドに使うMSBuild.exeのパス。Visual Studio本体が無くても、Build Tools for Visual Studioに入っているMSBuild.exeでビルドできる");
            toolTip.SetToolTip(textBox_BuildOption, "MSBuildにそのまま渡す引数(ターゲットと構成名。例: /t:Rebuild /p:Configuration=Release /p:Platform=x64)");
            toolTip.SetToolTip(button_Build, "ビルド欄が○のソリューションを順にビルドする。開始時にログ出力先フォルダを中身ごと削除する");
            toolTip.SetToolTip(checkBox_DeleteDirectory, "オンならビルド完了後、ビルド対象の各プロジェクトフォルダ以下(サブフォルダ含む)から下の欄の名前と一致するフォルダを探して削除する");

            toolTip.SetToolTip(textBox_LogDirectory, "ソリューションごとのビルドログ(ソリューション名.log)の出力先。ビルド開始時に中身ごと削除して作り直す。空欄ならログを出さない。Enterキーでフォルダを開く");
            toolTip.SetToolTip(checkBox_DetectBuildError, "ビルド後に各ログを検知ワードで調べ、成功/失敗の一覧を表示する");
            toolTip.SetToolTip(textBox_DetectBuildErrorWord, "この文字列を含むログをビルド失敗と判定する");
            toolTip.SetToolTip(checkBox_IsExclude, "オンなら下の検知ワードを含むログを成功扱いにせず、「実行ファイルの上書きに失敗」として別枠で表示する");
            toolTip.SetToolTip(textBox_ExcludeWord, "この文字列を含むログを、実行中のexeを差し替えられなかったものとして別枠に分ける");

            toolTip.SetToolTip(button_AddAllSolution, "指定フォルダ配下(サブフォルダ含む)の.slnを全て登録する。今の一覧は消える");
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
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, _appName));
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

        private String CreateDetectTargetList()
        {
            var detectTargetLogList = new StringBuilder();

            if (checkBox_DetectBuildError.Checked)
            {
                for (int i = 0; i < dataGridView.RowCount; i++)
                {
                    if (GetCellData(i, _buildEnableIdx) != Logic.BuildListEnable)
                    {
                        continue;
                    }

                    String solutionName = GetCellData(i, _solutionNameIdx);
                    detectTargetLogList.Append(Logic.GetLogPathName(textBox_LogDirectory.Text, solutionName));
                    detectTargetLogList.Append(Environment.NewLine);
                }
            }

            return _util.TrimEndGarbage(detectTargetLogList.ToString());
        }

        // ビルド対象(○)のプロジェクトフォルダ一覧。ビルド後のディレクトリ削除で、
        // どのソリューションの配下を掃除するかに使う
        private String[] CreateEnabledProjectPathList()
        {
            List<String> projectPathList = new List<String>();

            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                if (GetCellData(i, _buildEnableIdx) != Logic.BuildListEnable)
                {
                    continue;
                }

                String projectPath = GetCellData(i, _projectPathIdx);
                if (projectPath != String.Empty)
                {
                    projectPathList.Add(projectPath);
                }
            }

            return projectPathList.ToArray();
        }

        private Boolean CheckSolutionPath()
        {
            var fileNotFoundList = new StringBuilder();

            // ファイルパスチェック
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                String solutionName = GetCellData(i, _solutionNameIdx);
                String projectPath = GetCellData(i, _projectPathIdx);
                String solutionPath = Logic.GetSolutionPathName(projectPath, solutionName);

                if (!File.Exists(solutionPath))
                {
                    // 存在しないパス
                    fileNotFoundList.Append("No").Append(i).Append(' ').Append(solutionName).Append(Environment.NewLine);
                }
            }

            if (fileNotFoundList.Length > 0)
            {
                DialogResult dlgResult = MessageBox.Show(
                    "いくつかのソリューションが見つかりませんでした。" +
                    "処理を継続しますか？" + Environment.NewLine + fileNotFoundList.ToString(),
                    "Warning",
                    MessageBoxButtons.YesNo);
                if (dlgResult == DialogResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        private void button_Build_Click(object sender, EventArgs e)
        {
            // ビルド中に押されると、実行中のビルドのログフォルダを消してしまうため先に弾く
            if (BuildWorker.IsBusy)
            {
                MessageBox.Show("ビルド実行中です。終わってからもう一度押してください");
                return;
            }

            // MSBuild.exeが無いとバッチが即エラーで終わり、ログフォルダだけ消えてしまうため、
            // ログ削除やバッチ作成より前に確かめる
            String msBuildPathError = Logic.ValidateMsBuildPath(textBox_VisualStudioExePath.Text, File.Exists);
            if (msBuildPathError != String.Empty)
            {
                MessageBox.Show(msBuildPathError, "Warning");
                return;
            }

            if (!CheckSolutionPath())
            {
                return;
            }

            // ビルドログを削除
            _fio.DeleteDirectoryAndFile(textBox_LogDirectory.Text);

            // 以前はList<object>へ順番に詰めてDoWork側で位置で取り出していたが、
            // 並び順がずれると気づきにくいため、専用クラスをそのまま渡す
            BuildWorkerInfo bwi = new BuildWorkerInfo
            {
                BuildScript = CreateAllBuildScript(),
                IsDetectError = checkBox_DetectBuildError.Checked,
                DetectBuildErrorWord = textBox_DetectBuildErrorWord.Text,
                IsExclude = checkBox_IsExclude.Checked,
                IgnoreExecuteFile = textBox_ExcludeWord.Text,
                IsDeleteDirectory = checkBox_DeleteDirectory.Checked,
                DeleteDirectoryName = textBox_DeleteDirectoryName.Text,
                ProjectPathList = CreateEnabledProjectPathList(),
                DetectTargetLogList = CreateDetectTargetList(),
            };

            BuildWorker.RunWorkerAsync(bwi);
        }

        private void BuildWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // このメソッドへのパラメータ
            BuildWorkerInfo bwi = (BuildWorkerInfo)e.Argument;

            // ビルド実行
            String batchFile = _fio.CreateTempFile("Bat");
            _fio.CreateFile(batchFile, bwi.BuildScript);
            _util.ExecutePathWithWait(batchFile);

            // ディレクトリ削除。ビルド全体(全ソリューション)が終わった後に、
            // ビルド対象だった各ソリューションのプロジェクトフォルダ以下を再帰的に探して消す
            // (objフォルダ等、ビルド中はまだ使われているため各ソリューションのビルド直後ではなく
            // 全ビルド完了後にまとめて行う)。判定・削除処理自体はLogic側(テスト可能)に置く
            if (bwi.IsDeleteDirectory && bwi.DeleteDirectoryName != String.Empty)
            {
                foreach (String projectPath in bwi.ProjectPathList)
                {
                    Logic.DeleteDirectoriesByName(projectPath, bwi.DeleteDirectoryName);
                }
            }

            // ビルドエラー検出。振り分けの判定はLogic側(テスト可能)に置き、
            // ログファイルの中身を見る部分だけここから渡す
            if (bwi.IsDetectError)
            {
                String[] targetArray = _util.ChangeStrLinear2Array(bwi.DetectTargetLogList, Environment.NewLine);
                e.Result = Logic.ClassifyBuildResult(targetArray, bwi.DetectBuildErrorWord,
                                                     bwi.IsExclude, bwi.IgnoreExecuteFile,
                                                     _fio.DetectFileData);
            }
            else
            {
                e.Result = "ビルド完了";
            }
        }

        private void BuildWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // この場合にはe.Resultにはアクセスできない
                MessageBox.Show("キャンセルされました");
            }
            else if (e.Error != null)
            {
                // この場合にはe.Resultにはアクセスできない
                MessageBox.Show("処理が中断されました" + Environment.NewLine + e.Error.Message);
            }
            else
            {
                // 処理結果の表示
                MessageBox.Show((String)e.Result);
            }
        }

        private void InitializeDataGridView()
        {
            // ComboBoxを除く項目数
            dataGridView.ColumnCount = 2;

            // 左端プロパティを非表示
            dataGridView.RowHeadersVisible = false;

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

        private String CreateAllBuildScript()
        {
            Boolean isExportLog = false;
            if (textBox_LogDirectory.Text != String.Empty)
            {
                _fio.EnsureDirectory(textBox_LogDirectory.Text, true);
                isExportLog = true;
            }

            // ヘッダー生成
            var script = new StringBuilder(Logic.CreateScriptHeader(textBox_VisualStudioExePath.Text, textBox_BuildOption.Text));

            // GridDataからビルド設定コマンド生成
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                script.Append(CreateBuildScript(i, isExportLog));
            }

            return script.ToString();
        }

        private String GetCellData(int rowIndex, int columnIndex)
        {
            return dataGridView.Rows[rowIndex].Cells[columnIndex].Value?.ToString() ?? "";
        }

        private String CreateBuildScript(int rowIndex, Boolean isExportLog)
        {
            String buildEnable = GetCellData(rowIndex, _buildEnableIdx);
            String solutionName = GetCellData(rowIndex, _solutionNameIdx);
            String projectPath = GetCellData(rowIndex, _projectPathIdx);

            return Logic.CreateBuildScript(buildEnable, solutionName, projectPath, textBox_LogDirectory.Text, isExportLog);
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
