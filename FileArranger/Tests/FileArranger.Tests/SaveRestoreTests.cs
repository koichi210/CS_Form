using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArranger.Tests
{
    /// <summary>
    /// FileArranger.SaveRestore（StcSaveRestore を継承した設定保存クラス）のテスト。
    /// Cheetos / FFEdit / PerforceWrapper と同じ方式で、実際の FileArranger フォームを
    /// 生成して確認する。
    ///
    /// クラス名がフォームと同じ "FileArranger"（名前空間も FileArranger）なので、
    /// このテストファイル内では常に完全修飾名 global::FileArranger.FileArranger で
    /// フォームの型を指す。
    /// </summary>
    [TestClass]
    public class SaveRestoreTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "FileArrangerSaveRestoreTests_" + Guid.NewGuid().ToString("N"));
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

        private static global::FileArranger.FileArranger NewForm()
        {
            return new global::FileArranger.FileArranger();
        }

        private static SaveRestore NewSaveRestore(global::FileArranger.FileArranger form)
        {
            var sr = new SaveRestore();
            sr.RegistLoadItem(form);
            return sr;
        }

        private string PathFor(string name)
        {
            return Path.Combine(tempDirectory, name + ".xml");
        }

        [TestMethod]
        public void 共通タブの入力値が保存して読み直すと戻る()
        {
            using (global::FileArranger.FileArranger writer = NewForm())
            {
                writer.cmn_textBox_Reference.Text = @"D:\source";
                writer.cmn_textBox_AddList.Text = "add1\r\nadd2";
                writer.cmn_textBox_AddListSuffix.Text = "_suffix";

                string path = PathFor("common");
                Assert.IsTrue(NewSaveRestore(writer).SaveXmlFile(path));

                using (global::FileArranger.FileArranger reader = NewForm())
                {
                    Assert.IsTrue(NewSaveRestore(reader).LoadXmlFile(path));

                    Assert.AreEqual(@"D:\source", reader.cmn_textBox_Reference.Text);
                    Assert.AreEqual("add1\r\nadd2", reader.cmn_textBox_AddList.Text);
                    Assert.AreEqual("_suffix", reader.cmn_textBox_AddListSuffix.Text);
                }
            }
        }

        [TestMethod]
        public void フォルダ移動タブの設定が保存して読み直すと戻る()
        {
            using (global::FileArranger.FileArranger writer = NewForm())
            {
                writer.md_textBox_SourceDir.Text = @"D:\move_src";
                writer.md_comboBox_TargetDir.Text = @"D:\move_dst";

                string path = PathFor("movedir");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (global::FileArranger.FileArranger reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(@"D:\move_src", reader.md_textBox_SourceDir.Text);
                    Assert.AreEqual(@"D:\move_dst", reader.md_comboBox_TargetDir.Text);
                }
            }
        }

        [TestMethod]
        public void リネームタブの設定が保存して読み直すと戻る()
        {
            using (global::FileArranger.FileArranger writer = NewForm())
            {
                writer.rd_textBox_ExistItemDir.Text = @"D:\rename";
                writer.rd_comboBox_MergeWord.Text = "merge";
                writer.rd_checkBox_FileOpen.Checked = true;
                writer.rd_textBox_SplitWord3.Text = "_";
                writer.rd_textBox_AddTitlePreWord.Text = "pre_";
                writer.rd_textBox_SearchTitleLine.Text = "0";
                writer.rd_textBox_SearchTitleLength.Text = "3";
                writer.rd_comboBox_AddTitlePostWord.Text = "_post";

                string path = PathFor("rename");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (global::FileArranger.FileArranger reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(@"D:\rename", reader.rd_textBox_ExistItemDir.Text);
                    Assert.AreEqual("merge", reader.rd_comboBox_MergeWord.Text);
                    Assert.IsTrue(reader.rd_checkBox_FileOpen.Checked);
                    Assert.AreEqual("_", reader.rd_textBox_SplitWord3.Text);
                    Assert.AreEqual("pre_", reader.rd_textBox_AddTitlePreWord.Text);
                    Assert.AreEqual("0", reader.rd_textBox_SearchTitleLine.Text);
                    Assert.AreEqual("3", reader.rd_textBox_SearchTitleLength.Text);
                    Assert.AreEqual("_post", reader.rd_comboBox_AddTitlePostWord.Text);
                }
            }
        }

        [TestMethod]
        public void 振り分けタブの設定が保存して読み直すと戻る()
        {
            using (global::FileArranger.FileArranger writer = NewForm())
            {
                writer.pf_textBox_TargetFile.Text = @"D:\pf_target";
                writer.pf_textBox_RefrenceFile.Text = @"D:\pf_reference";
                writer.pf_textBox_TargetSeprator.Text = "-";
                writer.pf_textBox_SearchTitleLine.Text = "1";
                writer.pf_textBox_SearchTitleLength.Text = "2";
                writer.pf_checkBox_CreateNewDir.Checked = true;

                string path = PathFor("partition");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (global::FileArranger.FileArranger reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(@"D:\pf_target", reader.pf_textBox_TargetFile.Text);
                    Assert.AreEqual(@"D:\pf_reference", reader.pf_textBox_RefrenceFile.Text);
                    Assert.AreEqual("-", reader.pf_textBox_TargetSeprator.Text);
                    Assert.AreEqual("1", reader.pf_textBox_SearchTitleLine.Text);
                    Assert.AreEqual("2", reader.pf_textBox_SearchTitleLength.Text);
                    Assert.IsTrue(reader.pf_checkBox_CreateNewDir.Checked);
                }
            }
        }

        [TestMethod]
        public void ソートタブとファイル移動タブの設定が保存して読み直すと戻る()
        {
            using (global::FileArranger.FileArranger writer = NewForm())
            {
                writer.sf_textBox_TargetFile.Text = @"D:\sf_target";
                writer.mf_textBox_SourceDir.Text = @"D:\mf_src";
                writer.mf_textBox_TargetDir.Text = @"D:\mf_dst";

                string path = PathFor("sort_move");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (global::FileArranger.FileArranger reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(@"D:\sf_target", reader.sf_textBox_TargetFile.Text);
                    Assert.AreEqual(@"D:\mf_src", reader.mf_textBox_SourceDir.Text);
                    Assert.AreEqual(@"D:\mf_dst", reader.mf_textBox_TargetDir.Text);
                }
            }
        }

        [TestMethod]
        public void コンボボックスの入力履歴が保存して読み直すと戻る()
        {
            using (global::FileArranger.FileArranger writer = NewForm())
            {
                writer.md_comboBox_TargetDir.Items.Add(@"D:\a");
                writer.md_comboBox_TargetDir.Items.Add(@"D:\b");
                writer.rd_comboBox_RenameDir.Items.Add("rename1");
                writer.rd_comboBox_AddTitlePostWord.Items.Add("post1");

                string path = PathFor("history");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (global::FileArranger.FileArranger reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(2, reader.md_comboBox_TargetDir.Items.Count);
                    Assert.AreEqual(@"D:\a", reader.md_comboBox_TargetDir.Items[0]);
                    Assert.AreEqual(@"D:\b", reader.md_comboBox_TargetDir.Items[1]);
                    CollectionAssert.Contains(reader.rd_comboBox_RenameDir.Items, "rename1");
                    CollectionAssert.Contains(reader.rd_comboBox_AddTitlePostWord.Items, "post1");
                }
            }
        }

        [TestMethod]
        public void LoadProcは参照候補フォルダとリストを読み込んで反映する()
        {
            using (global::FileArranger.FileArranger writer = NewForm())
            {
                writer.RefrenceCandidateFolders = new[] { @"D:\ref\a", @"D:\ref\b" };

                var sr = new SaveRestore();
                sr.RegistLoadItem(writer);

                string path = PathFor("reference");
                Assert.IsTrue(sr.SaveSetting(path, writer));

                using (global::FileArranger.FileArranger reader = NewForm())
                {
                    var readerSr = new SaveRestore();
                    readerSr.RegistLoadItem(reader);

                    Assert.IsTrue(readerSr.LoadProc(path, reader));
                    CollectionAssert.AreEqual(new[] { @"D:\ref\a", @"D:\ref\b" }, reader.RefrenceCandidateFolders);
                }
            }
        }

        [TestMethod]
        public void LoadProcは読み込んだあとリストをクリアする()
        {
            using (global::FileArranger.FileArranger writer = NewForm())
            {
                writer.RefrenceCandidateFolders = new string[0];
                string path = PathFor("clearlist");
                var sr = new SaveRestore();
                sr.RegistLoadItem(writer);
                sr.SaveSetting(path, writer);

                using (global::FileArranger.FileArranger reader = NewForm())
                {
                    reader.sf_listBox_Target.Items.Add("残っててはいけない項目");
                    reader.rd_listView_Target.Items.Add("残っててはいけない項目");
                    reader.pf_listView_Target.Items.Add("残っててはいけない項目");

                    var readerSr = new SaveRestore();
                    readerSr.RegistLoadItem(reader);
                    readerSr.LoadProc(path, reader);

                    Assert.AreEqual(0, reader.sf_listBox_Target.Items.Count);
                    Assert.AreEqual(0, reader.rd_listView_Target.Items.Count);
                    Assert.AreEqual(0, reader.pf_listView_Target.Items.Count);
                }
            }
        }

        [TestMethod]
        public void 存在しないファイルを読んでも例外にならず失敗を返す()
        {
            using (global::FileArranger.FileArranger form = NewForm())
            {
                Assert.IsFalse(NewSaveRestore(form).LoadXmlFile(PathFor("nothing")));
            }
        }
    }
}
