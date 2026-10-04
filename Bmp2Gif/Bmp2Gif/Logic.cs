using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Bmp2Gif
{
    /// <summary>
    /// もともと Form1.cs の button_Change_Click に埋め込まれていた、BMP画像を
    /// GIF形式に変換する(必要ならコメント文字列を焼き込む)ロジックをテストできる
    /// 形に切り出したもの。
    /// textBox_SrcBmp.Text などのコントロール参照は、呼び出し元(Form1)で
    /// 読み取った値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static void ConvertBmpToGif(String srcPath, String dstPath, Boolean addComment)
        {
            // 例外時もファイルロックやGDIリソース(ブラシ・フォント含む)が残らないようusingで破棄する
            using (Bitmap bmp = new Bitmap(srcPath))
            {
                if (addComment)
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (SolidBrush backBrush = new SolidBrush(Color.OrangeRed))
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    using (Font font = new Font("Times New Roman", 20))
                    {
                        g.FillRectangle(backBrush, 0, 0, 400, 100);
                        g.DrawString("gifに変換", font, textBrush, 40, 25);
                    }
                }
                bmp.Save(dstPath, ImageFormat.Gif);
            }
        }
    }
}
