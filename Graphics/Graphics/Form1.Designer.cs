namespace Graphics
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
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonBarGraph = new System.Windows.Forms.Button();
            this.buttonCircleGraph = new System.Windows.Forms.Button();
            this.Delete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(22, 12);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(300, 300);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // buttonBarGraph
            // 
            this.buttonBarGraph.Location = new System.Drawing.Point(166, 318);
            this.buttonBarGraph.Name = "buttonBarGraph";
            this.buttonBarGraph.Size = new System.Drawing.Size(75, 23);
            this.buttonBarGraph.TabIndex = 1;
            this.buttonBarGraph.Text = "棒グラフ";
            this.buttonBarGraph.UseVisualStyleBackColor = true;
            this.buttonBarGraph.Click += new System.EventHandler(this.buttonBarGraph_Click);
            // 
            // buttonCircleGraph
            // 
            this.buttonCircleGraph.Location = new System.Drawing.Point(85, 318);
            this.buttonCircleGraph.Name = "buttonCircleGraph";
            this.buttonCircleGraph.Size = new System.Drawing.Size(75, 23);
            this.buttonCircleGraph.TabIndex = 2;
            this.buttonCircleGraph.Text = "円グラフ";
            this.buttonCircleGraph.UseVisualStyleBackColor = true;
            this.buttonCircleGraph.Click += new System.EventHandler(this.buttonCircleGraph_Click);
            // 
            // Delete
            // 
            this.Delete.Location = new System.Drawing.Point(247, 318);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(75, 23);
            this.Delete.TabIndex = 3;
            this.Delete.Text = "消去";
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 400);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.buttonCircleGraph);
            this.Controls.Add(this.buttonBarGraph);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button buttonBarGraph;
        private System.Windows.Forms.Button buttonCircleGraph;
        private System.Windows.Forms.Button Delete;
    }
}

