using System;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DigitalClockDisplayTitle.Tests
{
    /// <summary>
    /// Form1（タイトルバーに時刻を表示する時計）のテスト。
    ///
    /// ⚠️ Form1_FormClosing は Properties.Settings.Default.Save() を呼び、実際の
    /// ユーザー設定ファイルに書き込んでしまうため、このハンドラはテスト対象から
    /// 除外する（副作用が永続化されるのを避けるため）。Form1_Load は設定を読むだけで
    /// 書き込みはしないため、テスト対象に含めている。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        // Form1.cs 内の private const WM_NCLBUTTONDBLCLK と同じ値
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;

        [TestMethod]
        public void コンストラクタでタイトルバーに現在時刻が表示される()
        {
            using (var form = new Form1())
            {
                // "HH:mm:ss" 形式であることだけを確認（実行タイミングで秒は変わるため）
                Assert.AreEqual(8, form.Text.Length);
                Assert.AreEqual(':', form.Text[2]);
                Assert.AreEqual(':', form.Text[5]);
            }
        }

        [TestMethod]
        public void ClockTimerTickでタイトルバーが更新される()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "ClockTimer_Tick", form);

                Assert.AreEqual(8, form.Text.Length);
                Assert.AreEqual(':', form.Text[2]);
                Assert.AreEqual(':', form.Text[5]);
            }
        }

        [TestMethod]
        public void Form1_Loadはサイズを75x20に固定する()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "Form1_Load", form);

                Assert.AreEqual(new System.Drawing.Size(75, 20), form.Size);
            }
        }

        [TestMethod]
        public void タイトルバーのダブルクリックでフォームが閉じる()
        {
            using (var form = new Form1())
            {
                bool closingRaised = false;
                form.FormClosing += (s, e) => closingRaised = true;

                // ハンドル生成後、WM_NCLBUTTONDBLCLK を直接 WndProc に送る
                IntPtr handle = form.Handle;
                var message = Message.Create(handle, WM_NCLBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero);

                MethodInfo wndProc = typeof(Form1).GetMethod("WndProc", BindingFlags.Instance | BindingFlags.NonPublic);
                object[] args = { message };
                wndProc.Invoke(form, args);

                Assert.IsTrue(closingRaised);
            }
        }
    }
}
