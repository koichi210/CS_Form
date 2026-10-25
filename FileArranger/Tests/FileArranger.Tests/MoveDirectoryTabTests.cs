using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArranger.Tests
{
    /// <summary>
    /// フォルダ移動タブ(md)の削除のテスト。実際に一時フォルダを作り、ボタンのClickハンドラを呼んで確認する。
    /// フォームを表示していないとButton.PerformClickは何もしないので、privateのハンドラをリフレクションで直接呼ぶ。
    ///
    /// クラス名がフォームと同じ "FileArranger"（名前空間も FileArranger）なので、
    /// このテストファイル内では常に完全修飾名 global::FileArranger.FileArranger で
    /// フォームの型を指す。
    /// </summary>
    [TestClass]
    public class MoveDirectoryTabTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "FileArrangerMoveDirectoryTabTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                if (Directory.Exists(tempDirectory)) Directory.Delete(tempDirectory, true);
            }
            catch (IOException)
            {
                // 後片付けの失敗はテストの成否に関係ないので黙って流す
            }
        }

        private static void InvokeHandler(global::FileArranger.FileArranger form, string handlerName)
        {
            MethodInfo method = typeof(global::FileArranger.FileArranger).GetMethod(handlerName, BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, new object[] { form, EventArgs.Empty });
        }

        [TestMethod]
        public void 親フォルダと子フォルダを両方選んで削除しても例外にならず両方消える()
        {
            string source = Path.Combine(tempDirectory, "source");
            string target = Path.Combine(tempDirectory, "target");
            Directory.CreateDirectory(Path.Combine(source, @"A\B"));
            Directory.CreateDirectory(target);
            File.WriteAllText(Path.Combine(source, @"A\a.txt"), "dummy");
            File.WriteAllText(Path.Combine(source, @"A\B\b.txt"), "dummy");
            File.WriteAllText(Path.Combine(source, @"keep.txt"), "dummy");

            using (global::FileArranger.FileArranger form = new global::FileArranger.FileArranger())
            {
                form.md_textBox_SourceDir.Text = source;
                form.md_comboBox_TargetDir.Text = target;
                InvokeHandler(form, "md_button_Listup_Click");
                // 前提: 親(A)と子(A\B)が両方一覧に出ている
                Assert.AreEqual(2, form.md_listBox_Listup.Items.Count);

                for (int i = 0; i < form.md_listBox_Listup.Items.Count; i++)
                {
                    form.md_listBox_Listup.SetSelected(i, true);
                }
                form.ConfirmDelete = text => true;
                InvokeHandler(form, "md_button_Delete_Click");

                Assert.IsFalse(Directory.Exists(Path.Combine(source, "A")));
                Assert.IsTrue(File.Exists(Path.Combine(source, "keep.txt")), "選んでいないものは消さない");
                Assert.AreEqual(0, form.md_listBox_Listup.Items.Count);
            }
        }

        [TestMethod]
        public void 削除は確認を出し_いいえなら消さない()
        {
            string source = Path.Combine(tempDirectory, "source");
            string target = Path.Combine(tempDirectory, "target");
            Directory.CreateDirectory(Path.Combine(source, "A"));
            Directory.CreateDirectory(target);
            File.WriteAllText(Path.Combine(source, @"A\a.txt"), "dummy");

            using (global::FileArranger.FileArranger form = new global::FileArranger.FileArranger())
            {
                form.md_textBox_SourceDir.Text = source;
                form.md_comboBox_TargetDir.Text = target;
                InvokeHandler(form, "md_button_Listup_Click");
                form.md_listBox_Listup.SetSelected(0, true);

                string asked = null;
                form.ConfirmDelete = text => { asked = text; return false; };
                InvokeHandler(form, "md_button_Delete_Click");

                Assert.IsNotNull(asked, "削除前に確認を出す");
                StringAssert.Contains(asked, "1個のフォルダを中身ごと削除します");
                Assert.IsTrue(File.Exists(Path.Combine(source, @"A\a.txt")), "いいえなら消さない");
            }
        }

        [TestMethod]
        public void リストアップ後に格納元の欄を書き換えても_リストアップしたフォルダの中を操作する()
        {
            // 以前は操作時の欄の値＋前の一覧の名前で操作していたため、別のフォルダの同名フォルダを消していた
            string source = Path.Combine(tempDirectory, "source");
            string other = Path.Combine(tempDirectory, "other");
            string target = Path.Combine(tempDirectory, "target");
            Directory.CreateDirectory(Path.Combine(source, "A"));
            Directory.CreateDirectory(Path.Combine(other, "A"));
            Directory.CreateDirectory(target);
            File.WriteAllText(Path.Combine(source, @"A\a.txt"), "dummy");
            File.WriteAllText(Path.Combine(other, @"A\a.txt"), "dummy");

            using (global::FileArranger.FileArranger form = new global::FileArranger.FileArranger())
            {
                form.md_textBox_SourceDir.Text = source;
                form.md_comboBox_TargetDir.Text = target;
                InvokeHandler(form, "md_button_Listup_Click");
                form.md_listBox_Listup.SetSelected(0, true);

                form.md_textBox_SourceDir.Text = other;
                form.ConfirmDelete = text => true;
                InvokeHandler(form, "md_button_Delete_Click");

                Assert.IsFalse(Directory.Exists(Path.Combine(source, "A")), "一覧に出ていたフォルダを消す");
                Assert.IsTrue(Directory.Exists(Path.Combine(other, "A")), "書き換えた欄のフォルダには触らない");
            }
        }
    }
}
