using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ProgressBar
{
    public partial class Form1
    {
        private void StartMultiThreadBkgWork()
        {
            if (backgroundWorker_BkgWorker.IsBusy)
            {
                // 実行中
                return;
            }

            progressBar_MultiThreadBkgWork.Maximum = m_ProgressBarMax;
            progressBar_MultiThreadBkgWork.Minimum = m_ProgressBarMin;
            progressBar_MultiThreadBkgWork.Value = 0;

            backgroundWorker_BkgWorker.RunWorkerAsync();   // ⇒DoWork()
        }

        private void StopMultiThreadBkgWork()
        {
            // キャンセル
            if (backgroundWorker_BkgWorker.IsBusy)
            {
                backgroundWorker_BkgWorker.CancelAsync();
                return;
            }
        }

        private void backgroundWorker_BkgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            for (int i = progressBar_MultiThreadBkgWork.Minimum; i < progressBar_MultiThreadBkgWork.Maximum; i++)
            {
                // キャンセルされてないかチェック
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                worker.ReportProgress(i + 1);      // ⇒ProgressChanged()
                System.Threading.Thread.Sleep(50);
            }
        }

        private void backgroundWorker_BkgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            progressBar_MultiThreadBkgWork.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_BkgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
