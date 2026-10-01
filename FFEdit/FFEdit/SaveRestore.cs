using System;
using StandardTemplate;

namespace FFEdit
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 parent)
        {
            SetElement("Setting");

            // コントロールを列挙
            RegistCtrlList("comboBox_TargetDir", "Value_", parent.comboBox_TargetDir);
            RegistCtrlList("comboBox_String1", "Value_", parent.comboBox_String1);
            RegistCtrlList("comboBox_String2", "Value_", parent.comboBox_String2);
            RegistCtrl("textBox_Target_Extension", "Value", parent.textBox_Target_Extension, "*");
        }

        public bool LoadProc(String loadFileName, Form1 parent)
        {
            if (loadFileName == String.Empty)
            {
                return false;
            }

            // Default設定
            parent.textBox_Target_Extension.Text = "*";

            return LoadXmlFile(loadFileName);
        }

        public bool SaveSetting(String saveFileName, Form1 parent)
        {
            if (saveFileName == String.Empty)
            {
                return false;
            }

            UpdateComboHistory(parent);

            return SaveXmlFile(saveFileName);
        }

        // 入力中の文字をコンボボックスの履歴に足す。XML/JSONどちらで保存する場合も保存前に呼ぶこと
        public void UpdateComboHistory(Form1 parent)
        {
            StcUtils util = new StcUtils();
            util.ModifyCombBoxList(parent.comboBox_TargetDir);
            util.ModifyCombBoxList(parent.comboBox_String1);
            util.ModifyCombBoxList(parent.comboBox_String2);
        }
    }
}
