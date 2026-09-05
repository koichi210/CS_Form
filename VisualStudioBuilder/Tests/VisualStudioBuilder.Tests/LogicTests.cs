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
        public void CreateScriptHeaderはDEV_ENVとBUILD_OPTを設定する()
        {
            string result = Logic.CreateScriptHeader(@"C:\devenv.exe", "/rebuild release");

            StringAssert.Contains(result, @"set DEV_ENV=""C:\devenv.exe""");
            StringAssert.Contains(result, "set BUILD_OPT=/rebuild release");
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
        public void CreateBuildScriptはログ出力なしならdevenvコマンドのみ生成する()
        {
            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, false);

            StringAssert.Contains(result, @"%DEV_ENV% %BUILD_OPT% C:\proj\a.sln");
            Assert.IsFalse(result.Contains("/out"));
        }

        [TestMethod]
        public void CreateBuildScriptはログ出力ありならoutオプション付きで生成する()
        {
            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            string expectedLog = Path.Combine(tempDirectory, "a.log");
            StringAssert.Contains(result, "/out " + expectedLog);
        }

        [TestMethod]
        public void CreateBuildScriptは既存ログファイルがあれば削除コマンドを先頭に付ける()
        {
            string logPath = Path.Combine(tempDirectory, "a.log");
            File.WriteAllText(logPath, "old log");

            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            StringAssert.Contains(result, "del " + logPath);
        }

        [TestMethod]
        public void CreateBuildScriptは既存ログファイルがなければ削除コマンドを付けない()
        {
            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            Assert.IsFalse(result.Contains("del "));
        }
    }
}
