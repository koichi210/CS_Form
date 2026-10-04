using System;
using System.Collections.Generic;
using System.Linq;
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

        private void rd_button_Execute_Rename_Click(object sender, EventArgs e)
        {
            ExecuteRenameFolder();
        }

        private void rd_button_RenameFolderRestore_Click(object sender, EventArgs e)
        {
            if (!_renameDirMemory.RestoreLastBatch(Directory.Move))
            {
                MessageBox.Show("これ以上復元できません");
                return;
            }

            ListupRenameTargetDirectory();
        }

        private void ExecuteRenameFolder()
        {
            List<ListViewItem> selectedItems = Utils.GetSelectedItems(rd_listView_Target);
            if (!HasSelectedItems(selectedItems.Count))
            {
                return;
            }

            for (int i = 0; i < selectedItems.Count; i++)
            {
                String srcName = Path.Combine(rd_comboBox_RenameDir.Text, selectedItems[i].SubItems[_renameSrcIdx].Text);
                String destName = Path.Combine(rd_comboBox_RenameDir.Text, selectedItems[i].SubItems[_renameDestIdx].Text);

                // フォルダ名の重複回避
                _util.AvoidFileNameConflict(ref destName, i);
                _fio.MoveDirectory(srcName, destName);
                _renameDirMemory.AddRestoreItem(srcName, destName);
            }
            _renameDirMemory.IncrementSerialNumber();

            // 最後にリネームした項目の位置が見えるようにしておく
            ListupRenameTargetDirectory(selectedItems.Last().Index);
        }

        // visibleItemIdx: リストアップ後に見える位置までスクロールしておく項目のIdx
        private void ListupRenameTargetDirectory(int visibleItemIdx = 0)
        {
            String[] folders = ListupInto(rd_listView_Target, _renameDirColumns.Length, rd_label_TotalNum, "フォルダ数",
                rd_comboBox_RenameDir.Text, Directory.GetDirectories);
            if (folders == null)
            {
                return;
            }

            visibleItemIdx = Math.Min(visibleItemIdx, folders.Length - 1);
            if (visibleItemIdx > 0)
            {
                rd_listView_Target.EnsureVisible(visibleItemIdx);
            }

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
            _renameSelectionUpdate.Request();   // ⇒RefreshRenameDestNames()
        }

        // 選択が変わった時: 一旦全行の[変更後]を消してから、選択中の行だけ作り直す
        private void RefreshRenameDestNames()
        {
            rd_listView_Target.BeginUpdate();
            Utils.SetSubItemText(rd_listView_Target, rd_listView_Target.Items.Cast<ListViewItem>(), "", _renameDestIdx);
            UpdateRenameDestNames();
            rd_listView_Target.EndUpdate();
        }

        private void rd_RenameSetting_TextChanged(object sender, EventArgs e)
        {
            UpdateRenameDestNames();
        }

        // 選択中の項目の[変更後]列に、変更後のフォルダ名を生成して表示する
        private void UpdateRenameDestNames()
        {
            List<ListViewItem> selectedItems = Utils.GetSelectedItems(rd_listView_Target);
            rd_label_SelectNum.Text = FormatSelectedCount(selectedItems.Count);

            // 番号以外の部分は全行共通なので、ループの外で1回だけ組み立てる
            String prefix = rd_comboBox_MergeWord.Text + rd_textBox_AddTitlePreWord.Text;
            String postfix = rd_comboBox_AddTitlePostWord.Text;

            rd_listView_Target.BeginUpdate();
            foreach (ListViewItem item in selectedItems)
            {
                //文字列から数値を取得
                String srcString = Logic.ChangeWide2Narrow(item.SubItems[_renameSrcIdx].Text);
                long destNumber = _util.GetNumberFromRear(srcString,
                    rd_textBox_SearchTitleLine.Text,
                    rd_textBox_SearchTitleLength.Text);

                item.SubItems[_renameDestIdx].Text = prefix + Logic.ToPaddedNumberString(destNumber) + postfix;
            }
            rd_listView_Target.EndUpdate();
        }

        private void rd_listView_Rename_DoubleClick(object sender, EventArgs e)
        {
            if (rd_listView_Target.SelectedItems.Count == 0)
            {
                return;
            }

            String openPath = Path.Combine(rd_comboBox_RenameDir.Text, rd_listView_Target.SelectedItems[0].SubItems[_renameSrcIdx].Text);
            if (!Directory.Exists(openPath))
            {
                return;
            }

            if (rd_checkBox_FileOpen.Checked)
            {
                // 先頭の1件だけ分かればよいので、サブフォルダ以下を全部列挙せずに最初の1件で打ち切る
                openPath = Directory.EnumerateFiles(openPath, "*", SearchOption.AllDirectories).FirstOrDefault() ?? openPath;
            }
            System.Diagnostics.Process.Start(openPath);
            rd_comboBox_MergeWord.Focus();
        }

        private void RecreateRenameColumnsEvenly()
        {
            RecreateColumnsEvenly(rd_listView_Target, _renameDirColumns);
        }

        // rdタブの各入力欄で共通: Ctrl+Enterでリネーム実行
        private void rd_RenameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsCtrlEnter(e))
            {
                ExecuteRenameFolder();
            }
        }

        private void rd_listView_Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsCtrlEnter(e))
            {
                ExecuteRenameFolder();
                rd_comboBox_MergeWord.Focus();
            }
            else if (e.Control && e.KeyCode == Keys.C)
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
    }
}
