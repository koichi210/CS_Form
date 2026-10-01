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
        readonly String SettingFile = @"PictMerge.json";

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG 
            DestWidth.Text = "200";
            DestHeight.Text = "200";
            TrimmingHeight.Text = "50,100" + Environment.NewLine + "150,200";
            SourceFolderPath.Text = @"D:\tmp\cheetos\Test3\Color";
#endif
            SourceFile1Prefix.Text = "_1.";
            SourceFile2Prefix.Text = "_2.";

            LoadSetting();
        }

        private void MergeImage(String targetFile, String sourceFile, String mergeFile, int trimHeight)
        {
            // 入力値の変換は画像を開く前に行う(途中で失敗しても画像ファイルがロックされたまま残らないように)
            int width = int.Parse(DestWidth.Text);
            int height = int.Parse(DestHeight.Text);

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
                for (int j = 0; j < trimHeights.Length; j++)
                {
                    //　オリジナルファイル＆バックアップファイル
                    String sourceFilePath = SourceFolderPath.Text + @"\" + ListBox_ListUp.SelectedItems[i].ToString();
                    String backupSourceFilePath = SourceFolderPath.Text + @"\" + @"org" + @"\" + ListBox_ListUp.SelectedItems[i].ToString();

                    Directory.CreateDirectory(SourceFolderPath.Text + @"\" + @"org");

                    // 文字列が部分一致したら処理
                    if (sourceFilePath.IndexOf(SourceFile1Prefix.Text) != -1)
                    {
                        //　マージファイル＆バックアップファイル
                        String mergeFilePath = sourceFilePath.Replace(SourceFile1Prefix.Text, SourceFile2Prefix.Text);
                        String backupMergeFilePath = backupSourceFilePath.Replace(SourceFile1Prefix.Text, SourceFile2Prefix.Text);

                        // 元ファイルをバックアップ

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
        }

        // XMLの組み立て/読み取りは同じ形式を手書きしていた4プロジェクトで共通だったため
        // [[_Common/SimpleSettings.cs]]へ集約した。ファイル形式は従来と同じ
        private void SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            StandardTemplate.StcSimpleSettings settings = new StandardTemplate.StcSimpleSettings();
            settings.Set("SourceFolderPath", SourceFolderPath.Text);
            settings.Set("SourceFile1Prefix", SourceFile1Prefix.Text);
            settings.Set("SourceFile2Prefix", SourceFile2Prefix.Text);
            settings.Set("DestWidth", DestWidth.Text);
            settings.Set("DestHeight", DestHeight.Text);
            settings.Set("TrimmingHeight", TrimmingHeight.Text);
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
            DestWidth.Text = settings.Get("DestWidth", DestWidth.Text);
            DestHeight.Text = settings.Get("DestHeight", DestHeight.Text);
            TrimmingHeight.Text = settings.Get("TrimmingHeight", TrimmingHeight.Text);
        }

        private void CutExec_Click(object sender, RoutedEventArgs e)
        {
            String sourcePictName = @"D:\tmp\cheetos\Test3\Color\Sample_1.png";
            String targetPictName = @"D:\tmp\cheetos\Test3\Color\Sample_1_cut.png";

            int destWidth = 200;
            int destHeight = 200;

            int putX = 0;
            int putY = 0;

            int cutX = 50;
            int cutY = 50;
            int cutWidth = 100;
            int cutHeight = 100;

            System.Drawing.Rectangle cutParam = new System.Drawing.Rectangle(cutX, cutY, cutWidth, cutHeight);
            System.Drawing.Point putParam = new System.Drawing.Point(putX, putY);

            // キャンバス作成
            using (PicEdit trimmer = new PicEdit(destWidth, destHeight))
            {
                // 切り取り
                trimmer.TrimExec(sourcePictName, cutParam, putParam);

                // キャンバス保存
                trimmer.SaveCanvas(targetPictName);
            }
        }

        private void Merge_Click(object sender, RoutedEventArgs e)
        {
            String sourcePictName = @"D:\tmp\cheetos\Test3\Color\Sample_1.png";
            String mergePictName = @"D:\tmp\cheetos\Test3\Color\Sample_2.png";
            String targetPictName = @"D:\tmp\cheetos\Test3\Color\Sample_Merge.png";

            int cutX = 0;
            int cutWidth = int.Parse(DestWidth.Text);
            int cutHeight = int.Parse(DestHeight.Text);

            // キャンバス作成
            using (PicEdit merger = new PicEdit(sourcePictName))
            {
                // 切断基準となる高さ（1行に「開始,終了」）
                string[] trimRanges = TrimmingHeight.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // マージ
                merger.CreateSourceImg(mergePictName);

                foreach (string trimRange in trimRanges)
                {
                    string[] heights = trimRange.Split(new[] { "," }, StringSplitOptions.None);
                    int startHeight = int.Parse(heights[0]);
                    int endHeight = int.Parse(heights[1]);

                    System.Drawing.Rectangle cutParam = new System.Drawing.Rectangle(cutX, startHeight, cutWidth, endHeight - startHeight);
                    merger.MergeExec(cutParam);
                }

                merger.ReleaseSourceImg();

                // キャンバス保存
                merger.SaveCanvas(targetPictName);
            }
        }
    }
}
