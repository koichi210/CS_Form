using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Picture;

namespace PictMerge2.Tests
{
    /// <summary>
    /// PicEdit / PicEditCustom（PicEdit.cs、Windowとは独立した画像切り取り/合成
    /// クラス）のテスト。実際の画像ファイルを使い、描画結果のピクセルを確認する
    /// ことで検証する。
    ///
    /// ⚠️ PicEdit のデストラクタは m_SourceImg.Dispose() を無条件に呼ぶため
    /// (CreateSourceImgを一度も呼んでいないとm_SourceImgがnullのままで、
    /// ファイナライザスレッドでNullReferenceExceptionが発生しプロセスが
    /// クラッシュしかねない)、テストで生成するPicEditは必ずCreateSourceImgを
    /// 呼んでm_SourceImgを非nullにしてから使う。
    /// </summary>
    [TestClass]
    public class PicEditTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "PictMerge2Tests_" + Guid.NewGuid().ToString("N"));
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
            edit.CreateSourceImg(sourcePath); // ファイナライザのNRE回避のため必ず呼ぶ
            edit.TrimExec(sourcePath, new Rectangle(0, 0, 50, 50), new Point(0, 0));

            string savedPath = Path.Combine(tempDirectory, "result.bmp");
            edit.SaveCanvas(savedPath);
            edit.ReleaseSourceImg();

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

            string savedPath = Path.Combine(tempDirectory, "merged.bmp");
            edit.SaveCanvas(savedPath);
            edit.ReleaseSourceImg();

            using (var result = new Bitmap(savedPath))
            {
                Assert.AreEqual(Color.Red.ToArgb(), result.GetPixel(10, 10).ToArgb());
            }
        }

        [TestMethod]
        public void MergeExecはPutParamを指定した位置に貼り付ける()
        {
            string sourcePath = CreateSolidColorBmp("source.bmp", 100, 100, Color.Lime);

            var edit = new PicEdit(100, 100);
            edit.CreateSourceImg(sourcePath);
            edit.MergeExec(new Rectangle(0, 0, 20, 20), new Point(60, 60));

            string savedPath = Path.Combine(tempDirectory, "merged_offset.bmp");
            edit.SaveCanvas(savedPath);
            edit.ReleaseSourceImg();

            using (var result = new Bitmap(savedPath))
            {
                Assert.AreEqual(Color.Lime.ToArgb(), result.GetPixel(65, 65).ToArgb());
            }
        }

        [TestMethod]
        public void PicEditCustomはDestWidthプロパティを保持する()
        {
            string sourcePath = CreateSolidColorBmp("source.bmp", 10, 10, Color.Black);

            var custom = new PicEditCustom(10, 10);
            custom.CreateSourceImg(sourcePath); // ファイナライザのNRE回避のため必ず呼ぶ
            custom.DestWidth = 123;

            Assert.AreEqual(123, custom.DestWidth);

            custom.ReleaseSourceImg();
        }
    }
}
