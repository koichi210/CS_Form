using System;
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
