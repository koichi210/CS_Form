using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialogChild.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、子ウィンドウ移動先座標を画面端でクランプする
    /// ロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void GetSubValueは通常時は単純に減算する()
        {
            Assert.AreEqual(70, Logic.GetSubValue(100, 30));
        }

        [TestMethod]
        public void GetSubValueは負値になる場合0に張り付く()
        {
            Assert.AreEqual(0, Logic.GetSubValue(10, 30));
        }

        [TestMethod]
        public void GetSubValueはoffsetValueも考慮する()
        {
            Assert.AreEqual(0, Logic.GetSubValue(50, 30, 25));
        }

        [TestMethod]
        public void GetAddValueは通常時は単純に加算する()
        {
            Assert.AreEqual(130, Logic.GetAddValue(1000, 100, 30));
        }

        [TestMethod]
        public void GetAddValueは上限を超える場合maxValueに張り付く()
        {
            Assert.AreEqual(1000, Logic.GetAddValue(1000, 990, 30));
        }

        [TestMethod]
        public void GetAddValueはoffsetValueを考慮して張り付く()
        {
            // maxValue(1000) - offsetValue(200) = 800 が上限
            Assert.AreEqual(800, Logic.GetAddValue(1000, 790, 30, 200));
        }
    }
}
