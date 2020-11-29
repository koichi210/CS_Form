namespace ToyingData
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_Wide2Narrow_Space = new System.Windows.Forms.CheckBox();
            this.checkBox_Wide2Narrow_Alpha_Small = new System.Windows.Forms.CheckBox();
            this.checkBox_Wide2Narrow_Alpha_Large = new System.Windows.Forms.CheckBox();
            this.checkBox_Wide2Narrow_Number = new System.Windows.Forms.CheckBox();
            this.radioButton_ChangeWide2Narrow = new System.Windows.Forms.RadioButton();
            this.radioButton_DeleteDuplicate = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Source = new System.Windows.Forms.TextBox();
            this.textBox_Dest = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_Execute = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.checkBox_Wide2Narrow_Space);
            this.groupBox1.Controls.Add(this.checkBox_Wide2Narrow_Alpha_Small);
            this.groupBox1.Controls.Add(this.checkBox_Wide2Narrow_Alpha_Large);
            this.groupBox1.Controls.Add(this.checkBox_Wide2Narrow_Number);
            this.groupBox1.Controls.Add(this.radioButton_ChangeWide2Narrow);
            this.groupBox1.Controls.Add(this.radioButton_DeleteDuplicate);
            this.groupBox1.Location = new System.Drawing.Point(20, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBox1.Size = new System.Drawing.Size(333, 261);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "メニュー";
            // 
            // checkBox_Wide2Narrow_Space
            // 
            this.checkBox_Wide2Narrow_Space.AutoSize = true;
            this.checkBox_Wide2Narrow_Space.Location = new System.Drawing.Point(58, 198);
            this.checkBox_Wide2Narrow_Space.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBox_Wide2Narrow_Space.Name = "checkBox_Wide2Narrow_Space";
            this.checkBox_Wide2Narrow_Space.Size = new System.Drawing.Size(92, 22);
            this.checkBox_Wide2Narrow_Space.TabIndex = 5;
            this.checkBox_Wide2Narrow_Space.Text = "スペース";
            this.checkBox_Wide2Narrow_Space.UseVisualStyleBackColor = true;
            // 
            // checkBox_Wide2Narrow_Alpha_Small
            // 
            this.checkBox_Wide2Narrow_Alpha_Small.AutoSize = true;
            this.checkBox_Wide2Narrow_Alpha_Small.Location = new System.Drawing.Point(58, 165);
            this.checkBox_Wide2Narrow_Alpha_Small.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBox_Wide2Narrow_Alpha_Small.Name = "checkBox_Wide2Narrow_Alpha_Small";
            this.checkBox_Wide2Narrow_Alpha_Small.Size = new System.Drawing.Size(137, 22);
            this.checkBox_Wide2Narrow_Alpha_Small.TabIndex = 4;
            this.checkBox_Wide2Narrow_Alpha_Small.Text = "英小字[ａ～ｚ]";
            this.checkBox_Wide2Narrow_Alpha_Small.UseVisualStyleBackColor = true;
            // 
            // checkBox_Wide2Narrow_Alpha_Large
            // 
            this.checkBox_Wide2Narrow_Alpha_Large.AutoSize = true;
            this.checkBox_Wide2Narrow_Alpha_Large.Location = new System.Drawing.Point(58, 132);
            this.checkBox_Wide2Narrow_Alpha_Large.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBox_Wide2Narrow_Alpha_Large.Name = "checkBox_Wide2Narrow_Alpha_Large";
            this.checkBox_Wide2Narrow_Alpha_Large.Size = new System.Drawing.Size(143, 22);
            this.checkBox_Wide2Narrow_Alpha_Large.TabIndex = 3;
            this.checkBox_Wide2Narrow_Alpha_Large.Text = "英大字[Ａ～Ｚ]";
            this.checkBox_Wide2Narrow_Alpha_Large.UseVisualStyleBackColor = true;
            // 
            // checkBox_Wide2Narrow_Number
            // 
            this.checkBox_Wide2Narrow_Number.AutoSize = true;
            this.checkBox_Wide2Narrow_Number.Location = new System.Drawing.Point(58, 99);
            this.checkBox_Wide2Narrow_Number.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBox_Wide2Narrow_Number.Name = "checkBox_Wide2Narrow_Number";
            this.checkBox_Wide2Narrow_Number.Size = new System.Drawing.Size(124, 22);
            this.checkBox_Wide2Narrow_Number.TabIndex = 2;
            this.checkBox_Wide2Narrow_Number.Text = "数値[１～２]";
            this.checkBox_Wide2Narrow_Number.UseVisualStyleBackColor = true;
            // 
            // radioButton_ChangeWide2Narrow
            // 
            this.radioButton_ChangeWide2Narrow.AutoSize = true;
            this.radioButton_ChangeWide2Narrow.Location = new System.Drawing.Point(27, 66);
            this.radioButton_ChangeWide2Narrow.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.radioButton_ChangeWide2Narrow.Name = "radioButton_ChangeWide2Narrow";
            this.radioButton_ChangeWide2Narrow.Size = new System.Drawing.Size(169, 22);
            this.radioButton_ChangeWide2Narrow.TabIndex = 1;
            this.radioButton_ChangeWide2Narrow.TabStop = true;
            this.radioButton_ChangeWide2Narrow.Text = "全角を半角に変換";
            this.radioButton_ChangeWide2Narrow.UseVisualStyleBackColor = true;
            // 
            // radioButton_DeleteDuplicate
            // 
            this.radioButton_DeleteDuplicate.AutoSize = true;
            this.radioButton_DeleteDuplicate.Location = new System.Drawing.Point(27, 33);
            this.radioButton_DeleteDuplicate.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.radioButton_DeleteDuplicate.Name = "radioButton_DeleteDuplicate";
            this.radioButton_DeleteDuplicate.Size = new System.Drawing.Size(119, 22);
            this.radioButton_DeleteDuplicate.TabIndex = 0;
            this.radioButton_DeleteDuplicate.TabStop = true;
            this.radioButton_DeleteDuplicate.Text = "重複を削除";
            this.radioButton_DeleteDuplicate.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(385, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "変更前データ";
            // 
            // textBox_Source
            // 
            this.textBox_Source.AllowDrop = true;
            this.textBox_Source.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_Source.Location = new System.Drawing.Point(388, 40);
            this.textBox_Source.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox_Source.Multiline = true;
            this.textBox_Source.Name = "textBox_Source";
            this.textBox_Source.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Source.Size = new System.Drawing.Size(362, 236);
            this.textBox_Source.TabIndex = 2;
            this.textBox_Source.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_Source_DragDrop);
            this.textBox_Source.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox_Source_DragEnter);
            this.textBox_Source.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Source_KeyDown);
            // 
            // textBox_Dest
            // 
            this.textBox_Dest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Dest.Location = new System.Drawing.Point(803, 40);
            this.textBox_Dest.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox_Dest.Multiline = true;
            this.textBox_Dest.Name = "textBox_Dest";
            this.textBox_Dest.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Dest.Size = new System.Drawing.Size(362, 236);
            this.textBox_Dest.TabIndex = 5;
            this.textBox_Dest.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Dest_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(800, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "変更後データ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(763, 136);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "⇒";
            // 
            // button_Execute
            // 
            this.button_Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Execute.Location = new System.Drawing.Point(1043, 288);
            this.button_Execute.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.button_Execute.Name = "button_Execute";
            this.button_Execute.Size = new System.Drawing.Size(125, 34);
            this.button_Execute.TabIndex = 6;
            this.button_Execute.Text = "実行";
            this.button_Execute.UseVisualStyleBackColor = true;
            this.button_Execute.Click += new System.EventHandler(this.button_Execute_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1188, 330);
            this.Controls.Add(this.button_Execute);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Dest);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Source);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MinimumSize = new System.Drawing.Size(1200, 359);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Source;
        private System.Windows.Forms.TextBox textBox_Dest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Execute;
        private System.Windows.Forms.RadioButton radioButton_DeleteDuplicate;
        private System.Windows.Forms.RadioButton radioButton_ChangeWide2Narrow;
        private System.Windows.Forms.CheckBox checkBox_Wide2Narrow_Alpha_Small;
        private System.Windows.Forms.CheckBox checkBox_Wide2Narrow_Alpha_Large;
        private System.Windows.Forms.CheckBox checkBox_Wide2Narrow_Number;
        private System.Windows.Forms.CheckBox checkBox_Wide2Narrow_Space;
    }
}

