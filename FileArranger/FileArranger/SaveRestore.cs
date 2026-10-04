using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using StandardTemplate;

namespace FileArranger
{
    class SaveRestore : StcSaveRestore
    {
        // ReferenceCandidateFoldersの保存キー(XMLの属性名 / 要素の属性値の接頭辞)
        private const String _referenceCandidateAttrName = "ReferenceCandidate";
        private const String _referenceCandidateAttrValue = "Value_";
        // typo修正前の旧キー。旧キーで保存された既存の設定ファイルも読めるよう、新キーで見つからなければこちらを読む
        private const String _legacyReferenceCandidateAttrName = "RefrenceCandidate";

        private static String MakeJsonListKey(String attrName)
        {
            return attrName + "|" + _referenceCandidateAttrValue;
        }

        public void RegisterLoadItem(FileArranger parent)
        {
            SetElement("Setting");
            // 第2引数(設定ファイルのキー名)のtypoは修正済み。旧キー名で保存された既存の設定ファイルは
            // legacyAttrValueで旧キーを読み替えて読み込む([[_Common/StandardTemplateClass.cs]]のOriginDB)

            RegisterCtrl("Common", "cmn_textBox_Reference", parent.cmn_textBox_Reference);
            RegisterCtrl("Common", "cmn_textBox_AddList", parent.cmn_textBox_AddList);
            RegisterCtrl("Common", "cmn_textBox_AddListSuffix", parent.cmn_textBox_AddListSuffix);
            
            RegisterCtrl("MoveDir", "md_textBox_SourceDir", parent.md_textBox_SourceDir);
            RegisterCtrlList("MoveDir", "md_comboBox_TargetDir", parent.md_comboBox_TargetDir);
            RegisterCtrl("MoveDir", "md_comboBox_TargetDir", parent.md_comboBox_TargetDir);

           
            RegisterCtrlList("MoveDir", "rd_comboBox_RenameDir", parent.rd_comboBox_RenameDir);
            RegisterCtrl("MoveDir", "rd_comboBox_RenameDir", parent.rd_comboBox_RenameDir);
            RegisterCtrl("RenameDir", "rd_textBox_ExistItemDir", parent.rd_textBox_ExistItemDir);
            RegisterCtrl("RenameDir", "rd_comboBox_MergeWord", parent.rd_comboBox_MergeWord);
            RegisterCtrl("RenameDir", "rd_checkBox_FileOpen", parent.rd_checkBox_FileOpen);
            RegisterCtrl("RenameDir", "rd_textBox_SplitWord3", parent.rd_textBox_SplitWord3);
            RegisterCtrl("RenameDir", "rd_textBox_AddTitlePreWord", parent.rd_textBox_AddTitlePreWord);
            RegisterCtrl("RenameDir", "rd_textBox_SearchTitleLine", parent.rd_textBox_SearchTitleLine);
            RegisterCtrl("RenameDir", "rd_textBox_SearchTitleLength", parent.rd_textBox_SearchTitleLength);
            RegisterCtrlList("RenameDir", "rd_comboBox_AddTitlePostWord", parent.rd_comboBox_AddTitlePostWord);
            RegisterCtrl("RenameDir", "rd_comboBox_AddTitlePostWord", parent.rd_comboBox_AddTitlePostWord);

            RegisterCtrl("SortFileName", "sf_textBox_TargetFile", parent.sf_textBox_TargetFile);

            RegisterCtrl("MoveFile", "mf_textBox_SourceDir", parent.mf_textBox_SourceDir);
            RegisterCtrl("MoveFile", "mf_textBox_TargetDir", parent.mf_textBox_TargetDir);

            RegisterCtrl("PartitionFile", "pf_textBox_TargetFile", parent.pf_textBox_TargetFile);
            RegisterCtrl("PartitionFile", "pf_textBox_ReferenceFile", parent.pf_textBox_ReferenceFile, legacyAttrValue: "pf_textBox_RefrenceFile");
            RegisterCtrl("PartitionFile", "pf_textBox_TargetSeparator", parent.pf_textBox_TargetSeparator, legacyAttrValue: "pf_textBox_TargetSeprator");
            RegisterCtrl("PartitionFile", "pf_textBox_SearchTitleLine", parent.pf_textBox_SearchTitleLine);
            RegisterCtrl("PartitionFile", "pf_textBox_SearchTitleLength", parent.pf_textBox_SearchTitleLength);
            RegisterCtrl("PartitionFile", "pf_checkBox_CreateNewDir", parent.pf_checkBox_CreateNewDir);
        }

        public Boolean LoadProc(String loadFileName, FileArranger parent)
        {
            Boolean isSuccess = LoadXmlFile(loadFileName);
            if (isSuccess)
            {
                String[] folders = LoadXmlFileList(loadFileName, _referenceCandidateAttrName, _referenceCandidateAttrValue);
                if (folders.Length == 0)
                {
                    folders = LoadXmlFileList(loadFileName, _legacyReferenceCandidateAttrName, _referenceCandidateAttrValue);
                }
                parent.ReferenceCandidateFolders = folders;
                RefreshAfterLoad(parent);
            }

            return isSuccess;
        }

        public Boolean SaveSetting(String saveFileName, FileArranger parent)
        {
            ModifyComboBoxLists(parent);

            XmlDocument document = OpenSaveXmlFile();
            SaveXmlFile(document);
            SaveXmlParamAll(_referenceCandidateAttrName, _referenceCandidateAttrValue, parent.ReferenceCandidateFolders);
            return CloseSaveXmlFile(saveFileName);
        }

        // 保存前に、履歴を持つコンボボックスのリストを整える(XML/JSON共通)
        private static void ModifyComboBoxLists(FileArranger parent)
        {
            StcUtils util = new StcUtils();
            util.AddComboBoxTextToItems(parent.md_comboBox_TargetDir);
            util.AddComboBoxTextToItems(parent.rd_comboBox_RenameDir);
            util.AddComboBoxTextToItems(parent.rd_comboBox_AddTitlePostWord);
        }

        // 読み込み後の後処理(XML/JSON共通)。コンボボックスを更新し、リストをリセットする
        private static void RefreshAfterLoad(FileArranger parent)
        {
            // コンボボックス更新
            parent.UpdateRenameComboBox();
            parent.UpdateMoveDestDirComboBox();

            // リストをリセット
            parent.sf_listBox_Target.Items.Clear();
            parent.rd_listView_Target.Items.Clear();
            parent.pf_listView_Target.Items.Clear();
        }

        // JSON保存/読込([[_Common/JsonFileStorage.cs]])。RegisterLoadItemで登録済みのコントロールは
        // 汎用プロファイル(StcSaveRestore.BuildGenericProfile/ApplyGenericProfile)に詰め替えるだけで
        // 済むが、ReferenceCandidateFoldersだけはRegisterCtrlを介さない専用の配列なので、
        // "ReferenceCandidate|Value_"というキーで同じprofileに相乗りさせる
        public Boolean SaveJsonFile(String filePath, FileArranger parent)
        {
            try
            {
                ModifyComboBoxLists(parent);

                GenericProfile profile = BuildGenericProfile();
                profile.Lists[MakeJsonListKey(_referenceCandidateAttrName)] = (parent.ReferenceCandidateFolders ?? new String[0]).ToList();

                JsonFileStorage.Save(filePath, profile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean LoadJsonFile(String filePath, FileArranger parent)
        {
            GenericProfile profile = JsonFileStorage.Load<GenericProfile>(filePath);
            if (profile == null)
            {
                return false;
            }

            ApplyGenericProfile(profile);

            List<String> refFolders;
            Boolean hasFolders = profile.Lists.TryGetValue(MakeJsonListKey(_referenceCandidateAttrName), out refFolders)
                || profile.Lists.TryGetValue(MakeJsonListKey(_legacyReferenceCandidateAttrName), out refFolders);
            parent.ReferenceCandidateFolders = hasFolders ? refFolders.ToArray() : new String[0];

            // コンボボックス更新・リストリセット(LoadProcと同じ後処理)
            RefreshAfterLoad(parent);

            return true;
        }
    }
}
