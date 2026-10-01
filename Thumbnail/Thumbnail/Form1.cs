using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Thumbnail
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox_FilePath.Text = @"D:\sample.png";
        }

        private void button_Exe_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_FilePath.Text))
            {
                MessageBox.Show("ファイルが存在しません。" + textBox_FilePath.Text);
                return;
            }

            // プレビュー
            {
                Bitmap bmp = new Bitmap(textBox_FilePath.Text);
                Bitmap preview = new Bitmap(bmp, pictureBox_Image.Width, pictureBox_Image.Height);
                pictureBox_Image.Image = preview;
            }

            // サムネイル
            {
                Bitmap bmp = new Bitmap(textBox_FilePath.Text);
                Image thumbnail = bmp.GetThumbnailImage(pictureBox_Thumbnail.Width, pictureBox_Thumbnail.Height, null, IntPtr.Zero);
                pictureBox_Thumbnail.Image = thumbnail;
            }
        }
    }
}
