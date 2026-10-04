using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageViewer.Tests
{
    /// <summary>
    /// Preview（フォルダ内の画像をリストアップして表示するロジック。Form非依存の
    /// public class）のテスト。
    ///
    /// ⚠️ View() は無効なフォルダパスを渡すと MessageBox.Show を呼ぶため、
    /// テストでは常に実在するフォルダを渡す。
    /// </summary>
    [TestClass]
    public class PreviewTests
    {
        private string _tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "ImageViewerPreviewTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDirectory);
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                if (Directory.Exists(_tempDirectory)) Directory.Delete(_tempDirectory, true);
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
                bmp.Save(Path.Combine(_tempDirectory, fileName));
            }
        }

        [TestMethod]
        public void Viewは対象拡張子の画像をすべてリストへ追加する()
        {
            CreatePng("a.png");
            CreatePng("b.png");
            File.WriteAllText(Path.Combine(_tempDirectory, "note.txt"), "dummy"); // 対象外拡張子

            var pv = new Preview();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, _tempDirectory, "*.png");

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

            var pv = new Preview();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, _tempDirectory, "*.png", true);

                Assert.AreEqual(1, listView.Items.Count, "サンプル表示は先頭1枚だけのはず");
            }
        }

        [TestMethod]
        public void SetSizeで指定した大きさがImageListに反映される()
        {
            CreatePng("a.png");

            var pv = new Preview();
            pv.SetSize(64, 48);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, _tempDirectory, "*.png");

                Assert.AreEqual(64, imageList.ImageSize.Width);
                Assert.AreEqual(48, imageList.ImageSize.Height);
            }
        }

        [TestMethod]
        public void 対象拡張子の画像が無ければリストは空になる()
        {
            File.WriteAllText(Path.Combine(_tempDirectory, "note.txt"), "dummy");

            var pv = new Preview();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, _tempDirectory, "*.png");

                Assert.AreEqual(0, listView.Items.Count);
            }
        }

        // 回帰テスト: isSample=trueで該当ファイルが0件だと、以前は files[0] を読もうとして例外になっていた
        [TestMethod]
        public void IsSampleがtrueでも対象画像が無ければ例外にならず空になる()
        {
            File.WriteAllText(Path.Combine(_tempDirectory, "note.txt"), "dummy");

            var pv = new Preview();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, _tempDirectory, "*.png", true);

                Assert.AreEqual(0, listView.Items.Count);
            }
        }

        // 回帰テスト: 以前はImageListを消さずに追加していたため、表示し直すたびに画像が溜まり、
        // 項目のImageIndexが前回の古い画像を指していた
        [TestMethod]
        public void 表示し直してもImageListに前回の画像が残らない()
        {
            CreatePng("a.png");
            CreatePng("b.png");

            var pv = new Preview();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, _tempDirectory, "*.png");
                pv.View(imageList, listView, _tempDirectory, "*.png", true);

                Assert.AreEqual(1, listView.Items.Count);
                Assert.AreEqual(1, imageList.Images.Count);
            }
        }
    }
}
