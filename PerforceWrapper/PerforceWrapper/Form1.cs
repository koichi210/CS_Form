using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace PerforceWrapper
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private const String _settingFileName = "PerforceWrapper.json";

        // 設定ファイルはJSONが基本。旧XML(PerforceWrapper.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String _legacySettingFileName = @"PerforceWrapper.xml";
        private static readonly String[] _profileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\PerforceWrapper\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String _appName = "PerforceWrapper";
        private readonly String _userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(_appName);

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private Boolean LoadProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(_sr, filePath) : _sr.LoadProc(filePath);
        }

        private Boolean SaveProfile(String filePath)
        {
            _sr.AddComboBoxHistory(this);
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(_sr, filePath) : _sr.SaveSetting(filePath);
        }

        private StcFileInputOutput _fio = new StcFileInputOutput();
        private Boolean _isDebug = false;

        public Form1()
        {
            InitializeComponent();
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            InitializeCommonSettings(Properties.Resources.PerforceWrapper);

            InitializePlaceholders();
            InitializeToolTips();

            _sr.RegisterItem(this);
            String defaultJsonPath = Path.Combine(_userDataFolder, _settingFileName);
            String defaultXmlPath = Path.Combine(_userDataFolder, _legacySettingFileName);
            JsonSaveRestore.LoadWithMigration(_sr, defaultJsonPath, defaultXmlPath,
                path => _sr.LoadProc(path));
            _util.UpdateProfileList(comboBox_profile, _profileExtensions, "", _userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(textBox_tree_list、Windowsの仕様で表示されない)と、
        // パスワード欄(textBox_perforce_password、入力例を出す意味が無い)は対象外
        private void InitializePlaceholders()
        {
            textBox_so_changelist.PlaceholderText = "例: 12345";

            textBox_sl_label_name.PlaceholderText = "例: REL_1_0";
            textBox_sl_base_changelist.PlaceholderText = "例: 12345";

            textBox_dl_src_label_name.PlaceholderText = "例: REL_1_0";
            textBox_dl_src_tree.PlaceholderText = "例: //depot/main/...";
            textBox_dl_dest_label_name.PlaceholderText = "例: REL_1_1";
            textBox_dl_dest_tree.PlaceholderText = "例: //depot/release/...";

            textBox_al_label_name.PlaceholderText = "例: REL_1_0";
            textBox_al_branch_map.PlaceholderText = "例: main_to_release";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(comboBox_profile, "保存済みのプロファイル。選ぶとその内容を読み込む");
            toolTip.SetToolTip(comboBox_perforce_server, "P4PORTに設定する値。空欄ならPC側の既定設定を使う");
            toolTip.SetToolTip(comboBox_perforce_user, "P4USERに設定する値。空欄ならPC側の既定設定を使う");
            toolTip.SetToolTip(comboBox_perforce_workspace, "P4CLIENTに設定する値。空欄ならPC側の既定設定を使う");
            toolTip.SetToolTip(comboBox_perforce_charset, "P4CHARSETに設定する値。空欄ならPC側の既定設定を使う");
            toolTip.SetToolTip(textBox_perforce_password, "ユーザーとパスワードが両方入っていると、実行前にp4 loginする。空欄ならログインしない");
            toolTip.SetToolTip(textBox_tree_list, "対象のパス。1行に1つ書く。末尾に「...」が無ければ付けて配下全体を対象にする。「ラベルで比較」では使わない");

            toolTip.SetToolTip(radioButton_so_menu_checkout, "ツリー配下を編集用に開く(p4 edit)");
            toolTip.SetToolTip(radioButton_so_menu_restore, "ツリー配下の未サブミットの変更を破棄する(p4 revert)");
            toolTip.SetToolTip(radioButton_so_menu_delete, "ツリー配下のファイルを削除としてマークする(p4 delete)。サブミットはしない");
            toolTip.SetToolTip(radioButton_so_menu_get_latest, "ツリー配下をワークスペースに取得する(p4 sync)");
            toolTip.SetToolTip(textBox_so_changelist, "取得するチェンジリスト番号。空欄なら最新(#head)を取得する");

            toolTip.SetToolTip(textBox_sl_base_changelist, "ラベルを付けるチェンジリスト番号(p4 tag)。空欄なら最新(#head)に付ける");

            toolTip.SetToolTip(textBox_dl_src_tree, "比較するパス。末尾に「...」が無ければ付けて配下全体を比較する");
            toolTip.SetToolTip(textBox_dl_dest_tree, "比較するパス。末尾に「...」が無ければ付けて配下全体を比較する");

            toolTip.SetToolTip(textBox_al_label_name, "反映元として使うラベル。空欄なら最新(#head)を反映する");
            toolTip.SetToolTip(textBox_al_branch_map, "反映に使うブランチマップ名(-bに渡す)");
            toolTip.SetToolTip(radioButton_al_copy, "反映先を反映元と同じ内容にする(p4 copy)");
            toolTip.SetToolTip(radioButton_al_merge, "反映元の変更を統合する(p4 integrate)。resolveは別途行う");
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

        private void button_profile_save_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(_util, _fio, comboBox_profile, _profileExtensions, SaveProfile, _userDataFolder);
        }

        private void comboBox_profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String loadFileName = Path.Combine(_userDataFolder, comboBox_profile.Text);
            LoadProfile(loadFileName);
        }

        private void textBox_tree_list_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private String ExecuteForeground(String script)
        {
            String batchFile = _fio.CreateTempFile("bat");
            _fio.CreateFile(batchFile, script);

            _util.ExecuteProcess(out String output, batchFile);
            return output;
        }

        private void Execute(String script)
        {
            // 実行中に押されると RunWorkerAsync が例外になり、パスワード入りのバッチだけが残ってしまうため先に弾く
            if (backgroundWorker.IsBusy)
            {
                MessageBox.Show("実行中です。終わってからもう一度押してください");
                return;
            }

            String batchFile = _fio.CreateTempFile("bat");
            _fio.CreateFile(batchFile, script);

            // 実行(引数はバッチのパス1つだけなのでそのまま渡す)
            backgroundWorker.RunWorkerAsync(batchFile);   // ⇒DoWork()
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // このメソッドへのパラメータ
            String batchFile = (String)e.Argument;

            try
            {
                // コマンド実行(バッチ完了後にパスワード入りファイルを安全に削除するため、終了を待つ)
                using (Process process = _util.ExecuteProcess(batchFile))
                {
                    process.WaitForExit();
                }
            }
            finally
            {
                // パスワードが含まれるので、プロセス実行後は失敗時も含め必ず削除する
                File.Delete(batchFile);
            }

            // このメソッドからの戻り値
            e.Result = "SUCCESS";
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("コマンドの実行に失敗しました" + Environment.NewLine + e.Error.Message);
            }
        }

        private void SetPerforceEnv(Perforce pf)
        {
            pf.SetServerName(comboBox_perforce_server.Text);
            pf.SetWorkspace(comboBox_perforce_workspace.Text);
            pf.SetCharset(comboBox_perforce_charset.Text);
            pf.SetUserName(comboBox_perforce_user.Text);
            pf.SetUserPass(textBox_perforce_password.Text);
        }

        private void UpdateControlUI(object sender, EventArgs e)
        {
            // 変更リスト番号は「最新を取得」のときだけ、ツリー指定は「ラベル比較」以外のときだけ入力できる
            textBox_so_changelist.Enabled = radioButton_so_menu_get_latest.Checked;
            textBox_tree_list.Enabled = Logic.GetCurrentTabId(tabControl.SelectedIndex) != Logic.TabId.DiffLabel;
        }

        private void button_execute_Click(object sender, EventArgs e)
        {
            Logic.TabId tabId = Logic.GetCurrentTabId(tabControl.SelectedIndex);

            switch (tabId)
            {
                case Logic.TabId.BaseOperation:
                    BaseOperationExecute();
                    break;

                case Logic.TabId.SetLabel:
                    SetLabelExecute();
                    break;

                case Logic.TabId.DiffLabel:
                    DiffLabelExecute();
                    break;

                case Logic.TabId.ApplyLabel:
                    ApplyLabelExecute();
                    break;
            }
        }

        private void BaseOperationExecute()
        {
            if (textBox_tree_list.Text == String.Empty)
            {
                MessageBox.Show("ツリーが指定されていません");
                return;
            }

            Perforce pf = new Perforce();
            SetPerforceEnv(pf);

            // 選択された操作のコマンド生成
            Perforce.OperatorType operatorType = Logic.GetOperatorType(
                radioButton_so_menu_checkout.Checked,
                radioButton_so_menu_restore.Checked,
                radioButton_so_menu_delete.Checked,
                radioButton_so_menu_get_latest.Checked);
            pf.SetOperatorType(operatorType);
            if (operatorType == Perforce.OperatorType.Sync)
            {
                pf.SetRevision(textBox_so_changelist.Text);
            }
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetDebugMode(_isDebug);

            Execute(pf.CreateCommandUseTree());
        }

        private void SetLabelExecute()
        {
            if (textBox_tree_list.Text == String.Empty)
            {
                MessageBox.Show("ツリーが指定されていません");
                return;
            }
            if (textBox_sl_label_name.Text == String.Empty)
            {
                MessageBox.Show("ラベル名が指定されていません");
                return;
            }

            Perforce pf = new Perforce();
            SetPerforceEnv(pf);

            // 選択された操作のコマンド生成
            pf.SetOperatorType(Perforce.OperatorType.SetLabel);
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetRevision(textBox_sl_base_changelist.Text);
            pf.SetLabelName(textBox_sl_label_name.Text);
            pf.SetDebugMode(_isDebug);

            Execute(pf.CreateCommandUseTree());
        }

        private void DiffLabelExecute()
        {
            if (textBox_dl_src_label_name.Text == String.Empty ||
                textBox_dl_src_tree.Text == String.Empty ||
                textBox_dl_dest_label_name.Text == String.Empty ||
                textBox_dl_dest_tree.Text == String.Empty)
            {
                MessageBox.Show("設定されていない項目があります。" + Environment.NewLine + 
                    "比較元 ラベル名：" + textBox_dl_src_label_name.Text + Environment.NewLine +
                    "比較元 ツリー：" + textBox_dl_src_tree.Text + Environment.NewLine +
                    "比較先 ラベル名：" + textBox_dl_dest_label_name.Text + Environment.NewLine +
                    "比較先 ツリー：" + textBox_dl_dest_tree.Text + Environment.NewLine );
                return;
            }

            Perforce pf = new Perforce();
            SetPerforceEnv(pf);

            // 選択された操作のコマンド生成
            pf.SetOperatorType(Perforce.OperatorType.Diff);
            pf.SetDebugMode(_isDebug);

            // 比較対象
            // 比較結果格納
            String diffFile = _fio.CreateTempFile();
            String command =
                pf.GetLabelDesignationPathName(textBox_dl_src_tree.Text, textBox_dl_src_label_name.Text) + " " +
                pf.GetLabelDesignationPathName(textBox_dl_dest_tree.Text, textBox_dl_dest_label_name.Text) +
                " > " + diffFile;

            ExecuteForeground(pf.CreateCommandDefined(command));

            // 比較結果検証
            if (new FileInfo(diffFile).Length > 0)
            {
                MessageBox.Show("差分があります", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("差分はありません");
            }
        }

        private void ApplyLabelExecute()
        {
            if (textBox_al_branch_map.Text == String.Empty)
            {
                MessageBox.Show("ブランチマップが指定されていません");
                return;
            }

            Perforce pf = new Perforce();
            SetPerforceEnv(pf);

            // 選択された操作のコマンド生成
            pf.SetOperatorType(radioButton_al_merge.Checked ? Perforce.OperatorType.Merge : Perforce.OperatorType.Copy);
            pf.SetRevision(textBox_al_label_name.Text);
            pf.SetBranchMapName(textBox_al_branch_map.Text);
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetDebugMode(_isDebug);

            Execute(pf.CreateCommandUseTree());
        }

        private void label16_DoubleClick(object sender, EventArgs e)
        {
            _isDebug = !_isDebug;
            MessageBox.Show("DebugMode=" + _isDebug.ToString());
        }
    }
}
