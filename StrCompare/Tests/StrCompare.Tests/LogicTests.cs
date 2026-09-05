using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrCompare.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、文字列比較の挙動を確認するロジック）の
    /// テスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void Compareは完全一致で大文字小文字が異なればfalseになる()
        {
            string result = Logic.Compare("SampleString", "samplestring");

            StringAssert.Contains(result, "大文字小文字区別する（完全一致） =False");
        }

        [TestMethod]
        public void Compareは大文字小文字を無視すれば完全一致でtrueになる()
        {
            string result = Logic.Compare("SampleString", "samplestring");

            StringAssert.Contains(result, "大文字小文字区別しない（完全一致） =True");
        }

        [TestMethod]
        public void Compareは大文字小文字を無視した前方一致を判定する()
        {
            string result = Logic.Compare("SampleStringExtra", "samplestring");

            StringAssert.Contains(result, "大文字小文字区別しない（前方一致） =True");
        }

        [TestMethod]
        public void Compareは完全一致なら全ての比較結果がtrueになる()
        {
            string result = Logic.Compare("SameText", "SameText");

            StringAssert.Contains(result, "大文字小文字区別する（完全一致） =True");
            StringAssert.Contains(result, "大文字小文字区別しない（完全一致） =True");
            StringAssert.Contains(result, "大文字小文字区別しない（前方一致） =True");
        }

        [TestMethod]
        public void SampleCompareは固定文字列での比較結果を含む()
        {
            string result = Logic.SampleCompare();

            StringAssert.Contains(result, "大文字小文字区別する（完全一致） =True" + Environment.NewLine + Environment.NewLine);
            StringAssert.Contains(result, "大文字小文字区別する（完全一致） =False");
            StringAssert.Contains(result, "大文字小文字区別しない（完全一致） =True");
            StringAssert.Contains(result, "大文字小文字区別しない（前方一致） =True");
        }
    }
}
