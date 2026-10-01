using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace ImageViewer
{
    public class PreView
    {
        private int m_Width;
        private int m_Height;

        public void SetSize(int width)
        {
            m_Width = width;
            m_Height = width;
        }

        public void SetSize(int width, int height)
        {
            m_Width = width;
            m_Height = height;
        }

        public void View(ImageList imageList, ListView listView, String folderPath, String extension, bool isSample = false)
        {
            if (Directory.Exists(folderPath) == false)
            {
                MessageBox.Show("無効なパスです。" + Environment.NewLine + folderPath);
                return;
            }

            String[] files = Directory.GetFiles(folderPath, extension);
            imageList.ImageSize = new Size(m_Width, m_Height);

            listView.Clear();
            listView.LargeImageList = imageList;

            // サンプルのときは、1枚だけ表示
            int maxNum = isSample ? 1 : files.Length;

            for (int i = 0; i < maxNum; i++)
            {
                Image original = Bitmap.FromFile(files[i]);

                imageList.Images.Add(original);
                listView.Items.Add(files[i], i);

                original.Dispose();
            }
        }
    }
}
