using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlantUML.Tests
{
    /// <summary>
    /// Logic（MainWindow.xaml.cs から切り出した、PlantUML実行コマンドの組み立て
    /// ロジック）のテスト。
    ///
    /// ⚠️ button1_Click は実際にバッチファイルを書き出しProcess.Start+WaitForExit
    /// で外部プロセス(java)を実行するため、テスト環境で安全に実行できない。
    /// この抽出したコマンド組み立て部分だけをテスト対象にする。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void ConfigFileが存在する場合はconfigオプションを含む()
        {
            string result = Logic.BuildCommandParam(@"plantuml.jar", @"sample\config.txt", ConfigFileExists: true, InFile: @"sample\sequence.puml");

            Assert.AreEqual(@"java -jar plantuml.jar -config sample\config.txt -charset UTF-8 sample\sequence.puml", result);
        }

        [TestMethod]
        public void ConfigFileが存在しない場合はconfigオプションを含まない()
        {
            string result = Logic.BuildCommandParam(@"plantuml.jar", @"sample\config.txt", ConfigFileExists: false, InFile: @"sample\sequence.puml");

            Assert.AreEqual(@"java -jar plantuml.jar -charset UTF-8 sample\sequence.puml", result);
        }

        [TestMethod]
        public void 常にUTF8の文字コード指定と入力ファイルが末尾に付く()
        {
            string result = Logic.BuildCommandParam(@"C:\tools\plantuml.jar", @"cfg.txt", ConfigFileExists: false, InFile: @"diagram.puml");

            StringAssert.EndsWith(result, "-charset UTF-8 diagram.puml");
        }
    }
}
