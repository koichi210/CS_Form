using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressBar
{
    public partial class Form1
    {
        private Boolean m_ExeFlg2 = false;
        private void StartMultiThreadTask()
        {
            if (m_ExeFlg2)
            {
                // 実行中
                return;
            }
            m_ExeFlg2 = true;

            progressBar_MultiThreadTask.Maximum = m_ProgressBarMax;
            progressBar_MultiThreadTask.Minimum = m_ProgressBarMin;
            progressBar_MultiThreadTask.Value = 0;

            Task task = new Task(() =>
            {
                for (int i = progressBar_MultiThreadTask.Minimum; i < progressBar_MultiThreadTask.Maximum; i++)
                {
                    if (m_ExeFlg2 == false)
                    {
                        break;
                    }
                    Invoke(new Action(() =>
                    {
                        progressBar_MultiThreadTask.Value++;
                    }));
                    System.Threading.Thread.Sleep(50);
                }
            });
            task.Start();
        }

        private void StopMultiThreadTask()
        {
            // メイン処理は別タスクで実施しているので、Stop要求を受け付けられる
            m_ExeFlg2 = false;
        }
    }
}
