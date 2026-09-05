using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encoder.Tests
{
    /// <summary>
    /// Form1（UTF-8 → Shift-JIS 変換ツール）のテスト。
    ///
    /// ⚠️ Execute() はフォルダ指定・存在しないファイル指定のときに MessageBox.Show を呼ぶ
    /// （どちらもボタン1つの単純なものだが、自動テストでは誰も閉じられずハングする）。
    /// そのためテストでは常に実在するファイルパスだけを渡す。
    /// また既定で radioButton_Utf8ToSjis.Checked = true になっており、false のときは
    /// 「未実装です」の MessageBox が出るため、この既定値も変更しない。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "EncoderTests_" + Guid.NewGuid().ToString("N"));
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
        public void UTF8のファイルをShiftJISへ変換して別名で保存する()
        {
            string inputPath = Path.Combine(tempDirectory, "input.txt");
            File.WriteAllText(inputPath, "こんにちは", new UTF8Encoding(false));

            using (var form = new Form1())
            {
                FormReflection.InvokeMethod(form, "Execute", inputPath);
            }

            string outputPath = Path.Combine(tempDirectory, "input_sjis.txt");
            Assert.IsTrue(File.Exists(outputPath), "元のファイル名 + _sjis のファイルができるはず");

            string result = File.ReadAllText(outputPath, Encoding.GetEncoding("Shift_JIS"));
            Assert.AreEqual("こんにちは", result);
        }

        [TestMethod]
        public void 出力ファイル名は拡張子の前に_sjisが付く()
        {
            string inputPath = Path.Combine(tempDirectory, "report.log");
            File.WriteAllText(inputPath, "test", new UTF8Encoding(false));

            using (var form = new Form1())
            {
                FormReflection.InvokeMethod(form, "Execute", inputPath);
            }

            Assert.IsTrue(File.Exists(Path.Combine(tempDirectory, "report_sjis.log")));
        }

        [TestMethod]
        public void DragDropで複数ファイルをまとめて変換できる()
        {
            string file1 = Path.Combine(tempDirectory, "a.txt");
            string file2 = Path.Combine(tempDirectory, "b.txt");
            File.WriteAllText(file1, "one", new UTF8Encoding(false));
            File.WriteAllText(file2, "two", new UTF8Encoding(false));

            using (var form = new Form1())
            {
                var data = new System.Windows.Forms.DataObject();
                data.SetData(System.Windows.Forms.DataFormats.FileDrop, new[] { file1, file2 });
                var e = new System.Windows.Forms.DragEventArgs(data, 0, 0, 0,
                    System.Windows.Forms.DragDropEffects.Copy, System.Windows.Forms.DragDropEffects.Copy);

                FormReflection.InvokeHandler(form, "DropBox_DragDrop", null, e);
            }

            Assert.IsTrue(File.Exists(Path.Combine(tempDirectory, "a_sjis.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(tempDirectory, "b_sjis.txt")));
        }
    }
}
