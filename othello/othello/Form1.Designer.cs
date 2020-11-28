namespace othello
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
            this.pictureBoxField = new System.Windows.Forms.PictureBox();
            this.button_ReStart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxField)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxField
            // 
            this.pictureBoxField.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxField.Name = "pictureBoxField";
            this.pictureBoxField.Size = new System.Drawing.Size(200, 200);
            this.pictureBoxField.TabIndex = 0;
            this.pictureBoxField.TabStop = false;
            // 
            // button_ReStart
            // 
            this.button_ReStart.Location = new System.Drawing.Point(137, 218);
            this.button_ReStart.Name = "button_ReStart";
            this.button_ReStart.Size = new System.Drawing.Size(75, 23);
            this.button_ReStart.TabIndex = 1;
            this.button_ReStart.Text = "ReStart";
            this.button_ReStart.UseVisualStyleBackColor = true;
            this.button_ReStart.Click += new System.EventHandler(this.button_ReStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 250);
            this.Controls.Add(this.button_ReStart);
            this.Controls.Add(this.pictureBoxField);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxField)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_ReStart;
        public System.Windows.Forms.PictureBox pictureBoxField;
    }
}

