using System;
using System.Drawing;

namespace PicEdit
{
    class Trim
    {
        // 描画先
        protected Bitmap m_Canvas;
        protected Bitmap m_SourceImg;

        public Trim(String basePictFile)
        {
            //既存ファイルをもとに、描画先Imageオブジェクトを作成
            m_Canvas = new Bitmap(basePictFile);
        }

        public Trim(int destWidth, int destHeight)
        {
            //新規に描画先Imageオブジェクトを作成
            m_Canvas = new Bitmap(destWidth, destHeight);
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

        public void SaveCanvas(String savePictFile)
        {
            m_Canvas.Save(savePictFile);
        }

        public void TrimExec(String basePictFile, Rectangle cutParam, Point putParam)
        {
            //描画する部分の範囲を設定。位置(X, Y)、大きさ(Width, Height)
            Rectangle pasteRect = new Rectangle(putParam.X, putParam.Y, cutParam.Width, cutParam.Height);

            //画像ファイルのImageオブジェクトを作成
            using (Bitmap img = new Bitmap(basePictFile))
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成
                using (Graphics g = Graphics.FromImage(m_Canvas))
                {
                    //画像の一部を描画
                    g.DrawImage(img, pasteRect, cutParam, GraphicsUnit.Pixel);
                }
            }
        }

        public void CreateSourceImg(String sourceImgFile)
        {
            //加工元ファイルのImageオブジェクトを作成
            m_SourceImg = new Bitmap(sourceImgFile);
        }

        public void ReleaseSourceImg()
        {
            // CreateSourceImg未実行ならm_SourceImgはnullのまま(ファイナライザ側と同じ理由)
            if (m_SourceImg != null)
            {
                m_SourceImg.Dispose();
            }
        }

        public void MergeExec(Rectangle cutParam)
        {
            MergeExec(cutParam, new Point(cutParam.X, cutParam.Y));
        }

        public void MergeExec(Rectangle cutParam, Point putParam)
        {
            //描画する部分の範囲を設定。位置(X, Y)、大きさ(Width, Height)
            Rectangle pasteRect = new Rectangle(putParam.X, putParam.Y, cutParam.Width, cutParam.Height);

            //画像ファイルのImageオブジェクトを作成
            using (Bitmap img = new Bitmap(m_SourceImg))
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成
                using (Graphics g = Graphics.FromImage(m_Canvas))
                {
                    //画像の一部を描画
                    g.DrawImage(img, pasteRect, cutParam, GraphicsUnit.Pixel);
                }
            }
        }
    }
}
