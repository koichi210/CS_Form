namespace ChotChotChat
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
            this.button_Server = new System.Windows.Forms.Button();
            this.button_Client = new System.Windows.Forms.Button();
            this.textBox_Message = new System.Windows.Forms.TextBox();
            this.label_StatusBar = new System.Windows.Forms.Label();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.button_Send = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_Server
            // 
            this.button_Server.Location = new System.Drawing.Point(67, 12);
            this.button_Server.Name = "button_Server";
            this.button_Server.Size = new System.Drawing.Size(75, 23);
            this.button_Server.TabIndex = 0;
            this.button_Server.Text = "Server";
            this.button_Server.UseVisualStyleBackColor = true;
            this.button_Server.Click += new System.EventHandler(this.button_Server_Click);
            // 
            // button_Client
            // 
            this.button_Client.Location = new System.Drawing.Point(159, 12);
            this.button_Client.Name = "button_Client";
            this.button_Client.Size = new System.Drawing.Size(75, 23);
            this.button_Client.TabIndex = 1;
            this.button_Client.Text = "client";
            this.button_Client.UseVisualStyleBackColor = true;
            this.button_Client.Click += new System.EventHandler(this.button_Client_Click);
            // 
            // textBox_Message
            // 
            this.textBox_Message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Message.Location = new System.Drawing.Point(10, 152);
            this.textBox_Message.Name = "textBox_Message";
            this.textBox_Message.Size = new System.Drawing.Size(270, 19);
            this.textBox_Message.TabIndex = 2;
            // 
            // label_StatusBar
            // 
            this.label_StatusBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_StatusBar.Location = new System.Drawing.Point(10, 174);
            this.label_StatusBar.Name = "label_StatusBar";
            this.label_StatusBar.Size = new System.Drawing.Size(308, 19);
            this.label_StatusBar.TabIndex = 3;
            // 
            // textBox_Log
            // 
            this.textBox_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Log.Location = new System.Drawing.Point(12, 41);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.Size = new System.Drawing.Size(268, 105);
            this.textBox_Log.TabIndex = 4;
            // 
            // button_Send
            // 
            this.button_Send.Location = new System.Drawing.Point(286, 152);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(42, 23);
            this.button_Send.TabIndex = 5;
            this.button_Send.Text = "送信";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 202);
            this.Controls.Add(this.button_Send);
            this.Controls.Add(this.textBox_Log);
            this.Controls.Add(this.label_StatusBar);
            this.Controls.Add(this.textBox_Message);
            this.Controls.Add(this.button_Client);
            this.Controls.Add(this.button_Server);
            this.MinimumSize = new System.Drawing.Size(320, 240);
            this.Name = "Form1";
            this.Text = "ちょっと！ちょっとチャット！";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Server;
        private System.Windows.Forms.Button button_Client;
        public System.Windows.Forms.TextBox textBox_Message;
        public System.Windows.Forms.Label label_StatusBar;
        public System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.Button button_Send;
    }
}

