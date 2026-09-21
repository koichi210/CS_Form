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

using PicEdit;

namespace PictMerge
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly String SettingFile = @"PictMerge.json";

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
            // 入力値の変換は画像を開く前に行う(途中で失敗しても画像ファイルがロックされたまま残らないように)
            int Width = int.Parse(PictWidth.Text);
            int Height = int.Parse(PictHeight.Text);

            // 切り取る部分（上）
            int HeightLower = Height - TrimHeight;
            System.Drawing.Rectangle srcRectUpper = new System.Drawing.Rectangle(0, 0, Width, TrimHeight);
            System.Drawing.Rectangle srcRectLower = new System.Drawing.Rectangle(0, TrimHeight, Width, HeightLower);

            // 描画する部分
            System.Drawing.Rectangle desRectUpper = new System.Drawing.Rectangle(0, 0, Width, TrimHeight);
            System.Drawing.Rectangle desRectLower = new System.Drawing.Rectangle(0, 0, Width, HeightLower);

            using (Bitmap BmpTarget1 = new Bitmap(Width, Height))
            {
                using (Bitmap BmpSource1 = new Bitmap(SourceFile))
                using (Bitmap BmpSource2 = new Bitmap(MergeFile))
                using (Bitmap TargetUpper = new Bitmap(BmpSource1))
                using (Bitmap TargetLower = new Bitmap(BmpSource2))
                {
                    // 描画（上）
                    using (Graphics g = Graphics.FromImage(TargetUpper))
                    {
                        g.DrawImage(BmpSource1, desRectUpper, srcRectUpper, GraphicsUnit.Pixel);
                    }

                    // 描画（下）
                    using (Graphics g = Graphics.FromImage(TargetLower))
                    {
                        g.DrawImage(BmpSource2, desRectLower, srcRectLower, GraphicsUnit.Pixel);
                    }

                    using (Graphics g = Graphics.FromImage(BmpTarget1))
                    {
                        g.DrawImage(TargetUpper, 0, 0);
                        g.DrawImage(TargetLower, 0, TrimHeight);
                    }
                }
                BmpTarget1.Save(TargetFile);
            }
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

        // XMLの組み立て/読み取りは同じ形式を手書きしていた4プロジェクトで共通だったため
        // [[_Common/SimpleSettings.cs]]へ集約した。ファイル形式は従来と同じ
        private void SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            StandardTemplate.StcSimpleSettings settings = new StandardTemplate.StcSimpleSettings();
            settings.Set("SourceFolderPath", SourceFolderPath.Text);
            settings.Set("SourceFile1Prefix", SourceFile1Prefix.Text);
            settings.Set("SourceFile2Prefix", SourceFile2Prefix.Text);
            settings.Set("PictWidth", PictWidth.Text);
            settings.Set("PictHeight", PictHeight.Text);
            settings.Set("TrimingHeight", TrimingHeight.Text);
            settings.SaveJson(SettingFile);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            StandardTemplate.StcSimpleSettings settings = StandardTemplate.StcSimpleSettings.LoadWithMigration(SettingFile);
            if (settings == null)
            {
                // 設定ファイルが無い/壊れている場合は初期値のまま進める
                return;
            }

            SourceFolderPath.Text = settings.Get("SourceFolderPath", SourceFolderPath.Text);
            SourceFile1Prefix.Text = settings.Get("SourceFile1Prefix", SourceFile1Prefix.Text);
            SourceFile2Prefix.Text = settings.Get("SourceFile2Prefix", SourceFile2Prefix.Text);
            PictWidth.Text = settings.Get("PictWidth", PictWidth.Text);
            PictHeight.Text = settings.Get("PictHeight", PictHeight.Text);
            TrimingHeight.Text = settings.Get("TrimingHeight", TrimingHeight.Text);
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
