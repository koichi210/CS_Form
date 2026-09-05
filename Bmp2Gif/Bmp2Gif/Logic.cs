using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Bmp2Gif
{
    /// <summary>
    /// もともと Form1.cs の button_Change_Click に埋め込まれていた、BMP画像を
    /// GIF形式に変換する(必要ならコメント文字列を焼き込む)ロジックをテストできる
    /// 形に切り出したもの。コードはそのまま移しただけで書き換えていない。
    /// textBox_SrcBmp.Text などのコントロール参照は、呼び出し元(Form1)で
    /// 読み取った値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static void ConvertBmpToGif(String SrcPath, String DstPath, Boolean AddComment)
        {
            Bitmap bmp = new Bitmap(SrcPath);
            Graphics g = Graphics.FromImage(bmp);

            if (AddComment)
            {
                g.FillRectangle(
                    new SolidBrush(Color.OrangeRed),
                    0,
                    0,
                    400,
                    100);

                g.DrawString(
                    "gifに変換",
                    new Font("Times New Roman", 20),
                    new SolidBrush(Color.White),
                    40,
                    25);
            }
            bmp.Save(DstPath, ImageFormat.Gif);

            g.Dispose();
            bmp.Dispose();
        }
    }
}
