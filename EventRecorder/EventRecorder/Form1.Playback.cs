using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StandardTemplate;

namespace EventRecorder
{
    partial class Form1
    {
        // *******************************************************************************
        // 再生

        private void button_Play_Click(object sender, EventArgs e)
        {
            PlayOrStop();
        }

        // 単発再生とプレイリスト実行を1つのbutton_Play(表示名「再生」)に統合したので、
        // 開始時にどちらを動かすかはラジオボタンで選択中のモードで振り分ける。
        // 停止は共通(今動いている方が何であれ、そのままstopPlayRequestedで止める)。
        // ボタンクリックからも再生ホットキー(変換キー/Shift+F2)からも呼ばれる
        private void PlayOrStop()
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

            if (radioButton_Playback.Checked)
            {
                StartPlaylistRun();
            }
            else
            {
                StartSinglePlayback();
            }
        }

        // レコードグループの記録データ(dataGridView_Events)を単発再生する
        private void StartSinglePlayback()
        {
            if (!ValidateEventsForPlayback())
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

            // 単発再生には「全体ループ」の概念が無いので常に1/1固定にする
            playbackOverallLoopNo = 1;
            playbackOverallLoopMax = 1;
            playbackInnerLoopNo = 0;
            playbackInnerLoopMax = loopCount;

            isPlaying = true;
            stopPlayRequested = false;
            UpdatePlayButtons();
            UpdateTitle();
            MinimizeIfRequested();

            Task.Run(() => PlayLoop(rows, loopCount));
        }

        // checkBox_MinimizeOnPlayがチェックされていたら、再生開始と同時にウィンドウを最小化する。
        // 再生対象のアプリの操作を邪魔しないようにするための機能
        private void MinimizeIfRequested()
        {
            if (checkBox_MinimizeOnPlay.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        // MinimizeIfRequestedで最小化した場合、再生終了時に元の表示状態へ戻す
        private void RestoreIfMinimizedByPlay()
        {
            if (checkBox_MinimizeOnPlay.Checked && this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
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

                // PlayRows/PlayOneEventは[Type, X, Y, Key, Wait]の順を前提にしている
                list.Add(EventRowMapper.ReadFromRow(row, col_Type, col_X, col_Y, col_Key, col_Wait));
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
            catch (Exception ex)
            {
                ReportPlaybackError(ex);
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
                    UpdatePlayButtons();
                    UpdateTitle();
                    HighlightPlayingRow(-1);
                    RestoreIfMinimizedByPlay();
                }));
            }
        }

        // 再生スレッド(Task.Run)内の例外はどこにも通知されずに消えるため、UIスレッドへ投げ直して共通のエラー通知に乗せる
        private void ReportPlaybackError(Exception ex)
        {
            BeginInvoke((MethodInvoker)(() => { throw new InvalidOperationException("再生中にエラーが発生したよ", ex); }));
        }

        // rowsをloopCount回再生する処理そのもの(前後の状態管理は呼び出し元の責務)。
        // タブ1の単発再生・タブ2のプレイリスト再生の両方から共通で使う
        private void PlayRows(List<String[]> rows, int loopCount)
        {
            for (int i = 0; i < loopCount && !stopPlayRequested; i++)
            {
                playbackInnerLoopNo = i + 1;
                playbackInnerLoopMax = loopCount;

                for (int idx = 0; idx < rows.Count && !stopPlayRequested; idx++)
                {
                    String[] r = rows[idx];

                    this.Invoke((MethodInvoker)(() =>
                    {
                        HighlightPlayingRow(idx);
                        UpdateTitle();
                    }));

                    int wait = util.GetInteger(r[4]);
                    if (wait > 0)
                    {
                        InterruptibleSleep(wait);
                    }

                    if (stopPlayRequested)
                    {
                        return;
                    }

                    PlayOneEvent(r[0], r[1], r[2], r[3]);
                }
            }
        }

        // 待機を短い間隔(SleepPollIntervalMs)に分割し、都度stopPlayRequestedを見て
        // 早期に抜けられるようにする。単純にThread.Sleep(wait)のままだと、WAIT_MS行の
        // 待機時間が長い(例: 10秒)時に停止ボタンを押してもそのSleepが終わるまで
        // 一切反応しなくなってしまう不具合があったため、これに置き換えた
        private const int SleepPollIntervalMs = 50;

        private void InterruptibleSleep(int totalMs)
        {
            int remaining = totalMs;
            while (remaining > 0 && !stopPlayRequested)
            {
                int step = Math.Min(SleepPollIntervalMs, remaining);
                Thread.Sleep(step);
                remaining -= step;
            }
        }

        // 指定したグリッドのidx行を薄い黄色でハイライトし、見えるようにスクロールする。
        // idxに-1を渡すとハイライトを消す。記録中の最新行・単発再生中の実行中行・
        // プレイリスト実行中の実行中行(グリッドは呼び出し元次第)、どれも共通のこの処理を使う
        private void HighlightRow(StandardTemplate.DataGridViewEx grid, int idx, ref int highlightedIndexField)
        {
            if (highlightedIndexField >= 0 && highlightedIndexField < grid.Rows.Count)
            {
                grid.Rows[highlightedIndexField].DefaultCellStyle.BackColor = Color.Empty;
            }

            if (idx >= 0 && idx < grid.Rows.Count)
            {
                grid.Rows[idx].DefaultCellStyle.BackColor = Color.LightYellow;

                if (grid.Visible)
                {
                    try
                    {
                        grid.FirstDisplayedScrollingRowIndex = idx;
                    }
                    catch (InvalidOperationException)
                    {
                        // グリッドが未表示/高さ0等で行を表示できないタイミングでは
                        // スクロール位置合わせを諦めて処理自体は継続する(既知のWinForms挙動)
                    }
                }
            }

            highlightedIndexField = idx;
        }

        // 記録中の最新行・単発再生中の実行中行のハイライト(dataGridView_Events側)
        private void HighlightPlayingRow(int idx)
        {
            HighlightRow(dataGridView_Events, idx, ref highlightedRowIndex);
        }

        // プレイリスト実行中、今どのファイル(行)を再生しているかのハイライト(dataGridView_Playlist側)
        private void HighlightPlaylistRow(int idx)
        {
            HighlightRow(dataGridView_Playlist, idx, ref highlightedPlaylistRowIndex);
        }

        // Ctrl+V。DataGridViewはCtrl+A(全選択)/Ctrl+C(コピー)は標準対応してるけど、
        // 貼り付けだけは無いので自前で実装する
        private void dataGridView_Events_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                ClearSelectedCells(dataGridView_Events);
                e.Handled = true;
            }
        }

        // プレイリストのグリッドでもDeleteキーでセルの中身を空にできるようにする
        private void dataGridView_Playlist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ClearSelectedCells(dataGridView_Playlist);
                e.Handled = true;
                return;
            }

            // ループ数列(col_PlaylistLoopCount)にいる時は、↑/↓キーで行移動する代わりに
            // 値を1つ増減する(textBox_Loop側の↑/↓と同じ操作感にするため)
            if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                && dataGridView_Playlist.CurrentCell != null
                && dataGridView_Playlist.CurrentCell.ColumnIndex == col_PlaylistLoopCount.Index)
            {
                int delta = (e.KeyCode == Keys.Up) ? 1 : -1;
                StepPlaylistLoopCountCell(delta);
                e.Handled = true;
            }
        }

        // ループ数列のセルが編集モードになった時だけ、実体の編集用TextBoxに直接↑/↓キーを
        // フックする。編集中はキー入力がまずこのTextBoxに渡り、dataGridView_PlaylistのKeyDown
        // までは届かないため(未編集時のセル移動は上のdataGridView_Playlist_KeyDownで拾えている)。
        // 編集用コントロールはグリッド側で使い回されるので、フック済みハンドラの二重登録を防ぐ
        private TextBox playlistLoopCountEditingControl;

        private void dataGridView_Playlist_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (playlistLoopCountEditingControl != null)
            {
                playlistLoopCountEditingControl.KeyDown -= PlaylistLoopCountEditingControl_KeyDown;
                playlistLoopCountEditingControl = null;
            }

            if (dataGridView_Playlist.CurrentCell == null
                || dataGridView_Playlist.CurrentCell.ColumnIndex != col_PlaylistLoopCount.Index)
            {
                return;
            }

            TextBox editBox = e.Control as TextBox;
            if (editBox == null)
            {
                return;
            }

            playlistLoopCountEditingControl = editBox;
            playlistLoopCountEditingControl.KeyDown += PlaylistLoopCountEditingControl_KeyDown;
        }

        private void PlaylistLoopCountEditingControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down)
            {
                return;
            }

            StepPlaylistLoopCountCell(e.KeyCode == Keys.Up ? 1 : -1);
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        // dataGridView_PlaylistのCurrentCell(col_PlaylistLoopCount想定)の値をdeltaだけ増減する。
        // 1未満にはしない(ループ数0以下は再生時に1として扱われるため)。編集中/未編集どちらでも動く
        private void StepPlaylistLoopCountCell(int delta)
        {
            if (dataGridView_Playlist.IsCurrentCellInEditMode)
            {
                TextBox editBox = dataGridView_Playlist.EditingControl as TextBox;
                if (editBox == null)
                {
                    return;
                }

                int current;
                int.TryParse(editBox.Text, out current);
                int next = Math.Max(1, (current <= 0 ? 1 : current) + delta);
                editBox.Text = next.ToString();
                editBox.SelectionStart = editBox.Text.Length;
                return;
            }

            DataGridViewCell cell = dataGridView_Playlist.CurrentCell;
            int currentValue;
            int.TryParse(Convert.ToString(cell.Value), out currentValue);
            int nextValue = Math.Max(1, (currentValue <= 0 ? 1 : currentValue) + delta);
            cell.Value = nextValue.ToString();
        }

        // 選択中のセルの中身を空にする(行そのものは削除しない。行削除は右クリックメニューの担当)。
        // 記録中/再生中は誤操作防止のため無効にする。複数セルをまとめて1回のCtrl+Zで
        // 戻せるよう、DataGridViewExのUndoバッチでまとめる
        private void ClearSelectedCells(StandardTemplate.DataGridViewEx grid)
        {
            if (isRecording || isPlaying)
            {
                return;
            }

            grid.BeginUndoBatch();
            try
            {
                foreach (DataGridViewCell cell in grid.SelectedCells)
                {
                    // ComboBoxセル(プレイリストの設定ファイル列)は選択肢(Items)に無い値を許容しないため、
                    // nullを入れようとするとDataError(既定のエラーダイアログ)になる。クリア対象から除外する
                    if (cell.ReadOnly || cell is DataGridViewComboBoxCell)
                    {
                        continue;
                    }

                    cell.Value = null;
                }
            }
            finally
            {
                grid.EndUndoBatch();
            }
        }

        // 値の設定などで想定外のDataErrorが起きても、既定のエラーダイアログを出さずに無視して
        // 処理を継続する(ClearSelectedCellsで防いだ問題以外にも同種の不具合が今後起きうるための保険)
        private void dataGridView_Events_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Playlist_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
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

            // X/Y/Key列は非表示なので、実際に見えている列(DisplayIndex順)だけを貼り付け対象にする
            List<DataGridViewColumn> visibleColumns = GetVisibleColumnsInDisplayOrder(dataGridView_Events);

            DataGridViewCell startCell = dataGridView_Events.CurrentCell;
            int startRow = (startCell != null) ? startCell.RowIndex : 0;
            int startVisibleCol = (startCell != null) ? visibleColumns.FindIndex(c => c.Index == startCell.ColumnIndex) : 0;
            if (startVisibleCol < 0)
            {
                startVisibleCol = 0;
            }

            // 複数セルへの貼り付けをまとめて1回のCtrl+Zで戻せるよう、Undoバッチでまとめる
            dataGridView_Events.BeginUndoBatch();
            try
            {
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
                        int visibleColIndex = startVisibleCol + j;

                        // 見えている列数をはみ出る分は切り捨てる
                        if (visibleColIndex >= visibleColumns.Count)
                        {
                            break;
                        }

                        dataGridView_Events.Rows[rowIndex].Cells[visibleColumns[visibleColIndex].Index].Value = values[j];
                    }
                }
            }
            finally
            {
                dataGridView_Events.EndUndoBatch();
            }
        }

        // gridの表示列(Visible=true)だけを、DisplayIndex順に並べて返す
        private static List<DataGridViewColumn> GetVisibleColumnsInDisplayOrder(DataGridView grid)
        {
            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Visible)
                {
                    columns.Add(col);
                }
            }

            columns.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));
            return columns;
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

        // 再生時にint.Parse/Enum.Parse相当が失敗して、値が読めないまま無言で再生が止まって
        // しまう行を検出する(実際に例外の原因になりうる値だけをチェックする)。
        // 認識できないEvent種別の行(例: 打ち間違い)は、再生時に静かにスキップされるだけで
        // 例外は起きないため、ここではチェック対象にしない
        private Boolean IsRowInvalidForPlayback(DataGridViewRow row, out String message)
        {
            String type = Convert.ToString(row.Cells[col_Type.Index].Value);

            if (IsWaitEventType(type))
            {
                String wait = Convert.ToString(row.Cells[col_Wait.Index].Value);
                int waitValue;
                if (!int.TryParse(wait, out waitValue) || waitValue < 0)
                {
                    message = "WAIT_MS行の待機時間(" + wait + ")が数値として読み取れないよ";
                    return true;
                }

                message = "";
                return false;
            }

            InputSimulation.InputSimulator.MouseStroke mouseStroke;
            if (Enum.TryParse<InputSimulation.InputSimulator.MouseStroke>(type, out mouseStroke))
            {
                String x = Convert.ToString(row.Cells[col_X.Index].Value);
                String y = Convert.ToString(row.Cells[col_Y.Index].Value);
                int xValue, yValue;
                if (!int.TryParse(x, out xValue) || !int.TryParse(y, out yValue))
                {
                    message = "マウスイベント(" + type + ")のX/Y(" + x + ", " + y + ")が数値として読み取れないよ";
                    return true;
                }

                message = "";
                return false;
            }

            InputSimulation.InputSimulator.KeyboardStroke keyStroke;
            if (Enum.TryParse<InputSimulation.InputSimulator.KeyboardStroke>(type, out keyStroke))
            {
                String key = Convert.ToString(row.Cells[col_Key.Index].Value);
                Keys keyCode;
                if (!Enum.TryParse<Keys>(key, out keyCode))
                {
                    message = "キーボードイベント(" + type + ")のKey(" + key + ")が認識できないよ";
                    return true;
                }

                message = "";
                return false;
            }

            message = "";
            return false;
        }

        // マウスイベントの行なのにKey列にも値が入っている場合や、再生時に読み取れない値が
        // 入っている場合に、Detail列の背景を薄いピンクにして知らせる。
        // 加えてErrorTextにも同じ内容を入れて、セルのエラーアイコン+ホバー時の吹き出しでも分かるようにする
        // (Key列自体は非表示になったため、警告表示はDetail列(実際に見えている列)に出す)
        private void dataGridView_Events_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != col_Detail.Index || e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dataGridView_Events.Rows[e.RowIndex];

            String errorMessage;
            Boolean isInvalid = IsRowInvalidForPlayback(row, out errorMessage);

            String warningMessage;
            Boolean hasWarning = IsKeyIgnoredOnMouseRow(row, out warningMessage);

            if (isInvalid || hasWarning)
            {
                e.CellStyle.BackColor = Color.MistyRose;
            }

            row.Cells[col_Detail.Index].ErrorText = isInvalid ? errorMessage : warningMessage;
        }

        // 再生開始前に全行のパラメータをチェックする。不正な値が見つかった行があれば、
        // そのセルをピンクでハイライトした上でエラーメッセージをまとめてポップアップ表示し、
        // 再生を開始させない(false を返す)
        private Boolean ValidateEventsForPlayback()
        {
            List<String> errors = new List<String>();

            foreach (DataGridViewRow row in dataGridView_Events.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                String message;
                if (IsRowInvalidForPlayback(row, out message))
                {
                    errors.Add("行" + row.Index + ": " + message);
                }
            }

            // CellFormattingによるピンクのハイライトをすぐ反映させる
            dataGridView_Events.Refresh();

            if (errors.Count == 0)
            {
                return true;
            }

            MessageBox.Show(
                "再生できない行が見つかったよ(該当セルはピンク色で表示してるよ)" + Environment.NewLine + Environment.NewLine
                    + String.Join(Environment.NewLine, errors),
                "EventRecorder - 再生前チェック",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        // Detail列とX/Y/Key列(非表示)を相互に同期させている最中、再帰的な同期を防ぐためのフラグ
        private Boolean isSyncingDetailColumn = false;

        // Event列を書き換えた直後もDetail列の警告表示(CellFormatting)がすぐ反映されるように、
        // 明示的にDetail列セルを再描画する(別列の値変更ではCellFormattingが自動では呼ばれないため)。
        // 記録・貼り付け・XML読込・手動編集、どの経路でEvent列がセットされてもここを通るので、
        // KeyUp行を非表示にする処理もまとめてここでやる。
        // さらに、実データ(X/Y/Key、非表示)とDetail列(表示・編集用)を相互に同期させる
        private void dataGridView_Events_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || isSyncingDetailColumn)
            {
                return;
            }

            if (e.ColumnIndex == col_Type.Index)
            {
                dataGridView_Events.InvalidateCell(col_Detail.Index, e.RowIndex);
                UpdateRowVisibility(e.RowIndex);
                // WAIT_MS行への/からの切り替え等、Event列の変更でDetail列の意味も変わるので再計算する
                SyncDetailFromHiddenColumns(e.RowIndex);
                return;
            }

            if (e.ColumnIndex == col_X.Index || e.ColumnIndex == col_Y.Index || e.ColumnIndex == col_Key.Index
                || e.ColumnIndex == col_Wait.Index)
            {
                // 記録・XML読込・貼り付け等で実データ(X/Y/Key、WAIT_MS行ならWait)が更新されたら、
                // Detail列の表示も追従させる
                SyncDetailFromHiddenColumns(e.RowIndex);
                return;
            }

            if (e.ColumnIndex == col_Detail.Index)
            {
                // ユーザーがDetail列を直接編集した時は、逆に実データへ書き戻す
                SyncHiddenColumnsFromDetail(e.RowIndex);
                return;
            }
        }

        // Event列が"WAIT_MS"の行は、待機のためだけの行(実際の操作を伴わない)を表す
        private const String WaitEventType = "WAIT_MS";

        // 旧バージョンでは"WAIT"という表記で保存していたため、読込時だけはこちらも
        // 同じ意味として扱えるようにしておく(IsWaitEventType参照)
        private const String LegacyWaitEventType = "WAIT";

        private static Boolean IsWaitEventType(String type)
        {
            return type == WaitEventType || type == LegacyWaitEventType;
        }

        private void SyncDetailFromHiddenColumns(int rowIndex)
        {
            DataGridViewRow row = dataGridView_Events.Rows[rowIndex];
            String type = Convert.ToString(row.Cells[col_Type.Index].Value);

            isSyncingDetailColumn = true;
            try
            {
                if (IsWaitEventType(type))
                {
                    String wait = Convert.ToString(row.Cells[col_Wait.Index].Value);
                    row.Cells[col_Detail.Index].Value = FormatWaitDetail(wait);
                }
                else
                {
                    String x = Convert.ToString(row.Cells[col_X.Index].Value);
                    String y = Convert.ToString(row.Cells[col_Y.Index].Value);
                    String key = Convert.ToString(row.Cells[col_Key.Index].Value);
                    row.Cells[col_Detail.Index].Value = FormatDetail(x, y, key);
                }
            }
            finally
            {
                isSyncingDetailColumn = false;
            }
        }

        private void SyncHiddenColumnsFromDetail(int rowIndex)
        {
            DataGridViewRow row = dataGridView_Events.Rows[rowIndex];
            String type = Convert.ToString(row.Cells[col_Type.Index].Value);
            String detail = Convert.ToString(row.Cells[col_Detail.Index].Value);

            isSyncingDetailColumn = true;
            try
            {
                if (IsWaitEventType(type))
                {
                    String wait;
                    TryParseWaitDetail(detail, out wait);
                    row.Cells[col_Wait.Index].Value = wait;
                }
                else
                {
                    String x, y, key;
                    ParseDetail(detail, out x, out y, out key);
                    row.Cells[col_X.Index].Value = x;
                    row.Cells[col_Y.Index].Value = y;
                    row.Cells[col_Key.Index].Value = key;
                }
            }
            finally
            {
                isSyncingDetailColumn = false;
            }
        }

        // マウス行は"X:123 Y:456"、キーボード行は"Key:A"の形式でDetail列に表示する
        private static String FormatDetail(String x, String y, String key)
        {
            if (!String.IsNullOrEmpty(key))
            {
                return "Key:" + key;
            }

            if (!String.IsNullOrEmpty(x) || !String.IsNullOrEmpty(y))
            {
                return "X:" + x + " Y:" + y;
            }

            return String.Empty;
        }

        // FormatDetailの逆変換。"X:123 Y:456"や"Key:A"の形式から値を取り出す
        private static void ParseDetail(String detail, out String x, out String y, out String key)
        {
            x = String.Empty;
            y = String.Empty;
            key = String.Empty;

            if (String.IsNullOrEmpty(detail))
            {
                return;
            }

            foreach (String token in detail.Split(' '))
            {
                if (token.StartsWith("X:"))
                {
                    x = token.Substring(2);
                }
                else if (token.StartsWith("Y:"))
                {
                    y = token.Substring(2);
                }
                else if (token.StartsWith("Key:"))
                {
                    key = token.Substring(4);
                }
            }
        }

        // WAIT_MS行のDetail表示(数値だけ、単位は付けない)
        private static String FormatWaitDetail(String waitMs)
        {
            return waitMs;
        }

        // FormatWaitDetailの逆変換。旧バージョンの"500ms"のような末尾"ms"付き表記が
        // 残っていた場合もそのまま数値として読めるように、その場合だけ末尾を取り除く
        private static Boolean TryParseWaitDetail(String detail, out String waitMs)
        {
            waitMs = String.Empty;

            if (String.IsNullOrEmpty(detail))
            {
                return false;
            }

            waitMs = detail.EndsWith("ms") ? detail.Substring(0, detail.Length - 2) : detail;
            return true;
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
    }
}
