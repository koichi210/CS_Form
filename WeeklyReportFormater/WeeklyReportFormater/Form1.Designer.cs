namespace WeeklyReportFormater
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
            this.button_ThisWeekChange = new System.Windows.Forms.Button();
            this.textBox_ThisWeekBefore = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_ThisWeekAfter = new System.Windows.Forms.TextBox();
            this.textBox_NextWeekAfter = new System.Windows.Forms.TextBox();
            this.textBox_NextWeekBefore = new System.Windows.Forms.TextBox();
            this.button_NextWeekChange = new System.Windows.Forms.Button();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_PerforceAfter = new System.Windows.Forms.TextBox();
            this.textBox_PerforceBefore = new System.Windows.Forms.TextBox();
            this.button_PerforceChange = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_ThisWeekChange
            // 
            this.button_ThisWeekChange.Location = new System.Drawing.Point(356, 119);
            this.button_ThisWeekChange.Name = "button_ThisWeekChange";
            this.button_ThisWeekChange.Size = new System.Drawing.Size(75, 23);
            this.button_ThisWeekChange.TabIndex = 6;
            this.button_ThisWeekChange.Text = "変換";
            this.button_ThisWeekChange.UseVisualStyleBackColor = true;
            this.button_ThisWeekChange.Click += new System.EventHandler(this.button_ThisWeekChange_Click);
            // 
            // textBox_ThisWeekBefore
            // 
            this.textBox_ThisWeekBefore.Location = new System.Drawing.Point(22, 73);
            this.textBox_ThisWeekBefore.Multiline = true;
            this.textBox_ThisWeekBefore.Name = "textBox_ThisWeekBefore";
            this.textBox_ThisWeekBefore.Size = new System.Drawing.Size(185, 40);
            this.textBox_ThisWeekBefore.TabIndex = 3;
            this.textBox_ThisWeekBefore.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_ThisWeekBefore_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "今週";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "次週";
            // 
            // textBox_ThisWeekAfter
            // 
            this.textBox_ThisWeekAfter.Location = new System.Drawing.Point(246, 73);
            this.textBox_ThisWeekAfter.Multiline = true;
            this.textBox_ThisWeekAfter.Name = "textBox_ThisWeekAfter";
            this.textBox_ThisWeekAfter.Size = new System.Drawing.Size(185, 40);
            this.textBox_ThisWeekAfter.TabIndex = 5;
            this.textBox_ThisWeekAfter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_ThisWeekAfter_KeyDown);
            // 
            // textBox_NextWeekAfter
            // 
            this.textBox_NextWeekAfter.Location = new System.Drawing.Point(246, 162);
            this.textBox_NextWeekAfter.Multiline = true;
            this.textBox_NextWeekAfter.Name = "textBox_NextWeekAfter";
            this.textBox_NextWeekAfter.Size = new System.Drawing.Size(185, 40);
            this.textBox_NextWeekAfter.TabIndex = 10;
            this.textBox_NextWeekAfter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_NextWeekAfter_KeyDown);
            // 
            // textBox_NextWeekBefore
            // 
            this.textBox_NextWeekBefore.Location = new System.Drawing.Point(22, 162);
            this.textBox_NextWeekBefore.Multiline = true;
            this.textBox_NextWeekBefore.Name = "textBox_NextWeekBefore";
            this.textBox_NextWeekBefore.Size = new System.Drawing.Size(185, 40);
            this.textBox_NextWeekBefore.TabIndex = 8;
            this.textBox_NextWeekBefore.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_NextWeekBefore_KeyDown);
            // 
            // button_NextWeekChange
            // 
            this.button_NextWeekChange.Location = new System.Drawing.Point(356, 208);
            this.button_NextWeekChange.Name = "button_NextWeekChange";
            this.button_NextWeekChange.Size = new System.Drawing.Size(75, 23);
            this.button_NextWeekChange.TabIndex = 11;
            this.button_NextWeekChange.Text = "変換";
            this.button_NextWeekChange.UseVisualStyleBackColor = true;
            this.button_NextWeekChange.Click += new System.EventHandler(this.button_NextWeekChange_Click);
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Location = new System.Drawing.Point(84, 16);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(100, 19);
            this.textBox_UserName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "私の名前は";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(217, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "⇒";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(217, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "⇒";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(217, 262);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "⇒";
            // 
            // textBox_PerforceAfter
            // 
            this.textBox_PerforceAfter.Location = new System.Drawing.Point(246, 248);
            this.textBox_PerforceAfter.Multiline = true;
            this.textBox_PerforceAfter.Name = "textBox_PerforceAfter";
            this.textBox_PerforceAfter.Size = new System.Drawing.Size(185, 40);
            this.textBox_PerforceAfter.TabIndex = 15;
            this.textBox_PerforceAfter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_PerforceAfter_KeyDown);
            // 
            // textBox_PerforceBefore
            // 
            this.textBox_PerforceBefore.Location = new System.Drawing.Point(22, 248);
            this.textBox_PerforceBefore.Multiline = true;
            this.textBox_PerforceBefore.Name = "textBox_PerforceBefore";
            this.textBox_PerforceBefore.Size = new System.Drawing.Size(185, 40);
            this.textBox_PerforceBefore.TabIndex = 13;
            this.textBox_PerforceBefore.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_PerforceBefore_KeyDown);
            // 
            // button_PerforceChange
            // 
            this.button_PerforceChange.Location = new System.Drawing.Point(356, 294);
            this.button_PerforceChange.Name = "button_PerforceChange";
            this.button_PerforceChange.Size = new System.Drawing.Size(75, 23);
            this.button_PerforceChange.TabIndex = 16;
            this.button_PerforceChange.Text = "変換";
            this.button_PerforceChange.UseVisualStyleBackColor = true;
            this.button_PerforceChange.Click += new System.EventHandler(this.button_PerforceChange_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 232);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "Perforce";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 328);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox_PerforceAfter);
            this.Controls.Add(this.textBox_PerforceBefore);
            this.Controls.Add(this.button_PerforceChange);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_UserName);
            this.Controls.Add(this.textBox_NextWeekAfter);
            this.Controls.Add(this.textBox_NextWeekBefore);
            this.Controls.Add(this.button_NextWeekChange);
            this.Controls.Add(this.textBox_ThisWeekAfter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_ThisWeekBefore);
            this.Controls.Add(this.button_ThisWeekChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(467, 356);
            this.Name = "Form1";
            this.Text = "週報ふぉーまった！";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_ThisWeekChange;
        private System.Windows.Forms.TextBox textBox_ThisWeekBefore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_ThisWeekAfter;
        private System.Windows.Forms.TextBox textBox_NextWeekAfter;
        private System.Windows.Forms.TextBox textBox_NextWeekBefore;
        private System.Windows.Forms.Button button_NextWeekChange;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_PerforceAfter;
        private System.Windows.Forms.TextBox textBox_PerforceBefore;
        private System.Windows.Forms.Button button_PerforceChange;
        private System.Windows.Forms.Label label7;
    }
}

