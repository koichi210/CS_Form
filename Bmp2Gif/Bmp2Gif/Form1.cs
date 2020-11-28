using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Bmp2Gif
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            textBox_SrcBmp.Text = @"C:\tmp\Sample_3.bmp";
            textBox_DstGif.Text = @"C:\tmp\Sample_3.gif";
        }

        private void button_Change_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(textBox_SrcBmp.Text);
            Graphics g = Graphics.FromImage(bmp);

            if (checkBoxAddComent.Checked)
            {
                g.FillRectangle(
                    new SolidBrush(Color.OrangeRed),
                    0,
                    0,
                    400,
                    100);

                g.DrawString(
                    "gifに変換",
                    new Font("Times New Roman", 20),
                    new SolidBrush(Color.White),
                    40,
                    25);
            }
            bmp.Save(textBox_DstGif.Text, ImageFormat.Gif);

            g.Dispose();
            bmp.Dispose();
        }
    }
}
