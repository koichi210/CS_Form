﻿using System;
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
    // PictTrim
    public partial class Cheetos : Form
    {
        private void SourceFolderPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(pt_SourceFolderPath.Text, e);
        }

        private void Button_TrimListup_Click(object sender, EventArgs e)
        {
            ListupTrim();
        }

        private void pt_ListBox_ListUp_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void pt_ListBox_ListUp_SelectedIndexChanged(object sender, EventArgs e)
        {
            pt_TextBox_Status.Text = "ファイル数：" + pt_ListBox_ListUp.SelectedItems.Count.ToString();
        }

        private void UpdatePitTrimSize()
        {
            Point Target = new Point();
            if (pt_Radio_SelectPointOfEnd.Checked == true)
            {
                Target.X = int.Parse(pt_TargetX.Text) + int.Parse(pt_BaseX.Text);
                Target.Y = int.Parse(pt_TargetY.Text) + int.Parse(pt_BaseY.Text);
            }
            else // (pt_Radio_SelectPointOfEnd.Checked == true)
            {
                Target.X = int.Parse(pt_TargetX.Text) - int.Parse(pt_BaseX.Text);
                Target.Y = int.Parse(pt_TargetY.Text) - int.Parse(pt_BaseY.Text);
            }

            pt_TargetX.Text = Target.X.ToString();
            pt_TargetY.Text = Target.Y.ToString();
        }

        private void ListupTrim()
        {
            if (!Directory.Exists(pt_SourceFolderPath.Text))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }
            pt_ListBox_ListUp.Items.Clear();

            string[] files = Directory.GetFiles(pt_SourceFolderPath.Text, "*", SearchOption.TopDirectoryOnly);

            //配列の内容を一つ一つ追加する
            for (int i = 0; i <= files.Length - 1; i++)
            {
                var FileName = Path.GetFileName(files[i]);
                pt_ListBox_ListUp.Items.Add(FileName);
            }
        }

        private void Button_Trim_Click(object sender, EventArgs e)
        {
            Debug.WriteData("Button_Trim_Click" + Environment.NewLine, false);

            // キャンセル
            if (bkgWorkerTrim.IsBusy)
            {
                bkgWorkerTrim.CancelAsync();
                return;
            }

            String BackUpDirPath = pt_SourceFolderPath.Text + @"\" + @"Bk_Trim";
            if (!fio.IsExistDirectory(BackUpDirPath))
            {
                MessageBox.Show("無効なフォルダパスです。\n" + BackUpDirPath);
                return;
            }

            if (pt_TargetX.Text == String.Empty || pt_TargetY.Text == String.Empty)
            {
                MessageBox.Show("サイズが指定されていません");
                return;
            }

            int Target_Width;
            int Target_Height;
            if (pt_Radio_SelectPointOfEnd.Checked == true)
            {
                Target_Width = int.Parse(pt_TargetX.Text) - int.Parse(pt_BaseX.Text);
                Target_Height = int.Parse(pt_TargetY.Text) - int.Parse(pt_BaseY.Text);
            }
            else // if ( pt_Radio_SelectSizeOfEnd.IsChecked.Value )
            {
                Target_Width = int.Parse(pt_TargetX.Text);
                Target_Height = int.Parse(pt_TargetY.Text);
            }

            InitProgressBar(pt_ListBox_ListUp.SelectedItems.Count);

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(pt_BaseX.Text);
            arguments.Add(pt_BaseY.Text);
            arguments.Add(Target_Width);
            arguments.Add(Target_Height);
            arguments.Add(pt_SourceFolderPath.Text);
            arguments.Add(BackUpDirPath);

            Debug.WriteData("Source = " + pt_SourceFolderPath.Text);
            Debug.WriteData("Backup = " + BackUpDirPath);
            Debug.WriteData("Pos(" + pt_BaseX.Text + "," + pt_BaseY.Text + ")");
            Debug.WriteData("Size(" + Target_Width + "," + Target_Height + ")");

            // ListBoxの値を配列で取得
            String[] StrArray = util.GetStrArrayFromListBox(pt_ListBox_ListUp.SelectedItems);
            arguments.Add(StrArray);

            SetStartTime();
            pt_Button_Trim.Text = "中断";
            bkgWorkerTrim.RunWorkerAsync(arguments);   // ⇒DoWork()
        }

        private void bkgWorkerTrim_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;


            int BaseX = (int)int.Parse((String)genericlist[0]); // pt_BaseX
            int BaseY = (int)int.Parse((String)genericlist[1]); // pt_BaseY
            int Target_Width = (int)genericlist[2];             // Target_Width
            int Target_Height = (int)genericlist[3];            // Target_Height
            String SourceFolderPath = (String)genericlist[4];   // pt_SourceFolderPath
            String BackUpDirPath = (String)genericlist[5];      // BackUpDirPath
            String[] TargetNameAry = (String[])genericlist[6];  // pm_ListBox_ListUp

            for (int ItemIdx = 0; ItemIdx < TargetNameAry.Length; ItemIdx++)
            {
                if (TargetNameAry[ItemIdx] == String.Empty)
                {
                    continue;
                }
                Debug.WriteData(Environment.NewLine + "Trim " + (ItemIdx + 1).ToString() + " / " + TargetNameAry.Length.ToString());

                String FilePath = SourceFolderPath + @"\" + TargetNameAry[ItemIdx];
                String BackUpFilePath = BackUpDirPath + @"\" + TargetNameAry[ItemIdx];
                Debug.WriteData("FilePath=" + FilePath);
                Debug.WriteData("BackUpFilePath=" + BackUpFilePath);

                // オリジナルファイルをバックアップ
                File.Copy(FilePath, BackUpFilePath, true);

                // トリミング
                // キャンバス作成
                Debug.WriteData("PicEdit(" + Target_Width.ToString() + "," + Target_Height.ToString() + ")");
                PicEdit trm = new PicEdit(Target_Width, Target_Height);

                // 切り取り
                Debug.WriteData("Rectangle Pos(" + BaseX.ToString() + "," + BaseY.ToString() + ")");
                Debug.WriteData("Rectangle Size(" + Target_Width.ToString() + "," + Target_Height.ToString() + ")");
                Rectangle CutParam = new Rectangle(BaseX, BaseY, Target_Width, Target_Height);

                Point PutParam = new Point(0, 0);
                trm.TrimExec(BackUpFilePath, CutParam, PutParam);

                // キャンバス保存
                Debug.WriteData("SaveCanvas FilePath=" + FilePath);
                trm.SaveCanvas(FilePath);

                // 進捗率
                worker.ReportProgress(ItemIdx);      // ⇒ProgressChanged()

                trm.Dispose();

                // キャンセルされてないかチェック
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
            worker.ReportProgress(TargetNameAry.Length);      // ⇒ProgressChanged()
        }

        private void bkgWorkerTrim_ProgressChanged(object sender, ProgressChangedEventArgs e)
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

        private void bkgWorkerTrim_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                // 処理結果の表示
                //MessageBox.Show("正常に完了しました");
            }
            TextBox_Status.Text += " 完了";
            pt_Button_Trim.Text = "切り取り";

            // リストを更新
            ListupTrim();
        }
    }
}
