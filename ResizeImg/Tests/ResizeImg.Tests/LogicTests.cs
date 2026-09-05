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
        public void Trimingは指定範囲を切り取り拡張子の前にnewを付けて保存する()
        {
            string sourcePath = CreateSolidColorBmp("sample.bmp", 100, 100, Color.Blue);

            Logic.Triming(sourcePath, BaseX: 10, BaseY: 10, Width: 30, Height: 30);

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
                BaseX: 0, BaseY: 0, Width: 50, Height: 50,
                PictureBox1Width: 100, PictureBox1Height: 100,
                PictureBox2Width: 80, PictureBox2Height: 80,
                OrgSizeCandidate1: 592, OrgSizeCandidate2: 312,
                SampleCanvas: out sample, OrgCanvas: out org);

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
            // OrgPictSizeの選択(大きい方)を切り出し範囲経由で間接的に確認する。
            // 300x300の画像に対しOrgSizeCandidate2(250)の方が大きいので、
            // 250x250の範囲がオリジナル側の切り出し元になる。
            string sourcePath = CreateSolidColorBmp("sample.bmp", 300, 300, Color.Green);

            Bitmap sample, org;
            Logic.CreatePreviewImages(
                sourcePath,
                BaseX: 0, BaseY: 0, Width: 10, Height: 10,
                PictureBox1Width: 10, PictureBox1Height: 10,
                PictureBox2Width: 250, PictureBox2Height: 250,
                OrgSizeCandidate1: 100, OrgSizeCandidate2: 250,
                SampleCanvas: out sample, OrgCanvas: out org);

            using (sample)
            using (org)
            {
                // 250x250全体が緑色の画像からそのまま切り出されるので全域が緑になるはず
                Assert.AreEqual(Color.Green.ToArgb(), org.GetPixel(240, 240).ToArgb());
            }
        }
    }
}
