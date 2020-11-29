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
            StringComparison CmpOpt = StringComparison.OrdinalIgnoreCase;
            if (checkBox_WideNarrow.Checked)
            {
                CmpOpt = StringComparison.Ordinal;
            }

            String[] DeleteArray = textBox_DeleteString.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            StcFileInputOutput fio = new StcFileInputOutput();
            for (int i = 0; i < FileList.Length; i++)
            {
                // FileをStringに変換
                String FileData = fio.LoadFile(FileList[i]);
                String[] FileDataArray = FileData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                var list = new List<String>();
                list.AddRange(FileDataArray);

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] == String.Empty)
                    {
                        continue;
                    }

                    for (int k = 0; k < DeleteArray.Length; k++)
                    {
                        //削除対象の行か判別
                        if (list[j].IndexOf(DeleteArray[k], CmpOpt) != -1)
                        {
                            if (checkBox_DeleteLine.Checked)
                            {
                                // 一行削除&空行追加
                                list.RemoveAt(j);
                                list.Insert(j, "");
                            }
                            else
                            {
                                // 文字だけ削除ならReplace
                                list[j] = list[j].Replace(DeleteArray[k], "");
                            }
                        }
                    }
                }

                //StringをFileに変換
                String ResultData = String.Join(Environment.NewLine, list.ToArray());

                //FileSave
                fio.SaveFile(FileList[i], ResultData);
            }
        }
    }
}
