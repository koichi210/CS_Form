using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Runtime.InteropServices;
using StandardTemplate;

namespace FileArranger
{
    partial class FileArranger : StcBaseForm<SaveRestore>
    {
        readonly String SettingFileNameXml = @"FileArranger.xml";
        readonly String SettingFileNameJson = @"FileArranger.json";

        // プロファイル(FileArranger.xml/.json)の置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\FileArranger\配下(既定)にあり、exe直下にはその場所を示す
        // 小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common/UserDataLocation.cs]]、EventRecorderと同じ仕組み)
        private const String AppName = "FileArranger";
        readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(AppName);

        private readonly String[] RenameDirColumn = { "変更前", "変更後" };
        private readonly String[] PartitionFileColumn = { "対象", "移動前名称", "移動後名称" };

        private readonly int RenameSrcIdx = 0;
        private readonly int RenameDestIdx = 1;

        private readonly int CreateFolderTargetIdx = 0;
        private readonly int CreateFolderMoveSrcIdx = 1;
        private readonly int CreateFolderMoveDestIdx = 2;

        private readonly int SortFileRenameTargetIdx = 0;

        public String[] ReferenceCandidateFolders;      // リファレンス名の候補

        private StcFileInputOutput fio = new StcFileInputOutput();
        // StcBaseForm<SaveRestore>のprotected StcUtils utilを、FileArranger固有の拡張
        // メソッド(CreateFolderNameOverLapShirk等)を持つUtilsで意図的に隠す。
        // UtilsはStcUtilsを継承しているだけなので、既存のutil.ExecutePath()等の呼び出しは
        // そのまま継承元のメソッドとして動く。
        private new Utils util = new Utils();
        private StcProcessMemory pmd = new StcProcessMemory();
        private FileSorter sorter = new FileSorter();

        public FileArranger()
        {
            InitializeComponent();
            InitializeCommonSettings(Properties.Resources.FileArranger);

            //ListView初期設定
            rd_listView_Target_Update();
            pf_listView_Target_Update();

            sr.RegistLoadItem(this);

            // 起動時はJSONを読む。旧XMLしか無ければ読み込んでJSONへ保存し直し、旧XMLは削除する
            // ([[_Common/JsonSaveRestore.cs]])
            String defaultJsonPath = Path.Combine(userDataFolder, SettingFileNameJson);
            String defaultXmlPath = Path.Combine(userDataFolder, SettingFileNameXml);
            JsonSaveRestore.LoadWithMigration(sr, defaultJsonPath, defaultXmlPath, LoadProfileFromXml);

            UpdateProfileListAll("");
        }

        // *******************************************************************************
        // JSON保存/読込([[_Common/JsonFileStorage.cs]])。設定値はこれまでXML(StcSaveRestore)
        // 一本だったが、今後はJSONへ段階的に移行していく方針のため、拡張子で振り分ける
        // (EventRecorderと同じ考え方)

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 設定ファイルを拡張子で振り分けて読み込む。拡張子がjsonならJSON、それ以外は従来通りXML
        private Boolean LoadProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                return sr.LoadJsonFile(filePath, this);
            }

            return sr.LoadProc(filePath, this);
        }

        // 旧XMLの読み込み(移行用)。JsonSaveRestore.LoadWithMigrationへ渡す
        private Boolean LoadProfileFromXml(String path)
        {
            return sr.LoadProc(path, this);
        }

        // 設定ファイルを拡張子で振り分けて保存する
        private Boolean SaveProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                return sr.SaveJsonFile(filePath, this);
            }

            return sr.SaveSetting(filePath, this);
        }

        // comboBox_LoadSettingへ、userDataFolder配下の*.xmlと*.jsonの両方をまとめてリストアップする。
        // util.UpdateProfileListは拡張子を1パターンしか指定できないため、2回検索した結果をマージする
        private void UpdateProfileListAll(String defaultProfileName)
        {
            String[] xmlFiles = Directory.GetFiles(userDataFolder, "*.xml", SearchOption.AllDirectories);
            String[] jsonFiles = Directory.GetFiles(userDataFolder, "*.json", SearchOption.AllDirectories);
            String[] files = xmlFiles.Concat(jsonFiles).ToArray();

            util.SetComboBoxFromArray(comboBox_LoadSetting, files, userDataFolder);
            util.SetComboBoxText(comboBox_LoadSetting, defaultProfileName);
        }

        private void cmn_textBox_AddList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void SaveSetting_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(comboBox_LoadSetting.Text, userDataFolder);
            if (String.IsNullOrEmpty(SaveFileName))
            {
                return;
            }

            if (SaveProfile(SaveFileName))
            {
                UpdateProfileListAll(Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
            }
        }

        // Ctrl+Sで「設定値保存」ボタンと同じ動作にする(テキストボックス等にフォーカスがあっても拾える)
        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                SaveSetting_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // *******************************************************************************
        // データ保存先フォルダの変更(システムメニューから呼び出す)
        // ([[EventRecorder/Form1.cs]]の同名機能と同じ考え方)
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

        private void comboBox_LoadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Path.Combine(userDataFolder, comboBox_LoadSetting.Text);
            LoadProfile(LoadFileName);
        }

        private void FileArranger_ResizeEnd(object sender, EventArgs e)
        {
            rd_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            pf_listView_Target_Update();
        }

        private void cmn_textBox_Reference_TextChanged(object sender, EventArgs e)
        {
            rd_textBox_ExistItemDir.Text = cmn_textBox_Reference.Text;
            pf_textBox_ReferenceFile.Text = cmn_textBox_Reference.Text;
        }

        private void cmn_button_Listup_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(cmn_textBox_Reference.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + cmn_textBox_Reference.Text);
                return;
            }

            // フォルダをリストアップ
            ReferenceCandidateFolders = Directory.GetDirectories(cmn_textBox_Reference.Text);

            // 新規追加
            if ( !cmn_textBox_AddList.Text.Equals(String.Empty) )
            {
                String[] AddReferenceList = cmn_textBox_AddList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                Logic.DeleteDuplicate(ReferenceCandidateFolders, ref AddReferenceList, rd_textBox_SplitWord3.Text);
                AddReferenceList = AddReferenceList.Select(str => cmn_textBox_Reference.Text + @"\" + str + cmn_textBox_AddListSuffix.Text).ToArray();

                String[] SumReferenceList = new String[ReferenceCandidateFolders.Length + AddReferenceList.Length];
                ReferenceCandidateFolders.CopyTo(SumReferenceList, 0);
                AddReferenceList.CopyTo(SumReferenceList, ReferenceCandidateFolders.Length);
                ReferenceCandidateFolders = SumReferenceList;
            }

            // コンボボックス更新
            UpdateRenameComboBox();
            UpdateMoveDestDirComboBox();
        }

    }
}
