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
            this.components = new System.ComponentModel.Container();
            this.dataGridView_Events = new System.Windows.Forms.DataGridView();
            this.col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Wait = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip_Grid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItem_AddRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_DeleteRow = new System.Windows.Forms.ToolStripMenuItem();
            this.button_Record = new System.Windows.Forms.Button();
            this.button_Play = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.label_Loop = new System.Windows.Forms.Label();
            this.textBox_Loop = new System.Windows.Forms.TextBox();
            this.comboBox_Profile = new System.Windows.Forms.ComboBox();
            this.button_ProfileSave = new System.Windows.Forms.Button();
            this.label_MousePos = new System.Windows.Forms.Label();
            this.tabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPage_Record = new System.Windows.Forms.TabPage();
            this.tabPage_Playlist = new System.Windows.Forms.TabPage();
            this.label_PlaylistStatus = new System.Windows.Forms.Label();
            this.textBox_PlaylistLoop = new System.Windows.Forms.TextBox();
            this.label_PlaylistLoop = new System.Windows.Forms.Label();
            this.button_PlaylistRun = new System.Windows.Forms.Button();
            this.dataGridView_Playlist = new System.Windows.Forms.DataGridView();
            this.col_PlaylistEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_PlaylistFile = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.col_PlaylistLoopCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip_Playlist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItem_PlaylistAddRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistDeleteRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistCheckAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistUncheckAll = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Events)).BeginInit();
            this.contextMenuStrip_Grid.SuspendLayout();
            this.tabControl_Main.SuspendLayout();
            this.tabPage_Record.SuspendLayout();
            this.tabPage_Playlist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Playlist)).BeginInit();
            this.contextMenuStrip_Playlist.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_Events
            // 
            this.dataGridView_Events.AllowUserToAddRows = false;
            this.dataGridView_Events.AllowUserToResizeRows = false;
            this.dataGridView_Events.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Events.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_Events.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Type,
            this.col_X,
            this.col_Y,
            this.col_Key,
            this.col_Wait});
            this.dataGridView_Events.ContextMenuStrip = this.contextMenuStrip_Grid;
            this.dataGridView_Events.Location = new System.Drawing.Point(6, 6);
            this.dataGridView_Events.Name = "dataGridView_Events";
            this.dataGridView_Events.RowHeadersWidth = 30;
            this.dataGridView_Events.Size = new System.Drawing.Size(385, 158);
            this.dataGridView_Events.TabIndex = 0;
            this.dataGridView_Events.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView_Events_CellFormatting);
            this.dataGridView_Events.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_Events_CellMouseDown);
            this.dataGridView_Events.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Events_CellValueChanged);
            this.dataGridView_Events.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_Events_RowPostPaint);
            this.dataGridView_Events.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_Events_KeyDown);
            //
            // col_Type
            //
            // Fillモードでの各列の配分比率(FillWeight)。Eventを広めにしつつ、
            // 広げすぎた分(160→130、約8割)はX/Y/Keyに均等に割り戻す
            this.col_Type.FillWeight = 130F;
            this.col_Type.HeaderText = "Event";
            this.col_Type.Name = "col_Type";
            //
            // col_X
            //
            this.col_X.FillWeight = 70F;
            this.col_X.HeaderText = "X";
            this.col_X.Name = "col_X";
            //
            // col_Y
            //
            this.col_Y.FillWeight = 70F;
            this.col_Y.HeaderText = "Y";
            this.col_Y.Name = "col_Y";
            //
            // col_Key
            //
            this.col_Key.FillWeight = 70F;
            this.col_Key.HeaderText = "Key";
            this.col_Key.Name = "col_Key";
            //
            // col_Wait
            //
            this.col_Wait.FillWeight = 100F;
            this.col_Wait.HeaderText = "Delay(ms)";
            this.col_Wait.Name = "col_Wait";
            // 
            // contextMenuStrip_Grid
            // 
            this.contextMenuStrip_Grid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_AddRow,
            this.menuItem_DeleteRow});
            this.contextMenuStrip_Grid.Name = "contextMenuStrip_Grid";
            this.contextMenuStrip_Grid.Size = new System.Drawing.Size(121, 48);
            this.contextMenuStrip_Grid.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Grid_Opening);
            // 
            // menuItem_AddRow
            // 
            this.menuItem_AddRow.Name = "menuItem_AddRow";
            this.menuItem_AddRow.Size = new System.Drawing.Size(120, 22);
            this.menuItem_AddRow.Text = "行の追加";
            this.menuItem_AddRow.Click += new System.EventHandler(this.menuItem_AddRow_Click);
            // 
            // menuItem_DeleteRow
            // 
            this.menuItem_DeleteRow.Name = "menuItem_DeleteRow";
            this.menuItem_DeleteRow.Size = new System.Drawing.Size(120, 22);
            this.menuItem_DeleteRow.Text = "行の削除";
            this.menuItem_DeleteRow.Click += new System.EventHandler(this.menuItem_DeleteRow_Click);
            // 
            // button_Record
            // 
            this.button_Record.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Record.Location = new System.Drawing.Point(138, 170);
            this.button_Record.Name = "button_Record";
            this.button_Record.Size = new System.Drawing.Size(80, 27);
            this.button_Record.TabIndex = 3;
            this.button_Record.Text = "記録";
            this.button_Record.UseVisualStyleBackColor = true;
            this.button_Record.Click += new System.EventHandler(this.button_Record_Click);
            // 
            // button_Play
            // 
            this.button_Play.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Play.Location = new System.Drawing.Point(224, 170);
            this.button_Play.Name = "button_Play";
            this.button_Play.Size = new System.Drawing.Size(80, 27);
            this.button_Play.TabIndex = 4;
            this.button_Play.Text = "再生";
            this.button_Play.UseVisualStyleBackColor = true;
            this.button_Play.Click += new System.EventHandler(this.button_Play_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Clear.Location = new System.Drawing.Point(310, 170);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(80, 27);
            this.button_Clear.TabIndex = 5;
            this.button_Clear.Text = "クリア";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // label_Loop
            // 
            this.label_Loop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Loop.AutoSize = true;
            this.label_Loop.Location = new System.Drawing.Point(16, 178);
            this.label_Loop.Name = "label_Loop";
            this.label_Loop.Size = new System.Drawing.Size(58, 12);
            this.label_Loop.TabIndex = 1;
            this.label_Loop.Text = "ループ回数";
            // 
            // textBox_Loop
            // 
            this.textBox_Loop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_Loop.Location = new System.Drawing.Point(78, 174);
            this.textBox_Loop.Name = "textBox_Loop";
            this.textBox_Loop.Size = new System.Drawing.Size(48, 19);
            this.textBox_Loop.TabIndex = 2;
            this.textBox_Loop.Text = "1";
            // 
            // comboBox_Profile
            // 
            this.comboBox_Profile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_Profile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Profile.FormattingEnabled = true;
            this.comboBox_Profile.Location = new System.Drawing.Point(12, 235);
            this.comboBox_Profile.Name = "comboBox_Profile";
            this.comboBox_Profile.Size = new System.Drawing.Size(295, 20);
            this.comboBox_Profile.TabIndex = 2;
            this.comboBox_Profile.SelectedIndexChanged += new System.EventHandler(this.comboBox_Profile_SelectedIndexChanged);
            // 
            // button_ProfileSave
            // 
            this.button_ProfileSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ProfileSave.Location = new System.Drawing.Point(318, 234);
            this.button_ProfileSave.Name = "button_ProfileSave";
            this.button_ProfileSave.Size = new System.Drawing.Size(80, 23);
            this.button_ProfileSave.TabIndex = 3;
            this.button_ProfileSave.Text = "設定値保存";
            this.button_ProfileSave.UseVisualStyleBackColor = true;
            this.button_ProfileSave.Click += new System.EventHandler(this.button_ProfileSave_Click);
            // 
            // label_MousePos
            // 
            this.label_MousePos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_MousePos.AutoSize = true;
            this.label_MousePos.Location = new System.Drawing.Point(299, 7);
            this.label_MousePos.Name = "label_MousePos";
            this.label_MousePos.Size = new System.Drawing.Size(98, 12);
            this.label_MousePos.TabIndex = 1;
            this.label_MousePos.Text = "Mouse: 1234, 1234";
            // 
            // tabControl_Main
            // 
            this.tabControl_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl_Main.Controls.Add(this.tabPage_Record);
            this.tabControl_Main.Controls.Add(this.tabPage_Playlist);
            this.tabControl_Main.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Main.Name = "tabControl_Main";
            this.tabControl_Main.SelectedIndex = 0;
            this.tabControl_Main.Size = new System.Drawing.Size(405, 228);
            this.tabControl_Main.TabIndex = 0;
            this.tabControl_Main.SelectedIndexChanged += new System.EventHandler(this.tabControl_Main_SelectedIndexChanged);
            // 
            // tabPage_Record
            // 
            this.tabPage_Record.Controls.Add(this.textBox_Loop);
            this.tabPage_Record.Controls.Add(this.label_Loop);
            this.tabPage_Record.Controls.Add(this.button_Clear);
            this.tabPage_Record.Controls.Add(this.button_Play);
            this.tabPage_Record.Controls.Add(this.button_Record);
            this.tabPage_Record.Controls.Add(this.dataGridView_Events);
            this.tabPage_Record.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Record.Name = "tabPage_Record";
            this.tabPage_Record.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Record.Size = new System.Drawing.Size(397, 202);
            this.tabPage_Record.TabIndex = 0;
            this.tabPage_Record.Text = "記録・再生";
            this.tabPage_Record.UseVisualStyleBackColor = true;
            // 
            // tabPage_Playlist
            // 
            this.tabPage_Playlist.Controls.Add(this.label_PlaylistStatus);
            this.tabPage_Playlist.Controls.Add(this.textBox_PlaylistLoop);
            this.tabPage_Playlist.Controls.Add(this.label_PlaylistLoop);
            this.tabPage_Playlist.Controls.Add(this.button_PlaylistRun);
            this.tabPage_Playlist.Controls.Add(this.dataGridView_Playlist);
            this.tabPage_Playlist.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Playlist.Name = "tabPage_Playlist";
            this.tabPage_Playlist.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Playlist.Size = new System.Drawing.Size(397, 202);
            this.tabPage_Playlist.TabIndex = 1;
            this.tabPage_Playlist.Text = "プレイリスト";
            this.tabPage_Playlist.UseVisualStyleBackColor = true;
            // 
            // label_PlaylistStatus
            // 
            this.label_PlaylistStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_PlaylistStatus.AutoSize = true;
            this.label_PlaylistStatus.Location = new System.Drawing.Point(227, 178);
            this.label_PlaylistStatus.Name = "label_PlaylistStatus";
            this.label_PlaylistStatus.Size = new System.Drawing.Size(0, 12);
            this.label_PlaylistStatus.TabIndex = 4;
            // 
            // textBox_PlaylistLoop
            // 
            this.textBox_PlaylistLoop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_PlaylistLoop.Location = new System.Drawing.Point(78, 174);
            this.textBox_PlaylistLoop.Name = "textBox_PlaylistLoop";
            this.textBox_PlaylistLoop.Size = new System.Drawing.Size(48, 19);
            this.textBox_PlaylistLoop.TabIndex = 2;
            this.textBox_PlaylistLoop.Text = "1";
            // 
            // label_PlaylistLoop
            // 
            this.label_PlaylistLoop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_PlaylistLoop.AutoSize = true;
            this.label_PlaylistLoop.Location = new System.Drawing.Point(16, 178);
            this.label_PlaylistLoop.Name = "label_PlaylistLoop";
            this.label_PlaylistLoop.Size = new System.Drawing.Size(58, 12);
            this.label_PlaylistLoop.TabIndex = 1;
            this.label_PlaylistLoop.Text = "全体ループ";
            // 
            // button_PlaylistRun
            // 
            this.button_PlaylistRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_PlaylistRun.Location = new System.Drawing.Point(138, 170);
            this.button_PlaylistRun.Name = "button_PlaylistRun";
            this.button_PlaylistRun.Size = new System.Drawing.Size(80, 27);
            this.button_PlaylistRun.TabIndex = 3;
            this.button_PlaylistRun.Text = "実行";
            this.button_PlaylistRun.UseVisualStyleBackColor = true;
            this.button_PlaylistRun.Click += new System.EventHandler(this.button_PlaylistRun_Click);
            // 
            // dataGridView_Playlist
            // 
            this.dataGridView_Playlist.AllowUserToAddRows = false;
            this.dataGridView_Playlist.AllowUserToResizeRows = false;
            this.dataGridView_Playlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Playlist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_PlaylistEnabled,
            this.col_PlaylistFile,
            this.col_PlaylistLoopCount});
            this.dataGridView_Playlist.ContextMenuStrip = this.contextMenuStrip_Playlist;
            this.dataGridView_Playlist.Location = new System.Drawing.Point(6, 6);
            this.dataGridView_Playlist.Name = "dataGridView_Playlist";
            this.dataGridView_Playlist.RowHeadersWidth = 30;
            this.dataGridView_Playlist.Size = new System.Drawing.Size(383, 158);
            this.dataGridView_Playlist.TabIndex = 0;
            this.dataGridView_Playlist.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_Playlist_CellMouseDown);
            this.dataGridView_Playlist.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Playlist_CellValueChanged);
            this.dataGridView_Playlist.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_Playlist_CurrentCellDirtyStateChanged);
            this.dataGridView_Playlist.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_Events_RowPostPaint);
            // 
            // col_PlaylistEnabled
            // 
            this.col_PlaylistEnabled.HeaderText = "実行";
            this.col_PlaylistEnabled.Name = "col_PlaylistEnabled";
            this.col_PlaylistEnabled.Width = 40;
            // 
            // col_PlaylistFile
            // 
            this.col_PlaylistFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_PlaylistFile.HeaderText = "設定ファイル";
            this.col_PlaylistFile.Name = "col_PlaylistFile";
            // 
            // col_PlaylistLoopCount
            // 
            this.col_PlaylistLoopCount.HeaderText = "ループ回数";
            this.col_PlaylistLoopCount.Name = "col_PlaylistLoopCount";
            this.col_PlaylistLoopCount.Width = 70;
            // 
            // contextMenuStrip_Playlist
            // 
            this.contextMenuStrip_Playlist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_PlaylistAddRow,
            this.menuItem_PlaylistDeleteRow,
            this.menuItem_PlaylistCheckAll,
            this.menuItem_PlaylistUncheckAll});
            this.contextMenuStrip_Playlist.Name = "contextMenuStrip_Playlist";
            this.contextMenuStrip_Playlist.Size = new System.Drawing.Size(141, 92);
            this.contextMenuStrip_Playlist.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Playlist_Opening);
            // 
            // menuItem_PlaylistAddRow
            // 
            this.menuItem_PlaylistAddRow.Name = "menuItem_PlaylistAddRow";
            this.menuItem_PlaylistAddRow.Size = new System.Drawing.Size(140, 22);
            this.menuItem_PlaylistAddRow.Text = "行の追加";
            this.menuItem_PlaylistAddRow.Click += new System.EventHandler(this.menuItem_PlaylistAddRow_Click);
            // 
            // menuItem_PlaylistDeleteRow
            // 
            this.menuItem_PlaylistDeleteRow.Name = "menuItem_PlaylistDeleteRow";
            this.menuItem_PlaylistDeleteRow.Size = new System.Drawing.Size(140, 22);
            this.menuItem_PlaylistDeleteRow.Text = "行の削除";
            this.menuItem_PlaylistDeleteRow.Click += new System.EventHandler(this.menuItem_PlaylistDeleteRow_Click);
            // 
            // menuItem_PlaylistCheckAll
            // 
            this.menuItem_PlaylistCheckAll.Name = "menuItem_PlaylistCheckAll";
            this.menuItem_PlaylistCheckAll.Size = new System.Drawing.Size(140, 22);
            this.menuItem_PlaylistCheckAll.Text = "全チェックON";
            this.menuItem_PlaylistCheckAll.Click += new System.EventHandler(this.menuItem_PlaylistCheckAll_Click);
            // 
            // menuItem_PlaylistUncheckAll
            // 
            this.menuItem_PlaylistUncheckAll.Name = "menuItem_PlaylistUncheckAll";
            this.menuItem_PlaylistUncheckAll.Size = new System.Drawing.Size(140, 22);
            this.menuItem_PlaylistUncheckAll.Text = "全チェックOFF";
            this.menuItem_PlaylistUncheckAll.Click += new System.EventHandler(this.menuItem_PlaylistUncheckAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 261);
            this.Controls.Add(this.label_MousePos);
            this.Controls.Add(this.button_ProfileSave);
            this.Controls.Add(this.comboBox_Profile);
            this.Controls.Add(this.tabControl_Main);
            this.MinimumSize = new System.Drawing.Size(421, 300);
            this.Name = "Form1";
            this.Text = "EventRecorder";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Events)).EndInit();
            this.contextMenuStrip_Grid.ResumeLayout(false);
            this.tabControl_Main.ResumeLayout(false);
            this.tabPage_Record.ResumeLayout(false);
            this.tabPage_Record.PerformLayout();
            this.tabPage_Playlist.ResumeLayout(false);
            this.tabPage_Playlist.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Playlist)).EndInit();
            this.contextMenuStrip_Playlist.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.DataGridView dataGridView_Events;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_X;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Wait;
        private System.Windows.Forms.Button button_Record;
        private System.Windows.Forms.Button button_Play;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Label label_Loop;
        internal System.Windows.Forms.TextBox textBox_Loop;
        internal System.Windows.Forms.ComboBox comboBox_Profile;
        private System.Windows.Forms.Button button_ProfileSave;
        internal System.Windows.Forms.Label label_MousePos;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Grid;
        private System.Windows.Forms.ToolStripMenuItem menuItem_AddRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_DeleteRow;
        private System.Windows.Forms.TabControl tabControl_Main;
        private System.Windows.Forms.TabPage tabPage_Record;
        private System.Windows.Forms.TabPage tabPage_Playlist;
        internal System.Windows.Forms.DataGridView dataGridView_Playlist;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_PlaylistEnabled;
        private System.Windows.Forms.DataGridViewComboBoxColumn col_PlaylistFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PlaylistLoopCount;
        private System.Windows.Forms.Button button_PlaylistRun;
        private System.Windows.Forms.Label label_PlaylistLoop;
        internal System.Windows.Forms.TextBox textBox_PlaylistLoop;
        internal System.Windows.Forms.Label label_PlaylistStatus;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Playlist;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistAddRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistDeleteRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistCheckAll;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistUncheckAll;
    }
}
