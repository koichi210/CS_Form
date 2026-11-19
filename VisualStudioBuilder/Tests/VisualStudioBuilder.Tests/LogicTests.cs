using System;
using System.Collections.Generic;
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

            StringAssert.Contains(result, @"%DEV_ENV% %BUILD_OPT% ""C:\proj\a.sln""", "パスに空白があってもよいよう\"\"で囲む");
            Assert.IsFalse(result.Contains("/out"));
        }

        [TestMethod]
        public void CreateBuildScriptはログ出力ありならoutオプション付きで生成する()
        {
            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            // ログ名は「プロジェクトのパス＋ソリューション名」の\と:を_にしたもの
            string expectedLog = Path.Combine(tempDirectory, "C__proj_a.log");
            StringAssert.Contains(result, "/out \"" + expectedLog + "\" \"C:\\proj\\a.sln\"");
        }

        [TestMethod]
        public void CreateBuildScriptは既存ログファイルがあれば削除コマンドを先頭に付ける()
        {
            string logPath = Path.Combine(tempDirectory, "C__proj_a.log");
            File.WriteAllText(logPath, "old log");

            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            StringAssert.Contains(result, "del \"" + logPath + "\"");
        }

        [TestMethod]
        public void CreateBuildScriptは既存ログファイルがなければ削除コマンドを付けない()
        {
            string result = Logic.CreateBuildScript("○", "a.sln", @"C:\proj", tempDirectory, true);

            Assert.IsFalse(result.Contains("del "));
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

        // --- ログ名(プロジェクトのパス＋ソリューション名) -------------------------------

        [TestMethod]
        public void GetLogFileNameはパスとソリューション名の円記号とコロンをアンダーバーにする()
        {
            Assert.AreEqual("D__Work_Foo_Foo.log", Logic.GetLogFileName(@"D:\Work\Foo", "Foo.sln"));
            Assert.AreEqual("D__Work_Foo_Foo.log", Logic.GetLogFileName(@"D:\Work\Foo\", "Foo.sln"), "末尾の\\は1つにそろえる");
            Assert.AreEqual(@"C:\logs\D__Work_Foo_Foo.log", Logic.GetLogPathName(@"C:\logs", @"D:\Work\Foo", "Foo.sln"));
        }

        [TestMethod]
        public void GetLogFileNameは別フォルダの同じ名前のソリューションを別のログにする()
        {
            Assert.AreNotEqual(Logic.GetLogFileName(@"D:\A\App", "App.sln"), Logic.GetLogFileName(@"D:\B\App", "App.sln"));
        }

        [TestMethod]
        public void GetLogFileNameは拡張子が大文字のslnでもlogにする()
        {
            Assert.AreEqual("C__p_App.log", Logic.GetLogFileName(@"C:\p", "App.SLN"));
        }

        // --- devenv.exeのチェック --------------------------------------------------------

        [TestMethod]
        public void ValidateVisualStudioExePathはファイルがあればnullを返す()
        {
            Assert.IsNull(Logic.ValidateVisualStudioExePath(@"C:\VS\devenv.exe", path => path == @"C:\VS\devenv.exe"));
            Assert.IsNull(Logic.ValidateVisualStudioExePath(@" ""C:\VS\devenv.exe"" ", path => path == @"C:\VS\devenv.exe"), "前後の空白と\"\"は外して確かめる");
        }

        [TestMethod]
        public void ValidateVisualStudioExePathはファイルが無いか空欄ならメッセージを返す()
        {
            StringAssert.Contains(Logic.ValidateVisualStudioExePath(@"C:\VS\devenv.exe", path => false), "見つかりません");
            StringAssert.Contains(Logic.ValidateVisualStudioExePath("", path => true), "空欄");
            StringAssert.Contains(Logic.ValidateVisualStudioExePath("   ", path => true), "空欄");
        }

        // --- 存在しないリスト -------------------------------------------------------------

        [TestMethod]
        public void FindMissingSolutionsはビルド対象で見つからないものだけを行番号付きで返す()
        {
            var targets = new List<BuildTarget>
            {
                new BuildTarget { BuildEnable = "○", SolutionName = "a.sln", ProjectPath = @"C:\a" },
                new BuildTarget { BuildEnable = "○", SolutionName = "b.sln", ProjectPath = @"C:\b" },
                new BuildTarget { BuildEnable = "×", SolutionName = "c.sln", ProjectPath = @"C:\c" },
                new BuildTarget { BuildEnable = "○", SolutionName = "", ProjectPath = "" },
                new BuildTarget { BuildEnable = "○", SolutionName = "", ProjectPath = @"C:\e" },
            };

            List<string> missing = Logic.FindMissingSolutions(targets, path => path == @"C:\a\a.sln");

            CollectionAssert.AreEqual(new[] { @"No.2 C:\b\b.sln", @"No.5 C:\e\" }, missing,
                "ビルド対象外(×)と空欄の行は数えず、ソリューション名が無い行は見つからない扱い");
        }

        [TestMethod]
        public void FormatRowCountは登録数とビルド対象数を出す()
        {
            Assert.AreEqual("登録数: 3 件 (ビルド対象: 2 件)", Logic.FormatRowCount(3, 2));
        }

        // --- ビルド後に削除するもの(複数行) -----------------------------------------------

        [TestMethod]
        public void ParseDeleteTargetsは1行に1つずつ分け空行と前後の空白と重複を捨てる()
        {
            string[] invalid;
            string[] targets = Logic.ParseDeleteTargets(" obj \r\n\r\n*.ncb\nOBJ\r*.suo", out invalid);

            CollectionAssert.AreEqual(new[] { "obj", "*.ncb", "*.suo" }, targets);
            Assert.AreEqual(0, invalid.Length);
        }

        [TestMethod]
        public void ParseDeleteTargetsはパスや全部に当たるパターンを使わない()
        {
            string[] invalid;
            string[] targets = Logic.ParseDeleteTargets("obj\r\n*\r\n*.*\r\n..\r\nbin\\Debug\r\nC:\\tmp\r\na/b\r\na|b", out invalid);

            CollectionAssert.AreEqual(new[] { "obj" }, targets);
            CollectionAssert.AreEqual(new[] { "*", "*.*", "..", @"bin\Debug", @"C:\tmp", "a/b", "a|b" }, invalid);
        }

        [TestMethod]
        public void ParseDeleteTargetsは空欄やnullなら何も返さない()
        {
            string[] invalid;
            Assert.AreEqual(0, Logic.ParseDeleteTargets("", out invalid).Length);
            Assert.AreEqual(0, Logic.ParseDeleteTargets(null, out invalid).Length);
        }

        [TestMethod]
        public void DeleteTargetsByPatternsはフォルダ名とファイルのパターンの両方を階層を問わず削除する()
        {
            string objAtRoot = Path.Combine(tempDirectory, "obj");
            string nested = Path.Combine(tempDirectory, "Sub");
            string objNested = Path.Combine(nested, "obj", "Debug");
            string keepBin = Path.Combine(tempDirectory, "bin");
            Directory.CreateDirectory(objAtRoot);
            Directory.CreateDirectory(objNested);
            Directory.CreateDirectory(keepBin);
            string ncbAtRoot = Path.Combine(tempDirectory, "App.ncb");
            string ncbNested = Path.Combine(nested, "Lib.ncb");
            string keepCpp = Path.Combine(nested, "main.cpp");
            File.WriteAllText(ncbAtRoot, "x");
            File.WriteAllText(ncbNested, "x");
            File.WriteAllText(keepCpp, "x");
            File.WriteAllText(Path.Combine(objNested, "a.obj"), "x");

            int deleted = Logic.DeleteTargetsByPatterns(tempDirectory, new[] { "obj", "*.ncb" });

            Assert.IsFalse(Directory.Exists(objAtRoot));
            Assert.IsFalse(Directory.Exists(Path.Combine(nested, "obj")));
            Assert.IsFalse(File.Exists(ncbAtRoot));
            Assert.IsFalse(File.Exists(ncbNested));
            Assert.IsTrue(Directory.Exists(keepBin));
            Assert.IsTrue(File.Exists(keepCpp));
            Assert.AreEqual(4, deleted, "objフォルダ2つと.ncbファイル2つ");
        }

        [TestMethod]
        public void DeleteTargetsByPatternsはルートが無くても例外にならない()
        {
            Assert.AreEqual(0, Logic.DeleteTargetsByPatterns(Path.Combine(tempDirectory, "missing"), new[] { "obj" }));
            Assert.AreEqual(0, Logic.DeleteTargetsByPatterns(tempDirectory, null));
        }
    }
}
