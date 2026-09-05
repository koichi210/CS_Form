using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FFEdit.Tests
{
    /// <summary>
    /// FFEdit.SaveRestore（StcSaveRestore を継承した設定保存クラス）のテスト。
    ///
    /// Cheetos と同じ方式で、実際の Form1 を生成して確認する。FFEdit の RegistItem は
    /// コンボボックスの入力履歴3つとテキストボックス1つだけとシンプル。
    /// </summary>
    [TestClass]
    public class SaveRestoreTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "FFEditSaveRestoreTests_" + Guid.NewGuid().ToString("N"));
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

        private static SaveRestore NewSaveRestore(Form1 form)
        {
            var sr = new SaveRestore();
            sr.RegistItem(form);
            return sr;
        }

        private string PathFor(string name)
        {
            return Path.Combine(tempDirectory, name + ".xml");
        }

        [TestMethod]
        public void 拡張子の指定が保存して読み直すと戻る()
        {
            using (Form1 writer = NewForm())
            {
                writer.textBox_Target_Extension.Text = "*.png";

                string path = PathFor("ext");
                Assert.IsTrue(NewSaveRestore(writer).SaveXmlFile(path));

                using (Form1 reader = NewForm())
                {
                    Assert.IsTrue(NewSaveRestore(reader).LoadXmlFile(path));
                    Assert.AreEqual("*.png", reader.textBox_Target_Extension.Text);
                }
            }
        }

        [TestMethod]
        public void コンボボックスの入力履歴が保存して読み直すと戻る()
        {
            using (Form1 writer = NewForm())
            {
                writer.comboBox_TargetDir.Items.Add(@"C:\work");
                writer.comboBox_TargetDir.Items.Add(@"D:\data");
                writer.comboBox_String1.Items.Add("foo");
                writer.comboBox_String2.Items.Add("bar");

                string path = PathFor("history");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Form1 reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(2, reader.comboBox_TargetDir.Items.Count);
                    Assert.AreEqual(@"C:\work", reader.comboBox_TargetDir.Items[0]);
                    Assert.AreEqual(@"D:\data", reader.comboBox_TargetDir.Items[1]);
                    CollectionAssert.Contains(reader.comboBox_String1.Items, "foo");
                    CollectionAssert.Contains(reader.comboBox_String2.Items, "bar");
                }
            }
        }

        [TestMethod]
        public void 拡張子は未指定なら既定値のアスタリスクになる()
        {
            using (Form1 writer = NewForm())
            {
                // RegistCtrl の既定値は "*"。何も設定せず保存・読み込みしても既定値のまま
                string path = PathFor("default_ext");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Form1 reader = NewForm())
                {
                    reader.textBox_Target_Extension.Text = "書き換え";
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual("*", reader.textBox_Target_Extension.Text);
                }
            }
        }

        [TestMethod]
        public void LoadProcはファイル名が空なら何もせず失敗を返す()
        {
            using (Form1 form = NewForm())
            {
                var sr = new SaveRestore();
                Assert.IsFalse(sr.LoadProc("", form));
            }
        }

        [TestMethod]
        public void SaveSettingは今入力中の文字列を履歴に追加してから重複を整理する()
        {
            // ModifyCombBoxList は Items を無条件に整理するわけではない。
            // ComboCtrl.Text（今まさに入力/選択されている値）が空ならそのまま何もせず戻り、
            // 空でなければ Text を Items に追加したうえで重複を取り除く、という動き。
            // つまり「既存の Items に重複があっても、Text が空なら整理されない」。
            using (Form1 writer = NewForm())
            {
                writer.comboBox_String1.Items.Add("dup");
                writer.comboBox_String1.Text = "dup"; // 今回も同じ値を使った、という体にする

                string path = PathFor("dedup");
                var sr = new SaveRestore();
                sr.RegistItem(writer);
                Assert.IsTrue(sr.SaveSetting(path, writer));

                using (Form1 reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);
                    Assert.AreEqual(1, reader.comboBox_String1.Items.Count, "Text分と重複するので1件に整理される");
                }
            }
        }

        [TestMethod]
        public void SaveSettingは入力欄が空だと履歴を整理しない()
        {
            using (Form1 writer = NewForm())
            {
                writer.comboBox_String1.Items.Add("a");
                writer.comboBox_String1.Items.Add("a");
                // Text は空のまま（何も入力/選択していない）

                string path = PathFor("no_text");
                var sr = new SaveRestore();
                sr.RegistItem(writer);
                sr.SaveSetting(path, writer);

                using (Form1 reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);
                    Assert.AreEqual(2, reader.comboBox_String1.Items.Count,
                        "Text が空だと ModifyCombBoxList は即 return するので重複はそのまま残る");
                }
            }
        }
    }
}
