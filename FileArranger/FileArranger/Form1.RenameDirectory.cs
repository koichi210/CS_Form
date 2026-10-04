using System;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // フォルダ名変更タブ(rd)の処理(Form1.csから分割)
    partial class FileArranger
    {
        private void rd_button_Listup_Target_Click(object sender, EventArgs e)
        {
            ListupRenameTargetDirectory();
        }

        private void rd_textBox_ExistItemDir_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(rd_textBox_ExistItemDir.Text, e);
        }

        private void rd_button_Execute_Rename_Click(object sender, EventArgs e)
        {
            ExecuteRenameFolder();
        }

        private void rd_button_RenameFolderRestore_Click(object sender, EventArgs e)
        {
            if (!_renameDirMemory.DecrementSerialNumber())
            {
                MessageBox.Show("これ以上復元できません");
                return;
            }

            while (_renameDirMemory.HasRestoreItem())
            {
                String srcName = "";
                String destName = "";
                _renameDirMemory.PopRestoreItem(ref srcName, ref destName);
                Directory.Move(destName, srcName);
            }
            ListupRenameTargetDirectory();
        }

        private void ExecuteRenameFolder()
        {
            if (!HasSelectedItems(rd_listView_Target.SelectedItems.Count))
            {
                return;
            }

            int idx = 0;
            for (int i = 0; i < rd_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                idx = rd_listView_Target.SelectedItems[i].Index;

                // 変更するフォルダ名
                String srcName = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.Items[idx].SubItems[_renameSrcIdx].Text;
                String destName = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.Items[idx].SubItems[_renameDestIdx].Text;

                // フォルダ名の重複回避
                _util.AvoidFileNameConflict(ref destName, i);
                _fio.MoveDirectory(srcName, destName);
                _renameDirMemory.AddRestoreItem(srcName, destName);
            }
            _renameDirMemory.IncrementSerialNumber();

            ListupRenameTargetDirectory(idx);
        }

        // visibleItemIdx: リストアップ後に見える位置までスクロールしておく項目のIdx
        private void ListupRenameTargetDirectory(int visibleItemIdx = 0)
        {
            if (!IsValidFolderPath(rd_comboBox_RenameDir.Text))
            {
                return;
            }

            // フォルダをリストアップ
            String[] folders = Directory.GetDirectories(rd_comboBox_RenameDir.Text);
            FillListView(rd_listView_Target, folders, rd_comboBox_RenameDir.Text, _renameDirColumns.Length);

            visibleItemIdx = Math.Min(visibleItemIdx, folders.Length - 1);
            if (visibleItemIdx > 0)
            {
                rd_listView_Target.EnsureVisible(visibleItemIdx);
            }

            rd_label_TotalNum.Text = "フォルダ数：" + folders.Length.ToString();
            rd_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void UpdateRenameComboBox()
        {
            _util.SetComboBoxFromArraySubString(
                rd_comboBox_MergeWord,
                ReferenceCandidateFolders,
                rd_textBox_ExistItemDir.Text.Length + 1,
                rd_textBox_SplitWord3.Text,
                rd_comboBox_MergeWord.Text,
                true);
        }

        private void rd_listView_Rename_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 選択解除
            for (int i = 0; i < rd_listView_Target.Items.Count; i++)
            {
                rd_listView_Target.Items[i].SubItems[_renameDestIdx].Text = "";
            }

            UpdateRenameDestNames();
        }

        private void rd_RenameSetting_TextChanged(object sender, EventArgs e)
        {
            UpdateRenameDestNames();
        }

        // 選択中の項目の[変更後]列に、変更後のフォルダ名を生成して表示する
        private void UpdateRenameDestNames()
        {
            rd_label_SelectNum.Text = FormatSelectedCount(rd_listView_Target.SelectedItems.Count);
            for (int i = 0; i < rd_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = rd_listView_Target.SelectedItems[i].Index;

                //文字列から数値を取得
                String srcString = Logic.ChangeWide2Narrow(rd_listView_Target.Items[idx].SubItems[_renameSrcIdx].Text);
                long destNumber = _util.GetNumberFromRear(srcString,
                    rd_textBox_SearchTitleLine.Text,
                    rd_textBox_SearchTitleLength.Text);

                String number = Logic.ToPaddedNumberString(destNumber);

                // 変更後フォルダ名を生成
                String destName = rd_comboBox_MergeWord.Text + rd_textBox_AddTitlePreWord.Text + number + rd_comboBox_AddTitlePostWord.Text;
                rd_listView_Target.Items[idx].SubItems[_renameDestIdx].Text = destName;
            }
        }

        private void rd_listView_Rename_DoubleClick(object sender, EventArgs e)
        {
            String openPath = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.SelectedItems[0].SubItems[_renameSrcIdx].Text;
            if (Directory.Exists(openPath))
            {
                if (rd_checkBox_FileOpen.Checked)
                {
                    String[] files = Directory.GetFiles(openPath, "*", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        openPath = files[0];
                    }
                }
                System.Diagnostics.Process.Start(openPath);
                rd_comboBox_MergeWord.Focus();
            }
        }

        private void RecreateRenameColumnsEvenly()
        {
            RecreateColumnsEvenly(rd_listView_Target, _renameDirColumns);
        }

        private void rd_comboBox_MergeWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                ExecuteRenameFolder();
            }
        }

        private void rd_listView_Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                rd_button_Execute_Rename_Click(sender, e);
                rd_comboBox_MergeWord.Focus();
            }
            else if (e.Control == true && e.KeyCode == Keys.C)
            {
                _util.CopyToClipboard(e, rd_listView_Target, "", _renameSrcIdx);
            }
        }

        private void rd_label_ExistItemDir_DoubleClick(object sender, EventArgs e)
        {
            rd_textBox_ExistItemDir.ReadOnly = !rd_textBox_ExistItemDir.ReadOnly;
        }

        private void rd_comboBox_MergeWord_DropDown(object sender, EventArgs e)
        {
            UpdateRenameComboBox();
        }

        private void rd_comboBox_RenameDir_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(rd_comboBox_RenameDir.Text, e);
        }
    }
}
