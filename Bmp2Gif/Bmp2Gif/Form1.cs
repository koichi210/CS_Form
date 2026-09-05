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
            Logic.ConvertBmpToGif(textBox_SrcBmp.Text, textBox_DstGif.Text, checkBoxAddComent.Checked);
        }
    }
}
