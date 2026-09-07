using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StandardTemplate;

namespace EventRecorder
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();
        private StcFileInputOutput fio = new StcFileInputOutput();
        private SaveRestore sr = new SaveRestore();

        // 記録中/再生中フラグ
        private Boolean isRecording = false;
        private Boolean isPlaying = false;
        private Boolean stopPlayRequested = false;

        // 直前のイベント時刻(記録の待機ms算出用)
        private int lastEventTick = 0;

        // 現在扱っているマクロファイル
        private String currentFileName = "";

        public Form1()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.EventRecorder;
            util.SetCurrentDirectory();

            sr.RegistItem(this);
        }

        // *******************************************************************************
        // 記録

        private void button_Record_Click(object sender, EventArgs e)
        {
            if (isRecording)
            {
                GlobalHook.MouseHook.Stop();
                GlobalHook.KeyboardHook.Stop();
                isRecording = false;
                button_Record.Text = "記録";
                return;
            }

            if (isPlaying)
            {
                return;
            }

            dataGridView1.Rows.Clear();
            lastEventTick = Environment.TickCount;

            GlobalHook.MouseHook.AddEvent(OnMouseEvent);
            GlobalHook.MouseHook.Start();
            GlobalHook.KeyboardHook.AddEvent(OnKeyboardEvent);
            GlobalHook.KeyboardHook.Start();

            isRecording = true;
            button_Record.Text = "記録中…";
        }

        private void OnMouseEvent(ref GlobalHook.MouseHook.StateMouse s)
        {
            // カーソル移動そのものは記録しない(HiMacroExと同様、クリック単位のみ記録)
            if (s.Stroke == GlobalHook.MouseHook.Stroke.MOVE || s.Stroke == GlobalHook.MouseHook.Stroke.UNKNOWN)
            {
                return;
            }

            AddRow(s.Stroke.ToString(), s.X.ToString(), s.Y.ToString(), "", ElapsedMs());
        }

        private void OnKeyboardEvent(ref GlobalHook.KeyboardHook.StateKeyboard s)
        {
            if (s.Stroke == GlobalHook.KeyboardHook.Stroke.UNKNOWN)
            {
                return;
            }

            AddRow(s.Stroke.ToString(), "", "", s.Key.ToString(), ElapsedMs());
        }

        // 直前のイベントからの経過ms。記録開始直後の1件目は「記録開始からの待機」になる
        private int ElapsedMs()
        {
            int now = Environment.TickCount;
            int wait = now - lastEventTick;
            lastEventTick = now;
            return wait < 0 ? 0 : wait;
        }

        private void AddRow(String type, String x, String y, String key, int wait)
        {
            int idx = dataGridView1.Rows.Add();
            DataGridViewRow row = dataGridView1.Rows[idx];
            row.Cells[0].Value = type;
            row.Cells[1].Value = x;
            row.Cells[2].Value = y;
            row.Cells[3].Value = key;
            row.Cells[4].Value = wait.ToString();

            dataGridView1.FirstDisplayedScrollingRowIndex = idx;
        }

        // *******************************************************************************
        // 再生

        private void button_Play_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                stopPlayRequested = true;
                return;
            }

            if (isRecording)
            {
                return;
            }

            int loopCount = util.GetInteger(textBox_Loop.Text);
            if (loopCount <= 0)
            {
                loopCount = 1;
            }

            List<String[]> rows = SnapshotRows();
            if (rows.Count == 0)
            {
                return;
            }

            isPlaying = true;
            stopPlayRequested = false;
            button_Play.Text = "停止";

            Task.Run(() => PlayLoop(rows, loopCount));
        }

        private List<String[]> SnapshotRows()
        {
            List<String[]> list = new List<String[]>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                list.Add(new String[] {
                    Convert.ToString(row.Cells[0].Value),
                    Convert.ToString(row.Cells[1].Value),
                    Convert.ToString(row.Cells[2].Value),
                    Convert.ToString(row.Cells[3].Value),
                    Convert.ToString(row.Cells[4].Value)
                });
            }
            return list;
        }

        private void PlayLoop(List<String[]> rows, int loopCount)
        {
            for (int i = 0; i < loopCount && !stopPlayRequested; i++)
            {
                foreach (String[] r in rows)
                {
                    if (stopPlayRequested)
                    {
                        break;
                    }

                    int wait = util.GetInteger(r[4]);
                    if (wait > 0)
                    {
                        Thread.Sleep(wait);
                    }

                    PlayOneEvent(r[0], r[1], r[2], r[3]);
                }
            }

            isPlaying = false;
            stopPlayRequested = false;
            this.Invoke((MethodInvoker)(() => button_Play.Text = "再生"));
        }

        // 記録した1イベント分をSendInputで再現する
        // ※ マウスのX(サイド)ボタン/ホイールとキーボードのSYSKEYは、
        //    HiMacroEx相当を作る段階では未対応(将来の機能拡張項目)
        private void PlayOneEvent(String type, String x, String y, String key)
        {
            List<InputSimulation.InputSimulator.Input> inputs = new List<InputSimulation.InputSimulator.Input>();

            InputSimulation.InputSimulator.MouseStroke mouseStroke;
            if (Enum.TryParse<InputSimulation.InputSimulator.MouseStroke>(type, out mouseStroke))
            {
                List<InputSimulation.InputSimulator.MouseStroke> flags = new List<InputSimulation.InputSimulator.MouseStroke>();
                flags.Add(InputSimulation.InputSimulator.MouseStroke.MOVE);
                flags.Add(mouseStroke);

                int ix = util.GetInteger(x);
                int iy = util.GetInteger(y);
                InputSimulation.InputSimulator.AddMouseInput(ref inputs, flags, 0, true, ix, iy);
                InputSimulation.InputSimulator.SendInput(inputs);
                return;
            }

            InputSimulation.InputSimulator.KeyboardStroke keyStroke;
            Keys keyCode;
            if (Enum.TryParse<InputSimulation.InputSimulator.KeyboardStroke>(type, out keyStroke)
                && Enum.TryParse<Keys>(key, out keyCode))
            {
                InputSimulation.InputSimulator.AddKeyboardInput(ref inputs, keyStroke, keyCode);
                InputSimulation.InputSimulator.SendInput(inputs);
            }
        }

        // *******************************************************************************
        // クリア/保存/読込

        private void button_Clear_Click(object sender, EventArgs e)
        {
            if (isRecording || isPlaying)
            {
                return;
            }

            dataGridView1.Rows.Clear();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(currentFileName);
            if (sr.SaveSetting(SaveFileName))
            {
                currentFileName = SaveFileName;
                MessageBox.Show("保存したよ♪" + Environment.NewLine + SaveFileName);
            }
        }

        private void button_Load_Click(object sender, EventArgs e)
        {
            String LoadFileName = fio.SelectLoadFileName(currentFileName);
            if (sr.LoadProc(LoadFileName, this))
            {
                currentFileName = LoadFileName;
            }
        }
    }
}
