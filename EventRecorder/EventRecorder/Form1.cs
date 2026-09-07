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

        // 再生中にハイライトしている行のインデックス(-1ならハイライト無し)
        private int highlightedRowIndex = -1;

        // 再生開始時のマウスカーソル位置(再生終了後に戻すため)
        private Point cursorPositionBeforePlay;

        // 記録中に押下中のキー(OSのキーリピートによるKeyDown連発を抑制するため、
        // KeyUpが来るまで「押されている」とみなす)。記録セッションの開始のたびにクリアする
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();

        // ホットキー(F1=記録開始/停止、変換キー=再生開始/停止)の押下状態。
        // 記録セッションとは独立して管理する(pressedKeysをClearしても消えないように)
        private HashSet<Keys> pressedHotkeys = new HashSet<Keys>();

        // 記録開始/停止のホットキー。ボタンクリックだとクリック自体のマウスイベントが
        // 記録に混ざってしまうため、キー操作で完結できるようにしている
        private const Keys HotkeyToggleRecord = Keys.F1;

        // 再生開始/停止のホットキー
        private const Keys HotkeyTogglePlay = Keys.IMEConvert;

        // 記録したがまだDataGridViewに反映していない行(フックのコールバックを
        // 描画待ちで塞がないよう、一旦ここに貯めてタイマーでまとめて反映する)
        private List<String[]> pendingRows = new List<String[]>();
        private System.Windows.Forms.Timer gridFlushTimer;

        // マウスカーソル座標の常時表示用タイマー
        private System.Windows.Forms.Timer mousePosTimer;

        // 起動時に自動読込するデフォルトの設定ファイル名(Cheetosに倣う)
        private readonly String SettingFileName = @"EventRecorder.xml";

        public Form1()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.EventRecorder;
            util.SetCurrentDirectory();

            // DataGridViewの標準実装は行追加のたびにチラつき/再描画コストが出やすいため、
            // ダブルバッファを有効化する(DoubleBufferedはprotectedなのでリフレクション経由)
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null,
                dataGridView_Events,
                new object[] { true });

            gridFlushTimer = new System.Windows.Forms.Timer();
            gridFlushTimer.Interval = 150;
            gridFlushTimer.Tick += (s, e) => FlushPendingRows();

            // マウスカーソルの座標を起動中ずっと表示しておく(記録/再生の状態と関係なく常時更新)
            mousePosTimer = new System.Windows.Forms.Timer();
            mousePosTimer.Interval = 100;
            mousePosTimer.Tick += (s, e) =>
            {
                Point p = Cursor.Position;
                label_MousePos.Text = "Mouse: " + p.X + ", " + p.Y;
            };
            mousePosTimer.Start();

            // キーボードフックはホットキー(F1/変換キー)監視のためアプリ起動中ずっと張っておく。
            // マウスフックは記録中だけでよいのでToggleRecording側で開始/停止する
            GlobalHook.KeyboardHook.AddEvent(OnKeyboardEvent);
            GlobalHook.KeyboardHook.Start();

            this.FormClosing += Form1_FormClosing;

            sr.RegistItem(this);

            // 起動時にデフォルト設定を読み込み、コンボボックスに設定ファイル一覧を表示する(Cheetosと同じ手順)
            sr.LoadProc(SettingFileName, this);
            util.UpdateProfileList(ref comboBox_Profile);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            gridFlushTimer.Stop();
            mousePosTimer.Stop();
            GlobalHook.MouseHook.Stop();
            GlobalHook.KeyboardHook.Stop();
        }

        // *******************************************************************************
        // 記録

        private void button_Record_Click(object sender, EventArgs e)
        {
            ToggleRecording();
        }

        // 記録開始/停止を切り替える。ボタンクリックからもF1ホットキーからも呼ばれる
        private void ToggleRecording()
        {
            if (isRecording)
            {
                GlobalHook.MouseHook.Stop();
                gridFlushTimer.Stop();
                FlushPendingRows();
                isRecording = false;
                button_Record.Text = "記録";
                return;
            }

            if (isPlaying)
            {
                return;
            }

            // 記録開始時に既存の行はクリアしない(既存のマクロに追記で記録したい場合があるため)
            pendingRows.Clear();
            pressedKeys.Clear();
            lastEventTick = Environment.TickCount;

            GlobalHook.MouseHook.AddEvent(OnMouseEvent);
            GlobalHook.MouseHook.Start();

            gridFlushTimer.Start();
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

            QueueRow(s.Stroke.ToString(), s.X.ToString(), s.Y.ToString(), "", ElapsedMs());
        }

        private void OnKeyboardEvent(ref GlobalHook.KeyboardHook.StateKeyboard s)
        {
            if (s.Stroke == GlobalHook.KeyboardHook.Stroke.UNKNOWN)
            {
                return;
            }

            Boolean isDown = s.Stroke == GlobalHook.KeyboardHook.Stroke.KEY_DOWN || s.Stroke == GlobalHook.KeyboardHook.Stroke.SYSKEY_DOWN;
            Boolean isUp = s.Stroke == GlobalHook.KeyboardHook.Stroke.KEY_UP || s.Stroke == GlobalHook.KeyboardHook.Stroke.SYSKEY_UP;

            // ホットキーはアプリ操作用の予約キーなので、マクロの記録データには含めない
            if (s.Key == HotkeyToggleRecord || s.Key == HotkeyTogglePlay)
            {
                if (isDown)
                {
                    // リピート抑制。対応するKeyUpが来るまでは連続トグルさせない
                    if (!pressedHotkeys.Add(s.Key))
                    {
                        return;
                    }

                    if (s.Key == HotkeyToggleRecord)
                    {
                        ToggleRecording();
                    }
                    else
                    {
                        TogglePlay();
                    }
                }
                else if (isUp)
                {
                    pressedHotkeys.Remove(s.Key);
                }

                return;
            }

            if (!isRecording)
            {
                return;
            }

            if (isDown)
            {
                // すでに押されている状態なら、OSのキーリピートによるKeyDown連発なので無視する。
                // 対応するKeyUpが来るまでは「押されたまま」として扱う
                if (!pressedKeys.Add(s.Key))
                {
                    return;
                }
            }
            else if (isUp)
            {
                pressedKeys.Remove(s.Key);
            }

            QueueRow(s.Stroke.ToString(), "", "", s.Key.ToString(), ElapsedMs());
        }

        // 直前のイベントからの経過ms。記録開始直後の1件目は「記録開始からの待機」になる
        private int ElapsedMs()
        {
            int now = Environment.TickCount;
            int wait = now - lastEventTick;
            lastEventTick = now;
            return wait < 0 ? 0 : wait;
        }

        // 記録した1件をバッファに貯めるだけ(フックのコールバックを描画待ちで塞がないため)。
        // 実際のDataGridViewへの反映はgridFlushTimerがまとめて行う
        private void QueueRow(String type, String x, String y, String key, int wait)
        {
            pendingRows.Add(new String[] { type, x, y, key, wait.ToString() });
        }

        // 貯まった行をまとめてDataGridViewに反映する。1件ずつ反映するより
        // 再描画・スクロール処理の回数を大幅に減らせる
        private void FlushPendingRows()
        {
            if (pendingRows.Count == 0)
            {
                return;
            }

            dataGridView_Events.SuspendLayout();

            int lastIdx = -1;
            foreach (String[] r in pendingRows)
            {
                lastIdx = dataGridView_Events.Rows.Add();
                DataGridViewRow row = dataGridView_Events.Rows[lastIdx];
                row.Cells[0].Value = r[0];
                row.Cells[1].Value = r[1];
                row.Cells[2].Value = r[2];
                row.Cells[3].Value = r[3];
                row.Cells[4].Value = r[4];
            }

            pendingRows.Clear();

            dataGridView_Events.ResumeLayout();

            if (lastIdx >= 0)
            {
                dataGridView_Events.FirstDisplayedScrollingRowIndex = lastIdx;
            }
        }

        // *******************************************************************************
        // 再生

        private void button_Play_Click(object sender, EventArgs e)
        {
            TogglePlay();
        }

        // 再生開始/停止を切り替える。ボタンクリックからも変換キーホットキーからも呼ばれる
        private void TogglePlay()
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

            // 再生終了後にカーソルを戻せるよう、今の位置を覚えておく
            cursorPositionBeforePlay = Cursor.Position;

            isPlaying = true;
            stopPlayRequested = false;
            button_Play.Text = "停止";

            Task.Run(() => PlayLoop(rows, loopCount));
        }

        private List<String[]> SnapshotRows()
        {
            List<String[]> list = new List<String[]>();
            foreach (DataGridViewRow row in dataGridView_Events.Rows)
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
                for (int idx = 0; idx < rows.Count && !stopPlayRequested; idx++)
                {
                    String[] r = rows[idx];

                    this.Invoke((MethodInvoker)(() => HighlightPlayingRow(idx)));

                    int wait = util.GetInteger(r[4]);
                    if (wait > 0)
                    {
                        Thread.Sleep(wait);
                    }

                    PlayOneEvent(r[0], r[1], r[2], r[3]);
                }
            }

            // マウスカーソルを再生開始前の位置に戻す
            Cursor.Position = cursorPositionBeforePlay;

            isPlaying = false;
            stopPlayRequested = false;
            this.Invoke((MethodInvoker)(() =>
            {
                button_Play.Text = "再生";
                HighlightPlayingRow(-1);
            }));
        }

        // 再生中の行の背景色を変えて、今どの行を実行中か分かるようにする。
        // idxに-1を渡すとハイライトを消す
        private void HighlightPlayingRow(int idx)
        {
            if (highlightedRowIndex >= 0 && highlightedRowIndex < dataGridView_Events.Rows.Count)
            {
                dataGridView_Events.Rows[highlightedRowIndex].DefaultCellStyle.BackColor = Color.Empty;
            }

            if (idx >= 0 && idx < dataGridView_Events.Rows.Count)
            {
                dataGridView_Events.Rows[idx].DefaultCellStyle.BackColor = Color.LightYellow;
                dataGridView_Events.FirstDisplayedScrollingRowIndex = idx;
            }

            highlightedRowIndex = idx;
        }

        // Ctrl+V。DataGridViewはCtrl+A(全選択)/Ctrl+C(コピー)は標準対応してるけど、
        // 貼り付けだけは無いので自前で実装する
        private void dataGridView_Events_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard();
                e.Handled = true;
            }
        }

        // クリップボードのタブ区切りテキスト(Excelや他のDataGridViewからのコピーと同じ形式)を貼り付ける。
        // 列がグリッドの列数をはみ出る分は切り捨て、行が足りない場合は全部貼り付けられるように追加する
        private void PasteFromClipboard()
        {
            if (isRecording || isPlaying || !Clipboard.ContainsText())
            {
                return;
            }

            String[] lines = Clipboard.GetText().Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            if (lines.Length == 0)
            {
                return;
            }

            DataGridViewCell startCell = dataGridView_Events.CurrentCell;
            int startRow = (startCell != null) ? startCell.RowIndex : 0;
            int startCol = (startCell != null) ? startCell.ColumnIndex : 0;

            for (int i = 0; i < lines.Length; i++)
            {
                int rowIndex = startRow + i;

                // 行が足りなければ、貼り付け分を全部入れられるように追加する
                while (rowIndex >= dataGridView_Events.Rows.Count)
                {
                    dataGridView_Events.Rows.Add();
                }

                String[] values = lines[i].Split('\t');
                for (int j = 0; j < values.Length; j++)
                {
                    int colIndex = startCol + j;

                    // 列がグリッドの列数をはみ出る分は切り捨てる
                    if (colIndex >= dataGridView_Events.Columns.Count)
                    {
                        break;
                    }

                    dataGridView_Events.Rows[rowIndex].Cells[colIndex].Value = values[j];
                }
            }
        }

        // 右クリックしたセルの行(手動で行の追加/削除をする対象)。右クリック無しの状態(-1)なら末尾扱い
        private int contextMenuRowIndex = -1;

        // 右クリック時に、そのセルの行を選択状態にしつつ対象行を覚えておく
        private void dataGridView_Events_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            contextMenuRowIndex = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                dataGridView_Events.ClearSelection();
                dataGridView_Events.Rows[e.RowIndex].Selected = true;
            }
        }

        // 行が無い場所を右クリックした場合は「行の削除」を選べないようにする
        private void contextMenuStrip_Grid_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isRecording || isPlaying)
            {
                e.Cancel = true;
                return;
            }

            menuItem_DeleteRow.Enabled = contextMenuRowIndex >= 0 && contextMenuRowIndex < dataGridView_Events.Rows.Count;
        }

        // 右クリックした行のすぐ下に空行を1件挿入する(何も無い場所を右クリックした場合は末尾に追加)
        private void menuItem_AddRow_Click(object sender, EventArgs e)
        {
            int insertAt = (contextMenuRowIndex >= 0) ? contextMenuRowIndex + 1 : dataGridView_Events.Rows.Count;
            dataGridView_Events.Rows.Insert(insertAt, 1);
        }

        // 右クリックした行を削除する。KeyDown/SysKeyDownの行なら、非表示になっている
        // 対応するKeyUp/SysKeyUp行も一緒に探して削除する(片方だけ残って孤立するのを防ぐため)
        private void menuItem_DeleteRow_Click(object sender, EventArgs e)
        {
            if (contextMenuRowIndex < 0 || contextMenuRowIndex >= dataGridView_Events.Rows.Count)
            {
                return;
            }

            int pairedUpRowIndex = FindPairedKeyUpRowIndex(contextMenuRowIndex);

            dataGridView_Events.Rows.RemoveAt(contextMenuRowIndex);

            if (pairedUpRowIndex < 0)
            {
                return;
            }

            // 先に消した行より後ろにあった場合、その分インデックスが1つ前にずれる
            if (pairedUpRowIndex > contextMenuRowIndex)
            {
                pairedUpRowIndex--;
            }

            dataGridView_Events.Rows.RemoveAt(pairedUpRowIndex);
        }

        // downRowIndexがKeyDown/SysKeyDownの行なら、それより後ろで最初に見つかる
        // 「同じKeyの対応するKeyUp/SysKeyUp行」のインデックスを返す。見つからなければ-1
        private int FindPairedKeyUpRowIndex(int downRowIndex)
        {
            DataGridViewRow downRow = dataGridView_Events.Rows[downRowIndex];
            String downType = Convert.ToString(downRow.Cells[col_Type.Index].Value);
            String key = Convert.ToString(downRow.Cells[col_Key.Index].Value);

            String upType;
            if (downType == GlobalHook.KeyboardHook.Stroke.KEY_DOWN.ToString())
            {
                upType = GlobalHook.KeyboardHook.Stroke.KEY_UP.ToString();
            }
            else if (downType == GlobalHook.KeyboardHook.Stroke.SYSKEY_DOWN.ToString())
            {
                upType = GlobalHook.KeyboardHook.Stroke.SYSKEY_UP.ToString();
            }
            else
            {
                return -1;
            }

            for (int i = downRowIndex + 1; i < dataGridView_Events.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView_Events.Rows[i];
                String type = Convert.ToString(row.Cells[col_Type.Index].Value);
                String rowKey = Convert.ToString(row.Cells[col_Key.Index].Value);

                if (type == upType && rowKey == key)
                {
                    return i;
                }
            }

            return -1;
        }

        // 行ヘッダー(左端)に行番号(0始まりのインデックス)を描画する。
        // DataGridViewには自動で行番号を出す機能が無いので、定番のRowPostPaintで自前描画する
        private void dataGridView_Events_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            String rowIndexStr = e.RowIndex.ToString();
            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dataGridView_Events.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIndexStr, dataGridView_Events.Font, SystemBrushes.ControlText, headerBounds, format);
        }

        // マウスイベントの行なのにKey列にも値が入っているか判定する。
        // (PlayOneEventはEvent列でマウス/キーボードを判定するので、この場合Key列は再生時に無視される)
        private Boolean IsKeyIgnoredOnMouseRow(DataGridViewRow row, out String message)
        {
            String eventType = Convert.ToString(row.Cells[col_Type.Index].Value);
            String keyValue = Convert.ToString(row.Cells[col_Key.Index].Value);

            InputSimulation.InputSimulator.MouseStroke mouseStroke;
            Boolean isMouseRow = Enum.TryParse<InputSimulation.InputSimulator.MouseStroke>(eventType, out mouseStroke);

            if (isMouseRow && !String.IsNullOrEmpty(keyValue))
            {
                message = "マウスイベント(" + eventType + ")の行なので、Key(" + keyValue + ")は再生時には使われず無視されるよ";
                return true;
            }

            message = "";
            return false;
        }

        // マウスイベントの行なのにKey列にも値が入っている場合、Key列の背景を薄い赤にして知らせる。
        // 加えてErrorTextにも同じ内容を入れて、セルのエラーアイコン+ホバー時の吹き出しでも分かるようにする
        private void dataGridView_Events_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != col_Key.Index || e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dataGridView_Events.Rows[e.RowIndex];
            String message;
            if (IsKeyIgnoredOnMouseRow(row, out message))
            {
                e.CellStyle.BackColor = Color.MistyRose;
            }

            row.Cells[col_Key.Index].ErrorText = message;
        }

        // Event列を書き換えた直後もKey列の警告表示(CellFormatting)がすぐ反映されるように、
        // 明示的にKey列セルを再描画する(別列の値変更ではCellFormattingが自動では呼ばれないため)。
        // 記録・貼り付け・XML読込・手動編集、どの経路でEvent列がセットされてもここを通るので、
        // KeyUp行を非表示にする処理もまとめてここでやる
        private void dataGridView_Events_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != col_Type.Index)
            {
                return;
            }

            dataGridView_Events.InvalidateCell(col_Key.Index, e.RowIndex);
            UpdateRowVisibility(e.RowIndex);
        }

        // KeyUp/SysKeyUpの行は、リピート抑制のための内部データとしては保持したまま、
        // テーブルの見た目からは隠す(データは残るので保存/再生には引き続き使われる)
        private void UpdateRowVisibility(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView_Events.Rows.Count)
            {
                return;
            }

            DataGridViewRow row = dataGridView_Events.Rows[rowIndex];
            String eventType = Convert.ToString(row.Cells[col_Type.Index].Value);

            Boolean isKeyUp = eventType == GlobalHook.KeyboardHook.Stroke.KEY_UP.ToString()
                || eventType == GlobalHook.KeyboardHook.Stroke.SYSKEY_UP.ToString();

            row.Visible = !isKeyUp;
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

            dataGridView_Events.Rows.Clear();
        }

        // コンボボックスで設定ファイルを選び直したら、そのままそれを読み込む(Cheetosと同じ挙動)
        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = System.IO.Directory.GetCurrentDirectory() + @"\" + comboBox_Profile.Text;
            sr.LoadProc(LoadFileName, this);
        }

        private void button_ProfileLoad_Click(object sender, EventArgs e)
        {
            String LoadFileName = fio.SelectLoadFileName(comboBox_Profile.Text, System.IO.Directory.GetCurrentDirectory());
            if (sr.LoadProc(LoadFileName, this))
            {
                comboBox_Profile.Text = System.IO.Path.GetFileName(LoadFileName);
            }
        }

        // 読込ボタンと同じ感覚で使えるよう、「現在のファイルに上書きしますか?」の確認は挟まず、
        // 常にダイアログを直接開く(SelectSaveFileNameのCheetos流の確認ステップはあえて使わない)
        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = comboBox_Profile.Text;
            dlg.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            dlg.Filter = "XMLファイル(*.xml)|*.xml|すべてのファイル(*.*)|*.*";
            dlg.Title = "保存する設定ファイルを選択してください";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            String SaveFileName = dlg.FileName;
            if (sr.SaveSetting(SaveFileName))
            {
                util.UpdateProfileList(ref comboBox_Profile, System.IO.Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存したよ♪" + Environment.NewLine + SaveFileName);
            }
        }
    }
}
