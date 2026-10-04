using System;
using System.Threading.Tasks;

namespace ProgressBar
{
    public partial class Form1
    {
        private bool _isMultiThreadTaskRunning = false;

        private void StartMultiThreadTask()
        {
            if (_isMultiThreadTaskRunning)
            {
                // 実行中
                return;
            }
            _isMultiThreadTaskRunning = true;

            ResetProgressBar(progressBar_MultiThreadTask);

            Task task = new Task(() =>
            {
                try
                {
                    for (int i = progressBar_MultiThreadTask.Minimum; i < progressBar_MultiThreadTask.Maximum; i++)
                    {
                        if (!_isMultiThreadTaskRunning)
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
                    _isMultiThreadTaskRunning = false;
                }
            });
            task.Start();
        }

        private void StopMultiThreadTask()
        {
            // メイン処理は別タスクで実施しているので、Stop要求を受け付けられる
            _isMultiThreadTaskRunning = false;
        }
    }
}
