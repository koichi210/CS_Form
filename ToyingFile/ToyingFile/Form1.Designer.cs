namespace ToyingFile
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_DeleteString = new System.Windows.Forms.TextBox();
            this.checkBox_WideNarrow = new System.Windows.Forms.CheckBox();
            this.checkBox_DeleteLine = new System.Windows.Forms.CheckBox();
            this.radioButton_DeleteString = new System.Windows.Forms.RadioButton();
            this.textBox_Directory = new System.Windows.Forms.TextBox();
            this.button_Execute = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_SubDirectory = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_File = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBox_DeleteString);
            this.groupBox1.Controls.Add(this.checkBox_WideNarrow);
            this.groupBox1.Controls.Add(this.checkBox_DeleteLine);
            this.groupBox1.Controls.Add(this.radioButton_DeleteString);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 144);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "メニュー";
            // 
            // textBox_DeleteString
            // 
            this.textBox_DeleteString.AllowDrop = true;
            this.textBox_DeleteString.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DeleteString.Location = new System.Drawing.Point(38, 41);
            this.textBox_DeleteString.Multiline = true;
            this.textBox_DeleteString.Name = "textBox_DeleteString";
            this.textBox_DeleteString.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_DeleteString.Size = new System.Drawing.Size(156, 47);
            this.textBox_DeleteString.TabIndex = 1;
            this.textBox_DeleteString.WordWrap = false;
            this.textBox_DeleteString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_DeleteString_KeyDown);
            // 
            // checkBox_WideNarrow
            // 
            this.checkBox_WideNarrow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_WideNarrow.AutoSize = true;
            this.checkBox_WideNarrow.Location = new System.Drawing.Point(38, 94);
            this.checkBox_WideNarrow.Name = "checkBox_WideNarrow";
            this.checkBox_WideNarrow.Size = new System.Drawing.Size(129, 16);
            this.checkBox_WideNarrow.TabIndex = 2;
            this.checkBox_WideNarrow.Text = "大文字小文字を区別";
            this.checkBox_WideNarrow.UseVisualStyleBackColor = true;
            // 
            // checkBox_DeleteLine
            // 
            this.checkBox_DeleteLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_DeleteLine.AutoSize = true;
            this.checkBox_DeleteLine.Location = new System.Drawing.Point(38, 113);
            this.checkBox_DeleteLine.Name = "checkBox_DeleteLine";
            this.checkBox_DeleteLine.Size = new System.Drawing.Size(69, 16);
            this.checkBox_DeleteLine.TabIndex = 3;
            this.checkBox_DeleteLine.Text = "行を削除";
            this.checkBox_DeleteLine.UseVisualStyleBackColor = true;
            // 
            // radioButton_DeleteString
            // 
            this.radioButton_DeleteString.AutoSize = true;
            this.radioButton_DeleteString.Location = new System.Drawing.Point(16, 22);
            this.radioButton_DeleteString.Name = "radioButton_DeleteString";
            this.radioButton_DeleteString.Size = new System.Drawing.Size(104, 16);
            this.radioButton_DeleteString.TabIndex = 0;
            this.radioButton_DeleteString.TabStop = true;
            this.radioButton_DeleteString.Text = "指定文字を削除";
            this.radioButton_DeleteString.UseVisualStyleBackColor = true;
            // 
            // textBox_Directory
            // 
            this.textBox_Directory.AllowDrop = true;
            this.textBox_Directory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Directory.Location = new System.Drawing.Point(80, 18);
            this.textBox_Directory.Name = "textBox_Directory";
            this.textBox_Directory.Size = new System.Drawing.Size(156, 19);
            this.textBox_Directory.TabIndex = 1;
            this.textBox_Directory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Directory_KeyDown);
            // 
            // button_Execute
            // 
            this.button_Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Execute.Location = new System.Drawing.Point(384, 162);
            this.button_Execute.Name = "button_Execute";
            this.button_Execute.Size = new System.Drawing.Size(75, 23);
            this.button_Execute.TabIndex = 2;
            this.button_Execute.Text = "実行";
            this.button_Execute.UseVisualStyleBackColor = true;
            this.button_Execute.Click += new System.EventHandler(this.button_Execute_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.checkBox_SubDirectory);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBox_File);
            this.groupBox2.Controls.Add(this.textBox_Directory);
            this.groupBox2.Location = new System.Drawing.Point(218, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(242, 144);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "対象ファイル";
            // 
            // checkBox_SubDirectory
            // 
            this.checkBox_SubDirectory.AutoSize = true;
            this.checkBox_SubDirectory.Checked = true;
            this.checkBox_SubDirectory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_SubDirectory.Location = new System.Drawing.Point(80, 66);
            this.checkBox_SubDirectory.Name = "checkBox_SubDirectory";
            this.checkBox_SubDirectory.Size = new System.Drawing.Size(125, 16);
            this.checkBox_SubDirectory.TabIndex = 4;
            this.checkBox_SubDirectory.Text = "サブディレクトリも対象";
            this.checkBox_SubDirectory.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "ファイル：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ディレクトリ：";
            // 
            // textBox_File
            // 
            this.textBox_File.AllowDrop = true;
            this.textBox_File.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_File.Location = new System.Drawing.Point(80, 41);
            this.textBox_File.Name = "textBox_File";
            this.textBox_File.Size = new System.Drawing.Size(156, 19);
            this.textBox_File.TabIndex = 3;
            this.textBox_File.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_File_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 189);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_Execute);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(487, 200);
            this.Name = "Form1";
            this.Text = "ToyingFile";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_Directory;
        private System.Windows.Forms.Button button_Execute;
        private System.Windows.Forms.RadioButton radioButton_DeleteString;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_WideNarrow;
        private System.Windows.Forms.CheckBox checkBox_SubDirectory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_File;
        private System.Windows.Forms.CheckBox checkBox_DeleteLine;
        private System.Windows.Forms.TextBox textBox_DeleteString;
    }
}

