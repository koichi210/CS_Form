using StandardTemplate;

namespace TrimHtmlData
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(Form1 parent)
        {
            // コントロールを列挙
            RegisterCtrl("Common", "textBox_SourceList", parent.textBox_SourceList);
            RegisterCtrl("Common", "textBox_DestList", parent.textBox_DestList);
            RegisterCtrl("Common", "textBox_SearchWord", parent.textBox_SearchWord);
            RegisterCtrl("Common", "textBox_DelimiterWord", parent.textBox_DelimiterWord);
            RegisterCtrl("Common", "textBox_TrimLineNum", parent.textBox_TrimLineNum);
            RegisterCtrl("Common", "checkBox_OrdinalCase", parent.checkBox_OrdinalCase);
        }

        // LoadProc(string) / SaveSetting(string) は StcSaveRestore 側の共通実装をそのまま使う
        // （「空なら何もしない→委譲」という中身が完全に同じだったため）。
    }
}
