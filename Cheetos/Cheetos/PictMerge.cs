using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using Picture;

namespace Cheetos
{
    // PictMerge
    public partial class Cheetos : Form
    {
        private void MergeExec()
        {
            Debug.WriteData("MergeExec_Click" + Environment.NewLine, false);

            // キャンセル
            if (bkgWorkerMerge.IsBusy)
            {
                bkgWorkerMerge.CancelAsync();
                return;
            }

            String BackUpDirPath = pm_SourceFolderPath.Text + @"\" + @"Bk_Merge";
            if (!fio.IsExistDirectory(BackUpDirPath))
            {
                MessageBox.Show("無効なフォルダパスです。\n" + BackUpDirPath);
                return;
            }

            InitProgressBar(pm_ListBox_ListUp.SelectedItems.Count);

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(BackUpDirPath);
            arguments.Add(pm_SourceFolderPath.Text);
            arguments.Add(pm_SourceFile1Prefix.Text);
            arguments.Add(pm_SourceFile2Prefix.Text);

            Debug.WriteData("BackUpDirPath = " + BackUpDirPath);
            Debug.WriteData("SourceFolderPath = " + pm_SourceFolderPath.Text);
            Debug.WriteData("Prefix1 = " + pm_SourceFile1Prefix.Text);
            Debug.WriteData("Prefix2 = " + pm_SourceFile2Prefix.Text);

            // ListBoxの値を配列で取得
            String[] StrArray = util.GetStrArrayFromListBox(pm_ListBox_ListUp.SelectedItems);
            arguments.Add(StrArray);

            // 切断基準となる高さ
            String[] TrimHeightAry = pm_TrimingHeight.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            arguments.Add(TrimHeightAry);

            SetStartTime();
            pm_MergeExec_Click.Text = "中断";
            bkgWorkerMerge.RunWorkerAsync(arguments);   // ⇒DoWork()
        }

        private void ListupPictMerge()
        {
            if (!Directory.Exists(pm_SourceFolderPath.Text))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }
            pm_ListBox_ListUp.Items.Clear();

            string[] files = Directory.GetFiles(pm_SourceFolderPath.Text, "*", SearchOption.TopDirectoryOnly);

            //配列の内容を一つ一つ追加する
            for (int i = 0; i <= files.Length - 1; i++)
            {
                var FileName = Path.GetFileName(files[i]);
                pm_ListBox_ListUp.Items.Add(FileName);
            }
        }

        private void pm_SourceFolderPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(pm_SourceFolderPath.Text, e);
        }

        private void bkgWorkerMerge_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;

            String BackUpDirPath = (String)genericlist[0];      // BackUpDirPath
            String SourceFolderPath = (String)genericlist[1];   // pm_SourceFolderPath.Text
            String SourceFile1Prefix = (String)genericlist[2];  // pm_SourceFile1Prefix.Text
            String SourceFile2Prefix = (String)genericlist[3];  // pm_SourceFile2Prefix.Text
            String[] TargetNameAry = (String[])genericlist[4];  // pm_ListBox_ListUp.Text
            String[] TrimHeightAry = (String[])genericlist[5];  // pm_TrimingHeight.Text

            int CutX = 0;

            String ErrorList = "";
            for (int ItemIdx = 0; ItemIdx < TargetNameAry.Length; ItemIdx++)
            {
                String TargetFileName = TargetNameAry[ItemIdx];
                if (TargetFileName == "")
                {
                    continue;   // 空行だったら処理しない
                }
                Debug.WriteData(Environment.NewLine + "Merge(1) " + (ItemIdx + 1).ToString() + " / " + TargetNameAry.Length.ToString());

                ////////////////////////////////////////////////////////////////////////
                // 対象チェック

                // 文字列が部分一致したら処理
                String Prefix1 = SourceFile1Prefix + Path.GetExtension(TargetFileName);
                String Prefix2 = SourceFile2Prefix + Path.GetExtension(TargetFileName);
                if (TargetFileName.IndexOf(Prefix1) == -1)
                {
                    // 対象外のファイル
                    continue;
                }

                ////////////////////////////////////////////////////////////////////////
                // オリジナルファイル名生成＆バックアップ
                String SourceFileFullName = SourceFolderPath + @"\" + TargetFileName;
                String SourceBackUpFullName = BackUpDirPath + @"\" + TargetFileName;
                Debug.WriteData("SourceFileName=" + SourceFileFullName);
                Debug.WriteData("SourceBackUpFileName=" + SourceBackUpFullName);

                if (!File.Exists(SourceFileFullName))
                {
                    ErrorList += "ファイルが存在しません。" + SourceFileFullName + Environment.NewLine;
                    continue;
                }
                File.Copy(SourceFileFullName, SourceBackUpFullName, true);

                ////////////////////////////////////////////////////////////////////////
                // マージファイル名生成＆バックアップ
                String MergeFileName = TargetFileName.Replace(Prefix1, Prefix2);
                String MergeFileFullName = SourceFolderPath + @"\" + MergeFileName;
                String MergeBackUpFullName = BackUpDirPath + @"\" + MergeFileName;
                Debug.WriteData("MergeFileFullName=" + MergeFileFullName);
                Debug.WriteData("MergeBackUpFullName=" + MergeBackUpFullName);

                if (!File.Exists(MergeFileFullName))
                {
                    ErrorList += "ファイルが存在しません。" + MergeFileFullName + Environment.NewLine;
                    continue;
                }

                // キャンバス作成
                PicEdit mrg = new PicEdit(SourceBackUpFullName);
                
                // 新マージアルゴリズム
                {
                    mrg.CreateSourceImg(MergeFileFullName);
                    Size sz = mrg.GetCanvasSize();

                    for (int j = 0; j < TrimHeightAry.Length; j++)
                    {
                        if (TrimHeightAry[j] == String.Empty)
                        {
                            continue;
                        }

                        string[] TrimHeight = TrimHeightAry[j].Split(new[] { "," }, StringSplitOptions.None);
                        if (TrimHeight.Length != 2)
                        {
                            // 想定外の値
                            DialogResult dr = MessageBox.Show("フォーマットが不正です。[" + TrimHeightAry[j] + "]" +
                                Environment.NewLine + "処理を中断しますか？",
                                "Error",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Error);
                            if (dr == DialogResult.Yes)
                            {
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        Debug.WriteData(Environment.NewLine + "Merge(1) " + (ItemIdx + 1).ToString() + " / " + TargetNameAry.Length.ToString() + 
                                        "   Merge(2) " + (j + 1).ToString() + " / " + TrimHeightAry.Length.ToString());

                        if (TrimHeight[0] == "-")
                        {
                            TrimHeight[0] = "0";
                        }
                        int StartHeight = int.Parse(TrimHeight[0]);

                        if (TrimHeight[1] == "-")
                        {
                            TrimHeight[1] = sz.Height.ToString();
                        }
                        int EndHeight = int.Parse(TrimHeight[1]);

                        if (sz.Height < StartHeight)
                        {
                            // 画像サイズよりも指定されたサイズが大きい
                            break;
                        }
                        System.Drawing.Rectangle CutParam = new System.Drawing.Rectangle(CutX, StartHeight, sz.Width, EndHeight - StartHeight);

                        Debug.WriteData("StartHeight=" + StartHeight);
                        Debug.WriteData("EndHeight=" + EndHeight);
                        Debug.WriteData("Picture Height=" + sz.Height);
                        Debug.WriteData("Rectangle Pos(" + CutParam.X.ToString() + "," + CutParam.Y.ToString() + ")");
                        Debug.WriteData("Rectangle Size(" + CutParam.Width.ToString() + "," + CutParam.Height.ToString() + ")");
                        mrg.MergeExec(CutParam);
                    }

                    Debug.WriteData("ReleaseSourceImg()");
                    mrg.ReleaseSourceImg();

                    // キャンバス保存
                    Debug.WriteData("SaveCanvas=" + SourceFileFullName);
                    mrg.SaveCanvas(SourceFileFullName);

                    // マージ元ファイルをバックアップへ移動
                    File.Move(MergeFileFullName, MergeBackUpFullName);

                    // 進捗率
                    worker.ReportProgress(ItemIdx);      // ⇒ProgressChanged()

                    // キャンセルされてないかチェック
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        mrg.Dispose();
                        return;
                    }
                }
                mrg.Dispose();

            }
            e.Result = ErrorList;
            worker.ReportProgress(TargetNameAry.Length);      // ⇒ProgressChanged()
        }

        private void bkgWorkerMerge_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            TextBox_Status.Text = e.ProgressPercentage + "/" + ProgressBar_Status.Maximum;
            ProgressBar_Status.Value = e.ProgressPercentage;

            // 一回目の更新時に、予想終了時間を表示
            if (e.ProgressPercentage == 0)
            {
                SetExpectEndTime(ProgressBar_Status.Maximum);
            }
        }

        private void bkgWorkerMerge_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("キャンセルされました");
                // この場合はe.Resultにはアクセスできない
            }
            else if (!(e.Error == null))
            {
                MessageBox.Show("エラーが発生しました[" + e.Error.Message + "]");
            }
            else
            {
                String Result = e.Result.ToString();
                if (Result != String.Empty)
                {
                    MessageBox.Show("処理中にエラーが発生しました。" + Environment.NewLine + Result,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    // 処理結果の表示
                    //MessageBox.Show("正常に完了しました");
                }
            }
            TextBox_Status.Text += " 完了";
            pm_MergeExec_Click.Text = "結合";

            // リスト更新
            ListupPictMerge();
        }
    }
}
