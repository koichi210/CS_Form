using System;
using System.Windows.Forms;

namespace ProgressBar
{
    public partial class Form1 : Form
    {
        private const int _progressBarMax = 100;
        private const int _progressBarMin = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(button_StartSingleThread, "画面と同じスレッドで処理するため、完了まで(約5秒)画面が操作できなくなる");
            toolTip.SetToolTip(button_StopSingleThread, "処理中は画面が止まっているため押しても効かず、途中で停止できない(比較用)");
        }

        // 各方式で共通の、プログレスバーの範囲設定と初期化
        private static void ResetProgressBar(System.Windows.Forms.ProgressBar progressBar)
        {
            progressBar.Maximum = _progressBarMax;
            progressBar.Minimum = _progressBarMin;
            progressBar.Value = 0;
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            StartSingleThread();
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            StopSingleThread();
        }

        private void button_StartMultiThreadTask_Click(object sender, EventArgs e)
        {
            StartMultiThreadTask();
        }

        private void button_StopMultiThreadTask_Click(object sender, EventArgs e)
        {
            StopMultiThreadTask();
        }

        private void button_StartMultiThreadBkgWork_Click(object sender, EventArgs e)
        {
            StartMultiThreadBkgWork();
        }

        private void button_StopMultiThreadBkgWork_Click(object sender, EventArgs e)
        {
            StopMultiThreadBkgWork();
        }

        private void button_StartTaskInBackgroundWorker_Click(object sender, EventArgs e)
        {
            StartMultiThreadTaskInBkgWork();
        }

        private void button_StopTaskInBackgroundWorker_Click(object sender, EventArgs e)
        {
            StopMultiThreadTaskInBkgWork();
        }
    }
}
