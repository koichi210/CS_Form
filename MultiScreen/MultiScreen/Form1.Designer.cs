namespace MouseTrainingWithMultiScreen
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
            this.buttonAllPopup = new System.Windows.Forms.Button();
            this.textBox_DlgNum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSequencePopup = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_ButtonName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonAllPopup
            // 
            this.buttonAllPopup.Location = new System.Drawing.Point(14, 52);
            this.buttonAllPopup.Name = "buttonAllPopup";
            this.buttonAllPopup.Size = new System.Drawing.Size(192, 23);
            this.buttonAllPopup.TabIndex = 4;
            this.buttonAllPopup.Text = "一気に表示";
            this.buttonAllPopup.UseVisualStyleBackColor = true;
            this.buttonAllPopup.Click += new System.EventHandler(this.buttonAllPopup_Click);
            // 
            // textBox_DlgNum
            // 
            this.textBox_DlgNum.Location = new System.Drawing.Point(28, 27);
            this.textBox_DlgNum.Name = "textBox_DlgNum";
            this.textBox_DlgNum.Size = new System.Drawing.Size(52, 19);
            this.textBox_DlgNum.TabIndex = 2;
            this.textBox_DlgNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ダイアログ数：";
            // 
            // buttonSequencePopup
            // 
            this.buttonSequencePopup.Location = new System.Drawing.Point(14, 81);
            this.buttonSequencePopup.Name = "buttonSequencePopup";
            this.buttonSequencePopup.Size = new System.Drawing.Size(192, 23);
            this.buttonSequencePopup.TabIndex = 5;
            this.buttonSequencePopup.Text = "順次表示";
            this.buttonSequencePopup.UseVisualStyleBackColor = true;
            this.buttonSequencePopup.Click += new System.EventHandler(this.buttonSequencePopup_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(103, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "ボタン名：";
            // 
            // textBox_ButtonName
            // 
            this.textBox_ButtonName.Location = new System.Drawing.Point(105, 27);
            this.textBox_ButtonName.Name = "textBox_ButtonName";
            this.textBox_ButtonName.Size = new System.Drawing.Size(101, 19);
            this.textBox_ButtonName.TabIndex = 3;
            this.textBox_ButtonName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 116);
            this.Controls.Add(this.textBox_ButtonName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonSequencePopup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_DlgNum);
            this.Controls.Add(this.buttonAllPopup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAllPopup;
        private System.Windows.Forms.TextBox textBox_DlgNum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSequencePopup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_ButtonName;
    }
}

