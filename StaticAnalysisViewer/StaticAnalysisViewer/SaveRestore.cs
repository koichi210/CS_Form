﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Security.Cryptography;
using StandardTemplate;

namespace StaticAnalysisViewer
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 Parent)
        {
            SetElement("Setting");

            // コントロールを列挙
            RegistCtrl("Environment", "TextBox_LoadDataList", Parent.TextBox_LoadDataList);
            RegistCtrl("Environment", "TextBox_TopRankingNum", Parent.TextBox_TopRankingNum, "10");
        }

        public bool LoadProc(String LoadFileName, Form1 Parent)
        {
            if (LoadFileName == String.Empty)
            {
                return false;
            }

            // Default値
            Parent.TextBox_LoadDataList.Text = "";
            Parent.TextBox_TopRankingNum.Text = "10";
            Parent.HelpLink = "";

            Boolean IsSuccess = LoadXmlFile(LoadFileName);
            if (IsSuccess)
            {
                Parent.HelpLink = LoadXmlFile(LoadFileName, "Environment", "HelpLink");
            }
            return IsSuccess;
        }

        public bool SaveSetting(String SaveFileName, Form1 Parent)
        {
            XmlDocument document = OpenSaveXmlFile();
            SaveXmlFile(document);
            SaveXmlString("Environment", "HelpLink", Parent.HelpLink);
            return CloseSaveXmlFile(SaveFileName);
        }
    }
}
