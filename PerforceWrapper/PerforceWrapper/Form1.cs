﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace PerforceWrapper
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private const String SettingFileName = "PerforceWrapper.json";

        // 設定ファイルはJSONが基本。旧XML(PerforceWrapper.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"PerforceWrapper.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\PerforceWrapper\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String AppName = "PerforceWrapper";
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(AppName);

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private Boolean LoadProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(sr, filePath) : sr.LoadProc(filePath);
        }

        private Boolean SaveProfile(String filePath)
        {
            sr.AddComboBoxHistory(this);
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(sr, filePath) : sr.SaveSetting(filePath);
        }


        private StcFileInputOutput fio = new StcFileInputOutput();
        private Boolean m_IsDebug = false;

        public Form1()
        {
            InitializeComponent();
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            InitializeCommonSettings(Properties.Resources.PerforceWrapper);

            sr.RegisterItem(this);
            String defaultJsonPath = Path.Combine(userDataFolder, SettingFileName);
            String defaultXmlPath = Path.Combine(userDataFolder, LegacySettingFileName);
            JsonSaveRestore.LoadWithMigration(sr, defaultJsonPath, defaultXmlPath,
                path => sr.LoadProc(path));
            util.UpdateProfileList(comboBox_profile, ProfileExtensions, "", userDataFolder);
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

        private void button_profile_save_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_profile, ProfileExtensions, SaveProfile, userDataFolder);
        }

        private void comboBox_profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String loadFileName = Path.Combine(userDataFolder, comboBox_profile.Text);
            LoadProfile(loadFileName);
        }

        private void textBox_tree_list_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private String ExecuteForeground(String script)
        {
            String batchFile = fio.CreateTempFile("bat");
            fio.CreateFile(batchFile, script);

            util.ExecuteProcess(out String output, batchFile);
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

            String batchFile = fio.CreateTempFile("bat");
            fio.CreateFile(batchFile, script);

            // 実行(引数はバッチのパス1つだけなのでそのまま渡す)
            backgroundWorker.RunWorkerAsync(batchFile);   // ⇒DoWork()
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // このメソッドへのパラメータ
            String batchFile = (String)e.Argument;

            // コマンド実行
            util.ExecuteProcess(batchFile);

            // パスワードが含まれるのでファイルを削除する
            //File.Delete(batchFile);

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
            pf.SetUserPass(textbox_perforce_password.Text);
        }

        private void UpdateControlUI(object sender, EventArgs e)
        {
            // 変更リスト番号は「最新を取得」のときだけ、ツリー指定は「ラベル比較」以外のときだけ入力できる
            textBox_so_changelist.Enabled = radioButton_so_menu_get_latest.Checked;
            textBox_tree_list.Enabled = Logic.GetCurrentTabId(tabControl.SelectedIndex) != Logic.TAB_ID.DIFF_LABEL;
        }

        private void button_execute_Click(object sender, EventArgs e)
        {
            Logic.TAB_ID tabId = Logic.GetCurrentTabId(tabControl.SelectedIndex);

            switch(tabId)
            {
            case Logic.TAB_ID.BASE_OPERATION:
                BaseOperationExecute();
                break;

            case Logic.TAB_ID.SET_LABEL:
                SetLabelExecute();
                break;

            case Logic.TAB_ID.DIFF_LABEL:
                DiffLabelExecute();
                break;

            case Logic.TAB_ID.APPLY_LABEL:
                ApplyLabelExecute();
                break;

                default:
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
            Perforce.OPERATOR_TYPE operatorType = Logic.GetOperatorType(
                radioButton_so_menu_checkout.Checked,
                radioButton_so_menu_restore.Checked,
                radioButton_so_menu_delete.Checked,
                radioButton_so_menu_get_latest.Checked);
            pf.SetOperatorType(operatorType);
            if (operatorType == Perforce.OPERATOR_TYPE.SYNC)
            {
                pf.SetRevision(textBox_so_changelist.Text);
            }
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetDebugMode(m_IsDebug);

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
            pf.SetOperatorType(Perforce.OPERATOR_TYPE.SET_LABEL);
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetRevision(textBox_sl_base_changelist.Text);
            pf.SetLabelName(textBox_sl_label_name.Text);
            pf.SetDebugMode(m_IsDebug);

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
            pf.SetOperatorType(Perforce.OPERATOR_TYPE.DIFF);
            pf.SetDebugMode(m_IsDebug);

            // 比較対象
            String command = pf.GetLabelDesignationPathName(textBox_dl_src_tree.Text, textBox_dl_src_label_name.Text);
            command += " ";
            command += pf.GetLabelDesignationPathName(textBox_dl_dest_tree.Text, textBox_dl_dest_label_name.Text);

            // 比較結果格納
            String diffFile = fio.CreateTempFile();
            command += " > " + diffFile;

            ExecuteForeground(pf.CreateCommandDefined(command));

            // 比較結果検証
            FileInfo diffFileInfo = new FileInfo(diffFile);
            if ( 0 < diffFileInfo.Length)
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
            Perforce.OPERATOR_TYPE operatorType = Perforce.OPERATOR_TYPE.COPY;
            if (radioButton_al_merge.Checked)
            {
                operatorType = Perforce.OPERATOR_TYPE.MERGE;
            }
            pf.SetOperatorType(operatorType);
            pf.SetRevision(textBox_al_label_name.Text);
            pf.SetBranchMapName(textBox_al_branch_map.Text);
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetDebugMode(m_IsDebug);

            Execute(pf.CreateCommandUseTree());
        }

        private void label16_DoubleClick(object sender, EventArgs e)
        {
            m_IsDebug = !m_IsDebug;
            MessageBox.Show("DebugMode=" + m_IsDebug.ToString());
        }
    }
}
