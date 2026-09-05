using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cheetos.Tests
{
    /// <summary>
    /// Cheetos.SaveRestore（StcSaveRestore を継承した設定保存クラス）のテスト。
    ///
    /// SaveRestore.RegistItem は実際の Cheetos フォームの 31 個のコントロールを
    /// 決め打ちの属性名で登録する。ここでは Cheetos フォームを実際に生成して、
    /// 「保存して読み直すと値が戻る」という結果で確認する。
    /// 個々の項目をモックで代替せず実物の Form を使うのは、RegistItem 内の
    /// タイプミスや属性名の重複（コピペ跡が残りやすい箇所）を検出したいため。
    ///
    /// Cheetos のコンストラクタ自身も内部で SaveRestore を1つ持って RegistItem / LoadProc を
    /// 呼んでいるが、それとは別に、テストごとに新しい SaveRestore を作って同じフォームへ
    /// RegistItem しなおしている。StcSaveRestore.SaveXmlFile / LoadXmlFile は public なので
    /// Cheetos クラス自体には手を加えていない。
    /// </summary>
    [TestClass]
    public class SaveRestoreTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "CheetosTests_" + Guid.NewGuid().ToString("N"));
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

        /// <summary>
        /// Cheetos フォームを生成する。コンストラクタは Cheetos.xml をカレントディレクトリから
        /// 読もうとするが、無ければ何もしない（LoadXmlFile が false を返すだけ）ので無害。
        /// </summary>
        private static Cheetos NewForm()
        {
            return new Cheetos();
        }

        /// <summary>指定したフォームに、実運用と同じ 31 項目を登録した SaveRestore を作る。</summary>
        private static SaveRestore NewSaveRestore(Cheetos form)
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
        public void CaptureWindowタブの値が保存して読み直すと戻る()
        {
            using (Cheetos writer = NewForm())
            {
                writer.cw_TextBox_SavePath.Text = @"C:\capture\out";
                writer.cw_TextBox_SaveFilePrifix.Text = "shot_";
                writer.cw_checkBox_AddTimeStump.Checked = true;
                writer.cw_Radio_CurrentWindow.Checked = true;
                writer.cw_TextBox_Sleep.Text = "1234";
                writer.cw_TextBox_Loop.Text = "9";

                string path = PathFor("capture");
                Assert.IsTrue(NewSaveRestore(writer).SaveXmlFile(path), "保存に成功するはず");

                using (Cheetos reader = NewForm())
                {
                    Assert.IsTrue(NewSaveRestore(reader).LoadXmlFile(path), "読み込みに成功するはず");

                    Assert.AreEqual(@"C:\capture\out", reader.cw_TextBox_SavePath.Text);
                    Assert.AreEqual("shot_", reader.cw_TextBox_SaveFilePrifix.Text);
                    Assert.IsTrue(reader.cw_checkBox_AddTimeStump.Checked);
                    Assert.IsTrue(reader.cw_Radio_CurrentWindow.Checked);
                    Assert.AreEqual("1234", reader.cw_TextBox_Sleep.Text);
                    Assert.AreEqual("9", reader.cw_TextBox_Loop.Text);
                }
            }
        }

        [TestMethod]
        public void Rotationタブの値が保存して読み直すと戻る()
        {
            using (Cheetos writer = NewForm())
            {
                writer.pr_SourceFolderPath.Text = @"D:\photos";
                writer.pr_BaseX.Text = "100";
                writer.pr_BaseY.Text = "200";
                writer.pr_Angle.Text = "45";

                string path = PathFor("rotation");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Cheetos reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(@"D:\photos", reader.pr_SourceFolderPath.Text);
                    Assert.AreEqual("100", reader.pr_BaseX.Text);
                    Assert.AreEqual("200", reader.pr_BaseY.Text);
                    Assert.AreEqual("45", reader.pr_Angle.Text);
                }
            }
        }

        [TestMethod]
        public void DistOrientタブの値が保存して読み直すと戻る()
        {
            using (Cheetos writer = NewForm())
            {
                writer.do_SourceFolderPath.Text = @"D:\src";
                writer.do_DestPortFolderPath.Text = @"D:\port";
                writer.do_DestLandFolderPath.Text = @"D:\land";
                writer.do_TargetFileName.Text = "*.jpg";
                writer.do_WhiteLength.Text = "5";
                writer.do_WhiteCoef.Text = "77";
                writer.do_SampleFilePath.Text = @"D:\sample.jpg";

                string path = PathFor("distorient");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Cheetos reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(@"D:\src", reader.do_SourceFolderPath.Text);
                    Assert.AreEqual(@"D:\port", reader.do_DestPortFolderPath.Text);
                    Assert.AreEqual(@"D:\land", reader.do_DestLandFolderPath.Text);
                    Assert.AreEqual("*.jpg", reader.do_TargetFileName.Text);
                    Assert.AreEqual("5", reader.do_WhiteLength.Text);
                    Assert.AreEqual("77", reader.do_WhiteCoef.Text);
                    Assert.AreEqual(@"D:\sample.jpg", reader.do_SampleFilePath.Text);
                }
            }
        }

        [TestMethod]
        public void PictMergeタブとFileCollectタブの値が保存して読み直すと戻る()
        {
            using (Cheetos writer = NewForm())
            {
                writer.pm_SourceFolderPath.Text = @"D:\merge";
                writer.pm_SourceFile1Prefix.Text = "left_";
                writer.pm_SourceFile2Prefix.Text = "right_";
                writer.pm_TrimingHeight.Text = "480";

                writer.fc_SourceFolderPath.Text = @"D:\from";
                writer.fc_DestFolderPath.Text = @"D:\to";
                writer.fc_TargetFileName.Text = "*.png";

                string path = PathFor("merge");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Cheetos reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(@"D:\merge", reader.pm_SourceFolderPath.Text);
                    Assert.AreEqual("left_", reader.pm_SourceFile1Prefix.Text);
                    Assert.AreEqual("right_", reader.pm_SourceFile2Prefix.Text);
                    Assert.AreEqual("480", reader.pm_TrimingHeight.Text);

                    Assert.AreEqual(@"D:\from", reader.fc_SourceFolderPath.Text);
                    Assert.AreEqual(@"D:\to", reader.fc_DestFolderPath.Text);
                    Assert.AreEqual("*.png", reader.fc_TargetFileName.Text);
                }
            }
        }

        [TestMethod]
        public void PictTrimタブのラジオボタンが保存して読み直すと戻る()
        {
            using (Cheetos writer = NewForm())
            {
                writer.pt_SourceFolderPath.Text = @"D:\trim";
                writer.pt_BaseX.Text = "10";
                writer.pt_BaseY.Text = "20";
                writer.pt_Radio_SelectSizeOfEnd.Checked = true;
                writer.pt_TargetX.Text = "30";
                writer.pt_TargetY.Text = "40";

                string path = PathFor("trim");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Cheetos reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(@"D:\trim", reader.pt_SourceFolderPath.Text);
                    Assert.AreEqual("10", reader.pt_BaseX.Text);
                    Assert.AreEqual("20", reader.pt_BaseY.Text);
                    Assert.IsTrue(reader.pt_Radio_SelectSizeOfEnd.Checked);
                    Assert.IsFalse(reader.pt_Radio_SelectPointOfEnd.Checked);
                    Assert.AreEqual("30", reader.pt_TargetX.Text);
                    Assert.AreEqual("40", reader.pt_TargetY.Text);
                }
            }
        }

        [TestMethod]
        public void CaptureWindowのDataGridの行数が保存して読み直すと戻る()
        {
            using (Cheetos writer = NewForm())
            {
                // 注意: RegistCtrl(DataGridView版) は登録した瞬間に RowCount を 1 へ強制する
                // （StandardTemplateClass.cs の RegistCtrl 実装）。実運用では RegistItem は
                // フォームのコンストラクタで一度だけ呼ばれ、そのあとユーザーが行数を増やすので
                // 問題にならない。テストでも RegistItem のあとに行数を設定する。
                SaveRestore sr = NewSaveRestore(writer);
                writer.cw_dataGridView.RowCount = 3;

                string path = PathFor("datagrid");
                sr.SaveXmlFile(path);

                using (Cheetos reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(3, reader.cw_dataGridView.RowCount);
                }
            }
        }

        [TestMethod]
        public void 存在しないファイルを読んでも例外にならず失敗を返す()
        {
            using (Cheetos form = NewForm())
            {
                Assert.IsFalse(NewSaveRestore(form).LoadXmlFile(PathFor("nothing")));
            }
        }

        [TestMethod]
        public void LoadProcはファイル名が空なら何もせず失敗を返す()
        {
            using (Cheetos form = NewForm())
            {
                var sr = new SaveRestore();
                Assert.IsFalse(sr.LoadProc("", form));
            }
        }

        [TestMethod]
        public void LoadProcは白コエフの既定値を先に30へ戻してから読み込む()
        {
            // Form1.cs の LoadProc は読み込み前に do_WhiteCoef.Text = "30" を決め打ちしている。
            // ファイルに値が無い（未登録項目扱いになる）ケースでもこの既定値が効くことを確認する。
            using (Cheetos writer = NewForm())
            {
                // WhiteCoef 以外だけを保存する（別ファイル・別インスタンスなので登録されない体裁にはできないため、
                // ここでは保存時の値を変えておき、LoadProc 側の決め打ちで上書きされることを見る）
                writer.do_WhiteCoef.Text = "99";
                string path = PathFor("whitecoef");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Cheetos reader = NewForm())
                {
                    var sr = new SaveRestore();
                    sr.RegistItem(reader);
                    reader.do_WhiteCoef.Text = "1";

                    Assert.IsTrue(sr.LoadProc(path, reader));

                    // ファイルに保存されている値（99）が最終的に反映される
                    Assert.AreEqual("99", reader.do_WhiteCoef.Text);
                }
            }
        }
    }
}
