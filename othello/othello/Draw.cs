using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace othello
{
    class Draw
    {
        private readonly int CellMax = 8;
        //private readonly int EdgeOffset = 3;
        private readonly int EdgeOffset = 0;
        private readonly Color LineColor = Color.Black;
        private readonly int LineWidth = 2;

        private PictureBox pb;

        public Draw()
        {
        }

        public Draw(PictureBox pict_box)
        {
            SetDrawArea(pict_box);
        }

        //~Draw()
        //{
        //    DeleteCanvas();
        //}

        public void SetDrawArea(PictureBox pict_box)
        {
            pb = pict_box;
            CreateCanvas();
        }

        public void CreateCanvas()
        {
            DeleteCanvas();

            Bitmap canvas = new Bitmap(pb.Width, pb.Height);
            pb.Image = canvas;
        }

        public void DeleteCanvas()
        {
            if (pb.Image != null)
            {
                pb.Image.Dispose();
                pb.Image = null;
            }
        }

        public Bitmap GetCanvas()
        {
            Bitmap canvas = new Bitmap(pb.Image);
            return canvas;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void InitField()
        {
            // 基盤作成
            FillBackground(Brushes.Green);

            // 線描画
            int PartOfWidth = (pb.Width - (EdgeOffset * 2)) / CellMax;      // (全体の幅 - 両端)   / セルの数
            int PartOfHeight = (pb.Height - (EdgeOffset * 2)) / CellMax;    // (全体の高さ - 両端) / セルの数

            // 縦線
            for (int i = 0; i < CellMax + 1; i++)
            {
                Point MovePt = new Point(EdgeOffset + PartOfWidth * i, EdgeOffset);
                Point LinePt = new Point(EdgeOffset + PartOfWidth * i, pb.Height - EdgeOffset);
                WriteLine(MovePt, LinePt, LineColor, LineWidth);
            }

            // 横線
            for (int i = 0; i < CellMax + 1; i++)
            {
                Point MovePt = new Point(EdgeOffset, EdgeOffset + PartOfHeight * i);
                Point LinePt = new Point(pb.Width- EdgeOffset, EdgeOffset + PartOfHeight * i);
                WriteLine(MovePt, LinePt, LineColor, LineWidth);
            }
        }

        public void FillBackground(Brush color)
        {
            Rectangle rect = new Rectangle(0, 0, pb.Width, pb.Bottom);

            Bitmap canvas = GetCanvas();
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.FillRectangle(color, rect);
                g.Dispose();
            }

            pb.Image = canvas;
        }

        public void WriteLine(Point MovePt, Point LinePt, Color clr, int LineWidth)
        {
            Bitmap canvas = GetCanvas();
            using (Graphics g = Graphics.FromImage(canvas))
            {
                using (Pen pen = new Pen(clr, LineWidth))
                {
                    g.DrawLine(pen, MovePt, LinePt);
                    pen.Dispose();
                    g.Dispose();
                }
            }

            pb.Image = canvas;
        }
    }
}
