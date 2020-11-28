namespace DrawImage
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
            this.button_DrawLine = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button_DrawLine2 = new System.Windows.Forms.Button();
            this.button_Circle = new System.Windows.Forms.Button();
            this.button_Delete = new System.Windows.Forms.Button();
            this.button_Circle2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_DrawLine
            // 
            this.button_DrawLine.Location = new System.Drawing.Point(197, 12);
            this.button_DrawLine.Name = "button_DrawLine";
            this.button_DrawLine.Size = new System.Drawing.Size(75, 23);
            this.button_DrawLine.TabIndex = 0;
            this.button_DrawLine.Text = "青線描画";
            this.button_DrawLine.UseVisualStyleBackColor = true;
            this.button_DrawLine.Click += new System.EventHandler(this.button_DrawLine_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(179, 238);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // button_DrawLine2
            // 
            this.button_DrawLine2.Location = new System.Drawing.Point(197, 41);
            this.button_DrawLine2.Name = "button_DrawLine2";
            this.button_DrawLine2.Size = new System.Drawing.Size(75, 23);
            this.button_DrawLine2.TabIndex = 2;
            this.button_DrawLine2.Text = "赤線描画";
            this.button_DrawLine2.UseVisualStyleBackColor = true;
            this.button_DrawLine2.Click += new System.EventHandler(this.button_DrawLine2_Click);
            // 
            // button_Circle
            // 
            this.button_Circle.Location = new System.Drawing.Point(197, 77);
            this.button_Circle.Name = "button_Circle";
            this.button_Circle.Size = new System.Drawing.Size(75, 23);
            this.button_Circle.TabIndex = 3;
            this.button_Circle.Text = "白円描画";
            this.button_Circle.UseVisualStyleBackColor = true;
            this.button_Circle.Click += new System.EventHandler(this.button_Circle_Click);
            // 
            // button_Delete
            // 
            this.button_Delete.Location = new System.Drawing.Point(197, 144);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(75, 23);
            this.button_Delete.TabIndex = 4;
            this.button_Delete.Text = "削除";
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // button_Circle2
            // 
            this.button_Circle2.Location = new System.Drawing.Point(197, 106);
            this.button_Circle2.Name = "button_Circle2";
            this.button_Circle2.Size = new System.Drawing.Size(75, 23);
            this.button_Circle2.TabIndex = 5;
            this.button_Circle2.Text = "黒円描画";
            this.button_Circle2.UseVisualStyleBackColor = true;
            this.button_Circle2.Click += new System.EventHandler(this.button_Circle2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.button_Circle2);
            this.Controls.Add(this.button_Delete);
            this.Controls.Add(this.button_Circle);
            this.Controls.Add(this.button_DrawLine2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button_DrawLine);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_DrawLine;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button_DrawLine2;
        private System.Windows.Forms.Button button_Circle;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.Button button_Circle2;
    }
}

