using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DropDown.Tests
{
    /// <summary>
    /// Form1（ドラッグ&ドロップされたファイルパス一覧を表示するツール）のテスト。
    /// ロジックは共通クラス(StcUtils.GetDropListLinear)の呼び出しのみ。
    /// production コードは変更せず、リフレクションで private な部分を検証する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        private static DragEventArgs CreateFileDropArgs(params string[] files)
        {
            var data = new DataObject();
            data.SetData(DataFormats.FileDrop, files);
            return new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);
        }

        [TestMethod]
        public void ドロップしたファイル一覧が改行区切りで表示される()
        {
            using (var form = new Form1())
            {
                var e = CreateFileDropArgs(@"C:\a.txt", @"C:\b.txt");

                FormReflection.InvokeHandler(form, "textBox1_DragDrop", null, e);

                Assert.AreEqual(@"C:\a.txt" + Environment.NewLine + @"C:\b.txt", FormReflection.GetText(form, "textBox1"));
            }
        }

        [TestMethod]
        public void ファイル以外のドラッグ内容だと空欄になる()
        {
            using (var form = new Form1())
            {
                var data = new DataObject();
                data.SetData(DataFormats.Text, "not a file");
                var e = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

                FormReflection.InvokeHandler(form, "textBox1_DragDrop", null, e);

                Assert.AreEqual("", FormReflection.GetText(form, "textBox1"));
            }
        }

        [TestMethod]
        public void DragEnterを呼んでも例外にならない()
        {
            using (var form = new Form1())
            {
                var e = CreateFileDropArgs(@"C:\a.txt");

                FormReflection.InvokeHandler(form, "textBox1_DragEnter", null, e);
            }
        }
    }
}
