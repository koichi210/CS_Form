using System.Windows.Forms;

namespace VisualStudioBuilder
{
    partial class FormSelectDirectory
    {
        private Label label1;
        private Button button_OK;
        private Button button_Cancel;
        private TextBox textBox_DirectoryPath;

        private void InitializeComponent()
        {
            this.textBox_DirectoryPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_DirectoryPath
            // 
            this.textBox_DirectoryPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DirectoryPath.Location = new System.Drawing.Point(14, 24);
            this.textBox_DirectoryPath.Name = "textBox_DirectoryPath";
            this.textBox_DirectoryPath.Size = new System.Drawing.Size(258, 19);
            this.textBox_DirectoryPath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ルートディレクトリパスを設定";
            // 
            // button_OK
            // 
            this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(116, 49);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 2;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(197, 49);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 3;
            this.button_Cancel.Text = "キャンセル";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // FormSelectDirectory
            // 
            this.ClientSize = new System.Drawing.Size(284, 80);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_DirectoryPath);
            this.MaximumSize = new System.Drawing.Size(3000, 118);
            this.MinimumSize = new System.Drawing.Size(300, 118);
            this.Name = "FormSelectDirectory";
            this.Text = "SelectDirectory";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
