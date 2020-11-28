namespace FFEdit
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
            this.butotn_Listup = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Target_Extension = new System.Windows.Forms.TextBox();
            this.checkBox_Target_SubDirectory = new System.Windows.Forms.CheckBox();
            this.checkBox_Target_SelectFile = new System.Windows.Forms.CheckBox();
            this.radioButton_Target_Dir = new System.Windows.Forms.RadioButton();
            this.radioButton_Target_File = new System.Windows.Forms.RadioButton();
            this.listBox = new System.Windows.Forms.ListBox();
            this.button_Execute = new System.Windows.Forms.Button();
            this.button_Restore = new System.Windows.Forms.Button();
            this.radioButton_ChangeNumber = new System.Windows.Forms.RadioButton();
            this.radioButton_ChangeDelNum = new System.Windows.Forms.RadioButton();
            this.label_ChangeNumber_FirstVal = new System.Windows.Forms.Label();
            this.textBox_ChangeNumber_FirstVal = new System.Windows.Forms.TextBox();
            this.comboBox_ChangeNumber_Digit = new System.Windows.Forms.ComboBox();
            this.checkBox_ChangeNumber_OrgName = new System.Windows.Forms.CheckBox();
            this.label_String1 = new System.Windows.Forms.Label();
            this.label_String2 = new System.Windows.Forms.Label();
            this.radioButton_ChangeAdd = new System.Windows.Forms.RadioButton();
            this.radioButton_ChangeDelete = new System.Windows.Forms.RadioButton();
            this.radioButton_ChangeReplace = new System.Windows.Forms.RadioButton();
            this.radioButton_ChangeExt = new System.Windows.Forms.RadioButton();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage_ChangeName = new System.Windows.Forms.TabPage();
            this.radioButton_ChangeAddDirName = new System.Windows.Forms.RadioButton();
            this.comboBox_String2 = new System.Windows.Forms.ComboBox();
            this.comboBox_String1 = new System.Windows.Forms.ComboBox();
            this.tabPage_TimeStump = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_TimeSpan = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_CreationTime = new System.Windows.Forms.CheckBox();
            this.checkBox_LastWriteTime = new System.Windows.Forms.CheckBox();
            this.checkBox_LastAccessTime = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePicker_Time = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_Days = new System.Windows.Forms.DateTimePicker();
            this.tabPage_Function = new System.Windows.Forms.TabPage();
            this.radioButton_Copy_Target = new System.Windows.Forms.RadioButton();
            this.textBox_Function_Any_Directory = new System.Windows.Forms.TextBox();
            this.radioButton_Move_Target = new System.Windows.Forms.RadioButton();
            this.radioButton_Delete_BlankDir = new System.Windows.Forms.RadioButton();
            this.textBox_StatusBar = new System.Windows.Forms.TextBox();
            this.button_SaveSetting = new System.Windows.Forms.Button();
            this.comboBox_TargetDir = new System.Windows.Forms.ComboBox();
            this.checkBox_Operatoin_AnyDir = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage_ChangeName.SuspendLayout();
            this.tabPage_TimeStump.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage_Function.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "対象フォルダ";
            // 
            // butotn_Listup
            // 
            this.butotn_Listup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butotn_Listup.Location = new System.Drawing.Point(403, 4);
            this.butotn_Listup.Name = "butotn_Listup";
            this.butotn_Listup.Size = new System.Drawing.Size(75, 23);
            this.butotn_Listup.TabIndex = 2;
            this.butotn_Listup.Text = "リストアップ";
            this.butotn_Listup.UseVisualStyleBackColor = true;
            this.butotn_Listup.Click += new System.EventHandler(this.button_Listup_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_Target_Extension);
            this.groupBox1.Controls.Add(this.checkBox_Target_SubDirectory);
            this.groupBox1.Controls.Add(this.checkBox_Target_SelectFile);
            this.groupBox1.Controls.Add(this.radioButton_Target_Dir);
            this.groupBox1.Controls.Add(this.radioButton_Target_File);
            this.groupBox1.Location = new System.Drawing.Point(15, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 128);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "対象";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "フィルター";
            // 
            // textBox_Target_Extension
            // 
            this.textBox_Target_Extension.Location = new System.Drawing.Point(92, 54);
            this.textBox_Target_Extension.Name = "textBox_Target_Extension";
            this.textBox_Target_Extension.Size = new System.Drawing.Size(141, 19);
            this.textBox_Target_Extension.TabIndex = 3;
            this.textBox_Target_Extension.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_Target_Extension_KeyUp);
            // 
            // checkBox_Target_SubDirectory
            // 
            this.checkBox_Target_SubDirectory.AutoSize = true;
            this.checkBox_Target_SubDirectory.Location = new System.Drawing.Point(13, 83);
            this.checkBox_Target_SubDirectory.Name = "checkBox_Target_SubDirectory";
            this.checkBox_Target_SubDirectory.Size = new System.Drawing.Size(111, 16);
            this.checkBox_Target_SubDirectory.TabIndex = 4;
            this.checkBox_Target_SubDirectory.Text = "サブフォルダも表示";
            this.checkBox_Target_SubDirectory.UseVisualStyleBackColor = true;
            this.checkBox_Target_SubDirectory.CheckedChanged += new System.EventHandler(this.UpdateListBox);
            // 
            // checkBox_Target_SelectFile
            // 
            this.checkBox_Target_SelectFile.AutoSize = true;
            this.checkBox_Target_SelectFile.Location = new System.Drawing.Point(13, 103);
            this.checkBox_Target_SelectFile.Name = "checkBox_Target_SelectFile";
            this.checkBox_Target_SelectFile.Size = new System.Drawing.Size(93, 16);
            this.checkBox_Target_SelectFile.TabIndex = 5;
            this.checkBox_Target_SelectFile.Text = "選択項目のみ";
            this.checkBox_Target_SelectFile.UseVisualStyleBackColor = true;
            // 
            // radioButton_Target_Dir
            // 
            this.radioButton_Target_Dir.AutoSize = true;
            this.radioButton_Target_Dir.Location = new System.Drawing.Point(13, 18);
            this.radioButton_Target_Dir.Name = "radioButton_Target_Dir";
            this.radioButton_Target_Dir.Size = new System.Drawing.Size(58, 16);
            this.radioButton_Target_Dir.TabIndex = 0;
            this.radioButton_Target_Dir.Text = "フォルダ";
            this.radioButton_Target_Dir.UseVisualStyleBackColor = true;
            this.radioButton_Target_Dir.CheckedChanged += new System.EventHandler(this.UpdateListBox);
            // 
            // radioButton_Target_File
            // 
            this.radioButton_Target_File.AutoSize = true;
            this.radioButton_Target_File.Checked = true;
            this.radioButton_Target_File.Location = new System.Drawing.Point(13, 38);
            this.radioButton_Target_File.Name = "radioButton_Target_File";
            this.radioButton_Target_File.Size = new System.Drawing.Size(57, 16);
            this.radioButton_Target_File.TabIndex = 1;
            this.radioButton_Target_File.TabStop = true;
            this.radioButton_Target_File.Text = "ファイル";
            this.radioButton_Target_File.UseVisualStyleBackColor = true;
            this.radioButton_Target_File.CheckedChanged += new System.EventHandler(this.UpdateListBox);
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.HorizontalScrollbar = true;
            this.listBox.ItemHeight = 12;
            this.listBox.Location = new System.Drawing.Point(256, 34);
            this.listBox.Name = "listBox";
            this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox.Size = new System.Drawing.Size(222, 424);
            this.listBox.Sorted = true;
            this.listBox.TabIndex = 7;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);
            // 
            // button_Execute
            // 
            this.button_Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Execute.Location = new System.Drawing.Point(413, 468);
            this.button_Execute.Name = "button_Execute";
            this.button_Execute.Size = new System.Drawing.Size(65, 23);
            this.button_Execute.TabIndex = 10;
            this.button_Execute.Text = "実行";
            this.button_Execute.UseVisualStyleBackColor = true;
            this.button_Execute.Click += new System.EventHandler(this.button_Execute_Click);
            // 
            // button_Restore
            // 
            this.button_Restore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Restore.Location = new System.Drawing.Point(344, 468);
            this.button_Restore.Name = "button_Restore";
            this.button_Restore.Size = new System.Drawing.Size(65, 23);
            this.button_Restore.TabIndex = 9;
            this.button_Restore.Text = "復元";
            this.button_Restore.UseVisualStyleBackColor = true;
            this.button_Restore.Click += new System.EventHandler(this.button_Restore_Click);
            // 
            // radioButton_ChangeNumber
            // 
            this.radioButton_ChangeNumber.AutoSize = true;
            this.radioButton_ChangeNumber.Checked = true;
            this.radioButton_ChangeNumber.Location = new System.Drawing.Point(9, 11);
            this.radioButton_ChangeNumber.Name = "radioButton_ChangeNumber";
            this.radioButton_ChangeNumber.Size = new System.Drawing.Size(68, 16);
            this.radioButton_ChangeNumber.TabIndex = 0;
            this.radioButton_ChangeNumber.TabStop = true;
            this.radioButton_ChangeNumber.Text = "通し番号";
            this.radioButton_ChangeNumber.UseVisualStyleBackColor = true;
            this.radioButton_ChangeNumber.CheckedChanged += new System.EventHandler(this.UpdateNameChangeControl);
            // 
            // radioButton_ChangeDelNum
            // 
            this.radioButton_ChangeDelNum.AutoSize = true;
            this.radioButton_ChangeDelNum.Location = new System.Drawing.Point(9, 100);
            this.radioButton_ChangeDelNum.Name = "radioButton_ChangeDelNum";
            this.radioButton_ChangeDelNum.Size = new System.Drawing.Size(126, 16);
            this.radioButton_ChangeDelNum.TabIndex = 6;
            this.radioButton_ChangeDelNum.Text = "指定文字数だけ削除";
            this.radioButton_ChangeDelNum.UseVisualStyleBackColor = true;
            this.radioButton_ChangeDelNum.CheckedChanged += new System.EventHandler(this.UpdateNameChangeControl);
            // 
            // label_ChangeNumber_FirstVal
            // 
            this.label_ChangeNumber_FirstVal.AutoSize = true;
            this.label_ChangeNumber_FirstVal.Location = new System.Drawing.Point(26, 34);
            this.label_ChangeNumber_FirstVal.Name = "label_ChangeNumber_FirstVal";
            this.label_ChangeNumber_FirstVal.Size = new System.Drawing.Size(51, 12);
            this.label_ChangeNumber_FirstVal.TabIndex = 1;
            this.label_ChangeNumber_FirstVal.Text = "最初の値";
            // 
            // textBox_ChangeNumber_FirstVal
            // 
            this.textBox_ChangeNumber_FirstVal.Location = new System.Drawing.Point(78, 30);
            this.textBox_ChangeNumber_FirstVal.Name = "textBox_ChangeNumber_FirstVal";
            this.textBox_ChangeNumber_FirstVal.Size = new System.Drawing.Size(68, 19);
            this.textBox_ChangeNumber_FirstVal.TabIndex = 2;
            this.textBox_ChangeNumber_FirstVal.Text = "1";
            this.textBox_ChangeNumber_FirstVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // comboBox_ChangeNumber_Digit
            // 
            this.comboBox_ChangeNumber_Digit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ChangeNumber_Digit.FormattingEnabled = true;
            this.comboBox_ChangeNumber_Digit.Location = new System.Drawing.Point(152, 30);
            this.comboBox_ChangeNumber_Digit.Name = "comboBox_ChangeNumber_Digit";
            this.comboBox_ChangeNumber_Digit.Size = new System.Drawing.Size(74, 20);
            this.comboBox_ChangeNumber_Digit.TabIndex = 3;
            // 
            // checkBox_ChangeNumber_OrgName
            // 
            this.checkBox_ChangeNumber_OrgName.AutoSize = true;
            this.checkBox_ChangeNumber_OrgName.Location = new System.Drawing.Point(28, 54);
            this.checkBox_ChangeNumber_OrgName.Name = "checkBox_ChangeNumber_OrgName";
            this.checkBox_ChangeNumber_OrgName.Size = new System.Drawing.Size(128, 16);
            this.checkBox_ChangeNumber_OrgName.TabIndex = 4;
            this.checkBox_ChangeNumber_OrgName.Text = "もとのファイル名を残す";
            this.checkBox_ChangeNumber_OrgName.UseVisualStyleBackColor = true;
            // 
            // label_String1
            // 
            this.label_String1.Location = new System.Drawing.Point(15, 208);
            this.label_String1.Name = "label_String1";
            this.label_String1.Size = new System.Drawing.Size(54, 16);
            this.label_String1.TabIndex = 11;
            this.label_String1.Text = "先頭から";
            this.label_String1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label_String2
            // 
            this.label_String2.Location = new System.Drawing.Point(15, 231);
            this.label_String2.Name = "label_String2";
            this.label_String2.Size = new System.Drawing.Size(54, 16);
            this.label_String2.TabIndex = 13;
            this.label_String2.Text = "後方から";
            this.label_String2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // radioButton_ChangeAdd
            // 
            this.radioButton_ChangeAdd.AutoSize = true;
            this.radioButton_ChangeAdd.Location = new System.Drawing.Point(9, 160);
            this.radioButton_ChangeAdd.Name = "radioButton_ChangeAdd";
            this.radioButton_ChangeAdd.Size = new System.Drawing.Size(81, 16);
            this.radioButton_ChangeAdd.TabIndex = 9;
            this.radioButton_ChangeAdd.Text = "文字の追加";
            this.radioButton_ChangeAdd.UseVisualStyleBackColor = true;
            this.radioButton_ChangeAdd.CheckedChanged += new System.EventHandler(this.UpdateNameChangeControl);
            // 
            // radioButton_ChangeDelete
            // 
            this.radioButton_ChangeDelete.AutoSize = true;
            this.radioButton_ChangeDelete.Location = new System.Drawing.Point(9, 140);
            this.radioButton_ChangeDelete.Name = "radioButton_ChangeDelete";
            this.radioButton_ChangeDelete.Size = new System.Drawing.Size(105, 16);
            this.radioButton_ChangeDelete.TabIndex = 8;
            this.radioButton_ChangeDelete.Text = "指定文字の削除";
            this.radioButton_ChangeDelete.UseVisualStyleBackColor = true;
            this.radioButton_ChangeDelete.CheckedChanged += new System.EventHandler(this.UpdateNameChangeControl);
            // 
            // radioButton_ChangeReplace
            // 
            this.radioButton_ChangeReplace.AutoSize = true;
            this.radioButton_ChangeReplace.Location = new System.Drawing.Point(9, 180);
            this.radioButton_ChangeReplace.Name = "radioButton_ChangeReplace";
            this.radioButton_ChangeReplace.Size = new System.Drawing.Size(81, 16);
            this.radioButton_ChangeReplace.TabIndex = 10;
            this.radioButton_ChangeReplace.Text = "文字の置換";
            this.radioButton_ChangeReplace.UseVisualStyleBackColor = true;
            this.radioButton_ChangeReplace.CheckedChanged += new System.EventHandler(this.UpdateNameChangeControl);
            // 
            // radioButton_ChangeExt
            // 
            this.radioButton_ChangeExt.AutoSize = true;
            this.radioButton_ChangeExt.Location = new System.Drawing.Point(9, 120);
            this.radioButton_ChangeExt.Name = "radioButton_ChangeExt";
            this.radioButton_ChangeExt.Size = new System.Drawing.Size(83, 16);
            this.radioButton_ChangeExt.TabIndex = 7;
            this.radioButton_ChangeExt.Text = "拡張子変更";
            this.radioButton_ChangeExt.UseVisualStyleBackColor = true;
            this.radioButton_ChangeExt.CheckedChanged += new System.EventHandler(this.UpdateNameChangeControl);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl.Controls.Add(this.tabPage_ChangeName);
            this.tabControl.Controls.Add(this.tabPage_TimeStump);
            this.tabControl.Controls.Add(this.tabPage_Function);
            this.tabControl.Location = new System.Drawing.Point(14, 171);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(240, 288);
            this.tabControl.TabIndex = 5;
            // 
            // tabPage_ChangeName
            // 
            this.tabPage_ChangeName.Controls.Add(this.radioButton_ChangeAddDirName);
            this.tabPage_ChangeName.Controls.Add(this.comboBox_String2);
            this.tabPage_ChangeName.Controls.Add(this.comboBox_String1);
            this.tabPage_ChangeName.Controls.Add(this.radioButton_ChangeExt);
            this.tabPage_ChangeName.Controls.Add(this.radioButton_ChangeNumber);
            this.tabPage_ChangeName.Controls.Add(this.radioButton_ChangeReplace);
            this.tabPage_ChangeName.Controls.Add(this.radioButton_ChangeDelNum);
            this.tabPage_ChangeName.Controls.Add(this.radioButton_ChangeDelete);
            this.tabPage_ChangeName.Controls.Add(this.label_ChangeNumber_FirstVal);
            this.tabPage_ChangeName.Controls.Add(this.radioButton_ChangeAdd);
            this.tabPage_ChangeName.Controls.Add(this.textBox_ChangeNumber_FirstVal);
            this.tabPage_ChangeName.Controls.Add(this.comboBox_ChangeNumber_Digit);
            this.tabPage_ChangeName.Controls.Add(this.label_String2);
            this.tabPage_ChangeName.Controls.Add(this.checkBox_ChangeNumber_OrgName);
            this.tabPage_ChangeName.Controls.Add(this.label_String1);
            this.tabPage_ChangeName.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ChangeName.Name = "tabPage_ChangeName";
            this.tabPage_ChangeName.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ChangeName.Size = new System.Drawing.Size(232, 262);
            this.tabPage_ChangeName.TabIndex = 0;
            this.tabPage_ChangeName.Text = "名称変換";
            this.tabPage_ChangeName.UseVisualStyleBackColor = true;
            // 
            // radioButton_ChangeAddDirName
            // 
            this.radioButton_ChangeAddDirName.AutoSize = true;
            this.radioButton_ChangeAddDirName.Location = new System.Drawing.Point(9, 80);
            this.radioButton_ChangeAddDirName.Name = "radioButton_ChangeAddDirName";
            this.radioButton_ChangeAddDirName.Size = new System.Drawing.Size(158, 16);
            this.radioButton_ChangeAddDirName.TabIndex = 5;
            this.radioButton_ChangeAddDirName.Text = "フォルダ名をファイル名に追加";
            this.radioButton_ChangeAddDirName.UseVisualStyleBackColor = true;
            this.radioButton_ChangeAddDirName.CheckedChanged += new System.EventHandler(this.UpdateNameChangeControl);
            // 
            // comboBox_String2
            // 
            this.comboBox_String2.Enabled = false;
            this.comboBox_String2.FormattingEnabled = true;
            this.comboBox_String2.Location = new System.Drawing.Point(70, 228);
            this.comboBox_String2.Name = "comboBox_String2";
            this.comboBox_String2.Size = new System.Drawing.Size(156, 20);
            this.comboBox_String2.TabIndex = 14;
            // 
            // comboBox_String1
            // 
            this.comboBox_String1.Enabled = false;
            this.comboBox_String1.FormattingEnabled = true;
            this.comboBox_String1.Location = new System.Drawing.Point(70, 205);
            this.comboBox_String1.Name = "comboBox_String1";
            this.comboBox_String1.Size = new System.Drawing.Size(156, 20);
            this.comboBox_String1.TabIndex = 12;
            // 
            // tabPage_TimeStump
            // 
            this.tabPage_TimeStump.Controls.Add(this.label3);
            this.tabPage_TimeStump.Controls.Add(this.comboBox_TimeSpan);
            this.tabPage_TimeStump.Controls.Add(this.groupBox2);
            this.tabPage_TimeStump.Controls.Add(this.label2);
            this.tabPage_TimeStump.Controls.Add(this.dateTimePicker_Time);
            this.tabPage_TimeStump.Controls.Add(this.dateTimePicker_Days);
            this.tabPage_TimeStump.Location = new System.Drawing.Point(4, 22);
            this.tabPage_TimeStump.Name = "tabPage_TimeStump";
            this.tabPage_TimeStump.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_TimeStump.Size = new System.Drawing.Size(232, 262);
            this.tabPage_TimeStump.TabIndex = 1;
            this.tabPage_TimeStump.Text = "タイムスタンプ";
            this.tabPage_TimeStump.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "加算間隔";
            // 
            // comboBox_TimeSpan
            // 
            this.comboBox_TimeSpan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TimeSpan.FormattingEnabled = true;
            this.comboBox_TimeSpan.Location = new System.Drawing.Point(12, 104);
            this.comboBox_TimeSpan.Name = "comboBox_TimeSpan";
            this.comboBox_TimeSpan.Size = new System.Drawing.Size(214, 20);
            this.comboBox_TimeSpan.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_CreationTime);
            this.groupBox2.Controls.Add(this.checkBox_LastWriteTime);
            this.groupBox2.Controls.Add(this.checkBox_LastAccessTime);
            this.groupBox2.Location = new System.Drawing.Point(11, 145);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(214, 86);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "更新対象";
            // 
            // checkBox_CreationTime
            // 
            this.checkBox_CreationTime.AutoSize = true;
            this.checkBox_CreationTime.Checked = true;
            this.checkBox_CreationTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_CreationTime.Location = new System.Drawing.Point(23, 18);
            this.checkBox_CreationTime.Name = "checkBox_CreationTime";
            this.checkBox_CreationTime.Size = new System.Drawing.Size(72, 16);
            this.checkBox_CreationTime.TabIndex = 0;
            this.checkBox_CreationTime.Text = "作成日時";
            this.checkBox_CreationTime.UseVisualStyleBackColor = true;
            // 
            // checkBox_LastWriteTime
            // 
            this.checkBox_LastWriteTime.AutoSize = true;
            this.checkBox_LastWriteTime.Checked = true;
            this.checkBox_LastWriteTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_LastWriteTime.Location = new System.Drawing.Point(23, 40);
            this.checkBox_LastWriteTime.Name = "checkBox_LastWriteTime";
            this.checkBox_LastWriteTime.Size = new System.Drawing.Size(72, 16);
            this.checkBox_LastWriteTime.TabIndex = 1;
            this.checkBox_LastWriteTime.Text = "更新日時";
            this.checkBox_LastWriteTime.UseVisualStyleBackColor = true;
            // 
            // checkBox_LastAccessTime
            // 
            this.checkBox_LastAccessTime.AutoSize = true;
            this.checkBox_LastAccessTime.Checked = true;
            this.checkBox_LastAccessTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_LastAccessTime.Location = new System.Drawing.Point(23, 62);
            this.checkBox_LastAccessTime.Name = "checkBox_LastAccessTime";
            this.checkBox_LastAccessTime.Size = new System.Drawing.Size(84, 16);
            this.checkBox_LastAccessTime.TabIndex = 2;
            this.checkBox_LastAccessTime.Text = "アクセス日時";
            this.checkBox_LastAccessTime.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "基準日";
            // 
            // dateTimePicker_Time
            // 
            this.dateTimePicker_Time.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker_Time.Location = new System.Drawing.Point(12, 52);
            this.dateTimePicker_Time.MaxDate = new System.DateTime(3000, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker_Time.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_Time.Name = "dateTimePicker_Time";
            this.dateTimePicker_Time.ShowUpDown = true;
            this.dateTimePicker_Time.Size = new System.Drawing.Size(214, 19);
            this.dateTimePicker_Time.TabIndex = 2;
            // 
            // dateTimePicker_Days
            // 
            this.dateTimePicker_Days.Location = new System.Drawing.Point(12, 27);
            this.dateTimePicker_Days.MaxDate = new System.DateTime(3000, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker_Days.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_Days.Name = "dateTimePicker_Days";
            this.dateTimePicker_Days.Size = new System.Drawing.Size(214, 19);
            this.dateTimePicker_Days.TabIndex = 1;
            // 
            // tabPage_Function
            // 
            this.tabPage_Function.Controls.Add(this.checkBox_Operatoin_AnyDir);
            this.tabPage_Function.Controls.Add(this.radioButton_Copy_Target);
            this.tabPage_Function.Controls.Add(this.textBox_Function_Any_Directory);
            this.tabPage_Function.Controls.Add(this.radioButton_Move_Target);
            this.tabPage_Function.Controls.Add(this.radioButton_Delete_BlankDir);
            this.tabPage_Function.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Function.Name = "tabPage_Function";
            this.tabPage_Function.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Function.Size = new System.Drawing.Size(232, 262);
            this.tabPage_Function.TabIndex = 2;
            this.tabPage_Function.Text = "機能";
            this.tabPage_Function.UseVisualStyleBackColor = true;
            // 
            // radioButton_Copy_Target
            // 
            this.radioButton_Copy_Target.AutoSize = true;
            this.radioButton_Copy_Target.Location = new System.Drawing.Point(9, 53);
            this.radioButton_Copy_Target.Name = "radioButton_Copy_Target";
            this.radioButton_Copy_Target.Size = new System.Drawing.Size(50, 16);
            this.radioButton_Copy_Target.TabIndex = 2;
            this.radioButton_Copy_Target.Text = "コピー";
            this.radioButton_Copy_Target.UseVisualStyleBackColor = true;
            this.radioButton_Copy_Target.CheckedChanged += new System.EventHandler(this.UpdateFunctionControl);
            // 
            // textBox_Function_Any_Directory
            // 
            this.textBox_Function_Any_Directory.Enabled = false;
            this.textBox_Function_Any_Directory.Location = new System.Drawing.Point(40, 95);
            this.textBox_Function_Any_Directory.Name = "textBox_Function_Any_Directory";
            this.textBox_Function_Any_Directory.Size = new System.Drawing.Size(186, 19);
            this.textBox_Function_Any_Directory.TabIndex = 4;
            this.textBox_Function_Any_Directory.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox_Function_Target_Directory_MouseClick);
            this.textBox_Function_Any_Directory.Enter += new System.EventHandler(this.textBox_Function_Target_Directory_Enter);
            // 
            // radioButton_Move_Target
            // 
            this.radioButton_Move_Target.AutoSize = true;
            this.radioButton_Move_Target.Location = new System.Drawing.Point(9, 31);
            this.radioButton_Move_Target.Name = "radioButton_Move_Target";
            this.radioButton_Move_Target.Size = new System.Drawing.Size(47, 16);
            this.radioButton_Move_Target.TabIndex = 1;
            this.radioButton_Move_Target.Text = "移動";
            this.radioButton_Move_Target.UseVisualStyleBackColor = true;
            this.radioButton_Move_Target.CheckedChanged += new System.EventHandler(this.UpdateFunctionControl);
            // 
            // radioButton_Delete_BlankDir
            // 
            this.radioButton_Delete_BlankDir.AutoSize = true;
            this.radioButton_Delete_BlankDir.Checked = true;
            this.radioButton_Delete_BlankDir.Location = new System.Drawing.Point(9, 11);
            this.radioButton_Delete_BlankDir.Name = "radioButton_Delete_BlankDir";
            this.radioButton_Delete_BlankDir.Size = new System.Drawing.Size(113, 16);
            this.radioButton_Delete_BlankDir.TabIndex = 0;
            this.radioButton_Delete_BlankDir.TabStop = true;
            this.radioButton_Delete_BlankDir.Text = "空のフォルダを削除";
            this.radioButton_Delete_BlankDir.UseVisualStyleBackColor = true;
            this.radioButton_Delete_BlankDir.CheckedChanged += new System.EventHandler(this.UpdateFunctionControl);
            // 
            // textBox_StatusBar
            // 
            this.textBox_StatusBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_StatusBar.Location = new System.Drawing.Point(14, 470);
            this.textBox_StatusBar.Name = "textBox_StatusBar";
            this.textBox_StatusBar.ReadOnly = true;
            this.textBox_StatusBar.Size = new System.Drawing.Size(236, 19);
            this.textBox_StatusBar.TabIndex = 6;
            this.textBox_StatusBar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button_SaveSetting
            // 
            this.button_SaveSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveSetting.Location = new System.Drawing.Point(255, 468);
            this.button_SaveSetting.Name = "button_SaveSetting";
            this.button_SaveSetting.Size = new System.Drawing.Size(84, 23);
            this.button_SaveSetting.TabIndex = 8;
            this.button_SaveSetting.Text = "設定保存";
            this.button_SaveSetting.UseVisualStyleBackColor = true;
            this.button_SaveSetting.Click += new System.EventHandler(this.button_SaveSetting_Click);
            // 
            // comboBox_TargetDir
            // 
            this.comboBox_TargetDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_TargetDir.FormattingEnabled = true;
            this.comboBox_TargetDir.Location = new System.Drawing.Point(82, 5);
            this.comboBox_TargetDir.Name = "comboBox_TargetDir";
            this.comboBox_TargetDir.Size = new System.Drawing.Size(313, 20);
            this.comboBox_TargetDir.TabIndex = 1;
            this.comboBox_TargetDir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox_TargetDir_KeyDown);
            // 
            // checkBox_Operatoin_AnyDir
            // 
            this.checkBox_Operatoin_AnyDir.AutoSize = true;
            this.checkBox_Operatoin_AnyDir.Enabled = false;
            this.checkBox_Operatoin_AnyDir.Location = new System.Drawing.Point(25, 75);
            this.checkBox_Operatoin_AnyDir.Name = "checkBox_Operatoin_AnyDir";
            this.checkBox_Operatoin_AnyDir.Size = new System.Drawing.Size(111, 16);
            this.checkBox_Operatoin_AnyDir.TabIndex = 3;
            this.checkBox_Operatoin_AnyDir.Text = "フォルダを指定する";
            this.checkBox_Operatoin_AnyDir.UseVisualStyleBackColor = true;
            this.checkBox_Operatoin_AnyDir.CheckedChanged += new System.EventHandler(this.UpdateFunctionControl);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 497);
            this.Controls.Add(this.comboBox_TargetDir);
            this.Controls.Add(this.button_SaveSetting);
            this.Controls.Add(this.textBox_StatusBar);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.button_Restore);
            this.Controls.Add(this.button_Execute);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.butotn_Listup);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(506, 535);
            this.Name = "Form1";
            this.Text = "FFEdit";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage_ChangeName.ResumeLayout(false);
            this.tabPage_ChangeName.PerformLayout();
            this.tabPage_TimeStump.ResumeLayout(false);
            this.tabPage_TimeStump.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage_Function.ResumeLayout(false);
            this.tabPage_Function.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button butotn_Listup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_Target_Dir;
        private System.Windows.Forms.RadioButton radioButton_Target_File;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button button_Execute;
        private System.Windows.Forms.Button button_Restore;
        private System.Windows.Forms.RadioButton radioButton_ChangeNumber;
        private System.Windows.Forms.RadioButton radioButton_ChangeDelNum;
        private System.Windows.Forms.Label label_ChangeNumber_FirstVal;
        private System.Windows.Forms.TextBox textBox_ChangeNumber_FirstVal;
        private System.Windows.Forms.ComboBox comboBox_ChangeNumber_Digit;
        private System.Windows.Forms.CheckBox checkBox_ChangeNumber_OrgName;
        private System.Windows.Forms.Label label_String1;
        private System.Windows.Forms.Label label_String2;
        private System.Windows.Forms.RadioButton radioButton_ChangeAdd;
        private System.Windows.Forms.RadioButton radioButton_ChangeDelete;
        private System.Windows.Forms.RadioButton radioButton_ChangeReplace;
        private System.Windows.Forms.RadioButton radioButton_ChangeExt;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage_ChangeName;
        private System.Windows.Forms.TabPage tabPage_TimeStump;
        private System.Windows.Forms.DateTimePicker dateTimePicker_Days;
        private System.Windows.Forms.DateTimePicker dateTimePicker_Time;
        public System.Windows.Forms.CheckBox checkBox_LastAccessTime;
        public System.Windows.Forms.CheckBox checkBox_LastWriteTime;
        public System.Windows.Forms.CheckBox checkBox_CreationTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_TimeSpan;
        private System.Windows.Forms.TextBox textBox_StatusBar;
        private System.Windows.Forms.CheckBox checkBox_Target_SelectFile;
        private System.Windows.Forms.CheckBox checkBox_Target_SubDirectory;
        private System.Windows.Forms.TabPage tabPage_Function;
        private System.Windows.Forms.RadioButton radioButton_Delete_BlankDir;
        private System.Windows.Forms.RadioButton radioButton_Move_Target;
        private System.Windows.Forms.TextBox textBox_Function_Any_Directory;
        private System.Windows.Forms.Button button_SaveSetting;
        public System.Windows.Forms.ComboBox comboBox_TargetDir;
        public System.Windows.Forms.ComboBox comboBox_String2;
        public System.Windows.Forms.ComboBox comboBox_String1;
        private System.Windows.Forms.RadioButton radioButton_ChangeAddDirName;
        private System.Windows.Forms.RadioButton radioButton_Copy_Target;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox textBox_Target_Extension;
        private System.Windows.Forms.CheckBox checkBox_Operatoin_AnyDir;
    }
}

