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
            
            RegistCtrl("RenameDir", "rd_textBox_RenameDir", Parent.rd_textBox_RenameDir);
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
            util.ModifyCombBoxList(Parent.rd_comboBox_AddTitlePostWord);
            
            XmlDocument document = OpenSaveXmlFile();
            SaveXmlFile(document);
            SaveXmlParamAll("RefrenceCandidate", "Value_", Parent.RefrenceCandidateFolders);
            return CloseSaveXmlFile(SaveFileName);
        }
    }
}
