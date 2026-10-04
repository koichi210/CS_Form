namespace PlantUML
{
    /// <summary>
    /// もともと MainWindow.xaml.cs の Execute_Click に埋め込まれていた、PlantUML
    /// 実行コマンドの組み立てロジックをテストできる形に切り出したもの。実際のバッチファイル書き出しと
    /// プロセス起動(Process.Start/WaitForExit)は呼び出し元(MainWindow)に残し、
    /// コマンド文字列を組み立てる部分だけを渡した。File.Existsの結果も
    /// 呼び出し元で判定した値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static string BuildCommandParam(string plantumlPath, string configFile, bool configFileExists, string inFile)
        {
            string configParam = configFileExists ? " -config " + configFile : "";
            return "java -jar " + plantumlPath + configParam + " -charset UTF-8 " + inFile;
        }
    }
}
