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

namespace ToyingFile
{
    public partial class Form1 : Form
    {
        StcUtils util = new StcUtils();
        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.ToyingFile;
        }

        private void textBox_Directory_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void textBox_File_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void textBox_DeleteString_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            if (textBox_Directory.Text == String.Empty)
            {
                MessageBox.Show("対象ファイルのディレクトリが設定されていません");
                return;
            }

            //リストアップ
            String[] TargetFileList = GetTargetFile();

            // メニュー
            if (radioButton_DeleteString.Checked)
            {
                FunctionDeleteString(TargetFileList);
            }
        }

        private String[] GetTargetFile()
        {
            String SerchPattern = "*";
            if (textBox_File.Text != String.Empty)
            {
                SerchPattern = textBox_File.Text;
            }

            SearchOption opt = SearchOption.TopDirectoryOnly;
            if (checkBox_SubDirectory.Checked)
            {
                opt = SearchOption.AllDirectories;
            }

            return Directory.GetFiles(textBox_Directory.Text, SerchPattern, opt);
        }

        private void FunctionDeleteString(String[] FileList)
        {
            String[] DeleteArray = textBox_DeleteString.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            StcFileInputOutput fio = new StcFileInputOutput();
            for (int i = 0; i < FileList.Length; i++)
            {
                String FileData = fio.LoadFile(FileList[i]);
                String ResultData = Logic.DeleteStringFromContent(FileData, DeleteArray, checkBox_WideNarrow.Checked, checkBox_DeleteLine.Checked);
                fio.SaveFile(FileList[i], ResultData);
            }
        }
    }
}
