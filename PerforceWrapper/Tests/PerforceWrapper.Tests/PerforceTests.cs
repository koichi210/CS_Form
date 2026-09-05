using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PerforceWrapper.Tests
{
    /// <summary>
    /// Perforce（p4コマンドの文字列を組み立てるクラス、internal）のテスト。
    ///
    /// 実際に p4 コマンドやシェルを実行するクラスではなく、実行用のコマンド文字列を
    /// 組み立てるだけの純粋なロジック。そのため文字列比較で安全に検証できる。
    ///
    /// ⚠️ 例外が1つある: ユーザー名とパスワードを両方設定すると、CreateEnvCommand が
    /// パスワードを平文のまま一時ファイルに書き出す（CreatePasswordFile）。コマンド文字列を
    /// 組み立てるだけのつもりが、実際にディスクへ書き込む副作用を持っている。
    /// テストではダミーの値しか使わないが、後片付けとして生成された一時ファイルを
    /// コマンド文字列から拾って確実に削除する。
    /// </summary>
    [TestClass]
    public class PerforceTests
    {
        [TestCleanup]
        public void TearDown()
        {
            // CreateEnvCommand が作った一時パスワードファイルを、生成されたコマンド文字列から
            // 拾って削除する。"cat <path> | " という形で埋め込まれている。
            foreach (string path in tempPasswordFilesToClean)
            {
                try
                {
                    if (File.Exists(path)) File.Delete(path);
                }
                catch (IOException)
                {
                    // 後片付けの失敗はテストの成否に関係ないので黙って流す
                }
            }
            tempPasswordFilesToClean.Clear();
        }

        private readonly System.Collections.Generic.List<string> tempPasswordFilesToClean = new System.Collections.Generic.List<string>();

        /// <summary>生成されたコマンド文字列から一時パスワードファイルのパスを抜き出して、後で消す対象に登録する。</summary>
        private void RememberPasswordFileIfAny(string command)
        {
            Match m = Regex.Match(command, @"cat (.+?) \|");
            if (m.Success)
            {
                tempPasswordFilesToClean.Add(m.Groups[1].Value);
            }
        }

        [TestMethod]
        public void CreateCommandUseTreeはサーバーとワークスペースの環境変数を先頭に積む()
        {
            var p4 = new Perforce();
            p4.SetServerName("myserver:1666");
            p4.SetWorkspace("my_workspace");
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "set P4PORT=myserver:1666");
            StringAssert.Contains(command, "set P4CLIENT=my_workspace");
        }

        [TestMethod]
        public void CreateCommandUseTreeは未設定の環境変数を出力しない()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            Assert.IsFalse(command.Contains("P4PORT"));
            Assert.IsFalse(command.Contains("P4CLIENT"));
            Assert.IsFalse(command.Contains("P4CHARSET"));
            Assert.IsFalse(command.Contains("P4USER"));
        }

        [TestMethod]
        public void CreateCommandUseTreeはツリーの末尾に3点リーダーを補って再帰指定にする()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, @"//depot/main...");
        }

        [TestMethod]
        public void CreateCommandUseTreeはすでに3点リーダーが付いていれば重複させない()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main/...");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, @"//depot/main/...");
            Assert.IsFalse(command.Contains("......"), "3点リーダーが二重にならない");
        }

        [TestMethod]
        public void CreateCommandUseTreeは複数行のツリーをそれぞれコマンド化する()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree("//depot/a" + Environment.NewLine + "//depot/b");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "//depot/a...");
            StringAssert.Contains(command, "//depot/b...");
        }

        [TestMethod]
        public void CreateCommandUseTreeは空行を無視する()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree("//depot/a" + Environment.NewLine + Environment.NewLine + "//depot/b");

            string command = p4.CreateCommandUseTree();

            int occurrences = Regex.Matches(command, "p4 sync").Count;
            Assert.AreEqual(2, occurrences, "空行の分は p4 sync が増えないはず");
        }

        [TestMethod]
        public void リビジョンを指定しなければheadリビジョンになる()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "...#head");
        }

        [TestMethod]
        public void リビジョンを指定するとそのリビジョンになる()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");
            p4.SetRevision("100");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "...@100");
        }

        // Perforce.OPERATOR_TYPE は internal な列挙型なので、public な [DataTestMethod] の
        // シグネチャには直接出せない（アクセシビリティの一貫性エラーになる）。
        // 列挙値の名前を文字列で渡し、メソッド内で Enum.Parse する。
        [DataTestMethod]
        [DataRow("EDIT", "p4 edit ")]
        [DataRow("REVENT", "p4 revert ")]
        [DataRow("DELETE", "p4 delete ")]
        [DataRow("SYNC", "p4 sync ")]
        [DataRow("DIFF", "p4 diff2 -qt ")]
        public void OperatorTypeごとに対応するp4コマンドになる(string typeName, string expectedPrefix)
        {
            var type = (Perforce.OPERATOR_TYPE)Enum.Parse(typeof(Perforce.OPERATOR_TYPE), typeName);

            var p4 = new Perforce();
            p4.SetOperatorType(type);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, expectedPrefix);
        }

        [TestMethod]
        public void SET_LABELはラベル名を含んだtagコマンドになる()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SET_LABEL);
            p4.SetLabelName("REL_1_0");
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "p4 tag -l REL_1_0 ");
        }

        [TestMethod]
        public void COPYとMERGEはブランチマップ名を含んだコマンドになる()
        {
            var copy = new Perforce();
            copy.SetOperatorType(Perforce.OPERATOR_TYPE.COPY);
            copy.SetBranchMapName("my_branch");
            copy.SetTargetTree(@"//depot/main");
            StringAssert.Contains(copy.CreateCommandUseTree(), "p4 copy -b my_branch -s ");

            var merge = new Perforce();
            merge.SetOperatorType(Perforce.OPERATOR_TYPE.MERGE);
            merge.SetBranchMapName("my_branch");
            merge.SetTargetTree(@"//depot/main");
            StringAssert.Contains(merge.CreateCommandUseTree(), "p4 integrate -b my_branch -s ");
        }

        [TestMethod]
        public void CreateCommandDefinedは指定したコマンド文字列をそのまま使う()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);

            string command = p4.CreateCommandDefined("//depot/main/file.txt#5");

            StringAssert.Contains(command, "p4 sync //depot/main/file.txt#5");
        }

        [TestMethod]
        public void GetLabelDesignationPathNameはパスにラベル名を付与する()
        {
            var p4 = new Perforce();

            string result = p4.GetLabelDesignationPathName(@"//depot/main", "REL_1_0");

            Assert.AreEqual(@"//depot/main...@REL_1_0", result);
        }

        [TestMethod]
        public void デバッグモードだと末尾にPAUSEが付く()
        {
            var p4 = new Perforce();
            p4.SetDebugMode(true);
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "PAUSE");
        }

        [TestMethod]
        public void デバッグモードでなければPAUSEは付かない()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            Assert.IsFalse(command.Contains("PAUSE"));
        }

        [TestMethod]
        public void ユーザー名とパスワードを設定するとログイン用コマンドが追加される()
        {
            // ダミーの値。実在のパスワードは使わない。
            const string dummyPassword = "dummy_test_password_12345";

            var p4 = new Perforce();
            p4.SetUserName("test_user");
            p4.SetUserPass(dummyPassword);
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();
            RememberPasswordFileIfAny(command);

            StringAssert.Contains(command, "set P4USER=test_user");
            StringAssert.Contains(command, "p4 -u test_user login -a");
            StringAssert.Contains(command, "del ", "後片付け用の del コマンドが末尾に積まれる");
        }

        [TestMethod]
        public void パスワードは平文のまま一時ファイルに書き出される()
        {
            // これは仕様として現状こうなっている、という記録。
            // コマンド文字列を組み立てるだけに見えて、実際にはディスクへの書き込みを伴う。
            const string dummyPassword = "dummy_test_password_12345";

            var p4 = new Perforce();
            p4.SetUserName("test_user");
            p4.SetUserPass(dummyPassword);
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();
            RememberPasswordFileIfAny(command);

            Match m = Regex.Match(command, @"cat (.+?) \|");
            Assert.IsTrue(m.Success, "パスワードファイルのパスがコマンド中に埋め込まれているはず");

            string path = m.Groups[1].Value;
            Assert.IsTrue(File.Exists(path), "実際にファイルが作られている");
            Assert.AreEqual(dummyPassword, File.ReadAllText(path), "中身は平文のパスワードそのもの");
        }

        [TestMethod]
        public void ユーザー名だけでパスワードが無ければログインコマンドは追加されない()
        {
            var p4 = new Perforce();
            p4.SetUserName("test_user");
            p4.SetOperatorType(Perforce.OPERATOR_TYPE.SYNC);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "set P4USER=test_user");
            Assert.IsFalse(command.Contains("login -a"));
        }
    }
}
