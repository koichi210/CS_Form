﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Security.Cryptography;
using StandardTemplate;

namespace VisualStudioBuilder
{
    class SaveRestore : StcSaveRestore
    {
        private StcUtils util = new StcUtils();

        public void RegistItem(Form1 Parent)
        {
            SetElement("Setting");

            // コントロールを列挙
            RegistCtrl("Common", "textBox_VisualStudioExePath", Parent.textBox_VisualStudioExePath, @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe");
            RegistCtrl("Common", "textBox_BuildOption", Parent.textBox_BuildOption, "/rebuild release");
            RegistCtrl("Common", "checkBox_DeleteDirectory", Parent.checkBox_DeleteDirectory, "False");
            RegistCtrl("Common", "textBox_DeleteDirectoryName", Parent.textBox_DeleteDirectoryName, "obj");
            RegistCtrl("Common", "textBox_LogDirectory", Parent.textBox_LogDirectory, System.Environment.CurrentDirectory);
            RegistCtrl("Common", "checkBox_DetectBuildError", Parent.checkBox_DetectBuildError);
            RegistCtrl("Common", "textBox_DetectBuildErrorWord", Parent.textBox_DetectBuildErrorWord);
            RegistCtrl("Common", "checkBox_IsExclude", Parent.checkBox_IsExclude);
            RegistCtrl("Common", "textBox_ExcludeWord", Parent.textBox_ExcludeWord);
            RegistCtrl("DataGrid", "Cell", "RowCount", Parent.dataGridView);
        }

        public bool LoadProc(String LoadFileName, Form1 Parent)
        {
            if (LoadFileName == String.Empty)
            {
                return false;
            }

            // Default値
            Parent.textBox_VisualStudioExePath.Text = @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe";
            Parent.textBox_BuildOption.Text = "/rebuild release";
            Parent.textBox_LogDirectory.Text = System.Environment.CurrentDirectory;
            Parent.dataGridView.RowCount = 1;

            return LoadXmlFile(LoadFileName);
        }
    }
}
