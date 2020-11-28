using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PicEdit
{
    class Trim
    {
        // 描画先
        protected Bitmap m_Canvas;
        protected Bitmap m_SourceImg;

        public Trim(String BasePictFile)
        {
            //既存ファイルをもとに、描画先Imageオブジェクトを作成
            m_Canvas = new Bitmap(BasePictFile);
        }

        public Trim(int DestWidth, int DestHeight)
        {
            //新規に描画先Imageオブジェクトを作成
            m_Canvas = new Bitmap(DestWidth, DestHeight);
        }

        ~Trim()
        {
            // リソース解放
            m_Canvas.Dispose();

            if (m_SourceImg != null)
            {
                m_SourceImg.Dispose();
                m_SourceImg = null;
            }
        }

        public void SaveCanvas(String SavePictFile)
        {
            m_Canvas.Save(SavePictFile);
        }

        public void TrimExec(String BasePictFile, Rectangle CutParam, Point PutParam)
        {
            //切り取る部分の範囲を設定。位置(X, Y)、大きさ(Width, Height)
            //Rectangle CutRect = new Rectangle(CutParam.X, CutParam.Y, CutParam.Width, CutParam.Height);

            //描画する部分の範囲を設定。位置(X, Y)、大きさ(Width, Height)
            Rectangle PasteRect = new Rectangle(PutParam.X, PutParam.Y, CutParam.Width, CutParam.Height);

            //画像ファイルのImageオブジェクトを作成
            using (Bitmap img = new Bitmap(BasePictFile))
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成
                using (Graphics g = Graphics.FromImage(m_Canvas))
                {
                    //画像の一部を描画
                    g.DrawImage(img, PasteRect, CutParam, GraphicsUnit.Pixel);

                    //Graphicsオブジェクトのリソースを解放
                    g.Dispose();
                }
                img.Dispose();
            }
        }

        public void CreateSourceImg(String SourceImgFile)
        {
            //加工元ファイルのImageオブジェクトを作成
            m_SourceImg = new Bitmap(SourceImgFile);
        }

        public void ReleaseSourceImg()
        {
            m_SourceImg.Dispose();
        }

        public void MergeExec(Rectangle CutParam)
        {
            MergeExec(CutParam, new Point(CutParam.X, CutParam.Y));
        }

        public void MergeExec(Rectangle CutParam, Point PutParam)
        {
            //描画する部分の範囲を設定。位置(X, Y)、大きさ(Width, Height)
            Rectangle PasteRect = new Rectangle(PutParam.X, PutParam.Y, CutParam.Width, CutParam.Height);

            //画像ファイルのImageオブジェクトを作成
            using (Bitmap img = new Bitmap(m_SourceImg))
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成
                using (Graphics g = Graphics.FromImage(m_Canvas))
                {
                    //画像の一部を描画
                    g.DrawImage(img, PasteRect, CutParam, GraphicsUnit.Pixel);

                    //Graphicsオブジェクトのリソースを解放
                    g.Dispose();
                }
                img.Dispose();
            }
        }
    }
}
