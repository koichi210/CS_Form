using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrimFileData.Tests
{
    /// <summary>
    /// Form1（リファレンスファイルから検索ワードにヒットする行を抽出するツール）のテスト。
    ///
    /// ⚠️ button_SaveSetting_Click は保存に成功すると必ず MessageBox.Show を呼ぶため
    /// （失敗時だけでなく成功時にも出る作り）、テスト対象から除外する。
    /// button_Execute_Click はリファレンスファイルが開けないと MessageBox.Show を呼ぶため、
    /// テストでは常に実在するファイルを指定して回避する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "TrimFileDataTests_" + Guid.NewGuid().ToString("N"));
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
        public void 実行ボタンでリファレンスファイルから検索結果を作成する()
        {
            string referPath = Path.Combine(tempDirectory, "refer.txt");
            File.WriteAllText(referPath, "apple pie" + Environment.NewLine + "banana split", Encoding.GetEncoding("Shift_JIS"));

            string result = null;
            Exception failure = null;

            // button_Execute_Click は内部で Clipboard.SetText を呼ぶため、
            // OLE呼び出しに必要な STA スレッド上で実行する。
            var thread = new Thread(() =>
            {
                try
                {
                    using (var form = new Form1())
                    {
                        FormReflection.SetText(form, "textBox_ReferencePath", referPath);
                        FormReflection.SetText(form, "textBox_SerchWordList", "apple");

                        FormReflection.InvokeHandler(form, "button_Execute_Click");

                        result = FormReflection.GetText(form, "textBox_SerchResultList");
                    }
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (failure != null)
            {
                throw failure;
            }

            StringAssert.Contains(result, "◆apple");
            StringAssert.Contains(result, "apple pie");
            Assert.IsFalse(result.Contains("banana"));
        }
    }
}
