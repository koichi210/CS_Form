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
    public partial class Cheetos : Form
    {
        private void pr_Button_RotationPreview_Click(object sender, EventArgs e)
        {
            RotationPreview rp = new RotationPreview(pr_BaseX.Text, pr_BaseY.Text, pr_Angle.Text);

            DialogResult dr = rp.ShowDialog();
            if (dr == DialogResult.OK)
            {
                pr_BaseX.Text = rp.OriginX;
                pr_BaseY.Text = rp.OriginY;
                pr_Angle.Text = rp.Angle;
            }
        }

        private void pr_SourceFolderPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(pr_SourceFolderPath.Text, e);
        }

        private void pr_Button_Listup_Click(object sender, EventArgs e)
        {
            ListupRotation();
        }

        private void ListupRotation()
        {
            if (!Directory.Exists(pr_SourceFolderPath.Text))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }
            pr_ListBox_ListUp.Items.Clear();

            string[] files = Directory.GetFiles(pr_SourceFolderPath.Text, "*", SearchOption.TopDirectoryOnly);

            //配列の内容を一つ一つ追加する
            for (int i = 0; i <= files.Length - 1; i++)
            {
                var FileName = Path.GetFileName(files[i]);
                pr_ListBox_ListUp.Items.Add(FileName);
            }
        }

        private void pr_Button_Rotation_Click(object sender, EventArgs e)
        {
            if (pr_ListBox_ListUp.SelectedItems.Count == 0)
            {
                MessageBox.Show("ファイルが選択されていません。");
                return;
            }

            // キャンセル
            if (bkgWorkerRotation.IsBusy)
            {
                bkgWorkerRotation.CancelAsync();
                return;
            }

            String BackUpDirPath = pr_SourceFolderPath.Text + @"\" + @"Bk_Rotate";
            if (!fio.IsExistDirectory(BackUpDirPath))
            {
                MessageBox.Show("無効なフォルダパスです。\n" + BackUpDirPath);
                return;
            }

            int val;
            if (!Int32.TryParse(pr_BaseX.Text.ToString(), out val))
            {
                pr_BaseX.Text = "";
            }
            if (!Int32.TryParse(pr_BaseY.Text.ToString(), out val))
            {
                pr_BaseY.Text = "";
            }
            if (!Int32.TryParse(pr_Angle.Text.ToString(), out val))
            {
                pr_Angle.Text = "";
            }

            InitProgressBar(pr_ListBox_ListUp.SelectedItems.Count);

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(pr_BaseX.Text);
            arguments.Add(pr_BaseY.Text);
            arguments.Add(pr_Angle.Text);
            arguments.Add(pr_SourceFolderPath.Text);
            arguments.Add(BackUpDirPath);

            // ListBoxの値を配列で取得
            String[] StrArray = util.GetStrArrayFromListBox(pr_ListBox_ListUp.SelectedItems);
            arguments.Add(StrArray);

            SetStartTime();
            pr_Button_Rotation.Text = "中断";
            bkgWorkerRotation.RunWorkerAsync(arguments);   // ⇒DoWork()
        }

        private void bkgWorkerRotation_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;


            int BaseX = (int)int.Parse((String)genericlist[0]); // pt_BaseX
            int BaseY = (int)int.Parse((String)genericlist[1]); // pt_BaseY
            int Angle = (int)int.Parse((String)genericlist[2]); // pr_Angle
            String SourceFolderPath = (String)genericlist[3];   // pr_SourceFolderPath
            String BackUpDirPath = (String)genericlist[4];      // BackUpDirPath
            String[] TargetNameAry = (String[])genericlist[5];  // pm_ListBox_ListUp

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

                // TODOメソッド化
                {
                    Bitmap img = new Bitmap(FilePath);
                    
                    int length = (int)Math.Sqrt(img.Width*img.Width + img.Height*img.Height);

                    Bitmap canvas;
                    using (PictureBox pic_box = new PictureBox())
                    {
                        pic_box.Size = new Size(length, length);
                        canvas = new Bitmap(pic_box.Width, pic_box.Height);
                    }

                    //ラジアン単位に変換
                    double d = Angle / (180 / Math.PI);

                    //新しい座標位置を計算する
                    float x1 = BaseX + img.Width * (float)Math.Cos(d);
                    float y1 = BaseY + img.Width * (float)Math.Sin(d);
                    float x2 = BaseX - img.Height * (float)Math.Sin(d);
                    float y2 = BaseY + img.Height * (float)Math.Cos(d);

                    //PointF配列を作成
                    PointF[] destinationPoints =
                    {
                        new PointF(BaseX, BaseY),
                        new PointF(x1, y1),
                        new PointF(x2, y2)
                    };

                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        //画像を表示
                        g.DrawImage(img, destinationPoints);

                        g.Dispose();
                        img.Dispose();
                    }

                    canvas.Save(FilePath);
                    canvas.Dispose();
                }

                // 進捗率
                worker.ReportProgress(ItemIdx);      // ⇒ProgressChanged()

                // キャンセルされてないかチェック
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
            worker.ReportProgress(TargetNameAry.Length);      // ⇒ProgressChanged()
        }

        private void bkgWorkerRotation_ProgressChanged(object sender, ProgressChangedEventArgs e)
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

        private void bkgWorkerRotation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            pr_Button_Rotation.Text = "回転実行";

            // リストを更新
            ListupRotation();
        }
    }

    // TODOタブの中身を別クラスに集約したい
    class Rotation
    {
    }
}
