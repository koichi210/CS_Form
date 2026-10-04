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
            InitializePlaceholders();
            textBox_FilePath.Text = @"D:\sample.png";
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnly・Multilineの欄は無いので全欄が対象
        private void InitializePlaceholders()
        {
            textBox_FilePath.PlaceholderText = @"例: C:\Work\sample.png";
        }

        private void button_Exe_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_FilePath.Text))
            {
                MessageBox.Show("ファイルが存在しません。" + textBox_FilePath.Text);
                return;
            }

            // 画像ファイルは1回だけ読み込み、使い終わったら破棄する(Disposeしないとファイルがロックされたままになる)
            using (Bitmap bmp = new Bitmap(textBox_FilePath.Text))
            {
                // プレビュー
                pictureBox_Image.Image?.Dispose();
                pictureBox_Image.Image = new Bitmap(bmp, pictureBox_Image.Width, pictureBox_Image.Height);

                // サムネイル
                pictureBox_Thumbnail.Image?.Dispose();
                pictureBox_Thumbnail.Image = bmp.GetThumbnailImage(pictureBox_Thumbnail.Width, pictureBox_Thumbnail.Height, null, IntPtr.Zero);
            }
        }
    }
}
