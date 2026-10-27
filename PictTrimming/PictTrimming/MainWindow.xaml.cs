using System;
using System.Collections.Generic;
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
            if (ListBox_ListUp.SelectedItems.Count == 0)
            {
                return;
            }

            int baseX;
            int baseY;
            int targetX;
            int targetY;
            if (!int.TryParse(BaseX.Text, out baseX) || !int.TryParse(BaseY.Text, out baseY) ||
                !int.TryParse(TargetX.Text, out targetX) || !int.TryParse(TargetY.Text, out targetY))
            {
                MessageBox.Show("座標は整数で入力してください");
                return;
            }

            int targetWidth;
            int targetHeight;
            if (Radio_SelectPointOfEnd.IsChecked.Value)
            {
                targetWidth = targetX - baseX;
                targetHeight = targetY - baseY;
            }
            else // if ( Radio_SelectSizeOfEnd.IsChecked.Value )
            {
                targetWidth = targetX;
                targetHeight = targetY;
            }

            // 幅・高さが0以下だとBitmapの作成で例外になるため、先に弾く
            if (targetWidth <= 0 || targetHeight <= 0)
            {
                MessageBox.Show("切り取る範囲の幅と高さが0以下です");
                return;
            }

            // バックアップ先(org)が無いとFile.Copyが失敗するため、先に作っておく
            String backupFolderPath = SourceFolderPath.Text + @"\" + @"org";
            Directory.CreateDirectory(backupFolderPath);

            List<String> failedFiles = new List<String>();
            for (int i = 0; i < ListBox_ListUp.SelectedItems.Count; i++)
            {
                String fileName = ListBox_ListUp.SelectedItems[i].ToString();
                String filePath = SourceFolderPath.Text + @"\" + fileName;
                String backupFilePath = backupFolderPath + @"\" + fileName;

                try
                {
                    // オリジナルファイルをバックアップ。既にバックアップがある時は上書きしない
                    // (2回目以降に上書きすると、トリミング済みの画像がオリジナルとして残ってしまう)。
                    // トリミングは常にバックアップ(=オリジナル)を元に行う
                    if (!File.Exists(backupFilePath))
                    {
                        File.Copy(filePath, backupFilePath);
                    }

                    // トリミング
                    Logic.Trim(filePath, backupFilePath, baseX, baseY, targetWidth, targetHeight);
                }
                catch (Exception ex)
                {
                    // 画像でないファイルなどで1件失敗しても、残りのファイルは処理を続ける
                    failedFiles.Add(fileName + " (" + ex.Message + ")");
                }
            }

            if (failedFiles.Count > 0)
            {
                MessageBox.Show("次のファイルは処理できませんでした\n" + String.Join("\n", failedFiles));
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
