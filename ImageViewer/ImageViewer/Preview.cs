using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace ImageViewer
{
    public class Preview
    {
        private int _width;
        private int _height;

        // 一覧の並び順(既定は名前順。名前順はエクスプローラーと同じく数字を数として比べる。[[FileSorter.cs]])
        public FileSortOrder SortOrder { get; set; } = FileSortOrder.Name;
        public bool SortDescending { get; set; }

        // 拡張子のパターンを複数書くときの区切り(例: "*.png;*.mp4")
        private static readonly char[] _patternSeparators = { ';' };

        // 動画として扱う拡張子。Image.FromFileでは読めないので、Windowsのシェルが作る
        // サムネイル(先頭付近のフレーム)を表示する([[ShellThumbnail.cs]])。
        // TypeScriptと被る.tsは誤判定を避けるため入れていない
        private static readonly HashSet<String> _videoExtensions = new HashSet<String>(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4", ".m4v", ".mov", ".avi", ".wmv", ".asf", ".mkv", ".webm",
            ".mpg", ".mpeg", ".m2ts", ".mts", ".flv", ".3gp", ".3g2",
        };

        public void SetSize(int width)
        {
            SetSize(width, width);
        }

        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        // 動画ファイル(シェルのサムネイルで表示する対象)か
        public static Boolean IsVideoFile(String filePath)
        {
            return !String.IsNullOrEmpty(filePath) && _videoExtensions.Contains(Path.GetExtension(filePath));
        }

        // 表示対象のファイルを集める。パターンは";"区切りで複数書ける(例: "*.png;*.mp4")。
        // 1つだけのときは従来どおりDirectory.GetFilesの順のまま返し、複数のときは
        // 重複を除いてファイル名順に並べ直す(パターンごとに固まらず、画像と動画が名前順に混ざる)
        public static String[] GetTargetFiles(String folderPath, String extension)
        {
            String[] patterns = (extension ?? String.Empty)
                .Split(_patternSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(pattern => pattern.Trim())
                .Where(pattern => pattern.Length != 0)
                .ToArray();

            if (patterns.Length <= 1)
            {
                return Directory.GetFiles(folderPath, patterns.Length == 1 ? patterns[0] : (extension ?? String.Empty));
            }

            return patterns
                .SelectMany(pattern => Directory.GetFiles(folderPath, pattern))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        // 表示対象のファイルを集めて、SortOrder/SortDescendingの並び順にする
        public String[] GetSortedTargetFiles(String folderPath, String extension)
        {
            return FileSorter.Sort(GetTargetFiles(folderPath, extension), SortOrder, SortDescending);
        }

        // 一覧に入れるサムネイル(SetSizeの大きさに縮めた画像)を作る。読めないファイル・サムネイルが取れない動画はnull。
        // 画面に触らないので、読み込み用のスレッドから呼んでよい(元の画像はすぐ閉じるので、ファイルを握ったままにしない)
        public Image LoadThumbnail(String filePath)
        {
            try
            {
                using (Image original = LoadSourceImage(filePath))
                {
                    if (original == null)
                    {
                        return null;
                    }
                    // ImageListに入れるとこの大きさに引き伸ばされるので、同じ大きさにしておく(大きな画像を持ち回らない)
                    return new Bitmap(original, Math.Max(1, _width), Math.Max(1, _height));
                }
            }
            catch (Exception ex) when (ex is OutOfMemoryException || ex is IOException
                || ex is UnauthorizedAccessException || ex is ArgumentException)
            {
                // Image.FromFileは画像として読めないファイルにOutOfMemoryExceptionを投げる
                return null;
            }
        }

        // サムネイルの元になる画像を読む。動画はシェルのサムネイルで代用し、取れなければnull
        private Image LoadSourceImage(String filePath)
        {
            if (IsVideoFile(filePath))
            {
                return ShellThumbnail.GetThumbnail(filePath, _width, _height);
            }
            return Image.FromFile(filePath);
        }

        public void View(ImageList imageList, ListView listView, String folderPath, String extension, bool isSample = false)
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("無効なパスです。" + Environment.NewLine + folderPath);
                return;
            }

            String[] files = GetSortedTargetFiles(folderPath, extension);
            imageList.ImageSize = new Size(_width, _height);

            // 前回表示した画像が残っていると、項目のImageIndex(0始まり)が古い画像を指してしまうので消しておく
            imageList.Images.Clear();
            listView.Clear();
            listView.LargeImageList = imageList;

            // サンプルのときは、1枚だけ表示(該当ファイルが0件のときに範囲外アクセスしないようMinを取る)
            int maxNum = isSample ? Math.Min(1, files.Length) : files.Length;

            for (int i = 0; i < maxNum; i++)
            {
                // 壊れた画像・読めないファイル(一覧作成後に消された・ロック中など)が1枚でもあると、
                // Image.FromFileの例外でそこから先が表示されなかった。読めない画像はサムネイル無し(ImageIndex=-1)で並べる
                // (ImageListに追加できた枚数とファイルの番号がずれるので、ImageIndexは追加した位置を使う)。
                // 動画でサムネイルが取れなかったとき(コーデックが無い等)も同じくサムネイル無しで並べる
                int imageIndex = -1;
                try
                {
                    using (Image original = LoadSourceImage(files[i]))
                    {
                        if (original != null)
                        {
                            imageList.Images.Add(original);
                            imageIndex = imageList.Images.Count - 1;
                        }
                    }
                }
                catch (Exception ex) when (ex is OutOfMemoryException || ex is IOException
                    || ex is UnauthorizedAccessException || ex is ArgumentException)
                {
                    // Image.FromFileは画像として読めないファイルにOutOfMemoryExceptionを投げる
                }
                listView.Items.Add(files[i], imageIndex);
            }
        }
    }
}
