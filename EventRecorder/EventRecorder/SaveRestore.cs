using System;
using StandardTemplate;

namespace EventRecorder
{
    // マクロ(記録したイベント一覧+ループ回数)のXML保存/読込
    // ※ いずれJSON保存に置き換えたい(やりたいことリスト)
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 Parent)
        {
            SetElement("Setting");

            RegistCtrl("Record", "textBox_Loop", Parent.textBox_Loop, "1");
            RegistCtrl("Record", "Cell", "RowCount", Parent.dataGridView_Events);
        }

        public Boolean LoadProc(String LoadFileName, Form1 Parent)
        {
            if (LoadFileName == String.Empty)
            {
                return false;
            }

            Parent.dataGridView_Events.Rows.Clear();
            return LoadXmlFile(LoadFileName);
        }
    }
}
