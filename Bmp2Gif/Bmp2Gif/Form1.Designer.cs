namespace Bmp2Gif
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
            this.textBox_SrcBmp = new System.Windows.Forms.TextBox();
            this.textBox_DstGif = new System.Windows.Forms.TextBox();
            this.button_Change = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxAddComent = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox_SrcBmp
            // 
            this.textBox_SrcBmp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_SrcBmp.Location = new System.Drawing.Point(78, 6);
            this.textBox_SrcBmp.Name = "textBox_SrcBmp";
            this.textBox_SrcBmp.Size = new System.Drawing.Size(246, 19);
            this.textBox_SrcBmp.TabIndex = 1;
            // 
            // textBox_DstGif
            // 
            this.textBox_DstGif.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DstGif.Location = new System.Drawing.Point(78, 49);
            this.textBox_DstGif.Name = "textBox_DstGif";
            this.textBox_DstGif.Size = new System.Drawing.Size(246, 19);
            this.textBox_DstGif.TabIndex = 4;
            // 
            // button_Change
            // 
            this.button_Change.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Change.Location = new System.Drawing.Point(249, 96);
            this.button_Change.Name = "button_Change";
            this.button_Change.Size = new System.Drawing.Size(75, 23);
            this.button_Change.TabIndex = 6;
            this.button_Change.Text = "変換";
            this.button_Change.UseVisualStyleBackColor = true;
            this.button_Change.Click += new System.EventHandler(this.button_Change_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "bmpファイル";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "gifファイル";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(142, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "↓";
            // 
            // checkBoxAddComent
            // 
            this.checkBoxAddComent.AutoSize = true;
            this.checkBoxAddComent.Location = new System.Drawing.Point(222, 74);
            this.checkBoxAddComent.Name = "checkBoxAddComent";
            this.checkBoxAddComent.Size = new System.Drawing.Size(100, 16);
            this.checkBoxAddComent.TabIndex = 5;
            this.checkBoxAddComent.Text = "コメント追加する";
            this.checkBoxAddComent.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 132);
            this.Controls.Add(this.checkBoxAddComent);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Change);
            this.Controls.Add(this.textBox_DstGif);
            this.Controls.Add(this.textBox_SrcBmp);
            this.MaximumSize = new System.Drawing.Size(3000, 170);
            this.MinimumSize = new System.Drawing.Size(300, 170);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_SrcBmp;
        private System.Windows.Forms.TextBox textBox_DstGif;
        private System.Windows.Forms.Button button_Change;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxAddComent;
    }
}

