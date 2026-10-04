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
            if (pictureBox1.Image == null)
            {
                //描画先とするImageオブジェクトを作成する
                return new Bitmap(pictureBox1.Width, pictureBox1.Height);
            }

            //すでに描画済みだったら、表示されているもののイメージを取得
            return new Bitmap(pictureBox1.Image);
        }

        // キャンバスに描画してpictureBox1に表示する。差し替え前の画像は破棄する(以前は描くたびに古いBitmapが残っていた)
        private void DrawOnCanvas(Action<Graphics> draw)
        {
            Bitmap canvas = GetCanvas();
            using (Graphics g = Graphics.FromImage(canvas))
            {
                draw(g);
            }

            Image oldImage = pictureBox1.Image;
            pictureBox1.Image = canvas;
            oldImage?.Dispose();
        }

        private void DrawLine(Point startPoint, Point endPoint, Color color)
        {
            DrawOnCanvas(g =>
            {
                using (Pen pen = new Pen(color, 3))
                {
                    g.DrawLine(pen, startPoint, endPoint);
                }
            });
        }

        private void DrawCircle(Rectangle rect, Brush brush)
        {
            DrawOnCanvas(g => g.FillEllipse(brush, rect));
        }
    }
}
