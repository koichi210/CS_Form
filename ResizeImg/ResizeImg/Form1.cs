using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResizeImg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // デバッグ用
            textBox1.Text = @"sample.jpg";
            textBox2.Text = @"0";
            textBox3.Text = @"0";
            textBox4.Text = @"200";
            textBox5.Text = @"300";
            textBox7.Text = @"592";
            textBox8.Text = @"312";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Triming(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PreView(textBox1.Text);
        }

        private void Triming(String FileName)
        {
            //描画先とするImageオブジェクトを作成
            Bitmap canvas = new Bitmap(int.Parse(textBox4.Text), int.Parse(textBox5.Text));

            //画像ファイルのImageオブジェクトを作成
            Bitmap img = new Bitmap(FileName);

            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text), int.Parse(textBox5.Text));

            //描画する部分の範囲を決定
            Rectangle desRect = new Rectangle(0, 0, int.Parse(textBox4.Text), int.Parse(textBox5.Text));

            //ImageオブジェクトのGraphicsオブジェクトを作成
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.DrawImage(img, desRect, srcRect, GraphicsUnit.Pixel);
                g.Dispose();
            }

            String TargetName = FileName.Replace(".", "_new.");
            canvas.Save(TargetName);
        }
        
        private void PreView(String FileName)
        {
            //描画先とするImageオブジェクトを作成
            Bitmap canvas_sample = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Bitmap canvas_org = new Bitmap(pictureBox2.Width, pictureBox2.Height);

            //画像ファイルのImageオブジェクトを作成
            Bitmap img = new Bitmap(FileName);

            int OrgPictSize;
            if ( int.Parse(textBox7.Text) > int.Parse(textBox8.Text) )
            {
                OrgPictSize = int.Parse(textBox7.Text);
            }
            else
            {
                OrgPictSize = int.Parse(textBox8.Text);
            }
            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text), int.Parse(textBox5.Text));
            Rectangle srcRectOrg = new Rectangle(0, 0, OrgPictSize, OrgPictSize);

            //描画する部分の範囲を決定
            Rectangle desRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            Rectangle desRectOrg = new Rectangle(0, 0, pictureBox2.Width, pictureBox2.Height);

            //ImageオブジェクトのGraphicsオブジェクトを作成
            using (Graphics g = Graphics.FromImage(canvas_sample))
            {
                //画像の一部を描画する
                g.DrawImage(img, desRect, srcRect, GraphicsUnit.Pixel);
                g.Dispose();
            }

            using (Graphics g = Graphics.FromImage(canvas_org))
            {
                // オリジナル画像を描画
                g.DrawImage(img, desRectOrg, srcRectOrg, GraphicsUnit.Pixel);
                g.Dispose();
            }

            //pictureBox1に表示する
            pictureBox1.Image = canvas_sample;
            pictureBox2.Image = canvas_org;
        }
    }
}
