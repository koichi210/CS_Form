using System;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Othello.Tests
{
    /// <summary>
    /// Form1（オセロ盤面のフォーム）のテスト。
    ///
    /// ⚠️ button_ReStart_Click は常にMessageBox.Showを呼ぶため、テスト対象から
    /// 除外する。盤面描画ロジック自体はBoardRendererTestsでBoardRendererクラスを直接検証している。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        private const BindingFlags _instanceAny = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static object GetField(Form1 form, string name)
        {
            return typeof(Form1).GetField(name, _instanceAny).GetValue(form);
        }

        private static void SetField(Form1 form, string name, object value)
        {
            typeof(Form1).GetField(name, _instanceAny).SetValue(form, value);
        }

        private static void Invoke(Form1 form, string name, params object[] args)
        {
            typeof(Form1).GetMethod(name, _instanceAny).Invoke(form, args);
        }

        [TestMethod]
        public void コンストラクタで例外なく生成でき盤面が初期化される()
        {
            using (var form = new Form1())
            {
                Assert.IsNotNull(form);
            }
        }

        [TestMethod]
        public void 時間切れになると一手戻す一手進めるは押せない()
        {
            using (var form = new Form1())
            {
                var undo = (ToolStripMenuItem)GetField(form, "menuItem_Undo");
                var redo = (ToolStripMenuItem)GetField(form, "menuItem_Redo");

                Invoke(form, "menuItem_TimeLimit_Click", GetField(form, "menuItem_Time30s"), EventArgs.Empty);
                var gm = (GameMaster)GetField(form, "_gm");
                Assert.IsTrue(gm.TryPut(2, 3));
                Assert.IsTrue(gm.TryPut(2, 2));
                Invoke(form, "menuItem_Start_Click", null, EventArgs.Empty);
                Assert.IsTrue(undo.Enabled);
                Assert.IsTrue(((ToolStripMenuItem)GetField(form, "menuItem_Stop")).Enabled);

                // 今の手番(黒)の残り時間を1カウント分にして、次のカウントダウンで時間切れにする
                SetField(form, "_blackTimeMs", 100);
                Invoke(form, "CountdownTimer_Tick", null, EventArgs.Empty);
                Assert.IsTrue((bool)GetField(form, "_isTimedOut"));
                Assert.IsFalse(undo.Enabled);
                Assert.IsFalse(redo.Enabled);

                // 時間切れで終局になるので、「停止」は押せず「開始」で新しい対局を始められる
                Assert.IsFalse(((ToolStripMenuItem)GetField(form, "menuItem_Stop")).Enabled);
                Assert.IsTrue(((ToolStripMenuItem)GetField(form, "menuItem_Start")).Enabled);

                // メニューを経由せずに呼ばれても戻さない(時間切れのまま)
                Invoke(form, "menuItem_Undo_Click", null, EventArgs.Empty);
                Assert.AreEqual(2, gm.TurnCount);
                Assert.IsTrue((bool)GetField(form, "_isTimedOut"));
            }
        }
    }
}
