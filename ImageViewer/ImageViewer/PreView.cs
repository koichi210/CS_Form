using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace ImageViewer
{
    public class PreView
    {
        private StcUtils util = new StcUtils();
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

        public void View(ImageList ImgLst, ListView LstView, String FolderPath, String Extension, bool IsSample = false)
        {
            if (Directory.Exists(FolderPath) == false)
            {
                MessageBox.Show("無効なパスです。" + Environment.NewLine + FolderPath);
                return;
            }

            String[] Files = Directory.GetFiles(FolderPath, Extension);
            ImgLst.ImageSize = new Size(m_Width, m_Height);

            LstView.Clear();
            LstView.LargeImageList = ImgLst;

            // サンプルのときは、1枚だけ表示
            int MaxNum = 1;
            if (IsSample == false)
            {
                MaxNum = Files.Length;
            }

            for (int i = 0; i < MaxNum; i++)
            {
                Image original = Bitmap.FromFile(Files[i]);
                //Image thumbnail = createThumbnail(original, width, height);

                ImgLst.Images.Add(original);
                LstView.Items.Add(Files[i], i);

                original.Dispose();
                //thumbnail.Dispose();
            }
        }

        private Image createThumbnail(Image image, int width, int height)
        {
            Bitmap canvas = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(canvas);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);

            float fw = (float)width / (float)image.Width;
            float fh = (float)height / (float)image.Height;

            float scale = Math.Min(fw, fh);
            fw = image.Width * scale;
            fh = image.Height * scale;

            g.DrawImage(image, (width - fw) / 2, (height - fh) / 2, fw, fh);
            g.Dispose();

            return canvas;
        }
    }
}
