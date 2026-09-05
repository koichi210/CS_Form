using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageViewer.Tests
{
    /// <summary>
    /// PreView（フォルダ内の画像をリストアップして表示するロジック。Form非依存の
    /// public class）のテスト。
    ///
    /// ⚠️ View() は無効なフォルダパスを渡すと MessageBox.Show を呼ぶため、
    /// テストでは常に実在するフォルダを渡す。
    /// </summary>
    [TestClass]
    public class PreViewTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "ImageViewerPreViewTests_" + Guid.NewGuid().ToString("N"));
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

        private void CreatePng(string fileName)
        {
            using (var bmp = new Bitmap(4, 4))
            {
                bmp.Save(Path.Combine(tempDirectory, fileName));
            }
        }

        [TestMethod]
        public void Viewは対象拡張子の画像をすべてリストへ追加する()
        {
            CreatePng("a.png");
            CreatePng("b.png");
            File.WriteAllText(Path.Combine(tempDirectory, "note.txt"), "dummy"); // 対象外拡張子

            var pv = new PreView();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, tempDirectory, "*.png");

                Assert.AreEqual(2, listView.Items.Count, "png2枚だけが対象になるはず");
                Assert.AreEqual(2, imageList.Images.Count);
            }
        }

        [TestMethod]
        public void IsSampleがtrueなら1枚だけ表示する()
        {
            CreatePng("a.png");
            CreatePng("b.png");
            CreatePng("c.png");

            var pv = new PreView();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, tempDirectory, "*.png", true);

                Assert.AreEqual(1, listView.Items.Count, "サンプル表示は先頭1枚だけのはず");
            }
        }

        [TestMethod]
        public void SetSizeで指定した大きさがImageListに反映される()
        {
            CreatePng("a.png");

            var pv = new PreView();
            pv.SetSize(64, 48);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, tempDirectory, "*.png");

                Assert.AreEqual(64, imageList.ImageSize.Width);
                Assert.AreEqual(48, imageList.ImageSize.Height);
            }
        }

        [TestMethod]
        public void 対象拡張子の画像が無ければリストは空になる()
        {
            File.WriteAllText(Path.Combine(tempDirectory, "note.txt"), "dummy");

            var pv = new PreView();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, tempDirectory, "*.png");

                Assert.AreEqual(0, listView.Items.Count);
            }
        }
    }
}
