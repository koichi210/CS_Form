using System;
using System.IO;
using System.Linq;

namespace VisualStudioBuilder
{
    /// <summary>
    /// もともと Form1.cs に実装されていた、ビルドスクリプト生成やパス組み立てに関する
    /// ロジックをテストできる形に切り出したもの。コードはそのまま移しただけで
    /// 書き換えていない。DataGridView/TextBox など Form のコントロール参照は、
    /// 呼び出し元(Form1)で読み取った値を引数として渡す形に変えた。
    ///
    /// StrDataGridBuildListEnable / ExtSln / ExtLog は Form1.cs 側の readonly フィールドと
    /// 同じ値を持つ定数として、ここに複製している(値の二重管理だが、UI初期化に必要な
    /// Form1側のフィールドは残したまま、Logic側だけを切り出すための妥協)。
    /// </summary>
    internal static class Logic
    {
        private const String StrDataGridBuildListEnable = "○";
        private const String ExtSln = ".sln";
        private const String ExtLog = ".log";

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
            return GetFilePathName(pathName, fileName.Replace(ExtSln, ExtLog));
        }

        public static String CreateScriptHeader(String visualStudioExePath, String buildOption)
        {
            String script = "";

            //script += @"set DEV_ENV=\""C:\Program Files (x86)\""Microsoft Visual Studio 10.0\""Common7\""IDE\""devenv.exe";
            script += @"set DEV_ENV=""" + visualStudioExePath + @"""" + Environment.NewLine;

            //script += @"set BUILD_OPT=/rebuild release";
            script += "set BUILD_OPT=" + buildOption + Environment.NewLine;
            script += Environment.NewLine;

            return script;
        }

        public static String CreateBuildScript(String buildEnable, String solutionName, String projectPath, String logDirectory, Boolean isExportLog)
        {
            // パラメータチェック
            if (buildEnable != StrDataGridBuildListEnable ||
                solutionName == String.Empty ||
                projectPath == String.Empty)
            {
                return "";
            }

            String solutionPath = GetSolutionPathName(projectPath, solutionName);
            String logName = GetLogPathName(logDirectory, solutionName);

            String script = "";
            if (isExportLog)
            {
                // ファイルが存在したら削除
                if (File.Exists(logName))
                {
                    script += "del " + logName + Environment.NewLine;
                }
                script += @"%DEV_ENV% %BUILD_OPT% /out " + logName + " " + solutionPath + Environment.NewLine;
            }
            else
            {
                script += @"%DEV_ENV% %BUILD_OPT% " + solutionPath + Environment.NewLine;
            }
            script += Environment.NewLine;

            return script;
        }

        // ビルド結果(ログの一覧)を成功/失敗/上書き失敗に振り分けて、表示用の文字列にする。
        // ログの中身を見る部分だけ呼び出し側から渡してもらうことで、ここはファイルに触らない
        // 純粋な判定処理になり、テストできる形になっている。
        // isDetectContent(ログのパス, 探す語) が「そのログにその語が含まれるか」を返す
        public static String ClassifyBuildResult(String[] logPathList, String buildErrorWord,
                                                 Boolean isExclude, String ignoreExecuteFileWord,
                                                 Func<String, String, Boolean> isDetectContent)
        {
            String successList = "[ビルド成功]" + Environment.NewLine;
            String errorList = "[ビルド失敗]" + Environment.NewLine;
            String excludeList = "[実行ファイルの上書きに失敗]" + Environment.NewLine;

            foreach (String logPath in logPathList)
            {
                Boolean isSuccess = true;
                if (isDetectContent(logPath, buildErrorWord))
                {
                    // ビルド失敗
                    errorList += logPath + Environment.NewLine;
                    isSuccess = false;
                }

                if (isExclude && isDetectContent(logPath, ignoreExecuteFileWord))
                {
                    // 上書き不可
                    excludeList += logPath + Environment.NewLine;
                    isSuccess = false;
                }

                if (isSuccess)
                {
                    // ビルド成功
                    successList += logPath + Environment.NewLine;
                }
            }

            String result = successList + Environment.NewLine;
            if (isExclude)
            {
                result += excludeList + Environment.NewLine;
            }
            result += errorList + Environment.NewLine;
            return result;
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
