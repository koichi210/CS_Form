using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaptureWindow.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、設定値のXML保存/読み込みロジック）のテスト。
    ///
    /// ⚠️ Form1.csの大半のハンドラは実際のマウス操作(SendInput/Cursor.Position)や
    /// キー送信(SendKeys.SendWait)、画面キャプチャ、確認ダイアログ(MessageBox)を
    /// 伴うため、それらは安全にテストできずテスト対象から除外した。
    /// 設定値のXML保存/読み込みだけは純粋なファイルI/Oなので、ここで検証する。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "CaptureWindowTests_" + Guid.NewGuid().ToString("N"));
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
        public void 保存した設定を読み込むと同じ値が復元される()
        {
            string path = Path.Combine(tempDirectory, "settings.json");

            Logic.SaveSetting(path, @"C:	mp", "500", "600", "3");

            Logic.Settings settings = Logic.LoadSetting(path);

            Assert.IsNotNull(settings);
            Assert.AreEqual(@"C:	mp", settings.SavePath);
            Assert.AreEqual("500", settings.MouseX);
            Assert.AreEqual("600", settings.MouseY);
            Assert.AreEqual("3", settings.Sleep);
        }

        [TestMethod]
        public void ファイルが存在しなければnullを返す()
        {
            string path = Path.Combine(tempDirectory, "notfound.json");

            Logic.Settings settings = Logic.LoadSetting(path);

            Assert.IsNull(settings);
        }

        [TestMethod]
        public void 保存ファイルはJSONとして書き出される()
        {
            string path = Path.Combine(tempDirectory, "settings.json");

            Logic.SaveSetting(path, @"C:	mp", "1", "2", "3");

            string content = File.ReadAllText(path);
            StringAssert.Contains(content, "\"TextBox_SavePath\"");
            StringAssert.StartsWith(content.TrimStart(), "{");
        }

        [TestMethod]
        public void 旧XMLしか無ければJSONへ移行され旧XMLは削除される()
        {
            string jsonPath = Path.Combine(tempDirectory, "migrate.json");
            string xmlPath = Path.Combine(tempDirectory, "migrate.xml");

            // 旧形式(XML)の設定ファイルを用意する
            File.WriteAllText(xmlPath,
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                + "<root>"
                + "<Setting attribute=\"TextBox_SavePath\">C:\\old</Setting>"
                + "<Setting attribute=\"TextBox_MouseX\">10</Setting>"
                + "<Setting attribute=\"TextBox_MouseY\">20</Setting>"
                + "<Setting attribute=\"TextBox_Sleep\">30</Setting>"
                + "</root>");

            Logic.Settings settings = Logic.LoadSetting(jsonPath);

            Assert.IsNotNull(settings, "旧XMLの内容が読み込めること");
            Assert.AreEqual(@"C:\old", settings.SavePath);
            Assert.AreEqual("10", settings.MouseX);
            Assert.IsTrue(File.Exists(jsonPath), "JSONへ保存し直されること");
            Assert.IsFalse(File.Exists(xmlPath), "移行後は旧XMLが削除されること");
        }

        [TestMethod]
        public void JSONと旧XMLが両方あればJSONが使われ旧XMLは残る()
        {
            string jsonPath = Path.Combine(tempDirectory, "both.json");
            string xmlPath = Path.Combine(tempDirectory, "both.xml");

            Logic.SaveSetting(jsonPath, @"C:\json", "1", "2", "3");
            File.WriteAllText(xmlPath,
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                + "<root><Setting attribute=\"TextBox_SavePath\">C:\\xml</Setting></root>");

            Logic.Settings settings = Logic.LoadSetting(jsonPath);

            Assert.AreEqual(@"C:\json", settings.SavePath, "JSONが優先されること");
            Assert.IsTrue(File.Exists(xmlPath), "移行していないので旧XMLは触らないこと");
        }
    }
}
