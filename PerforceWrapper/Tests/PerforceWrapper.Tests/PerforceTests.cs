using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PerforceWrapper.Tests
{
    /// <summary>
    /// Perforce（p4コマンドの文字列を組み立てるクラス、internal）のテスト。
    ///
    /// 実際に p4 コマンドやシェルを実行するクラスではなく、実行用のコマンド文字列を
    /// 組み立てるだけの純粋なロジック。そのため文字列比較で安全に検証できる。
    /// (以前はパスワードを平文の一時ファイルに書き出す副作用があったが、今はファイルを作らない)
    /// </summary>
    [TestClass]
    public class PerforceTests
    {
        [TestMethod]
        public void CreateCommandUseTreeはサーバーとワークスペースの環境変数を先頭に積む()
        {
            var p4 = new Perforce();
            p4.SetServerName("myserver:1666");
            p4.SetWorkspace("my_workspace");
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "set P4PORT=myserver:1666");
            StringAssert.Contains(command, "set P4CLIENT=my_workspace");
        }

        [TestMethod]
        public void CreateCommandUseTreeは未設定の環境変数を出力しない()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OperatorType.Sync);
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
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, @"//depot/main...");
        }

        [TestMethod]
        public void CreateCommandUseTreeはすでに3点リーダーが付いていれば重複させない()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main/...");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, @"//depot/main/...");
            Assert.IsFalse(command.Contains("......"), "3点リーダーが二重にならない");
        }

        [TestMethod]
        public void CreateCommandUseTreeは複数行のツリーをそれぞれコマンド化する()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree("//depot/a" + Environment.NewLine + "//depot/b");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "//depot/a...");
            StringAssert.Contains(command, "//depot/b...");
        }

        [TestMethod]
        public void CreateCommandUseTreeは空行を無視する()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree("//depot/a" + Environment.NewLine + Environment.NewLine + "//depot/b");

            string command = p4.CreateCommandUseTree();

            int occurrences = Regex.Matches(command, "p4 sync").Count;
            Assert.AreEqual(2, occurrences, "空行の分は p4 sync が増えないはず");
        }

        [TestMethod]
        public void リビジョンを指定しなければheadリビジョンになる()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "...#head");
        }

        [TestMethod]
        public void リビジョンを指定するとそのリビジョンになる()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main");
            p4.SetRevision("100");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "...@100");
        }

        // Perforce.OperatorType は internal な列挙型なので、public な [DataTestMethod] の
        // シグネチャには直接出せない（アクセシビリティの一貫性エラーになる）。
        // 列挙値の名前を文字列で渡し、メソッド内で Enum.Parse する。
        [DataTestMethod]
        [DataRow("Edit", "p4 edit ")]
        [DataRow("Revert", "p4 revert ")]
        [DataRow("Delete", "p4 delete ")]
        [DataRow("Sync", "p4 sync ")]
        [DataRow("Diff", "p4 diff2 -qt ")]
        public void OperatorTypeごとに対応するp4コマンドになる(string typeName, string expectedPrefix)
        {
            var type = (Perforce.OperatorType)Enum.Parse(typeof(Perforce.OperatorType), typeName);

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
            p4.SetOperatorType(Perforce.OperatorType.SetLabel);
            p4.SetLabelName("REL_1_0");
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "p4 tag -l REL_1_0 ");
        }

        [TestMethod]
        public void COPYとMERGEはブランチマップ名を含んだコマンドになる()
        {
            var copy = new Perforce();
            copy.SetOperatorType(Perforce.OperatorType.Copy);
            copy.SetBranchMapName("my_branch");
            copy.SetTargetTree(@"//depot/main");
            StringAssert.Contains(copy.CreateCommandUseTree(), "p4 copy -b my_branch -s ");

            var merge = new Perforce();
            merge.SetOperatorType(Perforce.OperatorType.Merge);
            merge.SetBranchMapName("my_branch");
            merge.SetTargetTree(@"//depot/main");
            StringAssert.Contains(merge.CreateCommandUseTree(), "p4 integrate -b my_branch -s ");
        }

        [TestMethod]
        public void CreateCommandDefinedは指定したコマンド文字列をそのまま使う()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OperatorType.Sync);

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
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "PAUSE");
        }

        [TestMethod]
        public void デバッグモードでなければPAUSEは付かない()
        {
            var p4 = new Perforce();
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            Assert.IsFalse(command.Contains("PAUSE"));
        }

        [TestMethod]
        public void ユーザー名とパスワードを設定してもバッチにはログインもパスワードも含めない()
        {
            // ログインはバッチより先に別プロセスで行い、パスワードは標準入力で渡す。
            // 以前はパスワードを平文で一時ファイルに書き出し、"cat <path> | p4 login" をバッチに入れていた
            const string dummyPassword = "dummy_test_password_12345";

            var p4 = new Perforce();
            p4.SetUserName("test_user");
            p4.SetUserPass(dummyPassword);
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "set P4USER=test_user");
            Assert.IsFalse(command.Contains("login"), "ログインはバッチの外で行う");
            Assert.IsFalse(command.Contains(dummyPassword), "パスワードはバッチに書かない");
            Assert.IsFalse(command.Contains("cat "), "Windows標準に無いcatは使わない");
            Assert.IsFalse(command.Contains("del "), "消すべきパスワードファイルを作らない");
        }

        [TestMethod]
        public void ユーザー名とパスワードを両方設定した時だけログインが必要になる()
        {
            var both = new Perforce();
            both.SetUserName("test_user");
            both.SetUserPass("dummy");
            Assert.IsTrue(both.NeedsLogin);

            var userOnly = new Perforce();
            userOnly.SetUserName("test_user");
            Assert.IsFalse(userOnly.NeedsLogin);

            var passOnly = new Perforce();
            passOnly.SetUserPass("dummy");
            Assert.IsFalse(passOnly.NeedsLogin);
        }

        [TestMethod]
        public void ログインの引数はバッチと同じ設定をグローバルオプションで渡す()
        {
            var p4 = new Perforce();
            p4.SetServerName("myserver:1666");
            p4.SetWorkspace("my_workspace");
            p4.SetCharset("utf8");
            p4.SetUserName("test_user");
            p4.SetUserPass("dummy_test_password_12345");

            string arguments = p4.CreateLoginArguments();

            Assert.AreEqual("-p myserver:1666 -c my_workspace -C utf8 -u test_user login -a", arguments);
            Assert.IsFalse(arguments.Contains("dummy_test_password_12345"), "パスワードはコマンドラインに出さない");
        }

        [TestMethod]
        public void ログインの引数は未設定の項目を省き空白を含む値は囲む()
        {
            var p4 = new Perforce();
            p4.SetWorkspace("my workspace");
            p4.SetUserName("test_user");

            Assert.AreEqual("-c \"my workspace\" -u test_user login -a", p4.CreateLoginArguments());
        }

        [TestMethod]
        public void ユーザー名だけでパスワードが無ければログインコマンドは追加されない()
        {
            var p4 = new Perforce();
            p4.SetUserName("test_user");
            p4.SetOperatorType(Perforce.OperatorType.Sync);
            p4.SetTargetTree(@"//depot/main");

            string command = p4.CreateCommandUseTree();

            StringAssert.Contains(command, "set P4USER=test_user");
            Assert.IsFalse(command.Contains("login -a"));
            Assert.IsFalse(p4.NeedsLogin);
        }
    }
}
