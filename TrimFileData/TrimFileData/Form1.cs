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
            textBox_SerchResultList.Text = GetSearchData(SourceArray, ReferList);
            Clipboard.SetText(textBox_SerchResultList.Text);
        }

        private String GetSearchData(String[] SourceArray, String[] ReferList)
        {
            StringComparison CmpOpt = StringComparison.OrdinalIgnoreCase;
            if (checkBox_OrdinalCase.Checked)
            {
                CmpOpt = StringComparison.Ordinal;
            }

            String Result = "";
            for (int i = 0; i < SourceArray.Length; i++)
            {
                Result += "◆" + SourceArray[i] + Environment.NewLine;

                String[] SourceList = SourceArray[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                String Candidate = GetHitWord(SourceList, ReferList, CmpOpt);
                Result += util.TrimDuplication(Candidate, Environment.NewLine);
                Result += Environment.NewLine + Environment.NewLine;
            }

            return Result;
        }

        private String GetHitWord(String[] SourceList, String[] ReferList, StringComparison CmpOpt)
        {
            String Result = "";

            for (int j = 0; j < SourceList.Length; j++)
            {
                for (int k = 0; k < ReferList.Length; k++)
                {
                    if (textBox_SerchCommonWord.Text != "" &&
                        ReferList[k].IndexOf(textBox_SerchCommonWord.Text, CmpOpt) == -1)
                    {
                        continue;
                    }

                    if (ReferList[k].IndexOf(SourceList[j], CmpOpt) != -1)
                    {
                        //Fileから抽出
                        Result += ReferList[k] + Environment.NewLine;

                        // 最初に見つかった項目のみ抽出
                        if (checkBox_FirstWordOnly.Checked)
                        {
                            break;
                        }
                    }
                }
            }

            return Result;
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
