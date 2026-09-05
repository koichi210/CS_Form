using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rotation
{
    /// <summary>
    /// もともと Form1.cs の UpdateValue / DrawPicturebox2 に実装されていた、
    /// 矢印キーによる数値インクリメントと、画像回転描画のための座標計算ロジックを
    /// テストできる形に切り出したもの。コードはそのまま移しただけで書き換えて
    /// いない。textBoxのコントロール参照は、呼び出し元(Form1)で読み取った値を
    /// 引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static String UpdateValue(String base_value, Keys KeyCode)
        {
            int add_value = 0;
            switch (KeyCode)
            {
                case Keys.Up:
                    add_value = 1;
                    break;
                case Keys.Down:
                    add_value = -1;
                    break;
                case Keys.Enter:
                    break;
                default:
                    break;
            }

            int val;
            if (Int32.TryParse(base_value.ToString(), out val))
            {
                val += add_value;
                return val.ToString();
            }
            return base_value;
        }

        /// <summary>
        /// pictureBox_Destのサイズ(元画像の長辺の2倍の正方形)を計算する。
        /// </summary>
        public static Size ComputeCanvasSize(int ImgWidth, int ImgHeight)
        {
            int max = ImgWidth;
            if (ImgWidth < ImgHeight)
            {
                max = ImgHeight;
            }
            return new Size(max * 2, max * 2);
        }

        /// <summary>
        /// Graphics.DrawImage(Image, PointF[]) に渡す変換先3点を、回転角度と
        /// 原点座標から計算する。
        /// </summary>
        public static PointF[] ComputeDestinationPoints(int ImgWidth, int ImgHeight, int AngleDegrees, float OriginX, float OriginY)
        {
            //ラジアン単位に変換
            double d = AngleDegrees / (180 / Math.PI);

            float x = OriginX;
            float y = OriginY;
            float x1 = x + ImgWidth * (float)Math.Cos(d);
            float y1 = y + ImgWidth * (float)Math.Sin(d);
            float x2 = x - ImgHeight * (float)Math.Sin(d);
            float y2 = y + ImgHeight * (float)Math.Cos(d);

            return new PointF[]
            {
                new PointF(x, y),
                new PointF(x1, y1),
                new PointF(x2, y2)
            };
        }
    }
}
