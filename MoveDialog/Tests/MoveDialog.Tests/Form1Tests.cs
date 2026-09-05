using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoveDialog.Tests
{
    /// <summary>
    /// Form1（子ウィンドウの位置・サイズ・色を操作するサンプル）のテスト。
    /// MessageBox.Show を呼ぶ箇所は無いため、全ハンドラをテスト対象にできる。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void 上ボタンで子ウィンドウのTopが移動距離分減る()
        {
            using (var form = new Form1())
            {
                var child = (Form)FormReflection.GetField(form, "child");
                var trackBarMoveDistance = FormReflection.GetControl(form, "trackBarMoveDistance");
                trackBarMoveDistance.GetType().GetProperty("Value").SetValue(trackBarMoveDistance, 20, null);

                int before = child.Top;
                var sender = new Button { Name = "buttonMoveDirectionUp" };
                FormReflection.InvokeHandler(form, "updatePosition", sender);

                Assert.AreEqual(System.Math.Max(0, before - 20), child.Top);

                child.Dispose();
            }
        }

        [TestMethod]
        public void 中央ボタンで子ウィンドウが中央寄せされる()
        {
            using (var form = new Form1())
            {
                var child = (Form)FormReflection.GetField(form, "child");
                var trackBarWindowWidth = (TrackBar)FormReflection.GetControl(form, "trackBarWindowWidth");
                var trackBarWindowHeight = (TrackBar)FormReflection.GetControl(form, "trackBarWindowHeight");

                var sender = new Button { Name = "buttonMoveDirectionCentor" };
                FormReflection.InvokeHandler(form, "updatePosition", sender);

                int expectedLeft = (trackBarWindowWidth.Maximum - child.Width) / 2;
                int expectedTop = (trackBarWindowHeight.Maximum - child.Height) / 2;
                Assert.AreEqual(expectedLeft, child.Left);
                Assert.AreEqual(expectedTop, child.Top);

                child.Dispose();
            }
        }

        [TestMethod]
        public void チェックボックスで子ウィンドウの表示非表示が切り替わる()
        {
            using (var form = new Form1())
            {
                var child = (Form)FormReflection.GetField(form, "child");
                var checkBoxVisible = (CheckBox)FormReflection.GetControl(form, "checkBoxVisible");

                checkBoxVisible.Checked = false;
                FormReflection.InvokeHandler(form, "checkBoxVisible_CheckedChanged", checkBoxVisible);
                Assert.IsFalse(child.Visible);

                checkBoxVisible.Checked = true;
                FormReflection.InvokeHandler(form, "checkBoxVisible_CheckedChanged", checkBoxVisible);
                Assert.IsTrue(child.Visible);

                child.Dispose();
            }
        }

        [TestMethod]
        public void 移動距離トラックバーの値がラベルに反映される()
        {
            using (var form = new Form1())
            {
                var trackBarMoveDistance = FormReflection.GetControl(form, "trackBarMoveDistance");
                trackBarMoveDistance.GetType().GetProperty("Value").SetValue(trackBarMoveDistance, 42, null);

                FormReflection.InvokeHandler(form, "trackBarMoveDistance_Scroll", trackBarMoveDistance);

                Control label = FormReflection.GetControl(form, "labelMoveDistanceValue");
                Assert.AreEqual("42", label.Text);

                ((Form)FormReflection.GetField(form, "child")).Dispose();
            }
        }

        [TestMethod]
        public void サイズ変更で子ウィンドウの幅高さが更新される()
        {
            using (var form = new Form1())
            {
                var child = (Form)FormReflection.GetField(form, "child");
                var trackBarWindowWidth = FormReflection.GetControl(form, "trackBarWindowWidth");
                var trackBarWindowHeight = FormReflection.GetControl(form, "trackBarWindowHeight");
                trackBarWindowWidth.GetType().GetProperty("Value").SetValue(trackBarWindowWidth, 300, null);
                trackBarWindowHeight.GetType().GetProperty("Value").SetValue(trackBarWindowHeight, 200, null);

                FormReflection.InvokeHandler(form, "updateRectSize", trackBarWindowWidth);

                Assert.AreEqual(300, child.Width);
                Assert.AreEqual(200, child.Height);

                child.Dispose();
            }
        }

        [TestMethod]
        public void 色変更で子ウィンドウの背景色が更新される()
        {
            using (var form = new Form1())
            {
                var child = (Form)FormReflection.GetField(form, "child");
                var trackBarRed = FormReflection.GetControl(form, "trackBarWindowColorRed");
                var trackBarGreen = FormReflection.GetControl(form, "trackBarWindowColorGreen");
                var trackBarBlue = FormReflection.GetControl(form, "trackBarWindowColorBlue");
                trackBarRed.GetType().GetProperty("Value").SetValue(trackBarRed, 10, null);
                trackBarGreen.GetType().GetProperty("Value").SetValue(trackBarGreen, 20, null);
                trackBarBlue.GetType().GetProperty("Value").SetValue(trackBarBlue, 30, null);

                FormReflection.InvokeHandler(form, "updateColor", trackBarRed);

                Assert.AreEqual(Color.FromArgb(10, 20, 30), child.BackColor);

                child.Dispose();
            }
        }
    }
}
