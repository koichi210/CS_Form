using System;
using System.Threading.Tasks;

namespace ProgressBar
{
    public partial class Form1
    {
        private Boolean m_IsMultiThreadTaskRunning = false;

        private void StartMultiThreadTask()
        {
            if (m_IsMultiThreadTaskRunning)
            {
                // 実行中
                return;
            }
            m_IsMultiThreadTaskRunning = true;

            progressBar_MultiThreadTask.Maximum = ProgressBarMax;
            progressBar_MultiThreadTask.Minimum = ProgressBarMin;
            progressBar_MultiThreadTask.Value = 0;

            Task task = new Task(() =>
            {
                try
                {
                    for (int i = progressBar_MultiThreadTask.Minimum; i < progressBar_MultiThreadTask.Maximum; i++)
                    {
                        if (!m_IsMultiThreadTaskRunning)
                        {
                            break;
                        }
                        Invoke(new Action(() =>
                        {
                            progressBar_MultiThreadTask.Value++;
                        }));
                        System.Threading.Thread.Sleep(50);
                    }
                }
                finally
                {
                    // 最後まで進んだ場合や途中で失敗した場合も、次のStartを受け付けられるように戻す
                    m_IsMultiThreadTaskRunning = false;
                }
            });
            task.Start();
        }

        private void StopMultiThreadTask()
        {
            // メイン処理は別タスクで実施しているので、Stop要求を受け付けられる
            m_IsMultiThreadTaskRunning = false;
        }
    }
}
