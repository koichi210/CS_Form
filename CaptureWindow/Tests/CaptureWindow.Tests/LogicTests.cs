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
            string path = Path.Combine(tempDirectory, "settings.xml");

            Logic.SaveSettingXml(path, @"C:\tmp", "500", "600", "3");

            Logic.Settings settings = Logic.LoadSettingXml(path);

            Assert.IsNotNull(settings);
            Assert.AreEqual(@"C:\tmp", settings.SavePath);
            Assert.AreEqual("500", settings.MouseX);
            Assert.AreEqual("600", settings.MouseY);
            Assert.AreEqual("3", settings.Sleep);
        }

        [TestMethod]
        public void ファイルが存在しなければnullを返す()
        {
            string path = Path.Combine(tempDirectory, "notfound.xml");

            Logic.Settings settings = Logic.LoadSettingXml(path);

            Assert.IsNull(settings);
        }

        [TestMethod]
        public void 保存ファイルは有効なXMLとして書き出される()
        {
            string path = Path.Combine(tempDirectory, "settings.xml");

            Logic.SaveSettingXml(path, @"C:\tmp", "1", "2", "3");

            string content = File.ReadAllText(path);
            StringAssert.Contains(content, "<root>");
            StringAssert.Contains(content, "TextBox_SavePath");
        }
    }
}
