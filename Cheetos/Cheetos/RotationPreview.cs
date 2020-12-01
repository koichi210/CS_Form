using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
namespace Cheetos
{
    public partial class RotationPreview : Form
    {
        private Button buttonCancel;
        private Button buttonOk;
        private Label label1;
        private TextBox textBox_loadfiepath;
        private Button button_draw;
        private PictureBox pictureBox_Source;
        private PictureBox pictureBox_Dest;
        private TextBox textBox_OriginX;
        private Label label4;
        private TextBox textBox_OriginY;
        private Label label3;
        private Label label2;
        private TextBox textBox_angle;

        public String OriginX = "";
        public String OriginY = "";
        public String Angle = "";

        private void InitializeComponent()
        {
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_loadfiepath = new System.Windows.Forms.TextBox();
            this.button_draw = new System.Windows.Forms.Button();
            this.pictureBox_Source = new System.Windows.Forms.PictureBox();
            this.pictureBox_Dest = new System.Windows.Forms.PictureBox();
            this.textBox_OriginX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_OriginY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_angle = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Source)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Dest)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(556, 37);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(556, 10);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 9;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ファイルパス";
            // 
            // textBox_loadfiepath
            // 
            this.textBox_loadfiepath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_loadfiepath.Location = new System.Drawing.Point(77, 12);
            this.textBox_loadfiepath.Name = "textBox_loadfiepath";
            this.textBox_loadfiepath.Size = new System.Drawing.Size(355, 19);
            this.textBox_loadfiepath.TabIndex = 1;
            // 
            // button_draw
            // 
            this.button_draw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_draw.Location = new System.Drawing.Point(438, 12);
            this.button_draw.Name = "button_draw";
            this.button_draw.Size = new System.Drawing.Size(75, 21);
            this.button_draw.TabIndex = 2;
            this.button_draw.Text = "load";
            this.button_draw.UseVisualStyleBackColor = true;
            this.button_draw.Click += new System.EventHandler(this.button_ClickDraw);
            // 
            // pictureBox_Source
            // 
            this.pictureBox_Source.Location = new System.Drawing.Point(15, 66);
            this.pictureBox_Source.Name = "pictureBox_Source";
            this.pictureBox_Source.Size = new System.Drawing.Size(280, 280);
            this.pictureBox_Source.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_Source.TabIndex = 13;
            this.pictureBox_Source.TabStop = false;
            // 
            // pictureBox_Dest
            // 
            this.pictureBox_Dest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_Dest.Location = new System.Drawing.Point(317, 66);
            this.pictureBox_Dest.Name = "pictureBox_Dest";
            this.pictureBox_Dest.Size = new System.Drawing.Size(280, 280);
            this.pictureBox_Dest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_Dest.TabIndex = 14;
            this.pictureBox_Dest.TabStop = false;
            // 
            // textBox_OriginX
            // 
            this.textBox_OriginX.Location = new System.Drawing.Point(206, 37);
            this.textBox_OriginX.Name = "textBox_OriginX";
            this.textBox_OriginX.Size = new System.Drawing.Size(46, 19);
            this.textBox_OriginX.TabIndex = 4;
            this.textBox_OriginX.Text = "0";
            this.textBox_OriginX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_OriginX_KeyDown);
            this.textBox_OriginX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_OriginX_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(257, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "原点Y";
            // 
            // textBox_OriginY
            // 
            this.textBox_OriginY.Location = new System.Drawing.Point(299, 37);
            this.textBox_OriginY.Name = "textBox_OriginY";
            this.textBox_OriginY.Size = new System.Drawing.Size(46, 19);
            this.textBox_OriginY.TabIndex = 6;
            this.textBox_OriginY.Text = "0";
            this.textBox_OriginY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_OriginY_KeyDown);
            this.textBox_OriginY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_OriginY_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(350, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "角度";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "原点X";
            // 
            // textBox_angle
            // 
            this.textBox_angle.Location = new System.Drawing.Point(385, 37);
            this.textBox_angle.Name = "textBox_angle";
            this.textBox_angle.Size = new System.Drawing.Size(46, 19);
            this.textBox_angle.TabIndex = 8;
            this.textBox_angle.Text = "0";
            this.textBox_angle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Angle_KeyDown);
            this.textBox_angle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_Angle_KeyPress);
            // 
            // RotationPreview
            // 
            this.ClientSize = new System.Drawing.Size(634, 649);
            this.Controls.Add(this.textBox_OriginX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_OriginY);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_angle);
            this.Controls.Add(this.pictureBox_Dest);
            this.Controls.Add(this.pictureBox_Source);
            this.Controls.Add(this.button_draw);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_loadfiepath);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.MinimumSize = new System.Drawing.Size(550, 200);
            this.Name = "RotationPreview";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Source)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Dest)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public RotationPreview()
        {
            InitializeComponent();
        }

        public RotationPreview(String OriginX, String OriginY, String Angle )
        {
            InitializeComponent();

            textBox_OriginX.Text = OriginX;
            textBox_OriginY.Text = OriginY;
            textBox_angle.Text = Angle;
        }

        private void button_ClickDraw(object sender, EventArgs e)
        {
            if (pictureBox_Source.Image != null)
            {
                pictureBox_Source.Image.Dispose();
                pictureBox_Source.Image = null;
            }
            pictureBox_Source.Image = Image.FromFile(textBox_loadfiepath.Text);
            Draw();
        }

        private void textBox_OriginX_KeyPress(object sender, KeyPressEventArgs e)
        {
            Draw();
        }

        private void textBox_OriginY_KeyPress(object sender, KeyPressEventArgs e)
        {
            Draw();
        }

        private void textBox_Angle_KeyPress(object sender, KeyPressEventArgs e)
        {
            Draw();
        }

        private void textBox_OriginX_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_OriginX.Text = UpdateValue(textBox_OriginX.Text, e);
            Draw();
        }

        private void textBox_OriginY_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_OriginY.Text = UpdateValue(textBox_OriginY.Text, e);
            Draw();
        }

        private void textBox_Angle_KeyDown(object sender, KeyEventArgs e)
        {
            textBox_angle.Text = UpdateValue(textBox_angle.Text, e);
            Draw();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            OriginX = textBox_OriginX.Text;
            OriginY = textBox_OriginY.Text;
            Angle = textBox_angle.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private Boolean AdjustParam()
        {
            if (!File.Exists(textBox_loadfiepath.Text))
            {
                return false;
            }

            int val;
            if (!Int32.TryParse(textBox_OriginX.Text.ToString(), out val))
            {
                textBox_OriginX.Text = "";
            }
            if (!Int32.TryParse(textBox_OriginY.Text.ToString(), out val))
            {
                textBox_OriginY.Text = "";
            }
            if (!Int32.TryParse(textBox_angle.Text.ToString(), out val))
            {
                textBox_angle.Text = "";
            }
            
            return true;
        }

        private void Draw()
        {
            if ( ! AdjustParam() )
            {
                return;
            }

            Bitmap img = new Bitmap(textBox_loadfiepath.Text);
            int max = img.Width;
            if (img.Width < img.Height)
            {
                max = img.Height;
            }
            pictureBox_Dest.Size = new Size(max * 2, max * 2);
            Bitmap canvas = new Bitmap(pictureBox_Dest.Width, pictureBox_Dest.Height);

            //ラジアン単位に変換
            int angle = 0;
            Int32.TryParse(textBox_angle.Text.ToString(), out angle);
            double d = angle / (180 / Math.PI);

            //新しい座標位置を計算する
            float x = float.Parse(textBox_OriginX.Text.ToString());
            float y = float.Parse(textBox_OriginY.Text.ToString());

            float x1 = x + img.Width * (float)Math.Cos(d);
            float y1 = y + img.Width * (float)Math.Sin(d);
            float x2 = x - img.Height * (float)Math.Sin(d);
            float y2 = y + img.Height * (float)Math.Cos(d);

            //PointF配列を作成
            PointF[] destinationPoints =
            {
                new PointF(x, y),
                new PointF(x1, y1),
                new PointF(x2, y2)
            };

            using (Graphics g = Graphics.FromImage(canvas))
            {
                //画像を表示
                g.DrawImage(img, destinationPoints);

                g.Dispose();
                img.Dispose();
            }

            //pictureBoxに表示
            if (pictureBox_Dest.Image != null)
            {
                pictureBox_Dest.Image.Dispose();
                pictureBox_Dest.Image = null;
            }
            pictureBox_Dest.Image = canvas;
        }

        private String UpdateValue(String base_value, KeyEventArgs e)
        {
            int add_value = 0;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    add_value = 1;
                    break;
                case Keys.Down:
                    add_value = -1;
                    break;
                case Keys.Enter:
                    break;
                default:
                    break;
            }

            int val;
            if (Int32.TryParse(base_value.ToString(), out val))
            {
                val += add_value;
                return val.ToString();
            }
            return base_value;
        }
    }
}
