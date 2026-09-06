using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StandardTemplate;

namespace TrimHtmlData
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 Parent)
        {
            // コントロールを列挙
            RegistCtrl("Common", "textBox_SourceList", Parent.textBox_SourceList);
            RegistCtrl("Common", "textBox_DestList", Parent.textBox_DestList);
            RegistCtrl("Common", "textBox_SearchWord", Parent.textBox_SearchWord);
            RegistCtrl("Common", "textBox_DelimiterWord", Parent.textBox_DelimiterWord);
            RegistCtrl("Common", "textBox_TrimLineNum", Parent.textBox_TrimLineNum);
            RegistCtrl("Common", "checkBox_OrdinalCase", Parent.checkBox_OrdinalCase);
        }

        // LoadProc(string) / SaveSetting(string) は StcSaveRestore 側の共通実装をそのまま使う
        // （「空なら何もしない→委譲」という中身が完全に同じだったため）。
    }
}
