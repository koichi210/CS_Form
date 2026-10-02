using System;
using System.Windows.Forms;

namespace Bmp2Gif
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();

            textBox_SrcBmp.Text = @"C:\tmp\Sample_3.bmp";
            textBox_DstGif.Text = @"C:\tmp\Sample_3.gif";
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 対象外の欄は無し(2つとも手入力するパス欄)
        private void InitializePlaceholders()
        {
            textBox_SrcBmp.PlaceholderText = @"例: C:\tmp\Sample_3.bmp";
            textBox_DstGif.PlaceholderText = @"例: C:\tmp\Sample_3.gif";
        }

        private void button_Change_Click(object sender, EventArgs e)
        {
            Logic.ConvertBmpToGif(textBox_SrcBmp.Text, textBox_DstGif.Text, checkBoxAddComment.Checked);
        }
    }
}
