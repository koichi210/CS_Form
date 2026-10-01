using System;
using System.Windows.Forms;

namespace ProgressBar
{
    public partial class Form1 : Form
    {
        private const int ProgressBarMax = 100;
        private const int ProgressBarMin = 0;

        public Form1()
        {
            InitializeComponent();
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
