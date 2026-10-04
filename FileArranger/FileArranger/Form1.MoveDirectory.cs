using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // フォルダ移動タブ(md)の処理(Form1.csから分割)
    partial class FileArranger
    {
        private void md_button_Listup_Click(object sender, EventArgs e)
        {
            ListupMoveDirectory();
        }

        private void md_button_MoveTopDir_Click(object sender, EventArgs e)
        {
            MoveSelectedDirectories(true);
        }

        private void md_button_MoveSubDir_Click(object sender, EventArgs e)
        {
            MoveSelectedDirectories(false);
        }

        private void md_listBox_Listup_KeyDown(object sender, KeyEventArgs e)
        {
            HandleListBoxKeyDown(e, () => MoveSelectedDirectories(false));
        }

        private void md_listBox_Listup_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (md_listBox_Listup.SelectedItems.Count > 0)
            {
                _util.ExecutePath(Path.Combine(md_textBox_SourceDir.Text, md_listBox_Listup.SelectedItem.ToString()));
            }
        }

        private void md_listBox_Listup_SelectedIndexChanged(object sender, EventArgs e)
        {
            md_label_SelectNum.Text = FormatSelectedCount(md_listBox_Listup.SelectedItems.Count);
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

            foreach (String selectedName in Utils.GetSelectedNames(md_listBox_Listup))
            {
                Directory.Delete(Path.Combine(md_textBox_SourceDir.Text, selectedName), true);
            }

            // リストを更新
            ListupMoveDirectory();
        }

        private void MoveSelectedDirectories(Boolean isMoveTopDir)
        {
            if (!CanOperateSelectedDirectories())
            {
                return;
            }

            List<String> selectedNames = Utils.GetSelectedNames(md_listBox_Listup);
            for (int i = 0; i < selectedNames.Count; i++)
            {
                // 最上位フォルダごと移動する時は、格納元直下のフォルダ名をそのまま移動先でも使う。
                // 選択したフォルダだけ移動する時は、途中の階層を持っていかず末尾のフォルダ名だけにする
                String sourceTargetName = isMoveTopDir ? _fio.GetFirstPathName(selectedNames[i]) : selectedNames[i];
                String destTargetName = isMoveTopDir ? sourceTargetName : _fio.GetLastPathName(selectedNames[i]);

                String sourcePath = Path.Combine(md_textBox_SourceDir.Text, sourceTargetName);
                String destPath = Path.Combine(md_comboBox_TargetDir.Text, destTargetName);

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
            // フォルダパスの末尾に'\\'があったら削除
            md_textBox_SourceDir.Text = md_textBox_SourceDir.Text.TrimEnd('\\', '/');

            int topIndex = keepScrollPosition ? md_listBox_Listup.TopIndex : 0;

            // フォルダ直下にファイルが1つでもあればリストアップ。
            // EnumerateFilesは遅延列挙なので、最初の1件が見つかった時点でAny()が打ち切ってくれる
            // (フォルダ内の全件を毎回列挙しなくて済む)。
            String[] directories = ListupInto(md_listBox_Listup, md_label_TotalNum, "フォルダ数", md_textBox_SourceDir.Text,
                folder => Directory.EnumerateDirectories(folder, "*", SearchOption.AllDirectories)
                    .Where(directory => Directory.EnumerateFiles(directory).Any())
                    .ToArray());

            if (directories != null)
            {
                md_listBox_Listup.TopIndex = topIndex;
            }
        }

        public void UpdateMoveDestDirComboBox()
        {
            _util.SetComboBoxFromArray(pf_comboBox_MoveDestDirName, ReferenceCandidateFolders, pf_textBox_ReferenceFile.Text);
        }
    }
}
