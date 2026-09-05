using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeleteDuplicateElement.Tests
{
    /// <summary>
    /// Form1（重複行削除ツール）のテスト。
    ///
    /// このプロジェクトのロジックはすべて共通クラス(StcUtils)呼び出しの薄いラッパーで、
    /// 独自のロジックは無い。呼び出し先(TrimDuplication等)は _Common 側で
    /// テスト済みのため、ここでは「ボタンを押すと実際に重複除去が動くか」という
    /// 配線を、実際の Form を生成してリフレクション経由で検証する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void 実行ボタンで重複行が除去される()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Source", "a" + Environment.NewLine + "b" + Environment.NewLine + "a");

                FormReflection.InvokeHandler(form, "button_Execute_Click");

                Assert.AreEqual("a" + Environment.NewLine + "b", FormReflection.GetText(form, "textBox_Dest"));
            }
        }

        [TestMethod]
        public void 重複が無ければそのまま出力される()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Source", "a" + Environment.NewLine + "b" + Environment.NewLine + "c");

                FormReflection.InvokeHandler(form, "button_Execute_Click");

                Assert.AreEqual("a" + Environment.NewLine + "b" + Environment.NewLine + "c", FormReflection.GetText(form, "textBox_Dest"));
            }
        }

        [TestMethod]
        public void 空文字なら空文字のまま()
        {
            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Source", "");

                FormReflection.InvokeHandler(form, "button_Execute_Click");

                Assert.AreEqual("", FormReflection.GetText(form, "textBox_Dest"));
            }
        }

        [TestMethod]
        public void ドラッグドロップでファイルパス一覧が改行区切りで入力欄へ入る()
        {
            using (var form = new Form1())
            {
                string[] files = { @"C:\a.txt", @"C:\b.txt" };
                var data = new DataObject();
                data.SetData(typeof(String), files);
                var dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

                FormReflection.InvokeHandler(form, "textBox_Source_DragDrop", null, dragEventArgs);

                Assert.AreEqual(@"C:\a.txt" + Environment.NewLine + @"C:\b.txt", FormReflection.GetText(form, "textBox_Source"));
            }
        }
    }
}
