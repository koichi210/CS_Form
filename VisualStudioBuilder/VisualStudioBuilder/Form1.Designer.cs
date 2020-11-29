﻿namespace VisualStudioBuilder
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_VisualStudioExePath = new System.Windows.Forms.TextBox();
            this.textBox_BuildOption = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_Build = new System.Windows.Forms.Button();
            this.button_AddRaw = new System.Windows.Forms.Button();
            this.BuildWorker = new System.ComponentModel.BackgroundWorker();
            this.button_RemoveRaw = new System.Windows.Forms.Button();
            this.button_SaveSetting = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox_DeleteDirectoryName = new System.Windows.Forms.TextBox();
            this.checkBox_DeleteDirectory = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_DetectBuildError = new System.Windows.Forms.CheckBox();
            this.textBox_DetectBuildErrorWord = new System.Windows.Forms.TextBox();
            this.label_ExcludeWord = new System.Windows.Forms.Label();
            this.label_DetectBuildErrorWord = new System.Windows.Forms.Label();
            this.textBox_ExcludeWord = new System.Windows.Forms.TextBox();
            this.checkBox_IsExclude = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_LogDirectory = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button_AddAllSolution = new System.Windows.Forms.Button();
            this.comboBox_Profile = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 27);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 21;
            this.dataGridView.Size = new System.Drawing.Size(432, 272);
            this.dataGridView.TabIndex = 7;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
            this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "VisualStudioパス：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ビルドオプション：";
            // 
            // textBox_VisualStudioExePath
            // 
            this.textBox_VisualStudioExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_VisualStudioExePath.Location = new System.Drawing.Point(109, 16);
            this.textBox_VisualStudioExePath.Name = "textBox_VisualStudioExePath";
            this.textBox_VisualStudioExePath.Size = new System.Drawing.Size(333, 19);
            this.textBox_VisualStudioExePath.TabIndex = 1;
            // 
            // textBox_BuildOption
            // 
            this.textBox_BuildOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_BuildOption.Location = new System.Drawing.Point(109, 41);
            this.textBox_BuildOption.Name = "textBox_BuildOption";
            this.textBox_BuildOption.Size = new System.Drawing.Size(333, 19);
            this.textBox_BuildOption.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "プロジェクト一覧：";
            // 
            // button_Build
            // 
            this.button_Build.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Build.Location = new System.Drawing.Point(369, 305);
            this.button_Build.Name = "button_Build";
            this.button_Build.Size = new System.Drawing.Size(75, 23);
            this.button_Build.TabIndex = 11;
            this.button_Build.Text = "ビルド実行";
            this.button_Build.UseVisualStyleBackColor = true;
            this.button_Build.Click += new System.EventHandler(this.button_Build_Click);
            // 
            // button_AddRaw
            // 
            this.button_AddRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_AddRaw.Location = new System.Drawing.Point(12, 305);
            this.button_AddRaw.Name = "button_AddRaw";
            this.button_AddRaw.Size = new System.Drawing.Size(75, 23);
            this.button_AddRaw.TabIndex = 8;
            this.button_AddRaw.Text = "一行追加";
            this.button_AddRaw.UseVisualStyleBackColor = true;
            this.button_AddRaw.Click += new System.EventHandler(this.button_AddRaw_Click);
            // 
            // BuildWorker
            // 
            this.BuildWorker.WorkerSupportsCancellation = true;
            this.BuildWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BuildWorker_DoWork);
            this.BuildWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BuildWorker_RunWorkerCompleted);
            // 
            // button_RemoveRaw
            // 
            this.button_RemoveRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_RemoveRaw.Location = new System.Drawing.Point(93, 305);
            this.button_RemoveRaw.Name = "button_RemoveRaw";
            this.button_RemoveRaw.Size = new System.Drawing.Size(75, 23);
            this.button_RemoveRaw.TabIndex = 9;
            this.button_RemoveRaw.Text = "一行削除";
            this.button_RemoveRaw.UseVisualStyleBackColor = true;
            this.button_RemoveRaw.Click += new System.EventHandler(this.button_RemoveRaw_Click);
            // 
            // button_SaveSetting
            // 
            this.button_SaveSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveSetting.Location = new System.Drawing.Point(393, 377);
            this.button_SaveSetting.Name = "button_SaveSetting";
            this.button_SaveSetting.Size = new System.Drawing.Size(85, 23);
            this.button_SaveSetting.TabIndex = 10;
            this.button_SaveSetting.Text = "設定保存";
            this.button_SaveSetting.UseVisualStyleBackColor = true;
            this.button_SaveSetting.Click += new System.EventHandler(this.button_SaveSetting_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(470, 363);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox_DeleteDirectoryName);
            this.tabPage1.Controls.Add(this.checkBox_DeleteDirectory);
            this.tabPage1.Controls.Add(this.textBox_BuildOption);
            this.tabPage1.Controls.Add(this.textBox_VisualStudioExePath);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(462, 337);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ビルド設定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox_DeleteDirectoryName
            // 
            this.textBox_DeleteDirectoryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DeleteDirectoryName.Location = new System.Drawing.Point(109, 103);
            this.textBox_DeleteDirectoryName.Name = "textBox_DeleteDirectoryName";
            this.textBox_DeleteDirectoryName.Size = new System.Drawing.Size(333, 19);
            this.textBox_DeleteDirectoryName.TabIndex = 5;
            // 
            // checkBox_DeleteDirectory
            // 
            this.checkBox_DeleteDirectory.AutoSize = true;
            this.checkBox_DeleteDirectory.Enabled = false;
            this.checkBox_DeleteDirectory.Location = new System.Drawing.Point(109, 81);
            this.checkBox_DeleteDirectory.Name = "checkBox_DeleteDirectory";
            this.checkBox_DeleteDirectory.Size = new System.Drawing.Size(263, 16);
            this.checkBox_DeleteDirectory.TabIndex = 4;
            this.checkBox_DeleteDirectory.Text = "ビルド後にプロジェクト以下の任意ディレクトリを削除";
            this.checkBox_DeleteDirectory.UseVisualStyleBackColor = true;
            this.checkBox_DeleteDirectory.CheckedChanged += new System.EventHandler(this.checkBox_DeleteDirectory_CheckedChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.textBox_LogDirectory);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(462, 337);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "アウトプット設定";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.checkBox_DetectBuildError);
            this.groupBox3.Controls.Add(this.textBox_DetectBuildErrorWord);
            this.groupBox3.Controls.Add(this.label_ExcludeWord);
            this.groupBox3.Controls.Add(this.label_DetectBuildErrorWord);
            this.groupBox3.Controls.Add(this.textBox_ExcludeWord);
            this.groupBox3.Controls.Add(this.checkBox_IsExclude);
            this.groupBox3.Location = new System.Drawing.Point(25, 46);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(417, 141);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ビルドエラー設定";
            // 
            // checkBox_DetectBuildError
            // 
            this.checkBox_DetectBuildError.AutoSize = true;
            this.checkBox_DetectBuildError.Location = new System.Drawing.Point(24, 23);
            this.checkBox_DetectBuildError.Name = "checkBox_DetectBuildError";
            this.checkBox_DetectBuildError.Size = new System.Drawing.Size(132, 16);
            this.checkBox_DetectBuildError.TabIndex = 6;
            this.checkBox_DetectBuildError.Text = "ビルド後にエラーを検知";
            this.checkBox_DetectBuildError.UseVisualStyleBackColor = true;
            this.checkBox_DetectBuildError.CheckedChanged += new System.EventHandler(this.checkBox_UpdateGUI);
            // 
            // textBox_DetectBuildErrorWord
            // 
            this.textBox_DetectBuildErrorWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DetectBuildErrorWord.Location = new System.Drawing.Point(113, 45);
            this.textBox_DetectBuildErrorWord.Name = "textBox_DetectBuildErrorWord";
            this.textBox_DetectBuildErrorWord.Size = new System.Drawing.Size(288, 19);
            this.textBox_DetectBuildErrorWord.TabIndex = 8;
            // 
            // label_ExcludeWord
            // 
            this.label_ExcludeWord.AutoSize = true;
            this.label_ExcludeWord.Location = new System.Drawing.Point(64, 105);
            this.label_ExcludeWord.Name = "label_ExcludeWord";
            this.label_ExcludeWord.Size = new System.Drawing.Size(127, 12);
            this.label_ExcludeWord.TabIndex = 10;
            this.label_ExcludeWord.Text = "実行中exeの検知ワード：";
            // 
            // label_DetectBuildErrorWord
            // 
            this.label_DetectBuildErrorWord.AutoSize = true;
            this.label_DetectBuildErrorWord.Location = new System.Drawing.Point(44, 48);
            this.label_DetectBuildErrorWord.Name = "label_DetectBuildErrorWord";
            this.label_DetectBuildErrorWord.Size = new System.Drawing.Size(63, 12);
            this.label_DetectBuildErrorWord.TabIndex = 7;
            this.label_DetectBuildErrorWord.Text = "検知ワード：";
            // 
            // textBox_ExcludeWord
            // 
            this.textBox_ExcludeWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_ExcludeWord.Location = new System.Drawing.Point(197, 102);
            this.textBox_ExcludeWord.Name = "textBox_ExcludeWord";
            this.textBox_ExcludeWord.Size = new System.Drawing.Size(204, 19);
            this.textBox_ExcludeWord.TabIndex = 11;
            // 
            // checkBox_IsExclude
            // 
            this.checkBox_IsExclude.AutoSize = true;
            this.checkBox_IsExclude.Location = new System.Drawing.Point(44, 80);
            this.checkBox_IsExclude.Name = "checkBox_IsExclude";
            this.checkBox_IsExclude.Size = new System.Drawing.Size(249, 16);
            this.checkBox_IsExclude.TabIndex = 9;
            this.checkBox_IsExclude.Text = "実行中exeが差し替えられないエラーは無視する";
            this.checkBox_IsExclude.UseVisualStyleBackColor = true;
            this.checkBox_IsExclude.CheckedChanged += new System.EventHandler(this.checkBox_UpdateGUI);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "ログ出力先：";
            // 
            // textBox_LogDirectory
            // 
            this.textBox_LogDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_LogDirectory.Location = new System.Drawing.Point(80, 16);
            this.textBox_LogDirectory.Name = "textBox_LogDirectory";
            this.textBox_LogDirectory.Size = new System.Drawing.Size(362, 19);
            this.textBox_LogDirectory.TabIndex = 5;
            this.textBox_LogDirectory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_LogDirectory_KeyDown);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button_AddAllSolution);
            this.tabPage2.Controls.Add(this.dataGridView);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.button_Build);
            this.tabPage2.Controls.Add(this.button_RemoveRaw);
            this.tabPage2.Controls.Add(this.button_AddRaw);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(462, 337);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "対象一覧";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button_AddAllSolution
            // 
            this.button_AddAllSolution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_AddAllSolution.Location = new System.Drawing.Point(174, 305);
            this.button_AddAllSolution.Name = "button_AddAllSolution";
            this.button_AddAllSolution.Size = new System.Drawing.Size(75, 23);
            this.button_AddAllSolution.TabIndex = 10;
            this.button_AddAllSolution.Text = "一括登録";
            this.button_AddAllSolution.UseVisualStyleBackColor = true;
            this.button_AddAllSolution.Click += new System.EventHandler(this.button_AddAllSolution_Click);
            // 
            // comboBox_Profile
            // 
            this.comboBox_Profile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_Profile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Profile.FormattingEnabled = true;
            this.comboBox_Profile.Location = new System.Drawing.Point(16, 379);
            this.comboBox_Profile.Name = "comboBox_Profile";
            this.comboBox_Profile.Size = new System.Drawing.Size(371, 20);
            this.comboBox_Profile.TabIndex = 13;
            this.comboBox_Profile.SelectedIndexChanged += new System.EventHandler(this.comboBox_Profile_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 412);
            this.Controls.Add(this.comboBox_Profile);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_SaveSetting);
            this.MinimumSize = new System.Drawing.Size(510, 450);
            this.Name = "Form1";
            this.Text = "VisualStudioBuilder";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Build;
        public System.Windows.Forms.TextBox textBox_VisualStudioExePath;
        public System.Windows.Forms.TextBox textBox_BuildOption;
        private System.Windows.Forms.Button button_AddRaw;
        private System.ComponentModel.BackgroundWorker BuildWorker;
        private System.Windows.Forms.Button button_RemoveRaw;
        private System.Windows.Forms.Button button_SaveSetting;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox comboBox_Profile;
        private System.Windows.Forms.Button button_AddAllSolution;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.CheckBox checkBox_DetectBuildError;
        public System.Windows.Forms.TextBox textBox_DetectBuildErrorWord;
        private System.Windows.Forms.Label label_ExcludeWord;
        private System.Windows.Forms.Label label_DetectBuildErrorWord;
        public System.Windows.Forms.TextBox textBox_ExcludeWord;
        public System.Windows.Forms.CheckBox checkBox_IsExclude;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox textBox_LogDirectory;
        public System.Windows.Forms.TextBox textBox_DeleteDirectoryName;
        public System.Windows.Forms.CheckBox checkBox_DeleteDirectory;

    }
}

