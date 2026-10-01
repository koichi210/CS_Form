using System;
using System.Drawing;

namespace Picture
{
    class PicEdit
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
            // リソース解放
            // m_SourceImgはCreateSourceImg()を呼ぶまでnullのまま(MergeExec系を
            // 使わないインスタンスだと一度も設定されない)。以前は無条件にDispose()
            // していたため、その場合ファイナライザ内でNullReferenceExceptionが
            // 起きるバグだった(TODO「必ず走るけどok？」が指摘していた通り)。
            m_Canvas.Dispose();
            if (m_SourceImg != null)
            {
                m_SourceImg.Dispose();
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
            // 上のファイナライザと同じ理由でnullチェックを追加(CreateSourceImg未実行なら
            // m_SourceImgはnullのまま)
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

    class PicEditCustom : PicEdit
    {
        /// ///////////////////////////////////////////////
        /// sample Start
        private int m_DestWidth = 0;

        public int DestWidth
        {
            get
            {
                return m_DestWidth;
            }
            set
            {
                m_DestWidth = value;
            }
        }

        /// sample End
        /// ///////////////////////////////////////////////

        public PicEditCustom(String basePictFile)
            : base(basePictFile)
        {
        }

        public PicEditCustom(int destWidth, int destHeight) : base(destWidth, destHeight)
        {
        }
    }
}
