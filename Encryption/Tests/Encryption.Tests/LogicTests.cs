using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encryption.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、置換テーブルによる数字の暗号化/復号ロジック）
    /// のテスト。既定のテーブル "8 1 4 7 2 3 9 5 6 0" を使って検証する。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        private const string DefaultTable = "8 1 4 7 2 3 9 5 6 0";

        [TestMethod]
        public void エンコードでテーブルどおりに桁を置換する()
        {
            // table[2]=4, table[5]=3, table[7]=5 -> "257" -> "435"
            string result = Logic.Execute(DefaultTable, isDecode: false, keyText: "257");

            Assert.AreEqual("435", result);
        }

        [TestMethod]
        public void デコードでは反転したテーブルを使って置換する()
        {
            string result = Logic.Execute(DefaultTable, isDecode: true, keyText: "257");

            Assert.AreEqual("473", result);
        }

        [TestMethod]
        public void エンコードしてからデコードすると元の桁に戻る()
        {
            string encoded = Logic.Execute(DefaultTable, isDecode: false, keyText: "7");
            string decoded = Logic.Execute(DefaultTable, isDecode: true, keyText: encoded);

            Assert.AreEqual("7", decoded);
        }

        [TestMethod]
        public void キーが0なら空文字を返す()
        {
            // while(word != 0) の条件により、word=0のときはループが一度も実行されない
            string result = Logic.Execute(DefaultTable, isDecode: false, keyText: "0");

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void 単一桁のエンコードはテーブルの該当位置の値になる()
        {
            // table[9] = 0
            string result = Logic.Execute(DefaultTable, isDecode: false, keyText: "9");

            Assert.AreEqual("0", result);
        }
    }
}
