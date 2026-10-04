using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace ImageViewer
{
    public class Preview
    {
        private int _width;
        private int _height;

        public void SetSize(int width)
        {
            SetSize(width, width);
        }

        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void View(ImageList imageList, ListView listView, String folderPath, String extension, bool isSample = false)
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("無効なパスです。" + Environment.NewLine + folderPath);
                return;
            }

            String[] files = Directory.GetFiles(folderPath, extension);
            imageList.ImageSize = new Size(_width, _height);

            // 前回表示した画像が残っていると、項目のImageIndex(0始まり)が古い画像を指してしまうので消しておく
            imageList.Images.Clear();
            listView.Clear();
            listView.LargeImageList = imageList;

            // サンプルのときは、1枚だけ表示(該当ファイルが0件のときに範囲外アクセスしないようMinを取る)
            int maxNum = isSample ? Math.Min(1, files.Length) : files.Length;

            for (int i = 0; i < maxNum; i++)
            {
                using (Image original = Image.FromFile(files[i]))
                {
                    imageList.Images.Add(original);
                }
                listView.Items.Add(files[i], i);
            }
        }
    }
}
