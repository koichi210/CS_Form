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
                Logic.Triming(FilePath, BackUpFilePath, int.Parse(BaseX.Text), int.Parse(BaseY.Text), Target_Width, Target_Height);
            }

            //ListupExecute();
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
            Logic.SaveSettingXml(SaveXmlFile, SourceFolderPath.Text, BaseX.Text, BaseY.Text, TargetX.Text, TargetY.Text);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            Logic.Settings settings = Logic.LoadSettingXml(SaveXmlFile);
            if (settings == null)
            {
                return;
            }

            if (settings.SourceFolderPath != null)
            {
                SourceFolderPath.Text = settings.SourceFolderPath;
            }
            if (settings.BaseX != null)
            {
                BaseX.Text = settings.BaseX;
            }
            if (settings.BaseY != null)
            {
                BaseY.Text = settings.BaseY;
            }
            if (settings.TargetX != null)
            {
                TargetX.Text = settings.TargetX;
            }
            if (settings.TargetY != null)
            {
                TargetY.Text = settings.TargetY;
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
