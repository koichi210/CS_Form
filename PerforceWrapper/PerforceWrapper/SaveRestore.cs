﻿using System;
using StandardTemplate;

namespace PerforceWrapper
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 parent)
        {
            SetElement("Setting");

            RegistCtrlList("Perforce", "comboBox_perforce_server", parent.comboBox_perforce_server);
            RegistCtrl("Perforce", "comboBox_perforce_server", parent.comboBox_perforce_server);
            RegistCtrlList("Perforce", "comboBox_perforce_user", parent.comboBox_perforce_user);
            RegistCtrl("Perforce", "comboBox_perforce_user", parent.comboBox_perforce_user);
            RegistSecureCtrl("Perforce", "comboBox_perforce_password", parent.textbox_perforce_password);
            RegistCtrlList("Perforce", "comboBox_perforce_workspace", parent.comboBox_perforce_workspace);
            RegistCtrl("Perforce", "comboBox_perforce_workspace", parent.comboBox_perforce_workspace);
            RegistCtrl("Perforce", "comboBox_perforce_charset", parent.comboBox_perforce_charset);
            RegistCtrl("Perforce", "textBox_tree_list", parent.textBox_tree_list);

            RegistCtrl("Perforce", "radioButton_so_menu_get_latest", parent.radioButton_so_menu_get_latest, "True");
            RegistCtrl("Perforce", "radioButton_so_menu_restore", parent.radioButton_so_menu_restore, "False");
            RegistCtrl("Perforce", "radioButton_so_menu_checkout", parent.radioButton_so_menu_checkout, "False");
            RegistCtrl("Perforce", "radioButton_so_menu_delete", parent.radioButton_so_menu_delete, "False");
            RegistCtrl("Perforce", "textBox_so_changelist", parent.textBox_so_changelist);

            RegistCtrl("Perforce", "textBox_sl_label_name", parent.textBox_sl_label_name);
            RegistCtrl("Perforce", "textBox_sl_base_changelist", parent.textBox_sl_base_changelist);

            RegistCtrl("Perforce", "textBox_dl_src_label_name", parent.textBox_dl_src_label_name);
            RegistCtrl("Perforce", "textBox_dl_src_tree", parent.textBox_dl_src_tree);
            RegistCtrl("Perforce", "textBox_dl_dest_label_name", parent.textBox_dl_dest_label_name);
            RegistCtrl("Perforce", "textBox_dl_dest_tree", parent.textBox_dl_dest_tree);

            RegistCtrl("Perforce", "textBox_ak_label_name", parent.textBox_ak_label_name);
            RegistCtrl("Perforce", "textBox_ak_branch_map", parent.textBox_ak_branch_map);
            RegistCtrl("Perforce", "radioButton_al_copy", parent.radioButton_al_copy, "True");
            RegistCtrl("Perforce", "radioButton_al_merge", parent.radioButton_al_merge, "False");
        }

        // LoadProc(string) は StcSaveRestore 側の共通実装をそのまま使う
        // （「空なら何もしない→委譲」という中身が完全に同じだったため）。

        public bool SaveSetting(String saveFileName, Form1 parent)
        {
            if (saveFileName == String.Empty)
            {
                return false;
            }

            // コンボボックスの更新
            StcUtils util = new StcUtils();
            util.ModifyCombBoxList(parent.comboBox_perforce_server);
            util.ModifyCombBoxList(parent.comboBox_perforce_user);
            util.ModifyCombBoxList(parent.comboBox_perforce_workspace);
            util.ModifyCombBoxList(parent.comboBox_perforce_charset);

            return SaveXmlFile(saveFileName);
        }
    }
}
