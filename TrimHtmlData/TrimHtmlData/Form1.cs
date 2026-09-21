using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using StandardTemplate;

namespace TrimHtmlData
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private Boolean IsDebug = false;
        private readonly String SettingFile = @"TrimHtmlData.json";

        // 設定ファイルはJSONが基本。旧XML(TrimHtmlData.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"TrimHtmlData.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\TrimHtmlData\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String AppName = "TrimHtmlData";
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

            InitializeCommonSettings(Properties.Resources.TrimHtmlData);

            sr.RegistItem(this);
            String defaultJsonPath = Path.Combine(userDataFolder, SettingFile);
            String defaultXmlPath = Path.Combine(userDataFolder, LegacySettingFileName);
            JsonSaveRestore.LoadWithMigration(sr, defaultJsonPath, defaultXmlPath,
                path => sr.LoadProc(path));
            util.UpdateProfileList(ref comboBox_LoadSetting, ProfileExtensions, SettingFile, userDataFolder);
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

        private void textBox_SourceList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_SourceList, e);
        }

        private void textBox_DestList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_DestList, e);
        }

        private void textBox_SearchWord_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_SearchWord, e);
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            IsDebug = !IsDebug;
            MessageBox.Show("IsDebug=" + IsDebug.ToString());
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            // 出力先をクリア
            textBox_DestList.Text = "";

            int TrimLineNum = Logic.GetTrimLine(textBox_TrimLineNum.Text);

            StringComparison CmpOpt = StringComparison.OrdinalIgnoreCase;
            if (checkBox_OrdinalCase.Checked)
            {
                CmpOpt = StringComparison.Ordinal;
            }

            StcDebug dbg = new StcDebug();
            dbg.SetDebugMode(IsDebug);

            String[] SourceArray = textBox_SourceList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < SourceArray.Length; i++)
            {
                textBox_DestList.Text += "◆" + SourceArray[i] + Environment.NewLine;

                String HtmlSource = GetHtmlSource(SourceArray[i]);
                dbg.WriteDataInNewFile(HtmlSource, "_1_source");

                String Result = Logic.GetSearchString(HtmlSource, textBox_SearchWord.Text, TrimLineNum, CmpOpt, checkBox_FirstWordOnly.Checked);
                dbg.WriteDataInNewFile(Result, "_2_search");

                textBox_DestList.Text += Result;
            }

            util.SetClipboardText(textBox_DestList.Text);
        }

        /// <summary>
        /// Htmlのソースを取得
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        private String GetHtmlSource(String Url)
        {
            String HtmlSource = "";
            WebClient client = new WebClient();
            try
            {
                client.Encoding = System.Text.Encoding.UTF8;
                HtmlSource = client.DownloadString(Url);
            }
            catch (Exception)
            {
                MessageBox.Show("ソース取得に失敗しました。" + Environment.NewLine + Url);
            }

            return util.ChangeNewLineCodeLF2CRLF(HtmlSource);
        }

        /// <summary>
        /// Htmlに埋め込まれた画像を取得
        /// </summary>
        private void GetHtmlPicture()
        {
            WebClient client = new WebClient();
            Byte[] data = client.DownloadData("https://www.yahoo.co.jp/weather.jpg");

            String PicFileName = @"D:\tmp\sample.jpg";
            File.WriteAllBytes(PicFileName, data);
        }

        private void comboBox_LoadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Path.Combine(userDataFolder, comboBox_LoadSetting.Text);
            if (File.Exists(LoadFileName))
            {
                LoadProfile(LoadFileName);
            }
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_LoadSetting, ProfileExtensions, SaveProfile, userDataFolder);
        }
    }
}
