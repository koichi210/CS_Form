﻿﻿using System;
using StandardTemplate;

namespace Cheetos
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(Cheetos parent)
        {
            SetElement("Setting");
            // 第2引数(設定ファイルのキー名)にtypoが残っているものがあるが、ここを直すと
            // 既存の設定ファイルの値が読めなくなるため、コントロール名だけを修正してある。
            // キー名はXML->JSON移行時に旧キーの読み替えと一緒に直す([[_TechnicalNote/typo修正リスト.md]])

            RegisterCtrl("CaptureWindow", "cw_TextBox_SavePath", parent.cw_TextBox_SavePath);
            RegisterCtrl("CaptureWindow", "cw_TextBox_SaveFilePrefix", parent.cw_TextBox_SaveFilePrefix, legacyAttrValue: "cw_TextBox_SaveFilePrifix");
            RegisterCtrl("CaptureWindow", "cw_checkBox_AddTimeStamp", parent.cw_checkBox_AddTimeStamp, legacyAttrValue: "cw_checkBox_AddTimeStump");
            RegisterCtrl("CaptureWindow", "cw_Radio_FullScreen", parent.cw_Radio_FullScreen,"True");
            RegisterCtrl("CaptureWindow", "cw_Radio_CurrentScreen", parent.cw_Radio_CurrentScreen);
            RegisterCtrl("CaptureWindow", "cw_Radio_CurrentWindow", parent.cw_Radio_CurrentWindow);
            RegisterCtrl("CaptureWindow", "cw_TextBox_Sleep", parent.cw_TextBox_Sleep,"2000");
            RegisterCtrl("CaptureWindow", "cw_TextBox_Loop", parent.cw_TextBox_Loop,"2");
            RegisterCtrl("DataGrid", "Cell", "RowCount", parent.cw_dataGridView);

            RegisterCtrl("PictTrim", "pt_SourceFolderPath", parent.pt_SourceFolderPath);
            RegisterCtrl("PictTrim", "pt_BaseX", parent.pt_BaseX);
            RegisterCtrl("PictTrim", "pt_BaseY", parent.pt_BaseY);
            RegisterCtrl("PictTrim", "pt_Radio_SelectPointOfEnd", parent.pt_Radio_SelectPointOfEnd);
            RegisterCtrl("PictTrim", "pt_Radio_SelectSizeOfEnd", parent.pt_Radio_SelectSizeOfEnd);
            RegisterCtrl("PictTrim", "pt_TargetX", parent.pt_TargetX);
            RegisterCtrl("PictTrim", "pt_TargetY", parent.pt_TargetY);

            RegisterCtrl("Rotation", "pr_SourceFolderPath", parent.pr_SourceFolderPath);
            RegisterCtrl("Rotation", "pr_BaseX", parent.pr_BaseX);
            RegisterCtrl("Rotation", "pr_BaseY", parent.pr_BaseY);
            RegisterCtrl("Rotation", "pr_Angle", parent.pr_Angle);

            RegisterCtrl("DistOrient", "do_SourceFolderPath", parent.do_SourceFolderPath);
            RegisterCtrl("DistOrient", "do_DestPortFolderPath", parent.do_DestPortFolderPath);
            RegisterCtrl("DistOrient", "do_DestLandFolderPath", parent.do_DestLandFolderPath);
            RegisterCtrl("DistOrient", "do_TargetFileName", parent.do_TargetFileName);
            RegisterCtrl("DistOrient", "do_WhiteLength", parent.do_WhiteLength);
            RegisterCtrl("DistOrient", "do_WhiteCoef", parent.do_WhiteCoef, "30");
            RegisterCtrl("DistOrient", "do_SampleFilePath", parent.do_SampleFilePath);

            RegisterCtrl("PictMerge", "pm_SourceFolderPath", parent.pm_SourceFolderPath);
            RegisterCtrl("PictMerge", "pm_SourceFile1Prefix", parent.pm_SourceFile1Prefix);
            RegisterCtrl("PictMerge", "pm_SourceFile2Prefix", parent.pm_SourceFile2Prefix);
            RegisterCtrl("PictMerge", "pm_TrimingHeight", parent.pm_TrimingHeight);

            RegisterCtrl("FileCollect", "fc_SourceFolderPath", parent.fc_SourceFolderPath);
            RegisterCtrl("FileCollect", "fc_DestFolderPath", parent.fc_DestFolderPath);
            RegisterCtrl("FileCollect", "fc_TargetFileName", parent.fc_TargetFileName);
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
