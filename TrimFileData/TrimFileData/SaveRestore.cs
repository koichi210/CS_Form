using StandardTemplate;

namespace TrimFileData
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 parent)
        {
            // コントロールを列挙
            // 第2引数(設定ファイルのキー名)にtypoが残っているものがあるが、ここを直すと
            // 既存の設定ファイルの値が読めなくなるため、コントロール名だけを修正してある。
            // キー名はXML->JSON移行時に旧キーの読み替えと一緒に直す([[_TechnicalNote/typo修正リスト.md]])
            RegistCtrl("Common", "textBox_ReferencePath", parent.textBox_ReferencePath);
            RegistCtrl("Common", "textBox_SearchCommonWord", parent.textBox_SearchCommonWord, LegacyAttrValue: "textBox_SerchCommonWord");
            RegistCtrl("Common", "checkBox_FirstWordOnly", parent.checkBox_FirstWordOnly);
            RegistCtrl("Common", "checkBox_OrdinalCase", parent.checkBox_OrdinalCase);
            RegistCtrl("Common", "textBox_SearchWordList", parent.textBox_SearchWordList, LegacyAttrValue: "textBox_SerchWordList");
            RegistCtrl("Common", "textBox_SearchResultList", parent.textBox_SearchResultList, LegacyAttrValue: "textBox_SerchResultList");
        }

        // LoadProc(string) / SaveSetting(string) は StcSaveRestore 側の共通実装をそのまま使う
        // （「空なら何もしない→委譲」という中身が完全に同じだったため）。
    }
}
