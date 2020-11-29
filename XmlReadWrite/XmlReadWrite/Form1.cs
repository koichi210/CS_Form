using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            textBox_FileName.Text = @"Sample.xml";
            textBox_Param1.Text = @"Hello World";
            textBox_Param2.Text = "123";
            textBox_Param3.Text = @"c:\tmp";
            textBox_Param4.Text = "テスト";
        }

        private void button_write_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_FileName.Text))
            {
                MessageBox.Show("ファイルがみつかりません。" + textBox_FileName.Text);
                return;
            }

            XmlTextWriter write = null;

            try
            {
                write = new XmlTextWriter(textBox_FileName.Text, null);
                write.WriteStartElement("x", "root", "urn:1");
                write.WriteStartElement("Setting");

                write.WriteStartElement("set");
                write.WriteAttributeString("Param", "FileName");
                write.WriteString(textBox_FileName.Text);
                write.WriteEndElement();

                write.WriteStartElement("set");
                write.WriteAttributeString("Param", "Param1");
                write.WriteString(textBox_Param1.Text);
                write.WriteEndElement();

                //write.WriteElementString("space", "hoge");
                //write.WriteElementString("FileName", textBox_FileName.Text);
                //write.WriteElementString("Param1", textBox_Param1.Text);
                //write.WriteElementString("Param2", textBox_Param2.Text);
                //write.WriteElementString("Param3", textBox_Param3.Text);
                //write.WriteElementString("Param4", textBox_Param4.Text);
                write.WriteEndElement();
                write.WriteEndElement();
                write.Close();
            }

            finally
            {
                if (write != null)
                {
                    write.Close();
                }
            }
        }

        private void button_write2_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_FileName.Text))
            {
                MessageBox.Show("ファイルがみつかりません。" + textBox_FileName.Text);
                return;
            }

            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            XmlElement root = document.CreateElement("root");  // ルート要素

            document.AppendChild(declaration);
            document.AppendChild(root);

            //XmlElement element = document.CreateElement("Setting");
            //element.InnerText = "text";                     // 要素の内容
            //element.SetAttribute("attribute", "256");       // 属性
            //root.AppendChild(element);

            XmlElement element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "FileName");  // 属性
            element.InnerText = textBox_FileName.Text;      // 要素の内容
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "Param1");    // 属性
            element.InnerText = textBox_Param1.Text;        // 要素の内容
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "Param2");    // 属性
            element.InnerText = textBox_Param2.Text;        // 要素の内容
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "Param3");    // 属性
            element.InnerText = textBox_Param3.Text;        // 要素の内容
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "Param4");    // 属性
            element.InnerText = textBox_Param4.Text;        // 要素の内容
            root.AppendChild(element);

            // ファイルに保存する
            document.Save(textBox_FileName.Text);
        }

        private void button_read_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_FileName.Text))
            {
                MessageBox.Show("ファイルがみつかりません。" + textBox_FileName.Text);
                return;
            }

            XmlTextReader reader = null;

            try
            {
                // Load the reader with the data file and ignore all white space nodes.         
                reader = new XmlTextReader(textBox_FileName.Text);
                reader.WhitespaceHandling = WhitespaceHandling.None;

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

                        case XmlNodeType.Document:
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

                        default:
                            break;
                    }
                }
            }

            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private void button_read2_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_FileName.Text))
            {
                MessageBox.Show("ファイルがみつかりません。" + textBox_FileName.Text);
                return;
            }

            XmlTextReader reader = null;

            try
            {
                // Load the reader with the data file and ignore all white space nodes.         
                reader = new XmlTextReader(textBox_FileName.Text);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.Write("<{0}> ", reader.Name);
                            if (reader.Name.Equals("FileName") )
                            {
                                Console.Write("<{0}>", reader.ReadElementString(reader.Name));
                            }
                            break;

                        default :
                            break;
                    }
                }
            }

            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private void button_read3_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_FileName.Text))
            {
                MessageBox.Show("ファイルがみつかりません。" + textBox_FileName.Text);
                return;
            }

            XmlTextReader reader = null;

            try
            {
                // Load the reader with the data file and ignore all white space nodes.         
                reader = new XmlTextReader(textBox_FileName.Text);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.HasAttributes)
                        {
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                // 属性ノードへ移動
                                reader.MoveToAttribute(i);
                                // 属性名、及び属性の値を表示
                                Console.Write("{1} = {2} ", i, reader.Name, reader.Value);
                            }
                        }
                        else
                        {
                            Console.Write("{0}",reader.Value);
                        }
                    }
                }
            }

            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private void button_read4_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_FileName.Text))
            {
                MessageBox.Show("ファイルがみつかりません。" + textBox_FileName.Text);
                return;
            }

            XmlDocument document = new XmlDocument();

            // ファイルから読み込む
            document.Load(textBox_FileName.Text);

            foreach (XmlElement element in document.DocumentElement)
            {
                string attribute = element.GetAttribute("attribute");   // 属性
                string text = element.InnerText;                        // 要素の内容

                if (attribute.Equals("Param1"))
                {
                    textBox_Param1.Text = text;
                }
                else if (attribute.Equals("Param2"))
                {
                    textBox_Param2.Text = text;
                }
                else if (attribute.Equals("Param3"))
                {
                    textBox_Param3.Text = text;
                }
                else if (attribute.Equals("Param4"))
                {
                    textBox_Param4.Text = text;
                }
            }
        }
    }
}
