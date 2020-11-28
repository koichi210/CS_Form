namespace Encoder
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
            this.radioButton_Utf8ToSjis = new System.Windows.Forms.RadioButton();
            this.radioButton_SjisToUtf8 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.DropBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // radioButton_Utf8ToSjis
            // 
            this.radioButton_Utf8ToSjis.AutoSize = true;
            this.radioButton_Utf8ToSjis.Location = new System.Drawing.Point(22, 28);
            this.radioButton_Utf8ToSjis.Name = "radioButton_Utf8ToSjis";
            this.radioButton_Utf8ToSjis.Size = new System.Drawing.Size(96, 16);
            this.radioButton_Utf8ToSjis.TabIndex = 1;
            this.radioButton_Utf8ToSjis.TabStop = true;
            this.radioButton_Utf8ToSjis.Text = "UTF-8 ⇒ Sjis";
            this.radioButton_Utf8ToSjis.UseVisualStyleBackColor = true;
            // 
            // radioButton_SjisToUtf8
            // 
            this.radioButton_SjisToUtf8.AutoSize = true;
            this.radioButton_SjisToUtf8.Location = new System.Drawing.Point(22, 50);
            this.radioButton_SjisToUtf8.Name = "radioButton_SjisToUtf8";
            this.radioButton_SjisToUtf8.Size = new System.Drawing.Size(96, 16);
            this.radioButton_SjisToUtf8.TabIndex = 2;
            this.radioButton_SjisToUtf8.TabStop = true;
            this.radioButton_SjisToUtf8.Text = "Sjis ⇒ UTF-8";
            this.radioButton_SjisToUtf8.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "文字コード変換";
            // 
            // DropBox
            // 
            this.DropBox.AllowDrop = true;
            this.DropBox.Location = new System.Drawing.Point(134, 12);
            this.DropBox.Multiline = true;
            this.DropBox.Name = "DropBox";
            this.DropBox.ReadOnly = true;
            this.DropBox.Size = new System.Drawing.Size(94, 54);
            this.DropBox.TabIndex = 3;
            this.DropBox.Text = "pls drop me!!";
            this.DropBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.DropBox_DragDrop);
            this.DropBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.DropBox_DragEnter);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 77);
            this.Controls.Add(this.DropBox);
            this.Controls.Add(this.radioButton_SjisToUtf8);
            this.Controls.Add(this.radioButton_Utf8ToSjis);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(256, 115);
            this.MinimumSize = new System.Drawing.Size(256, 115);
            this.Name = "Form1";
            this.Text = "EncordDecord";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButton_Utf8ToSjis;
        private System.Windows.Forms.RadioButton radioButton_SjisToUtf8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DropBox;
    }
}

