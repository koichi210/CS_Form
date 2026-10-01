﻿﻿using System;
using StandardTemplate;

namespace Cheetos
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Cheetos parent)
        {
            SetElement("Setting");
            // 第2引数(設定ファイルのキー名)にtypoが残っているものがあるが、ここを直すと
            // 既存の設定ファイルの値が読めなくなるため、コントロール名だけを修正してある。
            // キー名はXML->JSON移行時に旧キーの読み替えと一緒に直す([[_TechnicalNote/typo修正リスト.md]])

            RegistCtrl("CaptureWindow", "cw_TextBox_SavePath", parent.cw_TextBox_SavePath);
            RegistCtrl("CaptureWindow", "cw_TextBox_SaveFilePrefix", parent.cw_TextBox_SaveFilePrefix, LegacyAttrValue: "cw_TextBox_SaveFilePrifix");
            RegistCtrl("CaptureWindow", "cw_checkBox_AddTimeStamp", parent.cw_checkBox_AddTimeStamp, LegacyAttrValue: "cw_checkBox_AddTimeStump");
            RegistCtrl("CaptureWindow", "cw_Radio_FullScreen", parent.cw_Radio_FullScreen,"True");
            RegistCtrl("CaptureWindow", "cw_Radio_CurrentScreen", parent.cw_Radio_CurrentScreen);
            RegistCtrl("CaptureWindow", "cw_Radio_CurrentWindow", parent.cw_Radio_CurrentWindow);
            RegistCtrl("CaptureWindow", "cw_TextBox_Sleep", parent.cw_TextBox_Sleep,"2000");
            RegistCtrl("CaptureWindow", "cw_TextBox_Loop", parent.cw_TextBox_Loop,"2");
            RegistCtrl("DataGrid", "Cell", "RowCount", parent.cw_dataGridView);

            RegistCtrl("PictTrim", "pt_SourceFolderPath", parent.pt_SourceFolderPath);
            RegistCtrl("PictTrim", "pt_BaseX", parent.pt_BaseX);
            RegistCtrl("PictTrim", "pt_BaseY", parent.pt_BaseY);
            RegistCtrl("PictTrim", "pt_Radio_SelectPointOfEnd", parent.pt_Radio_SelectPointOfEnd);
            RegistCtrl("PictTrim", "pt_Radio_SelectSizeOfEnd", parent.pt_Radio_SelectSizeOfEnd);
            RegistCtrl("PictTrim", "pt_TargetX", parent.pt_TargetX);
            RegistCtrl("PictTrim", "pt_TargetY", parent.pt_TargetY);

            RegistCtrl("Rotation", "pr_SourceFolderPath", parent.pr_SourceFolderPath);
            RegistCtrl("Rotation", "pr_BaseX", parent.pr_BaseX);
            RegistCtrl("Rotation", "pr_BaseY", parent.pr_BaseY);
            RegistCtrl("Rotation", "pr_Angle", parent.pr_Angle);

            RegistCtrl("DistOrient", "do_SourceFolderPath", parent.do_SourceFolderPath);
            RegistCtrl("DistOrient", "do_DestPortFolderPath", parent.do_DestPortFolderPath);
            RegistCtrl("DistOrient", "do_DestLandFolderPath", parent.do_DestLandFolderPath);
            RegistCtrl("DistOrient", "do_TargetFileName", parent.do_TargetFileName);
            RegistCtrl("DistOrient", "do_WhiteLength", parent.do_WhiteLength);
            RegistCtrl("DistOrient", "do_WhiteCoef", parent.do_WhiteCoef, "30");
            RegistCtrl("DistOrient", "do_SampleFilePath", parent.do_SampleFilePath);

            RegistCtrl("PictMerge", "pm_SourceFolderPath", parent.pm_SourceFolderPath);
            RegistCtrl("PictMerge", "pm_SourceFile1Prefix", parent.pm_SourceFile1Prefix);
            RegistCtrl("PictMerge", "pm_SourceFile2Prefix", parent.pm_SourceFile2Prefix);
            RegistCtrl("PictMerge", "pm_TrimingHeight", parent.pm_TrimingHeight);

            RegistCtrl("FileCollect", "fc_SourceFolderPath", parent.fc_SourceFolderPath);
            RegistCtrl("FileCollect", "fc_DestFolderPath", parent.fc_DestFolderPath);
            RegistCtrl("FileCollect", "fc_TargetFileName", parent.fc_TargetFileName);
        }

        public bool LoadProc(String loadFileName, Cheetos parent)
        {
            if (loadFileName == String.Empty)
            {
                return false;
            }

            // Default値
            parent.do_WhiteCoef.Text = @"30";

            return LoadXmlFile(loadFileName);
        }

        // JSON保存/読込([[_Common/JsonFileStorage.cs]])。RegistItemで登録済みのコントロールを
        // そのまま汎用プロファイル(StcSaveRestore.BuildGenericProfile/ApplyGenericProfile)に
        // 詰め替えるだけで、Cheetos専用のPOCOは作らない(コントロール数が多く、フィールドごとに
        // 手書きするとズレの元になるため)
        public Boolean SaveJsonFile(String filePath)
        {
            try
            {
                JsonFileStorage.Save(filePath, BuildGenericProfile());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean LoadJsonFile(String filePath)
        {
            GenericProfile profile = JsonFileStorage.Load<GenericProfile>(filePath);
            if (profile == null)
            {
                return false;
            }

            ApplyGenericProfile(profile);
            return true;
        }
    }
}
