using System;
using System.IO;
using System.Xml;

namespace CaptureWindow
{
    /// <summary>
    /// もともと Form1.cs の SaveSetting_Click / LoadSetting に実装されていた、
    /// 設定値をXMLファイルに保存/読み込みするロジックをテストできる形に切り出した
    /// もの。コードはそのまま移しただけで書き換えていない。TextBoxのコントロール
    /// 参照は、呼び出し元(Form1)で読み取った値を引数として渡す/戻り値として
    /// 受け取る形に変えた。
    /// </summary>
    internal static class Logic
    {
        public class Settings
        {
            public String SavePath;
            public String MouseX;
            public String MouseY;
            public String Sleep;
        }

        public static void SaveSettingXml(String Path, String SavePath, String MouseX, String MouseY, String Sleep)
        {
            XmlDocument document = new XmlDocument();

            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            XmlElement root = document.CreateElement("root");  // ルート要素

            document.AppendChild(declaration);
            document.AppendChild(root);

            AppendSetting(document, root, "TextBox_SavePath", SavePath);
            AppendSetting(document, root, "TextBox_MouseX", MouseX);
            AppendSetting(document, root, "TextBox_MouseY", MouseY);
            AppendSetting(document, root, "TextBox_Sleep", Sleep);

            // ファイルに保存する
            document.Save(Path);
        }

        private static void AppendSetting(XmlDocument document, XmlElement root, String attribute, String text)
        {
            XmlElement element = document.CreateElement("Setting");
            element.SetAttribute("attribute", attribute);
            element.InnerText = text;
            root.AppendChild(element);
        }

        public static Settings LoadSettingXml(String Path)
        {
            if (!File.Exists(Path))
            {
                return null;
            }

            // ファイルから読み込む
            XmlDocument document = new XmlDocument();
            document.Load(Path);

            Settings settings = new Settings();
            foreach (XmlElement element in document.DocumentElement)
            {
                string attribute = element.GetAttribute("attribute");   // 属性
                string text = element.InnerText;                        // 要素の内容

                if (attribute.Equals("TextBox_SavePath"))
                {
                    settings.SavePath = text;
                }
                else if (attribute.Equals("TextBox_MouseX"))
                {
                    settings.MouseX = text;
                }
                else if (attribute.Equals("TextBox_MouseY"))
                {
                    settings.MouseY = text;
                }
                else if (attribute.Equals("TextBox_Sleep"))
                {
                    settings.Sleep = text;
                }
            }

            return settings;
        }
    }
}
