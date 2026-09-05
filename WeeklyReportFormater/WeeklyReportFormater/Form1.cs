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

            textBox_ThisWeekAfter.Text = Logic.FormatThisWeek(textBox_ThisWeekBefore.Text, textBox_UserName.Text);
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

            textBox_NextWeekAfter.Text = Logic.FormatNextWeek(textBox_NextWeekBefore.Text, textBox_UserName.Text);
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

            textBox_PerforceAfter.Text = Logic.FormatPerforce(textBox_PerforceBefore.Text);
            Clipboard.SetText(textBox_PerforceAfter.Text);
        }
    }
}
