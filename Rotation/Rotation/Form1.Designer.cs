namespace Rotation
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.button_draw = new System.Windows.Forms.Button();
            this.pictureBox_Source = new System.Windows.Forms.PictureBox();
            this.textBox_angle = new System.Windows.Forms.TextBox();
            this.textBox_loadfiepath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Save = new System.Windows.Forms.Button();
            this.pictureBox_Dest = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_OriginY = new System.Windows.Forms.TextBox();
            this.textBox_OriginX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_savefiepath = new System.Windows.Forms.TextBox();
            this.panel_Dest = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Source)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Dest)).BeginInit();
            this.SuspendLayout();
            // 
            // button_draw
            // 
            this.button_draw.Location = new System.Drawing.Point(370, 39);
            this.button_draw.Name = "button_draw";
            this.button_draw.Size = new System.Drawing.Size(75, 21);
            this.button_draw.TabIndex = 2;
            this.button_draw.Text = "load";
            this.button_draw.UseVisualStyleBackColor = true;
            this.button_draw.Click += new System.EventHandler(this.button_ClickDraw);
            // 
            // pictureBox_Source
            // 
            this.pictureBox_Source.Location = new System.Drawing.Point(12, 63);
            this.pictureBox_Source.Name = "pictureBox_Source";
            this.pictureBox_Source.Size = new System.Drawing.Size(433, 402);
            this.pictureBox_Source.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_Source.TabIndex = 1;
            this.pictureBox_Source.TabStop = false;
            // 
            // textBox_angle
            // 
            this.textBox_angle.Location = new System.Drawing.Point(767, 40);
            this.textBox_angle.Name = "textBox_angle";
            this.textBox_angle.Size = new System.Drawing.Size(46, 19);
            this.textBox_angle.TabIndex = 11;
            this.textBox_angle.Text = "0";
            this.textBox_angle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_angle_KeyDown);
            // 
            // textBox_loadfiepath
            // 
            this.textBox_loadfiepath.Location = new System.Drawing.Point(114, 16);
            this.textBox_loadfiepath.Name = "textBox_loadfiepath";
            this.textBox_loadfiepath.Size = new System.Drawing.Size(331, 19);
            this.textBox_loadfiepath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "読込ファイルパス";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(546, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "原点X";
            // 
            // button_Save
            // 
            this.button_Save.Location = new System.Drawing.Point(819, 15);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 44);
            this.button_Save.TabIndex = 5;
            this.button_Save.Text = "save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_ClickSave);
            // 
            // pictureBox_Dest
            // 
            this.pictureBox_Dest.Location = new System.Drawing.Point(461, 63);
            this.pictureBox_Dest.Name = "pictureBox_Dest";
            this.pictureBox_Dest.Size = new System.Drawing.Size(433, 402);
            this.pictureBox_Dest.TabIndex = 8;
            this.pictureBox_Dest.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(732, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "角度";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(639, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "原点Y";
            // 
            // textBox_OriginY
            // 
            this.textBox_OriginY.Location = new System.Drawing.Point(681, 40);
            this.textBox_OriginY.Name = "textBox_OriginY";
            this.textBox_OriginY.Size = new System.Drawing.Size(46, 19);
            this.textBox_OriginY.TabIndex = 9;
            this.textBox_OriginY.Text = "0";
            this.textBox_OriginY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_OriginY_KeyDown);
            // 
            // textBox_OriginX
            // 
            this.textBox_OriginX.Location = new System.Drawing.Point(588, 40);
            this.textBox_OriginX.Name = "textBox_OriginX";
            this.textBox_OriginX.Size = new System.Drawing.Size(46, 19);
            this.textBox_OriginX.TabIndex = 7;
            this.textBox_OriginX.Text = "0";
            this.textBox_OriginX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_OriginX_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(459, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "保存ファイルパス";
            // 
            // textBox_savefiepath
            // 
            this.textBox_savefiepath.Location = new System.Drawing.Point(547, 16);
            this.textBox_savefiepath.Name = "textBox_savefiepath";
            this.textBox_savefiepath.Size = new System.Drawing.Size(266, 19);
            this.textBox_savefiepath.TabIndex = 4;
            // 
            // panel_Dest
            // 
            this.panel_Dest.AutoScroll = true;
            this.panel_Dest.Location = new System.Drawing.Point(12, 480);
            this.panel_Dest.Name = "panel_Dest";
            this.panel_Dest.Size = new System.Drawing.Size(433, 305);
            this.panel_Dest.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 811);
            this.Controls.Add(this.panel_Dest);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_savefiepath);
            this.Controls.Add(this.textBox_OriginX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_OriginY);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox_Dest);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_loadfiepath);
            this.Controls.Add(this.textBox_angle);
            this.Controls.Add(this.pictureBox_Source);
            this.Controls.Add(this.button_draw);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Source)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Dest)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_draw;
        private System.Windows.Forms.PictureBox pictureBox_Source;
        private System.Windows.Forms.TextBox textBox_angle;
        private System.Windows.Forms.TextBox textBox_loadfiepath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.PictureBox pictureBox_Dest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_OriginY;
        private System.Windows.Forms.TextBox textBox_OriginX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_savefiepath;
        private System.Windows.Forms.Panel panel_Dest;
    }
}

