using System;
using System.Drawing;

namespace Picture
{
    class PicEdit : IDisposable
    {
        // 描画先
        protected Bitmap _canvas;
        protected Bitmap _sourceImg;

        public PicEdit(String basePictFile)
        {
            //既存ファイルをもとに、描画先Imageオブジェクトを作成
            _canvas = new Bitmap(basePictFile);
        }

        public PicEdit(int destWidth, int destHeight)
        {
            //新規に描画先Imageオブジェクトを作成
            _canvas = new Bitmap(destWidth, destHeight);
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
            ReleaseImg(ref _canvas);
            ReleaseImg(ref _sourceImg);
        }

        public void SaveCanvas(String savePictFile)
        {
            _canvas.Save(savePictFile);
        }

        public void TrimExec(String basePictFile, Rectangle cutParam, Point putParam)
        {
            //画像ファイルのImageオブジェクトを作成
            using (Bitmap img = new Bitmap(basePictFile))
            {
                DrawToCanvas(img, cutParam, putParam);
            }
        }

        public void CreateSourceImg(String sourceImgFile)
        {
            //加工元ファイルのImageオブジェクトを作成。
            // 以前はMergeExecのたびに加工元の複製(new Bitmap(_sourceImg))を作り直していたが、
            // 加工元は変更されないので複製は毎回同じ内容になる。ここで1回だけ複製して使い回す
            // (描画元は従来と同じ「複製」なので出力は変わらない。ファイルのロックも早く外れる)
            ReleaseImg(ref _sourceImg);
            using (Bitmap loaded = new Bitmap(sourceImgFile))
            {
                _sourceImg = new Bitmap(loaded);
            }
        }

        public void ReleaseSourceImg()
        {
            // CreateSourceImg未実行ならm_SourceImgはnullのまま(ReleaseImg側でnullチェックする)
            ReleaseImg(ref _sourceImg);
        }

        public void MergeExec(Rectangle cutParam)
        {
            MergeExec(cutParam, new Point(cutParam.X, cutParam.Y));
        }

        public void MergeExec(Rectangle cutParam, Point putParam)
        {
            //加工元画像の複製(CreateSourceImgで作成済み)から描画する
            DrawToCanvas(_sourceImg, cutParam, putParam);
        }

        // 画像のcutParamの範囲を、キャンバスのputParamの位置へそのままの大きさで描画する(TrimExec/MergeExecで共通)
        private void DrawToCanvas(Image sourceImg, Rectangle cutParam, Point putParam)
        {
            //描画する部分の範囲を設定。位置(X, Y)、大きさ(Width, Height)
            Rectangle pasteRect = new Rectangle(putParam.X, putParam.Y, cutParam.Width, cutParam.Height);

            //ImageオブジェクトのGraphicsオブジェクトを作成
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                //画像の一部を描画
                g.DrawImage(sourceImg, pasteRect, cutParam, GraphicsUnit.Pixel);
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
