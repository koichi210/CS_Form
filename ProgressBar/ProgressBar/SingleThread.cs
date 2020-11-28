using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgressBar
{
    public partial class Form1
    {
        private Boolean m_ExeFlg1 = false;

        private void StartSingleThread()
        {
            m_ExeFlg1 = true;

            progressBar_SingleThread.Maximum = m_ProgressBarMax;
            progressBar_SingleThread.Minimum = m_ProgressBarMin;
            progressBar_SingleThread.Value = 0;

            for (int i = progressBar_SingleThread.Minimum; i < progressBar_SingleThread.Maximum; i++)
            {
                System.Threading.Thread.Sleep(50);
                if (m_ExeFlg1 == false)
                {
                    break;
                }

                progressBar_SingleThread.Value++;
            }
        }

        private void StopSingleThread()
        {
            // メイン処理と同一タスクでStop要求を送るので、設定が効かない（Cpuが空かない）
            m_ExeFlg1 = false;
        }
    }
}
