using System;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // ファイル並べ替えタブ(sf)の処理(Form1.csから分割)
    partial class FileArranger
    {
        private void sf_textBox_TargetFile_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(sf_textBox_TargetFile.Text, e);
        }

        private void sf_button_Listup_TargetFile_Click(object sender, EventArgs e)
        {
            if (!IsValidFolderPath(sf_textBox_TargetFile.Text))
            {
                return;
            }

            // フォルダをリストアップ
            String[] folders = Directory.GetDirectories(sf_textBox_TargetFile.Text);
            FillListBox(sf_listBox_Target, folders, sf_textBox_TargetFile.Text);
            sf_label_TotalNum.Text = "フォルダ数：" + folders.Length.ToString();
        }

        private void sf_button_SortFileRename_Click(object sender, EventArgs e)
        {
            if (!HasSelectedItems(sf_listBox_Target.SelectedItems.Count))
            {
                return;
            }

            foreach (object selectedItem in sf_listBox_Target.SelectedItems)
            {
                String folderPath = sf_textBox_TargetFile.Text + @"\" + selectedItem.ToString();
                _sorter.SortFolder(folderPath);
            }
            _sorter.CommitBatch();
        }

        private void sf_button_Sort_Restore_Click(object sender, EventArgs e)
        {
            if (!_sorter.Restore())
            {
                MessageBox.Show("これ以上復元できません");
            }
        }

        private void sf_listBox_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            sf_label_SelectNum.Text = FormatSelectedCount(sf_listBox_Target.SelectedItems.Count);
        }

        private void sf_listBox_Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sf_button_SortFileRename_Click(sender, e);
            }
            else
            {
                _util.SelectAll(e);
            }
        }
    }
}
