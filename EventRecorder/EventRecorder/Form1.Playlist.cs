using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private int _contextMenuPlaylistRowIndex = -1;

        // trueなら、実行チェック(col_PlaylistEnabled)がONの行だけをプレイリストに表示する
        private Boolean _showOnlyCheckedPlaylistRows;

        // UpdatePlaylistMissingFileHighlightsの非同期化用の世代カウンタ。
        // File.Existsチェック(バックグラウンド)が終わる前に別のプロファイルへ
        // 切り替えられた場合、古い結果をグリッドへ適用してしまわないようにするため
        private int _playlistHighlightGeneration;

        // プレイリストの設定ファイル列(col_PlaylistFile)を、comboBox_Profileと同じ内容に揃える。
        // ファイルシステムへの問い合わせはcomboBox_Profile側(UpdateProfileListAll)だけで行い、
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
            if (_isRecording || _isPlaying)
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
            // ファイル名の収集だけはUIスレッドで行う(ここはディスクI/Oが無いので一瞬で終わる)。
            // 同じファイル名が複数行にある場合に備えて重複は除いておく
            HashSet<String> fileNames = new HashSet<String>();
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                String fileName = Convert.ToString(row.Cells[col_PlaylistFile.Index].Value);
                if (!String.IsNullOrEmpty(fileName))
                {
                    fileNames.Add(fileName);
                }
            }

            // File.Existsはディスクアクセスを伴い、プレイリストの行数によっては待たされるため、
            // 判定自体はバックグラウンドスレッドで行い、グリッドへの反映(UIスレッド専用の操作)
            // だけ完了後にメインスレッドへ戻す。「今の設定値をまず表示し、ファイル存在チェックの
            // ような装飾的な処理は裏でこっそりやってほしい」というオグさんの要望に対応するため
            int generation = ++_playlistHighlightGeneration;
            String folder = _userDataFolder;

            Task.Run(() =>
            {
                Dictionary<String, Boolean> existsMap = new Dictionary<String, Boolean>();
                foreach (String fileName in fileNames)
                {
                    existsMap[fileName] = System.IO.File.Exists(System.IO.Path.Combine(folder, fileName));
                }
                return existsMap;
            }).ContinueWith(task =>
            {
                // 判定中に別のプロファイルへ切り替えられていたら、古い結果は捨てる
                if (generation != _playlistHighlightGeneration)
                {
                    return;
                }

                ApplyPlaylistMissingFileHighlights(task.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // UpdatePlaylistMissingFileHighlightsが裏で集めたファイル存在チェック結果を
        // グリッドへ反映する(こちらはUIスレッドの操作なので同期で行う)
        private void ApplyPlaylistMissingFileHighlights(Dictionary<String, Boolean> existsMap)
        {
            dataGridView_Playlist.SuspendLayout();
            try
            {
                Boolean changed = false;

                foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
                {
                    if (row.IsNewRow)
                    {
                        continue;
                    }

                    DataGridViewCell cell = row.Cells[col_PlaylistFile.Index];
                    String fileName = Convert.ToString(cell.Value);

                    Color newBackColor;
                    String newErrorText;

                    Boolean exists;
                    if (!String.IsNullOrEmpty(fileName) && existsMap.TryGetValue(fileName, out exists) && !exists)
                    {
                        newBackColor = Color.MistyRose;
                        newErrorText = "このファイル(" + fileName + ")は見つからないよ(削除された可能性があるよ)";
                    }
                    else
                    {
                        newBackColor = Color.Empty;
                        newErrorText = "";
                    }

                    // 前回と同じ内容なら、Styleの作り直し・代入自体を省略する
                    // (行数が多いとcell.Style=への代入コストが積み重なって描画が重くなるため)
                    if (cell.Style.BackColor == newBackColor && cell.ErrorText == newErrorText)
                    {
                        continue;
                    }

                    // cell.Style.BackColor = ... のように既存のStyleオブジェクトのプロパティだけを
                    // 書き換える形だと、DataGridViewComboBoxCellでは見た目に反映されないことがあったため、
                    // 新しいDataGridViewCellStyleを作ってStyleごと差し替える
                    DataGridViewCellStyle style = new DataGridViewCellStyle(cell.Style);
                    style.BackColor = newBackColor;
                    cell.ErrorText = newErrorText;
                    cell.Style = style;
                    changed = true;
                }

                if (changed)
                {
                    // cell.Style.XXXへの代入(既存のStyleオブジェクトのプロパティを書き換えるだけ)は
                    // 自動で再描画がかかるとは限らないため、明示的に再描画する
                    dataGridView_Playlist.Refresh();
                }
            }
            finally
            {
                dataGridView_Playlist.ResumeLayout();
            }
        }

        private void dataGridView_Playlist_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            _contextMenuPlaylistRowIndex = e.RowIndex;

            // 記録グリッドと同様、右クリックしたセル/行がすでに選択済みなら選択状態を維持する
            SelectRowOnRightClick(dataGridView_Playlist, e);
        }

        // *******************************************************************************
        // プレイリストの行のドラッグ&ドロップによる並び替え

        // 掴んだ(左ボタンを押した)行のインデックス。ドラッグ開始の起点にもする
        private int _dragStartPlaylistRowIndex = -1;
        private Point _dragStartPlaylistPoint;

        // ドラッグ中に「離すとここへ入る」位置へ線を引く行(-1なら引かない)と、その行の下側に引くかどうか
        private int _playlistDropLineRowIndex = -1;
        private Boolean _isPlaylistDropLineBelow;

        // ドラッグ中にグリッドの上端・下端へ近づけた時の自動スクロールの間隔(ms)。
        // DragOverはマウスを止めていても繰り返し来るので、間隔を空けないと一気に端まで流れてしまう
        private const int _playlistDragScrollIntervalMs = 80;
        private int _lastPlaylistDragScrollTick;

        private void dataGridView_Playlist_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _isRecording || _isPlaying)
            {
                return;
            }

            DataGridView.HitTestInfo hit = dataGridView_Playlist.HitTest(e.X, e.Y);
            _dragStartPlaylistRowIndex = hit.RowIndex;
            _dragStartPlaylistPoint = new Point(e.X, e.Y);
        }

        // マウスを一定距離動かして初めてドラッグとみなす(チェックボックスのクリック等が
        // 誤ってドラッグ扱いにならないようにするため)
        private void dataGridView_Playlist_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _dragStartPlaylistRowIndex < 0)
            {
                return;
            }

            if (Math.Abs(e.X - _dragStartPlaylistPoint.X) < SystemInformation.DragSize.Width
                && Math.Abs(e.Y - _dragStartPlaylistPoint.Y) < SystemInformation.DragSize.Height)
            {
                return;
            }

            int sourceIndex = _dragStartPlaylistRowIndex;
            _dragStartPlaylistRowIndex = -1;

            // 編集中のセル(ループ数の入力中など)は先に確定させる。確定できない値ならドラッグしない
            if (dataGridView_Playlist.IsCurrentCellInEditMode && !dataGridView_Playlist.EndEdit())
            {
                return;
            }

            try
            {
                dataGridView_Playlist.DoDragDrop(sourceIndex, DragDropEffects.Move);
            }
            finally
            {
                // Escでのキャンセル・グリッドの外でのドロップでも、位置を示す線は必ず消す
                SetPlaylistDropLine(-1, false);
            }
        }

        private void dataGridView_Playlist_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(int)))
            {
                e.Effect = DragDropEffects.None;
                SetPlaylistDropLine(-1, false);
                return;
            }

            e.Effect = DragDropEffects.Move;

            Point clientPoint = dataGridView_Playlist.PointToClient(new Point(e.X, e.Y));
            ScrollPlaylistWhileDragging(clientPoint.Y);

            int sourceIndex = (int)e.Data.GetData(typeof(int));
            int targetIndex = GetPlaylistDropTargetIndex(clientPoint);
            if (targetIndex < 0 || targetIndex == sourceIndex)
            {
                SetPlaylistDropLine(-1, false);
                return;
            }

            // 下へ動かす時はドロップ先の行の下に、上へ動かす時は上に入る(MoveGridRowの動き)ので、線もそちらに引く
            SetPlaylistDropLine(targetIndex, targetIndex > sourceIndex);
        }

        private void dataGridView_Playlist_DragLeave(object sender, EventArgs e)
        {
            SetPlaylistDropLine(-1, false);
        }

        private void dataGridView_Playlist_DragDrop(object sender, DragEventArgs e)
        {
            SetPlaylistDropLine(-1, false);

            if (!e.Data.GetDataPresent(typeof(int)))
            {
                return;
            }

            int sourceIndex = (int)e.Data.GetData(typeof(int));
            Point clientPoint = dataGridView_Playlist.PointToClient(new Point(e.X, e.Y));
            MovePlaylistRow(sourceIndex, GetPlaylistDropTargetIndex(clientPoint));
        }

        // ドラッグ中の位置(グリッドのクライアント座標)から、移動先の行を決める(移動できない位置なら-1)
        private int GetPlaylistDropTargetIndex(Point clientPoint)
        {
            DataGridView grid = dataGridView_Playlist;

            // 列の右側の余白の上で離しても、その高さの行へ入れられるよう、横位置は行ヘッダーの上で調べる
            int probeX = grid.RowHeadersVisible ? grid.RowHeadersWidth / 2 : clientPoint.X;
            DataGridView.HitTestInfo hit = grid.HitTest(probeX, clientPoint.Y);

            // 最後の行より下の空白で離したら末尾へ入れる(以前は何も起きなかった)。
            // 「チェックONのみ表示」中は、見えている最後の行の後ろに入れる
            int lastVisibleRowIndex = grid.Rows.GetLastRow(DataGridViewElementStates.Visible);
            Boolean isBelowLastRow = false;
            if (hit.RowIndex < 0 && lastVisibleRowIndex >= 0)
            {
                Rectangle lastRowBounds = grid.GetRowDisplayRectangle(lastVisibleRowIndex, false);
                isBelowLastRow = !lastRowBounds.IsEmpty && clientPoint.Y >= lastRowBounds.Bottom;
            }

            return ResolvePlaylistDropIndex(hit.RowIndex, isBelowLastRow, lastVisibleRowIndex);
        }

        // ドロップ先の行を決める。行の上ならその行、最後の行より下の空白なら見えている最後の行、
        // それ以外(列ヘッダー・スクロールバーの上など)は-1(移動しない)
        // (internal static: Form1を生成せずにテストできるようにするため)
        internal static int ResolvePlaylistDropIndex(int hitRowIndex, Boolean isBelowLastRow, int lastVisibleRowIndex)
        {
            if (hitRowIndex >= 0)
            {
                return hitRowIndex;
            }

            return isBelowLastRow ? lastVisibleRowIndex : -1;
        }

        // ドラッグ中、グリッドの上端・下端の1行分に入ったら1行ずつスクロールする(見えていない位置へも運べるように)
        private void ScrollPlaylistWhileDragging(int clientY)
        {
            DataGridView grid = dataGridView_Playlist;
            int firstIndex = grid.FirstDisplayedScrollingRowIndex;
            if (firstIndex < 0)
            {
                return;
            }

            int tick = Environment.TickCount;
            if (unchecked(tick - _lastPlaylistDragScrollTick) < _playlistDragScrollIntervalMs)
            {
                return;
            }

            int margin = grid.Rows[firstIndex].Height;
            int top = grid.ColumnHeadersVisible ? grid.ColumnHeadersHeight : 0;
            int nextFirstIndex = -1;

            if (clientY < top + margin)
            {
                nextFirstIndex = grid.Rows.GetPreviousRow(firstIndex, DataGridViewElementStates.Visible);
            }
            else if (clientY > grid.ClientSize.Height - margin)
            {
                // 最後の行が欠けずに見えていれば、もう下へはスクロールできない
                int lastVisibleRowIndex = grid.Rows.GetLastRow(DataGridViewElementStates.Visible);
                Boolean isLastRowFullyShown = lastVisibleRowIndex >= 0
                    && grid.GetRowDisplayRectangle(lastVisibleRowIndex, true).Height >= grid.Rows[lastVisibleRowIndex].Height;
                if (!isLastRowFullyShown)
                {
                    nextFirstIndex = grid.Rows.GetNextRow(firstIndex, DataGridViewElementStates.Visible);
                }
            }

            if (nextFirstIndex < 0)
            {
                return;
            }

            grid.FirstDisplayedScrollingRowIndex = nextFirstIndex;
            _lastPlaylistDragScrollTick = tick;
        }

        // 位置を示す線を引き直す(変わった時だけ再描画する。DragOverは何度も来るため)
        private void SetPlaylistDropLine(int rowIndex, Boolean isBelow)
        {
            if (_playlistDropLineRowIndex == rowIndex && _isPlaylistDropLineBelow == isBelow)
            {
                return;
            }

            _playlistDropLineRowIndex = rowIndex;
            _isPlaylistDropLineBelow = isBelow;
            dataGridView_Playlist.Invalidate();
        }

        private void dataGridView_Playlist_Paint(object sender, PaintEventArgs e)
        {
            if (_playlistDropLineRowIndex < 0 || _playlistDropLineRowIndex >= dataGridView_Playlist.Rows.Count)
            {
                return;
            }

            Rectangle bounds = dataGridView_Playlist.GetRowDisplayRectangle(_playlistDropLineRowIndex, false);
            if (bounds.IsEmpty)
            {
                return;
            }

            int y = _isPlaylistDropLineBelow ? bounds.Bottom - 1 : bounds.Top;
            using (Pen pen = new Pen(SystemColors.Highlight, 2))
            {
                e.Graphics.DrawLine(pen, bounds.Left, y, bounds.Right, y);
            }
        }

        // dataGridView_PlaylistのsourceIndex行をtargetIndexの位置へ移動する。並べ替えた順番は、
        // プロファイルを保存した時にそのまま保存される(BuildProfileFromGridsはグリッドの上から順に読む)
        private void MovePlaylistRow(int sourceIndex, int targetIndex)
        {
            if (dataGridView_Playlist.IsCurrentCellInEditMode && !dataGridView_Playlist.EndEdit())
            {
                return;
            }

            int currentColumnIndex = dataGridView_Playlist.CurrentCell != null ? dataGridView_Playlist.CurrentCell.ColumnIndex : -1;

            if (!MoveGridRow(dataGridView_Playlist, sourceIndex, targetIndex))
            {
                return;
            }

            // 動かした行を選択し直す(キー操作もその行から続けられるよう、カレントセルも移す)
            DataGridViewRow movedRow = dataGridView_Playlist.Rows[targetIndex];
            dataGridView_Playlist.ClearSelection();
            if (currentColumnIndex >= 0 && dataGridView_Playlist.Columns[currentColumnIndex].Visible)
            {
                dataGridView_Playlist.CurrentCell = movedRow.Cells[currentColumnIndex];
            }
            movedRow.Selected = true;
        }

        // gridのsourceIndex行をtargetIndexの位置へ移動する(ドロップ先の行の位置に割り込む。下へ動かした時はその行の下、
        // 上へ動かした時はその行の上に入る)。動かせない指定なら何もせずfalse。
        // 以前はセルの値を新しい行へコピーして作り直していたため、設定ファイル列を入れ直した時点で
        // 「ファイルを選んだらループ数を自動セット」が走ってファイルを読みに行き、入れ直した値がUndo履歴にも積まれて、
        // 並べ替え直後のCtrl+Zで動かした行のセルが空になっていた。行そのものを抜いて差し込み直すので、
        // 実行チェック・ループ数・ファイル不在のピンク表示もそのまま行と一緒に動き、値の変更イベントも起きない
        // (internal static: Form1を生成せずにテストできるようにするため)
        internal static Boolean MoveGridRow(DataGridView grid, int sourceIndex, int targetIndex)
        {
            int rowCount = grid.Rows.Count - (grid.AllowUserToAddRows ? 1 : 0);
            if (sourceIndex < 0 || sourceIndex >= rowCount
                || targetIndex < 0 || targetIndex >= rowCount
                || sourceIndex == targetIndex)
            {
                return false;
            }

            DataGridViewRow row = grid.Rows[sourceIndex];
            grid.Rows.RemoveAt(sourceIndex);
            grid.Rows.Insert(targetIndex, row);

            // DataGridViewExは行の削除・途中への挿入でUndo履歴を消すが、末尾へ差し込んだ時は差し込んだ行の値を
            // 空として覚えてしまう(次にその行を編集してからUndoすると空に戻ってしまう)ので、今の値で覚え直させる
            DataGridViewEx gridEx = grid as DataGridViewEx;
            if (gridEx != null)
            {
                gridEx.ClearUndoHistory();
            }

            return true;
        }

        private void contextMenuStrip_Playlist_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isRecording || _isPlaying)
            {
                e.Cancel = true;
                return;
            }

            menuItem_PlaylistDeleteRow.Enabled = _contextMenuPlaylistRowIndex >= 0 && _contextMenuPlaylistRowIndex < dataGridView_Playlist.Rows.Count;
            // 右クリックした行にファイルが指定されていて、実在する時だけ開けるようにする
            menuItem_PlaylistOpenFile.Enabled = System.IO.File.Exists(GetContextMenuPlaylistFilePath() ?? "");

            // 今どちらの表示モードか一目で分かるように、選択中の方にチェックを付ける
            menuItem_PlaylistShowCheckedOnly.Checked = _showOnlyCheckedPlaylistRows;
            menuItem_PlaylistShowAll.Checked = !_showOnlyCheckedPlaylistRows;
        }

        // 右クリックした行の設定ファイルのフルパスを返す(行が範囲外、またはファイル未指定ならnull)
        private String GetContextMenuPlaylistFilePath()
        {
            if (_contextMenuPlaylistRowIndex < 0 || _contextMenuPlaylistRowIndex >= dataGridView_Playlist.Rows.Count)
            {
                return null;
            }

            String fileName = Convert.ToString(dataGridView_Playlist.Rows[_contextMenuPlaylistRowIndex].Cells[col_PlaylistFile.Index].Value);
            if (String.IsNullOrEmpty(fileName))
            {
                return null;
            }

            return System.IO.Path.Combine(_userDataFolder, fileName);
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
            _showOnlyCheckedPlaylistRows = true;
            ApplyPlaylistRowFilter();
        }

        private void menuItem_PlaylistShowAll_Click(object sender, EventArgs e)
        {
            _showOnlyCheckedPlaylistRows = false;
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

                UpdatePlaylistRowVisibility(row);
            }
        }

        // 実行列(col_PlaylistEnabled)にチェックが入っている行か
        private Boolean IsPlaylistRowEnabled(DataGridViewRow row)
        {
            return Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false);
        }

        // 表示フィルタ(_showOnlyCheckedPlaylistRows)とその行のチェック状態から、行の表示/非表示を決める
        private void UpdatePlaylistRowVisibility(DataGridViewRow row)
        {
            row.Visible = !_showOnlyCheckedPlaylistRows || IsPlaylistRowEnabled(row);
        }

        private void menuItem_PlaylistAddRow_Click(object sender, EventArgs e)
        {
            // 全行削除直後など、行が無い状態で右クリックするとCellMouseDownが発火せず
            // _contextMenuPlaylistRowIndexが古い(削除済みの)行番号のまま残ることがあるため、範囲チェックする
            Boolean isContextMenuRowIndexValid = _contextMenuPlaylistRowIndex >= 0 && _contextMenuPlaylistRowIndex < dataGridView_Playlist.Rows.Count;
            int insertAt = isContextMenuRowIndexValid ? _contextMenuPlaylistRowIndex + 1 : dataGridView_Playlist.Rows.Count;
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
            newRow.Visible = !_showOnlyCheckedPlaylistRows || isEnabled;
        }

        private void menuItem_PlaylistDeleteRow_Click(object sender, EventArgs e)
        {
            List<int> targetIndexes = GetSelectedOrContextMenuRowIndexes(dataGridView_Playlist, _contextMenuPlaylistRowIndex);

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
                UpdatePlaylistRowVisibility(dataGridView_Playlist.Rows[e.RowIndex]);
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

            String fullPath = System.IO.Path.Combine(_userDataFolder, fileName);
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
            public String FileName { get; set; }
            public int LoopCount { get; set; }

            // dataGridView_Playlist上の元の行インデックス。実行中にその行をハイライトするために使う
            // (チェックが外れている行等は除外されるため、entries内のインデックスとは一致しない)
            public int RowIndex { get; set; }
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

                if (!IsPlaylistRowEnabled(row))
                {
                    continue;
                }

                String fileName = Convert.ToString(row.Cells[col_PlaylistFile.Index].Value);
                if (String.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                int loopCount = ParseLoopCount(Convert.ToString(row.Cells[col_PlaylistLoopCount.Index].Value));

                entries.Add(new PlaylistEntry() { FileName = fileName, LoopCount = loopCount, RowIndex = row.Index });
            }

            return entries;
        }

        // プレイリストグループの内容を実行する(単発再生とbutton_Playを共有しているため、
        // 呼び出し元はPlayOrStop。_isPlaying/_isRecordingのチェックは呼び出し元で済んでいる)
        private void StartPlaylistRun()
        {
            List<PlaylistEntry> entries = GetPlaylistEntries();
            if (entries.Count == 0)
            {
                return;
            }

            // 「全体ループ」も単発再生の「ループ回数」とtextBox_Loopを共有している
            int overallLoopCount = ParseLoopCount(textBox_Loop.Text);

            // 各ファイルを読み込むたびにtextBox_Loopがそのファイルの値で上書きされるため、再生後に元の値へ戻す
            String loopTextBeforeRun = textBox_Loop.Text;

            _playbackOverallLoopNo = 0;
            _playbackOverallLoopMax = overallLoopCount;
            _playbackInnerLoopNo = 0;
            _playbackInnerLoopMax = 0;

            StartPlaybackTask(() => PlaylistPlayLoop(entries, overallLoopCount, loopTextBeforeRun));
        }

        // プレイリスト全体をoverallLoopCount回繰り返す。各周回の中で、
        // リストの各行を上から順に読み込み→その行のループ回数分だけ再生、を繰り返す。
        // 終わったら(停止・エラーでも)、全体ループ欄(textBox_Loop)を再生前の値(loopTextBeforeRun)に戻す
        private void PlaylistPlayLoop(List<PlaylistEntry> entries, int overallLoopCount, String loopTextBeforeRun)
        {
            Boolean isCompleted = false;
            try
            {
                for (int loopNo = 0; loopNo < overallLoopCount && !_stopPlayRequested; loopNo++)
                {
                    int loopDisplayNo = loopNo + 1;
                    _playbackOverallLoopNo = loopDisplayNo;

                    for (int i = 0; i < entries.Count && !_stopPlayRequested; i++)
                    {
                        PlaylistEntry entry = entries[i];
                        int fileNo = i + 1;
                        List<String[]> rows = null;

                        InvokeOnUi(() =>
                        {
                            label_PlaylistStatus.Text = "実行中(全体" + loopDisplayNo + "/" + overallLoopCount + "): "
                                + entry.FileName + " (" + fileNo + "/" + entries.Count + ")";

                            // 今どのファイル(行)を再生しているか、プレイリスト側もハイライトする
                            HighlightPlaylistRow(entry.RowIndex);

                            String fullPath = System.IO.Path.Combine(_userDataFolder, entry.FileName);
                            // 再生対象の記録データ(タブ1)だけ差し替える。プレイリスト自体(タブ2)には
                            // 影響しない(XML/JSONどちらの形式でも、記録データのみを反映する)
                            LoadProfileForPlayback(fullPath);

                            // 読み込んだファイルに再生できない行が無いか確認する。あればプレイリスト
                            // 全体を停止する(rowsはnullのまま=このエントリは再生しない)
                            if (!ValidateEventsForPlayback())
                            {
                                _stopPlayRequested = true;
                                return;
                            }

                            rows = SnapshotRows();
                        });

                        if (rows != null && rows.Count > 0)
                        {
                            PlayRows(rows, entry.LoopCount);
                        }
                    }
                }

                isCompleted = true;
                Cursor.Position = _cursorPositionBeforePlay;
            }
            catch (Exception ex)
            {
                ReportPlaybackError(ex);
            }
            finally
            {
                // 中断された場合・例外で止まった場合は、Downのままのキー/マウスボタンを全部Upに戻す。
                // (以前はPlayRowsの中でだけ戻していたため、例外で止まった時や、ファイルの切り替わり目
                // (PlayRowsの外)で停止した時に、前のファイルで押したキーが押しっぱなしで残っていた)
                if (_stopPlayRequested || !isCompleted)
                {
                    ReleaseAllPressedInputs();
                }

                // 途中で例外が起きても、必ず「再生中」状態を解除する。
                // ここを素通りしてしまうと_isPlayingがtrueのまま固まり、記録も再生も
                // 二度とできなくなる(アプリが固まって見える不具合の原因になっていた)
                _isPlaying = false;
                _stopPlayRequested = false;
                InvokeOnUi(() =>
                {
                    UpdatePlayButton();
                    label_PlaylistStatus.Text = "";
                    textBox_Loop.Text = loopTextBeforeRun;
                    UpdateTitle();
                    HighlightEventRow(-1);
                    HighlightPlaylistRow(-1);
                    RestoreIfMinimizedByPlay();
                });
            }
        }
    }
}
