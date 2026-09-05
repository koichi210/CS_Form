using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToyingData.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、全角→半角変換のロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void GetRegesStr_何も選ばれていなければfalse()
        {
            string dummy;
            bool result = Logic.GetRegesStr(false, false, false, false, out dummy);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetRegesStr_数字のみ選ぶとその範囲だけ含む()
        {
            string result;
            Logic.GetRegesStr(true, false, false, false, out result);

            Assert.AreEqual("[０-９]", result);
        }

        [TestMethod]
        public void GetRegesStr_複数選ぶと結合される()
        {
            string result;
            bool success = Logic.GetRegesStr(true, true, false, true, out result);

            Assert.IsTrue(success);
            Assert.AreEqual("[０-９Ａ-Ｚ　]", result);
        }

        [TestMethod]
        public void ApplyWide2Narrow_全角数字を半角にする()
        {
            string regesStr;
            Logic.GetRegesStr(true, false, false, false, out regesStr);

            string[] result = Logic.ApplyWide2Narrow(new[] { "１２３", "abc" }, regesStr);

            Assert.AreEqual("123", result[0]);
            Assert.AreEqual("abc", result[1], "対象外の文字はそのまま");
        }

        [TestMethod]
        public void ApplyWide2Narrow_対象文字が無ければ変化しない()
        {
            string regesStr;
            Logic.GetRegesStr(true, false, false, false, out regesStr);

            string[] result = Logic.ApplyWide2Narrow(new[] { "あいう" }, regesStr);

            Assert.AreEqual("あいう", result[0]);
        }
    }
}
