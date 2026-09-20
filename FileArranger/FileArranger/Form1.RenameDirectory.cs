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
            RenameFolderExecute();
        }

        private void rd_button_RenameFolderRestore_Click(object sender, EventArgs e)
        {
            if (!pmd.DecrementRegistNumber())
            {
                MessageBox.Show("これ以上復元できません");
                return;
            }

            while (pmd.IsExistRestoreList())
            {
                String SrcName = "";
                String DestName = "";
                pmd.GetRestoreList(ref SrcName, ref DestName);
                Directory.Move(DestName, SrcName);

            }
            ListupRenameTargetDirectory();
        }

        private void RenameFolderExecute()
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

                // 変更するファイル名
                String SrcName = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.Items[idx].SubItems[RenameSrcIdx].Text;
                String DestName = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.Items[idx].SubItems[RenameDestIdx].Text;

                // ファイル名の重複回避
                util.CreateFileNameOverLapShirk(ref DestName, i);
                fio.MoveDirectory(SrcName, DestName);
                pmd.SetRestoreList(SrcName, DestName);
            }
            pmd.IncrementRegistNumber();

            ListupRenameTargetDirectory(idx);
        }

        private void ListupRenameTargetDirectory(int ScrollbarPos = 0)
        {
            if (!Directory.Exists(rd_comboBox_RenameDir.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + rd_comboBox_RenameDir.Text );
                return;
            }

            rd_listView_Target.Items.Clear();

            // フォルダをリストアップ
            String[] Folders = Directory.GetDirectories(rd_comboBox_RenameDir.Text);
            for (int i = 0; i < Folders.Length; i++)
            {
                String FileName = Folders[i].Remove(0, rd_comboBox_RenameDir.Text.Length + 1);   // "\\"の分を1加算

                String[] item = { FileName, "" };
                rd_listView_Target.Items.Add(new ListViewItem(item));
            }

            if (ScrollbarPos > Folders.Length)
            {
                ScrollbarPos = Folders.Length - 1;
            }

            if (ScrollbarPos > 0)
            {
                rd_listView_Target.EnsureVisible(ScrollbarPos);
            }

            rd_label_TotalNum.Text = "フォルダ数：" + Folders.Length.ToString();
            rd_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void UpdateRenameComboBox()
        {
            util.SetComboBoxFromArraySubString(
                rd_comboBox_MergeWord,
                RefrenceCandidateFolders,
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

            rd_listView_Rename_UpdteListBox();
        }

        private void rd_comboBox_MergeWord_TextChanged(object sender, EventArgs e)
        {
            rd_listView_Rename_UpdteListBox();
        }

        private void rd_listView_Rename_UpdteListBox()
        {
            rd_label_SelectNum.Text = "選択数：" + rd_listView_Target.SelectedItems.Count.ToString();
            for (int i = 0; i < rd_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = rd_listView_Target.SelectedItems[i].Index;

                //文字列から数値を取得
                String SrcString = Logic.ChangeWide2Narrow(rd_listView_Target.Items[idx].SubItems[RenameSrcIdx].Text);
                long DestNumber = util.GetNumberFromRear(SrcString,
                    rd_textBox_SearchTitleLine.Text,
                    rd_textBox_SearchTitleLength.Text);

                String Number = Logic.GetNumber(DestNumber);

                // 変更後ファイル名を生成
                String DestName = rd_comboBox_MergeWord.Text + rd_textBox_AddTitlePreWord.Text + Number + rd_comboBox_AddTitlePostWord.Text;
                rd_listView_Target.Items[idx].SubItems[RenameDestIdx].Text = DestName;
            }
        }

        private void rd_listView_Rename_DoubleClick(object sender, EventArgs e)
        {
            String FilePath = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.SelectedItems[SortFileRenameTargetIdx].SubItems[RenameSrcIdx].Text;
            if (Directory.Exists(FilePath))
            {
                if (rd_checkBox_FileOpen.Checked)
                {
                    String[] files = Directory.GetFiles(FilePath, "*", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        FilePath = files[0];
                    }
                }
                System.Diagnostics.Process.Start(FilePath);
                rd_comboBox_MergeWord.Focus();
            }
        }

        private void rd_listView_Target_Update()
        {
            rd_listView_Target.Columns.Clear();

            // ListViewコントロールのプロパティを設定
            rd_listView_Target.FullRowSelect = true;
            rd_listView_Target.GridLines = true;
            rd_listView_Target.Sorting = SortOrder.Ascending;
            rd_listView_Target.View = View.Details;

            // 列（コラム）ヘッダの作成
            ColumnHeader columnSource = new ColumnHeader();
            columnSource.Text = RenameDirColumn[0];
            columnSource.Width = rd_listView_Target.Width / RenameDirColumn.Length;

            ColumnHeader columnTarget = new ColumnHeader();
            columnTarget.Text = RenameDirColumn[1];
            columnTarget.Width = rd_listView_Target.Width / RenameDirColumn.Length;
            
            ColumnHeader[] colHeaderRegValue = { columnSource, columnTarget };
            rd_listView_Target.Columns.AddRange(colHeaderRegValue);
        }

        private void rd_comboBox_MergeWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                RenameFolderExecute();
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
