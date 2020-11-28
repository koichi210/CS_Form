using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ProgressBar
{
    public partial class Form1
    {
        private void StartMultiThreadTaskInBkgWork()
        {
            if (backgroundWorker_TaskInBkgWorker.IsBusy)
            {
                // 実行中
                return;
            }

            progressBar_MultiThreadTaskInBkgWork.Maximum = m_ProgressBarMax;
            progressBar_MultiThreadTaskInBkgWork.Minimum = m_ProgressBarMin;
            progressBar_MultiThreadTaskInBkgWork.Value = 0;

            backgroundWorker_TaskInBkgWorker.RunWorkerAsync();   // ⇒DoWork()
        }

        private void StopMultiThreadTaskInBkgWork()
        {
            // キャンセル
            if (backgroundWorker_TaskInBkgWorker.IsBusy)
            {
                backgroundWorker_TaskInBkgWorker.CancelAsync();
                return;
            }
        }


        private void backgroundWorker_TaskInBkgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            Task task = new Task(() =>
            {
                for (int i = progressBar_MultiThreadTaskInBkgWork.Minimum; i < progressBar_MultiThreadTaskInBkgWork.Maximum; i++)
                {
                    Invoke(new Action(() =>
                    {
                        worker.ReportProgress(i + 1);      // ⇒ProgressChanged()
                    }));
                    System.Threading.Thread.Sleep(50);
                }
            });
            task.Start();
        }

        private void backgroundWorker_TaskInBkgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            progressBar_MultiThreadTaskInBkgWork.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_TaskInBkgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // MessageBox.Show("キャンセルされました");
                // この場合はe.Resultにはアクセスできない
            }
            else if (!(e.Error == null))
            {
                // MessageBox.Show("エラーが発生しました[" + e.Error.Message + "]");
            }
            else
            {
                // 処理結果の表示
                //MessageBox.Show("正常に完了しました");
            }
        }
    }
}
