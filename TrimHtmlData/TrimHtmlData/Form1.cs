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
    public partial class Form1 : Form
    {
        private Boolean IsDebug = false;
        private readonly String SettingFile = @"TrimHtmlData.xml";

        private StcFileInputOutput fio = new StcFileInputOutput();
        private SaveRestore sr = new SaveRestore();
        private StcUtils util = new StcUtils();

        public Form1()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.TrimHtmlData;

            util.SetCurrentDirectory();

            sr.RegistItem(this);
            sr.LoadProc(SettingFile);
            util.UpdateProfileList(ref comboBox_LoadSetting, SettingFile);
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

            int TrimLineNum = GetTrimLine();

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

                String Result = GetSearchString(HtmlSource, textBox_SearchWord.Text, TrimLineNum, CmpOpt);
                dbg.WriteDataInNewFile(Result, "_2_search");

                textBox_DestList.Text += Result;
            }

            Clipboard.SetText(textBox_DestList.Text);
        }

        private int GetTrimLine()
        {
            int TrimLineNum = util.GetInteger(textBox_TrimLineNum.Text);
            if (TrimLineNum == 0)
            {
                TrimLineNum = 1;
            }

            return TrimLineNum;
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

        /// <summary>
        /// HtmlSourceから目的の文字列を検索し、ヒット行を取得
        /// </summary>
        /// <param name="SourceList"></param>
        /// <param name="SearchWord"></param>
        /// <returns></returns>
        private String GetSearchString(String SourceList, String SearchWord, int TrimLineNum, StringComparison CmpOpt)
        {
            String[] StringArray = SourceList.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            String ResultString = "";

            for (int i = 0; i < StringArray.Length; i++)
            {
                if (StringArray[i].IndexOf(SearchWord, CmpOpt) != -1)
                {
                    // Hitした行を含む指定行数分取得
                    for (int j = 0; j < TrimLineNum; j++)
                    {
                        ResultString += StringArray[i + j] + Environment.NewLine;
                    }

                    // 最初に見つかったワードのみ
                    if (checkBox_FirstWordOnly.Checked)
                    {
                        break;
                    }

                    // 次のワードとの境界
                    ResultString += Environment.NewLine;
                }
            }

            return ResultString + Environment.NewLine;
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
