namespace ProgressBar
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
            this.progressBar_SingleThread = new System.Windows.Forms.ProgressBar();
            this.button_StartSingleThread = new System.Windows.Forms.Button();
            this.button_StopSingleThread = new System.Windows.Forms.Button();
            this.button_StopMultiThreadTask = new System.Windows.Forms.Button();
            this.button_StartMultiThreadTask = new System.Windows.Forms.Button();
            this.progressBar_MultiThreadTask = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_StopMultiThreadBkgWork = new System.Windows.Forms.Button();
            this.button_StartMultiThreadBkgWork = new System.Windows.Forms.Button();
            this.progressBar_MultiThreadBkgWork = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker_BkgWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar_MultiThreadTaskInBkgWork = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.button_StopTaskInBackgroundWorker = new System.Windows.Forms.Button();
            this.button_StartTaskInBackgroundWorker = new System.Windows.Forms.Button();
            this.backgroundWorker_TaskInBkgWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progressBar_SingleThread
            // 
            this.progressBar_SingleThread.Location = new System.Drawing.Point(12, 21);
            this.progressBar_SingleThread.Name = "progressBar_SingleThread";
            this.progressBar_SingleThread.Size = new System.Drawing.Size(224, 23);
            this.progressBar_SingleThread.TabIndex = 0;
            // 
            // button_StartSingleThread
            // 
            this.button_StartSingleThread.Location = new System.Drawing.Point(242, 21);
            this.button_StartSingleThread.Name = "button_StartSingleThread";
            this.button_StartSingleThread.Size = new System.Drawing.Size(75, 23);
            this.button_StartSingleThread.TabIndex = 1;
            this.button_StartSingleThread.Text = "Start";
            this.button_StartSingleThread.UseVisualStyleBackColor = true;
            this.button_StartSingleThread.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // button_StopSingleThread
            // 
            this.button_StopSingleThread.Location = new System.Drawing.Point(323, 21);
            this.button_StopSingleThread.Name = "button_StopSingleThread";
            this.button_StopSingleThread.Size = new System.Drawing.Size(75, 23);
            this.button_StopSingleThread.TabIndex = 2;
            this.button_StopSingleThread.Text = "Stop";
            this.button_StopSingleThread.UseVisualStyleBackColor = true;
            this.button_StopSingleThread.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // button_StopMultiThreadTask
            // 
            this.button_StopMultiThreadTask.Location = new System.Drawing.Point(323, 71);
            this.button_StopMultiThreadTask.Name = "button_StopMultiThreadTask";
            this.button_StopMultiThreadTask.Size = new System.Drawing.Size(75, 23);
            this.button_StopMultiThreadTask.TabIndex = 5;
            this.button_StopMultiThreadTask.Text = "Stop";
            this.button_StopMultiThreadTask.UseVisualStyleBackColor = true;
            this.button_StopMultiThreadTask.Click += new System.EventHandler(this.button_StopMultiThreadTask_Click);
            // 
            // button_StartMultiThreadTask
            // 
            this.button_StartMultiThreadTask.Location = new System.Drawing.Point(242, 71);
            this.button_StartMultiThreadTask.Name = "button_StartMultiThreadTask";
            this.button_StartMultiThreadTask.Size = new System.Drawing.Size(75, 23);
            this.button_StartMultiThreadTask.TabIndex = 4;
            this.button_StartMultiThreadTask.Text = "Start";
            this.button_StartMultiThreadTask.UseVisualStyleBackColor = true;
            this.button_StartMultiThreadTask.Click += new System.EventHandler(this.button_StartMultiThreadTask_Click);
            // 
            // progressBar_MultiThreadTask
            // 
            this.progressBar_MultiThreadTask.Location = new System.Drawing.Point(12, 71);
            this.progressBar_MultiThreadTask.Name = "progressBar_MultiThreadTask";
            this.progressBar_MultiThreadTask.Size = new System.Drawing.Size(224, 23);
            this.progressBar_MultiThreadTask.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Single Thread";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "Multi Thread(Task)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "Multi Thread(BackgroundWorker)";
            // 
            // button_StopMultiThreadBkgWork
            // 
            this.button_StopMultiThreadBkgWork.Location = new System.Drawing.Point(323, 121);
            this.button_StopMultiThreadBkgWork.Name = "button_StopMultiThreadBkgWork";
            this.button_StopMultiThreadBkgWork.Size = new System.Drawing.Size(75, 23);
            this.button_StopMultiThreadBkgWork.TabIndex = 10;
            this.button_StopMultiThreadBkgWork.Text = "Stop";
            this.button_StopMultiThreadBkgWork.UseVisualStyleBackColor = true;
            this.button_StopMultiThreadBkgWork.Click += new System.EventHandler(this.button_StopMultiThreadBkgWork_Click);
            // 
            // button_StartMultiThreadBkgWork
            // 
            this.button_StartMultiThreadBkgWork.Location = new System.Drawing.Point(242, 121);
            this.button_StartMultiThreadBkgWork.Name = "button_StartMultiThreadBkgWork";
            this.button_StartMultiThreadBkgWork.Size = new System.Drawing.Size(75, 23);
            this.button_StartMultiThreadBkgWork.TabIndex = 9;
            this.button_StartMultiThreadBkgWork.Text = "Start";
            this.button_StartMultiThreadBkgWork.UseVisualStyleBackColor = true;
            this.button_StartMultiThreadBkgWork.Click += new System.EventHandler(this.button_StartMultiThreadBkgWork_Click);
            // 
            // progressBar_MultiThreadBkgWork
            // 
            this.progressBar_MultiThreadBkgWork.Location = new System.Drawing.Point(12, 121);
            this.progressBar_MultiThreadBkgWork.Name = "progressBar_MultiThreadBkgWork";
            this.progressBar_MultiThreadBkgWork.Size = new System.Drawing.Size(224, 23);
            this.progressBar_MultiThreadBkgWork.TabIndex = 8;
            // 
            // backgroundWorker_BkgWorker
            // 
            this.backgroundWorker_BkgWorker.WorkerReportsProgress = true;
            this.backgroundWorker_BkgWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker_BkgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_BkgWorker_DoWork);
            this.backgroundWorker_BkgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_BkgWorker_ProgressChanged);
            this.backgroundWorker_BkgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_BkgWorker_RunWorkerCompleted);
            // 
            // progressBar_MultiThreadTaskInBkgWork
            // 
            this.progressBar_MultiThreadTaskInBkgWork.Location = new System.Drawing.Point(12, 171);
            this.progressBar_MultiThreadTaskInBkgWork.Name = "progressBar_MultiThreadTaskInBkgWork";
            this.progressBar_MultiThreadTaskInBkgWork.Size = new System.Drawing.Size(224, 23);
            this.progressBar_MultiThreadTaskInBkgWork.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(206, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "Multi Thread(TaskInBackgroundWorker)";
            // 
            // button_StopTaskInBackgroundWorker
            // 
            this.button_StopTaskInBackgroundWorker.Location = new System.Drawing.Point(323, 171);
            this.button_StopTaskInBackgroundWorker.Name = "button_StopTaskInBackgroundWorker";
            this.button_StopTaskInBackgroundWorker.Size = new System.Drawing.Size(75, 23);
            this.button_StopTaskInBackgroundWorker.TabIndex = 15;
            this.button_StopTaskInBackgroundWorker.Text = "Stop";
            this.button_StopTaskInBackgroundWorker.UseVisualStyleBackColor = true;
            this.button_StopTaskInBackgroundWorker.Click += new System.EventHandler(this.button_StopTaskInBackgroundWorker_Click);
            // 
            // button_StartTaskInBackgroundWorker
            // 
            this.button_StartTaskInBackgroundWorker.Location = new System.Drawing.Point(242, 171);
            this.button_StartTaskInBackgroundWorker.Name = "button_StartTaskInBackgroundWorker";
            this.button_StartTaskInBackgroundWorker.Size = new System.Drawing.Size(75, 23);
            this.button_StartTaskInBackgroundWorker.TabIndex = 14;
            this.button_StartTaskInBackgroundWorker.Text = "Start";
            this.button_StartTaskInBackgroundWorker.UseVisualStyleBackColor = true;
            this.button_StartTaskInBackgroundWorker.Click += new System.EventHandler(this.button_StartTaskInBackgroundWorker_Click);
            // 
            // backgroundWorker_TaskInBkgWorker
            // 
            this.backgroundWorker_TaskInBkgWorker.WorkerReportsProgress = true;
            this.backgroundWorker_TaskInBkgWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker_TaskInBkgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_TaskInBkgWorker_DoWork);
            this.backgroundWorker_TaskInBkgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_TaskInBkgWorker_ProgressChanged);
            this.backgroundWorker_TaskInBkgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_TaskInBkgWorker_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 213);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_StopTaskInBackgroundWorker);
            this.Controls.Add(this.button_StartTaskInBackgroundWorker);
            this.Controls.Add(this.progressBar_MultiThreadTaskInBkgWork);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_StopMultiThreadBkgWork);
            this.Controls.Add(this.button_StartMultiThreadBkgWork);
            this.Controls.Add(this.progressBar_MultiThreadBkgWork);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_StopMultiThreadTask);
            this.Controls.Add(this.button_StartMultiThreadTask);
            this.Controls.Add(this.progressBar_MultiThreadTask);
            this.Controls.Add(this.button_StopSingleThread);
            this.Controls.Add(this.button_StartSingleThread);
            this.Controls.Add(this.progressBar_SingleThread);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ProgressBar progressBar_SingleThread;
        private System.Windows.Forms.Button button_StartSingleThread;
        private System.Windows.Forms.Button button_StopSingleThread;
        private System.Windows.Forms.Button button_StopMultiThreadTask;
        private System.Windows.Forms.Button button_StartMultiThreadTask;
        public System.Windows.Forms.ProgressBar progressBar_MultiThreadTask;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_StopMultiThreadBkgWork;
        private System.Windows.Forms.Button button_StartMultiThreadBkgWork;
        public System.Windows.Forms.ProgressBar progressBar_MultiThreadBkgWork;
        private System.ComponentModel.BackgroundWorker backgroundWorker_BkgWorker;
        public System.Windows.Forms.ProgressBar progressBar_MultiThreadTaskInBkgWork;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_StopTaskInBackgroundWorker;
        private System.Windows.Forms.Button button_StartTaskInBackgroundWorker;
        private System.ComponentModel.BackgroundWorker backgroundWorker_TaskInBkgWorker;
    }
}

