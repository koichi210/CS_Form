namespace Mailer
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_BrowserPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_MailBody = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_MailSubject = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_MailBcc = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_MailCc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_MailTo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_OpenBrowse = new System.Windows.Forms.Button();
            this.comboBox_LoadSetting = new System.Windows.Forms.ComboBox();
            this.button_SaveSetting = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBox_BrowserPath);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(20, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBox1.Size = new System.Drawing.Size(1030, 81);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Windows設定";
            // 
            // textBox_BrowserPath
            // 
            this.textBox_BrowserPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_BrowserPath.Location = new System.Drawing.Point(132, 30);
            this.textBox_BrowserPath.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox_BrowserPath.Name = "textBox_BrowserPath";
            this.textBox_BrowserPath.Size = new System.Drawing.Size(871, 25);
            this.textBox_BrowserPath.TabIndex = 2;
            this.textBox_BrowserPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_BrowserPath_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "ブラウザパス：";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.textBox_MailBody);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBox_MailSubject);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox_MailBcc);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBox_MailCc);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBox_MailTo);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(20, 108);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBox2.Size = new System.Drawing.Size(1030, 447);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "メール設定";
            // 
            // textBox_MailBody
            // 
            this.textBox_MailBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailBody.Location = new System.Drawing.Point(132, 198);
            this.textBox_MailBody.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox_MailBody.Multiline = true;
            this.textBox_MailBody.Name = "textBox_MailBody";
            this.textBox_MailBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_MailBody.Size = new System.Drawing.Size(871, 220);
            this.textBox_MailBody.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(70, 202);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 18);
            this.label6.TabIndex = 8;
            this.label6.Text = "本文：";
            // 
            // textBox_MailSubject
            // 
            this.textBox_MailSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailSubject.Location = new System.Drawing.Point(132, 152);
            this.textBox_MailSubject.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox_MailSubject.Name = "textBox_MailSubject";
            this.textBox_MailSubject.Size = new System.Drawing.Size(871, 25);
            this.textBox_MailSubject.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(70, 156);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 18);
            this.label5.TabIndex = 6;
            this.label5.Text = "宛先：";
            // 
            // textBox_MailBcc
            // 
            this.textBox_MailBcc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailBcc.Location = new System.Drawing.Point(132, 105);
            this.textBox_MailBcc.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox_MailBcc.Name = "textBox_MailBcc";
            this.textBox_MailBcc.Size = new System.Drawing.Size(871, 25);
            this.textBox_MailBcc.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(77, 110);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 18);
            this.label4.TabIndex = 4;
            this.label4.Text = "Bcc：";
            // 
            // textBox_MailCc
            // 
            this.textBox_MailCc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailCc.Location = new System.Drawing.Point(132, 68);
            this.textBox_MailCc.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox_MailCc.Name = "textBox_MailCc";
            this.textBox_MailCc.Size = new System.Drawing.Size(871, 25);
            this.textBox_MailCc.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 72);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Cc：";
            // 
            // textBox_MailTo
            // 
            this.textBox_MailTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailTo.Location = new System.Drawing.Point(132, 30);
            this.textBox_MailTo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox_MailTo.Name = "textBox_MailTo";
            this.textBox_MailTo.Size = new System.Drawing.Size(871, 25);
            this.textBox_MailTo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "To：";
            // 
            // button_OpenBrowse
            // 
            this.button_OpenBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_OpenBrowse.Location = new System.Drawing.Point(907, 564);
            this.button_OpenBrowse.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.button_OpenBrowse.Name = "button_OpenBrowse";
            this.button_OpenBrowse.Size = new System.Drawing.Size(143, 34);
            this.button_OpenBrowse.TabIndex = 2;
            this.button_OpenBrowse.Text = "ブラウザで表示";
            this.button_OpenBrowse.UseVisualStyleBackColor = true;
            this.button_OpenBrowse.Click += new System.EventHandler(this.button_OpenBrowse_Click);
            // 
            // comboBox_LoadSetting
            // 
            this.comboBox_LoadSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_LoadSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_LoadSetting.FormattingEnabled = true;
            this.comboBox_LoadSetting.Location = new System.Drawing.Point(20, 606);
            this.comboBox_LoadSetting.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.comboBox_LoadSetting.Name = "comboBox_LoadSetting";
            this.comboBox_LoadSetting.Size = new System.Drawing.Size(879, 26);
            this.comboBox_LoadSetting.TabIndex = 3;
            this.comboBox_LoadSetting.SelectedIndexChanged += new System.EventHandler(this.comboBox_LoadSetting_SelectedIndexChanged);
            // 
            // button_SaveSetting
            // 
            this.button_SaveSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveSetting.Location = new System.Drawing.Point(907, 603);
            this.button_SaveSetting.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.button_SaveSetting.Name = "button_SaveSetting";
            this.button_SaveSetting.Size = new System.Drawing.Size(143, 34);
            this.button_SaveSetting.TabIndex = 4;
            this.button_SaveSetting.Text = "設定保存";
            this.button_SaveSetting.UseVisualStyleBackColor = true;
            this.button_SaveSetting.Click += new System.EventHandler(this.button_SaveSetting_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 654);
            this.Controls.Add(this.button_SaveSetting);
            this.Controls.Add(this.comboBox_LoadSetting);
            this.Controls.Add(this.button_OpenBrowse);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MinimumSize = new System.Drawing.Size(1085, 684);
            this.Name = "Form1";
            this.Text = "Mailer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_OpenBrowse;
        private System.Windows.Forms.ComboBox comboBox_LoadSetting;
        private System.Windows.Forms.Button button_SaveSetting;
        public System.Windows.Forms.TextBox textBox_BrowserPath;
        public System.Windows.Forms.TextBox textBox_MailTo;
        public System.Windows.Forms.TextBox textBox_MailBody;
        public System.Windows.Forms.TextBox textBox_MailSubject;
        public System.Windows.Forms.TextBox textBox_MailBcc;
        public System.Windows.Forms.TextBox textBox_MailCc;
    }
}

