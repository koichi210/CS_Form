using StandardTemplate;

namespace Mailer
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(Form1 parent)
        {
            SetElement("Setting");

            RegisterCtrl("Common", "textBox_BrowserPath", parent.textBox_BrowserPath);
            RegisterCtrl("Common", "textBox_MailTo", parent.textBox_MailTo);
            RegisterCtrl("Common", "textBox_MailCc", parent.textBox_MailCc);
            RegisterCtrl("Common", "textBox_MailBcc", parent.textBox_MailBcc);
            RegisterCtrl("Common", "textBox_MailSubject", parent.textBox_MailSubject);
            RegisterCtrl("Common", "textBox_MailBody", parent.textBox_MailBody);
        }

        // 以前ここにあった LoadProc(string, Form1)/SaveSetting(string, Form1) は、
        // Parentを一切使わずLoadXmlFile/SaveXmlFileへそのまま委譲するだけだった
        // （SaveSettingの Open→Write→Close の3行も、StcSaveRestore.SaveXmlFile(string)が
        // 内部で行っているのと同じ内容）。呼び出し元をLoadProc(string)/SaveSetting(string)
        // というStcSaveRestore側の共通実装に直接向けるよう変更し、このラッパーは削除した。
    }
}
