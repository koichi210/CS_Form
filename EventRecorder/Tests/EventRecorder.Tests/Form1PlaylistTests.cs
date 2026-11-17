using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EventRecorder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StandardTemplate;

namespace EventRecorder.Tests
{
    /// <summary>
    /// プレイリストの行のドラッグ&ドロップでの並べ替えのうち、Form1を生成せずに確かめられる部分(static化してある処理)のテスト。
    /// (Form1を生成すると実際のユーザーデータフォルダを読み書きしてしまうため、ここでは生成しない。
    /// 実際のドラッグ操作(DoDragDrop)はOSのドラッグ&ドロップを動かしてしまうので呼ばない)
    /// </summary>
    [TestClass]
    public class Form1PlaylistTests
    {
        // Form1のプレイリストと同じ[実行チェック/設定ファイル/ループ数]の3列の表
        private static T CreatePlaylistGrid<T>(params Object[][] rows) where T : DataGridView, new()
        {
            T grid = new T();
            grid.AllowUserToAddRows = false;
            grid.Visible = false;
            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "col_PlaylistEnabled" });
            grid.Columns.Add("col_PlaylistFile", "プレイリスト");
            grid.Columns.Add("col_PlaylistLoopCount", "ループ数");
            foreach (Object[] r in rows)
            {
                grid.Rows.Add(r);
            }
            return grid;
        }

        private static String[] Files(DataGridView grid)
        {
            return grid.Rows.Cast<DataGridViewRow>().Select(r => Convert.ToString(r.Cells["col_PlaylistFile"].Value)).ToArray();
        }

        private static Object[][] ThreeRows()
        {
            return new[]
            {
                new Object[] { true, "a.json", "1" },
                new Object[] { false, "b.json", "2" },
                new Object[] { true, "c.json", "3" },
            };
        }

        [TestMethod]
        public void 下へ動かすとドロップ先の行の下に入り実行チェックとループ数も一緒に動く()
        {
            using (DataGridView grid = CreatePlaylistGrid<DataGridView>(ThreeRows()))
            {
                Assert.IsTrue(Form1.MoveGridRow(grid, 0, 2));

                CollectionAssert.AreEqual(new[] { "b.json", "c.json", "a.json" }, Files(grid));
                Assert.AreEqual(true, grid.Rows[2].Cells["col_PlaylistEnabled"].Value);
                Assert.AreEqual("1", grid.Rows[2].Cells["col_PlaylistLoopCount"].Value);
                Assert.AreEqual(false, grid.Rows[0].Cells["col_PlaylistEnabled"].Value);
                Assert.AreEqual("2", grid.Rows[0].Cells["col_PlaylistLoopCount"].Value);
            }
        }

        [TestMethod]
        public void 上へ動かすとドロップ先の行の上に入る()
        {
            using (DataGridView grid = CreatePlaylistGrid<DataGridView>(ThreeRows()))
            {
                Assert.IsTrue(Form1.MoveGridRow(grid, 2, 0));

                CollectionAssert.AreEqual(new[] { "c.json", "a.json", "b.json" }, Files(grid));
                Assert.AreEqual("3", grid.Rows[0].Cells["col_PlaylistLoopCount"].Value);
            }
        }

        [TestMethod]
        public void 動かせない指定では何もしない()
        {
            using (DataGridView grid = CreatePlaylistGrid<DataGridView>(ThreeRows()))
            {
                Assert.IsFalse(Form1.MoveGridRow(grid, 1, 1), "同じ位置");
                Assert.IsFalse(Form1.MoveGridRow(grid, -1, 0));
                Assert.IsFalse(Form1.MoveGridRow(grid, 0, -1), "行の無い所で離した");
                Assert.IsFalse(Form1.MoveGridRow(grid, 0, 3));
                Assert.IsFalse(Form1.MoveGridRow(grid, 3, 0));

                CollectionAssert.AreEqual(new[] { "a.json", "b.json", "c.json" }, Files(grid));
            }
        }

        [TestMethod]
        public void 並べ替えではセルの値の変更イベントが起きずファイル不在のピンク表示も行と一緒に動く()
        {
            using (DataGridView grid = CreatePlaylistGrid<DataGridView>(ThreeRows()))
            {
                // ファイル列の値が入れ直されると、Form1は「ファイルを選んだらループ数を自動セット」を走らせてしまう
                int valueChangedCount = 0;
                grid.CellValueChanged += (s, e) => valueChangedCount++;

                DataGridViewCellStyle missingStyle = new DataGridViewCellStyle { BackColor = Color.MistyRose };
                grid.Rows[1].Cells["col_PlaylistFile"].Style = missingStyle;
                grid.Rows[1].Cells["col_PlaylistFile"].ErrorText = "見つからない";

                Assert.IsTrue(Form1.MoveGridRow(grid, 1, 2));

                Assert.AreEqual(0, valueChangedCount);
                Assert.AreEqual("b.json", grid.Rows[2].Cells["col_PlaylistFile"].Value);
                Assert.AreEqual(Color.MistyRose, grid.Rows[2].Cells["col_PlaylistFile"].Style.BackColor);
                Assert.AreEqual("見つからない", grid.Rows[2].Cells["col_PlaylistFile"].ErrorText);
                Assert.AreNotEqual(Color.MistyRose, grid.Rows[1].Cells["col_PlaylistFile"].Style.BackColor);
            }
        }

        [TestMethod]
        public void 並べ替え直後のUndoで動かした行のセルが空にならない()
        {
            // 以前は値をコピーして行を作り直していたため、その書き込みがUndo履歴に積まれ、Ctrl+Zで移動した行が空になっていた
            foreach (int target in new[] { 2, 1 })   // 末尾へ / 途中へ
            {
                using (DataGridViewEx grid = CreatePlaylistGrid<DataGridViewEx>(ThreeRows()))
                {
                    grid.ClearUndoHistory();

                    Assert.IsTrue(Form1.MoveGridRow(grid, 0, target));
                    Assert.IsFalse(grid.CanUndo, "並べ替えそのものは取り消し対象にしない(行の削除と同じ)");

                    // 動かした行を編集してから戻すと、空ではなく動かした時の値に戻る
                    grid.Rows[target].Cells["col_PlaylistLoopCount"].Value = "9";
                    Assert.IsTrue(grid.CanUndo);
                    grid.Undo();

                    Assert.AreEqual("1", grid.Rows[target].Cells["col_PlaylistLoopCount"].Value, "target=" + target);
                    Assert.AreEqual("a.json", grid.Rows[target].Cells["col_PlaylistFile"].Value, "target=" + target);
                    Assert.AreEqual(true, grid.Rows[target].Cells["col_PlaylistEnabled"].Value, "target=" + target);
                }
            }
        }

        [TestMethod]
        public void ドロップ先は行の上ならその行_最後の行より下の空白なら見えている最後の行()
        {
            Assert.AreEqual(1, Form1.ResolvePlaylistDropIndex(1, false, 4));
            Assert.AreEqual(4, Form1.ResolvePlaylistDropIndex(-1, true, 4), "最後の行より下の空白");
            Assert.AreEqual(-1, Form1.ResolvePlaylistDropIndex(-1, false, 4), "列ヘッダー・スクロールバーの上");
            Assert.AreEqual(-1, Form1.ResolvePlaylistDropIndex(-1, true, -1), "行が1つも見えていない");
        }
    }
}
