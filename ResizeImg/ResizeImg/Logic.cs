using System;
using System.Drawing;

namespace ResizeImg
{
    /// <summary>
    /// もともと Form1.cs の Triming / PreView に実装されていた、画像の切り取り/
    /// プレビュー生成ロジックをテストできる形に切り出したもの。コードはそのまま
    /// 移しただけで書き換えていない。textBoxのコントロール参照は、呼び出し元
    /// (Form1)で読み取った値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static void Triming(String FileName, int BaseX, int BaseY, int Width, int Height)
        {
            //描画先とするImageオブジェクトを作成
            Bitmap canvas = new Bitmap(Width, Height);

            //画像ファイルのImageオブジェクトを作成
            Bitmap img = new Bitmap(FileName);

            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(BaseX, BaseY, Width, Height);

            //描画する部分の範囲を決定
            Rectangle desRect = new Rectangle(0, 0, Width, Height);

            //ImageオブジェクトのGraphicsオブジェクトを作成
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.DrawImage(img, desRect, srcRect, GraphicsUnit.Pixel);
                g.Dispose();
            }

            String TargetName = FileName.Replace(".", "_new.");
            canvas.Save(TargetName);
        }

        public static void CreatePreviewImages(
            String FileName,
            int BaseX, int BaseY, int Width, int Height,
            int PictureBox1Width, int PictureBox1Height,
            int PictureBox2Width, int PictureBox2Height,
            int OrgSizeCandidate1, int OrgSizeCandidate2,
            out Bitmap SampleCanvas, out Bitmap OrgCanvas)
        {
            //描画先とするImageオブジェクトを作成
            Bitmap canvas_sample = new Bitmap(PictureBox1Width, PictureBox1Height);
            Bitmap canvas_org = new Bitmap(PictureBox2Width, PictureBox2Height);

            //画像ファイルのImageオブジェクトを作成
            Bitmap img = new Bitmap(FileName);

            int OrgPictSize;
            if (OrgSizeCandidate1 > OrgSizeCandidate2)
            {
                OrgPictSize = OrgSizeCandidate1;
            }
            else
            {
                OrgPictSize = OrgSizeCandidate2;
            }
            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(BaseX, BaseY, Width, Height);
            Rectangle srcRectOrg = new Rectangle(0, 0, OrgPictSize, OrgPictSize);

            //描画する部分の範囲を決定
            Rectangle desRect = new Rectangle(0, 0, PictureBox1Width, PictureBox1Height);
            Rectangle desRectOrg = new Rectangle(0, 0, PictureBox2Width, PictureBox2Height);

            //ImageオブジェクトのGraphicsオブジェクトを作成
            using (Graphics g = Graphics.FromImage(canvas_sample))
            {
                //画像の一部を描画する
                g.DrawImage(img, desRect, srcRect, GraphicsUnit.Pixel);
                g.Dispose();
            }

            using (Graphics g = Graphics.FromImage(canvas_org))
            {
                // オリジナル画像を描画
                g.DrawImage(img, desRectOrg, srcRectOrg, GraphicsUnit.Pixel);
                g.Dispose();
            }

            SampleCanvas = canvas_sample;
            OrgCanvas = canvas_org;
        }
    }
}
