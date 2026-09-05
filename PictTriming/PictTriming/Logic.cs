using System;
using System.Drawing;
using System.IO;
using System.Xml;

namespace PictTriming
{
    /// <summary>
    /// もともと MainWindow.xaml.cs の Triming / SaveSetting_Click / LoadSetting に
    /// 実装されていたロジックをテストできる形に切り出したもの。コードはそのまま
    /// 移しただけで書き換えていない。BaseX.Text などのコントロール参照は、
    /// 呼び出し元(MainWindow)で読み取った値を引数として渡す/戻り値として
    /// 受け取る形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static void Triming(String TargetFilePath, String SourceFilePath, int BaseX, int BaseY, int Target_Width, int Target_Height)
        {
            //描画先とするImageオブジェクトを作成
            Bitmap canvas = new Bitmap(Target_Width, Target_Height);

            //画像ファイルのImageオブジェクトを作成
            Bitmap img = new Bitmap(SourceFilePath);

            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(BaseX, BaseY, Target_Width, Target_Height);

            //描画する部分の範囲を決定
            Rectangle desRect = new Rectangle(0, 0, Target_Width, Target_Height);

            //ImageオブジェクトのGraphicsオブジェクトを作成
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.DrawImage(img, desRect, srcRect, GraphicsUnit.Pixel);
                g.Dispose();
            }
            img.Dispose();

            canvas.Save(TargetFilePath);
            canvas.Dispose();
        }

        public class Settings
        {
            public String SourceFolderPath;
            public String BaseX;
            public String BaseY;
            public String TargetX;
            public String TargetY;
        }

        public static void SaveSettingXml(String Path, String SourceFolderPath, String BaseX, String BaseY, String TargetX, String TargetY)
        {
            XmlDocument document = new XmlDocument();

            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            XmlElement root = document.CreateElement("root");  // ルート要素

            document.AppendChild(declaration);
            document.AppendChild(root);

            AppendSetting(document, root, "SourceFolderPath", SourceFolderPath);
            AppendSetting(document, root, "BaseX", BaseX);
            AppendSetting(document, root, "BaseY", BaseY);
            AppendSetting(document, root, "TargetX", TargetX);
            AppendSetting(document, root, "TargetY", TargetY);

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

                if (attribute.Equals("SourceFolderPath"))
                {
                    settings.SourceFolderPath = text;
                }
                else if (attribute.Equals("BaseX"))
                {
                    settings.BaseX = text;
                }
                else if (attribute.Equals("BaseY"))
                {
                    settings.BaseY = text;
                }
                else if (attribute.Equals("TargetX"))
                {
                    settings.TargetX = text;
                }
                else if (attribute.Equals("TargetY"))
                {
                    settings.TargetY = text;
                }
            }

            return settings;
        }
    }
}
