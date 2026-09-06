using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using Picture;

namespace Cheetos
{
    // DistOrient
    partial class Cheetos
    {
        private void do_Distribute_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(do_SourceFolderPath.Text))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }

            if (!fio.IsExistDirectory(do_DestPortFolderPath.Text))
            {
                MessageBox.Show("無効なフォルダパスです。\n" + do_DestPortFolderPath.Text);
                return;
            }
            if (!fio.IsExistDirectory(do_DestLandFolderPath.Text))
            {
                MessageBox.Show("無効なフォルダパスです。\n" + do_DestLandFolderPath.Text);
                return;
            }

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(int.Parse(do_WhiteLength.Text));
            arguments.Add(int.Parse(do_WhiteCoef.Text));
            arguments.Add(do_DestPortFolderPath.Text);
            arguments.Add(do_DestLandFolderPath.Text);

            String[] files = Directory.GetFiles(do_SourceFolderPath.Text, do_TargetFileName.Text, SearchOption.TopDirectoryOnly);
            arguments.Add(files);

            InitProgressBar(files.Length);

            SetStartTime();
            do_Distribute.Text = "中断";
            bkgWorkerOrient.RunWorkerAsync(arguments);   // ⇒DoWork()
        }

        private void do_GetSampleParam_Click(object sender, EventArgs e)
        {
            if (!File.Exists(do_SampleFilePath.Text))
            {
                MessageBox.Show("無効なファイルパスです。\n" + do_SampleFilePath.Text);
                return;
            }

            Logic.IsPortrait(do_SampleFilePath.Text, int.Parse(do_WhiteLength.Text), int.Parse(do_WhiteCoef.Text), true);
        }

        private void sample2(String filePath, int WhiteLength)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(filePath);
            if (image == null)
            {
                return;
            }

            int width = WhiteLength;
            int height = image.Height;
            int fileSize = (int)new System.IO.FileInfo(filePath).Length;

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(fileSize))
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show(stream.Length.ToString());
            }
            image.Dispose();
        }

        private void sample3(String filePath, int WhiteLength)
        {
            Image image = Image.FromFile(filePath);
            if (image == null)
            {
                return;
            }

            int width = WhiteLength;
            int height = image.Height;
            int fileSize = (int)new System.IO.FileInfo(filePath).Length;
            Rectangle PasteRect = new Rectangle(0, 0, width, height);

            using (MemoryStream stream = new MemoryStream(fileSize))
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show(stream.Length.ToString());
            }
            image.Dispose();
        }

        private void sample4(String filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            long filesize = fi.Length;
        }

        private void bkgWorkerOrient_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            String ErrorString = "";

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;
            int WhiteLength = (int)genericlist[0];              // do_WhiteLength
            int WhiteCoef = (int)genericlist[1];                // do_WhiteCoef
            String DestPortFolderPath = (String)genericlist[2]; // do_DestPortFolderPath
            String DestLandFolderPath = (String)genericlist[3]; // do_DestLandFolderPath
            String[] files = (String[])genericlist[4];          // FileList

            for (int i = 0; i <= files.Length - 1; i++)
            {
                String DestName;
                if (Logic.IsPortrait(files[i], WhiteLength, WhiteCoef))
                {
                    DestName = DestPortFolderPath;
                }
                else
                {
                    DestName = DestLandFolderPath;
                }
                DestName += @"\" + Path.GetFileName(files[i]);

                if (!fio.FileMove(files[i], DestName))
                {
                    ErrorString += "Move " + files[i] + " " + DestName + Environment.NewLine;
                }

                // 進捗率の表示
                worker.ReportProgress(i);      // ⇒ProgressChanged()

                // キャンセルされてないかチェック
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }

            worker.ReportProgress(files.Length);      // ⇒ProgressChanged()
            e.Result = ErrorString;
        }

        private void bkgWorkerOrient_ProgressChanged(object sender, ProgressChangedEventArgs e)
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

        private void bkgWorkerOrient_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                    MessageBox.Show("処理が失敗しました。" + Environment.NewLine + Result,
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    // 処理結果の表示
                    //MessageBox.Show("正常に完了しました");
                }
            }
            TextBox_Status.Text += " 完了";
            do_Distribute.Text = "振り分け";
        }
    }
}
