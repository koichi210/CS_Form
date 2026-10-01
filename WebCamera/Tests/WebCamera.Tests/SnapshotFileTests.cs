using System;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebCamera.Tests
{
    /// <summary>
    /// スナップショット保存まわりの、カメラやUIに依存しないロジックのテスト。
    /// </summary>
    [TestClass]
    public class SnapshotFileTests
    {
        [TestMethod]
        public void 既定ファイル名は日時入りのpng()
        {
            string name = SnapshotFile.CreateDefaultFileName(new DateTime(2026, 10, 2, 13, 45, 1));
            Assert.AreEqual("20261002_134501.png", name);
        }

        [TestMethod]
        public void 既定ファイル名は一桁の月日時分秒をゼロ埋めし24時間表記()
        {
            Assert.AreEqual("20260102_030405.png", SnapshotFile.CreateDefaultFileName(new DateTime(2026, 1, 2, 3, 4, 5)));
            Assert.AreEqual("20261231_235959.png", SnapshotFile.CreateDefaultFileName(new DateTime(2026, 12, 31, 23, 59, 59)));
        }

        [TestMethod]
        public void 既定ファイル名は和暦カルチャでも西暦になる()
        {
            CultureInfo original = Thread.CurrentThread.CurrentCulture;
            try
            {
                var japanese = new CultureInfo("ja-JP");
                japanese.DateTimeFormat.Calendar = new JapaneseCalendar();
                Thread.CurrentThread.CurrentCulture = japanese;

                Assert.AreEqual("20261002_134501.png", SnapshotFile.CreateDefaultFileName(new DateTime(2026, 10, 2, 13, 45, 1)));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = original;
            }
        }

        [TestMethod]
        public void 保存ダイアログの設定はpng()
        {
            Assert.AreEqual("png", SnapshotFile.Extension);
            StringAssert.EndsWith(SnapshotFile.DialogFilter, "|*.png");
        }

        [TestMethod]
        public void 初期フォルダはマイピクチャ()
        {
            string expected = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (string.IsNullOrEmpty(expected))
            {
                expected = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            Assert.AreEqual(expected, SnapshotFile.GetInitialDirectory());
        }
    }
}
