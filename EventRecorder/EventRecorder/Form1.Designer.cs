namespace EventRecorder
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Wait = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_Record = new System.Windows.Forms.Button();
            this.button_Play = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Load = new System.Windows.Forms.Button();
            this.label_Loop = new System.Windows.Forms.Label();
            this.textBox_Loop = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            //
            // dataGridView1
            //
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Type,
            this.col_X,
            this.col_Y,
            this.col_Key,
            this.col_Wait});
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.Size = new System.Drawing.Size(406, 296);
            this.dataGridView1.TabIndex = 0;
            //
            // col_Type
            //
            this.col_Type.HeaderText = "Event";
            this.col_Type.Name = "col_Type";
            this.col_Type.Width = 90;
            //
            // col_X
            //
            this.col_X.HeaderText = "X";
            this.col_X.Name = "col_X";
            this.col_X.Width = 55;
            //
            // col_Y
            //
            this.col_Y.HeaderText = "Y";
            this.col_Y.Name = "col_Y";
            this.col_Y.Width = 55;
            //
            // col_Key
            //
            this.col_Key.HeaderText = "Key";
            this.col_Key.Name = "col_Key";
            this.col_Key.Width = 80;
            //
            // col_Wait
            //
            this.col_Wait.HeaderText = "Wait(ms)";
            this.col_Wait.Name = "col_Wait";
            this.col_Wait.Width = 70;
            //
            // button_Record
            //
            this.button_Record.Location = new System.Drawing.Point(12, 318);
            this.button_Record.Name = "button_Record";
            this.button_Record.Size = new System.Drawing.Size(80, 27);
            this.button_Record.TabIndex = 1;
            this.button_Record.Text = "記録";
            this.button_Record.UseVisualStyleBackColor = true;
            this.button_Record.Click += new System.EventHandler(this.button_Record_Click);
            //
            // button_Play
            //
            this.button_Play.Location = new System.Drawing.Point(98, 318);
            this.button_Play.Name = "button_Play";
            this.button_Play.Size = new System.Drawing.Size(80, 27);
            this.button_Play.TabIndex = 2;
            this.button_Play.Text = "再生";
            this.button_Play.UseVisualStyleBackColor = true;
            this.button_Play.Click += new System.EventHandler(this.button_Play_Click);
            //
            // button_Clear
            //
            this.button_Clear.Location = new System.Drawing.Point(184, 318);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(80, 27);
            this.button_Clear.TabIndex = 3;
            this.button_Clear.Text = "クリア";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            //
            // button_Save
            //
            this.button_Save.Location = new System.Drawing.Point(12, 351);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(80, 27);
            this.button_Save.TabIndex = 4;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            //
            // button_Load
            //
            this.button_Load.Location = new System.Drawing.Point(98, 351);
            this.button_Load.Name = "button_Load";
            this.button_Load.Size = new System.Drawing.Size(80, 27);
            this.button_Load.TabIndex = 5;
            this.button_Load.Text = "読込";
            this.button_Load.UseVisualStyleBackColor = true;
            this.button_Load.Click += new System.EventHandler(this.button_Load_Click);
            //
            // label_Loop
            //
            this.label_Loop.AutoSize = true;
            this.label_Loop.Location = new System.Drawing.Point(190, 357);
            this.label_Loop.Name = "label_Loop";
            this.label_Loop.Size = new System.Drawing.Size(56, 12);
            this.label_Loop.TabIndex = 6;
            this.label_Loop.Text = "ループ回数";
            //
            // textBox_Loop
            //
            this.textBox_Loop.Location = new System.Drawing.Point(252, 353);
            this.textBox_Loop.Name = "textBox_Loop";
            this.textBox_Loop.Size = new System.Drawing.Size(48, 19);
            this.textBox_Loop.TabIndex = 7;
            this.textBox_Loop.Text = "1";
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 390);
            this.Controls.Add(this.textBox_Loop);
            this.Controls.Add(this.label_Loop);
            this.Controls.Add(this.button_Load);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.button_Play);
            this.Controls.Add(this.button_Record);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "EventRecorder";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_X;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Wait;
        private System.Windows.Forms.Button button_Record;
        private System.Windows.Forms.Button button_Play;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_Load;
        private System.Windows.Forms.Label label_Loop;
        internal System.Windows.Forms.TextBox textBox_Loop;
    }
}
