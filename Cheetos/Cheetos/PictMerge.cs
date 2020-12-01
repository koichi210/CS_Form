using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using Picture;

namespace Cheetos
{
    public class PictMerge
    {
        public String BackUpDirPath = String.Empty;
        public String SourceFolderPath = String.Empty;
        public String SourceFile1Prefix = String.Empty;
        public String SourceFile2Prefix = String.Empty;
        public String[] TrimHeightAry = null;

        private String TargetFileName = String.Empty;
        private String Prefix1 = String.Empty;
        private String Prefix2 = String.Empty;
        private String SourceFileFullName = String.Empty;
        private String SourceBackUpFullName = String.Empty;
        private String MergeFileFullName = String.Empty;
        private String MergeBackUpFullName = String.Empty;
        private String ErrorList = String.Empty;

        public bool SetTargetFileName(String target_file_name)
        {
            if (target_file_name == String.Empty)
            {
                // 空行だったら処理しない
                return false;
            }
            TargetFileName = target_file_name;
            return true;
        }

        public bool IsProcTarget()
        {
            // 文字列が部分一致したら処理
            Prefix1 = SourceFile1Prefix + Path.GetExtension(TargetFileName);
            Prefix2 = SourceFile2Prefix + Path.GetExtension(TargetFileName);
            if (TargetFileName.IndexOf(Prefix1) == -1)
            {
                // 対象外のファイル
                return false;
            }
            return true;
        }
        
        public bool CreateMargeSourceFile()
        {
            SourceFileFullName = SourceFolderPath + @"\" + TargetFileName;
            SourceBackUpFullName = BackUpDirPath + @"\" + TargetFileName;

            if (!File.Exists(SourceFileFullName))
            {
                ErrorList += "ファイルが存在しません。" + SourceFileFullName + Environment.NewLine;
                return false;
            }
            File.Copy(SourceFileFullName, SourceBackUpFullName, true);
            return true;
        }

        public bool CreateMargeTargetFile()
        {
            String MergeFileName = TargetFileName.Replace(Prefix1, Prefix2);
            MergeFileFullName = SourceFolderPath + @"\" + MergeFileName;
            MergeBackUpFullName = BackUpDirPath + @"\" + MergeFileName;

            if (!File.Exists(MergeFileFullName))
            {
                ErrorList += "ファイルが存在しません。" + MergeFileFullName + Environment.NewLine;
                return false;
            }
            return true;
        }

        public int GetHeight(String StartHeight, int DefaultHeight = 0)
        {
            if (StartHeight == "-")
            {
                return DefaultHeight;
            }
            return int.Parse(StartHeight);
        }

        public bool MergeExecute()
        {
            // キャンバス作成
            PicEdit mrg = new PicEdit(SourceBackUpFullName);

            mrg.CreateSourceImg(MergeFileFullName);
            Size sz = mrg.GetCanvasSize();

            for (int i = 0; i < TrimHeightAry.Length; i++)
            {
                if (TrimHeightAry[i] == String.Empty)
                {
                    continue;
                }

                string[] TrimHeight = TrimHeightAry[i].Split(new[] { "," }, StringSplitOptions.None);
                if (TrimHeight.Length != 2)
                {
                    // 想定外の値
                    DialogResult dr = MessageBox.Show("フォーマットが不正です。[" + TrimHeightAry[i] + "]" +
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

                int StartHeight = GetHeight(TrimHeight[0], 0);
                int EndHeight = GetHeight(TrimHeight[1], sz.Height);
                if (sz.Height < StartHeight)
                {
                    // 画像サイズよりも指定されたサイズが大きい
                    break;
                }
                Rectangle CutParam = new Rectangle(0, StartHeight, sz.Width, EndHeight - StartHeight);
                mrg.MergeExec(CutParam);
            }

            mrg.ReleaseSourceImg();

            // キャンバス保存
            mrg.SaveCanvas(SourceFileFullName);

            // マージ元ファイルをバックアップへ移動
            File.Move(MergeFileFullName, MergeBackUpFullName);

            mrg.Dispose();
            return true;
        }

        public String GetErrorMessage()
        {
            return ErrorList;
        }
    }

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

            // 切断基準となる高さ
            String[] TrimHeightAry = pm_TrimingHeight.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            arguments.Add(TrimHeightAry);

            // ListBoxの値を配列で取得
            String[] StrArray = util.GetStrArrayFromListBox(pm_ListBox_ListUp.SelectedItems);
            arguments.Add(StrArray);

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

        private void bkgWorkerMerge_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;

            PictMerge pm = new PictMerge();
            pm.BackUpDirPath = (String)genericlist[0];      // BackUpDirPath
            pm.SourceFolderPath = (String)genericlist[1];   // pm_SourceFolderPath.Text
            pm.SourceFile1Prefix = (String)genericlist[2];  // pm_SourceFile1Prefix.Text
            pm.SourceFile2Prefix = (String)genericlist[3];  // pm_SourceFile2Prefix.Text
            pm.TrimHeightAry = (String[])genericlist[4];  // pm_TrimingHeight.Text
            String[] TargetNameAry = (String[])genericlist[5];  // pm_ListBox_ListUp.Text

            for (int ItemIdx = 0; ItemIdx < TargetNameAry.Length; ItemIdx++)
            {
                if (!pm.SetTargetFileName(TargetNameAry[ItemIdx]))
                {
                    continue;
                }

                if (!pm.IsProcTarget())
                {
                    continue;
                }

                if (!pm.CreateMargeSourceFile())
                {
                    continue;
                }

                if (!pm.CreateMargeTargetFile())
                {
                    continue;
                }

                pm.MergeExecute();
                worker.ReportProgress(ItemIdx);      // ⇒ProgressChanged()
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            e.Result = pm.GetErrorMessage();
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
