using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // フォルダ移動タブ(md)の処理(Form1.csから分割)
    partial class FileArranger
    {
        private void md_textBox_SourceDir_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(md_textBox_SourceDir.Text, e);
        }

        private void md_comboBox_TargetDir_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(md_comboBox_TargetDir.Text, e);
        }

        private void md_button_Listup_Click(object sender, EventArgs e)
        {
            ListupMoveDirectory();
        }

        private void md_button_MoveTopDir_Click(object sender, EventArgs e)
        {
            MoveSelectedDirectories(true);
        }

        private void md_listBox_Listup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                md_button_MoveSubDir_Click(sender, e);
            }
            else
            {
                _util.SelectAll(e);
            }
        }

        private void md_listBox_Listup_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (md_listBox_Listup.SelectedItems.Count > 0)
            {
                String targetPath = md_textBox_SourceDir.Text + @"\" + md_listBox_Listup.SelectedItem.ToString();
                _util.ExecutePath(targetPath);
            }
        }

        private void md_button_MoveSubDir_Click(object sender, EventArgs e)
        {
            MoveSelectedDirectories(false);
        }

        // 削除/移動の前提チェック(格納先フォルダの確保と選択有無)
        private Boolean CanOperateSelectedDirectories()
        {
            return _fio.EnsureDirectory(md_comboBox_TargetDir.Text)
                && HasSelectedItems(md_listBox_Listup.SelectedItems.Count);
        }

        private void md_button_Delete_Click(object sender, EventArgs e)
        {
            if (!CanOperateSelectedDirectories())
            {
                return;
            }

            foreach (object selectedItem in md_listBox_Listup.SelectedItems)
            {
                String delPath = md_textBox_SourceDir.Text + @"\" + selectedItem.ToString();
                Directory.Delete(delPath, true);
            }

            // リストを更新
            ListupMoveDirectory();
        }

        private void md_listBox_Listup_SelectedIndexChanged(object sender, EventArgs e)
        {
            md_label_SelectNum.Text = FormatSelectedCount(md_listBox_Listup.SelectedItems.Count);
        }

        private void MoveSelectedDirectories(Boolean isMoveTopDir)
        {
            if (!CanOperateSelectedDirectories())
            {
                return;
            }

            for (int i = 0; i < md_listBox_Listup.SelectedItems.Count; i++)
            {
                String selectedName = md_listBox_Listup.SelectedItems[i].ToString();
                String sourceTargetName;
                String destTargetName;
                if (isMoveTopDir)
                {
                    sourceTargetName = _fio.GetFirstPathName(selectedName);
                    destTargetName = sourceTargetName;
                }
                else
                {
                    sourceTargetName = selectedName;
                    destTargetName = _fio.GetLastPathName(selectedName);
                }

                String sourcePath = md_textBox_SourceDir.Text + @"\" + sourceTargetName;
                String destPath = md_comboBox_TargetDir.Text + @"\" + destTargetName;

                // Top階層ごと移動した場合などで、すでにDirectoryが存在しないケースをcare
                if (!Directory.Exists(sourcePath))
                {
                    continue;
                }

                // 移動先にすでにフォルダがある場合は重複回避
                _util.AvoidFolderNameConflict(ref destPath, i);

                _fio.MoveDirectory(sourcePath, destPath);
            }

            // リストを更新
            ListupMoveDirectory(true);
        }

        private void ListupMoveDirectory(Boolean keepScrollPosition = false)
        {
            if (!IsValidFolderPath(md_textBox_SourceDir.Text))
            {
                return;
            }

            // フォルダパスの末尾に'\\'があったら削除
            md_textBox_SourceDir.Text = md_textBox_SourceDir.Text.TrimEnd('\\', '/');

            int topIndex = keepScrollPosition ? md_listBox_Listup.TopIndex : 0;

            // フォルダ直下にファイルが1つでもあればリストアップ。
            // EnumerateFilesは遅延列挙なので、最初の1件が見つかった時点でAny()が打ち切ってくれる
            // (フォルダ内の全件を毎回列挙しなくて済む)。
            String[] directories = Directory.GetDirectories(md_textBox_SourceDir.Text, "*", SearchOption.AllDirectories)
                .Where(directory => Directory.EnumerateFiles(directory).Any())
                .ToArray();
            FillListBox(md_listBox_Listup, directories, md_textBox_SourceDir.Text);

            md_listBox_Listup.TopIndex = topIndex;
            md_label_TotalNum.Text = "フォルダ数：" + directories.Length.ToString();
        }

        public void UpdateMoveDestDirComboBox()
        {
            _util.SetComboBoxFromArray(pf_comboBox_MoveDestDirName, ReferenceCandidateFolders, pf_textBox_ReferenceFile.Text);
        }
    }
}
