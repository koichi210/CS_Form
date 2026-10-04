using System;
using StandardTemplate;

namespace PerforceWrapper
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(Form1 parent)
        {
            SetElement("Setting");

            RegisterCtrlList("Perforce", "comboBox_perforce_server", parent.comboBox_perforce_server);
            RegisterCtrl("Perforce", "comboBox_perforce_server", parent.comboBox_perforce_server);
            RegisterCtrlList("Perforce", "comboBox_perforce_user", parent.comboBox_perforce_user);
            RegisterCtrl("Perforce", "comboBox_perforce_user", parent.comboBox_perforce_user);
            RegisterSecureCtrl("Perforce", "textbox_perforce_password", parent.textBox_perforce_password);
            RegisterCtrlList("Perforce", "comboBox_perforce_workspace", parent.comboBox_perforce_workspace);
            RegisterCtrl("Perforce", "comboBox_perforce_workspace", parent.comboBox_perforce_workspace);
            RegisterCtrl("Perforce", "comboBox_perforce_charset", parent.comboBox_perforce_charset);
            RegisterCtrl("Perforce", "textBox_tree_list", parent.textBox_tree_list);

            RegisterCtrl("Perforce", "radioButton_so_menu_get_latest", parent.radioButton_so_menu_get_latest, "True");
            RegisterCtrl("Perforce", "radioButton_so_menu_restore", parent.radioButton_so_menu_restore, "False");
            RegisterCtrl("Perforce", "radioButton_so_menu_checkout", parent.radioButton_so_menu_checkout, "False");
            RegisterCtrl("Perforce", "radioButton_so_menu_delete", parent.radioButton_so_menu_delete, "False");
            RegisterCtrl("Perforce", "textBox_so_changelist", parent.textBox_so_changelist);

            RegisterCtrl("Perforce", "textBox_sl_label_name", parent.textBox_sl_label_name);
            RegisterCtrl("Perforce", "textBox_sl_base_changelist", parent.textBox_sl_base_changelist);

            RegisterCtrl("Perforce", "textBox_dl_src_label_name", parent.textBox_dl_src_label_name);
            RegisterCtrl("Perforce", "textBox_dl_src_tree", parent.textBox_dl_src_tree);
            RegisterCtrl("Perforce", "textBox_dl_dest_label_name", parent.textBox_dl_dest_label_name);
            RegisterCtrl("Perforce", "textBox_dl_dest_tree", parent.textBox_dl_dest_tree);

            RegisterCtrl("Perforce", "textBox_al_label_name", parent.textBox_al_label_name);
            RegisterCtrl("Perforce", "textBox_al_branch_map", parent.textBox_al_branch_map);
            RegisterCtrl("Perforce", "radioButton_al_copy", parent.radioButton_al_copy, "True");
            RegisterCtrl("Perforce", "radioButton_al_merge", parent.radioButton_al_merge, "False");
        }

        // LoadProc(string) は StcSaveRestore 側の共通実装をそのまま使う
        // （「空なら何もしない→委譲」という中身が完全に同じだったため）。

        // 保存前に、入力中のテキストをコンボボックスの履歴へ追加する(XML/JSON共通)
        public void AddComboBoxHistory(Form1 parent)
        {
            StcUtils util = new StcUtils();
            util.AddComboBoxTextToItems(parent.comboBox_perforce_server);
            util.AddComboBoxTextToItems(parent.comboBox_perforce_user);
            util.AddComboBoxTextToItems(parent.comboBox_perforce_workspace);
            util.AddComboBoxTextToItems(parent.comboBox_perforce_charset);
        }
    }
}
