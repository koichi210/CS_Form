using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArranger.Tests
{
    /// <summary>
    /// フォルダ名変更タブ(rd)のリネームのテスト。実際に一時フォルダを作り、ボタンのClickハンドラを呼んで確認する。
    /// フォームを表示していないとButton.PerformClickは何もしないので、privateのハンドラをリフレクションで直接呼ぶ。
    /// (クラス名がフォームと同じ "FileArranger" なので、フォームの型は完全修飾名 global::FileArranger.FileArranger で指す)
    /// </summary>
    [TestClass]
    public class RenameDirectoryTabTests
    {
        private const int _renameDestIdx = 1;

        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "FileArrangerRenameDirectoryTabTests_" + Guid.NewGuid().ToString("N"));
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

        // 本物のデータフォルダ(%LOCALAPPDATA%\FileArranger)のプロファイルを読まないよう、一時フォルダの中をデータフォルダにする
        private global::FileArranger.FileArranger NewForm()
        {
            string dataFolder = Path.Combine(tempDirectory, "data");
            Directory.CreateDirectory(dataFolder);
            return new global::FileArranger.FileArranger(dataFolder);
        }

        // 格納元の直下に1つだけフォルダを作ってリストアップし、[変更後]をdestNameにしてリネームを実行する
        private string[] RenameOnlyFolder(string srcName, string destName, bool restore = false)
        {
            string parent = Path.Combine(tempDirectory, "parent");
            Directory.CreateDirectory(Path.Combine(parent, srcName));

            using (global::FileArranger.FileArranger form = NewForm())
            {
                form.rd_comboBox_RenameDir.Text = parent;
                InvokeHandler(form, "rd_button_Listup_Target_Click");
                form.rd_listView_Target.Items[0].Selected = true;
                form.rd_listView_Target.Items[0].SubItems[_renameDestIdx].Text = destName;

                InvokeHandler(form, "rd_button_Execute_Rename_Click");
                if (restore)
                {
                    InvokeHandler(form, "rd_button_RenameFolderRestore_Click");
                }
            }

            return Directory.GetDirectories(parent).Select(Path.GetFileName).ToArray();
        }

        [TestMethod]
        public void 変更後が今と同じ名前なら何もしない()
        {
            // 以前は自分自身と重複したとみなして、連番付きの名前にリネームしていた
            CollectionAssert.AreEqual(new[] { "Folder01" }, RenameOnlyFolder("Folder01", "Folder01"));
        }

        [TestMethod]
        public void 大文字小文字だけ違う名前ならその名前に変える()
        {
            CollectionAssert.AreEqual(new[] { "FOLDER01" }, RenameOnlyFolder("folder01", "FOLDER01"));
        }

        [TestMethod]
        public void 大文字小文字だけ変えたリネームも復元できる()
        {
            CollectionAssert.AreEqual(new[] { "folder01" }, RenameOnlyFolder("folder01", "FOLDER01", true));
        }

        [TestMethod]
        public void 別の名前ならその名前に変える()
        {
            CollectionAssert.AreEqual(new[] { "Folder02" }, RenameOnlyFolder("Folder01", "Folder02"));
        }
    }
}
