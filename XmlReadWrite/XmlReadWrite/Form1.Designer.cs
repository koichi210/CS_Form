namespace XmlReadWrite
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
            this.textBox_FileName = new System.Windows.Forms.TextBox();
            this.button_read1 = new System.Windows.Forms.Button();
            this.button_write = new System.Windows.Forms.Button();
            this.textBox_Param1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Param2 = new System.Windows.Forms.TextBox();
            this.textBox_Param3 = new System.Windows.Forms.TextBox();
            this.textBox_Param4 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_read2 = new System.Windows.Forms.Button();
            this.button_read3 = new System.Windows.Forms.Button();
            this.button_read4 = new System.Windows.Forms.Button();
            this.button_write2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_FileName
            // 
            this.textBox_FileName.Location = new System.Drawing.Point(13, 24);
            this.textBox_FileName.Name = "textBox_FileName";
            this.textBox_FileName.Size = new System.Drawing.Size(195, 19);
            this.textBox_FileName.TabIndex = 1;
            // 
            // button_read1
            // 
            this.button_read1.Location = new System.Drawing.Point(120, 203);
            this.button_read1.Name = "button_read1";
            this.button_read1.Size = new System.Drawing.Size(88, 23);
            this.button_read1.TabIndex = 9;
            this.button_read1.Text = "Read1";
            this.button_read1.UseVisualStyleBackColor = true;
            this.button_read1.Click += new System.EventHandler(this.button_read_Click);
            // 
            // button_write
            // 
            this.button_write.Location = new System.Drawing.Point(26, 203);
            this.button_write.Name = "button_write";
            this.button_write.Size = new System.Drawing.Size(88, 23);
            this.button_write.TabIndex = 8;
            this.button_write.Text = "Write";
            this.button_write.UseVisualStyleBackColor = true;
            this.button_write.Click += new System.EventHandler(this.button_write_Click);
            // 
            // textBox_Param1
            // 
            this.textBox_Param1.Location = new System.Drawing.Point(14, 73);
            this.textBox_Param1.Name = "textBox_Param1";
            this.textBox_Param1.Size = new System.Drawing.Size(194, 19);
            this.textBox_Param1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Setting";
            // 
            // textBox_Param2
            // 
            this.textBox_Param2.Location = new System.Drawing.Point(14, 98);
            this.textBox_Param2.Name = "textBox_Param2";
            this.textBox_Param2.Size = new System.Drawing.Size(194, 19);
            this.textBox_Param2.TabIndex = 4;
            // 
            // textBox_Param3
            // 
            this.textBox_Param3.Location = new System.Drawing.Point(14, 123);
            this.textBox_Param3.Name = "textBox_Param3";
            this.textBox_Param3.Size = new System.Drawing.Size(194, 19);
            this.textBox_Param3.TabIndex = 5;
            // 
            // textBox_Param4
            // 
            this.textBox_Param4.Location = new System.Drawing.Point(14, 148);
            this.textBox_Param4.Name = "textBox_Param4";
            this.textBox_Param4.Size = new System.Drawing.Size(194, 19);
            this.textBox_Param4.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Xml FilePath";
            // 
            // button_read2
            // 
            this.button_read2.Location = new System.Drawing.Point(120, 232);
            this.button_read2.Name = "button_read2";
            this.button_read2.Size = new System.Drawing.Size(88, 23);
            this.button_read2.TabIndex = 10;
            this.button_read2.Text = "Read2";
            this.button_read2.UseVisualStyleBackColor = true;
            this.button_read2.Click += new System.EventHandler(this.button_read2_Click);
            // 
            // button_read3
            // 
            this.button_read3.Location = new System.Drawing.Point(120, 261);
            this.button_read3.Name = "button_read3";
            this.button_read3.Size = new System.Drawing.Size(88, 23);
            this.button_read3.TabIndex = 11;
            this.button_read3.Text = "Read3";
            this.button_read3.UseVisualStyleBackColor = true;
            this.button_read3.Click += new System.EventHandler(this.button_read3_Click);
            // 
            // button_read4
            // 
            this.button_read4.Location = new System.Drawing.Point(120, 325);
            this.button_read4.Name = "button_read4";
            this.button_read4.Size = new System.Drawing.Size(88, 23);
            this.button_read4.TabIndex = 14;
            this.button_read4.Text = "Read4";
            this.button_read4.UseVisualStyleBackColor = true;
            this.button_read4.Click += new System.EventHandler(this.button_read4_Click);
            // 
            // button_write2
            // 
            this.button_write2.Location = new System.Drawing.Point(26, 325);
            this.button_write2.Name = "button_write2";
            this.button_write2.Size = new System.Drawing.Size(88, 23);
            this.button_write2.TabIndex = 13;
            this.button_write2.Text = "Write2";
            this.button_write2.UseVisualStyleBackColor = true;
            this.button_write2.Click += new System.EventHandler(this.button_write2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 188);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "別のアプローチ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 310);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "使えそう";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 360);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_write2);
            this.Controls.Add(this.button_read4);
            this.Controls.Add(this.button_read3);
            this.Controls.Add(this.button_read2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Param4);
            this.Controls.Add(this.textBox_Param3);
            this.Controls.Add(this.textBox_Param2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Param1);
            this.Controls.Add(this.button_write);
            this.Controls.Add(this.button_read1);
            this.Controls.Add(this.textBox_FileName);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_FileName;
        private System.Windows.Forms.Button button_read1;
        private System.Windows.Forms.Button button_write;
        private System.Windows.Forms.TextBox textBox_Param1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Param2;
        private System.Windows.Forms.TextBox textBox_Param3;
        private System.Windows.Forms.TextBox textBox_Param4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_read2;
        private System.Windows.Forms.Button button_read3;
        private System.Windows.Forms.Button button_read4;
        private System.Windows.Forms.Button button_write2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

