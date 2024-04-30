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
            RegistCtrl("Common", "textBox_CreateNum", Parent.textBox_CreateNum, "5");
            
        }

        public Boolean LoadProc(String LoadFileName, Form1 Parent)
        {
            return LoadXmlFile(LoadFileName);
        }

        public Boolean SaveSetting(String SaveFileName, Form1 Parent)
        {
            XmlDocument document = OpenSaveXmlFile();
            SaveXmlFile(document);
            return CloseSaveXmlFile(SaveFileName);
        }
    }
}
