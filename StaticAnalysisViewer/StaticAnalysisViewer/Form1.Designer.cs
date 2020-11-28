namespace StaticAnalysisViewer
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.Button_LoadData = new System.Windows.Forms.Button();
            this.TextBox_LoadDataList = new System.Windows.Forms.TextBox();
            this.Combo_SortCategory = new System.Windows.Forms.ComboBox();
            this.Button_Sort = new System.Windows.Forms.Button();
            this.Button_Ranking = new System.Windows.Forms.Button();
            this.TextBox_Ranking = new System.Windows.Forms.TextBox();
            this.Label_Ranking = new System.Windows.Forms.Label();
            this.Combo_RankingWeekly = new System.Windows.Forms.ComboBox();
            this.TextBox_TopRankingNum = new System.Windows.Forms.TextBox();
            this.Label_TopRanking = new System.Windows.Forms.Label();
            this.Label_LoadDataList = new System.Windows.Forms.Label();
            this.Label_Sort = new System.Windows.Forms.Label();
            this.Chart_Result = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.ProgressBar_LoadStatus = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBox_CountLineTotal = new System.Windows.Forms.TextBox();
            this.TextBox_FileNumTotal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Help = new System.Windows.Forms.Button();
            this.comboBox_Profile = new System.Windows.Forms.ComboBox();
            this.button_ProfileSave = new System.Windows.Forms.Button();
            this.button_ProfileLoad = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Result)).BeginInit();
            this.SuspendLayout();
            // 
            // Button_LoadData
            // 
            this.Button_LoadData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_LoadData.Location = new System.Drawing.Point(411, 98);
            this.Button_LoadData.Name = "Button_LoadData";
            this.Button_LoadData.Size = new System.Drawing.Size(95, 23);
            this.Button_LoadData.TabIndex = 6;
            this.Button_LoadData.Text = "データ読み込み";
            this.Button_LoadData.UseVisualStyleBackColor = true;
            this.Button_LoadData.Click += new System.EventHandler(this.Button_AddData_Click);
            // 
            // TextBox_LoadDataList
            // 
            this.TextBox_LoadDataList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_LoadDataList.Location = new System.Drawing.Point(12, 18);
            this.TextBox_LoadDataList.Multiline = true;
            this.TextBox_LoadDataList.Name = "TextBox_LoadDataList";
            this.TextBox_LoadDataList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_LoadDataList.Size = new System.Drawing.Size(494, 78);
            this.TextBox_LoadDataList.TabIndex = 5;
            this.TextBox_LoadDataList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LoadDataPathKeyDown);
            // 
            // Combo_SortCategory
            // 
            this.Combo_SortCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Combo_SortCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Combo_SortCategory.Enabled = false;
            this.Combo_SortCategory.FormattingEnabled = true;
            this.Combo_SortCategory.Location = new System.Drawing.Point(12, 136);
            this.Combo_SortCategory.Name = "Combo_SortCategory";
            this.Combo_SortCategory.Size = new System.Drawing.Size(236, 20);
            this.Combo_SortCategory.TabIndex = 8;
            // 
            // Button_Sort
            // 
            this.Button_Sort.Location = new System.Drawing.Point(174, 157);
            this.Button_Sort.Name = "Button_Sort";
            this.Button_Sort.Size = new System.Drawing.Size(75, 23);
            this.Button_Sort.TabIndex = 9;
            this.Button_Sort.Text = "実行";
            this.Button_Sort.UseVisualStyleBackColor = true;
            this.Button_Sort.Visible = false;
            this.Button_Sort.Click += new System.EventHandler(this.Button_Sort_Click);
            // 
            // Button_Ranking
            // 
            this.Button_Ranking.Location = new System.Drawing.Point(87, 266);
            this.Button_Ranking.Name = "Button_Ranking";
            this.Button_Ranking.Size = new System.Drawing.Size(75, 23);
            this.Button_Ranking.TabIndex = 13;
            this.Button_Ranking.Text = "表示";
            this.Button_Ranking.UseVisualStyleBackColor = true;
            this.Button_Ranking.Click += new System.EventHandler(this.Button_RankShow_Click);
            // 
            // TextBox_Ranking
            // 
            this.TextBox_Ranking.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_Ranking.Location = new System.Drawing.Point(12, 315);
            this.TextBox_Ranking.Multiline = true;
            this.TextBox_Ranking.Name = "TextBox_Ranking";
            this.TextBox_Ranking.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_Ranking.Size = new System.Drawing.Size(494, 165);
            this.TextBox_Ranking.TabIndex = 17;
            this.TextBox_Ranking.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_SortResult_KeyDown);
            // 
            // Label_Ranking
            // 
            this.Label_Ranking.AutoSize = true;
            this.Label_Ranking.Location = new System.Drawing.Point(10, 190);
            this.Label_Ranking.Name = "Label_Ranking";
            this.Label_Ranking.Size = new System.Drawing.Size(60, 12);
            this.Label_Ranking.TabIndex = 10;
            this.Label_Ranking.Text = "表示する週";
            // 
            // Combo_RankingWeekly
            // 
            this.Combo_RankingWeekly.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Combo_RankingWeekly.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Combo_RankingWeekly.FormattingEnabled = true;
            this.Combo_RankingWeekly.Location = new System.Drawing.Point(12, 205);
            this.Combo_RankingWeekly.Name = "Combo_RankingWeekly";
            this.Combo_RankingWeekly.Size = new System.Drawing.Size(236, 20);
            this.Combo_RankingWeekly.TabIndex = 11;
            this.Combo_RankingWeekly.SelectedIndexChanged += new System.EventHandler(this.Combo_RankingWeekly_SelectedIndexChanged);
            // 
            // TextBox_TopRankingNum
            // 
            this.TextBox_TopRankingNum.Location = new System.Drawing.Point(11, 268);
            this.TextBox_TopRankingNum.Name = "TextBox_TopRankingNum";
            this.TextBox_TopRankingNum.Size = new System.Drawing.Size(70, 19);
            this.TextBox_TopRankingNum.TabIndex = 12;
            this.TextBox_TopRankingNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TextBox_TopRankingNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_TopRankingNum_KeyDown);
            this.TextBox_TopRankingNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_TopRankingNum_KeyPress);
            // 
            // Label_TopRanking
            // 
            this.Label_TopRanking.AutoSize = true;
            this.Label_TopRanking.Location = new System.Drawing.Point(9, 253);
            this.Label_TopRanking.Name = "Label_TopRanking";
            this.Label_TopRanking.Size = new System.Drawing.Size(72, 12);
            this.Label_TopRanking.TabIndex = 13;
            this.Label_TopRanking.Text = "表示する順位";
            // 
            // Label_LoadDataList
            // 
            this.Label_LoadDataList.AutoSize = true;
            this.Label_LoadDataList.Location = new System.Drawing.Point(10, 3);
            this.Label_LoadDataList.Name = "Label_LoadDataList";
            this.Label_LoadDataList.Size = new System.Drawing.Size(109, 12);
            this.Label_LoadDataList.TabIndex = 4;
            this.Label_LoadDataList.Text = "読み込みファイル一覧";
            // 
            // Label_Sort
            // 
            this.Label_Sort.AutoSize = true;
            this.Label_Sort.Location = new System.Drawing.Point(10, 120);
            this.Label_Sort.Name = "Label_Sort";
            this.Label_Sort.Size = new System.Drawing.Size(48, 12);
            this.Label_Sort.TabIndex = 7;
            this.Label_Sort.Text = "並び替え";
            // 
            // Chart_Result
            // 
            this.Chart_Result.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.Chart_Result.ChartAreas.Add(chartArea2);
            legend2.BorderColor = System.Drawing.Color.Black;
            legend2.Name = "Legend1";
            legend2.ShadowOffset = 10;
            legend2.TitleSeparator = System.Windows.Forms.DataVisualization.Charting.LegendSeparatorStyle.DashLine;
            this.Chart_Result.Legends.Add(legend2);
            this.Chart_Result.Location = new System.Drawing.Point(353, 136);
            this.Chart_Result.Name = "Chart_Result";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.Chart_Result.Series.Add(series2);
            this.Chart_Result.Size = new System.Drawing.Size(153, 149);
            this.Chart_Result.TabIndex = 15;
            this.Chart_Result.TabStop = false;
            this.Chart_Result.Text = "chart";
            this.Chart_Result.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 300);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "ランキング";
            // 
            // ProgressBar_LoadStatus
            // 
            this.ProgressBar_LoadStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar_LoadStatus.Location = new System.Drawing.Point(14, 484);
            this.ProgressBar_LoadStatus.Name = "ProgressBar_LoadStatus";
            this.ProgressBar_LoadStatus.Size = new System.Drawing.Size(492, 23);
            this.ProgressBar_LoadStatus.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(256, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 12);
            this.label3.TabIndex = 21;
            this.label3.Text = "CountLineの合計";
            // 
            // TextBox_CountLineTotal
            // 
            this.TextBox_CountLineTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_CountLineTotal.Location = new System.Drawing.Point(280, 266);
            this.TextBox_CountLineTotal.Name = "TextBox_CountLineTotal";
            this.TextBox_CountLineTotal.ReadOnly = true;
            this.TextBox_CountLineTotal.Size = new System.Drawing.Size(66, 19);
            this.TextBox_CountLineTotal.TabIndex = 22;
            this.TextBox_CountLineTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TextBox_FileNumTotal
            // 
            this.TextBox_FileNumTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_FileNumTotal.Location = new System.Drawing.Point(280, 203);
            this.TextBox_FileNumTotal.Name = "TextBox_FileNumTotal";
            this.TextBox_FileNumTotal.ReadOnly = true;
            this.TextBox_FileNumTotal.Size = new System.Drawing.Size(66, 19);
            this.TextBox_FileNumTotal.TabIndex = 24;
            this.TextBox_FileNumTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(261, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 12);
            this.label2.TabIndex = 23;
            this.label2.Text = "ファイル数の合計";
            // 
            // button_Help
            // 
            this.button_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Help.Location = new System.Drawing.Point(412, 511);
            this.button_Help.Name = "button_Help";
            this.button_Help.Size = new System.Drawing.Size(94, 23);
            this.button_Help.TabIndex = 22;
            this.button_Help.Text = "ヘルプ";
            this.button_Help.UseVisualStyleBackColor = true;
            this.button_Help.Click += new System.EventHandler(this.button_Help_Click);
            // 
            // comboBox_Profile
            // 
            this.comboBox_Profile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_Profile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Profile.FormattingEnabled = true;
            this.comboBox_Profile.Location = new System.Drawing.Point(14, 511);
            this.comboBox_Profile.Name = "comboBox_Profile";
            this.comboBox_Profile.Size = new System.Drawing.Size(189, 20);
            this.comboBox_Profile.TabIndex = 19;
            this.comboBox_Profile.SelectedIndexChanged += new System.EventHandler(this.comboBox_Profile_SelectedIndexChanged);
            // 
            // button_ProfileSave
            // 
            this.button_ProfileSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ProfileSave.Location = new System.Drawing.Point(311, 511);
            this.button_ProfileSave.Name = "button_ProfileSave";
            this.button_ProfileSave.Size = new System.Drawing.Size(94, 23);
            this.button_ProfileSave.TabIndex = 21;
            this.button_ProfileSave.Text = "設定値保存";
            this.button_ProfileSave.UseVisualStyleBackColor = true;
            this.button_ProfileSave.Click += new System.EventHandler(this.button_ProfileSave_Click);
            // 
            // button_ProfileLoad
            // 
            this.button_ProfileLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ProfileLoad.Location = new System.Drawing.Point(209, 511);
            this.button_ProfileLoad.Name = "button_ProfileLoad";
            this.button_ProfileLoad.Size = new System.Drawing.Size(94, 23);
            this.button_ProfileLoad.TabIndex = 20;
            this.button_ProfileLoad.Text = "設定値読込";
            this.button_ProfileLoad.UseVisualStyleBackColor = true;
            this.button_ProfileLoad.Click += new System.EventHandler(this.button_ProfileLoad_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 539);
            this.Controls.Add(this.comboBox_Profile);
            this.Controls.Add(this.button_ProfileLoad);
            this.Controls.Add(this.button_ProfileSave);
            this.Controls.Add(this.button_Help);
            this.Controls.Add(this.TextBox_FileNumTotal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBox_CountLineTotal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ProgressBar_LoadStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Chart_Result);
            this.Controls.Add(this.Label_Sort);
            this.Controls.Add(this.Label_LoadDataList);
            this.Controls.Add(this.Label_TopRanking);
            this.Controls.Add(this.TextBox_TopRankingNum);
            this.Controls.Add(this.Combo_RankingWeekly);
            this.Controls.Add(this.Label_Ranking);
            this.Controls.Add(this.Button_Ranking);
            this.Controls.Add(this.TextBox_Ranking);
            this.Controls.Add(this.Button_Sort);
            this.Controls.Add(this.Combo_SortCategory);
            this.Controls.Add(this.TextBox_LoadDataList);
            this.Controls.Add(this.Button_LoadData);
            this.MinimumSize = new System.Drawing.Size(534, 577);
            this.Name = "Form1";
            this.Text = "StaticAnalysisViewer";
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Result)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_LoadData;
        public System.Windows.Forms.TextBox TextBox_LoadDataList;
        private System.Windows.Forms.ComboBox Combo_SortCategory;
        private System.Windows.Forms.Button Button_Sort;
        private System.Windows.Forms.Button Button_Ranking;
        private System.Windows.Forms.TextBox TextBox_Ranking;
        private System.Windows.Forms.Label Label_Ranking;
        private System.Windows.Forms.ComboBox Combo_RankingWeekly;
        private System.Windows.Forms.Label Label_TopRanking;
        private System.Windows.Forms.Label Label_LoadDataList;
        private System.Windows.Forms.Label Label_Sort;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart_Result;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar ProgressBar_LoadStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBox_CountLineTotal;
        private System.Windows.Forms.TextBox TextBox_FileNumTotal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Help;
        private System.Windows.Forms.ComboBox comboBox_Profile;
        private System.Windows.Forms.Button button_ProfileSave;
        private System.Windows.Forms.Button button_ProfileLoad;
        public System.Windows.Forms.TextBox TextBox_TopRankingNum;
    }
}

