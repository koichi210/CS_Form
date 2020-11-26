namespace DigitalClock
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
            this.components = new System.ComponentModel.Container();
            this.label_time = new System.Windows.Forms.Label();
            this.clock_timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.label_time.Location = new System.Drawing.Point(0, 0);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(62, 16);
            this.label_time.TabIndex = 0;
            this.label_time.Text = "00:00:00";
            this.label_time.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_time_MouseDown);
            this.label_time.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_time_MouseMove);
            // 
            // clock_timer
            // 
            this.clock_timer.Enabled = true;
            this.clock_timer.Interval = 500;
            this.clock_timer.Tick += new System.EventHandler(this.ClockTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 11F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(61, 16);
            this.Controls.Add(this.label_time);
            this.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "Form1";
            this.Opacity = 0.5D;
            this.Text = "Clock";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Timer clock_timer;
    }
}

