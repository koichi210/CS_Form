using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BackGroundWorker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            // [排他制御1]実行中かどうかをIsBusyで制御する場合
            if (bgWorker.IsBusy)
            {
                MessageBox.Show("処理中です");
                return;
            }

            // [排他制御2]実行中にボタンが押せないように制御する場合
            buttonStart.Enabled = false;
            buttonCancel.Enabled = true;

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(100);

            // 別スレッドを非同期実行
            bgWorker.RunWorkerAsync(arguments);   // ⇒DoWork()
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            bgWorker.CancelAsync();
        }

        private void bgWorker_DoWork_1(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;

            int Param1 = (int)genericlist[0]; // 100

            // 時間のかかる処理
            for (int i = 0; i < Param1; i++)
            {
                System.Threading.Thread.Sleep(100);

                int percentage = 100 * i / Param1;      // 進捗率
                worker.ReportProgress(percentage);      // ⇒ProgressChanged()

                // キャンセルされてないかチェック
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // このメソッドからの戻り値
            e.Result = "すべて完了";

            // ⇒RunWorkerCompleted()
        }

        private void bgWorker_ProgressChanged_1(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            this.Text = e.ProgressPercentage + "％完了";
            progressBar.Value = e.ProgressPercentage;
        }

        private void bgWorker_RunWorkerCompleted_1(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("キャンセルされました");
                // この場合にはe.Resultにはアクセスできない
            }
            else
            {
                // 処理結果の表示
                this.Text = e.Result.ToString();
                MessageBox.Show("正常に完了");
            }

            buttonStart.Enabled = true;
            buttonCancel.Enabled = false;
        }
    }
}
