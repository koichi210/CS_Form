namespace WebCamera
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
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.pictureBox_Preview = new System.Windows.Forms.PictureBox();
            this.backgroundWorker_Capture = new System.ComponentModel.BackgroundWorker();
            this.button_StartStop = new System.Windows.Forms.Button();
            this.button_Snapshot = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Preview)).BeginInit();
            this.SuspendLayout();
            //
            // pictureBox_Preview
            //
            this.pictureBox_Preview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_Preview.BackColor = System.Drawing.Color.Black;
            this.pictureBox_Preview.Location = new System.Drawing.Point(12, 12);
            this.pictureBox_Preview.Name = "pictureBox_Preview";
            this.pictureBox_Preview.Size = new System.Drawing.Size(640, 480);
            this.pictureBox_Preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Preview.TabIndex = 0;
            this.pictureBox_Preview.TabStop = false;
            //
            // backgroundWorker_Capture
            //
            this.backgroundWorker_Capture.WorkerReportsProgress = true;
            this.backgroundWorker_Capture.WorkerSupportsCancellation = true;
            this.backgroundWorker_Capture.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_Capture_DoWork);
            this.backgroundWorker_Capture.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_Capture_ProgressChanged);
            this.backgroundWorker_Capture.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_Capture_RunWorkerCompleted);
            //
            // button_StartStop
            //
            this.button_StartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_StartStop.Location = new System.Drawing.Point(12, 504);
            this.button_StartStop.Name = "button_StartStop";
            this.button_StartStop.Size = new System.Drawing.Size(104, 36);
            this.button_StartStop.TabIndex = 1;
            this.button_StartStop.Text = "Start";
            this.button_StartStop.UseVisualStyleBackColor = true;
            this.button_StartStop.Click += new System.EventHandler(this.button_StartStop_Click);
            //
            // button_Snapshot
            //
            this.button_Snapshot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Snapshot.Enabled = false;
            this.button_Snapshot.Location = new System.Drawing.Point(122, 504);
            this.button_Snapshot.Name = "button_Snapshot";
            this.button_Snapshot.Size = new System.Drawing.Size(104, 36);
            this.button_Snapshot.TabIndex = 2;
            this.button_Snapshot.Text = "Snapshot";
            this.button_Snapshot.UseVisualStyleBackColor = true;
            this.button_Snapshot.Click += new System.EventHandler(this.button_Snapshot_Click);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 552);
            this.Controls.Add(this.button_Snapshot);
            this.Controls.Add(this.button_StartStop);
            this.Controls.Add(this.pictureBox_Preview);
            this.MinimumSize = new System.Drawing.Size(320, 280);
            this.Name = "Form1";
            this.Text = "WebCamera";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Preview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_Preview;
        private System.ComponentModel.BackgroundWorker backgroundWorker_Capture;
        private System.Windows.Forms.Button button_StartStop;
        private System.Windows.Forms.Button button_Snapshot;
    }
}

