using System;
using StandardTemplate;

namespace ImageViewer
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(Form1 parent)
        {
            // コントロールを列挙
            RegisterCtrl("Parent", "textBox_FolderPath", parent.textBox_FolderPath);
            RegisterCtrl("Parent", "textBox_Extension", parent.textBox_Extension);
            // 並び順は表示名(名前順/サイズ順/更新日時順)で保存する。ファイルに無いときは名前順
            RegisterCtrl("Parent", "comboBox_SortOrder", parent.comboBox_SortOrder, FileSorter.DisplayNames[0]);
            RegisterCtrl("Parent", "checkBox_SortDescending", parent.checkBox_SortDescending, "False");
        }
    }
}
