using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rotation
{
    /// <summary>
    /// もともと Form1.cs の UpdateValue / DrawPictureBox に実装されていた、
    /// 矢印キーによる数値インクリメントと、画像回転描画のための座標計算ロジックを
    /// テストできる形に切り出したもの。textBoxのコントロール参照は、呼び出し元(Form1)で読み取った値を
    /// 引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static string UpdateValue(string baseValue, Keys keyCode)
        {
            int value;
            if (!int.TryParse(baseValue, out value))
            {
                return baseValue;
            }

            switch (keyCode)
            {
                case Keys.Up:
                    value++;
                    break;
                case Keys.Down:
                    value--;
                    break;
            }
            return value.ToString();
        }

        /// <summary>
        /// pictureBox_Destのサイズ(元画像の長辺の2倍の正方形)を計算する。
        /// </summary>
        public static Size ComputeCanvasSize(int imgWidth, int imgHeight)
        {
            int max = Math.Max(imgWidth, imgHeight);
            return new Size(max * 2, max * 2);
        }

        /// <summary>
        /// Graphics.DrawImage(Image, PointF[]) に渡す変換先3点を、回転角度と
        /// 原点座標から計算する。
        /// </summary>
        public static PointF[] ComputeDestinationPoints(int imgWidth, int imgHeight, int angleDegrees, float originX, float originY)
        {
            //ラジアン単位に変換
            double radians = angleDegrees / (180 / Math.PI);

            float x = originX;
            float y = originY;
            float x1 = x + imgWidth * (float)Math.Cos(radians);
            float y1 = y + imgWidth * (float)Math.Sin(radians);
            float x2 = x - imgHeight * (float)Math.Sin(radians);
            float y2 = y + imgHeight * (float)Math.Cos(radians);

            return new PointF[]
            {
                new PointF(x, y),
                new PointF(x1, y1),
                new PointF(x2, y2)
            };
        }
    }
}
