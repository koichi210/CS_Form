using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageViewer.Tests
{
    /// <summary>
    /// ImageViewer.SaveRestore（StcSaveRestore を継承した設定保存クラス）のテスト。
    /// </summary>
    [TestClass]
    public class SaveRestoreTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "ImageViewerSaveRestoreTests_" + Guid.NewGuid().ToString("N"));
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

        private static Form1 NewForm()
        {
            return new Form1();
        }

        [TestMethod]
        public void フォルダパスと拡張子の指定が保存して読み直すと戻る()
        {
            using (Form1 writer = NewForm())
            {
                writer.textBox_FolderPath.Text = @"D:\photos";
                writer.textBox_Extension.Text = "*.jpg";

                var sr = new SaveRestore();
                sr.RegisterItem(writer);
                string path = Path.Combine(tempDirectory, "setting.xml");
                Assert.IsTrue(sr.SaveXmlFile(path));

                using (Form1 reader = NewForm())
                {
                    var readerSr = new SaveRestore();
                    readerSr.RegisterItem(reader);
                    Assert.IsTrue(readerSr.LoadXmlFile(path));

                    Assert.AreEqual(@"D:\photos", reader.textBox_FolderPath.Text);
                    Assert.AreEqual("*.jpg", reader.textBox_Extension.Text);
                }
            }
        }
    }
}
