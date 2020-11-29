﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StandardTemplate;

namespace TrimFileData
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 Parent)
        {
            // コントロールを列挙
            RegistCtrl("Common", "textBox_ReferencePath", Parent.textBox_ReferencePath);
            RegistCtrl("Common", "textBox_SerchCommonWord", Parent.textBox_SerchCommonWord);
            RegistCtrl("Common", "checkBox_FirstWordOnly", Parent.checkBox_FirstWordOnly);
            RegistCtrl("Common", "checkBox_OrdinalCase", Parent.checkBox_OrdinalCase);
            RegistCtrl("Common", "textBox_SerchWordList", Parent.textBox_SerchWordList);
            RegistCtrl("Common", "textBox_SerchResultList", Parent.textBox_SerchResultList);
        }

        public bool LoadProc(String LoadFileName)
        {
            if (LoadFileName == String.Empty)
            {
                return false;
            }

            return LoadXmlFile(LoadFileName);
        }

        public bool SaveSetting(String SaveFileName)
        {
            if (SaveFileName == String.Empty)
            {
                return false;
            }

            return SaveXmlFile(SaveFileName);
        }
    }
}
