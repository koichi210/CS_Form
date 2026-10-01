using System;
using System.Drawing;
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
            Point startPoint = new Point(10, 20);
            Point endPoint = new Point(100, 200);
            Color color = Color.Blue;
            DrawLine(startPoint, endPoint, color);
        }

        private void button_DrawLine2_Click(object sender, EventArgs e)
        {
            Point startPoint = new Point(100, 20);
            Point endPoint = new Point(10, 200);
            Color color = Color.Red;
            DrawLine(startPoint, endPoint, color);
        }

        private void button_Circle_Click(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle(15, 70, 50, 50);
            Brush brush = Brushes.White;
            DrawCircle(rect, brush);
        }

        private void button_Circle2_Click(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle(40, 100, 50, 50);
            Brush brush = Brushes.Black;
            DrawCircle(rect, brush);
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
                //すでに描画済みだったら、表示されているもののイメージを取得
                canvas = new Bitmap(pictureBox1.Image);
            }

            return canvas;
        }

        private void DrawLine(Point startPoint, Point endPoint, Color color)
        {
            Bitmap canvas = GetCanvas();
            Graphics g = Graphics.FromImage(canvas);

            Pen pen = new Pen(color, 3);
            g.DrawLine(pen, startPoint, endPoint);

            pen.Dispose();
            g.Dispose();

            pictureBox1.Image = canvas;
        }

        private void DrawCircle(Rectangle rect, Brush brush)
        {
            Bitmap canvas = GetCanvas();
            Graphics g = Graphics.FromImage(canvas);

            g.FillEllipse(brush, rect);
            g.Dispose();

            pictureBox1.Image = canvas;
        }
    }
}
