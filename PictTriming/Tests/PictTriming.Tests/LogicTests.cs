using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PictTriming.Tests
{
    /// <summary>
    /// Logic（MainWindow.xaml.cs から切り出した、画像トリミングと設定値のXML保存/
    /// 読み込みロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "PictTrimingTests_" + Guid.NewGuid().ToString("N"));
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

        private string CreateSolidColorBmp(string fileName, int width, int height, Color color)
        {
            string path = Path.Combine(tempDirectory, fileName);
            using (var bmp = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(color);
                }
                bmp.Save(path);
            }
            return path;
        }

        [TestMethod]
        public void Trimingは指定範囲を切り取って保存する()
        {
            string sourcePath = CreateSolidColorBmp("source.bmp", 100, 100, Color.Blue);
            string targetPath = Path.Combine(tempDirectory, "target.bmp");

            Logic.Triming(targetPath, sourcePath, BaseX: 10, BaseY: 10, Target_Width: 30, Target_Height: 30);

            using (var result = new Bitmap(targetPath))
            {
                Assert.AreEqual(30, result.Width);
                Assert.AreEqual(30, result.Height);
                Assert.AreEqual(Color.Blue.ToArgb(), result.GetPixel(5, 5).ToArgb());
            }
        }

        [TestMethod]
        public void 保存した設定を読み込むと同じ値が復元される()
        {
            string path = Path.Combine(tempDirectory, "settings.xml");

            Logic.SaveSettingXml(path, @"C:\tmp", "10", "20", "100", "200");

            Logic.Settings settings = Logic.LoadSettingXml(path);

            Assert.IsNotNull(settings);
            Assert.AreEqual(@"C:\tmp", settings.SourceFolderPath);
            Assert.AreEqual("10", settings.BaseX);
            Assert.AreEqual("20", settings.BaseY);
            Assert.AreEqual("100", settings.TargetX);
            Assert.AreEqual("200", settings.TargetY);
        }

        [TestMethod]
        public void 設定ファイルが存在しなければnullを返す()
        {
            string path = Path.Combine(tempDirectory, "notfound.xml");

            Logic.Settings settings = Logic.LoadSettingXml(path);

            Assert.IsNull(settings);
        }
    }
}
