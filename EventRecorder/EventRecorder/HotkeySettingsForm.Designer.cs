namespace EventRecorder
{
    partial class HotkeySettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label_Record = new System.Windows.Forms.Label();
            this.textBox_Record = new StandardTemplate.TextBoxEx();
            this.label_Play = new System.Windows.Forms.Label();
            this.textBox_Play = new StandardTemplate.TextBoxEx();
            this.label_Hint = new System.Windows.Forms.Label();
            this.button_Reset = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // label_Record
            //
            this.label_Record.AutoSize = true;
            this.label_Record.Location = new System.Drawing.Point(12, 16);
            this.label_Record.Name = "label_Record";
            this.label_Record.Size = new System.Drawing.Size(47, 12);
            this.label_Record.TabIndex = 0;
            this.label_Record.Text = "レコード";
            //
            // textBox_Record
            //
            this.textBox_Record.Location = new System.Drawing.Point(90, 13);
            this.textBox_Record.Name = "textBox_Record";
            this.textBox_Record.ReadOnly = true;
            this.textBox_Record.Size = new System.Drawing.Size(180, 19);
            this.textBox_Record.TabIndex = 1;
            this.textBox_Record.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Record_KeyDown);
            //
            // label_Play
            //
            this.label_Play.AutoSize = true;
            this.label_Play.Location = new System.Drawing.Point(12, 45);
            this.label_Play.Name = "label_Play";
            this.label_Play.Size = new System.Drawing.Size(29, 12);
            this.label_Play.TabIndex = 2;
            this.label_Play.Text = "再生";
            //
            // textBox_Play
            //
            this.textBox_Play.Location = new System.Drawing.Point(90, 42);
            this.textBox_Play.Name = "textBox_Play";
            this.textBox_Play.ReadOnly = true;
            this.textBox_Play.Size = new System.Drawing.Size(180, 19);
            this.textBox_Play.TabIndex = 3;
            this.textBox_Play.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Play_KeyDown);
            //
            // label_Hint
            //
            this.label_Hint.AutoSize = true;
            this.label_Hint.Location = new System.Drawing.Point(12, 74);
            this.label_Hint.Name = "label_Hint";
            this.label_Hint.Size = new System.Drawing.Size(258, 24);
            this.label_Hint.TabIndex = 4;
            this.label_Hint.Text = "テキストボックスにフォーカスして、\r\n割り当てたいキーを押してね";
            //
            // button_Reset
            //
            this.button_Reset.Location = new System.Drawing.Point(12, 106);
            this.button_Reset.Name = "button_Reset";
            this.button_Reset.Size = new System.Drawing.Size(100, 27);
            this.button_Reset.TabIndex = 5;
            this.button_Reset.Text = "初期値に戻す";
            this.button_Reset.UseVisualStyleBackColor = true;
            this.button_Reset.Click += new System.EventHandler(this.button_Reset_Click);
            //
            // button_Save
            //
            this.button_Save.Location = new System.Drawing.Point(198, 106);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(72, 27);
            this.button_Save.TabIndex = 6;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            //
            // HotkeySettingsForm
            //
            this.AcceptButton = this.button_Save;
            this.ClientSize = new System.Drawing.Size(282, 145);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.button_Reset);
            this.Controls.Add(this.label_Hint);
            this.Controls.Add(this.textBox_Play);
            this.Controls.Add(this.label_Play);
            this.Controls.Add(this.textBox_Record);
            this.Controls.Add(this.label_Record);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HotkeySettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EventRecorder - キーバインド設定";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label_Record;
        private StandardTemplate.TextBoxEx textBox_Record;
        private System.Windows.Forms.Label label_Play;
        private StandardTemplate.TextBoxEx textBox_Play;
        private System.Windows.Forms.Label label_Hint;
        private System.Windows.Forms.Button button_Reset;
        private System.Windows.Forms.Button button_Save;
    }
}
