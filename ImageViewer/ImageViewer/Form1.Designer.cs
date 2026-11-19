namespace ImageViewer
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_FolderPath = new StandardTemplate.TextBoxEx();
            this.button_ListView = new System.Windows.Forms.Button();
            this.listView_Image = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.textBox_Extension = new StandardTemplate.TextBoxEx();
            this.label2 = new System.Windows.Forms.Label();
            this.hScrollBar_Scaling = new System.Windows.Forms.HScrollBar();
            this.comboBox_Profile = new StandardTemplate.ComboBoxEx();
            this.button_ProfileLoad = new System.Windows.Forms.Button();
            this.button_ProfileSave = new System.Windows.Forms.Button();
            this.button_SampleView = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_SortOrder = new System.Windows.Forms.ComboBox();
            this.checkBox_SortDescending = new System.Windows.Forms.CheckBox();
            this.progressBar_Load = new System.Windows.Forms.ProgressBar();
            this.button_LargeView = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "フォルダパス：";
            // 
            // textBox_FolderPath
            // 
            this.textBox_FolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_FolderPath.Location = new System.Drawing.Point(99, 6);
            this.textBox_FolderPath.Name = "textBox_FolderPath";
            this.textBox_FolderPath.Size = new System.Drawing.Size(342, 19);
            this.textBox_FolderPath.TabIndex = 1;
            this.textBox_FolderPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_FolderPath_KeyDown);
            // 
            // button_ListView
            // 
            this.button_ListView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ListView.Location = new System.Drawing.Point(374, 31);
            this.button_ListView.Name = "button_ListView";
            this.button_ListView.Size = new System.Drawing.Size(67, 22);
            this.button_ListView.TabIndex = 6;
            this.button_ListView.Text = "一覧表示";
            this.button_ListView.UseVisualStyleBackColor = true;
            this.button_ListView.Click += new System.EventHandler(this.button_ListView_Click);
            // 
            // listView_Image
            // 
            this.listView_Image.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_Image.HideSelection = false;
            this.listView_Image.Location = new System.Drawing.Point(12, 84);
            this.listView_Image.Name = "listView_Image";
            this.listView_Image.Size = new System.Drawing.Size(429, 192);
            this.listView_Image.TabIndex = 12;
            this.listView_Image.UseCompatibleStateImageBehavior = false;
            this.listView_Image.SelectedIndexChanged += new System.EventHandler(this.listView_Image_SelectedIndexChanged);
            this.listView_Image.DoubleClick += new System.EventHandler(this.listView_Image_DoubleClick);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // textBox_Extension
            // 
            this.textBox_Extension.Location = new System.Drawing.Point(99, 31);
            this.textBox_Extension.Name = "textBox_Extension";
            this.textBox_Extension.Size = new System.Drawing.Size(90, 19);
            this.textBox_Extension.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ファイル拡張子：";
            // 
            // hScrollBar_Scaling
            // 
            this.hScrollBar_Scaling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar_Scaling.Location = new System.Drawing.Point(205, 31);
            this.hScrollBar_Scaling.Name = "hScrollBar_Scaling";
            this.hScrollBar_Scaling.Size = new System.Drawing.Size(78, 19);
            this.hScrollBar_Scaling.TabIndex = 4;
            this.hScrollBar_Scaling.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scaling_Scroll);
            // 
            // comboBox_Profile
            // 
            this.comboBox_Profile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_Profile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Profile.FormattingEnabled = true;
            this.comboBox_Profile.Location = new System.Drawing.Point(12, 281);
            this.comboBox_Profile.Name = "comboBox_Profile";
            this.comboBox_Profile.Size = new System.Drawing.Size(271, 20);
            this.comboBox_Profile.Sorted = true;
            this.comboBox_Profile.TabIndex = 13;
            this.comboBox_Profile.SelectedIndexChanged += new System.EventHandler(this.comboBox_Profile_SelectedIndexChanged);
            // 
            // button_ProfileLoad
            // 
            this.button_ProfileLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ProfileLoad.Location = new System.Drawing.Point(289, 279);
            this.button_ProfileLoad.Name = "button_ProfileLoad";
            this.button_ProfileLoad.Size = new System.Drawing.Size(75, 23);
            this.button_ProfileLoad.TabIndex = 14;
            this.button_ProfileLoad.Text = "設定読込";
            this.button_ProfileLoad.UseVisualStyleBackColor = true;
            this.button_ProfileLoad.Click += new System.EventHandler(this.button_ProfileLoad_Click);
            // 
            // button_ProfileSave
            // 
            this.button_ProfileSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ProfileSave.Location = new System.Drawing.Point(366, 279);
            this.button_ProfileSave.Name = "button_ProfileSave";
            this.button_ProfileSave.Size = new System.Drawing.Size(75, 23);
            this.button_ProfileSave.TabIndex = 15;
            this.button_ProfileSave.Text = "設定保存";
            this.button_ProfileSave.UseVisualStyleBackColor = true;
            this.button_ProfileSave.Click += new System.EventHandler(this.button_ProfileSave_Click);
            // 
            // button_SampleView
            // 
            this.button_SampleView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SampleView.Location = new System.Drawing.Point(289, 31);
            this.button_SampleView.Name = "button_SampleView";
            this.button_SampleView.Size = new System.Drawing.Size(79, 22);
            this.button_SampleView.TabIndex = 5;
            this.button_SampleView.Text = "サンプル表示";
            this.button_SampleView.UseVisualStyleBackColor = true;
            this.button_SampleView.Click += new System.EventHandler(this.button_SampleView_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "並び順：";
            // 
            // comboBox_SortOrder
            // 
            this.comboBox_SortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SortOrder.FormattingEnabled = true;
            this.comboBox_SortOrder.Location = new System.Drawing.Point(99, 57);
            this.comboBox_SortOrder.Name = "comboBox_SortOrder";
            this.comboBox_SortOrder.Size = new System.Drawing.Size(90, 20);
            this.comboBox_SortOrder.TabIndex = 8;
            // 
            // checkBox_SortDescending
            // 
            this.checkBox_SortDescending.AutoSize = true;
            this.checkBox_SortDescending.Location = new System.Drawing.Point(195, 59);
            this.checkBox_SortDescending.Name = "checkBox_SortDescending";
            this.checkBox_SortDescending.Size = new System.Drawing.Size(48, 16);
            this.checkBox_SortDescending.TabIndex = 9;
            this.checkBox_SortDescending.Text = "降順";
            this.checkBox_SortDescending.UseVisualStyleBackColor = true;
            // 
            // progressBar_Load
            // 
            this.progressBar_Load.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_Load.Location = new System.Drawing.Point(249, 58);
            this.progressBar_Load.Name = "progressBar_Load";
            this.progressBar_Load.Size = new System.Drawing.Size(119, 18);
            this.progressBar_Load.TabIndex = 10;
            // 
            // button_LargeView
            // 
            this.button_LargeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_LargeView.Location = new System.Drawing.Point(374, 56);
            this.button_LargeView.Name = "button_LargeView";
            this.button_LargeView.Size = new System.Drawing.Size(67, 22);
            this.button_LargeView.TabIndex = 11;
            this.button_LargeView.Text = "拡大表示";
            this.button_LargeView.UseVisualStyleBackColor = true;
            this.button_LargeView.Click += new System.EventHandler(this.button_LargeView_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 311);
            this.Controls.Add(this.button_LargeView);
            this.Controls.Add(this.progressBar_Load);
            this.Controls.Add(this.checkBox_SortDescending);
            this.Controls.Add(this.comboBox_SortOrder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_Profile);
            this.Controls.Add(this.button_ProfileLoad);
            this.Controls.Add(this.button_ProfileSave);
            this.Controls.Add(this.hScrollBar_Scaling);
            this.Controls.Add(this.textBox_Extension);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView_Image);
            this.Controls.Add(this.button_SampleView);
            this.Controls.Add(this.button_ListView);
            this.Controls.Add(this.textBox_FolderPath);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(469, 349);
            this.Name = "Form1";
            this.Text = "ImageViewer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ListView;
        private System.Windows.Forms.ListView listView_Image;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.HScrollBar hScrollBar_Scaling;
        public StandardTemplate.ComboBoxEx comboBox_Profile;
        public System.Windows.Forms.Button button_ProfileLoad;
        public System.Windows.Forms.Button button_ProfileSave;
        public StandardTemplate.TextBoxEx textBox_FolderPath;
        public StandardTemplate.TextBoxEx textBox_Extension;
        private System.Windows.Forms.Button button_SampleView;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox comboBox_SortOrder;
        public System.Windows.Forms.CheckBox checkBox_SortDescending;
        public System.Windows.Forms.ProgressBar progressBar_Load;
        private System.Windows.Forms.Button button_LargeView;
    }
}

