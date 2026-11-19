using System;
using System.Collections.Generic;
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

        // ログのファイル名を「プロジェクトのパス＋ソリューション名」から作る(\と:は_に置き換える)。
        // 例: D:\Work\Foo と Foo.sln → D__Work_Foo_Foo.log
        // 以前はソリューション名だけ(Foo.log)だったため、別フォルダに同じ名前のソリューションがあると
        // ログが上書きされて、どちらの結果か分からなくなっていた
        public static String GetLogFileName(String projectPath, String solutionName)
        {
            String solutionPath = GetSolutionPathName(projectPath ?? "", solutionName ?? "");
            String logName = solutionPath.EndsWith(SlnExtension, StringComparison.OrdinalIgnoreCase)
                ? solutionPath.Substring(0, solutionPath.Length - SlnExtension.Length) + _logExtension
                : solutionPath + _logExtension;
            return logName.Replace('\\', '_').Replace(':', '_');
        }

        public static String GetLogPathName(String logDirectory, String projectPath, String solutionName)
        {
            return GetFilePathName(logDirectory, GetLogFileName(projectPath, solutionName));
        }

        // devenv.exeの欄に書かれたパスが使えるかを調べる。使えればnull、使えなければ画面に出すメッセージを返す
        // (fileExistsはテストで差し替えられるようにしてある)
        public static String ValidateVisualStudioExePath(String exePath, Func<String, Boolean> fileExists)
        {
            if (String.IsNullOrWhiteSpace(exePath))
            {
                return "VisualStudioパス(devenv.exe)が空欄です。「ビルド設定」タブで指定してください";
            }
            if (!fileExists(exePath.Trim().Trim('"')))
            {
                return "VisualStudioパス(devenv.exe)が見つかりません。「ビルド設定」タブで確認してください" + Environment.NewLine + exePath;
            }
            return null;
        }

        // ビルド対象(○)の行のうち、ソリューションファイルが見つからないものを「No.行番号 パス」の形で返す
        // (行番号は表の左端に出している1始まりの番号)。空欄の行(ソリューション名もパスも無い)は数えない
        public static List<String> FindMissingSolutions(IList<BuildTarget> targets, Func<String, Boolean> fileExists)
        {
            List<String> missing = new List<String>();
            for (int i = 0; i < targets.Count; i++)
            {
                BuildTarget target = targets[i];
                if (!target.IsEnabled || target.IsBlank)
                {
                    continue;
                }

                String solutionPath = GetSolutionPathName(target.ProjectPath, target.SolutionName);
                if (target.SolutionName == String.Empty || target.ProjectPath == String.Empty || !fileExists(solutionPath))
                {
                    missing.Add("No." + (i + 1) + " " + solutionPath);
                }
            }
            return missing;
        }

        // 表の上に出す件数の文字列
        public static String FormatRowCount(int totalCount, int enabledCount)
        {
            return "登録数: " + totalCount + " 件 (ビルド対象: " + enabledCount + " 件)";
        }

        public static String CreateScriptHeader(String visualStudioExePath, String buildOption)
        {
            // 例: set DEV_ENV="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe"
            //     set BUILD_OPT=/rebuild release
            return @"set DEV_ENV=""" + visualStudioExePath + @"""" + Environment.NewLine
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

            // パスに空白があっても1つの引数として渡るよう、ログとソリューションのパスは""で囲む
            // (ログ名にプロジェクトのパスが入るようになり、Program Files等の空白を含みやすくなったため)
            String solutionPath = Quote(GetSolutionPathName(projectPath, solutionName));
            String logName = GetLogPathName(logDirectory, projectPath, solutionName);

            var script = new StringBuilder();
            if (isExportLog)
            {
                // ファイルが存在したら削除
                if (File.Exists(logName))
                {
                    script.Append("del ").Append(Quote(logName)).Append(Environment.NewLine);
                }
                script.Append("%DEV_ENV% %BUILD_OPT% /out ").Append(Quote(logName)).Append(' ').Append(solutionPath).Append(Environment.NewLine);
            }
            else
            {
                script.Append("%DEV_ENV% %BUILD_OPT% ").Append(solutionPath).Append(Environment.NewLine);
            }
            script.Append(Environment.NewLine);

            return script.ToString();
        }

        private static String Quote(String path)
        {
            return "\"" + path + "\"";
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

        // 「ビルド後に削除」欄の文字列を、1行に1つの削除対象(フォルダ名やファイルのパターン)に分ける。
        // 例: 「obj」「*.ncb」の2行 → { "obj", "*.ncb" }。前後の空白は落とし、空行・重複は捨てる。
        // 消しすぎを防ぐため、次のものは使わずにinvalidへ入れて返す(呼び出し元が結果欄に出す)
        //  ・\ / : を含むもの(名前やパターンではなくパスになっている。..で上の階層へ出るのも防ぐ)
        //  ・*と?と.だけでできているもの(*、*.*など、全部に当たってしまう)
        //  ・ファイル名に使えない文字(< > | " など。*と?はワイルドカードとして使える)を含むもの
        public static String[] ParseDeleteTargets(String text, out String[] invalid)
        {
            List<String> targets = new List<String>();
            List<String> invalidList = new List<String>();
            char[] invalidChars = Path.GetInvalidFileNameChars().Where(c => c != '*' && c != '?').ToArray();
            String[] lines = (text ?? "").Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            foreach (String rawLine in lines)
            {
                String line = rawLine.Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                Boolean hasPathChar = line.IndexOfAny(new[] { '\\', '/', ':' }) >= 0;
                Boolean isTooBroad = line.Trim('*', '?', '.').Length == 0;
                if (hasPathChar || isTooBroad || line.IndexOfAny(invalidChars) >= 0)
                {
                    invalidList.Add(line);
                    continue;
                }

                if (!targets.Contains(line, StringComparer.OrdinalIgnoreCase))
                {
                    targets.Add(line);
                }
            }

            invalid = invalidList.ToArray();
            return targets.ToArray();
        }

        // rootPath以下(サブフォルダ含む)から、patternsのどれかに名前が一致するフォルダとファイルを削除する。
        // パターンはフォルダにもファイルにも当てる(objならobjフォルダ、*.ncbなら.ncbファイル。ワイルドカード可)。
        // 先にフォルダを消し(中のファイルもまとめて消える)、残ったファイルを消す。
        // 消せなかったもの(ロック中など)は飛ばして続ける。消したフォルダとファイルの数を返す
        public static int DeleteTargetsByPatterns(String rootPath, IEnumerable<String> patterns)
        {
            if (patterns == null || !Directory.Exists(rootPath))
            {
                return 0;
            }

            int deletedCount = 0;
            foreach (String pattern in patterns)
            {
                if (String.IsNullOrEmpty(pattern))
                {
                    continue;
                }

                // 深い方から消す(DeleteDirectoriesByNameと同じ理由)
                foreach (String targetDir in SafeGetEntries(() => Directory.GetDirectories(rootPath, pattern, SearchOption.AllDirectories))
                    .OrderByDescending(dir => dir.Length))
                {
                    try
                    {
                        if (Directory.Exists(targetDir))
                        {
                            Directory.Delete(targetDir, true);
                            deletedCount++;
                        }
                    }
                    catch (Exception)
                    {
                        // 削除できないフォルダが1つあっても、他の削除やビルド完了処理を止めない
                    }
                }

                foreach (String targetFile in SafeGetEntries(() => Directory.GetFiles(rootPath, pattern, SearchOption.AllDirectories)))
                {
                    try
                    {
                        if (File.Exists(targetFile))
                        {
                            File.Delete(targetFile);
                            deletedCount++;
                        }
                    }
                    catch (Exception)
                    {
                        // 読み取り専用・ロック中などで消せないファイルは飛ばす
                    }
                }
            }
            return deletedCount;
        }

        // 一覧の取得に失敗したとき(アクセス権の無いフォルダがある等)は、そのパターンは何も消さない
        private static String[] SafeGetEntries(Func<String[]> getEntries)
        {
            try
            {
                return getEntries();
            }
            catch (Exception)
            {
                return new String[0];
            }
        }
    }

    // 表の1行分(ビルド欄・ソリューション名・プロジェクトのパス)。
    // バックグラウンドの処理へは画面のセルではなくこの写しを渡す(別スレッドから表に触らないため)
    internal sealed class BuildTarget
    {
        public String BuildEnable { get; set; } = "";
        public String SolutionName { get; set; } = "";
        public String ProjectPath { get; set; } = "";

        public Boolean IsEnabled
        {
            get { return BuildEnable == Logic.BuildListEnable; }
        }

        public Boolean IsBlank
        {
            get { return SolutionName == String.Empty && ProjectPath == String.Empty; }
        }
    }
}
