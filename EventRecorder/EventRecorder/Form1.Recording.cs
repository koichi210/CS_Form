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
        // 記録

        private void button_Record_Click(object sender, EventArgs e)
        {
            ToggleRecording();
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            if (isRecording || isPlaying)
            {
                return;
            }

            dataGridView_Events.Rows.Clear();
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
                // (表示例)プレイバック中：2 / 5：34/100
                // 「2 / 5」=全体ループ(プレイリストの全体周回。単発再生では常に1/1)の今の実行数/最大数、
                // 「34/100」=ループ(PlayRows呼び出し1回あたりの繰り返し)の今の実行回数/最大数
                this.Text = BaseTitle + " - プレイバック中："
                    + playbackOverallLoopNo + " / " + playbackOverallLoopMax + "："
                    + playbackInnerLoopNo + "/" + playbackInnerLoopMax;
            }
            else
            {
                this.Text = BaseTitle;
            }
        }

        // 単発再生とプレイリスト実行は1つのbutton_Play(表示名「再生」)に統合したので、
        // isPlayingが変わるたびにその表示を同期させるだけでよい
        private void UpdatePlayButtons()
        {
            button_Play.Text = isPlaying ? "停止" : "再生";
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

            // キーバインド設定画面が開いている間は、テスト入力したキーがホットキーとして
            // 誤発動しないよう、記録データへの取り込みも含めてここで丸ごと処理を止める
            if (isHotkeySettingsOpen)
            {
                return;
            }

            Boolean isDown = s.Stroke == GlobalHook.KeyboardHook.Stroke.KEY_DOWN || s.Stroke == GlobalHook.KeyboardHook.Stroke.SYSKEY_DOWN;
            Boolean isUp = s.Stroke == GlobalHook.KeyboardHook.Stroke.KEY_UP || s.Stroke == GlobalHook.KeyboardHook.Stroke.SYSKEY_UP;

            // ホットキーはアプリ操作用の予約キーなので、マクロの記録データには含めない。
            // s.Keyはフックから来る生のキー(修飾キーは含まない)なので、押した瞬間の
            // Ctrl/Alt/Shiftの状態を合わせて現在の組み合わせを作ってから、ユーザー設定の
            // ホットキー(hotkeyToggleRecord/hotkeyTogglePlay。これも同じ表現方法)と比較する
            if (isDown)
            {
                Keys heldModifiers = Control.ModifierKeys & (Keys.Control | Keys.Alt | Keys.Shift);
                Keys combo = s.Key | heldModifiers;
                Boolean isRecordHotkey = combo == hotkeyToggleRecord;
                Boolean isPlayHotkey = combo == hotkeyTogglePlay;

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
                        PlayOrStop();
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
        // waitが正の場合、待機だけを表す独立したWAIT_MS行を先に積んでから、実際のイベント行を積む
        // (待機時間はイベント自体の属性ではなく、シーケンス上の別行として表現する方式にしたため)
        private void QueueRow(String type, String x, String y, String key, int wait)
        {
            if (wait > 0)
            {
                pendingRows.Add(new String[] { WaitEventType, "", "", "", wait.ToString() });
            }

            pendingRows.Add(new String[] { type, x, y, key, "0" });
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
                EventRowMapper.ApplyToRow(row, col_Type, col_X, col_Y, col_Key, col_Wait, r);
            }

            pendingRows.Clear();

            dataGridView_Events.ResumeLayout();

            // 記録中も、今追加された最新行を薄い黄色でハイライト+自動スクロールする
            // (単発再生中のハイライトと同じ見た目・仕組みを流用)
            if (lastIdx >= 0)
            {
                HighlightPlayingRow(lastIdx);
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
                        if (type == LegacyWaitEventType)
                        {
                            row.Cells[col_Type.Index].Value = WaitEventType;
                        }
                        continue;
                    }

                    int wait = util.GetInteger(Convert.ToString(row.Cells[col_Wait.Index].Value));
                    if (wait <= 0)
                    {
                        continue;
                    }

                    dataGridView_Events.Rows.Insert(i, 1);
                    DataGridViewRow waitRow = dataGridView_Events.Rows[i];
                    waitRow.Cells[col_Type.Index].Value = WaitEventType;
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
