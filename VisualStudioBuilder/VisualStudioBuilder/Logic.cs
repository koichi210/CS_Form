using System;
using System.IO;

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

        public static String GetFilePathName(String PathName, String FileName, String Ext = "")
        {
            return PathName.TrimEnd('\\') + '\\' + FileName + Ext;
        }

        public static String GetSolutionPathName(String PathName, String FileName)
        {
            return GetFilePathName(PathName, FileName);
        }

        public static String GetLogPathName(String PathName, String FileName)
        {
            return GetFilePathName(PathName, FileName.Replace(ExtSln, ExtLog));
        }

        public static String CreateScriptHeader(String VisualStudioExePath, String BuildOption)
        {
            String Script = "";

            //Script += @"set DEV_ENV=\""C:\Program Files (x86)\""Microsoft Visual Studio 10.0\""Common7\""IDE\""devenv.exe";
            Script += @"set DEV_ENV=""" + VisualStudioExePath + @"""" + Environment.NewLine;

            //Script += @"set BUILD_OPT=/rebuild release";
            Script += "set BUILD_OPT=" + BuildOption + Environment.NewLine;
            Script += Environment.NewLine;

            return Script;
        }

        public static String CreateBuildScript(String BuildEnable, String SolutionName, String ProjectPath, String LogDirectory, Boolean IsExportLog)
        {
            // パラメータチェック
            if (BuildEnable != StrDataGridBuildListEnable ||
                SolutionName == String.Empty ||
                ProjectPath == String.Empty)
            {
                return "";
            }

            String SolutionPath = GetSolutionPathName(ProjectPath, SolutionName);
            String LogName = GetLogPathName(LogDirectory, SolutionName);

            String Script = "";
            if (IsExportLog)
            {
                // ファイルが存在したら削除
                if (File.Exists(LogName))
                {
                    Script += "del " + LogName + Environment.NewLine;
                }
                Script += @"%DEV_ENV% %BUILD_OPT% /out " + LogName + " " + SolutionPath + Environment.NewLine;
            }
            else
            {
                Script += @"%DEV_ENV% %BUILD_OPT% " + SolutionPath + Environment.NewLine;
            }
            Script += Environment.NewLine;

            return Script;
        }
    }
}
