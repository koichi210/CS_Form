using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rotation.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、矢印キーによる数値インクリメントと
    /// 画像回転描画のための座標計算ロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void UpdateValueは上矢印キーで1増える()
        {
            Assert.AreEqual("6", Logic.UpdateValue("5", Keys.Up));
        }

        [TestMethod]
        public void UpdateValueは下矢印キーで1減る()
        {
            Assert.AreEqual("4", Logic.UpdateValue("5", Keys.Down));
        }

        [TestMethod]
        public void UpdateValueはEnterキーでは変化しない()
        {
            Assert.AreEqual("5", Logic.UpdateValue("5", Keys.Enter));
        }

        [TestMethod]
        public void UpdateValueは数値でなければそのまま返す()
        {
            Assert.AreEqual("abc", Logic.UpdateValue("abc", Keys.Up));
        }

        [TestMethod]
        public void ComputeCanvasSizeは幅の方が大きければ幅を基準に正方形にする()
        {
            Size result = Logic.ComputeCanvasSize(100, 50);
            Assert.AreEqual(new Size(200, 200), result);
        }

        [TestMethod]
        public void ComputeCanvasSizeは高さの方が大きければ高さを基準に正方形にする()
        {
            Size result = Logic.ComputeCanvasSize(50, 100);
            Assert.AreEqual(new Size(200, 200), result);
        }

        [TestMethod]
        public void ComputeDestinationPointsは角度0で原点から幅高さ方向にそのまま伸びる()
        {
            PointF[] result = Logic.ComputeDestinationPoints(100, 50, 0, 10, 20);

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(new PointF(10, 20), result[0]);
            AssertApprox(110, result[1].X);
            AssertApprox(20, result[1].Y);
            AssertApprox(10, result[2].X);
            AssertApprox(70, result[2].Y);
        }

        private void AssertApprox(float expected, float actual, float tolerance = 0.01f)
        {
            Assert.IsTrue(Math.Abs(expected - actual) < tolerance, string.Format("期待値={0}, 実際={1}", expected, actual));
        }
    }
}
