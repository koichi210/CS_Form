using System;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // ファイル並べ替えタブ(sf)の処理(Form1.csから分割)
    partial class FileArranger
    {
        private void sf_button_Listup_TargetFile_Click(object sender, EventArgs e)
        {
            ListupInto(sf_listBox_Target, sf_label_TotalNum, "フォルダ数", sf_textBox_TargetFile.Text, Directory.GetDirectories);
        }

        private void sf_button_SortFileRename_Click(object sender, EventArgs e)
        {
            SortSelectedFolders();
        }

        private void SortSelectedFolders()
        {
            if (!HasSelectedItems(sf_listBox_Target.SelectedItems.Count))
            {
                return;
            }

            foreach (String selectedName in Utils.GetSelectedNames(sf_listBox_Target))
            {
                _sorter.SortFolder(Path.Combine(sf_textBox_TargetFile.Text, selectedName));
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
            HandleListBoxKeyDown(e, SortSelectedFolders);
        }
    }
}
