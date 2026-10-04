using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    // フォルダ振り分けタブ(pf)の処理(Form1.csから分割)
    partial class FileArranger
    {
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
            String[] files = ListupInto(pf_listView_Target, _partitionFileColumns.Length, pf_label_TotalNum, "ファイル数",
                pf_textBox_TargetFile.Text, Directory.GetFiles, showErrorPopup);
            if (files == null)
            {
                return;
            }

            // AutoResizeColumnsは左端しか広げないので、列を均等幅で作り直す
            RecreatePartitionColumnsEvenly();
        }

        private void pf_listView_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            _partitionSelectionUpdate.Request();   // ⇒RefreshPartitionMoveNames()
        }

        // 選択が変わった時: 一旦全行の[移動前名称][移動後名称]を消してから、選択中の行だけ作り直す
        private void RefreshPartitionMoveNames()
        {
            pf_listView_Target.BeginUpdate();
            ClearPartitionMoveNames();
            List<ListViewItem> selectedItems = UpdatePartitionFileList();
            pf_listView_Target.EndUpdate();

            pf_label_SelectNum.Text = FormatSelectedCount(selectedItems.Count);

            // 単独ファイル選択時はコンボボックスに表示する
            if (selectedItems.Count != 0)
            {
                pf_comboBox_MoveDestDirName.Text = selectedItems[0].SubItems[_partitionMoveDestIdx].Text;
            }
        }

        // 選択中の各ファイルについて[移動前名称][移動後名称]を決めて表示する。決めた対象(選択項目)を返す
        private List<ListViewItem> UpdatePartitionFileList()
        {
            List<ListViewItem> selectedItems = Utils.GetSelectedItems(pf_listView_Target);
            // 番号の加算数を数える時に使う、選択中ファイル名の一覧(ループ中は変わらないので1回だけ取る)
            List<String> selectedNames = selectedItems.Select(item => item.SubItems[_partitionTargetIdx].Text).ToList();

            foreach (ListViewItem item in selectedItems)
            {
                String srcFileName = item.SubItems[_partitionTargetIdx].Text;
                String srcFolderName;
                String targetFolderName;

                // ①選択中の別の行で既に決まっていれば流用 ②無ければ振り分け先の候補から検索
                // ③それも無く新規作成ONなら新しいフォルダ名を作る
                if (!TryGetPartitionNameFromListView(selectedItems, srcFileName, out srcFolderName, out targetFolderName)
                    && !TryGetPartitionNameFromComboBox(selectedNames, srcFileName, out srcFolderName, out targetFolderName)
                    && pf_checkBox_CreateNewDir.Checked)
                {
                    CreatePartitionName(selectedNames, srcFileName, out srcFolderName, out targetFolderName);
                }

                item.SubItems[_partitionMoveSrcIdx].Text = srcFolderName;
                item.SubItems[_partitionMoveDestIdx].Text = targetFolderName;
            }

            return selectedItems;
        }

        private Boolean TryGetPartitionNameFromListView(List<ListViewItem> selectedItems, String srcFileName, out String srcFolderName, out String targetFolderName)
        {
            ListViewItem sameItem = _util.FindItem(selectedItems, _partitionTargetIdx, srcFileName, pf_textBox_TargetSeparator.Text, true);
            srcFolderName = sameItem != null ? sameItem.SubItems[_partitionMoveSrcIdx].Text : "";
            targetFolderName = sameItem != null ? sameItem.SubItems[_partitionMoveDestIdx].Text : "";

            return targetFolderName != String.Empty;
        }

        private Boolean TryGetPartitionNameFromComboBox(List<String> selectedNames, String srcFileName, out String srcFolderName, out String targetFolderName)
        {
            String foundFolderName = _util.FindStringFromComboBox(pf_comboBox_MoveDestDirName, srcFileName, pf_textBox_TargetSeparator.Text, true);
            if (foundFolderName == String.Empty)
            {
                srcFolderName = "";
                targetFolderName = "";
                return false;
            }

            // 期待するフォルダ名が見つかった。移動後は番号をインクリした名前にする
            srcFolderName = foundFolderName;
            targetFolderName = GetPartitionTargetNameWithNumber(selectedNames, foundFolderName, srcFileName);
            return targetFolderName != String.Empty;
        }

        // 期待するフォルダ名が見つからなかった時に、ファイル名から新しいフォルダ名を作る
        private void CreatePartitionName(List<String> selectedNames, String srcFileName, out String srcFolderName, out String targetFolderName)
        {
            srcFolderName = "";
            String sampleSrcFolderName = _util.CreateNewFolderName(srcFileName, pf_textBox_TargetSeparator.Text, true)
                + cmn_textBox_AddListSuffix.Text;

            // 数値を考慮した文字列(複数ファイル選択時にインクリしてくれる)
            targetFolderName = GetPartitionTargetNameWithNumber(selectedNames, sampleSrcFolderName, srcFileName);
        }

        private String GetPartitionTargetNameWithNumber(List<String> selectedNames, String srcFolderName, String srcFileName)
        {
            long srcNumber = _util.GetNumberFromRear(srcFolderName, pf_textBox_SearchTitleLine.Text, pf_textBox_SearchTitleLength.Text, "0");
            int addCount = Logic.GetAddCount(selectedNames, srcFileName, pf_textBox_TargetSeparator.Text, true);

            String number = Logic.ToPaddedNumberString(srcNumber, addCount);
            int srcNumberDigits = Logic.GetPaddingDigits(srcNumber);

            return srcFolderName.Substring(0, srcFolderName.Length - srcNumberDigits) + number;
        }

        private void pf_button_ClearSelect_Click(object sender, EventArgs e)
        {
            ClearPartitionMoveNames();
        }

        // 選択解除([移動前名称][移動後名称]列を空にする)
        private void ClearPartitionMoveNames()
        {
            Utils.SetSubItemText(pf_listView_Target, pf_listView_Target.Items.Cast<ListViewItem>(), "",
                _partitionMoveSrcIdx, _partitionMoveDestIdx);
        }

        // 選択中の行の[移動後名称]をまとめて書き換える
        private void SetSelectedMoveDestName(String moveDestName)
        {
            Utils.SetSubItemText(pf_listView_Target, Utils.GetSelectedItems(pf_listView_Target), moveDestName, _partitionMoveDestIdx);
        }

        private void pf_button_CreateFolderExecute_Click(object sender, EventArgs e)
        {
            MovePartitionFile();
        }

        private void MovePartitionFile()
        {
            List<ListViewItem> selectedItems = Utils.GetSelectedItems(pf_listView_Target);
            if (!HasSelectedItems(selectedItems.Count))
            {
                return;
            }

            // 別スレッドを非同期実行
            PartitionWorkerParam param = new PartitionWorkerParam
            {
                SourceDir = pf_textBox_TargetFile.Text,
                ReferenceDir = pf_textBox_ReferenceFile.Text,
            };
            param.Items.AddRange(selectedItems.Select(item => new PartitionWorkerParam.Item
            {
                TargetName = item.SubItems[_partitionTargetIdx].Text,
                MoveSrc = item.SubItems[_partitionMoveSrcIdx].Text,
                MoveDest = item.SubItems[_partitionMoveDestIdx].Text,
            }));

            ResetProgressBar(param.Items.Count);
            bgPartition.RunWorkerAsync(param);   // ⇒bgPartition_DoWork()
        }

        private void pf_listView_Target_DoubleClick(object sender, EventArgs e)
        {
            if (pf_listView_Target.SelectedItems.Count == 0)
            {
                return;
            }

            String dirPath = Path.Combine(pf_textBox_ReferenceFile.Text, pf_listView_Target.SelectedItems[0].SubItems[_partitionMoveSrcIdx].Text);
            if (Directory.Exists(dirPath))
            {
                _util.ExecutePath(dirPath);
            }
        }

        private void pf_listView_Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MovePartitionFile();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                SetSelectedMoveDestName("");
            }
        }

        private void pf_comboBox_MoveDestDirName_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsCtrlEnter(e))
            {
                MovePartitionFile();
            }
            else
            {
                SetSelectedMoveDestName(pf_comboBox_MoveDestDirName.Text);
            }
        }

        private void bgPartition_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない
            BackgroundWorker worker = (BackgroundWorker)sender;
            PartitionWorkerParam param = (PartitionWorkerParam)e.Argument;

            // このスレッドから直接MessageBoxを出さず、完了時にUIスレッドへまとめて渡す
            List<String> messages = new List<String>();
            for (int i = 0; i < param.Items.Count; i++)
            {
                PartitionWorkerParam.Item item = param.Items[i];

                // ここまでに終わった件数(スキップした項目も1件と数える)
                worker.ReportProgress(i);      // ⇒bgWorker_ProgressChanged()

                if (item.MoveDest == String.Empty)
                {
                    // [移動後名称]が無い項目は処理対象外
                    continue;
                }

                // 振り分け先フォルダ(リネーム前 / リネーム後)
                String oldDestDir = Path.Combine(param.ReferenceDir, item.MoveSrc);
                String newDestDir = Path.Combine(param.ReferenceDir, item.MoveDest);

                String srcFilePath = Path.Combine(param.SourceDir, item.TargetName);
                String destFilePath = Path.Combine(newDestDir, item.TargetName);

                try
                {
                    // 移動先フォルダを生成
                    if (item.MoveSrc == String.Empty || !Directory.Exists(oldDestDir))
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
                        messages.Add("ファイル名が重複したので処理をスキップしました：" + item.TargetName);
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
            }
            worker.ReportProgress(param.Items.Count);

            // ⇒bgPartition_RunWorkerCompleted()
            e.Result = messages;
        }

        private void bgPartition_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsWorkerCompletedNormally(e, "フォルダ分けの途中でエラーが発生しました"))
            {
                // 別スレッド側で溜めたメッセージを、UIスレッドであるここでまとめて出す
                List<String> messages = (List<String>)e.Result;
                if (messages.Count > 0)
                {
                    MessageBox.Show(String.Join(Environment.NewLine, messages),
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            }

            // 選択解除
            ClearPartitionMoveNames();

            // リストを更新(リファレンスフォルダは自動更新しない)
            ListupPartitionTargetFiles(false);
        }

        private void pf_label_ReferenceFile_DoubleClick(object sender, EventArgs e)
        {
            pf_textBox_ReferenceFile.ReadOnly = !pf_textBox_ReferenceFile.ReadOnly;
        }

        private void pf_comboBox_MoveDestDirName_DropDown(object sender, EventArgs e)
        {
            UpdateMoveDestDirComboBox();
        }

        // pfタブの番号入力欄で共通: Ctrl+Enterでファイル移動
        private void pf_PartitionInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsCtrlEnter(e))
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
