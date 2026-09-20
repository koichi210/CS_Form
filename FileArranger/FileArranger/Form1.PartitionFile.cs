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
    // フォルダ振り分けタブ(pf)の処理(Form1.csから分割。コードは移しただけで中身は変えていない)
    partial class FileArranger
    {
        private void pf_textBox_TargetFile_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(pf_textBox_TargetFile.Text, e);
        }

        private void pf_textBox_RefrenceFile_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(pf_textBox_RefrenceFile.Text, e);
        }

        private void pf_button_Listup_Target_Click(object sender, EventArgs e)
        {
            ListupTargetMoveDirectory();
        }

        private void pf_listView_Target_Update()
        {
            pf_listView_Target.Columns.Clear();

            // ListViewコントロールのプロパティを設定
            pf_listView_Target.FullRowSelect = true;
            pf_listView_Target.GridLines = true;
            pf_listView_Target.Sorting = SortOrder.Ascending;
            pf_listView_Target.View = View.Details;

            // 列（コラム）ヘッダの作成
            ColumnHeader columnTarget = new ColumnHeader();
            columnTarget.Text = PartitionFileColumn[0];
            columnTarget.Width = pf_listView_Target.Width / PartitionFileColumn.Length;

            ColumnHeader columnMoveSrc = new ColumnHeader();
            columnMoveSrc.Text = PartitionFileColumn[1];
            columnMoveSrc.Width = pf_listView_Target.Width / PartitionFileColumn.Length;

            ColumnHeader columnMoveDest = new ColumnHeader();
            columnMoveDest.Text = PartitionFileColumn[2];
            columnMoveDest.Width = pf_listView_Target.Width / PartitionFileColumn.Length;

            ColumnHeader[] colHeaderRegValue = { columnTarget, columnMoveSrc, columnMoveDest };
            pf_listView_Target.Columns.AddRange(colHeaderRegValue);
        }

        private void ListupTargetMoveDirectory(bool IsErrorPopup = true)
        {
            if (!Directory.Exists(pf_textBox_TargetFile.Text))
            {
                if (IsErrorPopup )
                {
                    MessageBox.Show("フォルダパスが不正です。" + pf_textBox_TargetFile.Text);
                }
                return;
            }

            // 移動元フォルダをリストアップ
            String[] Files = Directory.GetFiles(pf_textBox_TargetFile.Text);
            pf_listView_Target.Items.Clear();
            for (int i = 0; i < Files.Length; i++)
            {
                String FileName = Files[i].Remove(0, pf_textBox_TargetFile.Text.Length + 1);   // "\\"の分を1加算

                String[] item = { FileName, "", "" };
                pf_listView_Target.Items.Add(new ListViewItem(item));
            }
            pf_label_TotalNum.Text = "ファイル数：" + Files.Length.ToString();

            // 左端しかAutoResizeしないので、使い勝手悪い。。
            //pf_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            pf_listView_Target_Update();
        }

        private void pf_listView_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            pf_button_ClearSelect_Click(sender, e);
            pf_label_SelectNum.Text = "選択数：" + pf_listView_Target.SelectedItems.Count.ToString();

            UpdatePartitionFileList();

            // 単独ファイル選択時はコンボボックスに表示する
            if (pf_listView_Target.SelectedItems.Count != 0)
            {
                int Idx = pf_listView_Target.SelectedItems[0].Index;
                pf_comboBox_MoveDestDirName.Text = pf_listView_Target.Items[Idx].SubItems[CreateFolderMoveDestIdx].Text;
            }
        }

        private Boolean GetPartitionNameFromListView(ref String SrcFolderName, ref String TargetFolderName, String SrcFileName)
        {
            Boolean IsSuccess = true;

            int SameIdx = util.GetStringFromListViewInSelect(pf_listView_Target, CreateFolderTargetIdx, SrcFileName, pf_textBox_TargetSeprator.Text, true);
            if (0 <= SameIdx)
            {
                SrcFolderName = pf_listView_Target.Items[SameIdx].SubItems[CreateFolderMoveSrcIdx].Text;
                TargetFolderName = pf_listView_Target.Items[SameIdx].SubItems[CreateFolderMoveDestIdx].Text;
            }

            if (TargetFolderName == String.Empty)
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }

        private Boolean GetPartitionNameFromComboBox(ref String SrcFolderName, ref String TargetFolderName, String SrcFileName)
        {
            Boolean IsSuccess = true;

            TargetFolderName = util.FindStringFromComboBox(pf_comboBox_MoveDestDirName, SrcFileName, pf_textBox_TargetSeprator.Text, true);
            if (TargetFolderName != String.Empty)
            {
                // 期待するフォルダ名が見つかった
                SrcFolderName = TargetFolderName;

                // 数値をインクリした文字列
                TargetFolderName = GetPatitionTargetNameWithNumber(SrcFolderName, SrcFileName, "0");
            }

            if (TargetFolderName == String.Empty)
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }

        private void CreatePartitionName(ref String SrcFolderName, ref String TargetFolderName, String SrcFileName)
        {
            // 期待するフォルダ名が見つからなかった
            SrcFolderName = "";
            String SampleSrcFolderName = util.CreateNewFolderName(SrcFileName, pf_textBox_TargetSeprator.Text, true);
            SampleSrcFolderName += cmn_textBox_AddListSuffix.Text;

            // 数値を考慮した文字列
            TargetFolderName = GetPatitionTargetNameWithNumber(SampleSrcFolderName, SrcFileName, "0");    // 複数ファイル選択時にインクリしてくれる
        }

        private String GetPatitionTargetNameWithNumber(String SrcFolderName, String SrcFileName, String DefaultNumber)
        {
            long SrcNumber = util.GetNumberFromRear(SrcFolderName, pf_textBox_SearchTitleLine.Text, pf_textBox_SearchTitleLength.Text, DefaultNumber);
            int AddCount = Logic.GetAddCount(pf_listView_Target, SrcFileName, pf_textBox_TargetSeprator.Text, true);

            String Number = Logic.GetNumber(SrcNumber, AddCount);
            int SrcNumberDigit = Logic.GetPadding(SrcNumber);

            return SrcFolderName.Substring(0, SrcFolderName.Length - SrcNumberDigit) + Number;
        }

        private void UpdatePartitionFileList()
        {
            for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = pf_listView_Target.SelectedItems[i].Index;

                String SrcFileName = pf_listView_Target.Items[idx].SubItems[CreateFolderTargetIdx].Text;
                String SrcFolderName = "";
                String TargetFolderName = "";

                Boolean IsSuccess = false;
                // ListViewに既出であれば流量
                IsSuccess = GetPartitionNameFromListView(ref SrcFolderName, ref TargetFolderName, SrcFileName);

                if (!IsSuccess)
                {
                    // ListViewに無ければCombBoxから検索
                    IsSuccess = GetPartitionNameFromComboBox(ref SrcFolderName, ref TargetFolderName, SrcFileName);
                }

                if (!IsSuccess)
                {
                    if (pf_checkBox_CreateNewDir.Checked)
                    {
                        // CombBoxにもなかったら新規作成
                        CreatePartitionName(ref SrcFolderName, ref TargetFolderName, SrcFileName);
                    }
                }

                pf_listView_Target.Items[idx].SubItems[CreateFolderMoveSrcIdx].Text = SrcFolderName;
                pf_listView_Target.Items[idx].SubItems[CreateFolderMoveDestIdx].Text = TargetFolderName;
            }
        }

        private void pf_button_ClearSelect_Click(object sender, EventArgs e)
        {
            // 選択解除
            for (int i = 0; i < pf_listView_Target.Items.Count; i++)
            {
                pf_listView_Target.Items[i].SubItems[CreateFolderMoveSrcIdx].Text = "";
                pf_listView_Target.Items[i].SubItems[CreateFolderMoveDestIdx].Text = "";
            }
        }

        private void pf_button_CreateFolderExecute_Click(object sender, EventArgs e)
        {
            MovePartitionFile();
        }

        private void MovePartitionFile()
        {
            if (pf_listView_Target.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            progressBar.Maximum = pf_listView_Target.SelectedItems.Count;
            progressBar.Minimum = 0;
            progressBar.Value = 0;

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(pf_textBox_TargetFile.Text);
            arguments.Add(pf_textBox_RefrenceFile.Text);
            arguments.Add(pf_listView_Target.SelectedItems.Count);

            for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
            {
                int idx = pf_listView_Target.SelectedItems[i].Index;
                arguments.Add(pf_listView_Target.Items[idx].SubItems[CreateFolderTargetIdx].Text);
                arguments.Add(pf_listView_Target.Items[idx].SubItems[CreateFolderMoveSrcIdx].Text);
                arguments.Add(pf_listView_Target.Items[idx].SubItems[CreateFolderMoveDestIdx].Text);
            }
            bgPartition.RunWorkerAsync(arguments);   // ⇒bgPartition_DoWork()
        }

        private void pf_listView_Target_DoubleClick(object sender, EventArgs e)
        {
            int idx = pf_listView_Target.SelectedItems[0].Index;
            String DirPath = pf_textBox_RefrenceFile.Text + @"\" + pf_listView_Target.Items[idx].SubItems[CreateFolderMoveSrcIdx].Text;
            if (Directory.Exists(DirPath))
            {
                util.ExecutePath(DirPath);
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
                        pf_listView_Target.Items[idx].SubItems[CreateFolderMoveDestIdx].Text = "";
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
                    pf_listView_Target.Items[idx].SubItems[CreateFolderMoveDestIdx].Text = pf_comboBox_MoveDestDirName.Text;
                }
            }
        }

        private void bgPartition_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;
            String TargetFilePath = (String)genericlist[0];
            String TargetDir = (String)genericlist[1];
            int ItemCount = (int)genericlist[2];

            // 以前は"generic_index"という1つの変数を「先頭からのオフセット」と
            // 「1件あたりの間隔(ストライド)」の2つの意味で使い回していた
            // (たまたま両方とも3だったので動いていたが、読み手には分かりにくかった)。
            // 意味ごとに変数を分けて、それぞれの役割を明確にした(値・挙動は変えていない)。
            const int BaseOffset = 3;   // 4個目(genericlist[3])から対象データが始まる
            const int Stride = 3;       // 1件あたり(対象名/移動前名称/移動後名称)3個おき

            // このスレッドから直接MessageBoxを出さず、完了時にUIスレッドへまとめて渡す
            List<String> Messages = new List<String>();
            for (int i = 0; i < ItemCount; i++)
            {
                String TargetName = (String)genericlist[BaseOffset + i * Stride + 0]; // 4個目以降が対象[3個おき]
                String MoveSrc = (String)genericlist[BaseOffset + i * Stride + 1];    // 5個目以降が対象[3個おき]
                String MoveDest = (String)genericlist[BaseOffset + i * Stride + 2];   // 6個目以降が対象[3個おき]

                if (MoveDest == String.Empty)
                {
                    // [移動後名称]が無い項目は処理対象外
                    continue;
                }

                // TODO：変数名整理したい
                String SrcDirName = TargetFilePath;
                String DestPreDirName = TargetDir + @"\" + MoveSrc;
                String DestPostDirName = TargetDir + @"\" + MoveDest;

                String FileName = TargetName;
                String SrcFileName = SrcDirName + @"\" + FileName;
                String DestFileName = DestPostDirName + @"\" + FileName;

                try
                {
                    // 移動先フォルダを生成
                    if (MoveSrc != String.Empty && // 移動元がカラじゃない
                         System.IO.Directory.Exists(DestPreDirName)     // フォルダが存在する
                         )
                    {
                        // 元フォルダと先フォルダが違うときだけ移動
                        if (DestPreDirName != DestPostDirName)
                        {
                            // フォルダ名が変わるのであればリネーム
                            System.IO.Directory.Move(DestPreDirName, DestPostDirName);
                        }
                    }
                    else
                    {
                        // 元フォルダが無かったら新規フォルダなので、先フォルダを作成
                        System.IO.Directory.CreateDirectory(DestPostDirName);
                    }

                    // ファイル名の重複回避
                    if (!util.CreateFileNameOverLapShirk(ref DestFileName, i))
                    {
                        Messages.Add("ファイル名が重複したので処理をスキップしました：" + FileName);
                        continue;
                    }
                    File.Move(SrcFileName, DestFileName);
                }
                catch (Exception)
                {
                    Messages.Add("エラーが発生したので処理を中断しました。" + Environment.NewLine +
                                 "移動元：" + SrcFileName + Environment.NewLine +
                                 "移動先：" + DestFileName);
                    break;
                }

                worker.ReportProgress(i);      // ⇒ProgressChanged()
            }
            worker.ReportProgress(ItemCount);

            // このメソッドからの戻り値
            e.Result = Messages;

            // ⇒RunWorkerCompleted()
        }

        private void bgPartition_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            progressText.Text = e.ProgressPercentage + "/" + progressBar.Maximum + " 完了";
            progressBar.Value = e.ProgressPercentage;
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
                List<String> Messages = e.Result as List<String>;
                if (Messages != null && Messages.Count > 0)
                {
                    MessageBox.Show(String.Join(Environment.NewLine, Messages.ToArray()),
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            }

            // 選択解除
            for (int i = 0; i < pf_listView_Target.Items.Count; i++)
            {
                pf_listView_Target.Items[i].SubItems[CreateFolderMoveSrcIdx].Text = "";
                pf_listView_Target.Items[i].SubItems[CreateFolderMoveDestIdx].Text = "";
            }

            // リストを更新
            ListupTargetMoveDirectory(false);

            //リファレンスフォルダは自動更新しない
            //UpdateMoveDestDirComboBox();
        }

        private void pf_label_RefrenceFile_DoubleClick(object sender, EventArgs e)
        {
            pf_textBox_RefrenceFile.ReadOnly = !pf_textBox_RefrenceFile.ReadOnly;
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
            ListupTargetMoveDirectory(false);
        }
    }
}
