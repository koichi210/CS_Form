using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HelloWolrd.Tests
{
    /// <summary>
    /// Logic（Program.cs の Main から切り出した挨拶文字列）のテスト。
    ///
    /// ⚠️ Main は Console.ReadKey() でキー入力待ちになりテストがハングするため、
    /// Main自体は呼び出さず、挨拶文字列を返すLogic.GetGreetingのみをテストする。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void 挨拶文字列はHelloWorldである()
        {
            Assert.AreEqual("Hello World!", Logic.GetGreeting());
        }
    }
}
