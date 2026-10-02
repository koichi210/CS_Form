using System;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace TrimFileData
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private readonly String SettingFileName = @"TrimFileData.json";

        // 設定ファイルはJSONが基本。旧XML(TrimFileData.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"TrimFileData.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\TrimFileData\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String AppName = "TrimFileData";
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
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(sr, filePath) : sr.SaveSetting(filePath);
        }

        private StcFileInputOutput fio = new StcFileInputOutput();

        public Form1()
        {
            InitializeComponent();

            InitializeCommonSettings(Properties.Resources.TrimFileData);

            InitializePlaceholders();
            InitializeToolTips();

            sr.RegisterItem(this);
            String defaultJsonPath = Path.Combine(userDataFolder, SettingFileName);
            String defaultXmlPath = Path.Combine(userDataFolder, LegacySettingFileName);
            JsonSaveRestore.LoadWithMigration(sr, defaultJsonPath, defaultXmlPath,
                path => sr.LoadProc(path));
            util.UpdateProfileList(comboBox_LoadSetting, ProfileExtensions, SettingFileName, userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(textBox_SearchWordList、Windowsの仕様で表示されない)と、
        // 検索結果の出力欄(textBox_SearchResultList、Multilineでもある)は対象外
        private void InitializePlaceholders()
        {
            textBox_ReferencePath.PlaceholderText = @"例: C:\Work\reference.txt";
            textBox_SearchCommonWord.PlaceholderText = "例: error";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_ReferencePath, "検索対象のテキストファイル(Shift_JISとして読み込む)。Enterキーで関連付けられたアプリで開く");
            toolTip.SetToolTip(textBox_SearchCommonWord, "指定するとこの文字列を含む行だけを検索対象にする。空欄なら全行が対象");
            toolTip.SetToolTip(textBox_SearchWordList, "1行が1グループ。行内をスペースで区切った各ワードを含む行を抽出し、行ごとに「◆」見出しを付けて出力する");
            toolTip.SetToolTip(checkBox_FirstWordOnly, "各ワードについて、最初に見つかった1行だけを抽出する");
            toolTip.SetToolTip(button_Execute, "検索結果を検索結果欄に出し、クリップボードにもコピーする");
            toolTip.SetToolTip(comboBox_LoadSetting, "選ぶと保存済みの設定を読み込み、各欄の内容が置き換わる");
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

        private void textBox_SearchWordList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_SearchWordList, e);
        }

        private void textBox_SearchResultList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_SearchResultList, e);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            // 出力先をクリア
            textBox_SearchResultList.Text = "";

            String referData = fio.LoadFile(textBox_ReferencePath.Text);
            if (referData == String.Empty)
            {
                MessageBox.Show("リファレンスファイルが開けません。" + Environment.NewLine + textBox_ReferencePath.Text);
                return;
            }

            // 検索ワードをリストアップ
            String[] searchWordLines = textBox_SearchWordList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // リファレンスをリスト化
            String[] referLines = referData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // 検索結果をコントロールにセット
            textBox_SearchResultList.Text = Logic.GetSearchData(searchWordLines, referLines, checkBox_OrdinalCase.Checked, checkBox_FirstWordOnly.Checked, textBox_SearchCommonWord.Text);
            util.SetClipboardText(textBox_SearchResultList.Text);
        }

        private void textBox_ReferencePath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                util.ExecutePath(textBox_ReferencePath.Text);
            }
        }

        private void comboBox_LoadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            String loadFileName = Path.Combine(userDataFolder, comboBox_LoadSetting.Text);
            if (File.Exists(loadFileName))
            {
                LoadProfile(loadFileName);
            }
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_LoadSetting, ProfileExtensions, SaveProfile, userDataFolder);
        }
    }
}
