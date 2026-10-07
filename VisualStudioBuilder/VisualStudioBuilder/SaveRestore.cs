using System;
using StandardTemplate;

namespace VisualStudioBuilder
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(Form1 parent)
        {
            SetElement("Setting");

            // コントロールを列挙。
            // textBox_VisualStudioExePathはdevenv.exeでビルドしていた頃の名前のまま
            // (今はMSBuild.exeのパスを入れる)。設定ファイルのキー名になっているので、
            // 変えると既存の設定ファイルから読めなくなる
            RegisterCtrl("Common", "textBox_VisualStudioExePath", parent.textBox_VisualStudioExePath, Logic.DefaultMsBuildPath);
            RegisterCtrl("Common", "textBox_BuildOption", parent.textBox_BuildOption, Logic.DefaultBuildOption);
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
            parent.textBox_VisualStudioExePath.Text = Logic.DefaultMsBuildPath;
            parent.textBox_BuildOption.Text = Logic.DefaultBuildOption;
            parent.textBox_LogDirectory.Text = System.Environment.CurrentDirectory;
            parent.dataGridView.RowCount = 1;

            return LoadXmlFile(loadFileName);
        }
    }
}
