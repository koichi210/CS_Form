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
    // レイアウトは上部(topPanel)/中央(resultsGrid)/下部(bottomPanel)のDockで組んでいる。
    // 絶対座標のLocation指定だけだと、検索モードでボタンが結果一覧と重なったりダイアログから
    // はみ出たりする不具合が起きたため、Dockベースにして常に3つの帯に収まるようにしてある
    internal class FindReplaceForm : DialogFormBase
    {
        private readonly StandardTemplate.DataGridViewEx targetGrid;

        private readonly RadioButton radioFind;
        private readonly RadioButton radioReplace;
        private readonly Label lblFind;
        private readonly TextBox txtFind;
        private readonly Label lblReplace;
        private readonly TextBox txtReplace;
        private readonly CheckBox chkMatchCase;
        private readonly Button btnSearch;
        private readonly Button btnFindNext;
        private readonly Button btnReplace;
        private readonly Button btnReplaceAll;
        private readonly DataGridView resultsGrid;
        private readonly Label lblStatus;

        // 検索モード・置換モードとも幅は共通にして、topPanel内のボタン位置(X=320)を使い回せるようにする。
        // 高さだけ、検索モードは結果一覧の分だけ余計に確保する
        private const int PanelWidth = 460;
        private static readonly Size FindModeSize = new Size(PanelWidth, 420);
        private static readonly Size ReplaceModeSize = new Size(PanelWidth, 170);

        // 検索結果一覧の1行が、レコード表(targetGrid)のどの行/どの可視列でヒットしたかを覚えておく
        private class HitRowInfo
        {
            public int TargetRowIndex;
            public List<int> HitVisibleColumnIndexes;
        }

        // 「次を検索」で最後に見つけたセル位置。次回はこの続き(次のセル)から探す
        private int lastFoundRow = -1;
        private int lastFoundCol = -1;

        public FindReplaceForm(StandardTemplate.DataGridViewEx grid, FindReplaceMode initialMode, String initialSearchText)
        {
            targetGrid = grid;

            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(360, 200);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;

            // ***** 上段: ラジオボタン+検索/置換の入力欄+モード別ボタン *****
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 120 };

            radioFind = new RadioButton { Text = "検索", Location = new Point(10, 8), AutoSize = true };
            radioReplace = new RadioButton { Text = "置換", Location = new Point(80, 8), AutoSize = true };

            lblFind = new Label { Text = "検索文字列:", Location = new Point(10, 38), AutoSize = true };
            txtFind = new TextBox { Location = new Point(90, 34), Width = 220, Text = initialSearchText ?? "" };

            lblReplace = new Label { Text = "置換後の文字列:", Location = new Point(10, 66), AutoSize = true };
            txtReplace = new TextBox { Location = new Point(110, 62), Width = 200 };

            chkMatchCase = new CheckBox { Text = "大文字/小文字を区別する", Location = new Point(10, 94), AutoSize = true };

            // 検索モード用/置換モード用のボタンは、それぞれの行の右端(X=320)に置き、ApplyModeでVisibleを切り替える
            btnSearch = new Button { Text = "検索(&S)", Location = new Point(320, 32), Width = 90 };
            btnFindNext = new Button { Text = "次を検索(&N)", Location = new Point(320, 32), Width = 90 };
            btnReplace = new Button { Text = "置換(&R)", Location = new Point(320, 60), Width = 90 };
            btnReplaceAll = new Button { Text = "すべて置換(&A)", Location = new Point(320, 88), Width = 90 };

            topPanel.Controls.Add(radioFind);
            topPanel.Controls.Add(radioReplace);
            topPanel.Controls.Add(lblFind);
            topPanel.Controls.Add(txtFind);
            topPanel.Controls.Add(lblReplace);
            topPanel.Controls.Add(txtReplace);
            topPanel.Controls.Add(chkMatchCase);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(btnFindNext);
            topPanel.Controls.Add(btnReplace);
            topPanel.Controls.Add(btnReplaceAll);

            // ***** 中段: 検索結果一覧(検索モードのみ表示、Dock=Fillで残り全体を使う) *****
            resultsGrid = new DataGridView
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
            resultsGrid.CellDoubleClick += ResultsGrid_CellDoubleClick;

            // ***** 下段: ステータス表示(結果一覧と重ならない専用の帯)。
            // 閉じるボタンは置かない。Escキー(DialogFormBase)で閉じられるので不要 *****
            Panel bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };

            lblStatus = new Label { Location = new Point(10, 10), AutoSize = true };

            bottomPanel.Controls.Add(lblStatus);

            // Dock=Fillのコントロールは、他のDock(Top/Bottom等)より後にControls.Addしないと、
            // 先に全クライアント領域を占有してしまい、後から追加した帯(bottomPanelの閉じるボタン等)が
            // その下に隠れてしまう。そのため、上下の帯を先に追加し、resultsGrid(Fill)を最後に追加する
            this.Controls.Add(topPanel);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(resultsGrid);

            radioFind.CheckedChanged += (s, e) => { if (radioFind.Checked) ApplyMode(); };
            radioReplace.CheckedChanged += (s, e) => { if (radioReplace.Checked) ApplyMode(); };
            btnSearch.Click += (s, e) => DoSearch();
            btnFindNext.Click += (s, e) => FindNext();
            btnReplace.Click += (s, e) => ReplaceCurrent();
            btnReplaceAll.Click += (s, e) => ReplaceAll();

            txtFind.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter)
                {
                    return;
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
                if (radioFind.Checked)
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
            radioFind.Checked = mode == FindReplaceMode.Find;
            radioReplace.Checked = mode == FindReplaceMode.Replace;
            ApplyMode();
        }

        private void ApplyMode()
        {
            Boolean isFind = radioFind.Checked;

            this.Text = isFind ? "検索" : "置換";
            this.ClientSize = isFind ? FindModeSize : ReplaceModeSize;

            lblReplace.Visible = !isFind;
            txtReplace.Visible = !isFind;

            btnSearch.Visible = isFind;
            btnFindNext.Visible = !isFind;
            btnReplace.Visible = !isFind;
            btnReplaceAll.Visible = !isFind;

            resultsGrid.Visible = isFind;

            lblStatus.Text = "";
        }

        private StringComparison Comparison
        {
            get { return chkMatchCase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase; }
        }

        // gridの表示列(Visible=true)だけを、DisplayIndex順に並べて返す
        private List<DataGridViewColumn> VisibleColumns()
        {
            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in targetGrid.Columns)
            {
                if (col.Visible)
                {
                    columns.Add(col);
                }
            }

            columns.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));
            return columns;
        }

        // レコード表(targetGrid)と同じ列構成(行番号+可視列)で検索結果を一覧表示する。
        // ヒットしたセルはLightYellowでハイライトし、どこがヒットしたか一目で分かるようにする
        private void DoSearch()
        {
            resultsGrid.Rows.Clear();
            resultsGrid.Columns.Clear();

            String keyword = txtFind.Text;
            if (String.IsNullOrEmpty(keyword))
            {
                lblStatus.Text = "";
                return;
            }

            StringComparison comparison = Comparison;
            List<DataGridViewColumn> visibleColumns = VisibleColumns();

            resultsGrid.Columns.Add("resultRowNo", "行");
            foreach (DataGridViewColumn col in visibleColumns)
            {
                resultsGrid.Columns.Add("result_" + col.Name, col.HeaderText);
            }

            int hitCount = 0;

            foreach (DataGridViewRow row in targetGrid.Rows)
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

                int newRowIndex = resultsGrid.Rows.Add(values);
                resultsGrid.Rows[newRowIndex].Tag = new HitRowInfo
                {
                    TargetRowIndex = row.Index,
                    HitVisibleColumnIndexes = hitVisibleIndexes,
                };

                foreach (int vi in hitVisibleIndexes)
                {
                    resultsGrid.Rows[newRowIndex].Cells[1 + vi].Style.BackColor = Color.LightYellow;
                }

                hitCount++;
            }

            lblStatus.Text = hitCount + " 件ヒット";
        }

        // 検索結果一覧をダブルクリックしたら、本体グリッドの該当セル(最初にヒットした列)へジャンプする
        private void ResultsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            HitRowInfo info = resultsGrid.Rows[e.RowIndex].Tag as HitRowInfo;
            if (info == null)
            {
                return;
            }

            List<DataGridViewColumn> visibleColumns = VisibleColumns();
            int targetColIndex = visibleColumns[info.HitVisibleColumnIndexes[0]].Index;
            JumpToCell(targetGrid, info.TargetRowIndex, targetColIndex);
        }

        internal static void JumpToCell(DataGridView grid, int rowIndex, int columnIndex)
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

        // lastFoundRow/lastFoundColの次のセルから、キーワードを含む次のセルを探す(見つかったらtrue)。
        // 末尾まで探して見つからなければ先頭に戻ってもう一周する(現在位置自体は含めない)
        private Boolean FindNext()
        {
            String keyword = txtFind.Text;
            if (String.IsNullOrEmpty(keyword))
            {
                lblStatus.Text = "検索文字列を入力してね";
                return false;
            }

            int rowCount = targetGrid.Rows.Count;
            int colCount = targetGrid.Columns.Count;
            if (rowCount == 0 || colCount == 0)
            {
                return false;
            }

            int r = lastFoundRow < 0 ? 0 : lastFoundRow;
            int c = lastFoundRow < 0 ? -1 : lastFoundCol;

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

                if (targetGrid.Rows[r].IsNewRow || !targetGrid.Columns[c].Visible)
                {
                    continue;
                }

                DataGridViewCell cell = targetGrid.Rows[r].Cells[c];
                if (cell.ReadOnly || cell is DataGridViewComboBoxCell)
                {
                    continue;
                }

                String text = Convert.ToString(cell.Value);
                if (!String.IsNullOrEmpty(text) && text.IndexOf(keyword, Comparison) >= 0)
                {
                    lastFoundRow = r;
                    lastFoundCol = c;
                    JumpToCell(targetGrid, r, c);
                    lblStatus.Text = "";
                    return true;
                }
            }

            lblStatus.Text = "見つからなかったよ";
            return false;
        }

        // 直前の「次を検索」で選択したセルが検索文字列にマッチしていれば置換して、続けて次を検索する
        private void ReplaceCurrent()
        {
            String keyword = txtFind.Text;
            if (String.IsNullOrEmpty(keyword))
            {
                return;
            }

            if (lastFoundRow >= 0 && lastFoundRow < targetGrid.Rows.Count
                && lastFoundCol >= 0 && lastFoundCol < targetGrid.Columns.Count)
            {
                DataGridViewCell cell = targetGrid.Rows[lastFoundRow].Cells[lastFoundCol];
                String text = Convert.ToString(cell.Value);
                if (!String.IsNullOrEmpty(text) && text.IndexOf(keyword, Comparison) >= 0)
                {
                    cell.Value = ReplaceAllOccurrences(text, keyword, txtReplace.Text, Comparison);
                }
            }

            FindNext();
        }

        // すべてのセルを対象に一括置換する。複数セルの変更を1回のCtrl+Zでまとめて戻せるよう、
        // DataGridViewExのUndoバッチで囲む
        private void ReplaceAll()
        {
            String keyword = txtFind.Text;
            if (String.IsNullOrEmpty(keyword))
            {
                lblStatus.Text = "検索文字列を入力してね";
                return;
            }

            String replacement = txtReplace.Text;
            int replacedCount = 0;

            targetGrid.BeginUndoBatch();
            try
            {
                foreach (DataGridViewRow row in targetGrid.Rows)
                {
                    if (row.IsNewRow)
                    {
                        continue;
                    }

                    foreach (DataGridViewColumn col in targetGrid.Columns)
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
                        if (String.IsNullOrEmpty(text) || text.IndexOf(keyword, Comparison) < 0)
                        {
                            continue;
                        }

                        cell.Value = ReplaceAllOccurrences(text, keyword, replacement, Comparison);
                        replacedCount++;
                    }
                }
            }
            finally
            {
                targetGrid.EndUndoBatch();
            }

            lblStatus.Text = replacedCount + " 件置換したよ";
        }

        // String.Replaceは比較方法(大文字/小文字を区別するか)を指定できないため、
        // chkMatchCaseの設定をそのまま使って全置換できるよう自前で実装する
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
