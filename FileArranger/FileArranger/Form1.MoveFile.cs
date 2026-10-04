using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // ファイル移動タブ(mf)の処理(Form1.csから分割)
    partial class FileArranger
    {
        private void mf_textBox_TargetDir_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(mf_textBox_TargetDir.Text, e);
        }

        private void mf_button_Listup_Click(object sender, EventArgs e)
        {
            ListupMoveFileTargets();
        }

        private void ListupMoveFileTargets()
        {
            if (!IsValidFolderPath(mf_textBox_SourceDir.Text))
            {
                return;
            }

            // 移動元フォルダをリストアップ
            String[] files = Directory.GetFiles(mf_textBox_SourceDir.Text);
            FillListBox(mf_listBox_Target, files, mf_textBox_SourceDir.Text);
            mf_label_TotalNum.Text = "ファイル数：" + files.Length.ToString();
        }

        private void mf_listBox_Target_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private void mf_listBox_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            mf_label_SelectNum.Text = FormatSelectedCount(mf_listBox_Target.SelectedItems.Count);
        }

        private void mf_button_MoveFile_Click(object sender, EventArgs e)
        {
            if (!_fio.EnsureDirectory(mf_textBox_TargetDir.Text))
            {
                return;
            }

            if (!HasSelectedItems(mf_listBox_Target.SelectedItems.Count))
            {
                return;
            }

            ResetProgressBar(mf_listBox_Target.SelectedItems.Count);

            // 別スレッドを非同期実行
            MoveFileWorkerParam param = new MoveFileWorkerParam
            {
                SourceDir = mf_textBox_SourceDir.Text,
                TargetDir = mf_textBox_TargetDir.Text,
            };
            foreach (object selectedItem in mf_listBox_Target.SelectedItems)
            {
                param.TargetNames.Add(selectedItem.ToString());
            }

            bgWorkerMove.RunWorkerAsync(param);   // ⇒bgWorker_DoWork()
        }

        private void mf_textBox_SourceDir_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(mf_textBox_SourceDir.Text, e);
        }

        private void bgWorkerMove_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            MoveFileWorkerParam param = (MoveFileWorkerParam)e.Argument;
            for (int i = 0; i < param.TargetNames.Count; i++)
            {
                String targetName = param.TargetNames[i];
                String sourcePath = param.SourceDir + @"\" + targetName;
                String targetPath = param.TargetDir + @"\" + _fio.GetLastPathName(targetName);

                // 移動先に同名のファイルがある場合は重複回避
                // (targetPathはファイルパスなので、フォルダの有無しか見ないAvoidFolderNameConflictでは
                //  同名ファイルの存在を検知できず、Move処理に失敗してしまう。AvoidFileNameConflictで
                //  ファイル/フォルダ両方の存在をチェックしてリネームする)
                _util.AvoidFileNameConflict(ref targetPath, i);
                _fio.MoveDirectory(sourcePath, targetPath);

                worker.ReportProgress(i);      // ⇒ProgressChanged()
            }
            worker.ReportProgress(param.TargetNames.Count);

            // このメソッドからの戻り値
            e.Result = "すべて完了";

            // ⇒RunWorkerCompleted()
        }

        private void bgWorkerMove_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            ShowProgress(e.ProgressPercentage);
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
            ListupMoveFileTargets();
        }
    }
}
