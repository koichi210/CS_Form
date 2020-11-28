namespace CaptureWindow
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
            this.Button_Capture = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_SavePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBox_MouseX = new System.Windows.Forms.TextBox();
            this.TextBox_MouseY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Radio_FullScreen = new System.Windows.Forms.RadioButton();
            this.Radio_CurrentScreen = new System.Windows.Forms.RadioButton();
            this.Radio_CurrentWindow = new System.Windows.Forms.RadioButton();
            this.TextBox_MousePoint = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TextBox_Sleep = new System.Windows.Forms.TextBox();
            this.SaveSetting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Button_Capture
            // 
            this.Button_Capture.Location = new System.Drawing.Point(12, 196);
            this.Button_Capture.Name = "Button_Capture";
            this.Button_Capture.Size = new System.Drawing.Size(219, 23);
            this.Button_Capture.TabIndex = 20;
            this.Button_Capture.Text = "Capture";
            this.Button_Capture.UseVisualStyleBackColor = true;
            this.Button_Capture.Click += new System.EventHandler(this.Button_Capture_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "保存フォルダパス：";
            // 
            // TextBox_SavePath
            // 
            this.TextBox_SavePath.Location = new System.Drawing.Point(12, 28);
            this.TextBox_SavePath.Name = "TextBox_SavePath";
            this.TextBox_SavePath.Size = new System.Drawing.Size(219, 19);
            this.TextBox_SavePath.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(134, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "クリックする座標：";
            // 
            // TextBox_MouseX
            // 
            this.TextBox_MouseX.Location = new System.Drawing.Point(168, 78);
            this.TextBox_MouseX.Name = "TextBox_MouseX";
            this.TextBox_MouseX.Size = new System.Drawing.Size(50, 19);
            this.TextBox_MouseX.TabIndex = 10;
            // 
            // TextBox_MouseY
            // 
            this.TextBox_MouseY.Location = new System.Drawing.Point(168, 103);
            this.TextBox_MouseY.Name = "TextBox_MouseY";
            this.TextBox_MouseY.Size = new System.Drawing.Size(50, 19);
            this.TextBox_MouseY.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "X：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(145, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Y：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "Capture対象：";
            // 
            // Radio_FullScreen
            // 
            this.Radio_FullScreen.AutoSize = true;
            this.Radio_FullScreen.Location = new System.Drawing.Point(28, 81);
            this.Radio_FullScreen.Name = "Radio_FullScreen";
            this.Radio_FullScreen.Size = new System.Drawing.Size(77, 16);
            this.Radio_FullScreen.TabIndex = 2;
            this.Radio_FullScreen.TabStop = true;
            this.Radio_FullScreen.Text = "FullScreen";
            this.Radio_FullScreen.UseVisualStyleBackColor = true;
            // 
            // Radio_CurrentScreen
            // 
            this.Radio_CurrentScreen.AutoSize = true;
            this.Radio_CurrentScreen.Location = new System.Drawing.Point(28, 102);
            this.Radio_CurrentScreen.Name = "Radio_CurrentScreen";
            this.Radio_CurrentScreen.Size = new System.Drawing.Size(96, 16);
            this.Radio_CurrentScreen.TabIndex = 3;
            this.Radio_CurrentScreen.TabStop = true;
            this.Radio_CurrentScreen.Text = "CurrentScreen";
            this.Radio_CurrentScreen.UseVisualStyleBackColor = true;
            // 
            // Radio_CurrentWindow
            // 
            this.Radio_CurrentWindow.AutoSize = true;
            this.Radio_CurrentWindow.Location = new System.Drawing.Point(28, 124);
            this.Radio_CurrentWindow.Name = "Radio_CurrentWindow";
            this.Radio_CurrentWindow.Size = new System.Drawing.Size(99, 16);
            this.Radio_CurrentWindow.TabIndex = 4;
            this.Radio_CurrentWindow.TabStop = true;
            this.Radio_CurrentWindow.Text = "CurrentWindow";
            this.Radio_CurrentWindow.UseVisualStyleBackColor = true;
            // 
            // TextBox_MousePoint
            // 
            this.TextBox_MousePoint.Location = new System.Drawing.Point(12, 225);
            this.TextBox_MousePoint.Name = "TextBox_MousePoint";
            this.TextBox_MousePoint.ReadOnly = true;
            this.TextBox_MousePoint.Size = new System.Drawing.Size(59, 19);
            this.TextBox_MousePoint.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(134, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "Sleep時間（Sec）：";
            // 
            // TextBox_Sleep
            // 
            this.TextBox_Sleep.Location = new System.Drawing.Point(168, 160);
            this.TextBox_Sleep.Name = "TextBox_Sleep";
            this.TextBox_Sleep.Size = new System.Drawing.Size(50, 19);
            this.TextBox_Sleep.TabIndex = 23;
            // 
            // SaveSetting
            // 
            this.SaveSetting.Location = new System.Drawing.Point(153, 225);
            this.SaveSetting.Name = "SaveSetting";
            this.SaveSetting.Size = new System.Drawing.Size(75, 23);
            this.SaveSetting.TabIndex = 24;
            this.SaveSetting.Text = "設定値保存";
            this.SaveSetting.UseVisualStyleBackColor = true;
            this.SaveSetting.Click += new System.EventHandler(this.SaveSetting_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 253);
            this.Controls.Add(this.SaveSetting);
            this.Controls.Add(this.TextBox_Sleep);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TextBox_MousePoint);
            this.Controls.Add(this.Radio_CurrentWindow);
            this.Controls.Add(this.Radio_CurrentScreen);
            this.Controls.Add(this.Radio_FullScreen);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TextBox_MouseY);
            this.Controls.Add(this.TextBox_MouseX);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBox_SavePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Button_Capture);
            this.Name = "Form1";
            this.Text = "CaptureWindow";
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_Capture;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_SavePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBox_MouseX;
        private System.Windows.Forms.TextBox TextBox_MouseY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton Radio_FullScreen;
        private System.Windows.Forms.RadioButton Radio_CurrentScreen;
        private System.Windows.Forms.RadioButton Radio_CurrentWindow;
        private System.Windows.Forms.TextBox TextBox_MousePoint;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TextBox_Sleep;
        private System.Windows.Forms.Button SaveSetting;
    }
}

