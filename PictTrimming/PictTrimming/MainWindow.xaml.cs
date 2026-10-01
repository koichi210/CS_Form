using System;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace PictTrimming
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly String SettingFileName = @"PictTrim.json";

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
            int targetWidth;
            int targetHeight;

            if (Radio_SelectPointOfEnd.IsChecked.Value)
            {
                targetWidth = int.Parse(TargetX.Text) - int.Parse(BaseX.Text);
                targetHeight = int.Parse(TargetY.Text) - int.Parse(BaseY.Text);
            }
            else // if ( Radio_SelectSizeOfEnd.IsChecked.Value )
            {
                targetWidth = int.Parse(TargetX.Text);
                targetHeight = int.Parse(TargetY.Text);
            }

            for (int i = 0; i < ListBox_ListUp.SelectedItems.Count; i++)
            {
                String filePath         = SourceFolderPath.Text + @"\" + ListBox_ListUp.SelectedItems[i].ToString();
                String backupFilePath   = SourceFolderPath.Text + @"\" + @"org" + @"\" + ListBox_ListUp.SelectedItems[i].ToString();

                // オリジナルファイルをバックアップ
                File.Copy(filePath, backupFilePath, true);

                // トリミング
                Logic.Trim(filePath, backupFilePath, int.Parse(BaseX.Text), int.Parse(BaseY.Text), targetWidth, targetHeight);
            }
        }

        private void Button_Listup_Click(object sender, RoutedEventArgs e)
        {
            ListUpFiles();
        }

        private void ListUpFiles()
        {
            if (SourceFolderPath.Text.Equals(""))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }
            ListBox_ListUp.Items.Clear();

            string[] files = Directory.GetFiles(SourceFolderPath.Text, "*", SearchOption.TopDirectoryOnly);

            //配列の内容を一つ一つ追加する
            foreach (string file in files)
            {
                ListBox_ListUp.Items.Add(Path.GetFileName(file));
            }
        }

        private void SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            Logic.SaveSetting(SettingFileName, SourceFolderPath.Text, BaseX.Text, BaseY.Text, TargetX.Text, TargetY.Text);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            Logic.Settings settings = Logic.LoadSetting(SettingFileName);
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
                ListUpFiles();
            }
        }
    }
}
