using System;
using System.Windows.Forms;

namespace EventRecorder
{
    // 記録/再生のホットキーをユーザーが変更するための設定画面。
    // ウィンドウ左上のシステムメニュー(Form1.OnHandleCreated参照)から開く。
    // 「保存」ボタンを押すまでは呼び出し元(Form1)には一切反映されない
    // (RecordHotkey/PlayHotkeyは、DialogResult.OKで閉じた時だけ呼び出し元が読み取って使う)
    public partial class HotkeySettingsForm : DialogFormBase
    {
        public Keys RecordHotkey { get; private set; }
        public Keys PlayHotkey { get; private set; }

        public HotkeySettingsForm(Keys currentRecordHotkey, Keys currentPlayHotkey)
        {
            InitializeComponent();

            RecordHotkey = currentRecordHotkey;
            PlayHotkey = currentPlayHotkey;
            textBox_Record.Text = HotkeyFormatter.Format(RecordHotkey);
            textBox_Play.Text = HotkeyFormatter.Format(PlayHotkey);
        }

        private void textBox_Record_KeyDown(object sender, KeyEventArgs e)
        {
            Keys captured;
            if (!TryCaptureHotkey(e, out captured))
            {
                return;
            }

            RecordHotkey = captured;
            textBox_Record.Text = HotkeyFormatter.Format(RecordHotkey);
        }

        private void textBox_Play_KeyDown(object sender, KeyEventArgs e)
        {
            Keys captured;
            if (!TryCaptureHotkey(e, out captured))
            {
                return;
            }

            PlayHotkey = captured;
            textBox_Play.Text = HotkeyFormatter.Format(PlayHotkey);
        }

        // テキストボックスへの通常の文字入力・キャレット移動は起こさせず、押されたキーの
        // 組み合わせをそのままホットキー候補として取り出す。Shift/Ctrl/Alt/Winキー単独の
        // 押下はホットキーとして成立しないので無視し、実キーが押されるまで待つ
        private static Boolean TryCaptureHotkey(KeyEventArgs e, out Keys captured)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            captured = Keys.None;

            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu
                || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                return false;
            }

            // KeyDataは押されているCtrl/Alt/Shiftのフラグ込みでKeyCodeを表す
            // (Form1.ProcessCmdKeyのkeyDataと同じ形。Ctrl+Sなら"Keys.Control | Keys.S")
            captured = e.KeyData;
            return true;
        }

        private void button_Reset_Click(object sender, EventArgs e)
        {
            RecordHotkey = HotkeyDefaults.Record;
            PlayHotkey = HotkeyDefaults.Play;
            textBox_Record.Text = HotkeyFormatter.Format(RecordHotkey);
            textBox_Play.Text = HotkeyFormatter.Format(PlayHotkey);
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (RecordHotkey == PlayHotkey)
            {
                MessageBox.Show(
                    "レコードと再生に同じキーは割り当てられないよ。どちらかを変えてね",
                    "EventRecorder - キーバインド設定",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
