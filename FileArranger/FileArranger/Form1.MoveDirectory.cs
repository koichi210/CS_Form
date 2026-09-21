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
            Move_Directory(true);
        }

        private void Move_Directory(Boolean IsMoveTopDir)
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
                String SourceTargetName;
                String DestTargetName;
                if (IsMoveTopDir)
                {

                    SourceTargetName = fio.GetFirstPathName(md_listBox_Listup.SelectedItems[i].ToString());
                    DestTargetName = SourceTargetName;
                }
                else
                {
                    SourceTargetName = md_listBox_Listup.SelectedItems[i].ToString();
                    DestTargetName = fio.GetLastPathName(md_listBox_Listup.SelectedItems[i].ToString());
                }

                String SourcePath = md_textBox_SourceDir.Text + @"\" + SourceTargetName;
                String DestPath = md_comboBox_TargetDir.Text + @"\" + DestTargetName;

                // Top階層ごと移動した場合などで、すでにDirectoryが存在しないケースをcare
                if (!Directory.Exists(SourcePath))
                {
                    continue;
                }

                // 移動先にすでにフォルダがある場合は重複回避
                util.CreateFolderNameOverLapShirk(ref DestPath, i);

                fio.MoveDirectory(SourcePath, DestPath);
            }

            // リストを更新
            ListupMoveDirectory(true);
        }

        private void ListupMoveDirectory(Boolean IsRestoreScrollBarPos = false)
        {
            if (!IsValidFolderPath(md_textBox_SourceDir.Text))
            {
                return;
            }

            int RegistNum = 0;
            // フォルダパスの末尾に'\\'があったら削除
            char[] chTrims = { '\\', '/' };
            md_textBox_SourceDir.Text = md_textBox_SourceDir.Text.TrimEnd(chTrims);

            int ScrollBarPos = 0;
            if (IsRestoreScrollBarPos)
            {
                ScrollBarPos = md_listBox_Listup.TopIndex;
            }
            md_listBox_Listup.Items.Clear();
            String[] files = Directory.GetDirectories(md_textBox_SourceDir.Text, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                // フォルダ直下にファイルが1つでもあればリストアップ。
                // 以前はGetFileSystemEntries(ファイル+サブフォルダ両方を一括取得)してから
                // File.Existsで1件ずつ判定していたが、EnumerateFilesなら最初から
                // ファイルだけを対象にでき、かつ遅延列挙なので最初の1件が見つかった時点で
                // Any()が打ち切ってくれる(フォルダ内の全件を毎回列挙しなくて済む)。
                if (Directory.EnumerateFiles(files[i]).Any())
                {
                    String FileName = GetDisplayName(files[i], md_textBox_SourceDir.Text);
                    md_listBox_Listup.Items.Add(FileName);
                    RegistNum++;
                }
            }
            md_listBox_Listup.TopIndex = ScrollBarPos;
            md_label_TotalNum.Text = "フォルダ数：" + RegistNum.ToString();
        }

        public void UpdateMoveDestDirComboBox()
        {
            util.SetComboBoxFromArray(pf_comboBox_MoveDestDirName, ReferenceCandidateFolders, pf_textBox_ReferenceFile.Text);
        }
    }
}
