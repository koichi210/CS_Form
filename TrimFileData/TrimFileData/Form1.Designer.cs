namespace TrimFileData
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
            this.textBox_SerchWordList = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_SerchResultList = new System.Windows.Forms.TextBox();
            this.button_Execute = new System.Windows.Forms.Button();
            this.button_SaveSetting = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_ReferencePath = new System.Windows.Forms.TextBox();
            this.checkBox_FirstWordOnly = new System.Windows.Forms.CheckBox();
            this.checkBox_OrdinalCase = new System.Windows.Forms.CheckBox();
            this.comboBox_LoadSetting = new System.Windows.Forms.ComboBox();
            this.textBox_SerchCommonWord = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_SerchWordList
            // 
            this.textBox_SerchWordList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_SerchWordList.Location = new System.Drawing.Point(17, 143);
            this.textBox_SerchWordList.Multiline = true;
            this.textBox_SerchWordList.Name = "textBox_SerchWordList";
            this.textBox_SerchWordList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_SerchWordList.Size = new System.Drawing.Size(210, 227);
            this.textBox_SerchWordList.TabIndex = 8;
            this.textBox_SerchWordList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_SourceList_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "リファレンスファイルパス";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(250, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "検索結果";
            // 
            // textBox_SerchResultList
            // 
            this.textBox_SerchResultList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_SerchResultList.Location = new System.Drawing.Point(252, 143);
            this.textBox_SerchResultList.Multiline = true;
            this.textBox_SerchResultList.Name = "textBox_SerchResultList";
            this.textBox_SerchResultList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_SerchResultList.Size = new System.Drawing.Size(210, 227);
            this.textBox_SerchResultList.TabIndex = 10;
            this.textBox_SerchResultList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_DestList_KeyDown);
            // 
            // button_Execute
            // 
            this.button_Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Execute.Location = new System.Drawing.Point(387, 376);
            this.button_Execute.Name = "button_Execute";
            this.button_Execute.Size = new System.Drawing.Size(75, 23);
            this.button_Execute.TabIndex = 11;
            this.button_Execute.Text = "実行";
            this.button_Execute.UseVisualStyleBackColor = true;
            this.button_Execute.Click += new System.EventHandler(this.button_Execute_Click);
            // 
            // button_SaveSetting
            // 
            this.button_SaveSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveSetting.Location = new System.Drawing.Point(387, 411);
            this.button_SaveSetting.Name = "button_SaveSetting";
            this.button_SaveSetting.Size = new System.Drawing.Size(75, 23);
            this.button_SaveSetting.TabIndex = 13;
            this.button_SaveSetting.Text = "設定保存";
            this.button_SaveSetting.UseVisualStyleBackColor = true;
            this.button_SaveSetting.Click += new System.EventHandler(this.button_SaveSetting_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 12);
            this.label7.TabIndex = 7;
            this.label7.Text = "検索ワード";
            // 
            // textBox_ReferencePath
            // 
            this.textBox_ReferencePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ReferencePath.Location = new System.Drawing.Point(129, 6);
            this.textBox_ReferencePath.Name = "textBox_ReferencePath";
            this.textBox_ReferencePath.Size = new System.Drawing.Size(333, 19);
            this.textBox_ReferencePath.TabIndex = 1;
            this.textBox_ReferencePath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_ReferencePath_KeyDown);
            // 
            // checkBox_FirstWordOnly
            // 
            this.checkBox_FirstWordOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_FirstWordOnly.AutoSize = true;
            this.checkBox_FirstWordOnly.Location = new System.Drawing.Point(260, 22);
            this.checkBox_FirstWordOnly.Name = "checkBox_FirstWordOnly";
            this.checkBox_FirstWordOnly.Size = new System.Drawing.Size(178, 16);
            this.checkBox_FirstWordOnly.TabIndex = 5;
            this.checkBox_FirstWordOnly.Text = "最初にヒットしたワードだけを抽出";
            this.checkBox_FirstWordOnly.UseVisualStyleBackColor = true;
            // 
            // checkBox_OrdinalCase
            // 
            this.checkBox_OrdinalCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_OrdinalCase.AutoSize = true;
            this.checkBox_OrdinalCase.Location = new System.Drawing.Point(260, 44);
            this.checkBox_OrdinalCase.Name = "checkBox_OrdinalCase";
            this.checkBox_OrdinalCase.Size = new System.Drawing.Size(148, 16);
            this.checkBox_OrdinalCase.TabIndex = 6;
            this.checkBox_OrdinalCase.Text = "大文字小文字を区別する";
            this.checkBox_OrdinalCase.UseVisualStyleBackColor = true;
            // 
            // comboBox_LoadSetting
            // 
            this.comboBox_LoadSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_LoadSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_LoadSetting.FormattingEnabled = true;
            this.comboBox_LoadSetting.Location = new System.Drawing.Point(18, 412);
            this.comboBox_LoadSetting.Name = "comboBox_LoadSetting";
            this.comboBox_LoadSetting.Size = new System.Drawing.Size(363, 20);
            this.comboBox_LoadSetting.TabIndex = 12;
            this.comboBox_LoadSetting.SelectedIndexChanged += new System.EventHandler(this.comboBox_LoadSetting_SelectedIndexChanged);
            // 
            // textBox_SerchCommonWord
            // 
            this.textBox_SerchCommonWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_SerchCommonWord.Location = new System.Drawing.Point(8, 37);
            this.textBox_SerchCommonWord.Name = "textBox_SerchCommonWord";
            this.textBox_SerchCommonWord.Size = new System.Drawing.Size(231, 19);
            this.textBox_SerchCommonWord.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "共通ワード";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBox_SerchCommonWord);
            this.groupBox1.Controls.Add(this.checkBox_OrdinalCase);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.checkBox_FirstWordOnly);
            this.groupBox1.Location = new System.Drawing.Point(18, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 74);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "設定";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 440);
            this.Controls.Add(this.comboBox_LoadSetting);
            this.Controls.Add(this.textBox_ReferencePath);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button_SaveSetting);
            this.Controls.Add(this.button_Execute);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_SerchResultList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_SerchWordList);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(350, 300);
            this.Name = "Form1";
            this.Text = "TrimFileData";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Execute;
        private System.Windows.Forms.Button button_SaveSetting;
        public System.Windows.Forms.TextBox textBox_SerchWordList;
        public System.Windows.Forms.TextBox textBox_SerchResultList;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox textBox_ReferencePath;
        public System.Windows.Forms.CheckBox checkBox_FirstWordOnly;
        public System.Windows.Forms.CheckBox checkBox_OrdinalCase;
        private System.Windows.Forms.ComboBox comboBox_LoadSetting;
        public System.Windows.Forms.TextBox textBox_SerchCommonWord;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

