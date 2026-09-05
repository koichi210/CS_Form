using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzBuzz.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、FizzBuzz+Woof拡張版のロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void 倍数が3ならFizzになる()
        {
            string result = Logic.FizzBuzz(3);
            StringAssert.Contains(result, "3 : Fizz" + Environment.NewLine);
        }

        [TestMethod]
        public void 倍数が5ならBuzzになる()
        {
            string result = Logic.FizzBuzz(5);
            StringAssert.Contains(result, "5 : Buzz" + Environment.NewLine);
        }

        [TestMethod]
        public void 倍数が7ならWoofになる()
        {
            string result = Logic.FizzBuzz(7);
            StringAssert.Contains(result, "7 : Woof" + Environment.NewLine);
        }

        [TestMethod]
        public void 倍数が3と5の公倍数ならFizzBuzzになる()
        {
            string result = Logic.FizzBuzz(15);
            StringAssert.Contains(result, "15 : FizzBuzz" + Environment.NewLine);
        }

        [TestMethod]
        public void 倍数が3と7の公倍数ならFizzWoofになる()
        {
            string result = Logic.FizzBuzz(21);
            StringAssert.Contains(result, "21 : FizzWoof" + Environment.NewLine);
        }

        [TestMethod]
        public void 倍数が5と7の公倍数ならBuzzWoofになる()
        {
            string result = Logic.FizzBuzz(35);
            StringAssert.Contains(result, "35 : BuzzWoof" + Environment.NewLine);
        }

        [TestMethod]
        public void 倍数が3と5と7の公倍数ならFizzBuzzWoofになる()
        {
            string result = Logic.FizzBuzz(105);
            StringAssert.Contains(result, "105 : FizzBuzzWoof" + Environment.NewLine);
        }

        [TestMethod]
        public void どれにも当てはまらない数はそのままの数字になる()
        {
            string result = Logic.FizzBuzz(1);
            Assert.AreEqual("1 : 1" + Environment.NewLine, result);
        }

        [TestMethod]
        public void 数がゼロなら空文字を返す()
        {
            string result = Logic.FizzBuzz(0);
            Assert.AreEqual("", result);
        }
    }
}
