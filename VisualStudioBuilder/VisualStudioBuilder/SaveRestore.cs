using System;
using StandardTemplate;

namespace VisualStudioBuilder
{
    class SaveRestore : StcSaveRestore
    {
        // 設定ファイルに値が無いときの既定値(RegisterItemとLoadProcで共通)
        private const String _defaultVisualStudioExePath = @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe";
        private const String _defaultBuildOption = "/rebuild release";

        public void RegisterItem(Form1 parent)
        {
            SetElement("Setting");

            // コントロールを列挙
            RegisterCtrl("Common", "textBox_VisualStudioExePath", parent.textBox_VisualStudioExePath, _defaultVisualStudioExePath);
            RegisterCtrl("Common", "textBox_BuildOption", parent.textBox_BuildOption, _defaultBuildOption);
            RegisterCtrl("Common", "checkBox_DeleteDirectory", parent.checkBox_DeleteDirectory, "False");
            RegisterCtrl("Common", "textBox_DeleteDirectoryName", parent.textBox_DeleteDirectoryName, "obj");
            RegisterCtrl("Common", "textBox_LogDirectory", parent.textBox_LogDirectory, System.Environment.CurrentDirectory);
            RegisterCtrl("Common", "checkBox_DetectBuildError", parent.checkBox_DetectBuildError);
            RegisterCtrl("Common", "textBox_DetectBuildErrorWord", parent.textBox_DetectBuildErrorWord);
            RegisterCtrl("Common", "checkBox_IsExclude", parent.checkBox_IsExclude);
            RegisterCtrl("Common", "textBox_ExcludeWord", parent.textBox_ExcludeWord);
            RegisterCtrl("DataGrid", "Cell", "RowCount", parent.dataGridView);
        }

        public bool LoadProc(String loadFileName, Form1 parent)
        {
            if (loadFileName == String.Empty)
            {
                return false;
            }

            // Default値
            parent.textBox_VisualStudioExePath.Text = _defaultVisualStudioExePath;
            parent.textBox_BuildOption.Text = _defaultBuildOption;
            parent.textBox_LogDirectory.Text = System.Environment.CurrentDirectory;
            parent.dataGridView.RowCount = 1;

            return LoadXmlFile(loadFileName);
        }
    }
}
