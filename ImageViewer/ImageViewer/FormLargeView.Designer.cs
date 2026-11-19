namespace ImageViewer
{
    partial class FormLargeView
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
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
            this.checkBox_Fit = new System.Windows.Forms.CheckBox();
            this.label_Zoom = new System.Windows.Forms.Label();
            this.numericUpDown_Zoom = new System.Windows.Forms.NumericUpDown();
            this.label_Gamma = new System.Windows.Forms.Label();
            this.trackBar_Gamma = new System.Windows.Forms.TrackBar();
            this.label_GammaValue = new System.Windows.Forms.Label();
            this.button_GammaReset = new System.Windows.Forms.Button();
            this.button_SavePng = new System.Windows.Forms.Button();
            this.label_VideoPosition = new System.Windows.Forms.Label();
            this.textBox_VideoPosition = new System.Windows.Forms.TextBox();
            this.radioButton_Seconds = new System.Windows.Forms.RadioButton();
            this.radioButton_Frame = new System.Windows.Forms.RadioButton();
            this.label_Fps = new System.Windows.Forms.Label();
            this.textBox_Fps = new System.Windows.Forms.TextBox();
            this.button_ShowFrame = new System.Windows.Forms.Button();
            this.label_Status = new System.Windows.Forms.Label();
            this.panel_Image = new System.Windows.Forms.Panel();
            this.pictureBox_Image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Zoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Gamma)).BeginInit();
            this.panel_Image.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Image)).BeginInit();
            this.SuspendLayout();
            //
            // checkBox_Fit
            //
            this.checkBox_Fit.AutoSize = true;
            this.checkBox_Fit.Checked = true;
            this.checkBox_Fit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Fit.Location = new System.Drawing.Point(12, 10);
            this.checkBox_Fit.Name = "checkBox_Fit";
            this.checkBox_Fit.Size = new System.Drawing.Size(72, 16);
            this.checkBox_Fit.TabIndex = 0;
            this.checkBox_Fit.Text = "全体表示";
            this.checkBox_Fit.UseVisualStyleBackColor = true;
            this.checkBox_Fit.CheckedChanged += new System.EventHandler(this.checkBox_Fit_CheckedChanged);
            //
            // label_Zoom
            //
            this.label_Zoom.AutoSize = true;
            this.label_Zoom.Location = new System.Drawing.Point(90, 11);
            this.label_Zoom.Name = "label_Zoom";
            this.label_Zoom.Size = new System.Drawing.Size(56, 12);
            this.label_Zoom.TabIndex = 1;
            this.label_Zoom.Text = "倍率(%)：";
            //
            // numericUpDown_Zoom
            //
            this.numericUpDown_Zoom.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_Zoom.Location = new System.Drawing.Point(152, 8);
            this.numericUpDown_Zoom.Maximum = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.numericUpDown_Zoom.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_Zoom.Name = "numericUpDown_Zoom";
            this.numericUpDown_Zoom.Size = new System.Drawing.Size(60, 19);
            this.numericUpDown_Zoom.TabIndex = 2;
            this.numericUpDown_Zoom.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown_Zoom.ValueChanged += new System.EventHandler(this.numericUpDown_Zoom_ValueChanged);
            //
            // label_Gamma
            //
            this.label_Gamma.AutoSize = true;
            this.label_Gamma.Location = new System.Drawing.Point(226, 11);
            this.label_Gamma.Name = "label_Gamma";
            this.label_Gamma.Size = new System.Drawing.Size(42, 12);
            this.label_Gamma.TabIndex = 3;
            this.label_Gamma.Text = "ガンマ：";
            //
            // trackBar_Gamma
            //
            this.trackBar_Gamma.AutoSize = false;
            this.trackBar_Gamma.LargeChange = 5;
            this.trackBar_Gamma.Location = new System.Drawing.Point(270, 6);
            this.trackBar_Gamma.Maximum = 30;
            this.trackBar_Gamma.Minimum = 2;
            this.trackBar_Gamma.Name = "trackBar_Gamma";
            this.trackBar_Gamma.Size = new System.Drawing.Size(130, 24);
            this.trackBar_Gamma.TabIndex = 4;
            this.trackBar_Gamma.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Gamma.Value = 10;
            this.trackBar_Gamma.ValueChanged += new System.EventHandler(this.trackBar_Gamma_ValueChanged);
            //
            // label_GammaValue
            //
            this.label_GammaValue.AutoSize = true;
            this.label_GammaValue.Location = new System.Drawing.Point(402, 11);
            this.label_GammaValue.Name = "label_GammaValue";
            this.label_GammaValue.Size = new System.Drawing.Size(17, 12);
            this.label_GammaValue.TabIndex = 5;
            this.label_GammaValue.Text = "1.0";
            //
            // button_GammaReset
            //
            this.button_GammaReset.Location = new System.Drawing.Point(428, 6);
            this.button_GammaReset.Name = "button_GammaReset";
            this.button_GammaReset.Size = new System.Drawing.Size(44, 22);
            this.button_GammaReset.TabIndex = 6;
            this.button_GammaReset.Text = "戻す";
            this.button_GammaReset.UseVisualStyleBackColor = true;
            this.button_GammaReset.Click += new System.EventHandler(this.button_GammaReset_Click);
            //
            // button_SavePng
            //
            this.button_SavePng.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SavePng.Location = new System.Drawing.Point(548, 6);
            this.button_SavePng.Name = "button_SavePng";
            this.button_SavePng.Size = new System.Drawing.Size(80, 22);
            this.button_SavePng.TabIndex = 7;
            this.button_SavePng.Text = "PNGで保存";
            this.button_SavePng.UseVisualStyleBackColor = true;
            this.button_SavePng.Click += new System.EventHandler(this.button_SavePng_Click);
            //
            // label_VideoPosition
            //
            this.label_VideoPosition.AutoSize = true;
            this.label_VideoPosition.Location = new System.Drawing.Point(12, 39);
            this.label_VideoPosition.Name = "label_VideoPosition";
            this.label_VideoPosition.Size = new System.Drawing.Size(72, 12);
            this.label_VideoPosition.TabIndex = 8;
            this.label_VideoPosition.Text = "動画の位置：";
            //
            // textBox_VideoPosition
            //
            this.textBox_VideoPosition.Location = new System.Drawing.Point(90, 36);
            this.textBox_VideoPosition.Name = "textBox_VideoPosition";
            this.textBox_VideoPosition.Size = new System.Drawing.Size(60, 19);
            this.textBox_VideoPosition.TabIndex = 9;
            this.textBox_VideoPosition.Text = "0";
            this.textBox_VideoPosition.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_VideoPosition_KeyDown);
            //
            // radioButton_Seconds
            //
            this.radioButton_Seconds.AutoSize = true;
            this.radioButton_Seconds.Checked = true;
            this.radioButton_Seconds.Location = new System.Drawing.Point(156, 37);
            this.radioButton_Seconds.Name = "radioButton_Seconds";
            this.radioButton_Seconds.Size = new System.Drawing.Size(35, 16);
            this.radioButton_Seconds.TabIndex = 10;
            this.radioButton_Seconds.TabStop = true;
            this.radioButton_Seconds.Text = "秒";
            this.radioButton_Seconds.UseVisualStyleBackColor = true;
            //
            // radioButton_Frame
            //
            this.radioButton_Frame.AutoSize = true;
            this.radioButton_Frame.Location = new System.Drawing.Point(196, 37);
            this.radioButton_Frame.Name = "radioButton_Frame";
            this.radioButton_Frame.Size = new System.Drawing.Size(41, 16);
            this.radioButton_Frame.TabIndex = 11;
            this.radioButton_Frame.Text = "コマ";
            this.radioButton_Frame.UseVisualStyleBackColor = true;
            //
            // label_Fps
            //
            this.label_Fps.AutoSize = true;
            this.label_Fps.Location = new System.Drawing.Point(246, 39);
            this.label_Fps.Name = "label_Fps";
            this.label_Fps.Size = new System.Drawing.Size(28, 12);
            this.label_Fps.TabIndex = 12;
            this.label_Fps.Text = "fps：";
            //
            // textBox_Fps
            //
            this.textBox_Fps.Location = new System.Drawing.Point(280, 36);
            this.textBox_Fps.Name = "textBox_Fps";
            this.textBox_Fps.Size = new System.Drawing.Size(45, 19);
            this.textBox_Fps.TabIndex = 13;
            this.textBox_Fps.Text = "30";
            //
            // button_ShowFrame
            //
            this.button_ShowFrame.Location = new System.Drawing.Point(331, 34);
            this.button_ShowFrame.Name = "button_ShowFrame";
            this.button_ShowFrame.Size = new System.Drawing.Size(75, 22);
            this.button_ShowFrame.TabIndex = 14;
            this.button_ShowFrame.Text = "コマを表示";
            this.button_ShowFrame.UseVisualStyleBackColor = true;
            this.button_ShowFrame.Click += new System.EventHandler(this.button_ShowFrame_Click);
            //
            // label_Status
            //
            this.label_Status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Status.AutoEllipsis = true;
            this.label_Status.Location = new System.Drawing.Point(412, 39);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(216, 12);
            this.label_Status.TabIndex = 15;
            //
            // panel_Image
            //
            this.panel_Image.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_Image.AutoScroll = true;
            this.panel_Image.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel_Image.Controls.Add(this.pictureBox_Image);
            this.panel_Image.Location = new System.Drawing.Point(12, 62);
            this.panel_Image.Name = "panel_Image";
            this.panel_Image.Size = new System.Drawing.Size(616, 406);
            this.panel_Image.TabIndex = 16;
            //
            // pictureBox_Image
            //
            this.pictureBox_Image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_Image.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Image.Name = "pictureBox_Image";
            this.pictureBox_Image.Size = new System.Drawing.Size(616, 406);
            this.pictureBox_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Image.TabIndex = 0;
            this.pictureBox_Image.TabStop = false;
            //
            // FormLargeView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.panel_Image);
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.button_ShowFrame);
            this.Controls.Add(this.textBox_Fps);
            this.Controls.Add(this.label_Fps);
            this.Controls.Add(this.radioButton_Frame);
            this.Controls.Add(this.radioButton_Seconds);
            this.Controls.Add(this.textBox_VideoPosition);
            this.Controls.Add(this.label_VideoPosition);
            this.Controls.Add(this.button_SavePng);
            this.Controls.Add(this.button_GammaReset);
            this.Controls.Add(this.label_GammaValue);
            this.Controls.Add(this.trackBar_Gamma);
            this.Controls.Add(this.label_Gamma);
            this.Controls.Add(this.numericUpDown_Zoom);
            this.Controls.Add(this.label_Zoom);
            this.Controls.Add(this.checkBox_Fit);
            this.MinimumSize = new System.Drawing.Size(656, 300);
            this.Name = "FormLargeView";
            this.Text = "拡大表示";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Zoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Gamma)).EndInit();
            this.panel_Image.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox checkBox_Fit;
        private System.Windows.Forms.Label label_Zoom;
        public System.Windows.Forms.NumericUpDown numericUpDown_Zoom;
        private System.Windows.Forms.Label label_Gamma;
        public System.Windows.Forms.TrackBar trackBar_Gamma;
        private System.Windows.Forms.Label label_GammaValue;
        private System.Windows.Forms.Button button_GammaReset;
        private System.Windows.Forms.Button button_SavePng;
        private System.Windows.Forms.Label label_VideoPosition;
        public System.Windows.Forms.TextBox textBox_VideoPosition;
        public System.Windows.Forms.RadioButton radioButton_Seconds;
        public System.Windows.Forms.RadioButton radioButton_Frame;
        private System.Windows.Forms.Label label_Fps;
        public System.Windows.Forms.TextBox textBox_Fps;
        private System.Windows.Forms.Button button_ShowFrame;
        public System.Windows.Forms.Label label_Status;
        private System.Windows.Forms.Panel panel_Image;
        public System.Windows.Forms.PictureBox pictureBox_Image;
    }
}
