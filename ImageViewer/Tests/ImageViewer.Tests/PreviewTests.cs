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

        // 回帰テスト: 壊れた画像が1枚でもあると、以前はImage.FromFileの例外でそこから先が表示されなかった
        [TestMethod]
        public void 壊れた画像があってもサムネイル無しで並べて残りの画像も表示する()
        {
            CreatePng("a.png");
            File.WriteAllText(Path.Combine(_tempDirectory, "b.png"), "これは画像ではない");
            CreatePng("c.png");

            var pv = new Preview();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, _tempDirectory, "*.png");

                Assert.AreEqual(3, listView.Items.Count);
                Assert.AreEqual(2, imageList.Images.Count);
                Assert.AreEqual(0, listView.Items[0].ImageIndex);
                Assert.AreEqual(-1, listView.Items[1].ImageIndex);
                Assert.AreEqual(1, listView.Items[2].ImageIndex);
            }
        }

        [TestMethod]
        public void IsVideoFileは動画の拡張子だけをtrueにする()
        {
            Assert.IsTrue(Preview.IsVideoFile(@"C:\tmp\a.mp4"));
            Assert.IsTrue(Preview.IsVideoFile(@"C:\tmp\b.MOV"), "大文字の拡張子も動画として扱うはず");
            Assert.IsTrue(Preview.IsVideoFile(@"C:\tmp\c.avi"));
            Assert.IsTrue(Preview.IsVideoFile(@"C:\tmp\d.wmv"));
            Assert.IsTrue(Preview.IsVideoFile(@"C:\tmp\e.mkv"));

            Assert.IsFalse(Preview.IsVideoFile(@"C:\tmp\a.png"));
            Assert.IsFalse(Preview.IsVideoFile(@"C:\tmp\a.txt"));
            Assert.IsFalse(Preview.IsVideoFile(@"C:\tmp\a.ts"), "TypeScriptと被る.tsは動画扱いしない");
            Assert.IsFalse(Preview.IsVideoFile(@"C:\tmp\mp4"), "拡張子が無ければ動画ではない");
            Assert.IsFalse(Preview.IsVideoFile(null));
            Assert.IsFalse(Preview.IsVideoFile(""));
        }

        [TestMethod]
        public void GetTargetFilesはセミコロン区切りの複数パターンをまとめて名前順に返す()
        {
            CreatePng("b.png");
            File.WriteAllText(Path.Combine(_tempDirectory, "a.mp4"), "dummy");
            File.WriteAllText(Path.Combine(_tempDirectory, "c.mp4"), "dummy");
            File.WriteAllText(Path.Combine(_tempDirectory, "note.txt"), "dummy");

            string[] files = Preview.GetTargetFiles(_tempDirectory, " *.png ; *.mp4 ;");

            CollectionAssert.AreEqual(
                new[] { "a.mp4", "b.png", "c.mp4" },
                Array.ConvertAll(files, Path.GetFileName),
                "空白と空の区切りは無視し、画像と動画が名前順に混ざるはず");
        }

        [TestMethod]
        public void GetTargetFilesは同じファイルに当たるパターンが重なっても1回だけ返す()
        {
            CreatePng("a.png");

            string[] files = Preview.GetTargetFiles(_tempDirectory, "*.png;a.*");

            Assert.AreEqual(1, files.Length);
        }

        [TestMethod]
        public void GetTargetFilesはパターン1つなら従来どおりその条件だけで探す()
        {
            CreatePng("a.png");
            File.WriteAllText(Path.Combine(_tempDirectory, "b.mp4"), "dummy");

            string[] files = Preview.GetTargetFiles(_tempDirectory, "*.png");

            Assert.AreEqual(1, files.Length);
            Assert.AreEqual("a.png", Path.GetFileName(files[0]));
        }

        // 動画のサムネイルが取れない(中身が動画でない・コーデックが無い等)ときも、落ちずにサムネイル無しで並べる
        [TestMethod]
        public void サムネイルが取れない動画があっても例外にならずサムネイル無しで並べる()
        {
            CreatePng("a.png");
            File.WriteAllText(Path.Combine(_tempDirectory, "b.mp4"), "これは動画ではない");

            var pv = new Preview();
            pv.SetSize(32);

            using (var imageList = new ImageList())
            using (var listView = new ListView())
            {
                pv.View(imageList, listView, _tempDirectory, "*.png;*.mp4");

                Assert.AreEqual(2, listView.Items.Count);
                Assert.AreEqual(0, listView.Items[0].ImageIndex);
                Assert.AreEqual(-1, listView.Items[1].ImageIndex);
                Assert.AreEqual(1, imageList.Images.Count);
            }
        }

        [TestMethod]
        public void ShellThumbnailは存在しないファイルや空のパスにnullを返す()
        {
            Assert.IsNull(ShellThumbnail.GetThumbnail(Path.Combine(_tempDirectory, "nothing.mp4"), 32, 32));
            Assert.IsNull(ShellThumbnail.GetThumbnail("", 32, 32));
            Assert.IsNull(ShellThumbnail.GetThumbnail(null, 32, 32));
        }
    }
}
