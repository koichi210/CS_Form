using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EventRecorder
{
    partial class Form1
    {
        // *******************************************************************************
        // 記録

        private void button_Record_Click(object sender, EventArgs e)
        {
            ToggleRecording();
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            if (_isRecording || _isPlaying)
            {
                return;
            }

            dataGridView_Events.Rows.Clear();
        }

        // 現在の記録中/再生中の状態をタイトルバーに反映する。
        // あわせて、記録中・再生中はプロファイルのプルダウンを無効にする(切り替えると読み込みが走り、
        // 記録中・再生中の表とプレイリストが別のプロファイルの内容に差し替わってしまうため)
        private void UpdateTitle()
        {
            comboBox_Profile.Enabled = !_isRecording && !_isPlaying;

            if (_isRecording)
            {
                this.Text = _baseTitle + " - 記録中";
            }
            else if (_isPlaying)
            {
                // (表示例)プレイバック中：2 / 5：34/100
                // 「2 / 5」=全体ループ(プレイリストの全体周回。単発再生では常に1/1)の今の実行数/最大数、
                // 「34/100」=ループ(PlayRows呼び出し1回あたりの繰り返し)の今の実行回数/最大数
                this.Text = _baseTitle + " - プレイバック中："
                    + _playbackOverallLoopNo + " / " + _playbackOverallLoopMax + "："
                    + _playbackInnerLoopNo + "/" + _playbackInnerLoopMax;
            }
            else
            {
                this.Text = _baseTitle;
            }
        }

        // 単発再生とプレイリスト実行は1つのbutton_Play(表示名「再生」)に統合したので、
        // _isPlayingが変わるたびにその表示を同期させるだけでよい
        private void UpdatePlayButton()
        {
            button_Play.Text = _isPlaying ? "停止" : "再生";
        }

        // 記録開始/停止を切り替える。ボタンクリックからもF1ホットキーからも呼ばれる
        private void ToggleRecording()
        {
            if (_isRecording)
            {
                GlobalHook.MouseHook.Stop();
                _gridFlushTimer.Stop();
                FlushPendingRows();

                // Ctrl+F5のような修飾キー付きのホットキーで止めた時に、修飾キーのDownだけが残って
                // 再生後も押されたままにならないよう片付ける(WPF版と同じ)
                if (ReleaseUnpairedModifierKeys(dataGridView_Events, col_Type, col_X, col_Y, col_Key, col_Wait, _pressedKeys))
                {
                    HighlightEventRow(dataGridView_Events.Rows.Count - 1);
                }
                _pressedKeys.Clear();

                _isRecording = false;
                button_Record.Text = "記録";
                UpdateTitle();
                return;
            }

            // 終了処理中は新しく記録を始めない
            if (_isPlaying || _isExiting)
            {
                return;
            }

            // 記録開始時に既存の行はクリアしない(既存のマクロに追記で記録したい場合があるため)
            _pendingRows.Clear();
            _pressedKeys.Clear();
            _lastEventTick = Environment.TickCount;

            GlobalHook.MouseHook.AddEvent(OnMouseEvent);
            GlobalHook.MouseHook.Start();

            _gridFlushTimer.Start();
            _isRecording = true;
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

            // ホイールは回転量(mouseDataの上位ワード)も残す。以前は残しておらず、再生は1行=1ノッチ固定だった
            Boolean isWheel = s.Stroke == GlobalHook.MouseHook.Stroke.WHEEL_UP || s.Stroke == GlobalHook.MouseHook.Stroke.WHEEL_DOWN;
            String delta = isWheel ? FormatWheelDelta(s.Data) : "";

            QueueRow(s.Stroke.ToString(), s.X.ToString(), s.Y.ToString(), "", TakeElapsedMsAndReset(), delta);
        }

        private void OnKeyboardEvent(ref GlobalHook.KeyboardHook.StateKeyboard s)
        {
            if (s.Stroke == GlobalHook.KeyboardHook.Stroke.UNKNOWN)
            {
                return;
            }

            // キーバインド設定画面が開いている間は、テスト入力したキーがホットキーとして
            // 誤発動しないよう、記録データへの取り込みも含めてここで丸ごと処理を止める
            if (_isHotkeySettingsOpen)
            {
                return;
            }

            Boolean isDown = s.Stroke == GlobalHook.KeyboardHook.Stroke.KEY_DOWN || s.Stroke == GlobalHook.KeyboardHook.Stroke.SYSKEY_DOWN;
            Boolean isUp = s.Stroke == GlobalHook.KeyboardHook.Stroke.KEY_UP || s.Stroke == GlobalHook.KeyboardHook.Stroke.SYSKEY_UP;

            // ホットキーはアプリ操作用の予約キーなので、マクロの記録データには含めない。
            // s.Keyはフックから来る生のキー(修飾キーは含まない)なので、押した瞬間の
            // Ctrl/Alt/Shiftの状態を合わせて現在の組み合わせを作ってから、ユーザー設定の
            // ホットキー(_hotkeyToggleRecord/_hotkeyTogglePlay。これも同じ表現方法)と比較する
            if (isDown)
            {
                Keys heldModifiers = Control.ModifierKeys & (Keys.Control | Keys.Alt | Keys.Shift);
                Keys combo = s.Key | heldModifiers;
                Boolean isRecordHotkey = combo == _hotkeyToggleRecord;
                Boolean isPlayHotkey = combo == _hotkeyTogglePlay;

                if (isRecordHotkey || isPlayHotkey)
                {
                    // リピート抑制。対応するKeyUpが来るまでは連続トグルさせない
                    if (!_pressedHotkeys.Add(s.Key))
                    {
                        return;
                    }

                    // ホットキーの処理(再生前チェックのエラー表示など、MessageBoxを出すことがある)は、
                    // フックのコールバックを抜けてから動かす。コールバックの中でモーダル画面を出すと、閉じるまで
                    // コールバックが戻らずキー入力全体が固まる上、一定時間でWindowsにフックを黙って外されるため
                    if (isRecordHotkey)
                    {
                        BeginInvoke((MethodInvoker)ToggleRecording);
                    }
                    else
                    {
                        BeginInvoke((MethodInvoker)PlayOrStop);
                    }

                    return;
                }
            }
            else if (isUp)
            {
                // Down時にホットキーとして処理したキーのUpは、マクロ記録に含めず消費する。
                // (Up時点で先にShiftを離していても、Down時の判定結果に合わせて対称に扱う)
                if (_pressedHotkeys.Remove(s.Key))
                {
                    return;
                }
            }

            if (!_isRecording)
            {
                return;
            }

            if (isDown)
            {
                // すでに押されている状態なら、OSのキーリピートによるKeyDown連発なので無視する。
                // 対応するKeyUpが来るまでは「押されたまま」として扱う
                if (!_pressedKeys.Add(s.Key))
                {
                    return;
                }
            }
            else if (isUp)
            {
                _pressedKeys.Remove(s.Key);
            }

            QueueRow(s.Stroke.ToString(), "", "", s.Key.ToString(), TakeElapsedMsAndReset());
        }

        // Ctrl/Shift/Alt(左右の区別ありも含む)
        private static readonly HashSet<Keys> _modifierKeys = new HashSet<Keys>
        {
            Keys.ControlKey, Keys.LControlKey, Keys.RControlKey,
            Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey,
            Keys.Menu, Keys.LMenu, Keys.RMenu,
        };

        // 記録停止時に、対になるUpが記録されていない修飾キー(Ctrl/Shift/Alt)のDown行を片付ける。行を変えたらtrue。
        // Ctrl+F5のような修飾キー付きのホットキーで記録を止めると、CtrlのDownだけが記録され(Upは停止後なので記録されない)、
        // 再生後もCtrlが押されたままになっていた。pressedKeysは記録中にDownしてまだUpしていないキー。
        // ・後ろにWAIT_MS行(と同じく片付け対象の修飾キーのDown行)しか無い: ホットキーのために押しただけなのでDown行を消す
        // ・後ろに他の操作がある(Ctrlを押したままクリックした等): Ctrl+クリックの意味を残すため、末尾に対のUp行を足す
        // (internal static: Form1を生成せずにテストできるようにするため)
        internal static Boolean ReleaseUnpairedModifierKeys(
            DataGridView grid,
            DataGridViewColumn colType, DataGridViewColumn colX, DataGridViewColumn colY,
            DataGridViewColumn colKey, DataGridViewColumn colWait,
            IEnumerable<Keys> pressedKeys)
        {
            HashSet<String> unpairedKeys = new HashSet<String>(pressedKeys.Where(k => _modifierKeys.Contains(k)).Select(k => k.ToString()));
            if (unpairedKeys.Count == 0)
            {
                return false;
            }

            Boolean changed = false;
            for (int i = grid.Rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow row = grid.Rows[i];
                String type = Convert.ToString(row.Cells[colType.Index].Value);
                if (IsWaitEventType(type))
                {
                    continue;
                }

                if (GetPairedKeyUpType(type) == null || !unpairedKeys.Remove(Convert.ToString(row.Cells[colKey.Index].Value)))
                {
                    break;
                }

                grid.Rows.RemoveAt(i);
                changed = true;
            }

            foreach (String key in unpairedKeys)
            {
                for (int i = grid.Rows.Count - 1; i >= 0; i--)
                {
                    DataGridViewRow row = grid.Rows[i];
                    String upType = GetPairedKeyUpType(Convert.ToString(row.Cells[colType.Index].Value));
                    if (upType != null && Convert.ToString(row.Cells[colKey.Index].Value) == key)
                    {
                        DataGridViewRow upRow = grid.Rows[grid.Rows.Add()];
                        EventRowMapper.ApplyToRow(upRow, colType, colX, colY, colKey, colWait, new String[] { upType, "", "", key, "0" });
                        changed = true;
                        break;
                    }
                }
            }

            return changed;
        }

        // KeyDown/SysKeyDownの行の種類なら、対になるKeyUp/SysKeyUpの種類を返す。それ以外はnull
        private static String GetPairedKeyUpType(String downType)
        {
            if (downType == GlobalHook.KeyboardHook.Stroke.KEY_DOWN.ToString())
            {
                return GlobalHook.KeyboardHook.Stroke.KEY_UP.ToString();
            }
            if (downType == GlobalHook.KeyboardHook.Stroke.SYSKEY_DOWN.ToString())
            {
                return GlobalHook.KeyboardHook.Stroke.SYSKEY_UP.ToString();
            }
            return null;
        }

        // 直前のイベント(前回この関数を呼んだ時点)からの経過msを返し、基準時刻(_lastEventTick)を今に更新する。
        // 記録開始直後の1件目は「記録開始からの待機」になる
        private int TakeElapsedMsAndReset()
        {
            int now = Environment.TickCount;
            int wait = now - _lastEventTick;
            _lastEventTick = now;
            return wait < 0 ? 0 : wait;
        }

        // 記録した1件をバッファに貯めるだけ(フックのコールバックを描画待ちで塞がないため)。
        // 実際のDataGridViewへの反映は_gridFlushTimerがまとめて行う
        // waitが正の場合、待機だけを表す独立したWAIT_MS行を先に積んでから、実際のイベント行を積む
        // (待機時間はイベント自体の属性ではなく、シーケンス上の別行として表現する方式にしたため)
        private void QueueRow(String type, String x, String y, String key, int wait, String delta = "")
        {
            if (wait > 0)
            {
                _pendingRows.Add(new String[] { _waitEventType, "", "", "", wait.ToString(), "" });
            }

            _pendingRows.Add(new String[] { type, x, y, key, "0", delta });
        }

        // 貯まった行をまとめてDataGridViewに反映する。1件ずつ反映するより
        // 再描画・スクロール処理の回数を大幅に減らせる
        private void FlushPendingRows()
        {
            if (_pendingRows.Count == 0)
            {
                return;
            }

            dataGridView_Events.SuspendLayout();

            int lastIdx = -1;
            foreach (String[] r in _pendingRows)
            {
                lastIdx = dataGridView_Events.Rows.Add();
                DataGridViewRow row = dataGridView_Events.Rows[lastIdx];
                EventRowMapper.ApplyToRow(row, col_Type, col_X, col_Y, col_Key, col_Wait, col_Delta, r);
            }

            _pendingRows.Clear();

            dataGridView_Events.ResumeLayout();

            // 記録中も、今追加された最新行を薄い黄色でハイライト+自動スクロールする
            // (単発再生中のハイライトと同じ見た目・仕組みを流用)
            if (lastIdx >= 0)
            {
                HighlightEventRow(lastIdx);
            }
        }

        // 旧バージョンの保存形式(各行が自分自身の待機時間をWait列に持つ)を、
        // 新形式(待機を独立したWAIT_MS行として挿入する形式)に変換する。
        // 併せて、Event列の表記が旧版の"WAIT"のままの行があれば"WAIT_MS"に揃える。
        // すでに新形式(各行のWaitが0)の場合は何もしない、何度呼んでも安全な処理。
        // ファイル読込のたびにSaveRestore.csから呼ばれる
        internal void MigrateWaitColumnToRows()
        {
            dataGridView_Events.SuspendLayout();
            try
            {
                // 後ろから処理すれば、Insertしても未処理の行のインデックスに影響しない
                for (int i = dataGridView_Events.Rows.Count - 1; i >= 0; i--)
                {
                    DataGridViewRow row = dataGridView_Events.Rows[i];
                    String type = Convert.ToString(row.Cells[col_Type.Index].Value);

                    if (IsWaitEventType(type))
                    {
                        // 旧バージョンの表記("WAIT")が残っていたら、ここで新表記に揃えておく
                        if (type == _legacyWaitEventType)
                        {
                            row.Cells[col_Type.Index].Value = _waitEventType;
                        }
                        continue;
                    }

                    // 読めない値は「待機なし」とみなす(_util.GetInteger(int.Parse)だと数値以外で読み込みごと例外になっていた。WPF版と同じ)
                    int wait;
                    if (!int.TryParse(Convert.ToString(row.Cells[col_Wait.Index].Value), out wait) || wait <= 0)
                    {
                        continue;
                    }

                    dataGridView_Events.Rows.Insert(i, 1);
                    DataGridViewRow waitRow = dataGridView_Events.Rows[i];
                    waitRow.Cells[col_Type.Index].Value = _waitEventType;
                    waitRow.Cells[col_Wait.Index].Value = wait.ToString();

                    // 元の行のWaitは移し替えたので0にしておく(でないと再生時に二重に待ってしまう)
                    row.Cells[col_Wait.Index].Value = "0";
                }
            }
            finally
            {
                dataGridView_Events.ResumeLayout();
            }
        }
    }
}
