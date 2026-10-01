using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResizeImg.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、画像の切り取り/プレビュー生成ロジック）の
    /// テスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "ResizeImgTests_" + Guid.NewGuid().ToString("N"));
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
        public void Trimmingは指定範囲を切り取り拡張子の前にnewを付けて保存する()
        {
            string sourcePath = CreateSolidColorBmp("sample.bmp", 100, 100, Color.Blue);

            Logic.TrimImage(sourcePath, baseX: 10, baseY: 10, width: 30, height: 30);

            string expectedTargetPath = sourcePath.Replace(".", "_new.");
            Assert.IsTrue(File.Exists(expectedTargetPath));

            using (var result = new Bitmap(expectedTargetPath))
            {
                Assert.AreEqual(30, result.Width);
                Assert.AreEqual(30, result.Height);
                Assert.AreEqual(Color.Blue.ToArgb(), result.GetPixel(5, 5).ToArgb());
            }
        }

        [TestMethod]
        public void CreatePreviewImagesは指定サイズのプレビューとオリジナルを生成する()
        {
            string sourcePath = CreateSolidColorBmp("sample.bmp", 200, 200, Color.Red);

            Bitmap sample, org;
            Logic.CreatePreviewImages(
                sourcePath,
                baseX: 0, baseY: 0, width: 50, height: 50,
                sampleWidth: 100, sampleHeight: 100,
                originalWidth: 80, originalHeight: 80,
                originalSizeCandidate1: 592, originalSizeCandidate2: 312,
                sampleImage: out sample, originalImage: out org);

            using (sample)
            using (org)
            {
                Assert.AreEqual(100, sample.Width);
                Assert.AreEqual(100, sample.Height);
                Assert.AreEqual(80, org.Width);
                Assert.AreEqual(80, org.Height);
                Assert.AreEqual(Color.Red.ToArgb(), sample.GetPixel(10, 10).ToArgb());
                Assert.AreEqual(Color.Red.ToArgb(), org.GetPixel(10, 10).ToArgb());
            }
        }

        [TestMethod]
        public void CreatePreviewImagesは大きい方のOrgSizeCandidateを採用する()
        {
            // originalPictSizeの選択(大きい方)を切り出し範囲経由で間接的に確認する。
            // 300x300の画像に対しoriginalSizeCandidate2(250)の方が大きいので、
            // 250x250の範囲がオリジナル側の切り出し元になる。
            string sourcePath = CreateSolidColorBmp("sample.bmp", 300, 300, Color.Green);

            Bitmap sample, org;
            Logic.CreatePreviewImages(
                sourcePath,
                baseX: 0, baseY: 0, width: 10, height: 10,
                sampleWidth: 10, sampleHeight: 10,
                originalWidth: 250, originalHeight: 250,
                originalSizeCandidate1: 100, originalSizeCandidate2: 250,
                sampleImage: out sample, originalImage: out org);

            using (sample)
            using (org)
            {
                // 250x250全体が緑色の画像からそのまま切り出されるので全域が緑になるはず
                Assert.AreEqual(Color.Green.ToArgb(), org.GetPixel(240, 240).ToArgb());
            }
        }
    }
}
