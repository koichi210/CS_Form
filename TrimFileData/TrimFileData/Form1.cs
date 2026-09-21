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

namespace TrimFileData
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private readonly String SettingFile = @"TrimFileData.json";

        // 設定ファイルはJSONが基本。旧XML(TrimFileData.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"TrimFileData.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

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

            sr.RegistItem(this);
            JsonSaveRestore.LoadWithMigration(sr, SettingFile, LegacySettingFileName,
                path => sr.LoadProc(path));
            util.UpdateProfileList(ref comboBox_LoadSetting, ProfileExtensions, SettingFile);
        }

        private void textBox_SourceList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_SearchWordList, e);
        }

        private void textBox_DestList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_SearchResultList, e);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            // 出力先をクリア
            textBox_SearchResultList.Text = "";

            StcFileInputOutput fio = new StcFileInputOutput();
            String ReferData = fio.LoadFile(textBox_ReferencePath.Text);
            if (ReferData == String.Empty)
            {
                MessageBox.Show("リファレンスファイルが開けません。" + Environment.NewLine + textBox_ReferencePath.Text);
                return;
            }

            // 検索ワードをリストアップ
            String[] SourceArray = textBox_SearchWordList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // リファレンスをリスト化
            String[] ReferList = ReferData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // 検索結果をコントロールにセット
            textBox_SearchResultList.Text = Logic.GetSearchData(SourceArray, ReferList, checkBox_OrdinalCase.Checked, checkBox_FirstWordOnly.Checked, textBox_SearchCommonWord.Text);
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
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + comboBox_LoadSetting.Text;
            if (File.Exists(LoadFileName))
            {
                LoadProfile(LoadFileName);
            }
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_LoadSetting, ProfileExtensions, SaveProfile);
        }
    }
}
