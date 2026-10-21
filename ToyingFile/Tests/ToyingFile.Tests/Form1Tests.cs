using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToyingFile.Tests
{
    /// <summary>
    /// Form1（複数ファイルから指定文字列を含む行を処理するツール）のテスト。
    ///
    /// ⚠️ textBox_Directory が空だと MessageBox.Show を呼ぶため、テストでは
    /// 常に実在するディレクトリを指定する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "ToyingFileTests_" + Guid.NewGuid().ToString("N"));
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

        [TestMethod]
        public void 実行ボタンで対象フォルダ内のファイルから文字列が削除される()
        {
            string filePath = Path.Combine(tempDirectory, "a.txt");
            File.WriteAllText(filePath, "keep" + Environment.NewLine + "delete SECRET here");

            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Directory", tempDirectory);
                FormReflection.SetText(form, "textBox_DeleteString", "SECRET");
                FormReflection.SetChecked(form, "radioButton_DeleteString", true);
                FormReflection.SetChecked(form, "checkBox_CaseSensitive", true);

                FormReflection.InvokeHandler(form, "button_Execute_Click");
            }

            string result = File.ReadAllText(filePath);
            Assert.AreEqual("keep" + Environment.NewLine + "delete  here", result);
        }

        [TestMethod]
        public void SubDirectoryチェックで再帰的に対象ファイルを見つける()
        {
            string subDir = Path.Combine(tempDirectory, "sub");
            Directory.CreateDirectory(subDir);
            string nestedFile = Path.Combine(subDir, "b.txt");
            File.WriteAllText(nestedFile, "delete TARGET word");

            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Directory", tempDirectory);
                FormReflection.SetText(form, "textBox_DeleteString", "TARGET");
                FormReflection.SetChecked(form, "checkBox_SubDirectory", true);
                FormReflection.SetChecked(form, "radioButton_DeleteString", true);
                FormReflection.SetChecked(form, "checkBox_CaseSensitive", true);

                FormReflection.InvokeHandler(form, "button_Execute_Click");
            }

            Assert.AreEqual("delete  word", File.ReadAllText(nestedFile));
        }

        [TestMethod]
        public void 削除する文字列が無かったファイルは書き戻さない()
        {
            // 以前は一致しなかったファイルもShift_JISで書き直していたため、UTF-8の日本語やバイナリが化けていた
            string hitFile = Path.Combine(tempDirectory, "hit.txt");
            string utf8File = Path.Combine(tempDirectory, "utf8.txt");
            byte[] utf8Bytes = new UTF8Encoding(false).GetBytes("日本語だけの行" + Environment.NewLine + "対象なし");
            File.WriteAllText(hitFile, "x TARGET y");
            File.WriteAllBytes(utf8File, utf8Bytes);

            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_Directory", tempDirectory);
                FormReflection.SetText(form, "textBox_DeleteString", "TARGET");
                FormReflection.SetChecked(form, "radioButton_DeleteString", true);
                FormReflection.SetChecked(form, "checkBox_CaseSensitive", true);

                FormReflection.InvokeHandler(form, "button_Execute_Click");
            }

            Assert.AreEqual("x  y", File.ReadAllText(hitFile));
            CollectionAssert.AreEqual(utf8Bytes, File.ReadAllBytes(utf8File), "一致しなかったファイルはバイト列ごとそのまま");
        }
    }
}
