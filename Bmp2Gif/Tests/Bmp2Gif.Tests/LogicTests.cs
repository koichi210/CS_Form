using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bmp2Gif.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、BMP画像をGIFに変換するロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "Bmp2GifTests_" + Guid.NewGuid().ToString("N"));
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

        private string CreateSampleBmp(int width, int height)
        {
            string bmpPath = Path.Combine(tempDirectory, "sample.bmp");
            using (var bmp = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Blue);
                }
                bmp.Save(bmpPath, ImageFormat.Bmp);
            }
            return bmpPath;
        }

        [TestMethod]
        public void コメントなしでBmpをGif形式に変換する()
        {
            string srcPath = CreateSampleBmp(20, 20);
            string dstPath = Path.Combine(tempDirectory, "result.gif");

            Logic.ConvertBmpToGif(srcPath, dstPath, AddComment: false);

            Assert.IsTrue(File.Exists(dstPath));
            using (var result = Image.FromFile(dstPath))
            {
                Assert.AreEqual(ImageFormat.Gif.Guid, result.RawFormat.Guid);
                Assert.AreEqual(20, result.Width);
                Assert.AreEqual(20, result.Height);
            }
        }

        [TestMethod]
        public void コメントありでもGif形式に変換できる()
        {
            string srcPath = CreateSampleBmp(400, 100);
            string dstPath = Path.Combine(tempDirectory, "result_comment.gif");

            Logic.ConvertBmpToGif(srcPath, dstPath, AddComment: true);

            Assert.IsTrue(File.Exists(dstPath));
            using (var result = Image.FromFile(dstPath))
            {
                Assert.AreEqual(ImageFormat.Gif.Guid, result.RawFormat.Guid);
                Assert.AreEqual(400, result.Width);
                Assert.AreEqual(100, result.Height);
            }
        }

        [TestMethod]
        public void コメントありは元画像の左上に矩形と文字を焼き込む()
        {
            string srcPath = CreateSampleBmp(400, 100);
            string dstPath = Path.Combine(tempDirectory, "result_pixel.gif");

            Logic.ConvertBmpToGif(srcPath, dstPath, AddComment: true);

            using (var result = new Bitmap(dstPath))
            {
                // コメント矩形(OrangeRed)が描画された領域の色を確認
                Color pixel = result.GetPixel(5, 5);
                Assert.AreNotEqual(Color.Blue.ToArgb(), pixel.ToArgb());
            }
        }
    }
}
