using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // フォルダ移動タブ(md)の処理(Form1.csから分割。コードは移しただけで中身は変えていない)
    partial class FileArranger
    {
        private void md_textBox_SourceDir_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(md_textBox_SourceDir.Text, e);
        }

        private void md_comboBox_TargetDir_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(md_comboBox_TargetDir.Text, e);
        }

        private void md_button_Listup_Click(object sender, EventArgs e)
        {
            ListupMoveDirectory();
        }

        private void md_button_MoveTopDir_Click(object sender, EventArgs e)
        {
            MoveSelectedDirectories(true);
        }

        private void MoveSelectedDirectories(Boolean isMoveTopDir)
        {
            if (!fio.EnsureDirectory(md_comboBox_TargetDir.Text))
            {
                return;
            }

            if (md_listBox_Listup.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            for (int i = 0; i < md_listBox_Listup.SelectedItems.Count; i++)
            {
                String sourceTargetName;
                String destTargetName;
                if (isMoveTopDir)
                {
                    sourceTargetName = fio.GetFirstPathName(md_listBox_Listup.SelectedItems[i].ToString());
                    destTargetName = sourceTargetName;
                }
                else
                {
                    sourceTargetName = md_listBox_Listup.SelectedItems[i].ToString();
                    destTargetName = fio.GetLastPathName(md_listBox_Listup.SelectedItems[i].ToString());
                }

                String sourcePath = md_textBox_SourceDir.Text + @"\" + sourceTargetName;
                String destPath = md_comboBox_TargetDir.Text + @"\" + destTargetName;

                // Top階層ごと移動した場合などで、すでにDirectoryが存在しないケースをcare
                if (!Directory.Exists(sourcePath))
                {
                    continue;
                }

                // 移動先にすでにフォルダがある場合は重複回避
                util.AvoidFolderNameConflict(ref destPath, i);

                fio.MoveDirectory(sourcePath, destPath);
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

            int listedCount = 0;
            // フォルダパスの末尾に'\\'があったら削除
            char[] chTrims = { '\\', '/' };
            md_textBox_SourceDir.Text = md_textBox_SourceDir.Text.TrimEnd(chTrims);

            int topIndex = 0;
            if (keepScrollPosition)
            {
                topIndex = md_listBox_Listup.TopIndex;
            }
            md_listBox_Listup.Items.Clear();
            String[] directories = Directory.GetDirectories(md_textBox_SourceDir.Text, "*", SearchOption.AllDirectories);
            for (int i = 0; i < directories.Length; i++)
            {
                // フォルダ直下にファイルが1つでもあればリストアップ。
                // 以前はGetFileSystemEntries(ファイル+サブフォルダ両方を一括取得)してから
                // File.Existsで1件ずつ判定していたが、EnumerateFilesなら最初から
                // ファイルだけを対象にでき、かつ遅延列挙なので最初の1件が見つかった時点で
                // Any()が打ち切ってくれる(フォルダ内の全件を毎回列挙しなくて済む)。
                if (Directory.EnumerateFiles(directories[i]).Any())
                {
                    String dirName = GetDisplayName(directories[i], md_textBox_SourceDir.Text);
                    md_listBox_Listup.Items.Add(dirName);
                    listedCount++;
                }
            }
            md_listBox_Listup.TopIndex = topIndex;
            md_label_TotalNum.Text = "フォルダ数：" + listedCount.ToString();
        }

        public void UpdateMoveDestDirComboBox()
        {
            util.SetComboBoxFromArray(pf_comboBox_MoveDestDirName, ReferenceCandidateFolders, pf_textBox_ReferenceFile.Text);
        }
    }
}
