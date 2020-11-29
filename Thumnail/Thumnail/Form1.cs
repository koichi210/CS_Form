using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Thumnail
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
                Bitmap small = new Bitmap(bmp, pictureBox_Image.Width, pictureBox_Image.Height);
                pictureBox_Image.Image = small;
            }

            // サムネイル
            {
                Bitmap bmp = new Bitmap(textBox_FilePath.Text);
                Image small = bmp.GetThumbnailImage(pictureBox_Thumnail.Width, pictureBox_Thumnail.Height, null, IntPtr.Zero);
                pictureBox_Thumnail.Image = small;
            }
        }
    }
}
