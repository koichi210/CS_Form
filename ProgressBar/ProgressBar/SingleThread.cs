using System;

namespace ProgressBar
{
    public partial class Form1
    {
        private bool _isSingleThreadRunning = false;

        private void StartSingleThread()
        {
            _isSingleThreadRunning = true;

            ResetProgressBar(progressBar_SingleThread);

            for (int i = progressBar_SingleThread.Minimum; i < progressBar_SingleThread.Maximum; i++)
            {
                System.Threading.Thread.Sleep(50);
                if (!_isSingleThreadRunning)
                {
                    break;
                }

                progressBar_SingleThread.Value++;
            }
        }

        private void StopSingleThread()
        {
            // メイン処理と同一タスクでStop要求を送るので、設定が効かない（Cpuが空かない）
            _isSingleThreadRunning = false;
        }
    }
}
