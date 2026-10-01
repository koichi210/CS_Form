using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Thumbnail.Tests
{
    /// <summary>
    /// Form1（画像のプレビュー/サムネイル表示サンプル）のテスト。
    ///
    /// ⚠️ button_Exe_Click はファイルが存在しないとMessageBox.Showを呼ぶため、
    /// テストでは常に実在するファイルを指定して回避する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "ThumbnailTests_" + Guid.NewGuid().ToString("N"));
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
        public void 実行ボタンでプレビューとサムネイルがコントロールのサイズで生成される()
        {
            string imagePath = Path.Combine(tempDirectory, "sample.png");
            using (var bmp = new Bitmap(400, 300))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Blue);
                }
                bmp.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_FilePath", imagePath);

                FormReflection.InvokeHandler(form, "button_Exe_Click", form);

                var pictureBoxImage = (PictureBox)FormReflection.GetControl(form, "pictureBox_Image");
                var pictureBoxThumbnail = (PictureBox)FormReflection.GetControl(form, "pictureBox_Thumbnail");

                Assert.IsNotNull(pictureBoxImage.Image);
                Assert.AreEqual(pictureBoxImage.Width, pictureBoxImage.Image.Width);
                Assert.AreEqual(pictureBoxImage.Height, pictureBoxImage.Image.Height);

                Assert.IsNotNull(pictureBoxThumbnail.Image);
                Assert.AreEqual(pictureBoxThumbnail.Width, pictureBoxThumbnail.Image.Width);
                Assert.AreEqual(pictureBoxThumbnail.Height, pictureBoxThumbnail.Image.Height);
            }
        }
    }
}
