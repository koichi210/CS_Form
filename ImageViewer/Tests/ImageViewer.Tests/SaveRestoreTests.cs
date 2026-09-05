using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageViewer.Tests
{
    /// <summary>
    /// ImageViewer.SaveRestore（StcSaveRestore を継承した設定保存クラス）のテスト。
    ///
    /// Form のクラス名が名前空間と同じ "ImageViewer" のため、テストコード内では
    /// 常に完全修飾名 global::ImageViewer.ImageViewer でフォームの型を指す。
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

        private static global::ImageViewer.ImageViewer NewForm()
        {
            return new global::ImageViewer.ImageViewer();
        }

        [TestMethod]
        public void フォルダパスと拡張子の指定が保存して読み直すと戻る()
        {
            using (global::ImageViewer.ImageViewer writer = NewForm())
            {
                writer.textBox_FolderPath.Text = @"D:\photos";
                writer.textBox_Extension.Text = "*.jpg";

                var sr = new SaveRestore();
                sr.RegistItem(writer);
                string path = Path.Combine(tempDirectory, "setting.xml");
                Assert.IsTrue(sr.SaveXmlFile(path));

                using (global::ImageViewer.ImageViewer reader = NewForm())
                {
                    var readerSr = new SaveRestore();
                    readerSr.RegistItem(reader);
                    Assert.IsTrue(readerSr.LoadXmlFile(path));

                    Assert.AreEqual(@"D:\photos", reader.textBox_FolderPath.Text);
                    Assert.AreEqual("*.jpg", reader.textBox_Extension.Text);
                }
            }
        }
    }
}
