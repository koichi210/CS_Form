using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictMerge;

namespace PictMerge2.Tests
{
    /// <summary>
    /// TrimHeights.Parse(切断位置の入力の読み取り)のテスト。
    /// 1行に1つの書き方と、「開始,終了」のカンマ区切りの書き方のどちらでも読めることを確認する。
    /// </summary>
    [TestClass]
    public class TrimHeightsTests
    {
        [TestMethod]
        public void Parse_1行に1つずつ_小さい順に並ぶ()
        {
            List<int> result = TrimHeights.Parse("150" + Environment.NewLine + "50" + Environment.NewLine + "100");
            CollectionAssert.AreEqual(new List<int> { 50, 100, 150 }, result);
        }

        [TestMethod]
        public void Parse_カンマ区切りの範囲指定_境目の一覧として読める()
        {
            // Debugビルドの初期値と同じ書き方。以前はこれで「結合」を押すと例外になっていた
            List<int> result = TrimHeights.Parse("50,100" + Environment.NewLine + "150,200");
            CollectionAssert.AreEqual(new List<int> { 50, 100, 150, 200 }, result);
        }

        [TestMethod]
        public void Parse_空行や空白は無視する()
        {
            List<int> result = TrimHeights.Parse(Environment.NewLine + " 50 " + Environment.NewLine + Environment.NewLine + "100," + Environment.NewLine);
            CollectionAssert.AreEqual(new List<int> { 50, 100 }, result);
        }

        [TestMethod]
        public void Parse_LFだけの改行でも読める()
        {
            List<int> result = TrimHeights.Parse("50\n100");
            CollectionAssert.AreEqual(new List<int> { 50, 100 }, result);
        }

        [TestMethod]
        public void Parse_空文字_空の一覧()
        {
            Assert.AreEqual(0, TrimHeights.Parse("").Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_数字でない値_FormatException()
        {
            TrimHeights.Parse("50" + Environment.NewLine + "abc");
        }
    }
}
