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
    // ファイル移動タブ(mf)の処理(Form1.csから分割。コードは移しただけで中身は変えていない)
    partial class FileArranger
    {
        private void mf_listBox_Listup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mf_button_Move_SubDir_Click(sender, e);
            }
            else
            {
                util.SelectAll(e);
            }
        }

        private void mf_textBox_TargetDir_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(mf_textBox_TargetDir.Text, e);
        }

        private void mf_listBox_Listup_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (md_listBox_Listup.SelectedItems.Count > 0)
            {
                String TargetPath = md_textBox_SourceDir.Text + @"\" + md_listBox_Listup.SelectedItem.ToString();
                util.ExecutePath(TargetPath);
            }
        }

        private void mf_button_Move_SubDir_Click(object sender, EventArgs e)
        {
            Move_Directory(false);
        }

        private void mf_button_Delete_Click(object sender, EventArgs e)
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
                String DelPath = md_textBox_SourceDir.Text + @"\" + md_listBox_Listup.SelectedItems[i].ToString();
                DirectoryInfo DelDir = new DirectoryInfo(DelPath);
                DelDir.Delete(true);
            }

            // リストを更新
            ListupMoveDirectory();
        }

        private void mf_listBox_Listup_SelectedIndexChanged(object sender, EventArgs e)
        {
            md_label_SelectNum.Text = "選択数：" + md_listBox_Listup.SelectedItems.Count.ToString();
        }

        private void mf_button_Listup_Click(object sender, EventArgs e)
        {
            MoveFileListup();
        }

        private void MoveFileListup()
        {
            if (!Directory.Exists(mf_textBox_SourceDir.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + mf_textBox_SourceDir.Text);
                return;
            }

            // 移動元フォルダをリストアップ
            String[] Files = Directory.GetFiles(mf_textBox_SourceDir.Text);
            mf_listBox_Target.Items.Clear();
            for (int i = 0; i < Files.Length; i++)
            {
                String FileName = Files[i].Remove(0, mf_textBox_SourceDir.Text.Length + 1);   // "\\"の分を1加算
                mf_listBox_Target.Items.Add(FileName);
            }
            mf_label_TotalNum.Text = "ファイル数：" + Files.Length.ToString();
        }

        private void mf_listBox_Target_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void mf_listBox_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            mf_label_SelectNum.Text = "選択数：" + mf_listBox_Target.SelectedItems.Count.ToString();
        }

        private void mf_button_MoveFile_Click(object sender, EventArgs e)
        {
            if (!fio.EnsureDirectory(mf_textBox_TargetDir.Text))
            {
                return;
            }

            if (mf_listBox_Target.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            progressBar.Maximum = mf_listBox_Target.SelectedItems.Count;
            progressBar.Minimum = 0;
            progressBar.Value = 0;

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(mf_textBox_SourceDir.Text);
            arguments.Add(mf_textBox_TargetDir.Text);
            arguments.Add(mf_listBox_Target.SelectedItems.Count);
            for (int i = 0; i < mf_listBox_Target.SelectedItems.Count; i++)
            {
                arguments.Add(mf_listBox_Target.SelectedItems[i].ToString() );
            }

            bgWorkerMove.RunWorkerAsync(arguments);   // ⇒bgWorker_DoWork()
        }

        private void mf_textBox_SourceDir_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(mf_textBox_SourceDir.Text, e);
        }

        private void bgWorkerMove_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;
            String Sourcedir = (String)genericlist[0];
            String TargetDir = (String)genericlist[1];
            int ItemCount = (int)genericlist[2];

            for (int i = 0; i < ItemCount; i++)
            {
                String TargetName = (String)genericlist[3 + i];     // 3個目以降が対象のファイル名
                String SourcePath = Sourcedir + @"\" + TargetName;
                String TargetPath = TargetDir + @"\" + fio.GetLastPathName(TargetName);

                // 移動先にすでにフォルダがある場合は重複回避
                util.CreateFolderNameOverLapShirk(ref TargetPath, i);
                fio.MoveDirectory(SourcePath, TargetPath);

                worker.ReportProgress(i);      // ⇒ProgressChanged()

                // キャンセルされてないかチェック
                //if (worker.CancellationPending)
                //{
                //    e.Cancel = true;
                //    return;
                //}
            }
            worker.ReportProgress(ItemCount);

            // このメソッドからの戻り値
            e.Result = "すべて完了";

            // ⇒RunWorkerCompleted()
        }

        private void bgWorkerMove_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            progressText.Text = e.ProgressPercentage + "/" + progressBar.Maximum + " 完了";
            progressBar.Value = e.ProgressPercentage;
        }

        private void bgWorkerMove_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // この場合はe.Resultにはアクセスできない
                MessageBox.Show("キャンセルされました");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("ファイルの移動中にエラーが発生しました" + Environment.NewLine + e.Error.Message);
            }

            // リストを更新
            MoveFileListup();
        }
    }
}
