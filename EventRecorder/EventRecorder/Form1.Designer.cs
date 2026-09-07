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
            this.dataGridView_Events = new System.Windows.Forms.DataGridView();
            this.col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Wait = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_Record = new System.Windows.Forms.Button();
            this.button_Play = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.label_Loop = new System.Windows.Forms.Label();
            this.textBox_Loop = new System.Windows.Forms.TextBox();
            this.comboBox_Profile = new System.Windows.Forms.ComboBox();
            this.button_ProfileLoad = new System.Windows.Forms.Button();
            this.button_ProfileSave = new System.Windows.Forms.Button();
            this.label_MousePos = new System.Windows.Forms.Label();
            this.contextMenuStrip_Grid = new System.Windows.Forms.ContextMenuStrip();
            this.menuItem_AddRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_DeleteRow = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPage_Record = new System.Windows.Forms.TabPage();
            this.tabPage_Playlist = new System.Windows.Forms.TabPage();
            this.dataGridView_Playlist = new System.Windows.Forms.DataGridView();
            this.col_PlaylistFile = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.button_PlaylistRun = new System.Windows.Forms.Button();
            this.label_PlaylistStatus = new System.Windows.Forms.Label();
            this.contextMenuStrip_Playlist = new System.Windows.Forms.ContextMenuStrip();
            this.menuItem_PlaylistAddRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_PlaylistDeleteRow = new System.Windows.Forms.ToolStripMenuItem();
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
            this.dataGridView_Events.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Type,
            this.col_X,
            this.col_Y,
            this.col_Key,
            this.col_Wait});
            this.dataGridView_Events.Location = new System.Drawing.Point(12, 12);
            this.dataGridView_Events.Name = "dataGridView_Events";
            this.dataGridView_Events.RowHeadersWidth = 30;
            this.dataGridView_Events.Size = new System.Drawing.Size(406, 296);
            this.dataGridView_Events.TabIndex = 0;
            // 右クリックで「行の追加/削除」を出せるようにする(ユーザーが手動でマクロを編集できるように)
            this.dataGridView_Events.ContextMenuStrip = this.contextMenuStrip_Grid;
            this.dataGridView_Events.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView_Events_CellFormatting);
            this.dataGridView_Events.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Events_CellValueChanged);
            this.dataGridView_Events.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_Events_RowPostPaint);
            this.dataGridView_Events.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_Events_CellMouseDown);
            this.dataGridView_Events.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_Events_KeyDown);
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
            // 「このイベントの前にどれだけ待つか」という意味を名前だけで伝わるようにDelayにした(旧: Wait(ms))
            this.col_Wait.HeaderText = "Delay(ms)";
            this.col_Wait.Name = "col_Wait";
            this.col_Wait.Width = 70;
            // 最後の列をFillにして、グリッド右端の余白を自動で埋める
            this.col_Wait.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
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
            // label_Loop
            //
            this.label_Loop.AutoSize = true;
            this.label_Loop.Location = new System.Drawing.Point(12, 357);
            this.label_Loop.Name = "label_Loop";
            this.label_Loop.Size = new System.Drawing.Size(56, 12);
            this.label_Loop.TabIndex = 4;
            this.label_Loop.Text = "ループ回数";
            //
            // textBox_Loop
            //
            this.textBox_Loop.Location = new System.Drawing.Point(74, 353);
            this.textBox_Loop.Name = "textBox_Loop";
            this.textBox_Loop.Size = new System.Drawing.Size(48, 19);
            this.textBox_Loop.TabIndex = 5;
            this.textBox_Loop.Text = "1";
            //
            // comboBox_Profile
            //
            // 設定ファイルの保存/読込は他プロジェクト(Cheetos等)に倣い、
            // 「設定ファイル一覧のコンボボックス + 読込/保存ボタン」の形にする
            this.comboBox_Profile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Profile.FormattingEnabled = true;
            this.comboBox_Profile.Location = new System.Drawing.Point(12, 384);
            this.comboBox_Profile.Name = "comboBox_Profile";
            this.comboBox_Profile.Size = new System.Drawing.Size(220, 20);
            this.comboBox_Profile.TabIndex = 6;
            this.comboBox_Profile.SelectedIndexChanged += new System.EventHandler(this.comboBox_Profile_SelectedIndexChanged);
            //
            // button_ProfileLoad
            //
            this.button_ProfileLoad.Location = new System.Drawing.Point(238, 383);
            this.button_ProfileLoad.Name = "button_ProfileLoad";
            this.button_ProfileLoad.Size = new System.Drawing.Size(80, 23);
            this.button_ProfileLoad.TabIndex = 7;
            this.button_ProfileLoad.Text = "設定値読込";
            this.button_ProfileLoad.UseVisualStyleBackColor = true;
            this.button_ProfileLoad.Click += new System.EventHandler(this.button_ProfileLoad_Click);
            //
            // button_ProfileSave
            //
            this.button_ProfileSave.Location = new System.Drawing.Point(324, 383);
            this.button_ProfileSave.Name = "button_ProfileSave";
            this.button_ProfileSave.Size = new System.Drawing.Size(80, 23);
            this.button_ProfileSave.TabIndex = 8;
            this.button_ProfileSave.Text = "設定値保存";
            this.button_ProfileSave.UseVisualStyleBackColor = true;
            this.button_ProfileSave.Click += new System.EventHandler(this.button_ProfileSave_Click);
            //
            // label_MousePos
            //
            // 起動中ずっとマウスカーソルの座標を表示しておく(画面端のステータス表示的な位置)
            this.label_MousePos.AutoSize = true;
            this.label_MousePos.Location = new System.Drawing.Point(12, 407);
            this.label_MousePos.Name = "label_MousePos";
            this.label_MousePos.Size = new System.Drawing.Size(76, 12);
            this.label_MousePos.TabIndex = 9;
            this.label_MousePos.Text = "Mouse: -, -";
            //
            // contextMenuStrip_Grid
            //
            this.contextMenuStrip_Grid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_AddRow,
            this.menuItem_DeleteRow});
            this.contextMenuStrip_Grid.Name = "contextMenuStrip_Grid";
            this.contextMenuStrip_Grid.Size = new System.Drawing.Size(122, 48);
            this.contextMenuStrip_Grid.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Grid_Opening);
            //
            // menuItem_AddRow
            //
            this.menuItem_AddRow.Name = "menuItem_AddRow";
            this.menuItem_AddRow.Size = new System.Drawing.Size(121, 22);
            this.menuItem_AddRow.Text = "行の追加";
            this.menuItem_AddRow.Click += new System.EventHandler(this.menuItem_AddRow_Click);
            //
            // menuItem_DeleteRow
            //
            this.menuItem_DeleteRow.Name = "menuItem_DeleteRow";
            this.menuItem_DeleteRow.Size = new System.Drawing.Size(121, 22);
            this.menuItem_DeleteRow.Text = "行の削除";
            this.menuItem_DeleteRow.Click += new System.EventHandler(this.menuItem_DeleteRow_Click);
            //
            // tabControl_Main
            //
            // タブ1に既存の記録・再生機能、タブ2に新規のプレイリスト機能を分離する
            this.tabControl_Main.Controls.Add(this.tabPage_Record);
            this.tabControl_Main.Controls.Add(this.tabPage_Playlist);
            this.tabControl_Main.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Main.Name = "tabControl_Main";
            this.tabControl_Main.SelectedIndex = 0;
            this.tabControl_Main.Size = new System.Drawing.Size(430, 468);
            this.tabControl_Main.TabIndex = 0;
            this.tabControl_Main.SelectedIndexChanged += new System.EventHandler(this.tabControl_Main_SelectedIndexChanged);
            //
            // tabPage_Record
            //
            this.tabPage_Record.Controls.Add(this.label_MousePos);
            this.tabPage_Record.Controls.Add(this.button_ProfileSave);
            this.tabPage_Record.Controls.Add(this.button_ProfileLoad);
            this.tabPage_Record.Controls.Add(this.comboBox_Profile);
            this.tabPage_Record.Controls.Add(this.textBox_Loop);
            this.tabPage_Record.Controls.Add(this.label_Loop);
            this.tabPage_Record.Controls.Add(this.button_Clear);
            this.tabPage_Record.Controls.Add(this.button_Play);
            this.tabPage_Record.Controls.Add(this.button_Record);
            this.tabPage_Record.Controls.Add(this.dataGridView_Events);
            this.tabPage_Record.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Record.Name = "tabPage_Record";
            this.tabPage_Record.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Record.Size = new System.Drawing.Size(422, 442);
            this.tabPage_Record.TabIndex = 0;
            this.tabPage_Record.Text = "記録・再生";
            this.tabPage_Record.UseVisualStyleBackColor = true;
            //
            // tabPage_Playlist
            //
            this.tabPage_Playlist.Controls.Add(this.label_PlaylistStatus);
            this.tabPage_Playlist.Controls.Add(this.button_PlaylistRun);
            this.tabPage_Playlist.Controls.Add(this.dataGridView_Playlist);
            this.tabPage_Playlist.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Playlist.Name = "tabPage_Playlist";
            this.tabPage_Playlist.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Playlist.Size = new System.Drawing.Size(422, 442);
            this.tabPage_Playlist.TabIndex = 1;
            this.tabPage_Playlist.Text = "プレイリスト";
            this.tabPage_Playlist.UseVisualStyleBackColor = true;
            //
            // dataGridView_Playlist
            //
            // 1タブ目の「設定値読込/保存」と同じ一覧(*.xml)から選ぶプルダウン列を1つだけ持つグリッド。
            // 上から順に実行する(並び替えは今回未対応、行のドラッグ&ドロップで別途対応予定)
            this.dataGridView_Playlist.AllowUserToAddRows = false;
            this.dataGridView_Playlist.AllowUserToResizeRows = false;
            this.dataGridView_Playlist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_PlaylistFile});
            this.dataGridView_Playlist.ContextMenuStrip = this.contextMenuStrip_Playlist;
            this.dataGridView_Playlist.Location = new System.Drawing.Point(6, 6);
            this.dataGridView_Playlist.Name = "dataGridView_Playlist";
            this.dataGridView_Playlist.RowHeadersWidth = 30;
            this.dataGridView_Playlist.Size = new System.Drawing.Size(410, 390);
            this.dataGridView_Playlist.TabIndex = 0;
            this.dataGridView_Playlist.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_Playlist_CellMouseDown);
            this.dataGridView_Playlist.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_Events_RowPostPaint);
            //
            // col_PlaylistFile
            //
            this.col_PlaylistFile.HeaderText = "設定ファイル";
            this.col_PlaylistFile.Name = "col_PlaylistFile";
            this.col_PlaylistFile.Width = 300;
            this.col_PlaylistFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //
            // button_PlaylistRun
            //
            this.button_PlaylistRun.Location = new System.Drawing.Point(6, 402);
            this.button_PlaylistRun.Name = "button_PlaylistRun";
            this.button_PlaylistRun.Size = new System.Drawing.Size(80, 27);
            this.button_PlaylistRun.TabIndex = 1;
            this.button_PlaylistRun.Text = "実行";
            this.button_PlaylistRun.UseVisualStyleBackColor = true;
            this.button_PlaylistRun.Click += new System.EventHandler(this.button_PlaylistRun_Click);
            //
            // label_PlaylistStatus
            //
            this.label_PlaylistStatus.AutoSize = true;
            this.label_PlaylistStatus.Location = new System.Drawing.Point(92, 409);
            this.label_PlaylistStatus.Name = "label_PlaylistStatus";
            this.label_PlaylistStatus.Size = new System.Drawing.Size(0, 12);
            this.label_PlaylistStatus.TabIndex = 2;
            //
            // contextMenuStrip_Playlist
            //
            this.contextMenuStrip_Playlist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_PlaylistAddRow,
            this.menuItem_PlaylistDeleteRow});
            this.contextMenuStrip_Playlist.Name = "contextMenuStrip_Playlist";
            this.contextMenuStrip_Playlist.Size = new System.Drawing.Size(122, 48);
            this.contextMenuStrip_Playlist.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Playlist_Opening);
            //
            // menuItem_PlaylistAddRow
            //
            this.menuItem_PlaylistAddRow.Name = "menuItem_PlaylistAddRow";
            this.menuItem_PlaylistAddRow.Size = new System.Drawing.Size(121, 22);
            this.menuItem_PlaylistAddRow.Text = "行の追加";
            this.menuItem_PlaylistAddRow.Click += new System.EventHandler(this.menuItem_PlaylistAddRow_Click);
            //
            // menuItem_PlaylistDeleteRow
            //
            this.menuItem_PlaylistDeleteRow.Name = "menuItem_PlaylistDeleteRow";
            this.menuItem_PlaylistDeleteRow.Size = new System.Drawing.Size(121, 22);
            this.menuItem_PlaylistDeleteRow.Text = "行の削除";
            this.menuItem_PlaylistDeleteRow.Click += new System.EventHandler(this.menuItem_PlaylistDeleteRow_Click);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 468);
            this.Controls.Add(this.tabControl_Main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
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
        private System.Windows.Forms.Button button_ProfileLoad;
        private System.Windows.Forms.Button button_ProfileSave;
        internal System.Windows.Forms.Label label_MousePos;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Grid;
        private System.Windows.Forms.ToolStripMenuItem menuItem_AddRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_DeleteRow;
        private System.Windows.Forms.TabControl tabControl_Main;
        private System.Windows.Forms.TabPage tabPage_Record;
        private System.Windows.Forms.TabPage tabPage_Playlist;
        internal System.Windows.Forms.DataGridView dataGridView_Playlist;
        private System.Windows.Forms.DataGridViewComboBoxColumn col_PlaylistFile;
        private System.Windows.Forms.Button button_PlaylistRun;
        internal System.Windows.Forms.Label label_PlaylistStatus;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Playlist;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistAddRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_PlaylistDeleteRow;
    }
}
