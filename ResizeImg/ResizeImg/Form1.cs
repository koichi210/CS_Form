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
            Logic.Triming(textBox1.Text, int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text), int.Parse(textBox5.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap sample, org;
            Logic.CreatePreviewImages(
                textBox1.Text,
                int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text), int.Parse(textBox5.Text),
                pictureBox1.Width, pictureBox1.Height,
                pictureBox2.Width, pictureBox2.Height,
                int.Parse(textBox7.Text), int.Parse(textBox8.Text),
                out sample, out org);

            //pictureBox1に表示する
            pictureBox1.Image = sample;
            pictureBox2.Image = org;
        }
    }
}
