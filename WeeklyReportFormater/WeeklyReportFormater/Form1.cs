﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StandardTemplate;
using System.IO;

namespace WeeklyReportFormater
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();
        private StcFileInputOutput fio = new StcFileInputOutput();
        
        private int NextWeekSetNum = 3;
        private int PerforceSetNum = 2;
        private String SaveFileName = "WhoAmI.txt";

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.WeeklyReportFormater;

            // カレントディレクトリ移動
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            String UserName = fio.GetFileData(SaveFileName);
            textBox_UserName.Text = UserName;
        }

        private void textBox_ThisWeekBefore_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_ThisWeekBefore, e);
        }

        private void textBox_ThisWeekAfter_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_ThisWeekAfter, e);
        }

        private void textBox_NextWeekBefore_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_NextWeekBefore, e);
        }

        private void textBox_NextWeekAfter_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_NextWeekAfter, e);
        }

        private void textBox_PerforceBefore_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_PerforceBefore, e);
        }

        private void textBox_PerforceAfter_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_PerforceAfter, e);
        }

        private void button_ThisWeekChange_Click(object sender, EventArgs e)
        {
            textBox_ThisWeekAfter.Clear();
            if (textBox_ThisWeekBefore.Text == String.Empty)
            {
                MessageBox.Show("変換元データが入力されていません");
                return;
            }

            String[] Line = textBox_ThisWeekBefore.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Line.Length; i++)
            {
                String NewLine = Line[i];
                NewLine = NewLine.TrimEnd();
                NewLine = NewLine.Replace("\t", "");                                // タブ ⇒ スペース
                NewLine = "\t" + NewLine;                                           // 先頭にタブ挿入
                NewLine = NewLine.Replace(textBox_UserName.Text + " ", "(") + ")";  // ユーザー名削除
                NewLine = NewLine.Replace(".0)", ")");                              // ストーリーポイントの".0"が邪魔
                NewLine += System.Environment.NewLine;                              // 終端に改行挿入

                textBox_ThisWeekAfter.Text += NewLine;
            }
            Clipboard.SetText(textBox_ThisWeekAfter.Text);
        }

        private void button_NextWeekChange_Click(object sender, EventArgs e)
        {
            textBox_NextWeekAfter.Clear();
            if (textBox_NextWeekBefore.Text == String.Empty)
            {
                MessageBox.Show("変換元データが入力されていません");
                return;
            }

            String[] Line = textBox_NextWeekBefore.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Line.Length; i++)
            {
                String NewLine = Line[i];
                NewLine = NewLine.TrimEnd();

                switch (i % NextWeekSetNum)
                {
                    case 0 :
                        NewLine = "\t" + NewLine;   // 先頭にタブ挿入
                        break;

                    case 1:
                        NewLine = " " + NewLine;   // 課題Noと課題名の間にスペース
                        break;

                    case 2:
                        int NameIdx = NewLine.IndexOf(textBox_UserName.Text);   // ユーザー名の先頭
                        NameIdx += textBox_UserName.Text.Length;                // ユーザー名の終端
                        NewLine = " (" + NewLine.Substring(NameIdx) + ")" + System.Environment.NewLine;  // ストーリーポイント
                        break;

                    default:
                        break;
                }

                textBox_NextWeekAfter.Text += NewLine;
            }
            Clipboard.SetText(textBox_NextWeekAfter.Text);
        }

        private void button_PerforceChange_Click(object sender, EventArgs e)
        {
            textBox_PerforceAfter.Clear();
            if (textBox_PerforceBefore.Text == String.Empty)
            {
                MessageBox.Show("変換元データが入力されていません");
                return;
            }

            
            String[] Line = textBox_PerforceBefore.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            String NewLine = "";
            for (int i = 0; i < Line.Length; i++)
            {
                Line[i] = Line[i].TrimStart();
                Line[i] = Line[i].TrimEnd();

                switch (i % PerforceSetNum)
                {
                    case 0 :
                        NewLine += Line[i] + " ";     // ProjectID
                        break;

                    case 1 :
                        NewLine += Line[i];     	  // Summary
                        break;

                    default:
                        break;
                }
            }
            textBox_PerforceAfter.Text += NewLine;
            Clipboard.SetText(textBox_PerforceAfter.Text);
        }
    }
}
