using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace BackgroundWorker
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

            // 別スレッドに渡すパラメータ
            List<object> arguments = new List<object> { 100 };

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
            System.ComponentModel.BackgroundWorker worker = (System.ComponentModel.BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> arguments = e.Argument as List<object>;

            int loopCount = (int)arguments[0]; // 100

            // 時間のかかる処理
            for (int i = 0; i < loopCount; i++)
            {
                System.Threading.Thread.Sleep(100);

                int percentage = 100 * i / loopCount;      // 進捗率
                worker.ReportProgress(percentage);      // ⇒ProgressChanged()

                // キャンセルされてないかチェック
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // このメソッドからの戻り値(好きな値を渡せる）
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
            if (e.Error != null)
            {
                // この場合もe.Resultにはアクセスできない(アクセスすると例外になる)
                MessageBox.Show("エラーが発生しました" + Environment.NewLine + e.Error.Message);
            }
            else if (e.Cancelled)
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
