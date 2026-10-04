using System;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace XmlReadWrite
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializePlaceholders();
            InitializeToolTips();
            textBox_FileName.Text = @"Sample.xml";
            textBox_Param1.Text = @"Hello World";
            textBox_Param2.Text = "123";
            textBox_Param3.Text = @"c:\tmp";
            textBox_Param4.Text = "テスト";
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnly・Multilineの欄は無いので全欄が対象
        // (Param1～4は読込4ボタンでXMLの値が書き戻されるが、書込用の入力欄でもあるので付けておく)
        private void InitializePlaceholders()
        {
            textBox_FileName.PlaceholderText = "例: Sample.xml";
            textBox_Param1.PlaceholderText = "例: Hello World";
            textBox_Param2.PlaceholderText = "例: 123";
            textBox_Param3.PlaceholderText = @"例: C:\Work";
            textBox_Param4.PlaceholderText = "例: テスト";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(button_write, "既存のXMLファイルを、ファイル名とParam1だけの内容で上書きする。ファイルが無いと書き込まない");
            toolTip.SetToolTip(button_write2, "既存のXMLファイルを、ファイル名とParam1～4の内容で上書きする。ファイルが無いと書き込まない");
            toolTip.SetToolTip(button_read1, "XMLの全ノードをコンソール出力に書き出す。画面には何も表示しない");
            toolTip.SetToolTip(button_read2, "要素名とFileName要素の値をコンソール出力に書き出す。画面には何も表示しない");
            toolTip.SetToolTip(button_read3, "各要素の属性をコンソール出力に書き出す。画面には何も表示しない");
            toolTip.SetToolTip(button_read4, "Write2で書いた形式のXMLを読み、Param1～4の欄に値を反映する");
        }

        // 対象のXMLファイルが無ければメッセージを出してfalseを返す(全ボタン共通の事前チェック)
        private bool CheckFileExists()
        {
            if (File.Exists(textBox_FileName.Text))
            {
                return true;
            }

            MessageBox.Show("ファイルがみつかりません。" + textBox_FileName.Text);
            return false;
        }

        // 空白ノードを無視するXmlTextReaderを作る
        private XmlTextReader CreateReader()
        {
            return new XmlTextReader(textBox_FileName.Text) { WhitespaceHandling = WhitespaceHandling.None };
        }

        private void button_write_Click(object sender, EventArgs e)
        {
            if (!CheckFileExists())
            {
                return;
            }

            using (XmlTextWriter writer = new XmlTextWriter(textBox_FileName.Text, null))
            {
                writer.WriteStartElement("x", "root", "urn:1");
                writer.WriteStartElement("Setting");

                writer.WriteStartElement("set");
                writer.WriteAttributeString("Param", "FileName");
                writer.WriteString(textBox_FileName.Text);
                writer.WriteEndElement();

                writer.WriteStartElement("set");
                writer.WriteAttributeString("Param", "Param1");
                writer.WriteString(textBox_Param1.Text);
                writer.WriteEndElement();

                //writer.WriteElementString("space", "hoge");
                //writer.WriteElementString("FileName", textBox_FileName.Text);
                //writer.WriteElementString("Param1", textBox_Param1.Text);
                //writer.WriteElementString("Param2", textBox_Param2.Text);
                //writer.WriteElementString("Param3", textBox_Param3.Text);
                //writer.WriteElementString("Param4", textBox_Param4.Text);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        private void button_write2_Click(object sender, EventArgs e)
        {
            if (!CheckFileExists())
            {
                return;
            }

            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            XmlElement root = document.CreateElement("root");  // ルート要素

            document.AppendChild(declaration);
            document.AppendChild(root);

            AppendSettingElement(document, root, "FileName", textBox_FileName.Text);
            AppendSettingElement(document, root, "Param1", textBox_Param1.Text);
            AppendSettingElement(document, root, "Param2", textBox_Param2.Text);
            AppendSettingElement(document, root, "Param3", textBox_Param3.Text);
            AppendSettingElement(document, root, "Param4", textBox_Param4.Text);

            // ファイルに保存する
            document.Save(textBox_FileName.Text);
        }

        // <Setting attribute="属性">内容</Setting> をrootの子に追加する
        private static void AppendSettingElement(XmlDocument document, XmlElement root, string attribute, string text)
        {
            XmlElement element = document.CreateElement("Setting");
            element.SetAttribute("attribute", attribute);  // 属性
            element.InnerText = text;                      // 要素の内容
            root.AppendChild(element);
        }

        private void button_read_Click(object sender, EventArgs e)
        {
            if (!CheckFileExists())
            {
                return;
            }

            using (XmlTextReader reader = CreateReader())
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.Write("<{0}> ", reader.Name);
                            break;

                        case XmlNodeType.Text:
                            Console.Write(reader.Value);
                            break;

                        case XmlNodeType.CDATA:
                            Console.Write("<![CDATA[{0}]]>", reader.Value);
                            break;

                        case XmlNodeType.ProcessingInstruction:
                            Console.Write("<?{0} {1}?>", reader.Name, reader.Value);
                            break;

                        case XmlNodeType.Comment:
                            Console.Write("<!--{0}-->", reader.Value);
                            break;

                        case XmlNodeType.XmlDeclaration:
                            Console.Write("<?xml version='1.0'?>");
                            break;

                        case XmlNodeType.DocumentType:
                            Console.Write("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
                            break;

                        case XmlNodeType.EntityReference:
                            Console.Write(reader.Name);
                            break;

                        case XmlNodeType.EndElement:
                            Console.Write("</{0}>", reader.Name);
                            break;
                    }
                }
            }
        }

        private void button_read2_Click(object sender, EventArgs e)
        {
            if (!CheckFileExists())
            {
                return;
            }

            using (XmlTextReader reader = CreateReader())
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    Console.Write("<{0}> ", reader.Name);
                    if (reader.Name.Equals("FileName"))
                    {
                        Console.Write("<{0}>", reader.ReadElementString(reader.Name));
                    }
                }
            }
        }

        private void button_read3_Click(object sender, EventArgs e)
        {
            if (!CheckFileExists())
            {
                return;
            }

            using (XmlTextReader reader = CreateReader())
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    if (reader.HasAttributes)
                    {
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            // 属性ノードへ移動
                            reader.MoveToAttribute(i);
                            // 属性名、及び属性の値を表示
                            Console.Write("{0} = {1} ", reader.Name, reader.Value);
                        }
                    }
                    else
                    {
                        Console.Write(reader.Value);
                    }
                }
            }
        }

        private void button_read4_Click(object sender, EventArgs e)
        {
            if (!CheckFileExists())
            {
                return;
            }

            XmlDocument document = new XmlDocument();

            // ファイルから読み込む
            document.Load(textBox_FileName.Text);

            foreach (XmlElement element in document.DocumentElement)
            {
                string text = element.InnerText;                        // 要素の内容

                switch (element.GetAttribute("attribute"))              // 属性
                {
                    case "Param1":
                        textBox_Param1.Text = text;
                        break;
                    case "Param2":
                        textBox_Param2.Text = text;
                        break;
                    case "Param3":
                        textBox_Param3.Text = text;
                        break;
                    case "Param4":
                        textBox_Param4.Text = text;
                        break;
                }
            }
        }
    }
}
