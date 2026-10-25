using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EventRecorder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventRecorder.Tests
{
    /// <summary>
    /// Form1の再生まわりのうち、Form1自体を生成せずに確かめられる部分(static化してある処理)のテスト。
    /// (Form1を生成すると実際のユーザーデータフォルダを読み書きしてしまうため、ここでは生成しない)
    /// </summary>
    [TestClass]
    public class Form1PlaybackTests
    {
        [TestMethod]
        public void ループ数が数値として読めない時は例外にせず1回扱い()
        {
            Assert.AreEqual(1, Form1.ParseLoopCount("abc"));
            Assert.AreEqual(1, Form1.ParseLoopCount(""));
            Assert.AreEqual(1, Form1.ParseLoopCount("1.5"));
            Assert.AreEqual(1, Form1.ParseLoopCount(null));
        }

        [TestMethod]
        public void ループ数は0以下なら1回扱いで正の数ならそのまま()
        {
            Assert.AreEqual(1, Form1.ParseLoopCount("0"));
            Assert.AreEqual(1, Form1.ParseLoopCount("-3"));
            Assert.AreEqual(5, Form1.ParseLoopCount("5"));
        }

        private static DataGridView CreateGrid(int rowCount)
        {
            DataGridView grid = new DataGridView();
            grid.AllowUserToAddRows = false;
            // スクロール位置合わせ(FirstDisplayedScrollingRowIndex)はこのテストの対象外なので非表示にしておく
            grid.Visible = false;
            grid.Columns.Add("col_Type", "Event");
            for (int i = 0; i < rowCount; i++)
            {
                grid.Rows.Add("row" + i);
            }
            return grid;
        }

        private static int CountHighlighted(DataGridView grid)
        {
            return grid.Rows.Cast<DataGridViewRow>().Count(r => r.DefaultCellStyle.BackColor == Color.LightYellow);
        }

        [TestMethod]
        public void ハイライト行より上の行を挿入削除してから次の行をハイライトしても黄色は1行だけ()
        {
            using (DataGridView grid = CreateGrid(3))
            {
                DataGridViewRow highlighted = null;
                Form1.HighlightRow(grid, 2, ref highlighted);
                DataGridViewRow lastRecorded = grid.Rows[2];
                Assert.AreEqual(Color.LightYellow, lastRecorded.DefaultCellStyle.BackColor);

                // ハイライト行より上に行を挿入・削除して、行の位置をずらす
                grid.Rows.Insert(0, "inserted1");
                grid.Rows.Insert(0, "inserted2");
                grid.Rows.RemoveAt(0);

                grid.Rows.Add("row3");
                Form1.HighlightRow(grid, grid.Rows.Count - 1, ref highlighted);

                Assert.AreEqual(Color.Empty, lastRecorded.DefaultCellStyle.BackColor, "前回ハイライトした行の黄色は消える");
                Assert.AreEqual(1, CountHighlighted(grid));
                Assert.AreEqual(Color.LightYellow, grid.Rows[grid.Rows.Count - 1].DefaultCellStyle.BackColor);
            }
        }

        // [Type, X, Y, Key, Wait]の5列の表(列の並びはForm1と違ってもよい。col_XXX.Index経由で読み書きするため)
        private static DataGridView CreateEventGrid(params String[][] rows)
        {
            DataGridView grid = new DataGridView();
            grid.AllowUserToAddRows = false;
            grid.Visible = false;
            grid.Columns.Add("col_Type", "Event");
            grid.Columns.Add("col_X", "X");
            grid.Columns.Add("col_Y", "Y");
            grid.Columns.Add("col_Key", "Key");
            grid.Columns.Add("col_Wait", "Wait");
            foreach (String[] r in rows)
            {
                grid.Rows.Add(r[0], r[1], r[2], r[3], r[4]);
            }
            return grid;
        }

        private static Boolean ReleaseUnpairedModifierKeys(DataGridView grid, params Keys[] pressedKeys)
        {
            return Form1.ReleaseUnpairedModifierKeys(grid, grid.Columns["col_Type"], grid.Columns["col_X"], grid.Columns["col_Y"],
                grid.Columns["col_Key"], grid.Columns["col_Wait"], pressedKeys);
        }

        private static String[] TypeAndKeys(DataGridView grid)
        {
            return grid.Rows.Cast<DataGridViewRow>().Select(r => r.Cells["col_Type"].Value + ":" + r.Cells["col_Key"].Value).ToArray();
        }

        [TestMethod]
        public void 記録停止時_末尾に残った修飾キーのDownだけの行は消す()
        {
            // Ctrl+Shift+F5で記録を止めた時: CtrlとShiftのDownだけが記録され、Upは停止後なので記録されない
            using (DataGridView grid = CreateEventGrid(
                new[] { "KEY_DOWN", "", "", "A", "0" },
                new[] { "KEY_UP", "", "", "A", "0" },
                new[] { "WAIT_MS", "", "", "", "300" },
                new[] { "KEY_DOWN", "", "", "LControlKey", "0" },
                new[] { "WAIT_MS", "", "", "", "100" },
                new[] { "KEY_DOWN", "", "", "LShiftKey", "0" }))
            {
                Assert.IsTrue(ReleaseUnpairedModifierKeys(grid, Keys.LControlKey, Keys.LShiftKey));

                CollectionAssert.AreEqual(new[] { "KEY_DOWN:A", "KEY_UP:A", "WAIT_MS:", "WAIT_MS:" }, TypeAndKeys(grid));
            }
        }

        [TestMethod]
        public void 記録停止時_修飾キーを押したまま他の操作をしていたら末尾にUpを足す()
        {
            // Ctrlを押したままクリックしてから、Ctrl+F5で止めた時: Ctrl+クリックの意味を残す
            using (DataGridView grid = CreateEventGrid(
                new[] { "KEY_DOWN", "", "", "LControlKey", "0" },
                new[] { "LEFT_DOWN", "1", "1", "", "0" },
                new[] { "LEFT_UP", "1", "1", "", "0" },
                new[] { "SYSKEY_DOWN", "", "", "LMenu", "0" },
                new[] { "RIGHT_DOWN", "1", "1", "", "0" }))
            {
                Assert.IsTrue(ReleaseUnpairedModifierKeys(grid, Keys.LControlKey, Keys.LMenu));

                Assert.AreEqual(7, grid.Rows.Count);
                CollectionAssert.AreEquivalent(new[] { "KEY_UP:LControlKey", "SYSKEY_UP:LMenu" }, TypeAndKeys(grid).Skip(5).ToArray());
                Assert.AreEqual("0", grid.Rows[6].Cells["col_Wait"].Value);
            }
        }

        [TestMethod]
        public void 記録停止時_修飾キー以外や押されていないキーは触らない()
        {
            using (DataGridView grid = CreateEventGrid(
                new[] { "KEY_DOWN", "", "", "A", "0" },
                new[] { "KEY_DOWN", "", "", "LControlKey", "0" },
                new[] { "KEY_UP", "", "", "LControlKey", "0" }))
            {
                // Aは押したままだが修飾キーではない。Ctrlは離してあるので押されているキーに入っていない
                Assert.IsFalse(ReleaseUnpairedModifierKeys(grid, Keys.A));
                Assert.IsFalse(ReleaseUnpairedModifierKeys(grid));
                Assert.AreEqual(3, grid.Rows.Count);
            }
        }

        [TestMethod]
        public void 再生時に飛ばされる行は種類ごとに行番号付きでまとめる()
        {
            List<String> types = new List<String> { "LEFT_DOWN", "SYSKEY_DOWN", "WAIT_MS", "SYSKEY_UP", "KEY_DOWN", "X1_DOWN" };
            for (int i = 0; i < 7; i++)
            {
                types.Add("WHEEL_DOWN");
            }

            List<String> lines = Form1.SummarizeUnsupportedRowsForPlayback(types);

            CollectionAssert.AreEqual(new[]
            {
                "SYSKEY_DOWN: 1行(行1)",
                "SYSKEY_UP: 1行(行3)",
                "X1_DOWN: 1行(行5)",
                "WHEEL_DOWN: 7行(行6, 行7, 行8, 行9, 行10, …)",
            }, lines);
        }

        [TestMethod]
        public void 再生できる行だけなら飛ばされる行のまとめは空()
        {
            List<String> types = new List<String> { "LEFT_DOWN", "LEFT_UP", "KEY_DOWN", "KEY_UP", "WAIT_MS", "MIDDLE_DOWN" };

            Assert.AreEqual(0, Form1.SummarizeUnsupportedRowsForPlayback(types).Count);
        }

        [TestMethod]
        public void ハイライト行が削除済みでも次のハイライトやマイナス1での解除は例外にならない()
        {
            using (DataGridView grid = CreateGrid(3))
            {
                DataGridViewRow highlighted = null;
                Form1.HighlightRow(grid, 1, ref highlighted);

                grid.Rows.RemoveAt(1);
                Form1.HighlightRow(grid, 0, ref highlighted);
                Assert.AreEqual(1, CountHighlighted(grid));
                Assert.AreSame(grid.Rows[0], highlighted);

                Form1.HighlightRow(grid, -1, ref highlighted);
                Assert.AreEqual(0, CountHighlighted(grid));
                Assert.IsNull(highlighted);
            }
        }
    }
}
