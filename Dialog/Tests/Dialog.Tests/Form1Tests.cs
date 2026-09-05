using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dialog.Tests
{
    /// <summary>
    /// Form1 / DialogDynamic / FormStatic（モーダル・モードレスダイアログのサンプル）
    /// のテスト。
    ///
    /// ⚠️ button_Click / button_static_modal_Click / button_static_modeless_Click は
    /// 実際にダイアログをShowDialog()/Show()で表示する。ShowDialog()は誰かが閉じる
    /// までブロックするためテストがハングしてしまう。これらのクリックハンドラは
    /// テスト対象から除外し、ダイアログを「表示せず生成しただけ」の状態で
    /// プロパティ設定を検証する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void コンストラクタで動的ボタンが生成されControlsに追加される()
        {
            using (var form = new Form1())
            {
                Button dynamicButton = null;
                foreach (Control c in form.Controls)
                {
                    if (c is Button button && button.Text == "DialogDynamic")
                    {
                        dynamicButton = button;
                        break;
                    }
                }

                Assert.IsNotNull(dynamicButton);
                Assert.AreEqual(new Point(20, 20), dynamicButton.Location);
            }
        }

        [TestMethod]
        public void DialogDynamicは表示せずに生成しただけならFixedDialogとして構成される()
        {
            using (var dd = new DialogDynamic())
            {
                Assert.AreEqual("DialogDynamic", dd.Text);
                Assert.IsFalse(dd.MaximizeBox);
                Assert.IsFalse(dd.MinimizeBox);
                Assert.IsFalse(dd.ShowInTaskbar);
                Assert.AreEqual(FormBorderStyle.FixedDialog, dd.FormBorderStyle);
                Assert.AreEqual(FormStartPosition.CenterParent, dd.StartPosition);
            }
        }

        [TestMethod]
        public void FormStaticは表示せずに生成できる()
        {
            using (var fs = new FormStatic())
            {
                Assert.IsNotNull(fs);
            }
        }
    }
}
