using System;

namespace PlantUML
{
    /// <summary>
    /// もともと MainWindow.xaml.cs の button1_Click に埋め込まれていた、PlantUML
    /// 実行コマンドの組み立てロジックをテストできる形に切り出したもの。コードは
    /// そのまま移しただけで書き換えていない。実際のバッチファイル書き出しと
    /// プロセス起動(Process.Start/WaitForExit)は呼び出し元(MainWindow)に残し、
    /// コマンド文字列を組み立てる部分だけを渡した。File.Existsの結果も
    /// 呼び出し元で判定した値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static String BuildCommandParam(String PlantumlPath, String ConfigFile, Boolean ConfigFileExists, String InFile)
        {
            String CommandParam;

            CommandParam = string.Format(@"java -jar " + PlantumlPath);
            if (ConfigFileExists)
            {
                CommandParam += string.Format(" -config {0}", ConfigFile);
            }
            CommandParam += string.Format(" -charset UTF-8 {0}", InFile);

            return CommandParam;
        }
    }
}
