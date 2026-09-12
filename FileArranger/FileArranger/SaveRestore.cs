using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using StandardTemplate;

namespace FileArranger
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistLoadItem(FileArranger Parent)
        {
            SetElement("Setting");

            RegistCtrl("Common", "cmn_textBox_Reference", Parent.cmn_textBox_Reference);
            RegistCtrl("Common", "cmn_textBox_AddList", Parent.cmn_textBox_AddList);
            RegistCtrl("Common", "cmn_textBox_AddListSuffix", Parent.cmn_textBox_AddListSuffix);
            
            RegistCtrl("MoveDir", "md_textBox_SourceDir", Parent.md_textBox_SourceDir);
            RegistCtrlList("MoveDir", "md_comboBox_TargetDir", Parent.md_comboBox_TargetDir);
            RegistCtrl("MoveDir", "md_comboBox_TargetDir", Parent.md_comboBox_TargetDir);

           
            RegistCtrlList("MoveDir", "rd_comboBox_RenameDir", Parent.rd_comboBox_RenameDir);
            RegistCtrl("MoveDir", "rd_comboBox_RenameDir", Parent.rd_comboBox_RenameDir);
            RegistCtrl("RenameDir", "rd_textBox_ExistItemDir", Parent.rd_textBox_ExistItemDir);
            RegistCtrl("RenameDir", "rd_comboBox_MergeWord", Parent.rd_comboBox_MergeWord);
            RegistCtrl("RenameDir", "rd_checkBox_FileOpen", Parent.rd_checkBox_FileOpen);
            RegistCtrl("RenameDir", "rd_textBox_SplitWord3", Parent.rd_textBox_SplitWord3);
            RegistCtrl("RenameDir", "rd_textBox_AddTitlePreWord", Parent.rd_textBox_AddTitlePreWord);
            RegistCtrl("RenameDir", "rd_textBox_SearchTitleLine", Parent.rd_textBox_SearchTitleLine);
            RegistCtrl("RenameDir", "rd_textBox_SearchTitleLength", Parent.rd_textBox_SearchTitleLength);
            RegistCtrlList("RenameDir", "rd_comboBox_AddTitlePostWord", Parent.rd_comboBox_AddTitlePostWord);
            RegistCtrl("RenameDir", "rd_comboBox_AddTitlePostWord", Parent.rd_comboBox_AddTitlePostWord);

            RegistCtrl("SortFileName", "sf_textBox_TargetFile", Parent.sf_textBox_TargetFile);

            RegistCtrl("MoveFile", "mf_textBox_SourceDir", Parent.mf_textBox_SourceDir);
            RegistCtrl("MoveFile", "mf_textBox_TargetDir", Parent.mf_textBox_TargetDir);

            RegistCtrl("PartitionFile", "pf_textBox_TargetFile", Parent.pf_textBox_TargetFile);
            RegistCtrl("PartitionFile", "pf_textBox_RefrenceFile", Parent.pf_textBox_RefrenceFile);
            RegistCtrl("PartitionFile", "pf_textBox_TargetSeprator", Parent.pf_textBox_TargetSeprator);
            RegistCtrl("PartitionFile", "pf_textBox_SearchTitleLine", Parent.pf_textBox_SearchTitleLine);
            RegistCtrl("PartitionFile", "pf_textBox_SearchTitleLength", Parent.pf_textBox_SearchTitleLength);
            RegistCtrl("PartitionFile", "pf_checkBox_CreateNewDir", Parent.pf_checkBox_CreateNewDir);
        }

        public Boolean LoadProc(String LoadFileName, FileArranger Parent)
        {
            Boolean IsSuccess = LoadXmlFile(LoadFileName);
            if (IsSuccess)
            {
                Parent.RefrenceCandidateFolders = LoadXmlFileList(LoadFileName, "RefrenceCandidate", "Value_");

                // コンボボックス更新
                Parent.UpdateRenameComboBox();
                Parent.UpdateMoveDestDirComboBox();

                // リストをリセット
                Parent.sf_listBox_Target.Items.Clear();
                Parent.rd_listView_Target.Items.Clear();
                Parent.pf_listView_Target.Items.Clear();
                Parent.sf_listBox_Target.Items.Clear();
            }

            return IsSuccess;
        }

        public Boolean SaveSetting(String SaveFileName, FileArranger Parent)
        {
            StcUtils util = new StcUtils();         // ツール系

            util.ModifyCombBoxList(Parent.md_comboBox_TargetDir);
            util.ModifyCombBoxList(Parent.rd_comboBox_RenameDir);
            util.ModifyCombBoxList(Parent.rd_comboBox_AddTitlePostWord);

            XmlDocument document = OpenSaveXmlFile();
            SaveXmlFile(document);
            SaveXmlParamAll("RefrenceCandidate", "Value_", Parent.RefrenceCandidateFolders);
            return CloseSaveXmlFile(SaveFileName);
        }

        // JSON保存/読込([[_Common/JsonFileStorage.cs]])。RegistLoadItemで登録済みのコントロールは
        // 汎用プロファイル(StcSaveRestore.BuildGenericProfile/ApplyGenericProfile)に詰め替えるだけで
        // 済むが、RefrenceCandidateFoldersだけはRegistCtrlを介さない専用の配列なので、
        // "RefrenceCandidate|Value_"というキーで同じprofileに相乗りさせる
        public Boolean SaveJsonFile(String filePath, FileArranger Parent)
        {
            try
            {
                StcUtils util = new StcUtils();
                util.ModifyCombBoxList(Parent.md_comboBox_TargetDir);
                util.ModifyCombBoxList(Parent.rd_comboBox_RenameDir);
                util.ModifyCombBoxList(Parent.rd_comboBox_AddTitlePostWord);

                GenericProfile profile = BuildGenericProfile();
                profile.Lists["RefrenceCandidate|Value_"] = (Parent.RefrenceCandidateFolders ?? new String[0]).ToList();

                JsonFileStorage.Save(filePath, profile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean LoadJsonFile(String filePath, FileArranger Parent)
        {
            GenericProfile profile = JsonFileStorage.Load<GenericProfile>(filePath);
            if (profile == null)
            {
                return false;
            }

            ApplyGenericProfile(profile);

            List<String> refFolders;
            Parent.RefrenceCandidateFolders = profile.Lists.TryGetValue("RefrenceCandidate|Value_", out refFolders)
                ? refFolders.ToArray()
                : new String[0];

            // コンボボックス更新・リストリセット(LoadProcと同じ後処理)
            Parent.UpdateRenameComboBox();
            Parent.UpdateMoveDestDirComboBox();

            Parent.sf_listBox_Target.Items.Clear();
            Parent.rd_listView_Target.Items.Clear();
            Parent.pf_listView_Target.Items.Clear();

            return true;
        }
    }
}
