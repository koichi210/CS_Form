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

using PicEdit;

namespace PictMerge
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly String SaveXmlFile = @"PictMerge.xml";

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG 
            PictWidth.Text = "200";
            PictHeight.Text = "200";
            TrimingHeight.Text = "50" + Environment.NewLine + "100" + Environment.NewLine + "150";
            SourceFolderPath.Text = @"D:\tmp\cheetos\Test3\Color";
#endif
            SourceFile1Prefix.Text = "_1.";
            SourceFile2Prefix.Text = "_2.";

            LoadSetting();
        }

        private void Merget(String TargetFile, String SourceFile, String MergeFile, int TrimHeight)
        {
            Bitmap BmpSource1 = new Bitmap(SourceFile);
            Bitmap BmpSource2 = new Bitmap(MergeFile);
            Bitmap BmpTarget1 = new Bitmap(int.Parse(PictWidth.Text), int.Parse(PictHeight.Text));

            // 切り取る元のTemp画像
            Bitmap TargetUpper = new Bitmap(BmpSource1);
            Bitmap TargetLower = new Bitmap(BmpSource2);

            // 切り取る部分（上）
            int HeightLower = int.Parse(PictHeight.Text) - TrimHeight;
            System.Drawing.Rectangle srcRectUpper = new System.Drawing.Rectangle(0, 0, int.Parse(PictWidth.Text), TrimHeight);
            System.Drawing.Rectangle srcRectLower = new System.Drawing.Rectangle(0, TrimHeight, int.Parse(PictWidth.Text), HeightLower);

            // 描画する部分
            System.Drawing.Rectangle desRectUpper = new System.Drawing.Rectangle(0, 0, int.Parse(PictWidth.Text), TrimHeight);
            System.Drawing.Rectangle desRectLower = new System.Drawing.Rectangle(0, 0, int.Parse(PictWidth.Text), HeightLower);

            // 描画（上）
            using (Graphics g = Graphics.FromImage(TargetUpper))
            {
                //画像の一部を描画する
                g.DrawImage(BmpSource1, desRectUpper, srcRectUpper, GraphicsUnit.Pixel);

                //Graphicsオブジェクトのリソースを解放する
                g.Dispose();
            }
            BmpSource1.Dispose();

            // 描画（下）
            using (Graphics g = Graphics.FromImage(TargetLower))
            {
                //画像の一部を描画する
                g.DrawImage(BmpSource2, desRectLower, srcRectLower, GraphicsUnit.Pixel);

                //Graphicsオブジェクトのリソースを解放する
                g.Dispose();
            }
            BmpSource2.Dispose();

            using (Graphics g = Graphics.FromImage(BmpTarget1))
            {
                g.DrawImage(TargetUpper, 0, 0);
                g.DrawImage(TargetLower, 0, TrimHeight);

                //Graphicsオブジェクトのリソースを解放する
                g.Dispose();
            }
            BmpTarget1.Save(TargetFile);
            BmpTarget1.Dispose();
            TargetUpper.Dispose();
            TargetLower.Dispose();
        }

        private void ListUp_Click(object sender, RoutedEventArgs e)
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

        private void MergeExec_Click(object sender, RoutedEventArgs e)
        {
            // オリジナル
            Merge_Org();
        }

        private void Merge_Org()
        {
            // 切断基準となる高さ
            string[] TrimHeight = TrimingHeight.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < ListBox_ListUp.SelectedItems.Count; i++)
            {
                for (int j = 0; j < TrimHeight.Length; j++)
                {
                    //　オリジナルファイル＆バックアップファイル
                    String SourceFilePath = SourceFolderPath.Text + @"\" + ListBox_ListUp.SelectedItems[i].ToString();
                    String BackUpSourceFilePath = SourceFolderPath.Text + @"\" + @"org" + @"\" + ListBox_ListUp.SelectedItems[i].ToString();

                    Directory.CreateDirectory(SourceFolderPath.Text + @"\" + @"org");

                    // 文字列が部分一致したら処理
                    if (SourceFilePath.IndexOf(SourceFile1Prefix.Text.ToString()) != -1)
                    {
                        //　マージファイル＆バックアップファイル
                        String MergeFilePath = SourceFilePath.Replace(SourceFile1Prefix.Text, SourceFile2Prefix.Text);
                        String BackUpMergeFilePath = BackUpSourceFilePath.Replace(SourceFile1Prefix.Text, SourceFile2Prefix.Text);

                        // 元ファイルをバックアップ

                        File.Copy(SourceFilePath, BackUpSourceFilePath, true);
                        File.Copy(MergeFilePath, BackUpMergeFilePath, true);

                        if (TrimHeight[j].Equals(""))
                        {
                            continue;
                        }

                        // TODO：入れ子にするための暫定
                        // マージ実行
                        if (j % 2 == 0)
                        {
                            Merget(SourceFilePath, BackUpSourceFilePath, BackUpMergeFilePath, int.Parse(TrimHeight[j].ToString()));
                            Merget(MergeFilePath, BackUpMergeFilePath, BackUpSourceFilePath, int.Parse(TrimHeight[j].ToString()));
                        }
                        else
                        {
                            Merget(SourceFilePath, BackUpMergeFilePath, BackUpSourceFilePath, int.Parse(TrimHeight[j].ToString()));
                            Merget(MergeFilePath, BackUpSourceFilePath, BackUpMergeFilePath, int.Parse(TrimHeight[j].ToString()));
                        }
                    }
                }
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
            element.SetAttribute("attribute", "SourceFile1Prefix");
            element.InnerText = SourceFile1Prefix.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "SourceFile2Prefix");
            element.InnerText = SourceFile2Prefix.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "PictWidth");
            element.InnerText = PictWidth.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "PictHeight");
            element.InnerText = PictHeight.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "TrimingHeight");
            element.InnerText = TrimingHeight.Text;
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
                else if (attribute.Equals("SourceFile1Prefix"))
                {
                    SourceFile1Prefix.Text = text;
                }
                else if (attribute.Equals("SourceFile2Prefix"))
                {
                    SourceFile2Prefix.Text = text;
                }
                else if (attribute.Equals("PictWidth"))
                {
                    PictWidth.Text = text;
                }
                else if (attribute.Equals("PictHeight"))
                {
                    PictHeight.Text = text;
                }
                else if (attribute.Equals("TrimingHeight"))
                {
                    TrimingHeight.Text = text;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //// Trim
            //Trim trm = new Trim();
            //trm.PointX = 0;
            //trm.PointY = 0;
            //trm.PictHeight = int.Parse(PictHeight.Text);
            //trm.PictWidth = int.Parse(PictWidth.Text);
            //trm.SourcePictName = @"D:\tmp\cheetos\Test3\Color\Sample_1.png";
            //trm.TargetPictName = @"D:\tmp\cheetos\Test3\Color\Sample_1_cut.png";
            //trm.Triming(50, 50, 100, 100);
        }
    }
}
