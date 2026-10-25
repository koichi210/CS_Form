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

        // 再生で送る入力イベントの中身だけを確かめる(SendInputは呼ばない。InputSimulatorTestsと同じ方針)
        private const int MouseeventfMove = 0x0001;
        private const int MouseeventfXDown = 0x0080;
        private const int MouseeventfXUp = 0x0100;
        private const int MouseeventfWheel = 0x0800;
        private const int MouseeventfAbsoluteVirtualDesk = 0x8000 | 0x4000;
        private const int KeyeventfKeyUp = 0x0002;

        [TestMethod]
        public void ホイールの行はクリックと同じ座標で1ノッチ分のWHEELを送る()
        {
            InputSimulation.InputSimulator.Input click = Form1.BuildPlaybackInputs("LEFT_DOWN", "100", "200", "").Single();
            InputSimulation.InputSimulator.Input wheelUp = Form1.BuildPlaybackInputs("WHEEL_UP", "100", "200", "").Single();
            InputSimulation.InputSimulator.Input wheelDown = Form1.BuildPlaybackInputs("WHEEL_DOWN", "100", "200", "").Single();

            foreach (InputSimulation.InputSimulator.Input input in new[] { wheelUp, wheelDown })
            {
                Assert.AreEqual(0, input.Type, "マウス入力は Type=0");
                Assert.AreEqual(MouseeventfMove | MouseeventfWheel | MouseeventfAbsoluteVirtualDesk, input.Mouse.Flags);
                Assert.AreEqual(click.Mouse.X, input.Mouse.X);
                Assert.AreEqual(click.Mouse.Y, input.Mouse.Y);
            }

            // 奥へ回す(WHEEL_UP)が+、手前へ回す(WHEEL_DOWN)が-。回転量は記録していないので1ノッチ(WHEEL_DELTA=120)
            Assert.AreEqual(120, wheelUp.Mouse.Data);
            Assert.AreEqual(-120, wheelDown.Mouse.Data);
        }

        [TestMethod]
        public void X1とX2ボタンの行はクリックと同じ座標でXDOWNとXUPをボタン番号付きで送る()
        {
            InputSimulation.InputSimulator.Input click = Form1.BuildPlaybackInputs("LEFT_DOWN", "-50", "30", "").Single();
            String[] types = { "X1_DOWN", "X1_UP", "X2_DOWN", "X2_UP" };
            int[] expectedButtonFlags = { MouseeventfXDown, MouseeventfXUp, MouseeventfXDown, MouseeventfXUp };
            int[] expectedData = { 1, 1, 2, 2 };   // XBUTTON1 / XBUTTON2

            for (int i = 0; i < types.Length; i++)
            {
                InputSimulation.InputSimulator.Input input = Form1.BuildPlaybackInputs(types[i], "-50", "30", "").Single();

                Assert.AreEqual(0, input.Type, types[i]);
                Assert.AreEqual(MouseeventfMove | expectedButtonFlags[i] | MouseeventfAbsoluteVirtualDesk, input.Mouse.Flags, types[i]);
                Assert.AreEqual(expectedData[i], input.Mouse.Data, types[i]);
                Assert.AreEqual(click.Mouse.X, input.Mouse.X, types[i]);
                Assert.AreEqual(click.Mouse.Y, input.Mouse.Y, types[i]);
            }
        }

        [TestMethod]
        public void SYSKEYの行は普通のキーとして送りAlt付きの組み合わせもそのまま送れる()
        {
            // Alt+F4を記録した時の並び(Altを押している間のキーはSYSKEY_DOWN/UPになる)
            String[][] rows =
            {
                new[] { "SYSKEY_DOWN", "LMenu" },
                new[] { "SYSKEY_DOWN", "F4" },
                new[] { "SYSKEY_UP", "F4" },
                new[] { "KEY_UP", "LMenu" },
                new[] { "SYSKEY_DOWN", "F10" },   // F10単独もSYSKEY
                new[] { "SYSKEY_UP", "F10" },
            };
            Keys[] expectedKeys = { Keys.LMenu, Keys.F4, Keys.F4, Keys.LMenu, Keys.F10, Keys.F10 };
            Boolean[] expectedUp = { false, false, true, true, false, true };

            for (int i = 0; i < rows.Length; i++)
            {
                InputSimulation.InputSimulator.Input input = Form1.BuildPlaybackInputs(rows[i][0], "", "", rows[i][1]).Single();

                Assert.AreEqual(1, input.Type, "キーボード入力は Type=1");
                Assert.AreEqual((short)expectedKeys[i], input.Keyboard.VirtualKey, rows[i][0] + ":" + rows[i][1]);
                Assert.AreEqual(expectedUp[i] ? KeyeventfKeyUp : 0, input.Keyboard.Flags & KeyeventfKeyUp, rows[i][0] + ":" + rows[i][1]);

                // 送る中身は普通のKEY_DOWN/KEY_UPの行と同じ
                String keyType = expectedUp[i] ? "KEY_UP" : "KEY_DOWN";
                InputSimulation.InputSimulator.Input asKey = Form1.BuildPlaybackInputs(keyType, "", "", rows[i][1]).Single();
                Assert.AreEqual(asKey.Keyboard.Flags, input.Keyboard.Flags);
                Assert.AreEqual(asKey.Keyboard.ScanCode, input.Keyboard.ScanCode);
            }
        }

        [TestMethod]
        public void 再生しない行は何も送らない()
        {
            Assert.AreEqual(0, Form1.BuildPlaybackInputs("WAIT_MS", "", "", "").Count);
            Assert.AreEqual(0, Form1.BuildPlaybackInputs("TYPO", "1", "2", "A").Count);
            Assert.AreEqual(0, Form1.BuildPlaybackInputs("SYSKEY_DOWN", "", "", "NoSuchKey").Count);
        }

        [TestMethod]
        public void ホイールとXボタンとSYSKEYの行もマウス行やキー行として再生前チェックの対象になる()
        {
            // 再生前チェック(IsRowInvalidForPlayback)・Keyが無視される警告(IsKeyIgnoredOnMouseRow)はこの判定で振り分ける
            InputSimulation.InputSimulator.MouseStroke mouseStroke;
            int data;
            foreach (String type in new[] { "WHEEL_UP", "WHEEL_DOWN", "X1_DOWN", "X1_UP", "X2_DOWN", "X2_UP", "LEFT_DOWN" })
            {
                Assert.IsTrue(Form1.TryGetPlaybackMouseStroke(type, out mouseStroke, out data), type);
            }
            Assert.IsFalse(Form1.TryGetPlaybackMouseStroke("SYSKEY_DOWN", out mouseStroke, out data));

            InputSimulation.InputSimulator.KeyboardStroke keyStroke;
            Assert.IsTrue(Form1.TryGetPlaybackKeyboardStroke("SYSKEY_DOWN", out keyStroke));
            Assert.AreEqual(InputSimulation.InputSimulator.KeyboardStroke.KEY_DOWN, keyStroke);
            Assert.IsTrue(Form1.TryGetPlaybackKeyboardStroke("SYSKEY_UP", out keyStroke));
            Assert.AreEqual(InputSimulation.InputSimulator.KeyboardStroke.KEY_UP, keyStroke);
            Assert.IsFalse(Form1.TryGetPlaybackKeyboardStroke("WHEEL_UP", out keyStroke));
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
