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
            InitializeToolTips();

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

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_DstGif, "保存先のGIFファイルのパス。同名のファイルがあれば上書きする");
            toolTip.SetToolTip(checkBoxAddComment, "変換した画像の左上に「gifに変換」と書いた帯を描き込む");
        }

        private void button_Change_Click(object sender, EventArgs e)
        {
            Logic.ConvertBmpToGif(textBox_SrcBmp.Text, textBox_DstGif.Text, checkBoxAddComment.Checked);
        }
    }
}
