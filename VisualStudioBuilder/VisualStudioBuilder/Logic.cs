using System;
using System.IO;
using System.Linq;
using System.Text;

namespace VisualStudioBuilder
{
    /// <summary>
    /// もともと Form1.cs に実装されていた、ビルドスクリプト生成やパス組み立てに関する
    /// ロジックをテストできる形に切り出したもの。DataGridView/TextBox など Form のコントロール参照は、
    /// 呼び出し元(Form1)で読み取った値を引数として渡す形に変えた。
    ///
    /// BuildListEnable / SlnExtension はForm1側(グリッドの初期化・一括登録)からも参照する。
    /// </summary>
    internal static class Logic
    {
        // グリッドの「ビルド」列で、ビルド対象を表す値
        public const String BuildListEnable = "○";
        public const String SlnExtension = ".sln";
        private const String _logExtension = ".log";
        private const String _msBuildFileName = "MSBuild.exe";

        // 設定ファイルに値が無いときの既定値([[SaveRestore.cs]]から参照)
        public const String DefaultMsBuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";
        public const String DefaultBuildOption = "/t:Rebuild /p:Configuration=Release";

        public static String GetFilePathName(String pathName, String fileName, String ext = "")
        {
            return pathName.TrimEnd('\\') + '\\' + fileName + ext;
        }

        public static String GetSolutionPathName(String pathName, String fileName)
        {
            return GetFilePathName(pathName, fileName);
        }

        public static String GetLogPathName(String pathName, String fileName)
        {
            return GetFilePathName(pathName, fileName.Replace(SlnExtension, _logExtension));
        }

        // ビルドに使うMSBuild.exeのパスを確かめる。問題なければ空文字、問題があれば
        // 画面に出すメッセージを返す。ファイルの有無は呼び出し側から渡してもらう
        // (テストで実ファイル無しに判定できるように)。
        // 以前はdevenv.exeでビルドしていたので、古い設定ファイルを読むとdevenv.exeのパスが
        // 入っている。そのまま動かすとオプションの書式が違って失敗するため、ここで弾いて案内する
        public static String ValidateMsBuildPath(String msBuildPath, Func<String, Boolean> fileExists)
        {
            if (String.IsNullOrWhiteSpace(msBuildPath))
            {
                return "MSBuildパスが空欄です。MSBuild.exeのパスを指定してください";
            }

            if (!String.Equals(Path.GetFileName(msBuildPath), _msBuildFileName, StringComparison.OrdinalIgnoreCase))
            {
                return "MSBuildパスには" + _msBuildFileName + "を指定してください" + Environment.NewLine
                    + "(devenv.exeでのビルドは廃止しました。ビルドオプションも「" + DefaultBuildOption + "」の書式に変えてください)" + Environment.NewLine
                    + msBuildPath;
            }

            if (!fileExists(msBuildPath))
            {
                return "MSBuild.exeが見つかりません" + Environment.NewLine + msBuildPath;
            }

            return "";
        }

        public static String CreateScriptHeader(String msBuildPath, String buildOption)
        {
            // 例: set MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
            //     set BUILD_OPT=/t:Rebuild /p:Configuration=Release
            return @"set MSBUILD=""" + msBuildPath + @"""" + Environment.NewLine
                + "set BUILD_OPT=" + buildOption + Environment.NewLine
                + Environment.NewLine;
        }

        public static String CreateBuildScript(String buildEnable, String solutionName, String projectPath, String logDirectory, Boolean isExportLog)
        {
            // パラメータチェック
            if (buildEnable != BuildListEnable ||
                solutionName == String.Empty ||
                projectPath == String.Empty)
            {
                return "";
            }

            String solutionPath = GetSolutionPathName(projectPath, solutionName);
            String logName = GetLogPathName(logDirectory, solutionName);

            // パスに空白が入っても1つの引数として渡るよう、ダブルクォートで囲む
            var script = new StringBuilder();
            if (isExportLog)
            {
                // ファイルが存在したら削除
                if (File.Exists(logName))
                {
                    script.Append("del \"").Append(logName).Append('"').Append(Environment.NewLine);
                }
                // ログはエラーと警告だけにする(verbosity=minimal)。既定の詳しさだとコンパイラの
                // コマンドライン(/errorreport:prompt等)まで載り、成功したビルドでも
                // 検知ワード「error」に引っかかってしまうため
                script.Append("%MSBUILD% \"").Append(solutionPath).Append("\" %BUILD_OPT%")
                      .Append(" /fl \"/flp:logfile=").Append(logName).Append(";verbosity=minimal\"")
                      .Append(Environment.NewLine);
            }
            else
            {
                script.Append("%MSBUILD% \"").Append(solutionPath).Append("\" %BUILD_OPT%").Append(Environment.NewLine);
            }
            script.Append(Environment.NewLine);

            return script.ToString();
        }

        // ビルド結果(ログの一覧)を成功/失敗/上書き失敗に振り分けて、表示用の文字列にする。
        // ログの中身を見る部分だけ呼び出し側から渡してもらうことで、ここはファイルに触らない
        // 純粋な判定処理になり、テストできる形になっている。
        // isDetectContent(ログのパス, 探す語) が「そのログにその語が含まれるか」を返す
        public static String ClassifyBuildResult(String[] logPathList, String buildErrorWord,
                                                 Boolean isExclude, String ignoreExecuteFileWord,
                                                 Func<String, String, Boolean> isDetectContent)
        {
            var successList = new StringBuilder("[ビルド成功]" + Environment.NewLine);
            var errorList = new StringBuilder("[ビルド失敗]" + Environment.NewLine);
            var excludeList = new StringBuilder("[実行ファイルの上書きに失敗]" + Environment.NewLine);

            foreach (String logPath in logPathList)
            {
                Boolean isSuccess = true;
                if (isDetectContent(logPath, buildErrorWord))
                {
                    // ビルド失敗
                    errorList.Append(logPath).Append(Environment.NewLine);
                    isSuccess = false;
                }

                if (isExclude && isDetectContent(logPath, ignoreExecuteFileWord))
                {
                    // 上書き不可
                    excludeList.Append(logPath).Append(Environment.NewLine);
                    isSuccess = false;
                }

                if (isSuccess)
                {
                    // ビルド成功
                    successList.Append(logPath).Append(Environment.NewLine);
                }
            }

            var result = new StringBuilder();
            result.Append(successList).Append(Environment.NewLine);
            if (isExclude)
            {
                result.Append(excludeList).Append(Environment.NewLine);
            }
            result.Append(errorList).Append(Environment.NewLine);
            return result.ToString();
        }

        // rootPath以下(サブフォルダ含む)から、名前がdirectoryNameと一致するフォルダを
        // 探して削除する。ビルド後にobj等の中間フォルダを掃除するための処理。
        // 深い方から削除しないと、親フォルダを先に消した後に子フォルダへ辿り着けず
        // 「既に無い」例外になる場合があるため、パスの長さの降順(深い=長い)で処理する。
        // 1つのフォルダが他プロセスにロックされていても、他のフォルダの削除や
        // ビルド全体が止まらないよう、フォルダ単位でtry/catchする
        public static void DeleteDirectoriesByName(String rootPath, String directoryName)
        {
            if (String.IsNullOrEmpty(directoryName) || !Directory.Exists(rootPath))
            {
                return;
            }

            String[] targetDirs = Directory.GetDirectories(rootPath, directoryName, SearchOption.AllDirectories);
            foreach (String targetDir in targetDirs.OrderByDescending(dir => dir.Length))
            {
                try
                {
                    if (Directory.Exists(targetDir))
                    {
                        Directory.Delete(targetDir, true);
                    }
                }
                catch (Exception)
                {
                    // ロックされている等で削除できないフォルダが1つあっても、
                    // 他のフォルダの削除やビルド完了処理を止めない
                }
            }
        }
    }
}
