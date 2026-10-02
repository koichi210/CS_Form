using System;
using System.Drawing;
using System.Windows.Forms;

namespace ResizeImg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializePlaceholders();
            // デバッグ用
            textBox1.Text = @"sample.jpg";
            textBox2.Text = @"0";
            textBox3.Text = @"0";
            textBox4.Text = @"200";
            textBox5.Text = @"300";
            textBox7.Text = @"592";
            textBox8.Text = @"312";
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnly・Multilineの欄は無いので全欄が対象
        // (textBox6(フォルダパス)は現状コードから参照されていないが、入力欄なので付けておく)
        private void InitializePlaceholders()
        {
            textBox1.PlaceholderText = "例: sample.jpg";
            textBox6.PlaceholderText = @"例: C:\Work";

            textBox2.PlaceholderText = "例: 0";
            textBox3.PlaceholderText = "例: 0";
            textBox4.PlaceholderText = "例: 200";
            textBox5.PlaceholderText = "例: 300";

            textBox7.PlaceholderText = "例: 592";
            textBox8.PlaceholderText = "例: 312";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Logic.TrimImage(textBox1.Text, int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text), int.Parse(textBox5.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap sample, original;
            Logic.CreatePreviewImages(
                textBox1.Text,
                int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text), int.Parse(textBox5.Text),
                pictureBox1.Width, pictureBox1.Height,
                pictureBox2.Width, pictureBox2.Height,
                int.Parse(textBox7.Text), int.Parse(textBox8.Text),
                out sample, out original);

            //pictureBox1に表示する
            pictureBox1.Image = sample;
            pictureBox2.Image = original;
        }
    }
}
