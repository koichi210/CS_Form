using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graphics.Tests
{
    /// <summary>
    /// Form1（Chartコントロールに棒グラフ/円グラフを描画するサンプル）のテスト。
    /// MessageBox.Show を呼ぶ箇所は無いため、全ハンドラをテスト対象にできる。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void 棒グラフボタンでchart1に360点のSin波Seriesが追加される()
        {
            using (var form = new Form1())
            {
                var chart1 = (Chart)FormReflection.GetControl(form, "chart1");
                int beforeCount = chart1.Series.Count; // デザイナーで既定のSeriesが1つ登録済み

                FormReflection.InvokeHandler(form, "buttonBarGraph_Click", form);

                Assert.AreEqual(beforeCount + 1, chart1.Series.Count);
                var added = chart1.Series[chart1.Series.Count - 1];
                Assert.AreEqual(SeriesChartType.Line, added.ChartType);
                Assert.AreEqual(360, added.Points.Count);
            }
        }

        [TestMethod]
        public void 棒グラフボタンを連続で押すとSeriesが積み重なる()
        {
            using (var form = new Form1())
            {
                var chart1 = (Chart)FormReflection.GetControl(form, "chart1");
                int beforeCount = chart1.Series.Count; // デザイナーで既定のSeriesが1つ登録済み

                FormReflection.InvokeHandler(form, "buttonBarGraph_Click", form);
                FormReflection.InvokeHandler(form, "buttonBarGraph_Click", form);

                Assert.AreEqual(beforeCount + 2, chart1.Series.Count);
            }
        }

        [TestMethod]
        public void 円グラフボタンでPieのSeriesを持つ子Chartが追加される()
        {
            using (var form = new Form1())
            {
                FormReflection.InvokeHandler(form, "buttonCircleGraph_Click", form);

                var chart1 = (Chart)FormReflection.GetControl(form, "chart1");

                Assert.AreEqual(1, chart1.Controls.Count);
                var childChart = (Chart)chart1.Controls[0];
                Assert.AreEqual(1, childChart.Series.Count);
                Assert.AreEqual(SeriesChartType.Pie, childChart.Series[0].ChartType);
                Assert.AreEqual(5, childChart.Series[0].Points.Count);
            }
        }

        [TestMethod]
        public void 削除ボタンでSeriesと子Chartが1つずつ削除される()
        {
            using (var form = new Form1())
            {
                var chart1 = (Chart)FormReflection.GetControl(form, "chart1");
                int beforeSeriesCount = chart1.Series.Count; // デザイナーで既定のSeriesが1つ登録済み

                FormReflection.InvokeHandler(form, "buttonBarGraph_Click", form);
                FormReflection.InvokeHandler(form, "buttonCircleGraph_Click", form);

                Assert.AreEqual(beforeSeriesCount + 1, chart1.Series.Count);
                Assert.AreEqual(1, chart1.Controls.Count);

                FormReflection.InvokeHandler(form, "buttonDelete_Click", form);

                Assert.AreEqual(beforeSeriesCount, chart1.Series.Count);
                Assert.AreEqual(0, chart1.Controls.Count);
            }
        }

        [TestMethod]
        public void 削除ボタンは子Chartが無い状態で呼んでも例外にならない()
        {
            using (var form = new Form1())
            {
                var chart1 = (Chart)FormReflection.GetControl(form, "chart1");
                int beforeSeriesCount = chart1.Series.Count;

                FormReflection.InvokeHandler(form, "buttonDelete_Click", form);

                // Seriesが1つあれば1つ減るが、例外は起きないことを確認する
                Assert.AreEqual(0, chart1.Controls.Count);
                Assert.IsTrue(chart1.Series.Count <= beforeSeriesCount);
            }
        }
    }
}
