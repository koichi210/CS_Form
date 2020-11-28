namespace Encryption
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
            this.textBox_Table = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton_Encode = new System.Windows.Forms.RadioButton();
            this.radioButton_Decode = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Key = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Result = new System.Windows.Forms.TextBox();
            this.button_Execute = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_Table
            // 
            this.textBox_Table.Location = new System.Drawing.Point(14, 24);
            this.textBox_Table.Name = "textBox_Table";
            this.textBox_Table.Size = new System.Drawing.Size(206, 19);
            this.textBox_Table.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "暗号テーブル";
            // 
            // radioButton_Encode
            // 
            this.radioButton_Encode.AutoSize = true;
            this.radioButton_Encode.Location = new System.Drawing.Point(43, 72);
            this.radioButton_Encode.Name = "radioButton_Encode";
            this.radioButton_Encode.Size = new System.Drawing.Size(59, 16);
            this.radioButton_Encode.TabIndex = 3;
            this.radioButton_Encode.TabStop = true;
            this.radioButton_Encode.Text = "暗号化";
            this.radioButton_Encode.UseVisualStyleBackColor = true;
            // 
            // radioButton_Decode
            // 
            this.radioButton_Decode.AutoSize = true;
            this.radioButton_Decode.Location = new System.Drawing.Point(130, 72);
            this.radioButton_Decode.Name = "radioButton_Decode";
            this.radioButton_Decode.Size = new System.Drawing.Size(59, 16);
            this.radioButton_Decode.TabIndex = 4;
            this.radioButton_Decode.TabStop = true;
            this.radioButton_Decode.Text = "複合化";
            this.radioButton_Decode.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "変換";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "キー";
            // 
            // textBox_Key
            // 
            this.textBox_Key.Location = new System.Drawing.Point(43, 114);
            this.textBox_Key.Name = "textBox_Key";
            this.textBox_Key.Size = new System.Drawing.Size(126, 19);
            this.textBox_Key.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "結果";
            // 
            // textBox_Result
            // 
            this.textBox_Result.Location = new System.Drawing.Point(43, 139);
            this.textBox_Result.Name = "textBox_Result";
            this.textBox_Result.ReadOnly = true;
            this.textBox_Result.Size = new System.Drawing.Size(126, 19);
            this.textBox_Result.TabIndex = 8;
            // 
            // button_Execute
            // 
            this.button_Execute.Location = new System.Drawing.Point(175, 112);
            this.button_Execute.Name = "button_Execute";
            this.button_Execute.Size = new System.Drawing.Size(45, 42);
            this.button_Execute.TabIndex = 9;
            this.button_Execute.Text = "実行";
            this.button_Execute.UseVisualStyleBackColor = true;
            this.button_Execute.Click += new System.EventHandler(this.button_Execute_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 168);
            this.Controls.Add(this.button_Execute);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_Result);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Key);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radioButton_Decode);
            this.Controls.Add(this.radioButton_Encode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Table);
            this.Name = "Form1";
            this.Text = "Encryption";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Table;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton_Encode;
        private System.Windows.Forms.RadioButton radioButton_Decode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Key;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_Result;
        private System.Windows.Forms.Button button_Execute;
    }
}

