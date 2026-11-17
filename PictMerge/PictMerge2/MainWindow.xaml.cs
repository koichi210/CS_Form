using System;
using System.Collections.Generic;
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
            DestWidth.Text = "200";
            DestHeight.Text = "200";
            TrimmingHeight.Text = "50,100" + Environment.NewLine + "150,200";
            SourceFolderPath.Text = @"D:\tmp\cheetos\Test3\Color";
#endif
            SourceFile1Prefix.Text = "_1.";
            SourceFile2Prefix.Text = "_2.";

            LoadSetting();
        }

        // 2枚の画像を切断位置で横に区切り、帯ごとに交互に組み合わせた(入れ子にした)画像を作る。
        // 一番上の帯はfirstFile、次の帯はsecondFile、その次はまたfirstFile…の順。
        // trimHeightsは小さい順に並んでいること(ParseTrimHeightsで並べ替え済み)
        private void MergeImage(String targetFile, String firstFile, String secondFile, List<int> trimHeights)
        {
            // 入力値の変換は画像を開く前に行う(途中で失敗しても画像ファイルがロックされたまま残らないように)
            int width = int.Parse(DestWidth.Text);
            int height = int.Parse(DestHeight.Text);

            using (Bitmap bmpTarget = new Bitmap(width, height))
            {
                using (Bitmap bmpFirst = new Bitmap(firstFile))
                using (Bitmap bmpSecond = new Bitmap(secondFile))
                using (Graphics g = Graphics.FromImage(bmpTarget))
                {
                    int top = 0;
                    bool useFirst = true;
                    for (int i = 0; i <= trimHeights.Count; i++)
                    {
                        // 最後の帯は画像の下端まで。画像の外を指す切断位置は端に寄せる
                        int bottom = (i < trimHeights.Count) ? trimHeights[i] : height;
                        bottom = Math.Min(Math.Max(bottom, top), height);

                        if (bottom > top)
                        {
                            // 同じ位置を元画像から同じ大きさのまま写す(解像度(DPI)の違いで拡大縮小されないようピクセル指定)
                            System.Drawing.Rectangle band = new System.Drawing.Rectangle(0, top, width, bottom - top);
                            g.DrawImage(useFirst ? bmpFirst : bmpSecond, band, band, GraphicsUnit.Pixel);
                        }

                        top = bottom;
                        useFirst = !useFirst;
                    }
                }
                bmpTarget.Save(targetFile);
            }
        }

        // 切断基準となる高さの入力(1行に1つ)を数値の一覧にする。空行は無視し、小さい順に並べる
        // (上から順に帯の境目として使うので、入力した行の順番には左右されない)
        private static List<int> ParseTrimHeights(String text)
        {
            List<int> trimHeights = new List<int>();
            foreach (String line in text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                trimHeights.Add(int.Parse(line.Trim()));
            }
            trimHeights.Sort();
            return trimHeights;
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
            // 切断基準となる高さ(入力の変換はファイルを触る前に済ませる)
            List<int> trimHeights = ParseTrimHeights(TrimmingHeight.Text);
            if (trimHeights.Count == 0)
            {
                return;
            }

            for (int i = 0; i < ListBox_ListUp.SelectedItems.Count; i++)
            {
                //　オリジナルファイル＆バックアップファイル
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

                // 元ファイルをバックアップ。結合はこのバックアップ(=結合前の画像)から1回で作るので、
                // orgフォルダには結合前の状態がそのまま残る
                File.Copy(sourceFilePath, backupSourceFilePath, true);
                File.Copy(mergeFilePath, backupMergeFilePath, true);

                // マージ実行。どちらのファイルも一番上の帯は自分のまま、切断位置ごとに相手の画像と交互に入れ替える
                MergeImage(sourceFilePath, backupSourceFilePath, backupMergeFilePath, trimHeights);
                MergeImage(mergeFilePath, backupMergeFilePath, backupSourceFilePath, trimHeights);
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
