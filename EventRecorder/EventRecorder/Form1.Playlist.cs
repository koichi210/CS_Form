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
        // プレイリスト(タブ2): 複数の設定ファイルを指定した順番で連続再生する

        // 右クリックしたセルの行(プレイリスト側)。右クリック無しの状態(-1)なら末尾扱い
        private int contextMenuPlaylistRowIndex = -1;

        // trueなら、実行チェック(col_PlaylistEnabled)がONの行だけをプレイリストに表示する
        private Boolean showOnlyCheckedPlaylistRows = false;

        // プレイリストの設定ファイル列(col_PlaylistFile)を、comboBox_Profileと同じ内容に揃える。
        // ファイルシステムへの問い合わせはcomboBox_Profile側(util.UpdateProfileList)だけで行い、
        // その結果をそのままコピーするだけにすることで、同じ一覧を二重に取得しないようにしている。
        // comboBox_Profileが更新されるタイミング(起動時・設定値保存時)で必ずこれも呼ぶこと
        private void SyncPlaylistFileItems()
        {
            col_PlaylistFile.Items.Clear();
            foreach (Object item in comboBox_Profile.Items)
            {
                col_PlaylistFile.Items.Add(item);
            }

            // 既にプレイリストの行が参照しているファイル名は、たとえ実体が削除されて
            // comboBox_Profile.Itemsから消えていても、col_PlaylistFile.Itemsには残しておく。
            // DataGridViewComboBoxCellはValueがItemsに無いと表示・書式設定でエラーになり、
            // dataGridView_Playlist_CellFormattingで意図したピンク表示ができなくなるため
            if (dataGridView_Playlist != null)
            {
                foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
                {
                    if (row.IsNewRow)
                    {
                        continue;
                    }

                    String fileName = Convert.ToString(row.Cells[col_PlaylistFile.Index].Value);
                    if (!String.IsNullOrEmpty(fileName) && !col_PlaylistFile.Items.Contains(fileName))
                    {
                        col_PlaylistFile.Items.Add(fileName);
                    }
                }
            }
        }

        // 右クリックメニュー「プレイリストを更新」から呼ばれる(専用ボタンは廃止した)。
        // 既存の行の並び・設定(実行チェック・ループ数)はそのまま残し、増えたファイルだけ
        // 末尾に追加する。消えたファイル(プレイリストにあるがプルダウンにはもう無いファイル)は
        // 削除せず残す(行ごとの設定を失わないため)。見た目の対応はdataGridView_Playlist_
        // CellFormattingが担当し、該当セルをピンクでハイライトする
        private void menuItem_PlaylistRefresh_Click(object sender, EventArgs e)
        {
            if (isRecording || isPlaying)
            {
                return;
            }

            HashSet<String> existingFiles = new HashSet<String>();
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                String fileName = Convert.ToString(row.Cells[col_PlaylistFile.Index].Value);
                if (!String.IsNullOrEmpty(fileName))
                {
                    existingFiles.Add(fileName);
                }
            }

            foreach (Object item in col_PlaylistFile.Items)
            {
                String fileName = Convert.ToString(item);
                if (existingFiles.Contains(fileName))
                {
                    continue;
                }

                int insertAt = dataGridView_Playlist.Rows.Count;
                AddPlaylistRow(insertAt, isEnabled: true);
                dataGridView_Playlist.Rows[insertAt].Cells[col_PlaylistFile.Index].Value = fileName;
            }

            UpdatePlaylistMissingFileHighlights();
        }

        // col_PlaylistFileが指しているファイルが(削除等で)もう存在しない行を、
        // レコード側の不正値ハイライトと同じ色(MistyRose)で知らせる。行自体は消さない
        // (実行チェック・ループ数等の設定を保持したまま、ファイルだけ後で選び直せるように)。
        // CellFormattingイベント任せだと、DataGridViewComboBoxCellの内部フォーマット処理と
        // 絡んでBackColorが反映されないことがあったため、HighlightRowと同様に
        // 各セルのStyle.BackColorを直接設定する方式にしている
        private void UpdatePlaylistMissingFileHighlights()
        {
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                DataGridViewCell cell = row.Cells[col_PlaylistFile.Index];
                String fileName = Convert.ToString(cell.Value);

                // cell.Style.BackColor = ... のように既存のStyleオブジェクトのプロパティだけを
                // 書き換える形だと、DataGridViewComboBoxCellでは見た目に反映されないことがあったため、
                // 新しいDataGridViewCellStyleを作ってStyleごと差し替える
                DataGridViewCellStyle style = new DataGridViewCellStyle(cell.Style);

                if (!String.IsNullOrEmpty(fileName)
                    && !System.IO.File.Exists(System.IO.Path.Combine(userDataFolder, fileName)))
                {
                    style.BackColor = Color.MistyRose;
                    cell.ErrorText = "このファイル(" + fileName + ")は見つからないよ(削除された可能性があるよ)";
                }
                else
                {
                    style.BackColor = Color.Empty;
                    cell.ErrorText = "";
                }

                cell.Style = style;
            }

            // cell.Style.XXXへの代入(既存のStyleオブジェクトのプロパティを書き換えるだけ)は
            // 自動で再描画がかかるとは限らないため、明示的に再描画する
            dataGridView_Playlist.Refresh();
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

        // *******************************************************************************
        // プレイリストの行のドラッグ&ドロップによる並び替え

        // 掴んだ(左ボタンを押した)行のインデックス。ドラッグ開始の起点にもする
        private int dragStartPlaylistRowIndex = -1;
        private Point dragStartPlaylistPoint;

        private void dataGridView_Playlist_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || isRecording || isPlaying)
            {
                return;
            }

            DataGridView.HitTestInfo hit = dataGridView_Playlist.HitTest(e.X, e.Y);
            dragStartPlaylistRowIndex = hit.RowIndex;
            dragStartPlaylistPoint = new Point(e.X, e.Y);
        }

        // マウスを一定距離動かして初めてドラッグとみなす(チェックボックスのクリック等が
        // 誤ってドラッグ扱いにならないようにするため)
        private void dataGridView_Playlist_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || dragStartPlaylistRowIndex < 0)
            {
                return;
            }

            if (Math.Abs(e.X - dragStartPlaylistPoint.X) < SystemInformation.DragSize.Width
                && Math.Abs(e.Y - dragStartPlaylistPoint.Y) < SystemInformation.DragSize.Height)
            {
                return;
            }

            dataGridView_Playlist.DoDragDrop(dragStartPlaylistRowIndex, DragDropEffects.Move);
            dragStartPlaylistRowIndex = -1;
        }

        private void dataGridView_Playlist_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(int)) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void dataGridView_Playlist_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(int)))
            {
                return;
            }

            int sourceIndex = (int)e.Data.GetData(typeof(int));

            Point clientPoint = dataGridView_Playlist.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo hit = dataGridView_Playlist.HitTest(clientPoint.X, clientPoint.Y);
            int targetIndex = hit.RowIndex;

            MovePlaylistRow(sourceIndex, targetIndex);
        }

        // dataGridView_PlaylistのsourceIndex行をtargetIndexの位置へ移動する。
        // DataGridViewは行そのものを並べ替える機能を持たないため、セルの値を丸ごとコピーして
        // 元の行を消し、新しい位置に作り直す形で実現する
        private void MovePlaylistRow(int sourceIndex, int targetIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= dataGridView_Playlist.Rows.Count
                || targetIndex < 0 || targetIndex >= dataGridView_Playlist.Rows.Count
                || sourceIndex == targetIndex)
            {
                return;
            }

            DataGridViewRow sourceRow = dataGridView_Playlist.Rows[sourceIndex];
            Object[] values = new Object[dataGridView_Playlist.Columns.Count];
            for (int c = 0; c < values.Length; c++)
            {
                values[c] = sourceRow.Cells[c].Value;
            }

            dataGridView_Playlist.Rows.RemoveAt(sourceIndex);

            // Insert(index)は「削除後の並びのindex番目の手前に挿入する」動きなので、
            // targetIndexをそのまま使えばドロップ先の行の位置に割り込む形になる
            // (下方向への移動時は元の配列基準の値をそのまま使うのが正しく、
            // 1引く補正を入れると1行分行き過ぎてしまっていた)
            int insertAt = targetIndex;

            dataGridView_Playlist.Rows.Insert(insertAt, 1);
            DataGridViewRow newRow = dataGridView_Playlist.Rows[insertAt];
            for (int c = 0; c < values.Length; c++)
            {
                newRow.Cells[c].Value = values[c];
            }

            dataGridView_Playlist.ClearSelection();
            newRow.Selected = true;

            // 行を作り直しているため、移動前のセルに付いていたStyle(ファイル不在の
            // ピンク表示等)は引き継がれない。作り直した行に対して計算し直す
            UpdatePlaylistMissingFileHighlights();
        }

        private void contextMenuStrip_Playlist_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isRecording || isPlaying)
            {
                e.Cancel = true;
                return;
            }

            menuItem_PlaylistDeleteRow.Enabled = contextMenuPlaylistRowIndex >= 0 && contextMenuPlaylistRowIndex < dataGridView_Playlist.Rows.Count;
            // 右クリックした行にファイルが指定されていて、実在する時だけ開けるようにする
            menuItem_PlaylistOpenFile.Enabled = System.IO.File.Exists(GetContextMenuPlaylistFilePath() ?? "");

            // 今どちらの表示モードか一目で分かるように、選択中の方にチェックを付ける
            menuItem_PlaylistShowCheckedOnly.Checked = showOnlyCheckedPlaylistRows;
            menuItem_PlaylistShowAll.Checked = !showOnlyCheckedPlaylistRows;
        }

        // 右クリックした行の設定ファイルのフルパスを返す(行が範囲外、またはファイル未指定ならnull)
        private String GetContextMenuPlaylistFilePath()
        {
            if (contextMenuPlaylistRowIndex < 0 || contextMenuPlaylistRowIndex >= dataGridView_Playlist.Rows.Count)
            {
                return null;
            }

            String fileName = Convert.ToString(dataGridView_Playlist.Rows[contextMenuPlaylistRowIndex].Cells[col_PlaylistFile.Index].Value);
            if (String.IsNullOrEmpty(fileName))
            {
                return null;
            }

            return System.IO.Path.Combine(userDataFolder, fileName);
        }

        // 右クリックした行の設定ファイルを、Windowsの関連付けアプリ(メモ帳など)で開く。
        // EventRecorder側のグリッドや設定ファイルの選択状態には一切影響しない
        private void menuItem_PlaylistOpenFile_Click(object sender, EventArgs e)
        {
            String filePath = GetContextMenuPlaylistFilePath();
            if (!System.IO.File.Exists(filePath ?? ""))
            {
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                // 拡張子に関連付けアプリが無い場合など
                MessageBox.Show(ex.Message, "ファイルを開けませんでした", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void menuItem_PlaylistShowCheckedOnly_Click(object sender, EventArgs e)
        {
            showOnlyCheckedPlaylistRows = true;
            ApplyPlaylistRowFilter();
        }

        private void menuItem_PlaylistShowAll_Click(object sender, EventArgs e)
        {
            showOnlyCheckedPlaylistRows = false;
            ApplyPlaylistRowFilter();
        }

        // 実行列(col_PlaylistEnabled)のチェック状態を見て、プレイリストの行の表示/非表示を切り替える
        private void ApplyPlaylistRowFilter()
        {
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                Boolean isEnabled = Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false);
                row.Visible = !showOnlyCheckedPlaylistRows || isEnabled;
            }
        }

        private void menuItem_PlaylistAddRow_Click(object sender, EventArgs e)
        {
            // 全行削除直後など、行が無い状態で右クリックするとCellMouseDownが発火せず
            // contextMenuPlaylistRowIndexが古い(削除済みの)行番号のまま残ることがあるため、範囲チェックする
            Boolean isContextMenuRowIndexValid = contextMenuPlaylistRowIndex >= 0 && contextMenuPlaylistRowIndex < dataGridView_Playlist.Rows.Count;
            int insertAt = isContextMenuRowIndexValid ? contextMenuPlaylistRowIndex + 1 : dataGridView_Playlist.Rows.Count;
            // 右クリックで能動的に追加した行は、すぐ使うつもりのはずなので実行チェックはONにしておく
            AddPlaylistRow(insertAt, isEnabled: true);
        }

        // プレイリストに1行追加する(ループ回数=1がデフォルト。実行チェックの初期値はisEnabledで指定)。
        // 右クリックメニューの「行の追加」と、起動時の初期空行の両方から使う
        private void AddPlaylistRow(int insertAt, Boolean isEnabled)
        {
            // 呼び出し元の計算ミスで範囲外indexが渡ってきても落ちないよう防御的にクランプする
            insertAt = Math.Max(0, Math.Min(insertAt, dataGridView_Playlist.Rows.Count));
            dataGridView_Playlist.Rows.Insert(insertAt, 1);

            DataGridViewRow newRow = dataGridView_Playlist.Rows[insertAt];
            newRow.Cells[col_PlaylistEnabled.Index].Value = isEnabled;
            newRow.Cells[col_PlaylistLoopCount.Index].Value = "1";
            newRow.Visible = !showOnlyCheckedPlaylistRows || isEnabled;
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

            ApplyPlaylistRowFilter();
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
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == col_PlaylistEnabled.Index)
            {
                // チェックON/OFFが変わったら、表示フィルタが有効な時はその場で表示/非表示を切り替える
                DataGridViewRow row = dataGridView_Playlist.Rows[e.RowIndex];
                Boolean isEnabled = Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false);
                row.Visible = !showOnlyCheckedPlaylistRows || isEnabled;
                return;
            }

            if (e.ColumnIndex != col_PlaylistFile.Index)
            {
                return;
            }

            // 選び直した/クリアしたファイルが存在するかどうかで、ピンク表示を更新する
            UpdatePlaylistMissingFileHighlights();

            String fileName = Convert.ToString(dataGridView_Playlist.Rows[e.RowIndex].Cells[col_PlaylistFile.Index].Value);
            if (String.IsNullOrEmpty(fileName))
            {
                return;
            }

            String fullPath = System.IO.Path.Combine(userDataFolder, fileName);
            String savedLoopCount = ReadSavedLoopCount(fullPath);
            if (savedLoopCount != null)
            {
                dataGridView_Playlist.Rows[e.RowIndex].Cells[col_PlaylistLoopCount.Index].Value = savedLoopCount;
            }
        }

        // マクロファイル自身が保存しているループ回数(textBox_Loopの値)だけを、
        // dataGridView_Events等には一切触れずに読み取る
        // (LoadProfileを使うとタブ1の記録データがまるごとクリア/差し替えられてしまうため使わない)
        private String ReadSavedLoopCount(String filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            if (IsJsonFile(filePath))
            {
                try
                {
                    EventRecorderProfile profile = JsonFileStorage.Load<EventRecorderProfile>(filePath);
                    return profile?.LoopCount;
                }
                catch (Exception)
                {
                    return null;
                }
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

            // dataGridView_Playlist上の元の行インデックス。実行中にその行をハイライトするために使う
            // (チェックが外れている行等は除外されるため、entries内のインデックスとは一致しない)
            public int RowIndex;
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

                entries.Add(new PlaylistEntry() { FileName = fileName, LoopCount = loopCount, RowIndex = row.Index });
            }

            return entries;
        }

        // プレイリストグループの内容を実行する(単発再生とbutton_Playを共有しているため、
        // 呼び出し元はPlayOrStop。isPlaying/isRecordingのチェックは呼び出し元で済んでいる)
        private void StartPlaylistRun()
        {
            List<PlaylistEntry> entries = GetPlaylistEntries();
            if (entries.Count == 0)
            {
                return;
            }

            // 「全体ループ」も単発再生の「ループ回数」とtextBox_Loopを共有している
            int overallLoopCount = util.GetInteger(textBox_Loop.Text);
            if (overallLoopCount <= 0)
            {
                overallLoopCount = 1;
            }

            cursorPositionBeforePlay = Cursor.Position;

            playbackOverallLoopNo = 0;
            playbackOverallLoopMax = overallLoopCount;
            playbackInnerLoopNo = 0;
            playbackInnerLoopMax = 0;

            isPlaying = true;
            stopPlayRequested = false;
            UpdatePlayButtons();
            UpdateTitle();
            MinimizeIfRequested();

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
                    playbackOverallLoopNo = loopDisplayNo;

                    for (int i = 0; i < entries.Count && !stopPlayRequested; i++)
                    {
                        PlaylistEntry entry = entries[i];
                        int fileNo = i + 1;
                        List<String[]> rows = null;

                        this.Invoke((MethodInvoker)(() =>
                        {
                            label_PlaylistStatus.Text = "実行中(全体" + loopDisplayNo + "/" + overallLoopCount + "): "
                                + entry.FileName + " (" + fileNo + "/" + entries.Count + ")";

                            // 今どのファイル(行)を再生しているか、プレイリスト側もハイライトする
                            HighlightPlaylistRow(entry.RowIndex);

                            String fullPath = System.IO.Path.Combine(userDataFolder, entry.FileName);
                            // 再生対象の記録データ(タブ1)だけ差し替える。プレイリスト自体(タブ2)には
                            // 影響しない(XML/JSONどちらの形式でも、記録データのみを反映する)
                            LoadProfileForPlayback(fullPath);

                            // 読み込んだファイルに再生できない行が無いか確認する。あればプレイリスト
                            // 全体を停止する(rowsはnullのまま=このエントリは再生しない)
                            if (!ValidateEventsForPlayback())
                            {
                                stopPlayRequested = true;
                                return;
                            }

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
                    label_PlaylistStatus.Text = "";
                    UpdateTitle();
                    HighlightPlayingRow(-1);
                    HighlightPlaylistRow(-1);
                    RestoreIfMinimizedByPlay();
                }));
            }
        }
    }
}
