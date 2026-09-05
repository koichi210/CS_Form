using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TabControl.Tests
{
    /// <summary>
    /// Form1（TabControlの操作サンプル）のテスト。
    ///
    /// ⚠️ tabControl1_Selected は e.TabPage が tabPage1/tabPage2 のとき
    /// MessageBox.Show を呼ぶため、テストではそれ以外(tabPage3)を指定して回避する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void ボタンクリックでtabPage1にボタンが追加される()
        {
            using (var form = new Form1())
            {
                var tabPage1 = (TabPage)FormReflection.GetControl(form, "tabPage1");
                int before = tabPage1.Controls.Count;

                FormReflection.InvokeHandler(form, "button1_Click", form);

                Assert.AreEqual(before + 1, tabPage1.Controls.Count);
                Assert.IsInstanceOfType(tabPage1.Controls[tabPage1.Controls.Count - 1], typeof(Button));
            }
        }

        [TestMethod]
        public void tabControl1_Selectedはpage1page2以外ならMessageBoxを表示しない()
        {
            using (var form = new Form1())
            {
                var tabControl1 = (System.Windows.Forms.TabControl)FormReflection.GetControl(form, "tabControl1");
                var tabPage3 = (TabPage)FormReflection.GetControl(form, "tabPage3");

                var args = new TabControlEventArgs(tabPage3, 2, TabControlAction.Selected);
                FormReflection.InvokeHandler(form, "tabControl1_Selected", tabControl1, args);

                // ここまで到達すればMessageBoxは呼ばれていない
                Assert.IsNotNull(form);
            }
        }

        [TestMethod]
        public void 空実装のハンドラは例外なく呼び出せる()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "tabPage1_Click", form);
                FormReflection.InvokeHandler(form, "tabPage2_Click", form);
                FormReflection.InvokeHandler(form, "tabPage3_Click", form);
                FormReflection.InvokeHandler(form, "button3_Click", form);
            }
        }
    }
}
