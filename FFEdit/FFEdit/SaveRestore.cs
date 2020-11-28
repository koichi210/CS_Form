﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using StandardTemplate;

namespace FFEdit
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 Parent)
        {
            SetElement("Setting");

            // コントロールを列挙
            RegistCtrlList("comboBox_TargetDir", "Value_", Parent.comboBox_TargetDir);
            RegistCtrlList("comboBox_String1", "Value_", Parent.comboBox_String1);
            RegistCtrlList("comboBox_String2", "Value_", Parent.comboBox_String2);
            RegistCtrl("textBox_Target_Extension", "Value", Parent.textBox_Target_Extension, "*");
        }

        public bool LoadProc(String LoadFileName, Form1 Parent)
        {
            if (LoadFileName == String.Empty)
            {
                return false;
            }

            // Default設定
            Parent.textBox_Target_Extension.Text = "*";

            return LoadXmlFile(LoadFileName);
        }

        public bool SaveSetting(String SaveFileName, Form1 Parent)
        {
            if (SaveFileName == String.Empty)
            {
                return false;
            }

            // コンボボックスの更新
            StcUtils util = new StcUtils();
            util.ModifyCombBoxList(Parent.comboBox_TargetDir);
            util.ModifyCombBoxList(Parent.comboBox_String1);
            util.ModifyCombBoxList(Parent.comboBox_String2);

            return SaveXmlFile(SaveFileName);
        }
    }
}
