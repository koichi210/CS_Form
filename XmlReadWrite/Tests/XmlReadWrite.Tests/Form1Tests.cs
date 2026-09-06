using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmlReadWrite.Tests
{
    /// <summary>
    /// Form1（XMLの読み書きサンプル、複数の書き込み/読み込み方式を比較する）の
    /// テスト。
    ///
    /// ⚠️ 全ハンドラは指定ファイルが存在しないとMessageBox.Showを呼ぶため、
    /// テストでは常に実在するファイルを指定して回避する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        private string tempDirectory;
        private string xmlPath;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "XmlReadWriteTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            xmlPath = Path.Combine(tempDirectory, "Sample.xml");
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
        public void write2で書いてread4で読み込むとtextBoxに値が復元される()
        {
            File.WriteAllText(xmlPath, ""); // File.Existsチェックを通すためのダミー

            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_FileName", xmlPath);
                FormReflection.SetText(form, "textBox_Param1", "Hello");
                FormReflection.SetText(form, "textBox_Param2", "42");
                FormReflection.SetText(form, "textBox_Param3", @"c:\tmp");
                FormReflection.SetText(form, "textBox_Param4", "テスト値");

                FormReflection.InvokeHandler(form, "button_write2_Click", form);

                // 書き込み後にtextBoxをクリアしてから読み込み、復元されることを確認する
                FormReflection.SetText(form, "textBox_Param1", "");
                FormReflection.SetText(form, "textBox_Param2", "");
                FormReflection.SetText(form, "textBox_Param3", "");
                FormReflection.SetText(form, "textBox_Param4", "");

                FormReflection.InvokeHandler(form, "button_read4_Click", form);

                Assert.AreEqual("Hello", FormReflection.GetControl(form, "textBox_Param1").Text);
                Assert.AreEqual("42", FormReflection.GetControl(form, "textBox_Param2").Text);
                Assert.AreEqual(@"c:\tmp", FormReflection.GetControl(form, "textBox_Param3").Text);
                Assert.AreEqual("テスト値", FormReflection.GetControl(form, "textBox_Param4").Text);
            }
        }

        [TestMethod]
        public void writeはXmlTextWriterで有効なXMLファイルを作成する()
        {
            File.WriteAllText(xmlPath, ""); // File.Existsチェックを通すためのダミー

            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_FileName", xmlPath);
                FormReflection.SetText(form, "textBox_Param1", "Hello World");

                FormReflection.InvokeHandler(form, "button_write_Click", form);

                string content = File.ReadAllText(xmlPath);
                StringAssert.Contains(content, "Hello World");
                StringAssert.Contains(content, "Setting");
            }
        }

        [TestMethod]
        public void read_read2_read3はwriteで作成したXMLを例外なく読み込める()
        {
            File.WriteAllText(xmlPath, "");

            using (var form = new Form1())
            {
                FormReflection.SetText(form, "textBox_FileName", xmlPath);
                FormReflection.InvokeHandler(form, "button_write_Click", form);

                // Console.Writeで出力するだけの実装だが、例外なく走ることを確認する
                FormReflection.InvokeHandler(form, "button_read_Click", form);
                FormReflection.InvokeHandler(form, "button_read2_Click", form);
                FormReflection.InvokeHandler(form, "button_read3_Click", form);
            }
        }
    }
}
