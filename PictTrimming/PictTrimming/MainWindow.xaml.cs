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
        private const String _settingFileName = @"PictTrim.json";

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

            if (ListBox_ListUp.SelectedItems.Count == 0)
            {
                return;
            }

            // 原点はファイルごとに変わらないので、ループの前に1回だけ読み取る
            int baseX = int.Parse(BaseX.Text);
            int baseY = int.Parse(BaseY.Text);

            // バックアップ先(org)が無いとFile.Copyが失敗するため、先に作っておく
            String backupFolderPath = SourceFolderPath.Text + @"\" + @"org";
            Directory.CreateDirectory(backupFolderPath);

            for (int i = 0; i < ListBox_ListUp.SelectedItems.Count; i++)
            {
                String fileName = ListBox_ListUp.SelectedItems[i].ToString();
                String filePath = SourceFolderPath.Text + @"\" + fileName;
                String backupFilePath = backupFolderPath + @"\" + fileName;

                // オリジナルファイルをバックアップ
                File.Copy(filePath, backupFilePath, true);

                // トリミング
                Logic.Trim(filePath, backupFilePath, baseX, baseY, targetWidth, targetHeight);
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
            Logic.SaveSetting(_settingFileName, SourceFolderPath.Text, BaseX.Text, BaseY.Text, TargetX.Text, TargetY.Text);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            Logic.Settings settings = Logic.LoadSetting(_settingFileName);
            if (settings == null)
            {
                return;
            }

            // 保存されていない項目(null)は今の値のまま
            SourceFolderPath.Text = settings.SourceFolderPath ?? SourceFolderPath.Text;
            BaseX.Text = settings.BaseX ?? BaseX.Text;
            BaseY.Text = settings.BaseY ?? BaseY.Text;
            TargetX.Text = settings.TargetX ?? TargetX.Text;
            TargetY.Text = settings.TargetY ?? TargetY.Text;
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
