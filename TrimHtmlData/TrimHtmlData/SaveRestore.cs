using StandardTemplate;

namespace TrimHtmlData
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 parent)
        {
            // コントロールを列挙
            RegistCtrl("Common", "textBox_SourceList", parent.textBox_SourceList);
            RegistCtrl("Common", "textBox_DestList", parent.textBox_DestList);
            RegistCtrl("Common", "textBox_SearchWord", parent.textBox_SearchWord);
            RegistCtrl("Common", "textBox_DelimiterWord", parent.textBox_DelimiterWord);
            RegistCtrl("Common", "textBox_TrimLineNum", parent.textBox_TrimLineNum);
            RegistCtrl("Common", "checkBox_OrdinalCase", parent.checkBox_OrdinalCase);
        }

        // LoadProc(string) / SaveSetting(string) は StcSaveRestore 側の共通実装をそのまま使う
        // （「空なら何もしない→委譲」という中身が完全に同じだったため）。
    }
}
