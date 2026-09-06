using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using StandardTemplate;

namespace Mailer
{
    class SaveRestore : StcSaveRestore
    {
        public void RegistLoadItem(Form1 Parent)
        {
            SetElement("Setting");

            RegistCtrl("Common", "textBox_BrowserPath", Parent.textBox_BrowserPath);
            RegistCtrl("Common", "textBox_MailTo", Parent.textBox_MailTo);
            RegistCtrl("Common", "textBox_MailCc", Parent.textBox_MailCc);
            RegistCtrl("Common", "textBox_MailBcc", Parent.textBox_MailBcc);
            RegistCtrl("Common", "textBox_MailSubject", Parent.textBox_MailSubject);
            RegistCtrl("Common", "textBox_MailBody", Parent.textBox_MailBody);
        }

        // 以前ここにあった LoadProc(string, Form1)/SaveSetting(string, Form1) は、
        // Parentを一切使わずLoadXmlFile/SaveXmlFileへそのまま委譲するだけだった
        // （SaveSettingの Open→Write→Close の3行も、StcSaveRestore.SaveXmlFile(string)が
        // 内部で行っているのと同じ内容）。呼び出し元をLoadProc(string)/SaveSetting(string)
        // というStcSaveRestore側の共通実装に直接向けるよう変更し、このラッパーは削除した。
    }
}
