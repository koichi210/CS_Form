using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Graphics
{
    public partial class Form1 : Form
    {
        private int StartBarPoint = 0;  // 棒グラフの描画基点（描画するたびに基点をインクリ）

        private Series series;
        private Chart chart;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBarGraph_Click(object sender, EventArgs e)
        {
            //Seriesの作成
            series = new Series();

            //グラフのタイプを指定(今回は線)
            series.ChartType = SeriesChartType.Line;

            //グラフのデータを追加(試しにsin関数)
            for (int i = 0; i < 360; i++)
            {
                series.Points.AddXY(i, Math.Sin((i - StartBarPoint) * Math.PI / 180.0));
            }

            //作ったSeriesをchartコントロールに追加する
            chart1.Series.Add(series);

            StartBarPoint += 10;
        }

        private void buttonCircleGraph_Click(object sender, EventArgs e)
        {
            chart = new Chart();

            chart.Width = 200;
            chart.Height = 200;

            series = new Series();
            series.ChartType = SeriesChartType.Pie;
            series["PieStartAngle"] = "270";

            DataPoint point = new DataPoint();
            point.XValue = 0;
            point.YValues = new double[] { 30 };
            point.Color = System.Drawing.Color.Red;
            //point.BackSecondaryColor = System.Drawing.Color.DarkRed;
            //point.BackGradientStyle = GradientStyle.DiagonalLeft;
            series.Points.Add(point);

            point = new DataPoint();
            point.XValue = 0;
            point.YValues = new double[] { 10 };
            point.Color = System.Drawing.Color.Khaki;
            series.Points.Add(point);

            point = new DataPoint();
            point.XValue = 0;
            point.YValues = new double[] { 20 };
            point.Color = System.Drawing.Color.AliceBlue;
            series.Points.Add(point);

            point = new DataPoint();
            point.XValue = 0;
            point.YValues = new double[] { 20 };
            series.Points.Add(point);

            point = new DataPoint();
            point.XValue = 0;
            point.YValues = new double[] { 20 };
            series.Points.Add(point);

            chart.Series.Add(series);

            ChartArea area = new ChartArea();
            area.AxisX.IsLabelAutoFit = true;
            area.AxisY.IsLabelAutoFit = true;
            chart.ChartAreas.Add(area);

            chart1.Controls.Add(chart);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            //chart1.Controls.Remove(chart);

            if (chart1.Controls.Count > 0)
            {
                chart1.Controls.RemoveAt(0);
            }

            if (chart1.Series.Count > 0)
            {
                chart1.Series.RemoveAt(0);
            }
        }
    }
}
