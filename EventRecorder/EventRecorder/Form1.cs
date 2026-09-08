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
        private SaveRestore sr = new SaveRestore();

        // プレイリスト再生時、各行の設定ファイルを読み込む専用のインスタンス。
        // srを使い回すと各ファイルに埋め込まれたプレイリストのスナップショットで
        // 今操作中のプレイリストが上書きされてしまうため、記録データだけを登録した別インスタンスで分離する
        private SaveRestore playbackLoader = new SaveRestore();

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

        // タイトルバーの基本文字列。記録中/再生中はここに状態を追記する
        private const String BaseTitle = "EventRecorder";

        // 記録開始/停止のホットキー。ボタンクリックだとクリック自体のマウスイベントが
        // 記録に混ざってしまうため、キー操作で完結できるようにしている
        private const Keys HotkeyToggleRecord = Keys.F1;

        // 再生開始/停止のホットキー。IME変換キー単独、またはShift+F2のどちらでも発動する
        private const Keys HotkeyTogglePlay = Keys.IMEConvert;
        private const Keys HotkeyTogglePlayAlt = Keys.F2;

        // 記録したがまだDataGridViewに反映していない行(フックのコールバックを
        // 描画待ちで塞がないよう、一旦ここに貯めてタイマーでまとめて反映する)
        private List<String[]> pendingRows = new List<String[]>();
        private System.Windows.Forms.Timer gridFlushTimer;

        // マウスカーソル座標の常時表示用タイマー
        private System.Windows.Forms.Timer mousePosTimer;

        // 起動時に自動読込するデフォルトの設定ファイル名(Cheetosに倣う)
        private readonly String SettingFileName = @"EventRecorder.xml";

        // 最小化する瞬間、OSから一時的にクライアント領域が極小サイズのリサイズ通知が来ることがあり、
        // Anchor/Fillでの再レイアウトがその極小サイズを基準に確定してしまい、元に戻した時に
        // コントロールが重なる/見切れる不具合の原因になる。最小化中はレイアウト計算自体をスキップし、
        // 元のサイズに戻った時のOnSizeChangedで正しく再計算させる
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            base.OnSizeChanged(e);
        }

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
            playbackLoader.RegistItemForPlayback(this);

            // 起動時にデフォルト設定を読み込み、コンボボックスに設定ファイル一覧を表示する(Cheetosと同じ手順)
            sr.LoadProc(SettingFileName, this);
            util.UpdateProfileList(ref comboBox_Profile);

            // プレイリストが空のままだと使うたびに毎回「行の追加」を押す羽目になるため、
            // 起動時点で編集開始しやすいよう空行を5行用意しておく。
            // ファイル未選択のまま実行されると困るので、デフォルトでは実行チェックは外しておく
            for (int i = 0; i < 5; i++)
            {
                AddPlaylistRow(i, isEnabled: false);
            }
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

        // 現在の記録中/再生中の状態をタイトルバーに反映する
        private void UpdateTitle()
        {
            if (isRecording)
            {
                this.Text = BaseTitle + " - 記録中";
            }
            else if (isPlaying)
            {
                this.Text = BaseTitle + " - プレイバック中";
            }
            else
            {
                this.Text = BaseTitle;
            }
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
                UpdateTitle();
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
            UpdateTitle();
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

            // ホットキーはアプリ操作用の予約キーなので、マクロの記録データには含めない。
            // Shift併用が必要なもの(Shift+F2)は、押した瞬間にShiftが押されていた
            // 場合だけホットキー扱いにする。素のF2は本来の用途やマクロ記録にそのまま使える
            if (isDown)
            {
                Boolean isShiftDown = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                Boolean isRecordHotkey = s.Key == HotkeyToggleRecord;
                Boolean isPlayHotkey = s.Key == HotkeyTogglePlay
                    || (s.Key == HotkeyTogglePlayAlt && isShiftDown);

                if (isRecordHotkey || isPlayHotkey)
                {
                    // リピート抑制。対応するKeyUpが来るまでは連続トグルさせない
                    if (!pressedHotkeys.Add(s.Key))
                    {
                        return;
                    }

                    if (isRecordHotkey)
                    {
                        ToggleRecording();
                    }
                    else
                    {
                        TogglePlay();
                    }

                    return;
                }
            }
            else if (isUp)
            {
                // Down時にホットキーとして処理したキーのUpは、マクロ記録に含めず消費する。
                // (Up時点で先にShiftを離していても、Down時の判定結果に合わせて対称に扱う)
                if (pressedHotkeys.Remove(s.Key))
                {
                    return;
                }
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

            if (lastIdx >= 0 && dataGridView_Events.Visible && dataGridView_Events.RowCount > 0)
            {
                try
                {
                    dataGridView_Events.FirstDisplayedScrollingRowIndex = lastIdx;
                }
                catch (InvalidOperationException)
                {
                    // グリッドが未表示/高さ0等で行を表示できないタイミングでは
                    // スクロール位置合わせを諦めて記録自体は継続する(既知のWinForms挙動)
                }
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
            UpdateTitle();

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
            try
            {
                PlayRows(rows, loopCount);

                // マウスカーソルを再生開始前の位置に戻す
                Cursor.Position = cursorPositionBeforePlay;
            }
            finally
            {
                // 途中で例外が起きても、必ず「再生中」状態を解除する。
                // ここを素通りしてしまうとisPlayingがtrueのまま固まり、記録も再生も
                // 二度とできなくなる(アプリが固まって見える不具合の原因になっていた)
                isPlaying = false;
                stopPlayRequested = false;
                this.Invoke((MethodInvoker)(() =>
                {
                    button_Play.Text = "再生";
                    UpdateTitle();
                    HighlightPlayingRow(-1);
                }));
            }
        }

        // rowsをloopCount回再生する処理そのもの(前後の状態管理は呼び出し元の責務)。
        // タブ1の単発再生・タブ2のプレイリスト再生の両方から共通で使う
        private void PlayRows(List<String[]> rows, int loopCount)
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

                if (dataGridView_Events.Visible)
                {
                    try
                    {
                        dataGridView_Events.FirstDisplayedScrollingRowIndex = idx;
                    }
                    catch (InvalidOperationException)
                    {
                        // グリッドが未表示/高さ0等で行を表示できないタイミングでは
                        // スクロール位置合わせを諦めて再生自体は継続する(既知のWinForms挙動)
                    }
                }
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

            // 左クリックで複数行選択済みの状態から右クリックでメニューを開きたいケースがあるため、
            // 右クリックした行がすでに選択済みならその選択状態を維持する。
            // 未選択の行を右クリックした場合だけ、その行の単一選択に切り替える。
            // 既定のRowHeaderSelectモードでは、行ヘッダーではなくセルのドラッグで複数選択すると
            // Rows[].Selectedはfalseのままになるため、右クリックしたセル自体の選択状態もあわせて見る
            Boolean isAlreadySelected = e.RowIndex >= 0 &&
                ((e.ColumnIndex >= 0 && dataGridView_Events.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
                || dataGridView_Events.Rows[e.RowIndex].Selected);

            if (e.RowIndex >= 0 && !isAlreadySelected)
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

        // 選択されている行(複数選択時は全行、未選択なら右クリックした行)を削除する。
        // KeyDown/SysKeyDownの行なら、非表示になっている対応するKeyUp/SysKeyUp行も
        // 一緒に探して削除する(片方だけ残って孤立するのを防ぐため)
        private void menuItem_DeleteRow_Click(object sender, EventArgs e)
        {
            List<int> targetIndexes = GetSelectedOrContextMenuRowIndexes(dataGridView_Events, contextMenuRowIndex);
            if (targetIndexes.Count == 0)
            {
                return;
            }

            // ペアのKeyUp行も含めて、重複なく集める
            HashSet<int> indexesToRemove = new HashSet<int>(targetIndexes);
            foreach (int idx in targetIndexes)
            {
                int pairedUpRowIndex = FindPairedKeyUpRowIndex(idx);
                if (pairedUpRowIndex >= 0)
                {
                    indexesToRemove.Add(pairedUpRowIndex);
                }
            }

            // 後ろのインデックスから消していけば、前方のインデックスがずれる心配がない
            foreach (int idx in indexesToRemove.OrderByDescending(x => x))
            {
                dataGridView_Events.Rows.RemoveAt(idx);
            }
        }

        // 複数行選択済みならその全行、未選択なら右クリックした1行(fallbackRowIndex)を対象にする。
        // 記録グリッド・プレイリストグリッドの両方の「行の削除」から共通で使う
        private List<int> GetSelectedOrContextMenuRowIndexes(DataGridView grid, int fallbackRowIndex)
        {
            HashSet<int> indexes = new HashSet<int>();

            foreach (DataGridViewRow row in grid.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    indexes.Add(row.Index);
                }
            }

            // 既定のRowHeaderSelectモードでは、行ヘッダーではなくセルをドラッグして複数選択することが
            // 多く、その場合SelectedRowsには何も入らないため、SelectedCellsからも対象行を拾う
            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                if (cell.RowIndex >= 0 && !grid.Rows[cell.RowIndex].IsNewRow)
                {
                    indexes.Add(cell.RowIndex);
                }
            }

            if (indexes.Count == 0 && fallbackRowIndex >= 0 && fallbackRowIndex < grid.Rows.Count)
            {
                indexes.Add(fallbackRowIndex);
            }

            return indexes.OrderBy(x => x).ToList();
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
        // DataGridViewには自動で行番号を出す機能が無いので、定番のRowPostPaintで自前描画する。
        // dataGridView_Events・dataGridView_Playlist両方から共用するので、対象はsenderから取る
        private void dataGridView_Events_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            String rowIndexStr = e.RowIndex.ToString();
            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIndexStr, grid.Font, SystemBrushes.ControlText, headerBounds, format);
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

        // *******************************************************************************
        // プレイリスト(タブ2): 複数の設定ファイルを指定した順番で連続再生する

        // 右クリックしたセルの行(プレイリスト側)。右クリック無しの状態(-1)なら末尾扱い
        private int contextMenuPlaylistRowIndex = -1;

        // タブを切り替えたタイミングで、プレイリストのプルダウンの中身を
        // 「設定値読込/保存」で使っているのと同じ*.xml一覧に更新する
        private void tabControl_Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl_Main.SelectedTab == tabPage_Playlist)
            {
                RefreshPlaylistFileList();
            }
        }

        private void RefreshPlaylistFileList()
        {
            col_PlaylistFile.Items.Clear();
            col_PlaylistFile.Items.AddRange(GetProfileFileNames().ToArray());
        }

        // util.UpdateProfileListが内部でやっているのと同じ「カレントフォルダ配下の*.xmlをファイル名一覧にする」
        // 処理。comboBox_Profileの選択状態には触りたくないので、直接ファイルシステムから作り直す
        private List<String> GetProfileFileNames()
        {
            List<String> names = new List<String>();
            String dir = System.IO.Directory.GetCurrentDirectory();
            if (!System.IO.Directory.Exists(dir))
            {
                return names;
            }

            String[] files = System.IO.Directory.GetFiles(dir, "*.xml", System.IO.SearchOption.AllDirectories);
            foreach (String f in files)
            {
                names.Add(f.Substring(dir.Length + 1));
            }

            return names;
        }

        private void dataGridView_Playlist_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            contextMenuPlaylistRowIndex = e.RowIndex;

            // 記録グリッドと同様、右クリックしたセル/行がすでに選択済みなら選択状態を維持する
            Boolean isAlreadySelected = e.RowIndex >= 0 &&
                ((e.ColumnIndex >= 0 && dataGridView_Playlist.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
                || dataGridView_Playlist.Rows[e.RowIndex].Selected);

            if (e.RowIndex >= 0 && !isAlreadySelected)
            {
                dataGridView_Playlist.ClearSelection();
                dataGridView_Playlist.Rows[e.RowIndex].Selected = true;
            }
        }

        private void contextMenuStrip_Playlist_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isRecording || isPlaying)
            {
                e.Cancel = true;
                return;
            }

            menuItem_PlaylistDeleteRow.Enabled = contextMenuPlaylistRowIndex >= 0 && contextMenuPlaylistRowIndex < dataGridView_Playlist.Rows.Count;
        }

        private void menuItem_PlaylistAddRow_Click(object sender, EventArgs e)
        {
            int insertAt = (contextMenuPlaylistRowIndex >= 0) ? contextMenuPlaylistRowIndex + 1 : dataGridView_Playlist.Rows.Count;
            // 右クリックで能動的に追加した行は、すぐ使うつもりのはずなので実行チェックはONにしておく
            AddPlaylistRow(insertAt, isEnabled: true);
        }

        // プレイリストに1行追加する(ループ回数=1がデフォルト。実行チェックの初期値はisEnabledで指定)。
        // 右クリックメニューの「行の追加」と、起動時の初期空行の両方から使う
        private void AddPlaylistRow(int insertAt, Boolean isEnabled)
        {
            dataGridView_Playlist.Rows.Insert(insertAt, 1);

            DataGridViewRow newRow = dataGridView_Playlist.Rows[insertAt];
            newRow.Cells[col_PlaylistEnabled.Index].Value = isEnabled;
            newRow.Cells[col_PlaylistLoopCount.Index].Value = "1";
        }

        private void menuItem_PlaylistDeleteRow_Click(object sender, EventArgs e)
        {
            List<int> targetIndexes = GetSelectedOrContextMenuRowIndexes(dataGridView_Playlist, contextMenuPlaylistRowIndex);

            foreach (int idx in targetIndexes.OrderByDescending(x => x))
            {
                dataGridView_Playlist.Rows.RemoveAt(idx);
            }
        }

        private void menuItem_PlaylistCheckAll_Click(object sender, EventArgs e)
        {
            SetAllPlaylistChecks(true);
        }

        private void menuItem_PlaylistUncheckAll_Click(object sender, EventArgs e)
        {
            SetAllPlaylistChecks(false);
        }

        private void SetAllPlaylistChecks(Boolean isChecked)
        {
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                row.Cells[col_PlaylistEnabled.Index].Value = isChecked;
            }
        }

        // チェックボックス列は、クリックしただけだと確定(コミット)されず見た目が変わらないことがあるので、
        // 値が変化した直後に明示的にコミットして即座に反映させる
        private void dataGridView_Playlist_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // チェックボックスと設定ファイルのプルダウンは、選んだ瞬間に値を確定させたい
            // (プルダウンの方は、選択直後にCellValueChangedでループ回数の初期値セットに使うため)
            Boolean isImmediateCommitTarget = dataGridView_Playlist.CurrentCell is DataGridViewCheckBoxCell
                || dataGridView_Playlist.CurrentCell is DataGridViewComboBoxCell;

            if (isImmediateCommitTarget && dataGridView_Playlist.IsCurrentCellDirty)
            {
                dataGridView_Playlist.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // プレイリストの行で設定ファイルを選ぶと、そのファイル自身に保存されている
        // ループ回数を、行のループ回数セルへ初期値として自動セットする
        // (すでに手で入力済みの値を上書きしたくはないので、あくまで選択した瞬間の初期値扱い)
        private void dataGridView_Playlist_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != col_PlaylistFile.Index)
            {
                return;
            }

            String fileName = Convert.ToString(dataGridView_Playlist.Rows[e.RowIndex].Cells[col_PlaylistFile.Index].Value);
            if (String.IsNullOrEmpty(fileName))
            {
                return;
            }

            String fullPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), fileName);
            String savedLoopCount = ReadSavedLoopCount(fullPath);
            if (savedLoopCount != null)
            {
                dataGridView_Playlist.Rows[e.RowIndex].Cells[col_PlaylistLoopCount.Index].Value = savedLoopCount;
            }
        }

        // マクロファイル自身が保存しているループ回数(textBox_Loopの値)だけを、
        // dataGridView_Events等には一切触れずに読み取る
        // (sr.LoadProcを使うとタブ1の記録データがまるごとクリア/差し替えられてしまうため使わない)
        private String ReadSavedLoopCount(String filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            try
            {
                System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(filePath);
                System.Xml.Linq.XElement el = doc.Root?.Elements("Setting")
                    .FirstOrDefault(x => (String)x.Attribute("Record") == "textBox_Loop");
                return el != null ? el.Value.Trim() : null;
            }
            catch (System.Xml.XmlException)
            {
                return null;
            }
        }

        // プレイリストの1行分(ファイル名+そのファイル専用のループ回数)
        private class PlaylistEntry
        {
            public String FileName;
            public int LoopCount;
        }

        // プレイリストの各行のうち、チェックが入っていてファイルが選ばれている行だけを、
        // 上から順番に取り出す(チェックが外れている行・ファイル未選択の行は実行対象から除外)
        private List<PlaylistEntry> GetPlaylistEntries()
        {
            List<PlaylistEntry> entries = new List<PlaylistEntry>();
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                Boolean isEnabled = Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false);
                if (!isEnabled)
                {
                    continue;
                }

                String fileName = Convert.ToString(row.Cells[col_PlaylistFile.Index].Value);
                if (String.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                int loopCount = util.GetInteger(Convert.ToString(row.Cells[col_PlaylistLoopCount.Index].Value));
                if (loopCount <= 0)
                {
                    loopCount = 1;
                }

                entries.Add(new PlaylistEntry() { FileName = fileName, LoopCount = loopCount });
            }

            return entries;
        }

        // 記録・再生タブと同じ理由(isRecording/isPlayingの使い回し)で、
        // 記録中や単発再生中はプレイリストの実行を弾く。実行中の停止も同じボタンで行う
        private void button_PlaylistRun_Click(object sender, EventArgs e)
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

            List<PlaylistEntry> entries = GetPlaylistEntries();
            if (entries.Count == 0)
            {
                return;
            }

            int overallLoopCount = util.GetInteger(textBox_PlaylistLoop.Text);
            if (overallLoopCount <= 0)
            {
                overallLoopCount = 1;
            }

            cursorPositionBeforePlay = Cursor.Position;
            isPlaying = true;
            stopPlayRequested = false;
            button_PlaylistRun.Text = "停止";
            UpdateTitle();

            Task.Run(() => PlaylistPlayLoop(entries, overallLoopCount));
        }

        // プレイリスト全体をoverallLoopCount回繰り返す。各周回の中で、
        // リストの各行を上から順に読み込み→その行のループ回数分だけ再生、を繰り返す
        private void PlaylistPlayLoop(List<PlaylistEntry> entries, int overallLoopCount)
        {
            try
            {
                for (int loopNo = 0; loopNo < overallLoopCount && !stopPlayRequested; loopNo++)
                {
                    int loopDisplayNo = loopNo + 1;

                    for (int i = 0; i < entries.Count && !stopPlayRequested; i++)
                    {
                        PlaylistEntry entry = entries[i];
                        int fileNo = i + 1;
                        List<String[]> rows = null;

                        this.Invoke((MethodInvoker)(() =>
                        {
                            label_PlaylistStatus.Text = "実行中(全体" + loopDisplayNo + "/" + overallLoopCount + "): "
                                + entry.FileName + " (" + fileNo + "/" + entries.Count + ")";

                            String fullPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), entry.FileName);
                            // 再生対象の記録データ(タブ1)だけ差し替える。playbackLoaderはPlaylist側を
                            // 一切登録していないため、ファイルにプレイリストのスナップショットが
                            // 含まれていてもプレイリスト自体(タブ2)には影響しない
                            playbackLoader.LoadProc(fullPath, this, false);
                            rows = SnapshotRows();
                        }));

                        if (rows != null && rows.Count > 0)
                        {
                            PlayRows(rows, entry.LoopCount);
                        }
                    }
                }

                Cursor.Position = cursorPositionBeforePlay;
            }
            finally
            {
                // 途中で例外が起きても、必ず「再生中」状態を解除する。
                // ここを素通りしてしまうとisPlayingがtrueのまま固まり、記録も再生も
                // 二度とできなくなる(アプリが固まって見える不具合の原因になっていた)
                isPlaying = false;
                stopPlayRequested = false;
                this.Invoke((MethodInvoker)(() =>
                {
                    button_PlaylistRun.Text = "実行";
                    label_PlaylistStatus.Text = "";
                    UpdateTitle();
                    HighlightPlayingRow(-1);
                }));
            }
        }
    }
}
