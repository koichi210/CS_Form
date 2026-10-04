using System;
using System.Drawing;

namespace ResizeImg
{
    /// <summary>
    /// もともと Form1.cs の Triming / PreView に実装されていた、画像の切り取り/
    /// プレビュー生成ロジックをテストできる形に切り出したもの。textBoxのコントロール参照は、呼び出し元
    /// (Form1)で読み取った値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static void TrimImage(string fileName, int baseX, int baseY, int width, int height)
        {
            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(baseX, baseY, width, height);

            //描画する部分の範囲を決定
            Rectangle destRect = new Rectangle(0, 0, width, height);

            //描画先とするImageオブジェクトと、画像ファイルのImageオブジェクトを作成
            // (Disposeしないと元画像ファイルがロックされたままになる)
            using (Bitmap canvas = new Bitmap(width, height))
            using (Bitmap img = new Bitmap(fileName))
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);
                }

                string targetName = fileName.Replace(".", "_new.");
                canvas.Save(targetName);
            }
        }

        public static void CreatePreviewImages(
            string fileName,
            int baseX, int baseY, int width, int height,
            int sampleWidth, int sampleHeight,
            int originalWidth, int originalHeight,
            int originalSizeCandidate1, int originalSizeCandidate2,
            out Bitmap sampleImage, out Bitmap originalImage)
        {
            //描画先とするImageオブジェクトを作成
            Bitmap sampleCanvas = new Bitmap(sampleWidth, sampleHeight);
            Bitmap originalCanvas = new Bitmap(originalWidth, originalHeight);

            int originalPictSize = Math.Max(originalSizeCandidate1, originalSizeCandidate2);

            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(baseX, baseY, width, height);
            Rectangle srcRectOriginal = new Rectangle(0, 0, originalPictSize, originalPictSize);

            //描画する部分の範囲を決定
            Rectangle destRect = new Rectangle(0, 0, sampleWidth, sampleHeight);
            Rectangle destRectOriginal = new Rectangle(0, 0, originalWidth, originalHeight);

            //画像ファイルのImageオブジェクトを作成(Disposeしないと元画像ファイルがロックされたままになる)
            using (Bitmap img = new Bitmap(fileName))
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成
                using (Graphics g = Graphics.FromImage(sampleCanvas))
                {
                    //画像の一部を描画する
                    g.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);
                }

                using (Graphics g = Graphics.FromImage(originalCanvas))
                {
                    // オリジナル画像を描画
                    g.DrawImage(img, destRectOriginal, srcRectOriginal, GraphicsUnit.Pixel);
                }
            }

            sampleImage = sampleCanvas;
            originalImage = originalCanvas;
        }
    }
}
