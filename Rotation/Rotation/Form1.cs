using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rotation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Draw(Boolean IsSave = false)
        {
            //DrawPicturebox(IsSave);
            DrawPicturebox2(IsSave);
            //DrawPanel(IsSave);
            //DrawPanel2(IsSave);
        }

        private void button_ClickDraw(object sender, EventArgs e)
        {
            if (pictureBox_Source.Image != null)
            {
                pictureBox_Source.Image.Dispose();
                pictureBox_Source.Image = null;
            }
            pictureBox_Source.Image = Image.FromFile(textBox_loadfiepath.Text);
            Draw();
        }

        private void button_ClickSave(object sender, EventArgs e)
        {
            Draw(true);
        }

        private void textBox_angle_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_angle.Text = UpdateValue(textBox_angle.Text, e);
            Draw();
        }

        private void textBox_OriginX_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_OriginX.Text = UpdateValue(textBox_OriginX.Text, e);
            Draw();
        }

        private void textBox_OriginY_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_OriginY.Text = UpdateValue(textBox_OriginY.Text, e);
            Draw();
        }

        private String UpdateValue(String base_value, KeyEventArgs e)
        {
            int add_value = 0;
            switch (e.KeyCode)
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
            if ( Int32.TryParse(base_value.ToString(), out val) )
            {
                val += add_value;
                return val.ToString();
            }
            return base_value;
        }

        private void DrawPicturebox(Boolean IsSave = false)
        {
            Bitmap canvas = new Bitmap(pictureBox_Dest.Width, pictureBox_Dest.Height);
            Bitmap img = new Bitmap(textBox_loadfiepath.Text);

            //ラジアン単位に変換
            int angle = Int32.Parse(textBox_angle.Text.ToString());
            double d = angle / (180 / Math.PI);

            //新しい座標位置を計算する
            float x = float.Parse(textBox_OriginX.Text.ToString());
            float y = float.Parse(textBox_OriginY.Text.ToString());
            float x1 = x + img.Width * (float)Math.Cos(d);
            float y1 = y + img.Width * (float)Math.Sin(d);
            float x2 = x - img.Height * (float)Math.Sin(d);
            float y2 = y + img.Height * (float)Math.Cos(d);

            //PointF配列を作成
            PointF[] destinationPoints =
            {
                new PointF(x, y),
                new PointF(x1, y1),
                new PointF(x2, y2)
            };

            using (Graphics g = Graphics.FromImage(canvas))
            {
                //画像を表示
                g.DrawImage(img, destinationPoints);

                g.Dispose();
                img.Dispose();
            }

            //pictureBoxに表示
            pictureBox_Dest.Image = canvas;

            if (IsSave)
            {
                canvas.Save(textBox_savefiepath.Text);
            }
        }

        private void DrawPicturebox2(Boolean IsSave = false)
        {
            Bitmap img = new Bitmap(textBox_loadfiepath.Text);
            int max = img.Width;
            if (img.Width < img.Height)
            {
                max = img.Height;
            }
            pictureBox_Dest.Size = new Size(max * 2, max * 2);
            Bitmap canvas = new Bitmap(pictureBox_Dest.Width, pictureBox_Dest.Height);

            //ラジアン単位に変換
            int angle = Int32.Parse(textBox_angle.Text.ToString());
            double d = angle / (180 / Math.PI);

            //新しい座標位置を計算する
            float x;
            float y;
            if (!float.TryParse(textBox_OriginX.Text.ToString(), out x) ||
                !float.TryParse(textBox_OriginY.Text.ToString(), out y) )
            {
                img.Dispose();
                canvas.Dispose();
                return;
            }

            float x1 = x + img.Width * (float)Math.Cos(d);
            float y1 = y + img.Width * (float)Math.Sin(d);
            float x2 = x - img.Height * (float)Math.Sin(d);
            float y2 = y + img.Height * (float)Math.Cos(d);

            //PointF配列を作成
            PointF[] destinationPoints =
            {
                new PointF(x, y),
                new PointF(x1, y1),
                new PointF(x2, y2)
            };

            using (Graphics g = Graphics.FromImage(canvas))
            {
                //画像を表示
                g.DrawImage(img, destinationPoints);

                g.Dispose();
                img.Dispose();
            }

            //pictureBoxに表示
            if (pictureBox_Dest.Image != null)
            {
                pictureBox_Dest.Image.Dispose();
                pictureBox_Dest.Image = null;
            }
            pictureBox_Dest.Image = canvas;

            if (IsSave)
            {
                canvas.Save(textBox_savefiepath.Text);
            }
        }

        private void DrawPanel(Boolean IsSave = false)
        {
            Bitmap canvas = new Bitmap(pictureBox_Dest.Width, pictureBox_Dest.Height);
            Bitmap img = new Bitmap(textBox_loadfiepath.Text);

            //PictureBoxオブジェクトの作成
            PictureBox pictureBox1 = new PictureBox();
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Size = new Size(img.Width*2, img.Height*2);
            // pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            //ラジアン単位に変換
            int angle = Int32.Parse(textBox_angle.Text.ToString());
            double d = angle / (180 / Math.PI);

            //新しい座標位置を計算する
            float x = float.Parse(textBox_OriginX.Text.ToString());
            float y = float.Parse(textBox_OriginY.Text.ToString());
            float x1 = x + img.Width * (float)Math.Cos(d);
            float y1 = y + img.Width * (float)Math.Sin(d);
            float x2 = x - img.Height * (float)Math.Sin(d);
            float y2 = y + img.Height * (float)Math.Cos(d);

            //PointF配列を作成
            PointF[] destinationPoints =
            {
                new PointF(x, y),
                new PointF(x1, y1),
                new PointF(x2, y2)
            };

            using (Graphics g = Graphics.FromImage(canvas))
            {
                //画像を表示
                g.DrawImage(img, destinationPoints);

                g.Dispose();
                img.Dispose();
            }

            //pictureBoxに表示
            pictureBox1.Image = canvas;

            //pictureBoxをpanelに表示
            panel_Dest.Controls.Add(pictureBox1);

            if (IsSave)
            {
                //pictureBox1.Image.Save(textBox_savefiepath.Text, System.Drawing.Imaging.ImageFormat.Png);
                canvas.Save(textBox_savefiepath.Text, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void DrawPanel2(Boolean IsSave = false)
        {
            Bitmap canvas = new Bitmap(pictureBox_Dest.Width, pictureBox_Dest.Height);
            Bitmap img = new Bitmap(textBox_loadfiepath.Text);

            int max = img.Width;
            if (img.Width < img.Height)
            {
                max = Height;
            }

            pictureBox_Dest.Size = new Size(max * 2, max * 2);
            // pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            //ラジアン単位に変換
            int angle = Int32.Parse(textBox_angle.Text.ToString());
            double d = angle / (180 / Math.PI);

            //新しい座標位置を計算
            float x = float.Parse(textBox_OriginX.Text.ToString());
            float y = float.Parse(textBox_OriginY.Text.ToString());
            float x1 = x + img.Width * (float)Math.Cos(d);
            float y1 = y + img.Width * (float)Math.Sin(d);
            float x2 = x - img.Height * (float)Math.Sin(d);
            float y2 = y + img.Height * (float)Math.Cos(d);

            //PointF配列を作成
            PointF[] destinationPoints =
            {
                new PointF(x, y),
                new PointF(x1, y1),
                new PointF(x2, y2)
            };

            using (Graphics g = Graphics.FromImage(canvas))
            {
                //画像を表示
                g.DrawImage(img, destinationPoints);

                g.Dispose();
                img.Dispose();
            }

            //pictureBoxに表示
            pictureBox_Dest.Image = canvas;

            //pictureBoxをpanelに表示
            panel_Dest.Controls.Add(pictureBox_Dest);

            if (IsSave)
            {
                //pictureBox1.Image.Save(textBox_savefiepath.Text, System.Drawing.Imaging.ImageFormat.Png);
                canvas.Save(textBox_savefiepath.Text, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
