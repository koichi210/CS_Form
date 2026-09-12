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
            this.dataGridView_Events = new StandardTemplate.DataGridViewEx();
            this.col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Detail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Wait = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip_Grid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItem_AddRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_DeleteRow = new System.Windows.Forms.ToolStripMenuItem();
            this.button_Record = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.label_Loop = new System.Windows.Forms.Label();
            this.textBox_Loop = new System.Windows.Forms.TextBox();
            this.button_Play = new System.Windows.Forms.Button();
            this.checkBox_MinimizeOnPlay = new System.Windows.Forms.CheckBox();
            this.comboBox_Profile = new System.Windows.Forms.ComboBox();
            this.button_ProfileSave = new System.Windows.Forms.Button();
            this.label_MousePos = new System.Windows.Forms.Label();
            this.radioButton_Playback = new System.Windows.Forms.RadioButton();
            this.radioButton_Record = new System.Windows.Forms.RadioButton();
            this.splitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.groupBox_Record = new System.Windows.Forms.GroupBox();
            this.groupBox_Playback = new System.Windows.Forms.GroupBox();
            this.label_PlaylistStatus = new System.Windows.Forms.Label();
            this.button_PlaylistListAll = new System.Windows.Forms.Button();
            this.dataGridView_Playlist = new StandardTemplate.DataGridViewEx();
            this.col_PlaylistEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_PlaylistFile = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.col_PlaylistLoopCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip_Playlist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItem_PlaylistAddRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistDeleteRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistDeleteRowSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem_PlaylistCheckAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistUncheckAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistFilterSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem_PlaylistShowCheckedOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistShowAll = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Events)).BeginInit();
            this.contextMenuStrip_Grid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).BeginInit();
            this.splitContainer_Main.Panel1.SuspendLayout();
            this.splitContainer_Main.Panel2.SuspendLayout();
            this.splitContainer_Main.SuspendLayout();
            this.groupBox_Record.SuspendLayout();
            this.groupBox_Playback.SuspendLayout();
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
            this.col_Detail,
            this.col_Wait,
            this.col_X,
            this.col_Y,
            this.col_Key,
            this.col_Remarks});
            this.dataGridView_Events.ContextMenuStrip = this.contextMenuStrip_Grid;
            this.dataGridView_Events.EnableUndoRedo = true;
            this.dataGridView_Events.Location = new System.Drawing.Point(6, 20);
            this.dataGridView_Events.Name = "dataGridView_Events";
            this.dataGridView_Events.RowHeadersWidth = 30;
            this.dataGridView_Events.Size = new System.Drawing.Size(318, 230);
            this.dataGridView_Events.TabIndex = 0;
            this.dataGridView_Events.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView_Events_CellFormatting);
            this.dataGridView_Events.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_Events_CellMouseDown);
            this.dataGridView_Events.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Events_CellValueChanged);
            this.dataGridView_Events.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_Events_DataError);
            this.dataGridView_Events.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_Events_RowPostPaint);
            this.dataGridView_Events.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_Events_KeyDown);
            // 
            // col_Type
            // 
            this.col_Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col_Type.HeaderText = "Event";
            this.col_Type.Name = "col_Type";
            this.col_Type.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Type.Width = 67;
            // 
            // col_Detail
            // 
            this.col_Detail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col_Detail.HeaderText = "Detail";
            this.col_Detail.Name = "col_Detail";
            this.col_Detail.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Detail.Width = 67;
            // 
            // col_Wait
            // 
            this.col_Wait.HeaderText = "Delay(ms)";
            this.col_Wait.Name = "col_Wait";
            this.col_Wait.Visible = false;
            // 
            // col_X
            // 
            this.col_X.HeaderText = "X";
            this.col_X.Name = "col_X";
            this.col_X.Visible = false;
            // 
            // col_Y
            // 
            this.col_Y.HeaderText = "Y";
            this.col_Y.Name = "col_Y";
            this.col_Y.Visible = false;
            // 
            // col_Key
            // 
            this.col_Key.HeaderText = "Key";
            this.col_Key.Name = "col_Key";
            this.col_Key.Visible = false;
            // 
            // col_Remarks
            // 
            this.col_Remarks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_Remarks.HeaderText = "備考";
            this.col_Remarks.Name = "col_Remarks";
            // 
            // contextMenuStrip_Grid
            // 
            this.contextMenuStrip_Grid.ImageScalingSize = new System.Drawing.Size(24, 24);
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
            this.button_Record.Location = new System.Drawing.Point(6, 261);
            this.button_Record.Name = "button_Record";
            this.button_Record.Size = new System.Drawing.Size(80, 27);
            this.button_Record.TabIndex = 1;
            this.button_Record.Text = "記録";
            this.button_Record.UseVisualStyleBackColor = true;
            this.button_Record.Click += new System.EventHandler(this.button_Record_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Clear.Location = new System.Drawing.Point(92, 261);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(80, 27);
            this.button_Clear.TabIndex = 2;
            this.button_Clear.Text = "クリア";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // label_Loop
            // 
            this.label_Loop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Loop.AutoSize = true;
            this.label_Loop.Location = new System.Drawing.Point(12, 337);
            this.label_Loop.Name = "label_Loop";
            this.label_Loop.Size = new System.Drawing.Size(46, 12);
            this.label_Loop.TabIndex = 4;
            this.label_Loop.Text = "ループ数";
            // 
            // textBox_Loop
            // 
            this.textBox_Loop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_Loop.Location = new System.Drawing.Point(74, 333);
            this.textBox_Loop.Name = "textBox_Loop";
            this.textBox_Loop.Size = new System.Drawing.Size(48, 19);
            this.textBox_Loop.TabIndex = 5;
            this.textBox_Loop.Text = "1";
            // 
            // button_Play
            // 
            this.button_Play.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Play.Location = new System.Drawing.Point(130, 330);
            this.button_Play.Name = "button_Play";
            this.button_Play.Size = new System.Drawing.Size(80, 27);
            this.button_Play.TabIndex = 6;
            this.button_Play.Text = "再生";
            this.button_Play.UseVisualStyleBackColor = true;
            this.button_Play.Click += new System.EventHandler(this.button_Play_Click);
            // 
            // checkBox_MinimizeOnPlay
            // 
            this.checkBox_MinimizeOnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_MinimizeOnPlay.AutoSize = true;
            this.checkBox_MinimizeOnPlay.Location = new System.Drawing.Point(220, 337);
            this.checkBox_MinimizeOnPlay.Name = "checkBox_MinimizeOnPlay";
            this.checkBox_MinimizeOnPlay.Size = new System.Drawing.Size(176, 16);
            this.checkBox_MinimizeOnPlay.TabIndex = 7;
            this.checkBox_MinimizeOnPlay.Text = "実行時にウィンドウを最小化する";
            this.checkBox_MinimizeOnPlay.UseVisualStyleBackColor = true;
            // 
            // comboBox_Profile
            // 
            this.comboBox_Profile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_Profile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Profile.FormattingEnabled = true;
            this.comboBox_Profile.Location = new System.Drawing.Point(12, 361);
            this.comboBox_Profile.Name = "comboBox_Profile";
            this.comboBox_Profile.Size = new System.Drawing.Size(456, 20);
            this.comboBox_Profile.TabIndex = 8;
            this.comboBox_Profile.SelectedIndexChanged += new System.EventHandler(this.comboBox_Profile_SelectedIndexChanged);
            // 
            // button_ProfileSave
            // 
            this.button_ProfileSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ProfileSave.Location = new System.Drawing.Point(474, 361);
            this.button_ProfileSave.Name = "button_ProfileSave";
            this.button_ProfileSave.Size = new System.Drawing.Size(108, 23);
            this.button_ProfileSave.TabIndex = 9;
            this.button_ProfileSave.Text = "プロファイル保存";
            this.button_ProfileSave.UseVisualStyleBackColor = true;
            this.button_ProfileSave.Click += new System.EventHandler(this.button_ProfileSave_Click);
            // 
            // label_MousePos
            // 
            this.label_MousePos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_MousePos.AutoSize = true;
            this.label_MousePos.Location = new System.Drawing.Point(513, 9);
            this.label_MousePos.Name = "label_MousePos";
            this.label_MousePos.Size = new System.Drawing.Size(62, 12);
            this.label_MousePos.TabIndex = 2;
            this.label_MousePos.Text = "Mouse: -, -";
            // 
            // radioButton_Playback
            // 
            this.radioButton_Playback.AutoSize = true;
            this.radioButton_Playback.Location = new System.Drawing.Point(111, 5);
            this.radioButton_Playback.Name = "radioButton_Playback";
            this.radioButton_Playback.Size = new System.Drawing.Size(75, 16);
            this.radioButton_Playback.TabIndex = 1;
            this.radioButton_Playback.TabStop = true;
            this.radioButton_Playback.Text = "プレイバック";
            this.radioButton_Playback.UseVisualStyleBackColor = true;
            this.radioButton_Playback.CheckedChanged += new System.EventHandler(this.radioButton_Mode_CheckedChanged);
            // 
            // radioButton_Record
            // 
            this.radioButton_Record.AutoSize = true;
            this.radioButton_Record.Location = new System.Drawing.Point(16, 5);
            this.radioButton_Record.Name = "radioButton_Record";
            this.radioButton_Record.Size = new System.Drawing.Size(59, 16);
            this.radioButton_Record.TabIndex = 0;
            this.radioButton_Record.TabStop = true;
            this.radioButton_Record.Text = "レコード";
            this.radioButton_Record.UseVisualStyleBackColor = true;
            this.radioButton_Record.CheckedChanged += new System.EventHandler(this.radioButton_Mode_CheckedChanged);
            // 
            // splitContainer_Main
            // 
            this.splitContainer_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer_Main.BackColor = System.Drawing.Color.SteelBlue;
            this.splitContainer_Main.Location = new System.Drawing.Point(5, 28);
            this.splitContainer_Main.Name = "splitContainer_Main";
            // 
            // splitContainer_Main.Panel1
            // 
            this.splitContainer_Main.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer_Main.Panel1.Controls.Add(this.groupBox_Record);
            this.splitContainer_Main.Panel1MinSize = 120;
            // 
            // splitContainer_Main.Panel2
            // 
            this.splitContainer_Main.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer_Main.Panel2.Controls.Add(this.groupBox_Playback);
            this.splitContainer_Main.Panel2MinSize = 120;
            this.splitContainer_Main.Size = new System.Drawing.Size(577, 295);
            this.splitContainer_Main.SplitterDistance = 330;
            this.splitContainer_Main.TabIndex = 3;
            // 
            // groupBox_Record
            // 
            this.groupBox_Record.Controls.Add(this.dataGridView_Events);
            this.groupBox_Record.Controls.Add(this.button_Record);
            this.groupBox_Record.Controls.Add(this.button_Clear);
            this.groupBox_Record.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Record.Location = new System.Drawing.Point(0, 0);
            this.groupBox_Record.Name = "groupBox_Record";
            this.groupBox_Record.Size = new System.Drawing.Size(330, 295);
            this.groupBox_Record.TabIndex = 0;
            this.groupBox_Record.TabStop = false;
            this.groupBox_Record.Text = "レコード";
            // 
            // groupBox_Playback
            // 
            this.groupBox_Playback.Controls.Add(this.label_PlaylistStatus);
            this.groupBox_Playback.Controls.Add(this.button_PlaylistListAll);
            this.groupBox_Playback.Controls.Add(this.dataGridView_Playlist);
            this.groupBox_Playback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Playback.Location = new System.Drawing.Point(0, 0);
            this.groupBox_Playback.Name = "groupBox_Playback";
            this.groupBox_Playback.Size = new System.Drawing.Size(243, 295);
            this.groupBox_Playback.TabIndex = 0;
            this.groupBox_Playback.TabStop = false;
            this.groupBox_Playback.Text = "プレイバック";
            // 
            // label_PlaylistStatus
            // 
            this.label_PlaylistStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_PlaylistStatus.AutoSize = true;
            this.label_PlaylistStatus.Location = new System.Drawing.Point(214, 253);
            this.label_PlaylistStatus.Name = "label_PlaylistStatus";
            this.label_PlaylistStatus.Size = new System.Drawing.Size(0, 12);
            this.label_PlaylistStatus.TabIndex = 2;
            // 
            // button_PlaylistListAll
            // 
            this.button_PlaylistListAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_PlaylistListAll.Location = new System.Drawing.Point(6, 261);
            this.button_PlaylistListAll.Name = "button_PlaylistListAll";
            this.button_PlaylistListAll.Size = new System.Drawing.Size(200, 27);
            this.button_PlaylistListAll.TabIndex = 1;
            this.button_PlaylistListAll.Text = "プレイリストを更新";
            this.button_PlaylistListAll.UseVisualStyleBackColor = true;
            this.button_PlaylistListAll.Click += new System.EventHandler(this.button_PlaylistListAll_Click);
            // 
            // dataGridView_Playlist
            // 
            this.dataGridView_Playlist.AllowDrop = true;
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
            this.dataGridView_Playlist.EnableUndoRedo = true;
            this.dataGridView_Playlist.Location = new System.Drawing.Point(6, 20);
            this.dataGridView_Playlist.Name = "dataGridView_Playlist";
            this.dataGridView_Playlist.RowHeadersWidth = 30;
            this.dataGridView_Playlist.Size = new System.Drawing.Size(231, 230);
            this.dataGridView_Playlist.TabIndex = 0;
            this.dataGridView_Playlist.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_Playlist_CellMouseDown);
            this.dataGridView_Playlist.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Playlist_CellValueChanged);
            this.dataGridView_Playlist.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_Playlist_CurrentCellDirtyStateChanged);
            this.dataGridView_Playlist.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_Playlist_DataError);
            this.dataGridView_Playlist.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_Events_RowPostPaint);
            this.dataGridView_Playlist.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView_Playlist_DragDrop);
            this.dataGridView_Playlist.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridView_Playlist_DragOver);
            this.dataGridView_Playlist.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_Playlist_KeyDown);
            this.dataGridView_Playlist.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_Playlist_MouseDown);
            this.dataGridView_Playlist.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridView_Playlist_MouseMove);
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
            this.col_PlaylistFile.HeaderText = "プレイリスト";
            this.col_PlaylistFile.Name = "col_PlaylistFile";
            // 
            // col_PlaylistLoopCount
            // 
            this.col_PlaylistLoopCount.HeaderText = "ループ数";
            this.col_PlaylistLoopCount.Name = "col_PlaylistLoopCount";
            this.col_PlaylistLoopCount.Width = 56;
            // 
            // contextMenuStrip_Playlist
            // 
            this.contextMenuStrip_Playlist.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip_Playlist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_PlaylistAddRow,
            this.menuItem_PlaylistDeleteRow,
            this.menuItem_PlaylistDeleteRowSeparator,
            this.menuItem_PlaylistCheckAll,
            this.menuItem_PlaylistUncheckAll,
            this.menuItem_PlaylistFilterSeparator,
            this.menuItem_PlaylistShowCheckedOnly,
            this.menuItem_PlaylistShowAll});
            this.contextMenuStrip_Playlist.Name = "contextMenuStrip_Playlist";
            this.contextMenuStrip_Playlist.Size = new System.Drawing.Size(204, 148);
            this.contextMenuStrip_Playlist.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Playlist_Opening);
            // 
            // menuItem_PlaylistAddRow
            // 
            this.menuItem_PlaylistAddRow.Name = "menuItem_PlaylistAddRow";
            this.menuItem_PlaylistAddRow.Size = new System.Drawing.Size(203, 22);
            this.menuItem_PlaylistAddRow.Text = "行の追加";
            this.menuItem_PlaylistAddRow.Click += new System.EventHandler(this.menuItem_PlaylistAddRow_Click);
            // 
            // menuItem_PlaylistDeleteRow
            // 
            this.menuItem_PlaylistDeleteRow.Name = "menuItem_PlaylistDeleteRow";
            this.menuItem_PlaylistDeleteRow.Size = new System.Drawing.Size(203, 22);
            this.menuItem_PlaylistDeleteRow.Text = "行の削除";
            this.menuItem_PlaylistDeleteRow.Click += new System.EventHandler(this.menuItem_PlaylistDeleteRow_Click);
            // 
            // menuItem_PlaylistDeleteRowSeparator
            // 
            this.menuItem_PlaylistDeleteRowSeparator.Name = "menuItem_PlaylistDeleteRowSeparator";
            this.menuItem_PlaylistDeleteRowSeparator.Size = new System.Drawing.Size(200, 6);
            // 
            // menuItem_PlaylistCheckAll
            // 
            this.menuItem_PlaylistCheckAll.Name = "menuItem_PlaylistCheckAll";
            this.menuItem_PlaylistCheckAll.Size = new System.Drawing.Size(203, 22);
            this.menuItem_PlaylistCheckAll.Text = "全チェックON";
            this.menuItem_PlaylistCheckAll.Click += new System.EventHandler(this.menuItem_PlaylistCheckAll_Click);
            // 
            // menuItem_PlaylistUncheckAll
            // 
            this.menuItem_PlaylistUncheckAll.Name = "menuItem_PlaylistUncheckAll";
            this.menuItem_PlaylistUncheckAll.Size = new System.Drawing.Size(203, 22);
            this.menuItem_PlaylistUncheckAll.Text = "全チェックOFF";
            this.menuItem_PlaylistUncheckAll.Click += new System.EventHandler(this.menuItem_PlaylistUncheckAll_Click);
            // 
            // menuItem_PlaylistFilterSeparator
            // 
            this.menuItem_PlaylistFilterSeparator.Name = "menuItem_PlaylistFilterSeparator";
            this.menuItem_PlaylistFilterSeparator.Size = new System.Drawing.Size(200, 6);
            // 
            // menuItem_PlaylistShowCheckedOnly
            // 
            this.menuItem_PlaylistShowCheckedOnly.Name = "menuItem_PlaylistShowCheckedOnly";
            this.menuItem_PlaylistShowCheckedOnly.Size = new System.Drawing.Size(203, 22);
            this.menuItem_PlaylistShowCheckedOnly.Text = "チェックONのみ表示";
            this.menuItem_PlaylistShowCheckedOnly.Click += new System.EventHandler(this.menuItem_PlaylistShowCheckedOnly_Click);
            // 
            // menuItem_PlaylistShowAll
            // 
            this.menuItem_PlaylistShowAll.Name = "menuItem_PlaylistShowAll";
            this.menuItem_PlaylistShowAll.Size = new System.Drawing.Size(203, 22);
            this.menuItem_PlaylistShowAll.Text = "チェックON・OFFともに表示";
            this.menuItem_PlaylistShowAll.Click += new System.EventHandler(this.menuItem_PlaylistShowAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 389);
            this.Controls.Add(this.splitContainer_Main);
            this.Controls.Add(this.radioButton_Record);
            this.Controls.Add(this.radioButton_Playback);
            this.Controls.Add(this.label_MousePos);
            this.Controls.Add(this.button_ProfileSave);
            this.Controls.Add(this.comboBox_Profile);
            this.Controls.Add(this.button_Play);
            this.Controls.Add(this.checkBox_MinimizeOnPlay);
            this.Controls.Add(this.textBox_Loop);
            this.Controls.Add(this.label_Loop);
            this.MinimumSize = new System.Drawing.Size(427, 257);
            this.Name = "Form1";
            this.Text = "EventRecorder";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Events)).EndInit();
            this.contextMenuStrip_Grid.ResumeLayout(false);
            this.splitContainer_Main.Panel1.ResumeLayout(false);
            this.splitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).EndInit();
            this.splitContainer_Main.ResumeLayout(false);
            this.groupBox_Record.ResumeLayout(false);
            this.groupBox_Playback.ResumeLayout(false);
            this.groupBox_Playback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Playlist)).EndInit();
            this.contextMenuStrip_Playlist.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal StandardTemplate.DataGridViewEx dataGridView_Events;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_X;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Wait;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Detail;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Remarks;
        private System.Windows.Forms.Button button_Record;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Label label_Loop;
        internal System.Windows.Forms.TextBox textBox_Loop;
        private System.Windows.Forms.Button button_Play;
        internal System.Windows.Forms.CheckBox checkBox_MinimizeOnPlay;
        internal System.Windows.Forms.ComboBox comboBox_Profile;
        private System.Windows.Forms.Button button_ProfileSave;
        internal System.Windows.Forms.Label label_MousePos;
        private System.Windows.Forms.RadioButton radioButton_Playback;
        private System.Windows.Forms.RadioButton radioButton_Record;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Grid;
        private System.Windows.Forms.ToolStripMenuItem menuItem_AddRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_DeleteRow;
        private System.Windows.Forms.SplitContainer splitContainer_Main;
        private System.Windows.Forms.GroupBox groupBox_Playback;
        private System.Windows.Forms.GroupBox groupBox_Record;
        internal StandardTemplate.DataGridViewEx dataGridView_Playlist;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_PlaylistEnabled;
        private System.Windows.Forms.DataGridViewComboBoxColumn col_PlaylistFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PlaylistLoopCount;
        internal System.Windows.Forms.Label label_PlaylistStatus;
        private System.Windows.Forms.Button button_PlaylistListAll;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Playlist;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistAddRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistDeleteRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistCheckAll;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistUncheckAll;
        private System.Windows.Forms.ToolStripSeparator menuItem_PlaylistDeleteRowSeparator;
        private System.Windows.Forms.ToolStripSeparator menuItem_PlaylistFilterSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistShowCheckedOnly;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistShowAll;
    }
}
