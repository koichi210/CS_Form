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
            this.dateTimePicker_Calendar = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox_CreateNum = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_OpenBrowse_OneWeek = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.button_Help = new System.Windows.Forms.Button();
            this.checkBoxReverse = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBox_BrowserPath);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(618, 54);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Windows設定";
            // 
            // textBox_BrowserPath
            // 
            this.textBox_BrowserPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_BrowserPath.Location = new System.Drawing.Point(79, 20);
            this.textBox_BrowserPath.Name = "textBox_BrowserPath";
            this.textBox_BrowserPath.Size = new System.Drawing.Size(524, 19);
            this.textBox_BrowserPath.TabIndex = 2;
            this.textBox_BrowserPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_BrowserPath_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
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
            this.groupBox2.Location = new System.Drawing.Point(12, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(618, 285);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "メール設定";
            // 
            // textBox_MailBody
            // 
            this.textBox_MailBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailBody.Location = new System.Drawing.Point(79, 132);
            this.textBox_MailBody.Multiline = true;
            this.textBox_MailBody.Name = "textBox_MailBody";
            this.textBox_MailBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_MailBody.Size = new System.Drawing.Size(524, 135);
            this.textBox_MailBody.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(42, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "本文：";
            // 
            // textBox_MailSubject
            // 
            this.textBox_MailSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailSubject.Location = new System.Drawing.Point(79, 101);
            this.textBox_MailSubject.Name = "textBox_MailSubject";
            this.textBox_MailSubject.Size = new System.Drawing.Size(524, 19);
            this.textBox_MailSubject.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "件名：";
            // 
            // textBox_MailBcc
            // 
            this.textBox_MailBcc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailBcc.Location = new System.Drawing.Point(79, 70);
            this.textBox_MailBcc.Name = "textBox_MailBcc";
            this.textBox_MailBcc.Size = new System.Drawing.Size(524, 19);
            this.textBox_MailBcc.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "Bcc：";
            // 
            // textBox_MailCc
            // 
            this.textBox_MailCc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailCc.Location = new System.Drawing.Point(79, 45);
            this.textBox_MailCc.Name = "textBox_MailCc";
            this.textBox_MailCc.Size = new System.Drawing.Size(524, 19);
            this.textBox_MailCc.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Cc：";
            // 
            // textBox_MailTo
            // 
            this.textBox_MailTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MailTo.Location = new System.Drawing.Point(79, 20);
            this.textBox_MailTo.Name = "textBox_MailTo";
            this.textBox_MailTo.Size = new System.Drawing.Size(524, 19);
            this.textBox_MailTo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "To：";
            // 
            // button_OpenBrowse
            // 
            this.button_OpenBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_OpenBrowse.Location = new System.Drawing.Point(544, 391);
            this.button_OpenBrowse.Name = "button_OpenBrowse";
            this.button_OpenBrowse.Size = new System.Drawing.Size(86, 23);
            this.button_OpenBrowse.TabIndex = 3;
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
            this.comboBox_LoadSetting.Location = new System.Drawing.Point(12, 445);
            this.comboBox_LoadSetting.Name = "comboBox_LoadSetting";
            this.comboBox_LoadSetting.Size = new System.Drawing.Size(529, 20);
            this.comboBox_LoadSetting.TabIndex = 5;
            this.comboBox_LoadSetting.SelectedIndexChanged += new System.EventHandler(this.comboBox_LoadSetting_SelectedIndexChanged);
            // 
            // button_SaveSetting
            // 
            this.button_SaveSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveSetting.Location = new System.Drawing.Point(544, 443);
            this.button_SaveSetting.Name = "button_SaveSetting";
            this.button_SaveSetting.Size = new System.Drawing.Size(86, 23);
            this.button_SaveSetting.TabIndex = 6;
            this.button_SaveSetting.Text = "設定保存";
            this.button_SaveSetting.UseVisualStyleBackColor = true;
            this.button_SaveSetting.Click += new System.EventHandler(this.button_SaveSetting_Click);
            // 
            // dateTimePicker_Calendar
            // 
            this.dateTimePicker_Calendar.Location = new System.Drawing.Point(78, 15);
            this.dateTimePicker_Calendar.Name = "dateTimePicker_Calendar";
            this.dateTimePicker_Calendar.Size = new System.Drawing.Size(142, 19);
            this.dateTimePicker_Calendar.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.checkBoxReverse);
            this.groupBox3.Controls.Add(this.textBox_CreateNum);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.button_OpenBrowse_OneWeek);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.dateTimePicker_Calendar);
            this.groupBox3.Location = new System.Drawing.Point(14, 369);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(524, 67);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "お試し機能";
            // 
            // textBox_CreateNum
            // 
            this.textBox_CreateNum.Location = new System.Drawing.Point(77, 40);
            this.textBox_CreateNum.Name = "textBox_CreateNum";
            this.textBox_CreateNum.Size = new System.Drawing.Size(71, 19);
            this.textBox_CreateNum.TabIndex = 3;
            this.textBox_CreateNum.Text = "5";
            this.textBox_CreateNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_CreateNum.WordWrap = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 43);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "生成数：";
            // 
            // button_OpenBrowse_OneWeek
            // 
            this.button_OpenBrowse_OneWeek.Location = new System.Drawing.Point(226, 15);
            this.button_OpenBrowse_OneWeek.Name = "button_OpenBrowse_OneWeek";
            this.button_OpenBrowse_OneWeek.Size = new System.Drawing.Size(292, 44);
            this.button_OpenBrowse_OneWeek.TabIndex = 4;
            this.button_OpenBrowse_OneWeek.Text = "一括表示";
            this.button_OpenBrowse_OneWeek.UseVisualStyleBackColor = true;
            this.button_OpenBrowse_OneWeek.Click += new System.EventHandler(this.button_OpenBrowse_OneWeek_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "起点日：";
            // 
            // button_Help
            // 
            this.button_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Help.Location = new System.Drawing.Point(544, 417);
            this.button_Help.Name = "button_Help";
            this.button_Help.Size = new System.Drawing.Size(86, 23);
            this.button_Help.TabIndex = 4;
            this.button_Help.Text = "ヘルプ";
            this.button_Help.UseVisualStyleBackColor = true;
            this.button_Help.Click += new System.EventHandler(this.button_Help_Click);
            // 
            // checkBoxReverse
            // 
            this.checkBoxReverse.AutoSize = true;
            this.checkBoxReverse.Checked = true;
            this.checkBoxReverse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxReverse.Location = new System.Drawing.Point(154, 42);
            this.checkBoxReverse.Name = "checkBoxReverse";
            this.checkBoxReverse.Size = new System.Drawing.Size(66, 16);
            this.checkBoxReverse.TabIndex = 5;
            this.checkBoxReverse.Text = "Reverse";
            this.checkBoxReverse.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 477);
            this.Controls.Add(this.button_Help);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_SaveSetting);
            this.Controls.Add(this.comboBox_LoadSetting);
            this.Controls.Add(this.button_OpenBrowse);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(657, 469);
            this.Name = "Form1";
            this.Text = "Mailer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.DateTimePicker dateTimePicker_Calendar;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button_OpenBrowse_OneWeek;
        private System.Windows.Forms.Button button_Help;
        public System.Windows.Forms.TextBox textBox_CreateNum;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxReverse;
    }
}

