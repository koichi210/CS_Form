using System;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // フォルダ名変更タブ(rd)の処理(Form1.csから分割。コードは移しただけで中身は変えていない)
    partial class FileArranger
    {
        private void rd_button_Listup_Target_Click(object sender, EventArgs e)
        {
            ListupRenameTargetDirectory();
        }

        private void rd_textBox_ExistItemDir_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(rd_textBox_ExistItemDir.Text, e);
        }

        private void rd_button_Execute_Rename_Click(object sender, EventArgs e)
        {
            ExecuteRenameFolder();
        }

        private void rd_button_RenameFolderRestore_Click(object sender, EventArgs e)
        {
            if (!renameDirMemory.DecrementSerialNumber())
            {
                MessageBox.Show("これ以上復元できません");
                return;
            }

            while (renameDirMemory.HasRestoreItem())
            {
                String srcName = "";
                String destName = "";
                renameDirMemory.PopRestoreItem(ref srcName, ref destName);
                Directory.Move(destName, srcName);
            }
            ListupRenameTargetDirectory();
        }

        private void ExecuteRenameFolder()
        {
            if (rd_listView_Target.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            int idx = 0;
            for (int i = 0; i < rd_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                idx = rd_listView_Target.SelectedItems[i].Index;

                // 変更するフォルダ名
                String srcName = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.Items[idx].SubItems[RenameSrcIdx].Text;
                String destName = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.Items[idx].SubItems[RenameDestIdx].Text;

                // フォルダ名の重複回避
                util.AvoidFileNameConflict(ref destName, i);
                fio.MoveDirectory(srcName, destName);
                renameDirMemory.AddRestoreItem(srcName, destName);
            }
            renameDirMemory.IncrementSerialNumber();

            ListupRenameTargetDirectory(idx);
        }

        // visibleItemIdx: リストアップ後に見える位置までスクロールしておく項目のIdx
        private void ListupRenameTargetDirectory(int visibleItemIdx = 0)
        {
            if (!IsValidFolderPath(rd_comboBox_RenameDir.Text))
            {
                return;
            }

            rd_listView_Target.Items.Clear();

            // フォルダをリストアップ
            String[] folders = Directory.GetDirectories(rd_comboBox_RenameDir.Text);
            for (int i = 0; i < folders.Length; i++)
            {
                String folderName = GetDisplayName(folders[i], rd_comboBox_RenameDir.Text);

                String[] item = { folderName, "" };
                rd_listView_Target.Items.Add(new ListViewItem(item));
            }

            if (visibleItemIdx > folders.Length)
            {
                visibleItemIdx = folders.Length - 1;
            }

            if (visibleItemIdx > 0)
            {
                rd_listView_Target.EnsureVisible(visibleItemIdx);
            }

            rd_label_TotalNum.Text = "フォルダ数：" + folders.Length.ToString();
            rd_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void UpdateRenameComboBox()
        {
            util.SetComboBoxFromArraySubString(
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
                rd_listView_Target.Items[i].SubItems[RenameDestIdx].Text = "";
            }

            UpdateRenameDestNames();
        }

        private void rd_comboBox_MergeWord_TextChanged(object sender, EventArgs e)
        {
            UpdateRenameDestNames();
        }

        // 選択中の項目の[変更後]列に、変更後のフォルダ名を生成して表示する
        private void UpdateRenameDestNames()
        {
            rd_label_SelectNum.Text = "選択数：" + rd_listView_Target.SelectedItems.Count.ToString();
            for (int i = 0; i < rd_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = rd_listView_Target.SelectedItems[i].Index;

                //文字列から数値を取得
                String srcString = Logic.ChangeWide2Narrow(rd_listView_Target.Items[idx].SubItems[RenameSrcIdx].Text);
                long destNumber = util.GetNumberFromRear(srcString,
                    rd_textBox_SearchTitleLine.Text,
                    rd_textBox_SearchTitleLength.Text);

                String number = Logic.ToPaddedNumberString(destNumber);

                // 変更後フォルダ名を生成
                String destName = rd_comboBox_MergeWord.Text + rd_textBox_AddTitlePreWord.Text + number + rd_comboBox_AddTitlePostWord.Text;
                rd_listView_Target.Items[idx].SubItems[RenameDestIdx].Text = destName;
            }
        }

        private void rd_listView_Rename_DoubleClick(object sender, EventArgs e)
        {
            String openPath = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.SelectedItems[0].SubItems[RenameSrcIdx].Text;
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

        private void SetupRenameListViewColumns()
        {
            rd_listView_Target.Columns.Clear();

            // ListViewコントロールのプロパティを設定
            rd_listView_Target.FullRowSelect = true;
            rd_listView_Target.GridLines = true;
            rd_listView_Target.Sorting = SortOrder.Ascending;
            rd_listView_Target.View = View.Details;

            // 列（コラム）ヘッダの作成
            ColumnHeader columnSource = new ColumnHeader();
            columnSource.Text = RenameDirColumns[0];
            columnSource.Width = rd_listView_Target.Width / RenameDirColumns.Length;

            ColumnHeader columnTarget = new ColumnHeader();
            columnTarget.Text = RenameDirColumns[1];
            columnTarget.Width = rd_listView_Target.Width / RenameDirColumns.Length;

            ColumnHeader[] columnHeaders = { columnSource, columnTarget };
            rd_listView_Target.Columns.AddRange(columnHeaders);
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
                util.CopyToClipboard(e, rd_listView_Target, "", RenameSrcIdx);
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
            util.ExecutePath(rd_comboBox_RenameDir.Text, e);
        }
    }
}
