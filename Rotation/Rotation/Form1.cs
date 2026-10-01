using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Rotation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_ClickDraw(object sender, EventArgs e)
        {
            if (pictureBox_Source.Image != null)
            {
                pictureBox_Source.Image.Dispose();
                pictureBox_Source.Image = null;
            }
            pictureBox_Source.Image = Image.FromFile(textBox_loadfiepath.Text);
            DrawPictureBox();
        }

        private void button_ClickSave(object sender, EventArgs e)
        {
            if (!DrawPictureBox(true))
            {
                MessageBox.Show("画像ファイルのパス・角度・原点の値を確認してください");
            }
        }

        private void textBox_angle_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_angle.Text = Logic.UpdateValue(textBox_angle.Text, e.KeyCode);
            DrawPictureBox();
        }

        private void textBox_OriginX_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_OriginX.Text = Logic.UpdateValue(textBox_OriginX.Text, e.KeyCode);
            DrawPictureBox();
        }

        private void textBox_OriginY_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_OriginY.Text = Logic.UpdateValue(textBox_OriginY.Text, e.KeyCode);
            DrawPictureBox();
        }

        // キー入力のたびに呼ばれるため、入力途中の値や画像未指定では例外にせず、描画せずにfalseを返す
        private Boolean DrawPictureBox(Boolean isSave = false)
        {
            int angle;
            float x;
            float y;
            if (!File.Exists(textBox_loadfiepath.Text) ||
                !Int32.TryParse(textBox_angle.Text, out angle) ||
                !float.TryParse(textBox_OriginX.Text, out x) ||
                !float.TryParse(textBox_OriginY.Text, out y))
            {
                return false;
            }

            Bitmap img;
            try
            {
                img = new Bitmap(textBox_loadfiepath.Text);
            }
            catch (ArgumentException)
            {
                // 画像として読めないファイル
                return false;
            }

            Bitmap canvas;
            using (img)
            {
                pictureBox_Dest.Size = Logic.ComputeCanvasSize(img.Width, img.Height);
                canvas = new Bitmap(pictureBox_Dest.Width, pictureBox_Dest.Height);

                //PointF配列を作成
                PointF[] destinationPoints = Logic.ComputeDestinationPoints(img.Width, img.Height, angle, x, y);

                using (Graphics g = Graphics.FromImage(canvas))
                {
                    //画像を表示
                    g.DrawImage(img, destinationPoints);
                }
            }

            //pictureBoxに表示
            if (pictureBox_Dest.Image != null)
            {
                pictureBox_Dest.Image.Dispose();
                pictureBox_Dest.Image = null;
            }
            pictureBox_Dest.Image = canvas;

            if (isSave)
            {
                canvas.Save(textBox_savefiepath.Text);
            }
            return true;
        }
    }
}
