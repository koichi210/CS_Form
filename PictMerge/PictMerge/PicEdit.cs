using System;
using System.Drawing;

namespace Picture
{
    class PicEdit : IDisposable
    {
        // 描画先
        protected Bitmap m_Canvas;
        protected Bitmap m_SourceImg;

        public PicEdit(String basePictFile)
        {
            //既存ファイルをもとに、描画先Imageオブジェクトを作成
            m_Canvas = new Bitmap(basePictFile);
        }

        public PicEdit(int destWidth, int destHeight)
        {
            //新規に描画先Imageオブジェクトを作成
            m_Canvas = new Bitmap(destWidth, destHeight);
        }

        ~PicEdit()
        {
            Dispose(false);
        }

        // IDisposable。using で囲めばファイナライザを待たずにその場で画像を解放できる。
        // ReleaseImg が解放後に null を入れるので、二重に呼んでも安全。
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // リソース解放(ファイナライザから呼ばれた場合も従来どおり両方の画像を解放する)
            ReleaseImg(ref m_Canvas);
            ReleaseImg(ref m_SourceImg);
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
            // CreateSourceImg未実行ならm_SourceImgはnullのまま(ReleaseImg側でnullチェックする)
            ReleaseImg(ref m_SourceImg);
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

        private void ReleaseImg(ref Bitmap img)
        {
            if (img != null)
            {
                img.Dispose();
                img = null;
            }
        }
    }
}
