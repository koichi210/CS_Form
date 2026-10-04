using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Rotation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
            InitializeToolTips();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnly・Multilineの欄は無いので全欄が対象
        private void InitializePlaceholders()
        {
            textBox_loadfilepath.PlaceholderText = @"例: C:\Work\input.png";
            textBox_savefilepath.PlaceholderText = @"例: C:\Work\output.png";

            textBox_angle.PlaceholderText = "例: 90";
            textBox_OriginX.PlaceholderText = "例: 0";
            textBox_OriginY.PlaceholderText = "例: 0";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_angle, "回転角度(度、整数)。正の値で時計回り。↑/↓キーで1ずつ増減してプレビューを更新する");
            toolTip.SetToolTip(textBox_OriginX, "回転後の画像の左上角を置くX座標(ピクセル)。↑/↓キーで1ずつ増減してプレビューを更新する");
            toolTip.SetToolTip(textBox_OriginY, "回転後の画像の左上角を置くY座標(ピクセル)。↑/↓キーで1ずつ増減してプレビューを更新する");
            toolTip.SetToolTip(button_Save, "回転結果を保存ファイルパスに書き出す。拡張子に関係なくPNG形式で保存し、同名ファイルは上書きする");
        }

        private void button_ClickDraw(object sender, EventArgs e)
        {
            pictureBox_Source.Image?.Dispose();
            pictureBox_Source.Image = null;
            pictureBox_Source.Image = Image.FromFile(textBox_loadfilepath.Text);
            DrawPictureBox();
        }

        private void button_ClickSave(object sender, EventArgs e)
        {
            if (!DrawPictureBox(true))
            {
                MessageBox.Show("画像ファイルのパス・角度・原点の値を確認してください");
            }
        }

        private void textBox_angle_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateValueAndRedraw(textBox_angle, e.KeyCode);
        }

        private void textBox_OriginX_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateValueAndRedraw(textBox_OriginX, e.KeyCode);
        }

        private void textBox_OriginY_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateValueAndRedraw(textBox_OriginY, e.KeyCode);
        }

        // ↑/↓キーで入力欄の値を1ずつ増減し、プレビューを更新する
        private void UpdateValueAndRedraw(TextBox textBox, Keys keyCode)
        {
            textBox.Text = Logic.UpdateValue(textBox.Text, keyCode);
            DrawPictureBox();
        }

        // キー入力のたびに呼ばれるため、入力途中の値や画像未指定では例外にせず、描画せずにfalseを返す
        private bool DrawPictureBox(bool isSave = false)
        {
            int angle;
            float x;
            float y;
            if (!File.Exists(textBox_loadfilepath.Text) ||
                !int.TryParse(textBox_angle.Text, out angle) ||
                !float.TryParse(textBox_OriginX.Text, out x) ||
                !float.TryParse(textBox_OriginY.Text, out y))
            {
                return false;
            }

            Bitmap img;
            try
            {
                img = new Bitmap(textBox_loadfilepath.Text);
            }
            catch (ArgumentException)
            {
                // 画像として読めないファイル
                return false;
            }

            Bitmap canvas;
            using (img)
            {
                pictureBox_Dest.Size = Logic.ComputeCanvasSize(img.Width, img.Height);
                canvas = new Bitmap(pictureBox_Dest.Width, pictureBox_Dest.Height);

                //PointF配列を作成
                PointF[] destinationPoints = Logic.ComputeDestinationPoints(img.Width, img.Height, angle, x, y);

                using (Graphics g = Graphics.FromImage(canvas))
                {
                    //画像を表示
                    g.DrawImage(img, destinationPoints);
                }
            }

            //pictureBoxに表示
            pictureBox_Dest.Image?.Dispose();
            pictureBox_Dest.Image = canvas;

            if (isSave)
            {
                canvas.Save(textBox_savefilepath.Text);
            }
            return true;
        }
    }
}
