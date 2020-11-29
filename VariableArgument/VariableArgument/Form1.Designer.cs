namespace VariableArgument
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
            this.button_Exceute_CS = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Output = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Input = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_Exceute_C = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Replace_Digit = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_Exceute_CS
            // 
            this.button_Exceute_CS.Location = new System.Drawing.Point(165, 150);
            this.button_Exceute_CS.Name = "button_Exceute_CS";
            this.button_Exceute_CS.Size = new System.Drawing.Size(100, 23);
            this.button_Exceute_CS.TabIndex = 8;
            this.button_Exceute_CS.Text = "C#風に実行";
            this.button_Exceute_CS.UseVisualStyleBackColor = true;
            this.button_Exceute_CS.Click += new System.EventHandler(this.button_Exceute_CS_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "入力文字列：";
            // 
            // textBox_Output
            // 
            this.textBox_Output.Location = new System.Drawing.Point(106, 98);
            this.textBox_Output.Name = "textBox_Output";
            this.textBox_Output.ReadOnly = true;
            this.textBox_Output.Size = new System.Drawing.Size(159, 19);
            this.textBox_Output.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "出力文字列：";
            // 
            // textBox_Input
            // 
            this.textBox_Input.Location = new System.Drawing.Point(106, 52);
            this.textBox_Input.Name = "textBox_Input";
            this.textBox_Input.Size = new System.Drawing.Size(159, 19);
            this.textBox_Input.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "%d を数値にして出力";
            // 
            // button_Exceute_C
            // 
            this.button_Exceute_C.Location = new System.Drawing.Point(165, 123);
            this.button_Exceute_C.Name = "button_Exceute_C";
            this.button_Exceute_C.Size = new System.Drawing.Size(100, 23);
            this.button_Exceute_C.TabIndex = 7;
            this.button_Exceute_C.Text = "C言語風に実行";
            this.button_Exceute_C.UseVisualStyleBackColor = true;
            this.button_Exceute_C.Click += new System.EventHandler(this.button_Exceute_C_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "置き換える値：";
            // 
            // textBox_Replace_Digit
            // 
            this.textBox_Replace_Digit.Location = new System.Drawing.Point(106, 76);
            this.textBox_Replace_Digit.Name = "textBox_Replace_Digit";
            this.textBox_Replace_Digit.Size = new System.Drawing.Size(159, 19);
            this.textBox_Replace_Digit.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 208);
            this.Controls.Add(this.textBox_Replace_Digit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_Exceute_C);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Input);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Output);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Exceute_CS);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Exceute_CS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Output;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Input;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Exceute_C;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_Replace_Digit;
    }
}

