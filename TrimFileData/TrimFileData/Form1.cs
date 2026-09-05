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
    public partial class Form1 : Form
    {
        private readonly String SettingFile = @"TrimFileData.xml";

        private StcFileInputOutput fio = new StcFileInputOutput();
        private StcUtils util = new StcUtils();
        private SaveRestore sr = new SaveRestore();

        public Form1()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.TrimFileData;

            util.SetCurrentDirectory();

            sr.RegistItem(this);
            sr.LoadProc(SettingFile);
            util.UpdateProfileList(ref comboBox_LoadSetting, SettingFile);
        }

        private void textBox_SourceList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_SerchWordList, e);
        }

        private void textBox_DestList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_SerchResultList, e);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            // 出力先をクリア
            textBox_SerchResultList.Text = "";

            StcFileInputOutput fio = new StcFileInputOutput();
            String ReferData = fio.GetFileData(textBox_ReferencePath.Text);
            if (ReferData == String.Empty)
            {
                MessageBox.Show("リファレンスファイルが開けません。" + Environment.NewLine + textBox_ReferencePath.Text);
                return;
            }

            // 検索ワードをリストアップ
            String[] SourceArray = textBox_SerchWordList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // リファレンスをリスト化
            String[] ReferList = ReferData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // 検索結果をコントロールにセット
            textBox_SerchResultList.Text = Logic.GetSearchData(SourceArray, ReferList, checkBox_OrdinalCase.Checked, checkBox_FirstWordOnly.Checked, textBox_SerchCommonWord.Text);
            Clipboard.SetText(textBox_SerchResultList.Text);
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
                sr.LoadProc(LoadFileName);
            }
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(comboBox_LoadSetting.Text);
            if (sr.SaveSetting(SaveFileName))
            {
                util.UpdateProfileList(ref comboBox_LoadSetting, Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
            }
        }
    }
}
