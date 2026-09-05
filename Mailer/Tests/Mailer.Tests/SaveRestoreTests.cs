using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mailer.Tests
{
    /// <summary>
    /// Mailer.SaveRestore（StcSaveRestore を継承した設定保存クラス）のテスト。
    /// 実際の Form1 を生成し、メール送信フォームの各項目を保存/読み込みで検証する。
    /// </summary>
    [TestClass]
    public class SaveRestoreTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "MailerSaveRestoreTests_" + Guid.NewGuid().ToString("N"));
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

        private static Form1 NewForm()
        {
            return new Form1();
        }

        [TestMethod]
        public void メール項目が保存して読み直すと戻る()
        {
            using (Form1 writer = NewForm())
            {
                writer.textBox_BrowserPath.Text = @"C:\chrome.exe";
                writer.textBox_MailTo.Text = "to@example.com";
                writer.textBox_MailCc.Text = "cc@example.com";
                writer.textBox_MailBcc.Text = "bcc@example.com";
                writer.textBox_MailSubject.Text = "件名 %%today%%";
                writer.textBox_MailBody.Text = "本文です";

                var sr = new SaveRestore();
                sr.RegistLoadItem(writer);
                string path = Path.Combine(tempDirectory, "setting.xml");
                Assert.IsTrue(sr.SaveSetting(path, writer));

                using (Form1 reader = NewForm())
                {
                    var readerSr = new SaveRestore();
                    readerSr.RegistLoadItem(reader);
                    Assert.IsTrue(readerSr.LoadProc(path, reader));

                    Assert.AreEqual(@"C:\chrome.exe", reader.textBox_BrowserPath.Text);
                    Assert.AreEqual("to@example.com", reader.textBox_MailTo.Text);
                    Assert.AreEqual("cc@example.com", reader.textBox_MailCc.Text);
                    Assert.AreEqual("bcc@example.com", reader.textBox_MailBcc.Text);
                    Assert.AreEqual("件名 %%today%%", reader.textBox_MailSubject.Text);
                    Assert.AreEqual("本文です", reader.textBox_MailBody.Text);
                }
            }
        }

        [TestMethod]
        public void LoadProcは存在しないファイルなら例外にならず失敗を返す()
        {
            using (Form1 form = NewForm())
            {
                var sr = new SaveRestore();
                sr.RegistLoadItem(form);

                Assert.IsFalse(sr.LoadProc(Path.Combine(tempDirectory, "nothing.xml"), form));
            }
        }
    }
}
