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
            InitializeToolTips();
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

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox1, "読み込む画像ファイルのパス。フォルダを含まない場合はカレントディレクトリから探す");
            toolTip.SetToolTip(textBox6, "現在の処理では使われない。読み込み先はファイル名欄だけで決まる");

            toolTip.SetToolTip(textBox2, "切り取る範囲の左上のX座標(ピクセル)");
            toolTip.SetToolTip(textBox3, "切り取る範囲の左上のY座標(ピクセル)");
            toolTip.SetToolTip(textBox4, "切り取る範囲の幅(ピクセル)");
            toolTip.SetToolTip(textBox5, "切り取る範囲の高さ(ピクセル)");

            toolTip.SetToolTip(textBox7, "トリミング前プレビューに出す範囲(ピクセル)。幅と高さの大きい方を一辺とする正方形を左上から表示する");
            toolTip.SetToolTip(textBox8, "トリミング前プレビューに出す範囲(ピクセル)。幅と高さの大きい方を一辺とする正方形を左上から表示する");

            toolTip.SetToolTip(button1, "切り取った画像を、ファイル名の「.」を「_new.」に置き換えた名前で保存する(例: sample.jpg → sample_new.jpg)。同名ファイルは上書きする");
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

            //pictureBoxに表示する(前回表示していた画像は破棄する)
            pictureBox1.Image?.Dispose();
            pictureBox2.Image?.Dispose();
            pictureBox1.Image = sample;
            pictureBox2.Image = original;
        }
    }
}
