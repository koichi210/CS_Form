using System;
using System.Windows;
using System.Drawing;
using System.IO;

using Picture;

namespace PictMerge
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String _settingFile = @"PictMerge.json";

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG 
            PictWidth.Text = "200";
            PictHeight.Text = "200";
            TrimmingHeight.Text = "50" + Environment.NewLine + "100" + Environment.NewLine + "150";
            SourceFolderPath.Text = @"D:\tmp\cheetos\Test3\Color";
#endif
            SourceFile1Prefix.Text = "_1.";
            SourceFile2Prefix.Text = "_2.";

            LoadSetting();
        }

        private void MergeImage(String targetFile, String sourceFile, String mergeFile, int trimHeight)
        {
            // 入力値の変換は画像を開く前に行う(途中で失敗しても画像ファイルがロックされたまま残らないように)
            int width = int.Parse(PictWidth.Text);
            int height = int.Parse(PictHeight.Text);

            // 切り取る部分（上）
            int lowerHeight = height - trimHeight;
            System.Drawing.Rectangle srcRectUpper = new System.Drawing.Rectangle(0, 0, width, trimHeight);
            System.Drawing.Rectangle srcRectLower = new System.Drawing.Rectangle(0, trimHeight, width, lowerHeight);

            // 描画する部分
            System.Drawing.Rectangle desRectUpper = new System.Drawing.Rectangle(0, 0, width, trimHeight);
            System.Drawing.Rectangle desRectLower = new System.Drawing.Rectangle(0, 0, width, lowerHeight);

            using (Bitmap bmpTarget = new Bitmap(width, height))
            {
                using (Bitmap bmpSource1 = new Bitmap(sourceFile))
                using (Bitmap bmpSource2 = new Bitmap(mergeFile))
                using (Bitmap targetUpper = new Bitmap(bmpSource1))
                using (Bitmap targetLower = new Bitmap(bmpSource2))
                {
                    // 描画（上）
                    using (Graphics g = Graphics.FromImage(targetUpper))
                    {
                        g.DrawImage(bmpSource1, desRectUpper, srcRectUpper, GraphicsUnit.Pixel);
                    }

                    // 描画（下）
                    using (Graphics g = Graphics.FromImage(targetLower))
                    {
                        g.DrawImage(bmpSource2, desRectLower, srcRectLower, GraphicsUnit.Pixel);
                    }

                    using (Graphics g = Graphics.FromImage(bmpTarget))
                    {
                        g.DrawImage(targetUpper, 0, 0);
                        g.DrawImage(targetLower, 0, trimHeight);
                    }
                }
                bmpTarget.Save(targetFile);
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
            foreach (string file in files)
            {
                ListBox_ListUp.Items.Add(System.IO.Path.GetFileName(file));
            }
        }

        private void MergeExec_Click(object sender, RoutedEventArgs e)
        {
            // オリジナル
            MergeSelectedFiles();
        }

        private void MergeSelectedFiles()
        {
            // 切断基準となる高さ
            string[] trimHeights = TrimmingHeight.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < ListBox_ListUp.SelectedItems.Count; i++)
            {
                //　オリジナルファイル＆バックアップファイル(切断位置の行ごとには変わらないので、行のループの外で1回だけ求める)
                String fileName = ListBox_ListUp.SelectedItems[i].ToString();
                String backupFolderPath = SourceFolderPath.Text + @"\" + @"org";
                String sourceFilePath = SourceFolderPath.Text + @"\" + fileName;
                String backupSourceFilePath = backupFolderPath + @"\" + fileName;

                Directory.CreateDirectory(backupFolderPath);

                // 文字列が部分一致しなければ対象外
                if (sourceFilePath.IndexOf(SourceFile1Prefix.Text) == -1)
                {
                    continue;
                }

                //　マージファイル＆バックアップファイル
                String mergeFilePath = sourceFilePath.Replace(SourceFile1Prefix.Text, SourceFile2Prefix.Text);
                String backupMergeFilePath = backupSourceFilePath.Replace(SourceFile1Prefix.Text, SourceFile2Prefix.Text);

                for (int j = 0; j < trimHeights.Length; j++)
                {
                    // 元ファイルをバックアップ(前の行でマージした結果を次の行の元にするため、行ごとに取り直す)
                    File.Copy(sourceFilePath, backupSourceFilePath, true);
                    File.Copy(mergeFilePath, backupMergeFilePath, true);

                    if (trimHeights[j].Equals(""))
                    {
                        continue;
                    }

                    int trimHeight = int.Parse(trimHeights[j]);

                    // TODO：入れ子にするための暫定
                    // マージ実行
                    if (j % 2 == 0)
                    {
                        MergeImage(sourceFilePath, backupSourceFilePath, backupMergeFilePath, trimHeight);
                        MergeImage(mergeFilePath, backupMergeFilePath, backupSourceFilePath, trimHeight);
                    }
                    else
                    {
                        MergeImage(sourceFilePath, backupMergeFilePath, backupSourceFilePath, trimHeight);
                        MergeImage(mergeFilePath, backupSourceFilePath, backupMergeFilePath, trimHeight);
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
            settings.Set("TrimmingHeight", TrimmingHeight.Text);
            settings.SaveJson(_settingFile);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            StandardTemplate.StcSimpleSettings settings = StandardTemplate.StcSimpleSettings.LoadWithMigration(_settingFile);
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
            TrimmingHeight.Text = settings.Get("TrimmingHeight", TrimmingHeight.Text);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
