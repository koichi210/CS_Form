namespace othello
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
            this.pictureBoxField = new System.Windows.Forms.PictureBox();
            this.button_ReStart = new System.Windows.Forms.Button();
            this.label_Status = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuItem_Game = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_Start = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem_HowToPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_Version = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlayMode = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PP = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PC = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_CP = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_CC = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxField)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // pictureBoxField
            //
            this.pictureBoxField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxField.Location = new System.Drawing.Point(12, 36);
            this.pictureBoxField.Name = "pictureBoxField";
            this.pictureBoxField.Size = new System.Drawing.Size(200, 200);
            this.pictureBoxField.TabIndex = 0;
            this.pictureBoxField.TabStop = false;
            this.pictureBoxField.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxField_MouseClick);
            this.pictureBoxField.Resize += new System.EventHandler(this.pictureBoxField_Resize);
            //
            // label_Status
            //
            this.label_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Status.Location = new System.Drawing.Point(12, 240);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(200, 18);
            this.label_Status.TabIndex = 2;
            this.label_Status.Text = "label_Status";
            //
            // button_ReStart
            //
            this.button_ReStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_ReStart.Location = new System.Drawing.Point(137, 264);
            this.button_ReStart.Name = "button_ReStart";
            this.button_ReStart.Size = new System.Drawing.Size(75, 23);
            this.button_ReStart.TabIndex = 1;
            this.button_ReStart.Text = "ReStart";
            this.button_ReStart.UseVisualStyleBackColor = true;
            this.button_ReStart.Click += new System.EventHandler(this.button_ReStart_Click);
            //
            // menuStrip1
            //
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_Game,
            this.menuItem_PlayMode});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(224, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            //
            // menuItem_Game
            //
            this.menuItem_Game.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_Start,
            this.toolStripSeparator1,
            this.menuItem_Exit,
            this.toolStripSeparator2,
            this.menuItem_HowToPlay,
            this.menuItem_Version});
            this.menuItem_Game.Name = "menuItem_Game";
            this.menuItem_Game.Size = new System.Drawing.Size(58, 20);
            this.menuItem_Game.Text = "ゲーム(&G)";
            //
            // menuItem_Start
            //
            this.menuItem_Start.Name = "menuItem_Start";
            this.menuItem_Start.Size = new System.Drawing.Size(180, 22);
            this.menuItem_Start.Text = "開始(&S)";
            this.menuItem_Start.Click += new System.EventHandler(this.menuItem_Start_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            //
            // menuItem_Exit
            //
            this.menuItem_Exit.Name = "menuItem_Exit";
            this.menuItem_Exit.Size = new System.Drawing.Size(180, 22);
            this.menuItem_Exit.Text = "終了(&X)";
            this.menuItem_Exit.Click += new System.EventHandler(this.menuItem_Exit_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            //
            // menuItem_HowToPlay
            //
            this.menuItem_HowToPlay.Name = "menuItem_HowToPlay";
            this.menuItem_HowToPlay.Size = new System.Drawing.Size(180, 22);
            this.menuItem_HowToPlay.Text = "遊び方(&H)";
            this.menuItem_HowToPlay.Click += new System.EventHandler(this.menuItem_HowToPlay_Click);
            //
            // menuItem_Version
            //
            this.menuItem_Version.Name = "menuItem_Version";
            this.menuItem_Version.Size = new System.Drawing.Size(180, 22);
            this.menuItem_Version.Text = "バージョン情報(&A)";
            this.menuItem_Version.Click += new System.EventHandler(this.menuItem_Version_Click);
            //
            // menuItem_PlayMode
            //
            this.menuItem_PlayMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_PP,
            this.menuItem_PC,
            this.menuItem_CP,
            this.menuItem_CC});
            this.menuItem_PlayMode.Name = "menuItem_PlayMode";
            this.menuItem_PlayMode.Size = new System.Drawing.Size(76, 20);
            this.menuItem_PlayMode.Text = "対戦モード(&P)";
            //
            // menuItem_PP
            //
            this.menuItem_PP.Checked = true;
            this.menuItem_PP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItem_PP.Name = "menuItem_PP";
            this.menuItem_PP.Size = new System.Drawing.Size(180, 22);
            this.menuItem_PP.Text = "人 vs 人";
            this.menuItem_PP.Click += new System.EventHandler(this.menuItem_PlayMode_Click);
            //
            // menuItem_PC
            //
            this.menuItem_PC.Name = "menuItem_PC";
            this.menuItem_PC.Size = new System.Drawing.Size(180, 22);
            this.menuItem_PC.Text = "人 vs COM";
            this.menuItem_PC.Click += new System.EventHandler(this.menuItem_PlayMode_Click);
            //
            // menuItem_CP
            //
            this.menuItem_CP.Name = "menuItem_CP";
            this.menuItem_CP.Size = new System.Drawing.Size(180, 22);
            this.menuItem_CP.Text = "COM vs 人";
            this.menuItem_CP.Click += new System.EventHandler(this.menuItem_PlayMode_Click);
            //
            // menuItem_CC
            //
            this.menuItem_CC.Name = "menuItem_CC";
            this.menuItem_CC.Size = new System.Drawing.Size(180, 22);
            this.menuItem_CC.Text = "COM vs COM";
            this.menuItem_CC.Click += new System.EventHandler(this.menuItem_PlayMode_Click);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 298);
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.button_ReStart);
            this.Controls.Add(this.pictureBoxField);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "othello";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxField)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_ReStart;
        private System.Windows.Forms.Label label_Status;
        public System.Windows.Forms.PictureBox pictureBoxField;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Game;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Start;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Exit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuItem_HowToPlay;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Version;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlayMode;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PP;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PC;
        private System.Windows.Forms.ToolStripMenuItem menuItem_CP;
        private System.Windows.Forms.ToolStripMenuItem menuItem_CC;
    }
}

