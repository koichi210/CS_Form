using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PerforceWrapper.Tests
{
    /// <summary>
    /// PerforceWrapper.SaveRestore（StcSaveRestore を継承した設定保存クラス）のテスト。
    ///
    /// Cheetos / FFEdit と同じ方式で、実際の Form1 を生成して確認する。ここでは
    /// これまで扱っていなかった RegistSecureCtrl（パスワードの暗号化保存）も対象になる。
    /// </summary>
    [TestClass]
    public class SaveRestoreTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "PerforceWrapperSaveRestoreTests_" + Guid.NewGuid().ToString("N"));
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
        public void サーバー接続設定が保存して読み直すと戻る()
        {
            using (Form1 writer = NewForm())
            {
                writer.comboBox_perforce_server.Text = "myserver:1666";
                writer.comboBox_perforce_user.Text = "test_user";
                writer.comboBox_perforce_workspace.Text = "my_workspace";
                writer.comboBox_perforce_charset.Text = "utf8";
                writer.textBox_tree_list.Text = "//depot/main";

                string path = PathFor("server");
                Assert.IsTrue(NewSaveRestore(writer).SaveXmlFile(path));

                using (Form1 reader = NewForm())
                {
                    Assert.IsTrue(NewSaveRestore(reader).LoadXmlFile(path));

                    Assert.AreEqual("myserver:1666", reader.comboBox_perforce_server.Text);
                    Assert.AreEqual("test_user", reader.comboBox_perforce_user.Text);
                    Assert.AreEqual("my_workspace", reader.comboBox_perforce_workspace.Text);
                    Assert.AreEqual("utf8", reader.comboBox_perforce_charset.Text);
                    Assert.AreEqual("//depot/main", reader.textBox_tree_list.Text);
                }
            }
        }

        [TestMethod]
        public void パスワードは暗号化されて保存され読み直すと復号される()
        {
            // ダミーの値。実在のパスワードは使わない。
            const string dummyPassword = "dummy_test_password_98765";

            using (Form1 writer = NewForm())
            {
                writer.textbox_perforce_password.Text = dummyPassword;

                string path = PathFor("password");
                NewSaveRestore(writer).SaveXmlFile(path);

                // 保存されたXMLの中に平文のパスワードがそのまま出ていないことを確認する
                string xml = File.ReadAllText(path);
                Assert.IsFalse(xml.Contains(dummyPassword), "平文のまま保存されてはいけない");

                using (Form1 reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(dummyPassword, reader.textbox_perforce_password.Text, "復号すると元のパスワードに戻る");
                }
            }
        }

        [TestMethod]
        public void 操作種別のラジオボタンが保存して読み直すと戻る()
        {
            using (Form1 writer = NewForm())
            {
                writer.radioButton_so_menu_checkout.Checked = true;
                writer.textBox_so_changelist.Text = "12345";

                string path = PathFor("radio");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Form1 reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.IsTrue(reader.radioButton_so_menu_checkout.Checked);
                    Assert.IsFalse(reader.radioButton_so_menu_get_latest.Checked, "既定値の True から書き換わるはず");
                    Assert.AreEqual("12345", reader.textBox_so_changelist.Text);
                }
            }
        }

        [TestMethod]
        public void ラベルとブランチマップの設定が保存して読み直すと戻る()
        {
            using (Form1 writer = NewForm())
            {
                writer.textBox_sl_label_name.Text = "REL_1_0";
                writer.textBox_sl_base_changelist.Text = "100";
                writer.textBox_dl_src_label_name.Text = "REL_1_0";
                writer.textBox_dl_src_tree.Text = "//depot/main";
                writer.textBox_dl_dest_label_name.Text = "REL_2_0";
                writer.textBox_dl_dest_tree.Text = "//depot/branch";
                writer.textBox_ak_label_name.Text = "REL_3_0";
                writer.textBox_ak_branch_map.Text = "my_branch";
                writer.radioButton_al_merge.Checked = true;

                string path = PathFor("label");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Form1 reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual("REL_1_0", reader.textBox_sl_label_name.Text);
                    Assert.AreEqual("100", reader.textBox_sl_base_changelist.Text);
                    Assert.AreEqual("REL_1_0", reader.textBox_dl_src_label_name.Text);
                    Assert.AreEqual("//depot/main", reader.textBox_dl_src_tree.Text);
                    Assert.AreEqual("REL_2_0", reader.textBox_dl_dest_label_name.Text);
                    Assert.AreEqual("//depot/branch", reader.textBox_dl_dest_tree.Text);
                    Assert.AreEqual("REL_3_0", reader.textBox_ak_label_name.Text);
                    Assert.AreEqual("my_branch", reader.textBox_ak_branch_map.Text);
                    Assert.IsTrue(reader.radioButton_al_merge.Checked);
                    Assert.IsFalse(reader.radioButton_al_copy.Checked, "既定値の True から書き換わるはず");
                }
            }
        }

        [TestMethod]
        public void コンボボックスの入力履歴が保存して読み直すと戻る()
        {
            using (Form1 writer = NewForm())
            {
                writer.comboBox_perforce_server.Items.Add("server1");
                writer.comboBox_perforce_server.Items.Add("server2");

                string path = PathFor("history");
                NewSaveRestore(writer).SaveXmlFile(path);

                using (Form1 reader = NewForm())
                {
                    NewSaveRestore(reader).LoadXmlFile(path);

                    Assert.AreEqual(2, reader.comboBox_perforce_server.Items.Count);
                    Assert.AreEqual("server1", reader.comboBox_perforce_server.Items[0]);
                    Assert.AreEqual("server2", reader.comboBox_perforce_server.Items[1]);
                }
            }
        }

        [TestMethod]
        public void LoadProcはファイル名が空なら何もせず失敗を返す()
        {
            var sr = new SaveRestore();
            Assert.IsFalse(sr.LoadProc(""));
        }

        [TestMethod]
        public void SaveSettingはファイル名が空なら何もせず失敗を返す()
        {
            using (Form1 form = NewForm())
            {
                var sr = new SaveRestore();
                sr.RegistItem(form);
                Assert.IsFalse(sr.SaveSetting("", form));
            }
        }
    }
}
