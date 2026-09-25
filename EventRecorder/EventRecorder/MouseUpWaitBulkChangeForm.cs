using System;
using System.Drawing;
using System.Windows.Forms;

namespace EventRecorder
{
    // 右クリックメニュー「MOUSE_UP時間を一括変更」から開く、待機時間(ms)を1つ入力するだけの小さなダイアログ
    internal class MouseUpWaitBulkChangeForm : DialogFormBase
    {
        private readonly TextBox txtWaitMs;
        private readonly Button btnOk;

        public int WaitMs { get; private set; }

        public MouseUpWaitBulkChangeForm()
        {
            this.Text = "MOUSE_UP時間を一括変更";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(380, 110);

            Label label = new Label
            {
                Text = "LEFT_UP、RIGHT_UP直前のWAIT時間を一括変更します(ms)",
                Location = new Point(10, 12),
                AutoSize = true,
            };

            txtWaitMs = new TextBox
            {
                Location = new Point(10, 38),
                Width = 100,
                Text = "100",
            };

            btnOk = new Button
            {
                Text = "OK",
                Location = new Point(285, 70),
                Width = 85,
            };
            btnOk.Click += BtnOk_Click;

            this.Controls.Add(label);
            this.Controls.Add(txtWaitMs);
            this.Controls.Add(btnOk);

            this.AcceptButton = btnOk;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            int wait;
            if (!int.TryParse(txtWaitMs.Text, out wait) || wait < 0)
            {
                MessageBox.Show(
                    "0以上の整数を入力してね",
                    "MOUSE_UP時間を一括変更",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            WaitMs = wait;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
