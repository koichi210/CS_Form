﻿using System;
using System.Xml;
using StandardTemplate;

namespace StaticAnalysisViewer
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 parent)
        {
            SetElement("Setting");

            // コントロールを列挙
            RegistCtrl("Environment", "TextBox_LoadDataList", parent.TextBox_LoadDataList);
            RegistCtrl("Environment", "TextBox_TopRankingNum", parent.TextBox_TopRankingNum, "10");
        }

        public bool LoadProc(String loadFileName, Form1 parent)
        {
            if (loadFileName == String.Empty)
            {
                return false;
            }

            // Default値
            parent.TextBox_LoadDataList.Text = "";
            parent.TextBox_TopRankingNum.Text = "10";
            parent.HelpLink = "";

            Boolean isSuccess = LoadXmlFile(loadFileName);
            if (isSuccess)
            {
                parent.HelpLink = LoadXmlFile(loadFileName, "Environment", "HelpLink");
            }
            return isSuccess;
        }

        public bool SaveSetting(String saveFileName, Form1 parent)
        {
            XmlDocument document = OpenSaveXmlFile();
            SaveXmlFile(document);
            SaveXmlString("Environment", "HelpLink", parent.HelpLink);
            return CloseSaveXmlFile(saveFileName);
        }
    }
}
