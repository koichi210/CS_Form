﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Security.Cryptography;
using StandardTemplate;

namespace Cheetos
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Cheetos Parent)
        {
            SetElement("Setting");

            RegistCtrl("CaptureWindow", "cw_TextBox_SavePath", Parent.cw_TextBox_SavePath);
            RegistCtrl("CaptureWindow", "cw_TextBox_SaveFilePrifix", Parent.cw_TextBox_SaveFilePrifix);
            RegistCtrl("CaptureWindow", "cw_checkBox_AddTimeStump", Parent.cw_checkBox_AddTimeStump);
            RegistCtrl("CaptureWindow", "cw_Radio_FullScreen", Parent.cw_Radio_FullScreen,"True");
            RegistCtrl("CaptureWindow", "cw_Radio_CurrentScreen", Parent.cw_Radio_CurrentScreen);
            RegistCtrl("CaptureWindow", "cw_Radio_CurrentWindow", Parent.cw_Radio_CurrentWindow);
            RegistCtrl("CaptureWindow", "cw_TextBox_Sleep", Parent.cw_TextBox_Sleep,"2000");
            RegistCtrl("CaptureWindow", "cw_TextBox_Loop", Parent.cw_TextBox_Loop,"2");
            RegistCtrl("DataGrid", "Cell", "RowCount", Parent.cw_dataGridView);

            RegistCtrl("PictTrim", "pt_SourceFolderPath", Parent.pt_SourceFolderPath);
            RegistCtrl("PictTrim", "pt_BaseX", Parent.pt_BaseX);
            RegistCtrl("PictTrim", "pt_BaseY", Parent.pt_BaseY);
            RegistCtrl("PictTrim", "pt_Radio_SelectPointOfEnd", Parent.pt_Radio_SelectPointOfEnd);
            RegistCtrl("PictTrim", "pt_Radio_SelectSizeOfEnd", Parent.pt_Radio_SelectSizeOfEnd);
            RegistCtrl("PictTrim", "pt_TargetX", Parent.pt_TargetX);
            RegistCtrl("PictTrim", "pt_TargetY", Parent.pt_TargetY);

            RegistCtrl("Rotation", "pr_SourceFolderPath", Parent.pr_SourceFolderPath);
            RegistCtrl("Rotation", "pr_BaseX", Parent.pr_BaseX);
            RegistCtrl("Rotation", "pr_BaseY", Parent.pr_BaseY);
            RegistCtrl("Rotation", "pr_Angle", Parent.pr_Angle);

            RegistCtrl("DistOrient", "do_SourceFolderPath", Parent.do_SourceFolderPath);
            RegistCtrl("DistOrient", "do_DestPortFolderPath", Parent.do_DestPortFolderPath);
            RegistCtrl("DistOrient", "do_DestLandFolderPath", Parent.do_DestLandFolderPath);
            RegistCtrl("DistOrient", "do_TargetFileName", Parent.do_TargetFileName);
            RegistCtrl("DistOrient", "do_WhiteLength", Parent.do_WhiteLength);
            RegistCtrl("DistOrient", "do_WhiteCoef", Parent.do_WhiteCoef, "30");
            RegistCtrl("DistOrient", "do_SampleFilePath", Parent.do_SampleFilePath);

            RegistCtrl("PictMerge", "pm_SourceFolderPath", Parent.pm_SourceFolderPath);
            RegistCtrl("PictMerge", "pm_SourceFile1Prefix", Parent.pm_SourceFile1Prefix);
            RegistCtrl("PictMerge", "pm_SourceFile2Prefix", Parent.pm_SourceFile2Prefix);
            RegistCtrl("PictMerge", "pm_TrimingHeight", Parent.pm_TrimingHeight);

            RegistCtrl("FileCollect", "fc_SourceFolderPath", Parent.fc_SourceFolderPath);
            RegistCtrl("FileCollect", "fc_DestFolderPath", Parent.fc_DestFolderPath);
            RegistCtrl("FileCollect", "fc_TargetFileName", Parent.fc_TargetFileName);
        }

        public bool LoadProc(String LoadFileName, Cheetos Parent)
        {
            if (LoadFileName == String.Empty)
            {
                return false;
            }

            // Default値
            Parent.do_WhiteCoef.Text = @"30";

            return LoadXmlFile(LoadFileName);
        }
    }
}
