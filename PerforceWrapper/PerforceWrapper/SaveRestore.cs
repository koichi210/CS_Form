﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Security.Cryptography;
using StandardTemplate;

namespace PerforceWrapper
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 Parent)
        {
            SetElement("Setting");

            RegistCtrlList("Perforce", "comboBox_perforce_server", Parent.comboBox_perforce_server);
            RegistCtrl("Perforce", "comboBox_perforce_server", Parent.comboBox_perforce_server);
            RegistCtrlList("Perforce", "comboBox_perforce_user", Parent.comboBox_perforce_user);
            RegistCtrl("Perforce", "comboBox_perforce_user", Parent.comboBox_perforce_user);
            RegistSecureCtrl("Perforce", "comboBox_perforce_password", Parent.textbox_perforce_password);
            RegistCtrlList("Perforce", "comboBox_perforce_workspace", Parent.comboBox_perforce_workspace);
            RegistCtrl("Perforce", "comboBox_perforce_workspace", Parent.comboBox_perforce_workspace);
            RegistCtrl("Perforce", "comboBox_perforce_charset", Parent.comboBox_perforce_charset);
            RegistCtrl("Perforce", "textBox_tree_list", Parent.textBox_tree_list);

            RegistCtrl("Perforce", "radioButton_so_menu_get_latest", Parent.radioButton_so_menu_get_latest, "True");
            RegistCtrl("Perforce", "radioButton_so_menu_restore", Parent.radioButton_so_menu_restore, "False");
            RegistCtrl("Perforce", "radioButton_so_menu_checkout", Parent.radioButton_so_menu_checkout, "False");
            RegistCtrl("Perforce", "radioButton_so_menu_delete", Parent.radioButton_so_menu_delete, "False");
            RegistCtrl("Perforce", "textBox_so_changelist", Parent.textBox_so_changelist);

            RegistCtrl("Perforce", "textBox_sl_label_name", Parent.textBox_sl_label_name);
            RegistCtrl("Perforce", "textBox_sl_base_changelist", Parent.textBox_sl_base_changelist);

            RegistCtrl("Perforce", "textBox_dl_src_label_name", Parent.textBox_dl_src_label_name);
            RegistCtrl("Perforce", "textBox_dl_src_tree", Parent.textBox_dl_src_tree);
            RegistCtrl("Perforce", "textBox_dl_dest_label_name", Parent.textBox_dl_dest_label_name);
            RegistCtrl("Perforce", "textBox_dl_dest_tree", Parent.textBox_dl_dest_tree);

            RegistCtrl("Perforce", "textBox_ak_label_name", Parent.textBox_ak_label_name);
            RegistCtrl("Perforce", "textBox_ak_branch_map", Parent.textBox_ak_branch_map);
            RegistCtrl("Perforce", "radioButton_al_copy", Parent.radioButton_al_copy, "True");
            RegistCtrl("Perforce", "radioButton_al_merge", Parent.radioButton_al_merge, "False");
        }

        public bool LoadProc(String LoadFileName)
        {
            if (LoadFileName == String.Empty)
            {
                return false;
            }

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
            util.ModifyCombBoxList(Parent.comboBox_perforce_server);
            util.ModifyCombBoxList(Parent.comboBox_perforce_user);
            util.ModifyCombBoxList(Parent.comboBox_perforce_workspace);
            util.ModifyCombBoxList(Parent.comboBox_perforce_charset);

            return SaveXmlFile(SaveFileName);
        }
    }
}
