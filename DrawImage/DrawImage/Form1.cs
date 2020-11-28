using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DrawImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_DrawLine_Click(object sender, EventArgs e)
        {
            //(10, 20)-(100, 200)に線を引く
            Point BasePt = new Point(10, 20);
            Point SizePt = new Point(100, 200);
            Color clr = Color.Blue;
            DrawLine(BasePt, SizePt, clr);
        }

        private void button_DrawLine2_Click(object sender, EventArgs e)
        {
            Point BasePt = new Point(100, 20);
            Point SizePt = new Point(10, 200);
            Color clr = Color.Red;
            DrawLine(BasePt, SizePt, clr);
        }

        private void button_Circle_Click(object sender, EventArgs e)
        {
            Rectangle rt = new Rectangle(15, 70, 50, 50);
            Brush brush = Brushes.White;
            DrawCircle(rt, brush);
        }

        private void button_Circle2_Click(object sender, EventArgs e)
        {
            Rectangle rt = new Rectangle(40, 100, 50, 50);
            Brush brush = Brushes.Black;
            DrawCircle(rt, brush);
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
        }

        private Bitmap GetCanvas()
        {
            Bitmap canvas;
            if (pictureBox1.Image == null)
            {
                //描画先とするImageオブジェクトを作成する
                canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            }
            else
            {
                //すでに描画済みだったら、表示されているものイメージを取得
                canvas = new Bitmap(pictureBox1.Image);
            }

            return canvas;
        }

        private void DrawLine(Point BasePt, Point SizePt, Color clr)
        {
            Bitmap canvas = GetCanvas();
            Graphics g = Graphics.FromImage(canvas);

            Pen pen = new Pen(clr, 3);
            g.DrawLine(pen, BasePt, SizePt);

            pen.Dispose();
            g.Dispose();

            pictureBox1.Image = canvas;
        }

        private void DrawCircle(Rectangle rt, Brush brush)
        {
            Bitmap canvas = GetCanvas();
            Graphics g = Graphics.FromImage(canvas);

            g.FillEllipse(brush, rt);
            g.Dispose();

            pictureBox1.Image = canvas;
        }
    }
}
