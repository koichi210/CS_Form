using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EventRecorder
{
    // 右クリックメニュー「WAIT時間を一括変更」から開く、対象イベント名と待機時間(ms)を入力するダイアログ。
    // 対象イベントは「直前がWAIT_MS行であるイベント」で、そのWAIT_MS行の待機時間をまとめて変更する
    internal class EventWaitBulkChangeForm : DialogFormBase
    {
        private readonly TextBox txtEventName;
        private readonly TextBox txtWaitMs;
        private readonly Button btnOk;

        public String EventName { get; private set; }
        public int WaitMs { get; private set; }

        // 変更対象に指定できるイベント名(WAIT_MS自身とUNKNOWNは対象外)。大文字小文字は区別せず、正規の名前へ揃える
        internal static Boolean TryNormalizeEventName(String text, out String eventName)
        {
            eventName = null;
            String trimmed = (text ?? String.Empty).Trim();
            if (trimmed.Length == 0)
            {
                return false;
            }

            foreach (String name in Enum.GetNames(typeof(GlobalHook.MouseHook.Stroke))
                .Concat(Enum.GetNames(typeof(GlobalHook.KeyboardHook.Stroke))))
            {
                if (name == "UNKNOWN")
                {
                    continue;
                }

                if (String.Equals(name, trimmed, StringComparison.OrdinalIgnoreCase))
                {
                    eventName = name;
                    return true;
                }
            }

            return false;
        }

        public EventWaitBulkChangeForm(String initialEventName)
        {
            this.Text = "WAIT時間を一括変更";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(380, 150);

            Label label = new Label
            {
                Text = "対象イベント(このイベント直前のWAIT_MS行が対象)",
                Location = new Point(10, 12),
                AutoSize = true,
            };

            txtEventName = new TextBox
            {
                Location = new Point(10, 34),
                Width = 200,
                Text = initialEventName,
            };

            Label waitLabel = new Label
            {
                Text = "変更後のWAIT時間(ms)",
                Location = new Point(10, 68),
                AutoSize = true,
            };

            txtWaitMs = new TextBox
            {
                Location = new Point(10, 90),
                Width = 100,
                Text = "100",
            };

            btnOk = new Button
            {
                Text = "OK",
                Location = new Point(285, 110),
                Width = 85,
            };
            btnOk.Click += BtnOk_Click;

            this.Controls.Add(label);
            this.Controls.Add(txtEventName);
            this.Controls.Add(waitLabel);
            this.Controls.Add(txtWaitMs);
            this.Controls.Add(btnOk);

            this.AcceptButton = btnOk;

            InitializeToolTips();
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(txtEventName, "Event列の名前(例: LEFT_DOWN、KEY_DOWN)。大文字/小文字は区別しない。直前がWAIT_MS行でない行は変更されない");
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            String eventName;
            if (!TryNormalizeEventName(txtEventName.Text, out eventName))
            {
                MessageBox.Show(
                    "イベント名が正しくないよ(例: LEFT_UP, RIGHT_DOWN, KEY_DOWN, KEY_UP)",
                    "WAIT時間を一括変更",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtEventName.Focus();
                return;
            }

            int wait;
            if (!int.TryParse(txtWaitMs.Text, out wait) || wait < 0)
            {
                MessageBox.Show(
                    "0以上の整数を入力してね",
                    "WAIT時間を一括変更",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            EventName = eventName;
            WaitMs = wait;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
