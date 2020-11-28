namespace DeleteDuplicateElement
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Source = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Dest = new System.Windows.Forms.TextBox();
            this.button_Execute = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "重複削除";
            // 
            // textBox_Source
            // 
            this.textBox_Source.Location = new System.Drawing.Point(14, 24);
            this.textBox_Source.Multiline = true;
            this.textBox_Source.Name = "textBox_Source";
            this.textBox_Source.Size = new System.Drawing.Size(178, 140);
            this.textBox_Source.TabIndex = 1;
            this.textBox_Source.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_Source_DragDrop);
            this.textBox_Source.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox_Source_DragEnter);
            this.textBox_Source.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Source_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(200, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "⇒";
            // 
            // textBox_Dest
            // 
            this.textBox_Dest.Location = new System.Drawing.Point(221, 24);
            this.textBox_Dest.Multiline = true;
            this.textBox_Dest.Name = "textBox_Dest";
            this.textBox_Dest.Size = new System.Drawing.Size(178, 140);
            this.textBox_Dest.TabIndex = 3;
            this.textBox_Dest.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Dest_KeyDown);
            // 
            // button_Execute
            // 
            this.button_Execute.Location = new System.Drawing.Point(324, 170);
            this.button_Execute.Name = "button_Execute";
            this.button_Execute.Size = new System.Drawing.Size(75, 23);
            this.button_Execute.TabIndex = 4;
            this.button_Execute.Text = "実行";
            this.button_Execute.UseVisualStyleBackColor = true;
            this.button_Execute.Click += new System.EventHandler(this.button_Execute_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 201);
            this.Controls.Add(this.button_Execute);
            this.Controls.Add(this.textBox_Dest);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Source);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(427, 239);
            this.MinimumSize = new System.Drawing.Size(427, 239);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Source;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Dest;
        private System.Windows.Forms.Button button_Execute;
    }
}

