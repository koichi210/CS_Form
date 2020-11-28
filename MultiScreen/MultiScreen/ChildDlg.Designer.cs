namespace MouseTrainingWithMultiScreen
{
    partial class ChildDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ChildDlg_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ChildDlg_button
            // 
            this.ChildDlg_button.Location = new System.Drawing.Point(0, 0);
            this.ChildDlg_button.MinimumSize = new System.Drawing.Size(75, 23);
            this.ChildDlg_button.Name = "ChildDlg_button";
            this.ChildDlg_button.Size = new System.Drawing.Size(127, 23);
            this.ChildDlg_button.TabIndex = 0;
            this.ChildDlg_button.Text = "Unknown";
            this.ChildDlg_button.UseVisualStyleBackColor = true;
            this.ChildDlg_button.Click += new System.EventHandler(this.buttonAllPopup_Click);
            // 
            // ChildDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(126, 22);
            this.Controls.Add(this.ChildDlg_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChildDlg";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ChildDlg";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ChildDlg_button;
    }
}