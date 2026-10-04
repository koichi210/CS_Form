using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // フォルダ振り分けタブ(pf)の処理(Form1.csから分割)
    partial class FileArranger
    {
        private void pf_textBox_TargetFile_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(pf_textBox_TargetFile.Text, e);
        }

        private void pf_textBox_ReferenceFile_KeyDown(object sender, KeyEventArgs e)
        {
            _util.ExecutePath(pf_textBox_ReferenceFile.Text, e);
        }

        private void pf_button_Listup_Target_Click(object sender, EventArgs e)
        {
            ListupPartitionTargetFiles();
        }

        private void RecreatePartitionColumnsEvenly()
        {
            RecreateColumnsEvenly(pf_listView_Target, _partitionFileColumns);
        }

        private void ListupPartitionTargetFiles(bool showErrorPopup = true)
        {
            if (!IsValidFolderPath(pf_textBox_TargetFile.Text, showErrorPopup))
            {
                return;
            }

            // 移動元フォルダをリストアップ
            String[] files = Directory.GetFiles(pf_textBox_TargetFile.Text);
            FillListView(pf_listView_Target, files, pf_textBox_TargetFile.Text, _partitionFileColumns.Length);
            pf_label_TotalNum.Text = "ファイル数：" + files.Length.ToString();

            // AutoResizeColumnsは左端しか広げないので、列を均等幅で作り直す
            RecreatePartitionColumnsEvenly();
        }

        private void pf_listView_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearPartitionMoveNames();
            pf_label_SelectNum.Text = FormatSelectedCount(pf_listView_Target.SelectedItems.Count);

            UpdatePartitionFileList();

            // 単独ファイル選択時はコンボボックスに表示する
            if (pf_listView_Target.SelectedItems.Count != 0)
            {
                int idx = pf_listView_Target.SelectedItems[0].Index;
                pf_comboBox_MoveDestDirName.Text = pf_listView_Target.Items[idx].SubItems[_partitionMoveDestIdx].Text;
            }
        }

        private Boolean GetPartitionNameFromListView(ref String srcFolderName, ref String targetFolderName, String srcFileName)
        {
            int sameIdx = _util.FindSelectedRowIndex(pf_listView_Target, _partitionTargetIdx, srcFileName, pf_textBox_TargetSeparator.Text, true);
            if (0 <= sameIdx)
            {
                srcFolderName = pf_listView_Target.Items[sameIdx].SubItems[_partitionMoveSrcIdx].Text;
                targetFolderName = pf_listView_Target.Items[sameIdx].SubItems[_partitionMoveDestIdx].Text;
            }

            return targetFolderName != String.Empty;
        }

        private Boolean GetPartitionNameFromComboBox(ref String srcFolderName, ref String targetFolderName, String srcFileName)
        {
            targetFolderName = _util.FindStringFromComboBox(pf_comboBox_MoveDestDirName, srcFileName, pf_textBox_TargetSeparator.Text, true);
            if (targetFolderName != String.Empty)
            {
                // 期待するフォルダ名が見つかった
                srcFolderName = targetFolderName;

                // 数値をインクリした文字列
                targetFolderName = GetPartitionTargetNameWithNumber(srcFolderName, srcFileName, "0");
            }

            return targetFolderName != String.Empty;
        }

        private void CreatePartitionName(ref String srcFolderName, ref String targetFolderName, String srcFileName)
        {
            // 期待するフォルダ名が見つからなかった
            srcFolderName = "";
            String sampleSrcFolderName = _util.CreateNewFolderName(srcFileName, pf_textBox_TargetSeparator.Text, true);
            sampleSrcFolderName += cmn_textBox_AddListSuffix.Text;

            // 数値を考慮した文字列
            targetFolderName = GetPartitionTargetNameWithNumber(sampleSrcFolderName, srcFileName, "0");    // 複数ファイル選択時にインクリしてくれる
        }

        private String GetPartitionTargetNameWithNumber(String srcFolderName, String srcFileName, String defaultNumber)
        {
            long srcNumber = _util.GetNumberFromRear(srcFolderName, pf_textBox_SearchTitleLine.Text, pf_textBox_SearchTitleLength.Text, defaultNumber);
            int addCount = Logic.GetAddCount(pf_listView_Target, srcFileName, pf_textBox_TargetSeparator.Text, true);

            String number = Logic.ToPaddedNumberString(srcNumber, addCount);
            int srcNumberDigits = Logic.GetPaddingDigits(srcNumber);

            return srcFolderName.Substring(0, srcFolderName.Length - srcNumberDigits) + number;
        }

        private void UpdatePartitionFileList()
        {
            for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = pf_listView_Target.SelectedItems[i].Index;

                String srcFileName = pf_listView_Target.Items[idx].SubItems[_partitionTargetIdx].Text;
                String srcFolderName = "";
                String targetFolderName = "";

                // ListViewに既出であれば流用
                Boolean isSuccess = GetPartitionNameFromListView(ref srcFolderName, ref targetFolderName, srcFileName);

                if (!isSuccess)
                {
                    // ListViewに無ければComboBoxから検索
                    isSuccess = GetPartitionNameFromComboBox(ref srcFolderName, ref targetFolderName, srcFileName);
                }

                if (!isSuccess && pf_checkBox_CreateNewDir.Checked)
                {
                    // ComboBoxにもなかったら新規作成
                    CreatePartitionName(ref srcFolderName, ref targetFolderName, srcFileName);
                }

                pf_listView_Target.Items[idx].SubItems[_partitionMoveSrcIdx].Text = srcFolderName;
                pf_listView_Target.Items[idx].SubItems[_partitionMoveDestIdx].Text = targetFolderName;
            }
        }

        private void pf_button_ClearSelect_Click(object sender, EventArgs e)
        {
            ClearPartitionMoveNames();
        }

        // 選択解除([移動前名称][移動後名称]列を空にする)
        private void ClearPartitionMoveNames()
        {
            for (int i = 0; i < pf_listView_Target.Items.Count; i++)
            {
                pf_listView_Target.Items[i].SubItems[_partitionMoveSrcIdx].Text = "";
                pf_listView_Target.Items[i].SubItems[_partitionMoveDestIdx].Text = "";
            }
        }

        private void pf_button_CreateFolderExecute_Click(object sender, EventArgs e)
        {
            MovePartitionFile();
        }

        private void MovePartitionFile()
        {
            if (!HasSelectedItems(pf_listView_Target.SelectedItems.Count))
            {
                return;
            }

            ResetProgressBar(pf_listView_Target.SelectedItems.Count);

            // 別スレッドを非同期実行
            PartitionWorkerParam param = new PartitionWorkerParam
            {
                SourceDir = pf_textBox_TargetFile.Text,
                ReferenceDir = pf_textBox_ReferenceFile.Text,
            };

            for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
            {
                int idx = pf_listView_Target.SelectedItems[i].Index;
                param.Items.Add(new PartitionWorkerParam.Item
                {
                    TargetName = pf_listView_Target.Items[idx].SubItems[_partitionTargetIdx].Text,
                    MoveSrc = pf_listView_Target.Items[idx].SubItems[_partitionMoveSrcIdx].Text,
                    MoveDest = pf_listView_Target.Items[idx].SubItems[_partitionMoveDestIdx].Text,
                });
            }
            bgPartition.RunWorkerAsync(param);   // ⇒bgPartition_DoWork()
        }

        private void pf_listView_Target_DoubleClick(object sender, EventArgs e)
        {
            int idx = pf_listView_Target.SelectedItems[0].Index;
            String dirPath = pf_textBox_ReferenceFile.Text + @"\" + pf_listView_Target.Items[idx].SubItems[_partitionMoveSrcIdx].Text;
            if (Directory.Exists(dirPath))
            {
                _util.ExecutePath(dirPath);
            }
        }

        private void pf_listView_Target_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    MovePartitionFile();
                    break;

                case Keys.Delete:
                    for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
                    {
                        // 参照しているListViewのIdx
                        int idx = pf_listView_Target.SelectedItems[i].Index;
                        pf_listView_Target.Items[idx].SubItems[_partitionMoveDestIdx].Text = "";
                    }
                    break;

                default:
                    break;
            }
        }

        private void pf_comboBox_MoveDestDirName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                MovePartitionFile();
            }
            else
            {
                for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
                {
                    int idx = pf_listView_Target.SelectedItems[i].Index;
                    pf_listView_Target.Items[idx].SubItems[_partitionMoveDestIdx].Text = pf_comboBox_MoveDestDirName.Text;
                }
            }
        }

        private void bgPartition_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            PartitionWorkerParam param = (PartitionWorkerParam)e.Argument;
            int itemCount = param.Items.Count;

            // このスレッドから直接MessageBoxを出さず、完了時にUIスレッドへまとめて渡す
            List<String> messages = new List<String>();
            for (int i = 0; i < itemCount; i++)
            {
                String fileName = param.Items[i].TargetName;
                String moveSrc = param.Items[i].MoveSrc;
                String moveDest = param.Items[i].MoveDest;

                if (moveDest == String.Empty)
                {
                    // [移動後名称]が無い項目は処理対象外
                    continue;
                }

                // 振り分け先フォルダ(リネーム前 / リネーム後)
                String oldDestDir = param.ReferenceDir + @"\" + moveSrc;
                String newDestDir = param.ReferenceDir + @"\" + moveDest;

                String srcFilePath = param.SourceDir + @"\" + fileName;
                String destFilePath = newDestDir + @"\" + fileName;

                try
                {
                    // 移動先フォルダを生成
                    if (moveSrc == String.Empty || !Directory.Exists(oldDestDir))
                    {
                        // 元フォルダが無かったら新規フォルダなので、先フォルダを作成
                        Directory.CreateDirectory(newDestDir);
                    }
                    else if (oldDestDir != newDestDir)
                    {
                        // フォルダ名が変わるのであればリネーム
                        Directory.Move(oldDestDir, newDestDir);
                    }

                    // ファイル名の重複回避
                    if (!_util.AvoidFileNameConflict(ref destFilePath, i))
                    {
                        messages.Add("ファイル名が重複したので処理をスキップしました：" + fileName);
                        continue;
                    }
                    File.Move(srcFilePath, destFilePath);
                }
                catch (Exception)
                {
                    messages.Add("エラーが発生したので処理を中断しました。" + Environment.NewLine +
                                 "移動元：" + srcFilePath + Environment.NewLine +
                                 "移動先：" + destFilePath);
                    break;
                }

                worker.ReportProgress(i);      // ⇒ProgressChanged()
            }
            worker.ReportProgress(itemCount);

            // このメソッドからの戻り値
            e.Result = messages;

            // ⇒RunWorkerCompleted()
        }

        private void bgPartition_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            ShowProgress(e.ProgressPercentage);
        }

        private void bgPartition_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // この場合はe.Resultにはアクセスできない
                MessageBox.Show("キャンセルされました");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("フォルダ分けの途中でエラーが発生しました" + Environment.NewLine + e.Error.Message);
            }
            else
            {
                // 別スレッド側で溜めたメッセージを、UIスレッドであるここでまとめて出す
                List<String> messages = e.Result as List<String>;
                if (messages != null && messages.Count > 0)
                {
                    MessageBox.Show(String.Join(Environment.NewLine, messages),
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            }

            // 選択解除
            ClearPartitionMoveNames();

            // リストを更新
            ListupPartitionTargetFiles(false);

            //リファレンスフォルダは自動更新しない
            //UpdateMoveDestDirComboBox();
        }

        private void pf_label_ReferenceFile_DoubleClick(object sender, EventArgs e)
        {
            pf_textBox_ReferenceFile.ReadOnly = !pf_textBox_ReferenceFile.ReadOnly;
        }

        private void pf_comboBox_MoveDestDirName_DropDown(object sender, EventArgs e)
        {
            UpdateMoveDestDirComboBox();
        }

        private void pf_textBox_SearchTitleLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                MovePartitionFile();
            }
        }

        private void pf_checkBox_CreateNewDir_CheckedChanged(object sender, EventArgs e)
        {
            ListupPartitionTargetFiles(false);
        }
    }
}
