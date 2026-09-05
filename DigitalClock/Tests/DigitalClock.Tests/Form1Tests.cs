using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DigitalClock.Tests
{
    /// <summary>
    /// Form1（時計表示・ドラッグ移動）のテスト。
    ///
    /// ⚠️ Form1_FormClosing は Properties.Settings.Default.Save() を呼び、実際の
    /// ユーザー設定ファイルに書き込んでしまうため、このハンドラはテスト対象から
    /// 除外する（副作用が永続化されるのを避けるため）。Form1_Load は設定を読むだけで
    /// 書き込みはしないため、テスト対象に含めている。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void コンストラクタで現在時刻がlabel_timeに表示される()
        {
            using (var form = new Form1())
            {
                string text = FormReflection.GetText(form, "label_time");

                // "HH:mm:ss" 形式であることだけを確認（実行タイミングで秒は変わるため）
                Assert.AreEqual(8, text.Length);
                Assert.AreEqual(':', text[2]);
                Assert.AreEqual(':', text[5]);
            }
        }

        [TestMethod]
        public void ClockTimerTickでlabel_timeが更新される()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "ClockTimer_Tick");

                string text = FormReflection.GetText(form, "label_time");

                Assert.AreEqual(8, text.Length);
                Assert.AreEqual(':', text[2]);
                Assert.AreEqual(':', text[5]);
            }
        }

        [TestMethod]
        public void 左ボタンドラッグでフォームが移動する()
        {
            using (var form = new Form1())
            {
                form.Left = 100;
                form.Top = 100;

                var downArgs = new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0);
                FormReflection.InvokeHandler(form, "label_time_MouseDown", form, downArgs);

                var moveArgs = new MouseEventArgs(MouseButtons.Left, 1, 15, 25, 0);
                FormReflection.InvokeHandler(form, "label_time_MouseMove", form, moveArgs);

                // X:10→15(+5), Y:10→25(+15) 移動したはず
                Assert.AreEqual(105, form.Left);
                Assert.AreEqual(115, form.Top);
            }
        }

        [TestMethod]
        public void 左ボタン以外のドラッグではフォームが移動しない()
        {
            using (var form = new Form1())
            {
                form.Left = 100;
                form.Top = 100;

                var downArgs = new MouseEventArgs(MouseButtons.Right, 1, 10, 10, 0);
                FormReflection.InvokeHandler(form, "label_time_MouseDown", form, downArgs);

                var moveArgs = new MouseEventArgs(MouseButtons.Right, 1, 50, 50, 0);
                FormReflection.InvokeHandler(form, "label_time_MouseMove", form, moveArgs);

                Assert.AreEqual(100, form.Left);
                Assert.AreEqual(100, form.Top);
            }
        }

        [TestMethod]
        public void Form1_Loadは保存済みサイズが0なら初期位置に配置する()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "Form1_Load", form, EventArgs.Empty);

                // 保存設定が既定(0,0)の場合、(100,100)に配置される想定
                if (Properties.Settings.Default.FormSize.Width == 0 || Properties.Settings.Default.FormSize.Height == 0)
                {
                    Assert.AreEqual(new System.Drawing.Point(100, 100), form.Location);
                }
            }
        }
    }
}
