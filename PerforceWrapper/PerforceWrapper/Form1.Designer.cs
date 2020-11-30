namespace PerforceWrapper
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButton_so_menu_restore = new System.Windows.Forms.RadioButton();
            this.radioButton_so_menu_get_latest = new System.Windows.Forms.RadioButton();
            this.radioButton_so_menu_delete = new System.Windows.Forms.RadioButton();
            this.radioButton_so_menu_checkout = new System.Windows.Forms.RadioButton();
            this.textBox_tree_list = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_profile_save = new System.Windows.Forms.Button();
            this.comboBox_profile = new System.Windows.Forms.ComboBox();
            this.comboBox_perforce_server = new System.Windows.Forms.ComboBox();
            this.comboBox_perforce_user = new System.Windows.Forms.ComboBox();
            this.comboBox_perforce_workspace = new System.Windows.Forms.ComboBox();
            this.comboBox_perforce_charset = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textbox_perforce_password = new System.Windows.Forms.TextBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage_standard_Operation = new System.Windows.Forms.TabPage();
            this.textBox_so_changelist = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPage_set_label = new System.Windows.Forms.TabPage();
            this.textBox_sl_base_changelist = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_sl_label_name = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage_diff_label = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_dl_dest_tree = new System.Windows.Forms.TextBox();
            this.textBox_dl_dest_label_name = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_dl_src_tree = new System.Windows.Forms.TextBox();
            this.textBox_dl_src_label_name = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPage_apply_label = new System.Windows.Forms.TabPage();
            this.textBox_ak_label_name = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton_al_merge = new System.Windows.Forms.RadioButton();
            this.radioButton_al_copy = new System.Windows.Forms.RadioButton();
            this.textBox_ak_branch_map = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_execute = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPage_standard_Operation.SuspendLayout();
            this.tabPage_set_label.SuspendLayout();
            this.tabPage_diff_label.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage_apply_label.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "サーバー：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ユーザー：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "ワークスペース：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "文字コード：";
            // 
            // radioButton_so_menu_restore
            // 
            this.radioButton_so_menu_restore.AutoSize = true;
            this.radioButton_so_menu_restore.Location = new System.Drawing.Point(14, 100);
            this.radioButton_so_menu_restore.Name = "radioButton_so_menu_restore";
            this.radioButton_so_menu_restore.Size = new System.Drawing.Size(66, 16);
            this.radioButton_so_menu_restore.TabIndex = 110;
            this.radioButton_so_menu_restore.Text = "元に戻す";
            this.radioButton_so_menu_restore.UseVisualStyleBackColor = true;
            this.radioButton_so_menu_restore.CheckedChanged += new System.EventHandler(this.UpdateControlUI);
            // 
            // radioButton_so_menu_get_latest
            // 
            this.radioButton_so_menu_get_latest.AutoSize = true;
            this.radioButton_so_menu_get_latest.Location = new System.Drawing.Point(14, 6);
            this.radioButton_so_menu_get_latest.Name = "radioButton_so_menu_get_latest";
            this.radioButton_so_menu_get_latest.Size = new System.Drawing.Size(47, 16);
            this.radioButton_so_menu_get_latest.TabIndex = 100;
            this.radioButton_so_menu_get_latest.Text = "取得";
            this.radioButton_so_menu_get_latest.UseVisualStyleBackColor = true;
            this.radioButton_so_menu_get_latest.CheckedChanged += new System.EventHandler(this.UpdateControlUI);
            // 
            // radioButton_so_menu_delete
            // 
            this.radioButton_so_menu_delete.AutoSize = true;
            this.radioButton_so_menu_delete.Location = new System.Drawing.Point(14, 77);
            this.radioButton_so_menu_delete.Name = "radioButton_so_menu_delete";
            this.radioButton_so_menu_delete.Size = new System.Drawing.Size(47, 16);
            this.radioButton_so_menu_delete.TabIndex = 103;
            this.radioButton_so_menu_delete.Text = "削除";
            this.radioButton_so_menu_delete.UseVisualStyleBackColor = true;
            this.radioButton_so_menu_delete.CheckedChanged += new System.EventHandler(this.UpdateControlUI);
            // 
            // radioButton_so_menu_checkout
            // 
            this.radioButton_so_menu_checkout.AutoSize = true;
            this.radioButton_so_menu_checkout.Location = new System.Drawing.Point(14, 53);
            this.radioButton_so_menu_checkout.Name = "radioButton_so_menu_checkout";
            this.radioButton_so_menu_checkout.Size = new System.Drawing.Size(80, 16);
            this.radioButton_so_menu_checkout.TabIndex = 102;
            this.radioButton_so_menu_checkout.Text = "チェックアウト";
            this.radioButton_so_menu_checkout.UseVisualStyleBackColor = true;
            this.radioButton_so_menu_checkout.CheckedChanged += new System.EventHandler(this.UpdateControlUI);
            // 
            // textBox_tree_list
            // 
            this.textBox_tree_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_tree_list.Location = new System.Drawing.Point(12, 232);
            this.textBox_tree_list.Multiline = true;
            this.textBox_tree_list.Name = "textBox_tree_list";
            this.textBox_tree_list.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_tree_list.Size = new System.Drawing.Size(487, 111);
            this.textBox_tree_list.TabIndex = 20;
            this.textBox_tree_list.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_tree_list_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 217);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "ツリー：";
            // 
            // button_profile_save
            // 
            this.button_profile_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_profile_save.Location = new System.Drawing.Point(424, 374);
            this.button_profile_save.Name = "button_profile_save";
            this.button_profile_save.Size = new System.Drawing.Size(75, 23);
            this.button_profile_save.TabIndex = 51;
            this.button_profile_save.Text = "設定保存";
            this.button_profile_save.UseVisualStyleBackColor = true;
            this.button_profile_save.Click += new System.EventHandler(this.button_profile_save_Click);
            // 
            // comboBox_profile
            // 
            this.comboBox_profile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_profile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_profile.FormattingEnabled = true;
            this.comboBox_profile.Location = new System.Drawing.Point(13, 376);
            this.comboBox_profile.Name = "comboBox_profile";
            this.comboBox_profile.Size = new System.Drawing.Size(405, 20);
            this.comboBox_profile.TabIndex = 50;
            this.comboBox_profile.SelectedIndexChanged += new System.EventHandler(this.comboBox_profile_SelectedIndexChanged);
            // 
            // comboBox_perforce_server
            // 
            this.comboBox_perforce_server.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_perforce_server.FormattingEnabled = true;
            this.comboBox_perforce_server.Location = new System.Drawing.Point(91, 18);
            this.comboBox_perforce_server.Name = "comboBox_perforce_server";
            this.comboBox_perforce_server.Size = new System.Drawing.Size(169, 20);
            this.comboBox_perforce_server.Sorted = true;
            this.comboBox_perforce_server.TabIndex = 1;
            // 
            // comboBox_perforce_user
            // 
            this.comboBox_perforce_user.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_perforce_user.FormattingEnabled = true;
            this.comboBox_perforce_user.Location = new System.Drawing.Point(91, 43);
            this.comboBox_perforce_user.Name = "comboBox_perforce_user";
            this.comboBox_perforce_user.Size = new System.Drawing.Size(169, 20);
            this.comboBox_perforce_user.Sorted = true;
            this.comboBox_perforce_user.TabIndex = 2;
            // 
            // comboBox_perforce_workspace
            // 
            this.comboBox_perforce_workspace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_perforce_workspace.FormattingEnabled = true;
            this.comboBox_perforce_workspace.Location = new System.Drawing.Point(91, 93);
            this.comboBox_perforce_workspace.Name = "comboBox_perforce_workspace";
            this.comboBox_perforce_workspace.Size = new System.Drawing.Size(169, 20);
            this.comboBox_perforce_workspace.Sorted = true;
            this.comboBox_perforce_workspace.TabIndex = 4;
            // 
            // comboBox_perforce_charset
            // 
            this.comboBox_perforce_charset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_perforce_charset.FormattingEnabled = true;
            this.comboBox_perforce_charset.Location = new System.Drawing.Point(91, 118);
            this.comboBox_perforce_charset.Name = "comboBox_perforce_charset";
            this.comboBox_perforce_charset.Size = new System.Drawing.Size(169, 20);
            this.comboBox_perforce_charset.Sorted = true;
            this.comboBox_perforce_charset.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "パスワード：";
            // 
            // textbox_perforce_password
            // 
            this.textbox_perforce_password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textbox_perforce_password.Location = new System.Drawing.Point(91, 68);
            this.textbox_perforce_password.Name = "textbox_perforce_password";
            this.textbox_perforce_password.PasswordChar = '*';
            this.textbox_perforce_password.Size = new System.Drawing.Size(169, 19);
            this.textbox_perforce_password.TabIndex = 3;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage_standard_Operation);
            this.tabControl.Controls.Add(this.tabPage_set_label);
            this.tabControl.Controls.Add(this.tabPage_diff_label);
            this.tabControl.Controls.Add(this.tabPage_apply_label);
            this.tabControl.Location = new System.Drawing.Point(296, 10);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(203, 204);
            this.tabControl.TabIndex = 10;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.UpdateControlUI);
            // 
            // tabPage_standard_Operation
            // 
            this.tabPage_standard_Operation.Controls.Add(this.textBox_so_changelist);
            this.tabPage_standard_Operation.Controls.Add(this.radioButton_so_menu_restore);
            this.tabPage_standard_Operation.Controls.Add(this.radioButton_so_menu_get_latest);
            this.tabPage_standard_Operation.Controls.Add(this.label9);
            this.tabPage_standard_Operation.Controls.Add(this.radioButton_so_menu_checkout);
            this.tabPage_standard_Operation.Controls.Add(this.radioButton_so_menu_delete);
            this.tabPage_standard_Operation.Location = new System.Drawing.Point(4, 22);
            this.tabPage_standard_Operation.Name = "tabPage_standard_Operation";
            this.tabPage_standard_Operation.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_standard_Operation.Size = new System.Drawing.Size(195, 178);
            this.tabPage_standard_Operation.TabIndex = 0;
            this.tabPage_standard_Operation.Text = "基本操作";
            this.tabPage_standard_Operation.UseVisualStyleBackColor = true;
            // 
            // textBox_so_changelist
            // 
            this.textBox_so_changelist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_so_changelist.Location = new System.Drawing.Point(87, 24);
            this.textBox_so_changelist.Name = "textBox_so_changelist";
            this.textBox_so_changelist.Size = new System.Drawing.Size(96, 19);
            this.textBox_so_changelist.TabIndex = 101;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 12);
            this.label9.TabIndex = 201;
            this.label9.Text = "基準CL：";
            // 
            // tabPage_set_label
            // 
            this.tabPage_set_label.Controls.Add(this.textBox_sl_base_changelist);
            this.tabPage_set_label.Controls.Add(this.label8);
            this.tabPage_set_label.Controls.Add(this.textBox_sl_label_name);
            this.tabPage_set_label.Controls.Add(this.label7);
            this.tabPage_set_label.Location = new System.Drawing.Point(4, 22);
            this.tabPage_set_label.Name = "tabPage_set_label";
            this.tabPage_set_label.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_set_label.Size = new System.Drawing.Size(195, 178);
            this.tabPage_set_label.TabIndex = 1;
            this.tabPage_set_label.Text = "ラベル設定";
            this.tabPage_set_label.UseVisualStyleBackColor = true;
            // 
            // textBox_sl_base_changelist
            // 
            this.textBox_sl_base_changelist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_sl_base_changelist.Location = new System.Drawing.Point(16, 65);
            this.textBox_sl_base_changelist.Name = "textBox_sl_base_changelist";
            this.textBox_sl_base_changelist.Size = new System.Drawing.Size(167, 19);
            this.textBox_sl_base_changelist.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "基準CL：";
            // 
            // textBox_sl_label_name
            // 
            this.textBox_sl_label_name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_sl_label_name.Location = new System.Drawing.Point(16, 24);
            this.textBox_sl_label_name.Name = "textBox_sl_label_name";
            this.textBox_sl_label_name.Size = new System.Drawing.Size(167, 19);
            this.textBox_sl_label_name.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "ラベル名：";
            // 
            // tabPage_diff_label
            // 
            this.tabPage_diff_label.Controls.Add(this.groupBox3);
            this.tabPage_diff_label.Controls.Add(this.groupBox2);
            this.tabPage_diff_label.Location = new System.Drawing.Point(4, 22);
            this.tabPage_diff_label.Name = "tabPage_diff_label";
            this.tabPage_diff_label.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_diff_label.Size = new System.Drawing.Size(195, 178);
            this.tabPage_diff_label.TabIndex = 3;
            this.tabPage_diff_label.Text = "ラベルで比較";
            this.tabPage_diff_label.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.textBox_dl_dest_tree);
            this.groupBox3.Controls.Add(this.textBox_dl_dest_label_name);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Location = new System.Drawing.Point(10, 91);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(179, 78);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "比較先";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(51, 12);
            this.label12.TabIndex = 4;
            this.label12.Text = "ラベル名：";
            // 
            // textBox_dl_dest_tree
            // 
            this.textBox_dl_dest_tree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_dl_dest_tree.Location = new System.Drawing.Point(69, 47);
            this.textBox_dl_dest_tree.Name = "textBox_dl_dest_tree";
            this.textBox_dl_dest_tree.Size = new System.Drawing.Size(97, 19);
            this.textBox_dl_dest_tree.TabIndex = 7;
            // 
            // textBox_dl_dest_label_name
            // 
            this.textBox_dl_dest_label_name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_dl_dest_label_name.Location = new System.Drawing.Point(69, 23);
            this.textBox_dl_dest_label_name.Name = "textBox_dl_dest_label_name";
            this.textBox_dl_dest_label_name.Size = new System.Drawing.Size(97, 19);
            this.textBox_dl_dest_label_name.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(14, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(37, 12);
            this.label13.TabIndex = 6;
            this.label13.Text = "ツリー：";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.textBox_dl_src_tree);
            this.groupBox2.Controls.Add(this.textBox_dl_src_label_name);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(10, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(179, 78);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "比較元";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 12);
            this.label11.TabIndex = 4;
            this.label11.Text = "ラベル名：";
            // 
            // textBox_dl_src_tree
            // 
            this.textBox_dl_src_tree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_dl_src_tree.Location = new System.Drawing.Point(69, 47);
            this.textBox_dl_src_tree.Name = "textBox_dl_src_tree";
            this.textBox_dl_src_tree.Size = new System.Drawing.Size(97, 19);
            this.textBox_dl_src_tree.TabIndex = 7;
            // 
            // textBox_dl_src_label_name
            // 
            this.textBox_dl_src_label_name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_dl_src_label_name.Location = new System.Drawing.Point(69, 23);
            this.textBox_dl_src_label_name.Name = "textBox_dl_src_label_name";
            this.textBox_dl_src_label_name.Size = new System.Drawing.Size(97, 19);
            this.textBox_dl_src_label_name.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 50);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 12);
            this.label10.TabIndex = 6;
            this.label10.Text = "ツリー：";
            // 
            // tabPage_apply_label
            // 
            this.tabPage_apply_label.Controls.Add(this.textBox_ak_label_name);
            this.tabPage_apply_label.Controls.Add(this.label15);
            this.tabPage_apply_label.Controls.Add(this.groupBox4);
            this.tabPage_apply_label.Controls.Add(this.textBox_ak_branch_map);
            this.tabPage_apply_label.Controls.Add(this.label14);
            this.tabPage_apply_label.Location = new System.Drawing.Point(4, 22);
            this.tabPage_apply_label.Name = "tabPage_apply_label";
            this.tabPage_apply_label.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_apply_label.Size = new System.Drawing.Size(195, 178);
            this.tabPage_apply_label.TabIndex = 2;
            this.tabPage_apply_label.Text = "ラベルで反映";
            this.tabPage_apply_label.UseVisualStyleBackColor = true;
            // 
            // textBox_ak_label_name
            // 
            this.textBox_ak_label_name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ak_label_name.Location = new System.Drawing.Point(16, 24);
            this.textBox_ak_label_name.Name = "textBox_ak_label_name";
            this.textBox_ak_label_name.Size = new System.Drawing.Size(167, 19);
            this.textBox_ak_label_name.TabIndex = 2;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 9);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(51, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "ラベル名：";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.radioButton_al_merge);
            this.groupBox4.Controls.Add(this.radioButton_al_copy);
            this.groupBox4.Location = new System.Drawing.Point(16, 93);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(167, 74);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "反映";
            // 
            // radioButton_al_merge
            // 
            this.radioButton_al_merge.AutoSize = true;
            this.radioButton_al_merge.Location = new System.Drawing.Point(34, 45);
            this.radioButton_al_merge.Name = "radioButton_al_merge";
            this.radioButton_al_merge.Size = new System.Drawing.Size(52, 16);
            this.radioButton_al_merge.TabIndex = 1;
            this.radioButton_al_merge.TabStop = true;
            this.radioButton_al_merge.Text = "マージ";
            this.radioButton_al_merge.UseVisualStyleBackColor = true;
            // 
            // radioButton_al_copy
            // 
            this.radioButton_al_copy.AutoSize = true;
            this.radioButton_al_copy.Location = new System.Drawing.Point(34, 23);
            this.radioButton_al_copy.Name = "radioButton_al_copy";
            this.radioButton_al_copy.Size = new System.Drawing.Size(50, 16);
            this.radioButton_al_copy.TabIndex = 0;
            this.radioButton_al_copy.TabStop = true;
            this.radioButton_al_copy.Text = "コピー";
            this.radioButton_al_copy.UseVisualStyleBackColor = true;
            // 
            // textBox_ak_branch_map
            // 
            this.textBox_ak_branch_map.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ak_branch_map.Location = new System.Drawing.Point(16, 65);
            this.textBox_ak_branch_map.Name = "textBox_ak_branch_map";
            this.textBox_ak_branch_map.Size = new System.Drawing.Size(167, 19);
            this.textBox_ak_branch_map.TabIndex = 4;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 50);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(83, 12);
            this.label14.TabIndex = 3;
            this.label14.Text = "ブランチマップ名：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox_perforce_charset);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textbox_perforce_password);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBox_perforce_workspace);
            this.groupBox1.Controls.Add(this.comboBox_perforce_server);
            this.groupBox1.Controls.Add(this.comboBox_perforce_user);
            this.groupBox1.Location = new System.Drawing.Point(13, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 204);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "共通設定";
            // 
            // button_execute
            // 
            this.button_execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_execute.Location = new System.Drawing.Point(424, 349);
            this.button_execute.Name = "button_execute";
            this.button_execute.Size = new System.Drawing.Size(75, 23);
            this.button_execute.TabIndex = 40;
            this.button_execute.Text = "実行";
            this.button_execute.UseVisualStyleBackColor = true;
            this.button_execute.Click += new System.EventHandler(this.button_execute_Click);
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(12, 335);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(471, 42);
            this.label16.TabIndex = 52;
            this.label16.DoubleClick += new System.EventHandler(this.label16_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 406);
            this.Controls.Add(this.button_execute);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.comboBox_profile);
            this.Controls.Add(this.button_profile_save);
            this.Controls.Add(this.textBox_tree_list);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label16);
            this.MinimumSize = new System.Drawing.Size(450, 370);
            this.Name = "Form1";
            this.Text = "PerforceWrapper";
            this.tabControl.ResumeLayout(false);
            this.tabPage_standard_Operation.ResumeLayout(false);
            this.tabPage_standard_Operation.PerformLayout();
            this.tabPage_set_label.ResumeLayout(false);
            this.tabPage_set_label.PerformLayout();
            this.tabPage_diff_label.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage_apply_label.ResumeLayout(false);
            this.tabPage_apply_label.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_profile_save;
        private System.Windows.Forms.ComboBox comboBox_profile;
        public System.Windows.Forms.RadioButton radioButton_so_menu_checkout;
        public System.Windows.Forms.TextBox textBox_tree_list;
        public System.Windows.Forms.ComboBox comboBox_perforce_server;
        public System.Windows.Forms.ComboBox comboBox_perforce_user;
        public System.Windows.Forms.ComboBox comboBox_perforce_workspace;
        public System.Windows.Forms.ComboBox comboBox_perforce_charset;
        private System.Windows.Forms.Label label6;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        public System.Windows.Forms.TextBox textbox_perforce_password;
        public System.Windows.Forms.RadioButton radioButton_so_menu_delete;
        public System.Windows.Forms.RadioButton radioButton_so_menu_get_latest;
        public System.Windows.Forms.RadioButton radioButton_so_menu_restore;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage_standard_Operation;
        private System.Windows.Forms.TabPage tabPage_set_label;
        private System.Windows.Forms.TabPage tabPage_apply_label;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabPage tabPage_diff_label;
        public System.Windows.Forms.TextBox textBox_sl_base_changelist;
        public System.Windows.Forms.TextBox textBox_sl_label_name;
        public System.Windows.Forms.TextBox textBox_so_changelist;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_execute;
        public System.Windows.Forms.TextBox textBox_dl_src_tree;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox textBox_dl_src_label_name;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.TextBox textBox_dl_dest_tree;
        public System.Windows.Forms.TextBox textBox_dl_dest_label_name;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.TextBox textBox_ak_branch_map;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.RadioButton radioButton_al_merge;
        public System.Windows.Forms.RadioButton radioButton_al_copy;
        public System.Windows.Forms.TextBox textBox_ak_label_name;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
    }
}

