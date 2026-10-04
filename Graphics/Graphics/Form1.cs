using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Graphics
{
    public partial class Form1 : Form
    {
        private int _startDegree = 0;  // Sin波の描画基点[度]（描画するたびに基点をインクリ）

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonLineGraph_Click(object sender, EventArgs e)
        {
            //Seriesの作成。グラフのタイプを指定(今回は線)
            Series series = new Series { ChartType = SeriesChartType.Line };

            //グラフのデータを追加(試しにsin関数)
            for (int i = 0; i < 360; i++)
            {
                series.Points.AddXY(i, Math.Sin((i - _startDegree) * Math.PI / 180.0));
            }

            //作ったSeriesをchartコントロールに追加する
            chart1.Series.Add(series);

            _startDegree += 10;
        }

        private void buttonCircleGraph_Click(object sender, EventArgs e)
        {
            Chart chart = new Chart
            {
                Width = 200,
                Height = 200,
            };

            Series series = new Series { ChartType = SeriesChartType.Pie };
            series["PieStartAngle"] = "270";

            // 色を指定しない(Color.Empty)要素はChartの既定の配色になる
            // (グラデーションにしたい場合はBackSecondaryColor/BackGradientStyleも指定する)
            AddPiePoint(series, 30, Color.Red);
            AddPiePoint(series, 10, Color.Khaki);
            AddPiePoint(series, 20, Color.AliceBlue);
            AddPiePoint(series, 20, Color.Empty);
            AddPiePoint(series, 20, Color.Empty);

            chart.Series.Add(series);

            ChartArea area = new ChartArea();
            area.AxisX.IsLabelAutoFit = true;
            area.AxisY.IsLabelAutoFit = true;
            chart.ChartAreas.Add(area);

            chart1.Controls.Add(chart);
        }

        private static void AddPiePoint(Series series, double value, Color color)
        {
            series.Points.Add(new DataPoint
            {
                XValue = 0,
                YValues = new double[] { value },
                Color = color,
            });
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
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
