using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Runtime.InteropServices;
using StandardTemplate;

namespace FileArranger
{
    // ファイル並べ替えタブ(sf)の処理(Form1.csから分割。コードは移しただけで中身は変えていない)
    partial class FileArranger
    {
        private void sf_textBox_TargetFile_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(sf_textBox_TargetFile.Text, e);
        }

        private void sf_button_Listup_TargetFile_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(sf_textBox_TargetFile.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + sf_textBox_TargetFile.Text);
                return;
            }

            // フォルダをリストアップ
            String[] Folders = Directory.GetDirectories(sf_textBox_TargetFile.Text);
            sf_listBox_Target.Items.Clear();
            for (int i = 0; i < Folders.Length; i++)
            {
                String FolderName = Folders[i].Remove(0, sf_textBox_TargetFile.Text.Length + 1);   // "\\"の分を1加算
                sf_listBox_Target.Items.Add(FolderName);
            }

            sf_label_TotalNum.Text = "フォルダ数：" + Folders.Length.ToString();

        }

        private void sf_button_SortFileRename_Click(object sender, EventArgs e)
        {
            if (sf_listBox_Target.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            for (int i = 0; i < sf_listBox_Target.SelectedItems.Count; i++)
            {
                String FilePath = sf_textBox_TargetFile.Text + @"\" + sf_listBox_Target.SelectedItems[i].ToString();
                sorter.SortFolder(FilePath);
            }
            sorter.CommitBatch();
        }

        private void sf_button_Sort_Restore_Click(object sender, EventArgs e)
        {
            if (!sorter.Restore())
            {
                MessageBox.Show("これ以上復元できません");
            }
        }

        private void sf_listBox_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            sf_label_SelectNum.Text = "選択数：" + sf_listBox_Target.SelectedItems.Count.ToString();
        }

        private void sf_listBox_Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sf_button_SortFileRename_Click(sender, e);
            }
            else
            {
                util.SelectAll(e);
            }
        }
    }
}
