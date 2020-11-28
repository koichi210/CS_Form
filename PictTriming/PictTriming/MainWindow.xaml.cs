using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Xml;

namespace PictTriming
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly String SaveXmlFile = @"PictTrim.xml";

        public MainWindow()
        {
            InitializeComponent();
            Radio_SelectPointOfEnd.IsChecked = true;
#if DEBUG 
            SourceFolderPath.Text = @"C:\tmp";
#endif
            LoadSetting();
        }

        private void Button_Trim_Click(object sender, RoutedEventArgs e)
        {
            int Target_Width;
            int Target_Height;

            if (Radio_SelectPointOfEnd.IsChecked.Value)
            {
                Target_Width = int.Parse(TargetX.Text) - int.Parse(BaseX.Text);
                Target_Height = int.Parse(TargetY.Text) - int.Parse(BaseY.Text);
            }
            else // if ( Radio_SelectSizeOfEnd.IsChecked.Value )
            {
                Target_Width = int.Parse(TargetX.Text);
                Target_Height = int.Parse(TargetY.Text);
            }

            for (int i = 0; i < ListBox_ListUp.SelectedItems.Count; i++)
            {
                String FilePath         = SourceFolderPath.Text + @"\" + ListBox_ListUp.SelectedItems[i].ToString();
                String BackUpFilePath   = SourceFolderPath.Text + @"\" + @"org" + @"\" + ListBox_ListUp.SelectedItems[i].ToString();

                // オリジナルファイルをバックアップ
                File.Copy(FilePath, BackUpFilePath, true);

                // トリミング
                Triming(FilePath, BackUpFilePath, Target_Width, Target_Height);
            }

            //ListupExecute();
        }

        private void Triming(String TargetFilePath, String SourceFilePath, int Target_Width, int Target_Height)
        {
            //描画先とするImageオブジェクトを作成
            //Bitmap canvas = new Bitmap(PictureBox1.Width, PictureBox1.Height);
            Bitmap canvas = new Bitmap(Target_Width, Target_Height);

            //画像ファイルのImageオブジェクトを作成
            //Bitmap img = new Bitmap(@"C:\test\1.jpg");
            Bitmap img = new Bitmap(SourceFilePath);

            //切り取る部分の範囲を決定
            System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(int.Parse(BaseX.Text), int.Parse(BaseY.Text), Target_Width, Target_Height);

            //描画する部分の範囲を決定
            System.Drawing.Rectangle desRect = new System.Drawing.Rectangle(0, 0, Target_Width, Target_Height);

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

        private void Button_Listup_Click(object sender, RoutedEventArgs e)
        {
            ListupExecute();
        }

        private void ListupExecute()
        {
            if (SourceFolderPath.Text.Equals(""))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }
            ListBox_ListUp.Items.Clear();

            string[] files = Directory.GetFiles(SourceFolderPath.Text, "*", SearchOption.TopDirectoryOnly);

            //配列の内容を一つ一つ追加する
            for (int i = 0; i <= files.Length - 1; i++)
            {
                var FileName = System.IO.Path.GetFileName(files[i]);
                ListBox_ListUp.Items.Add(FileName);
            }
        }

        private void SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument document = new XmlDocument();

            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            XmlElement root = document.CreateElement("root");  // ルート要素

            document.AppendChild(declaration);
            document.AppendChild(root);

            XmlElement element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "SourceFolderPath");
            element.InnerText = SourceFolderPath.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "BaseX");
            element.InnerText = BaseX.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "BaseY");
            element.InnerText = BaseY.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "TargetX");
            element.InnerText = TargetX.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "TargetY");
            element.InnerText = TargetY.Text;
            root.AppendChild(element);

            // ファイルに保存する
            document.Save(SaveXmlFile);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            if ( ! File.Exists(SaveXmlFile))
            {
                return;
            }

            // ファイルから読み込む
            XmlDocument document = new XmlDocument();
            document.Load(SaveXmlFile);

            foreach (XmlElement element in document.DocumentElement)
            {
                string attribute = element.GetAttribute("attribute");   // 属性
                string text = element.InnerText;                        // 要素の内容

                if (attribute.Equals("SourceFolderPath"))
                {
                    SourceFolderPath.Text = text;
                }
                else if (attribute.Equals("BaseX"))
                {
                    BaseX.Text = text;
                }
                else if (attribute.Equals("BaseY"))
                {
                    BaseY.Text = text;
                }
                else if (attribute.Equals("TargetX"))
                {
                    TargetX.Text = text;
                }
                else if (attribute.Equals("TargetY"))
                {
                    TargetY.Text = text;
                }
            }
          }

        private void SourceFolderPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListupExecute();
            }
        }
    }
}
