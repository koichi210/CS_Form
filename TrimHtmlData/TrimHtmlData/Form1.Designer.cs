namespace TrimHtmlData
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
            this.textBox_SourceList = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_DestList = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_SearchWord = new System.Windows.Forms.TextBox();
            this.button_Execute = new System.Windows.Forms.Button();
            this.button_SaveSetting = new System.Windows.Forms.Button();
            this.textBox_DelimiterWord = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_TrimLineNum = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox_OrdinalCase = new System.Windows.Forms.CheckBox();
            this.checkBox_FirstWordOnly = new System.Windows.Forms.CheckBox();
            this.comboBox_LoadSetting = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textBox_SourceList
            // 
            this.textBox_SourceList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_SourceList.Location = new System.Drawing.Point(14, 24);
            this.textBox_SourceList.Multiline = true;
            this.textBox_SourceList.Name = "textBox_SourceList";
            this.textBox_SourceList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_SourceList.Size = new System.Drawing.Size(306, 229);
            this.textBox_SourceList.TabIndex = 1;
            this.textBox_SourceList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_SourceList_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "取得元";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(351, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "取得値";
            // 
            // textBox_DestList
            // 
            this.textBox_DestList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DestList.Location = new System.Drawing.Point(353, 24);
            this.textBox_DestList.Multiline = true;
            this.textBox_DestList.Name = "textBox_DestList";
            this.textBox_DestList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_DestList.Size = new System.Drawing.Size(350, 229);
            this.textBox_DestList.TabIndex = 3;
            this.textBox_DestList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_DestList_KeyDown);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 263);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "検索文字先頭：";
            // 
            // textBox_SearchWord
            // 
            this.textBox_SearchWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_SearchWord.Location = new System.Drawing.Point(101, 260);
            this.textBox_SearchWord.Name = "textBox_SearchWord";
            this.textBox_SearchWord.Size = new System.Drawing.Size(219, 19);
            this.textBox_SearchWord.TabIndex = 5;
            this.textBox_SearchWord.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_SearchWord_KeyDown);
            // 
            // button_Execute
            // 
            this.button_Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Execute.Location = new System.Drawing.Point(628, 259);
            this.button_Execute.Name = "button_Execute";
            this.button_Execute.Size = new System.Drawing.Size(75, 45);
            this.button_Execute.TabIndex = 13;
            this.button_Execute.Text = "実行";
            this.button_Execute.UseVisualStyleBackColor = true;
            this.button_Execute.Click += new System.EventHandler(this.button_Execute_Click);
            // 
            // button_SaveSetting
            // 
            this.button_SaveSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveSetting.Location = new System.Drawing.Point(628, 340);
            this.button_SaveSetting.Name = "button_SaveSetting";
            this.button_SaveSetting.Size = new System.Drawing.Size(75, 23);
            this.button_SaveSetting.TabIndex = 15;
            this.button_SaveSetting.Text = "設定保存";
            this.button_SaveSetting.UseVisualStyleBackColor = true;
            this.button_SaveSetting.Click += new System.EventHandler(this.button_SaveSetting_Click);
            // 
            // textBox_DelimiterWord
            // 
            this.textBox_DelimiterWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_DelimiterWord.Location = new System.Drawing.Point(101, 285);
            this.textBox_DelimiterWord.Name = "textBox_DelimiterWord";
            this.textBox_DelimiterWord.Size = new System.Drawing.Size(219, 19);
            this.textBox_DelimiterWord.TabIndex = 7;
            this.textBox_DelimiterWord.Visible = false;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 288);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "検索文字終端：";
            this.label4.Visible = false;
            // 
            // textBox_TrimLineNum
            // 
            this.textBox_TrimLineNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_TrimLineNum.Location = new System.Drawing.Point(416, 259);
            this.textBox_TrimLineNum.Name = "textBox_TrimLineNum";
            this.textBox_TrimLineNum.Size = new System.Drawing.Size(98, 19);
            this.textBox_TrimLineNum.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(351, 263);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "取得行数：";
            // 
            // checkBox_OrdinalCase
            // 
            this.checkBox_OrdinalCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_OrdinalCase.AutoSize = true;
            this.checkBox_OrdinalCase.Location = new System.Drawing.Point(353, 304);
            this.checkBox_OrdinalCase.Name = "checkBox_OrdinalCase";
            this.checkBox_OrdinalCase.Size = new System.Drawing.Size(148, 16);
            this.checkBox_OrdinalCase.TabIndex = 11;
            this.checkBox_OrdinalCase.Text = "大文字小文字を区別する";
            this.checkBox_OrdinalCase.UseVisualStyleBackColor = true;
            // 
            // checkBox_FirstWordOnly
            // 
            this.checkBox_FirstWordOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_FirstWordOnly.AutoSize = true;
            this.checkBox_FirstWordOnly.Location = new System.Drawing.Point(353, 284);
            this.checkBox_FirstWordOnly.Name = "checkBox_FirstWordOnly";
            this.checkBox_FirstWordOnly.Size = new System.Drawing.Size(178, 16);
            this.checkBox_FirstWordOnly.TabIndex = 10;
            this.checkBox_FirstWordOnly.Text = "最初にヒットしたワードだけを抽出";
            this.checkBox_FirstWordOnly.UseVisualStyleBackColor = true;
            // 
            // comboBox_LoadSetting
            // 
            this.comboBox_LoadSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_LoadSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_LoadSetting.FormattingEnabled = true;
            this.comboBox_LoadSetting.Location = new System.Drawing.Point(14, 341);
            this.comboBox_LoadSetting.Name = "comboBox_LoadSetting";
            this.comboBox_LoadSetting.Size = new System.Drawing.Size(608, 20);
            this.comboBox_LoadSetting.TabIndex = 14;
            this.comboBox_LoadSetting.SelectedIndexChanged += new System.EventHandler(this.comboBox_LoadSetting_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 366);
            this.Controls.Add(this.comboBox_LoadSetting);
            this.Controls.Add(this.checkBox_FirstWordOnly);
            this.Controls.Add(this.checkBox_OrdinalCase);
            this.Controls.Add(this.textBox_TrimLineNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_DelimiterWord);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_SaveSetting);
            this.Controls.Add(this.button_Execute);
            this.Controls.Add(this.textBox_SearchWord);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_DestList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_SourceList);
            this.MinimumSize = new System.Drawing.Size(686, 405);
            this.Name = "Form1";
            this.Text = "TrimHtmlData";
            this.DoubleClick += new System.EventHandler(this.Form1_DoubleClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Execute;
        private System.Windows.Forms.Button button_SaveSetting;
        public System.Windows.Forms.TextBox textBox_SourceList;
        public System.Windows.Forms.TextBox textBox_DestList;
        public System.Windows.Forms.TextBox textBox_SearchWord;
        public System.Windows.Forms.TextBox textBox_DelimiterWord;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox textBox_TrimLineNum;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox checkBox_OrdinalCase;
        public System.Windows.Forms.CheckBox checkBox_FirstWordOnly;
        private System.Windows.Forms.ComboBox comboBox_LoadSetting;
    }
}

