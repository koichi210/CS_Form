using System;

namespace ProgressBar
{
    public partial class Form1
    {
        private Boolean m_IsSingleThreadRunning = false;

        private void StartSingleThread()
        {
            m_IsSingleThreadRunning = true;

            progressBar_SingleThread.Maximum = ProgressBarMax;
            progressBar_SingleThread.Minimum = ProgressBarMin;
            progressBar_SingleThread.Value = 0;

            for (int i = progressBar_SingleThread.Minimum; i < progressBar_SingleThread.Maximum; i++)
            {
                System.Threading.Thread.Sleep(50);
                if (!m_IsSingleThreadRunning)
                {
                    break;
                }

                progressBar_SingleThread.Value++;
            }
        }

        private void StopSingleThread()
        {
            // メイン処理と同一タスクでStop要求を送るので、設定が効かない（Cpuが空かない）
            m_IsSingleThreadRunning = false;
        }
    }
}
