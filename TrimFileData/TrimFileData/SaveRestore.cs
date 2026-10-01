using StandardTemplate;

namespace TrimFileData
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(Form1 parent)
        {
            // コントロールを列挙
            // 第2引数(設定ファイルのキー名)のtypoは修正済み。旧キー名で保存された既存の設定ファイルは
            // legacyAttrValueで旧キーを読み替えて読み込む([[_TechnicalNote/typo修正リスト.md]])
            RegisterCtrl("Common", "textBox_ReferencePath", parent.textBox_ReferencePath);
            RegisterCtrl("Common", "textBox_SearchCommonWord", parent.textBox_SearchCommonWord, legacyAttrValue: "textBox_SerchCommonWord");
            RegisterCtrl("Common", "checkBox_FirstWordOnly", parent.checkBox_FirstWordOnly);
            RegisterCtrl("Common", "checkBox_OrdinalCase", parent.checkBox_OrdinalCase);
            RegisterCtrl("Common", "textBox_SearchWordList", parent.textBox_SearchWordList, legacyAttrValue: "textBox_SerchWordList");
            RegisterCtrl("Common", "textBox_SearchResultList", parent.textBox_SearchResultList, legacyAttrValue: "textBox_SerchResultList");
        }

        // LoadProc(string) / SaveSetting(string) は StcSaveRestore 側の共通実装をそのまま使う
        // （「空なら何もしない→委譲」という中身が完全に同じだったため）。
    }
}
