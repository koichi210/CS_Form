using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisualStudioBuilder.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、ビルドスクリプト生成・パス組み立てロジック）
    /// のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        private string tempDirectory;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "VisualStudioBuilderTests_" + Guid.NewGuid().ToString("N"));
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
        public void GetFilePathNameは末尾のバックスラッシュを吸収して連結する()
        {
            string result = Logic.GetFilePathName(@"C:\proj\", "solution", ".sln");
            Assert.AreEqual(@"C:\proj\solution.sln", result);
        }

        [TestMethod]
        public void GetSolutionPathNameは拡張子を追加しない()
        {
            string result = Logic.GetSolutionPathName(@"C:\proj", "solution.sln");
            Assert.AreEqual(@"C:\proj\solution.sln", result);
        }

        [TestMethod]
        public void GetLogPathNameはslnをlogに置き換える()
        {
            string result = Logic.GetLogPathName(@"C:\logs", "solution.sln");
            Assert.AreEqual(@"C:\logs\solution.log", result);
        }

        [TestMethod]
        public void CreateScriptHeaderはMSBUILDとBUILD_OPTを設定する()
        {
            string result = Logic.CreateScriptHeader(@"C:\MSBuild.exe", "/t:Rebuild /p:Configuration=Release");

            StringAssert.Contains(result, @"set MSBUILD=""C:\MSBuild.exe""");
            StringAssert.Contains(result, "set BUILD_OPT=/t:Rebuild /p:Configuration=Release");
        }

        [TestMethod]
        public void CreateBuildScriptはビルド無効なら空文字を返す()
        {
            string result = Logic.CreateBuildScript("×", "a.sln", @"C:\proj", tempDirectory, false);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void CreateBuildScriptはソリューション名が空なら空文字を返す()
        {
            string result = Logic.CreateBuildScript("○", "", @"C:\proj", tempDirectory, false);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void CreateBuildScriptはログ出力なしならmsbuildコマンドのみ生成する()
        {
            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, false);

            StringAssert.Contains(result, @"%MSBUILD% ""C:\proj\a.sln"" %BUILD_OPT%");
            Assert.IsFalse(result.Contains("/fl"));
        }

        [TestMethod]
        public void CreateBuildScriptはログ出力ありならファイルロガー付きで生成する()
        {
            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            string expectedLog = Path.Combine(tempDirectory, "a.log");
            StringAssert.Contains(result, @"%MSBUILD% ""C:\proj\a.sln"" %BUILD_OPT% /fl ""/flp:logfile=" + expectedLog + @";verbosity=minimal""");
        }

        [TestMethod]
        public void CreateBuildScriptは既存ログファイルがあれば削除コマンドを先頭に付ける()
        {
            string logPath = Path.Combine(tempDirectory, "a.log");
            File.WriteAllText(logPath, "old log");

            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            StringAssert.StartsWith(result, @"del """ + logPath + @"""");
        }

        [TestMethod]
        public void CreateBuildScriptは既存ログファイルがなければ削除コマンドを付けない()
        {
            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            Assert.IsFalse(result.Contains("del "));
        }

        // --- ValidateMsBuildPath(ビルド前のMSBuild.exeの確認) ------------------------
        // ファイルの有無は差し替えられるので、実ファイル無しで判定だけを検証できる

        [TestMethod]
        public void ValidateMsBuildPathは存在するMSBuild_exeなら空文字を返す()
        {
            string result = Logic.ValidateMsBuildPath(Logic.DefaultMsBuildPath, path => true);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ValidateMsBuildPathはファイル名の大文字小文字を区別しない()
        {
            string result = Logic.ValidateMsBuildPath(@"C:\Tools\msbuild.EXE", path => true);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ValidateMsBuildPathは空欄ならエラーを返す()
        {
            Assert.AreNotEqual("", Logic.ValidateMsBuildPath("", path => true));
            Assert.AreNotEqual("", Logic.ValidateMsBuildPath("  ", path => true));
        }

        [TestMethod]
        public void ValidateMsBuildPathは存在しなければエラーを返す()
        {
            string result = Logic.ValidateMsBuildPath(Logic.DefaultMsBuildPath, path => false);

            StringAssert.Contains(result, "見つかりません");
            StringAssert.Contains(result, Logic.DefaultMsBuildPath);
        }

        [TestMethod]
        public void ValidateMsBuildPathは旧設定のdevenv_exeならファイルがあってもエラーを返す()
        {
            string devenv = @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe";

            string result = Logic.ValidateMsBuildPath(devenv, path => true);

            StringAssert.Contains(result, "MSBuild.exe");
            StringAssert.Contains(result, Logic.DefaultBuildOption);
        }

        // --- ClassifyBuildResult(ビルド結果の振り分け) ---------------------------------
        // ログの中身を見る部分は差し替えられるので、実ファイル無しで判定だけを検証できる

        [TestMethod]
        public void ビルド結果はエラー語を含むログだけ失敗に振り分けられる()
        {
            String[] logs = { "a.log", "b.log", "c.log" };

            String result = Logic.ClassifyBuildResult(logs, "error", false, "",
                (path, word) => path == "b.log");

            StringAssert.Contains(result, "[ビルド成功]" + Environment.NewLine + "a.log");
            StringAssert.Contains(result, "[ビルド失敗]" + Environment.NewLine + "b.log");
            Assert.IsFalse(result.Contains("[実行ファイルの上書きに失敗]"), "除外指定が無ければその欄は出ない");
        }

        [TestMethod]
        public void 上書き失敗の語を含むログは上書き失敗にも振り分けられる()
        {
            String[] logs = { "a.log", "b.log" };

            String result = Logic.ClassifyBuildResult(logs, "error", true, "locked",
                (path, word) => path == "b.log" && word == "locked");

            StringAssert.Contains(result, "[実行ファイルの上書きに失敗]" + Environment.NewLine + "b.log");
            // 上書きに失敗した分は成功側に入らない
            int successIdx = result.IndexOf("[ビルド成功]");
            int excludeIdx = result.IndexOf("[実行ファイルの上書きに失敗]");
            String successPart = result.Substring(successIdx, excludeIdx - successIdx);
            Assert.IsFalse(successPart.Contains("b.log"));
            StringAssert.Contains(successPart, "a.log");
        }

        [TestMethod]
        public void ログが1件も無ければ見出しだけが返る()
        {
            String result = Logic.ClassifyBuildResult(new String[0], "error", false, "",
                (path, word) => false);

            StringAssert.Contains(result, "[ビルド成功]");
            StringAssert.Contains(result, "[ビルド失敗]");
        }

        // --- DeleteDirectoriesByName(ビルド後の任意ディレクトリ削除) ---------------------
        // checkBox_DeleteDirectoryをオンにしても削除されない不具合(未実装)の再発防止。
        // 名前が一致するフォルダだけ、深い階層・浅い階層どちらにあっても再帰的に消えること、
        // 一致しないフォルダは残ることを確認する

        [TestMethod]
        public void DeleteDirectoriesByNameは名前が一致するフォルダだけを階層を問わず削除する()
        {
            // ルート直下のobj、サブフォルダ配下のobj(入れ子)、関係ないbinフォルダを用意
            string objAtRoot = Path.Combine(tempDirectory, "obj");
            string keepBin = Path.Combine(tempDirectory, "bin");
            string nested = Path.Combine(tempDirectory, "SubProject");
            string objNested = Path.Combine(nested, "obj");
            string objNestedChild = Path.Combine(objNested, "Debug");
            Directory.CreateDirectory(objAtRoot);
            Directory.CreateDirectory(keepBin);
            Directory.CreateDirectory(objNestedChild);
            File.WriteAllText(Path.Combine(objAtRoot, "dummy.txt"), "dummy");
            File.WriteAllText(Path.Combine(objNestedChild, "dummy.txt"), "dummy");

            Logic.DeleteDirectoriesByName(tempDirectory, "obj");

            Assert.IsFalse(Directory.Exists(objAtRoot), "ルート直下のobjは削除される");
            Assert.IsFalse(Directory.Exists(objNested), "入れ子になったobjも削除される(子フォルダobj\\Debugごと)");
            Assert.IsTrue(Directory.Exists(keepBin), "名前が一致しないbinは残る");
            Assert.IsTrue(Directory.Exists(nested), "obj自体ではない親フォルダ(SubProject)は残る");
        }

        [TestMethod]
        public void DeleteDirectoriesByNameはディレクトリ名が空なら何もしない()
        {
            string objAtRoot = Path.Combine(tempDirectory, "obj");
            Directory.CreateDirectory(objAtRoot);

            Logic.DeleteDirectoriesByName(tempDirectory, "");

            Assert.IsTrue(Directory.Exists(objAtRoot), "ディレクトリ名が空の場合は何も削除しない");
        }

        [TestMethod]
        public void DeleteDirectoriesByNameはルートが存在しなくても例外を投げない()
        {
            string missingRoot = Path.Combine(tempDirectory, "NotExist");

            Logic.DeleteDirectoriesByName(missingRoot, "obj");
            // ここまで到達すれば例外が起きていないのでOK
        }

        [TestMethod]
        public void DeleteDirectoriesByNameは一致するフォルダが無ければ何もしない()
        {
            string keepBin = Path.Combine(tempDirectory, "bin");
            Directory.CreateDirectory(keepBin);

            Logic.DeleteDirectoriesByName(tempDirectory, "obj");

            Assert.IsTrue(Directory.Exists(keepBin));
        }
    }
}
