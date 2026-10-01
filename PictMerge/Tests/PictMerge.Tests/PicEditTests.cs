using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Picture;

namespace PictMerge.Tests
{
    /// <summary>
    /// PicEdit（PicEdit.cs、Form/Windowとは独立した画像切り取り/合成クラス）のテスト。
    /// 実際の画像ファイルを使い、描画結果のピクセルを確認することで検証する。
    /// </summary>
    [TestClass]
    public class PicEditTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "PictMergeTests_" + Guid.NewGuid().ToString("N"));
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
        public void TrimExecは指定範囲を切り取ってキャンバスに貼り付ける()
        {
            string sourcePath = CreateSolidColorBmp("source.bmp", 100, 100, Color.Blue);

            var edit = new PicEdit(50, 50);
            edit.TrimExec(sourcePath, new Rectangle(0, 0, 50, 50), new Point(0, 0));

            string savedPath = Path.Combine(tempDirectory, "result.bmp");
            edit.SaveCanvas(savedPath);

            using (var result = new Bitmap(savedPath))
            {
                Assert.AreEqual(Color.Blue.ToArgb(), result.GetPixel(10, 10).ToArgb());
            }
        }

        [TestMethod]
        public void MergeExecはCreateSourceImgで指定した画像から切り取って合成する()
        {
            string sourcePath = CreateSolidColorBmp("source.bmp", 100, 100, Color.Red);

            var edit = new PicEdit(50, 50);
            edit.CreateSourceImg(sourcePath);
            edit.MergeExec(new Rectangle(0, 0, 50, 50));
            edit.ReleaseSourceImg();

            string savedPath = Path.Combine(tempDirectory, "merged.bmp");
            edit.SaveCanvas(savedPath);

            using (var result = new Bitmap(savedPath))
            {
                Assert.AreEqual(Color.Red.ToArgb(), result.GetPixel(10, 10).ToArgb());
            }
        }

        [TestMethod]
        public void 既存ファイルからPicEditを生成すると同じ内容がキャンバスになる()
        {
            string sourcePath = CreateSolidColorBmp("source.bmp", 30, 30, Color.Yellow);

            var edit = new PicEdit(sourcePath);

            string savedPath = Path.Combine(tempDirectory, "copied.bmp");
            edit.SaveCanvas(savedPath);

            using (var result = new Bitmap(savedPath))
            {
                Assert.AreEqual(30, result.Width);
                Assert.AreEqual(Color.Yellow.ToArgb(), result.GetPixel(5, 5).ToArgb());
            }
        }

        // ---- IDisposable ----

        [TestMethod]
        public void IDisposableを実装していてusingで使える()
        {
            using (var edit = new PicEdit(4, 4))
            {
                Assert.IsInstanceOfType(edit, typeof(IDisposable));
            }
        }

        [TestMethod]
        public void Disposeを2回呼んでも例外にならない()
        {
            string sourcePath = CreateSolidColorBmp("dispose_twice.bmp", 10, 10, Color.Blue);
            var edit = new PicEdit(10, 10);
            edit.CreateSourceImg(sourcePath);

            edit.Dispose();
            edit.Dispose();
        }

        [TestMethod]
        public void ReleaseSourceImgの後にDisposeしても例外にならない()
        {
            string sourcePath = CreateSolidColorBmp("release_then_dispose.bmp", 10, 10, Color.Blue);
            var edit = new PicEdit(10, 10);
            edit.CreateSourceImg(sourcePath);
            edit.ReleaseSourceImg();

            edit.Dispose();
        }

        [TestMethod]
        public void usingを抜けると画像ファイルのロックが外れる()
        {
            string basePath = CreateSolidColorBmp("lock.bmp", 10, 10, Color.Blue);
            using (var edit = new PicEdit(basePath))
            {
                edit.CreateSourceImg(basePath);
            }

            // Bitmap が解放されずにロックが残っていれば IOException になる
            File.Delete(basePath);
            Assert.IsFalse(File.Exists(basePath));
        }
    }
}
