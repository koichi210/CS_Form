using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EventRecorder
{
    // Ctrl+F/Ctrl+Hで開くダイアログのモード。同じダイアログ内のラジオボタンでも切り替えられる
    internal enum FindReplaceMode
    {
        Find,
        Replace,
    }

    // Ctrl+F(検索)/Ctrl+H(置換)共通のダイアログ。
    // 検索モードは、レコード表(dataGridView_Events)と同じ列構成(行番号+可視列)でヒットした
    // セルを一覧表示し、ヒットしたセル自体は薄い黄色でハイライトする。
    // 置換モードは、次を検索/置換/すべて置換の従来型ダイアログとして動く。
    //
    // レイアウトは上部(topPanel)/中央(_resultsGrid)/下部(bottomPanel)のDockで組んでいる。
    // 絶対座標のLocation指定だけだと、検索モードでボタンが結果一覧と重なったりダイアログから
    // はみ出たりする不具合が起きたため、Dockベースにして常に3つの帯に収まるようにしてある
    internal class FindReplaceForm : DialogFormBase
    {
        private readonly StandardTemplate.DataGridViewEx _targetGrid;

        private readonly RadioButton _radioFind;
        private readonly RadioButton _radioReplace;
        private readonly Label _lblFind;
        private readonly TextBox _txtFind;
        private readonly Label _lblReplace;
        private readonly TextBox _txtReplace;
        private readonly CheckBox _chkMatchCase;
        private readonly Button _btnSearch;
        private readonly Button _btnFindNext;
        private readonly Button _btnReplace;
        private readonly Button _btnReplaceAll;
        private readonly DataGridView _resultsGrid;
        private readonly Label _lblStatus;

        // 検索モード・置換モードとも幅は共通にして、topPanel内のボタン位置(X=320)を使い回せるようにする。
        // 高さだけ、検索モードは結果一覧の分だけ余計に確保する
        private const int _panelWidth = 460;
        private static readonly Size _findModeSize = new Size(_panelWidth, 420);
        private static readonly Size _replaceModeSize = new Size(_panelWidth, 170);

        // 検索結果一覧の1行が、レコード表(_targetGrid)のどの行/どの可視列でヒットしたかを覚えておく
        private class HitRowInfo
        {
            public int TargetRowIndex { get; set; }
            public List<int> HitVisibleColumnIndexes { get; set; }
        }

        // 「次を検索」で最後に見つけたセル位置。次回はこの続き(次のセル)から探す
        private int _lastFoundRow = -1;
        private int _lastFoundCol = -1;

        public FindReplaceForm(StandardTemplate.DataGridViewEx grid, FindReplaceMode initialMode, String initialSearchText)
        {
            _targetGrid = grid;

            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(360, 200);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;

            // ***** 上段: ラジオボタン+検索/置換の入力欄+モード別ボタン *****
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 120 };

            _radioFind = new RadioButton { Text = "検索", Location = new Point(10, 8), AutoSize = true };
            _radioReplace = new RadioButton { Text = "置換", Location = new Point(80, 8), AutoSize = true };

            _lblFind = new Label { Text = "検索文字列:", Location = new Point(10, 38), AutoSize = true };
            _txtFind = new TextBox { Location = new Point(90, 34), Width = 220, Text = initialSearchText ?? "" };

            _lblReplace = new Label { Text = "置換後の文字列:", Location = new Point(10, 66), AutoSize = true };
            _txtReplace = new TextBox { Location = new Point(110, 62), Width = 200 };

            _chkMatchCase = new CheckBox { Text = "大文字/小文字を区別する", Location = new Point(10, 94), AutoSize = true };

            // 検索モード用/置換モード用のボタンは、それぞれの行の右端(X=320)に置き、ApplyModeでVisibleを切り替える
            _btnSearch = new Button { Text = "検索(&S)", Location = new Point(320, 32), Width = 90 };
            _btnFindNext = new Button { Text = "次を検索(&N)", Location = new Point(320, 32), Width = 90 };
            _btnReplace = new Button { Text = "置換(&R)", Location = new Point(320, 60), Width = 90 };
            _btnReplaceAll = new Button { Text = "すべて置換(&A)", Location = new Point(320, 88), Width = 90 };

            topPanel.Controls.Add(_radioFind);
            topPanel.Controls.Add(_radioReplace);
            topPanel.Controls.Add(_lblFind);
            topPanel.Controls.Add(_txtFind);
            topPanel.Controls.Add(_lblReplace);
            topPanel.Controls.Add(_txtReplace);
            topPanel.Controls.Add(_chkMatchCase);
            topPanel.Controls.Add(_btnSearch);
            topPanel.Controls.Add(_btnFindNext);
            topPanel.Controls.Add(_btnReplace);
            topPanel.Controls.Add(_btnReplaceAll);

            // ***** 中段: 検索結果一覧(検索モードのみ表示、Dock=Fillで残り全体を使う) *****
            _resultsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            };
            _resultsGrid.CellDoubleClick += ResultsGrid_CellDoubleClick;

            // ***** 下段: ステータス表示(結果一覧と重ならない専用の帯)。
            // 閉じるボタンは置かない。Escキー(DialogFormBase)で閉じられるので不要 *****
            Panel bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };

            _lblStatus = new Label { Location = new Point(10, 10), AutoSize = true };

            bottomPanel.Controls.Add(_lblStatus);

            // Dock=Fillのコントロールは、他のDock(Top/Bottom等)より後にControls.Addしないと、
            // 先に全クライアント領域を占有してしまい、後から追加した帯(bottomPanelの閉じるボタン等)が
            // その下に隠れてしまう。そのため、上下の帯を先に追加し、_resultsGrid(Fill)を最後に追加する
            this.Controls.Add(topPanel);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(_resultsGrid);

            _radioFind.CheckedChanged += (s, e) => { if (_radioFind.Checked) ApplyMode(); };
            _radioReplace.CheckedChanged += (s, e) => { if (_radioReplace.Checked) ApplyMode(); };
            _btnSearch.Click += (s, e) => DoSearch();
            _btnFindNext.Click += (s, e) => FindNext();
            _btnReplace.Click += (s, e) => ReplaceCurrent();
            _btnReplaceAll.Click += (s, e) => ReplaceAll();

            _txtFind.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter)
                {
                    return;
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
                if (_radioFind.Checked)
                {
                    DoSearch();
                }
                else
                {
                    FindNext();
                }
            };

            SetMode(initialMode);

            if (!String.IsNullOrEmpty(initialSearchText) && initialMode == FindReplaceMode.Find)
            {
                DoSearch();
            }
        }

        // Ctrl+F/Ctrl+Hが押されるたびに、開き直さずこのメソッドでモードだけ切り替える
        internal void SetMode(FindReplaceMode mode)
        {
            _radioFind.Checked = mode == FindReplaceMode.Find;
            _radioReplace.Checked = mode == FindReplaceMode.Replace;
            ApplyMode();
        }

        private void ApplyMode()
        {
            Boolean isFind = _radioFind.Checked;

            this.Text = isFind ? "検索" : "置換";
            this.ClientSize = isFind ? _findModeSize : _replaceModeSize;

            _lblReplace.Visible = !isFind;
            _txtReplace.Visible = !isFind;

            _btnSearch.Visible = isFind;
            _btnFindNext.Visible = !isFind;
            _btnReplace.Visible = !isFind;
            _btnReplaceAll.Visible = !isFind;

            _resultsGrid.Visible = isFind;

            _lblStatus.Text = "";
        }

        private StringComparison Comparison
        {
            get { return _chkMatchCase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase; }
        }

        // _targetGridの表示列(Visible=true)だけを、DisplayIndex順に並べて返す
        private List<DataGridViewColumn> GetVisibleColumns()
        {
            return Form1.GetVisibleColumnsInDisplayOrder(_targetGrid);
        }

        // レコード表(_targetGrid)と同じ列構成(行番号+可視列)で検索結果を一覧表示する。
        // ヒットしたセルはLightYellowでハイライトし、どこがヒットしたか一目で分かるようにする
        private void DoSearch()
        {
            _resultsGrid.Rows.Clear();
            _resultsGrid.Columns.Clear();

            String keyword = _txtFind.Text;
            if (String.IsNullOrEmpty(keyword))
            {
                _lblStatus.Text = "";
                return;
            }

            StringComparison comparison = Comparison;
            List<DataGridViewColumn> visibleColumns = GetVisibleColumns();

            _resultsGrid.Columns.Add("resultRowNo", "行");
            foreach (DataGridViewColumn col in visibleColumns)
            {
                _resultsGrid.Columns.Add("result_" + col.Name, col.HeaderText);
            }

            int hitCount = 0;

            foreach (DataGridViewRow row in _targetGrid.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                List<int> hitVisibleIndexes = new List<int>();
                for (int vi = 0; vi < visibleColumns.Count; vi++)
                {
                    String cellText = Convert.ToString(row.Cells[visibleColumns[vi].Index].Value);
                    if (!String.IsNullOrEmpty(cellText) && cellText.IndexOf(keyword, comparison) >= 0)
                    {
                        hitVisibleIndexes.Add(vi);
                    }
                }

                if (hitVisibleIndexes.Count == 0)
                {
                    continue;
                }

                // 行番号はグリッド左端の行ヘッダーの見た目(dataGridView_Events_RowPostPaintでの
                // e.RowIndexそのまま表示)に合わせる。+1すると実際の行と1つずれてしまうので注意
                Object[] values = new Object[1 + visibleColumns.Count];
                values[0] = row.Index;
                for (int vi = 0; vi < visibleColumns.Count; vi++)
                {
                    values[1 + vi] = row.Cells[visibleColumns[vi].Index].Value;
                }

                int newRowIndex = _resultsGrid.Rows.Add(values);
                _resultsGrid.Rows[newRowIndex].Tag = new HitRowInfo
                {
                    TargetRowIndex = row.Index,
                    HitVisibleColumnIndexes = hitVisibleIndexes,
                };

                foreach (int vi in hitVisibleIndexes)
                {
                    _resultsGrid.Rows[newRowIndex].Cells[1 + vi].Style.BackColor = Color.LightYellow;
                }

                hitCount++;
            }

            _lblStatus.Text = hitCount + " 件ヒット";
        }

        // 検索結果一覧をダブルクリックしたら、本体グリッドの該当セル(最初にヒットした列)へジャンプする
        private void ResultsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            HitRowInfo info = _resultsGrid.Rows[e.RowIndex].Tag as HitRowInfo;
            if (info == null)
            {
                return;
            }

            List<DataGridViewColumn> visibleColumns = GetVisibleColumns();
            int targetColIndex = visibleColumns[info.HitVisibleColumnIndexes[0]].Index;
            JumpToCell(_targetGrid, info.TargetRowIndex, targetColIndex);
        }

        private static void JumpToCell(DataGridView grid, int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count || columnIndex < 0 || columnIndex >= grid.Columns.Count)
            {
                return;
            }

            grid.ClearSelection();
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            grid.Rows[rowIndex].Cells[columnIndex].Selected = true;

            try
            {
                grid.FirstDisplayedScrollingRowIndex = rowIndex;
            }
            catch (InvalidOperationException)
            {
                // グリッドが未表示/高さ0等でスクロール位置を合わせられないタイミングでは
                // 選択自体は済んでいるので、諦めて処理を継続する(HighlightRowと同じ考え方)
            }
        }

        // _lastFoundRow/_lastFoundColの次のセルから、キーワードを含む次のセルを探す(見つかったらtrue)。
        // 末尾まで探して見つからなければ先頭に戻ってもう一周する(現在位置自体は含めない)
        private Boolean FindNext()
        {
            String keyword = _txtFind.Text;
            if (String.IsNullOrEmpty(keyword))
            {
                _lblStatus.Text = "検索文字列を入力してね";
                return false;
            }

            int rowCount = _targetGrid.Rows.Count;
            int colCount = _targetGrid.Columns.Count;
            if (rowCount == 0 || colCount == 0)
            {
                return false;
            }

            StringComparison comparison = Comparison;
            int r = _lastFoundRow < 0 ? 0 : _lastFoundRow;
            int c = _lastFoundRow < 0 ? -1 : _lastFoundCol;

            for (int steps = 0; steps < rowCount * colCount; steps++)
            {
                c++;
                if (c >= colCount)
                {
                    c = 0;
                    r++;
                    if (r >= rowCount)
                    {
                        r = 0;
                    }
                }

                if (_targetGrid.Rows[r].IsNewRow || !_targetGrid.Columns[c].Visible)
                {
                    continue;
                }

                DataGridViewCell cell = _targetGrid.Rows[r].Cells[c];
                if (cell.ReadOnly || cell is DataGridViewComboBoxCell)
                {
                    continue;
                }

                String text = Convert.ToString(cell.Value);
                if (!String.IsNullOrEmpty(text) && text.IndexOf(keyword, comparison) >= 0)
                {
                    _lastFoundRow = r;
                    _lastFoundCol = c;
                    JumpToCell(_targetGrid, r, c);
                    _lblStatus.Text = "";
                    return true;
                }
            }

            _lblStatus.Text = "見つからなかったよ";
            return false;
        }

        // 直前の「次を検索」で選択したセルが検索文字列にマッチしていれば置換して、続けて次を検索する
        private void ReplaceCurrent()
        {
            String keyword = _txtFind.Text;
            if (String.IsNullOrEmpty(keyword))
            {
                return;
            }

            if (_lastFoundRow >= 0 && _lastFoundRow < _targetGrid.Rows.Count
                && _lastFoundCol >= 0 && _lastFoundCol < _targetGrid.Columns.Count)
            {
                DataGridViewCell cell = _targetGrid.Rows[_lastFoundRow].Cells[_lastFoundCol];
                String text = Convert.ToString(cell.Value);
                if (!String.IsNullOrEmpty(text) && text.IndexOf(keyword, Comparison) >= 0)
                {
                    cell.Value = ReplaceAllOccurrences(text, keyword, _txtReplace.Text, Comparison);
                }
            }

            FindNext();
        }

        // すべてのセルを対象に一括置換する。複数セルの変更を1回のCtrl+Zでまとめて戻せるよう、
        // DataGridViewExのUndoバッチで囲む
        private void ReplaceAll()
        {
            String keyword = _txtFind.Text;
            if (String.IsNullOrEmpty(keyword))
            {
                _lblStatus.Text = "検索文字列を入力してね";
                return;
            }

            String replacement = _txtReplace.Text;
            StringComparison comparison = Comparison;
            int replacedCount = 0;

            _targetGrid.BeginUndoBatch();
            try
            {
                foreach (DataGridViewRow row in _targetGrid.Rows)
                {
                    if (row.IsNewRow)
                    {
                        continue;
                    }

                    foreach (DataGridViewColumn col in _targetGrid.Columns)
                    {
                        if (!col.Visible)
                        {
                            continue;
                        }

                        DataGridViewCell cell = row.Cells[col.Index];
                        if (cell.ReadOnly || cell is DataGridViewComboBoxCell)
                        {
                            continue;
                        }

                        String text = Convert.ToString(cell.Value);
                        if (String.IsNullOrEmpty(text) || text.IndexOf(keyword, comparison) < 0)
                        {
                            continue;
                        }

                        cell.Value = ReplaceAllOccurrences(text, keyword, replacement, comparison);
                        replacedCount++;
                    }
                }
            }
            finally
            {
                _targetGrid.EndUndoBatch();
            }

            _lblStatus.Text = replacedCount + " 件置換したよ";
        }

        // String.Replaceは比較方法(大文字/小文字を区別するか)を指定できないため、
        // _chkMatchCaseの設定をそのまま使って全置換できるよう自前で実装する
        private static String ReplaceAllOccurrences(String source, String oldValue, String newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();
            int pos = 0;
            while (true)
            {
                int idx = source.IndexOf(oldValue, pos, comparison);
                if (idx < 0)
                {
                    sb.Append(source, pos, source.Length - pos);
                    break;
                }

                sb.Append(source, pos, idx - pos);
                sb.Append(newValue);
                pos = idx + oldValue.Length;
            }

            return sb.ToString();
        }
    }
}
