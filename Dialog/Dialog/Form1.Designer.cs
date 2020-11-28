namespace Dialog
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
            this.button_static_modal = new System.Windows.Forms.Button();
            this.button_static_modeless = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_static_modal
            // 
            this.button_static_modal.Location = new System.Drawing.Point(12, 75);
            this.button_static_modal.Name = "button_static_modal";
            this.button_static_modal.Size = new System.Drawing.Size(233, 34);
            this.button_static_modal.TabIndex = 0;
            this.button_static_modal.Text = "static_button_modal";
            this.button_static_modal.UseVisualStyleBackColor = true;
            this.button_static_modal.Click += new System.EventHandler(this.button_static_modal_Click);
            // 
            // button_static_modeless
            // 
            this.button_static_modeless.Location = new System.Drawing.Point(12, 115);
            this.button_static_modeless.Name = "button_static_modeless";
            this.button_static_modeless.Size = new System.Drawing.Size(233, 34);
            this.button_static_modeless.TabIndex = 1;
            this.button_static_modeless.Text = "static_button_modeless";
            this.button_static_modeless.UseVisualStyleBackColor = true;
            this.button_static_modeless.Click += new System.EventHandler(this.button_static_modeless_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Controls.Add(this.button_static_modeless);
            this.Controls.Add(this.button_static_modal);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_static_modal;
        private System.Windows.Forms.Button button_static_modeless;
    }
}

