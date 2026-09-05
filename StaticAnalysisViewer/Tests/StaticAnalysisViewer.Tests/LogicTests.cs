using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StaticAnalysisViewer.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、ランキング文字列の組み立てロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void CreateLabelName_直上のフォルダ名を取り出す()
        {
            string result = Logic.CreateLabelName(@"C:\reports\week1\output.csv");

            Assert.AreEqual("week1", result);
        }

        [TestMethod]
        public void CreatePreRankingString_前回データが無ければNew()
        {
            var db = new DataBase();
            db.Initialize();

            string result = Logic.CreatePreRankingString(db, 0, db.UNKNOWN_IDX);

            Assert.AreEqual("New!", result);
        }

        [TestMethod]
        public void CreatePreRankingString_順位が上がった場合()
        {
            var db = new DataBase();
            db.Initialize();

            // 前回3位(idx=2)から今回0位(idx=0)へ上昇
            string result = Logic.CreatePreRankingString(db, 0, 2);

            StringAssert.StartsWith(result, "↑");
            StringAssert.Contains(result, "3"); // 前回順位は1相対で表示される
        }

        [TestMethod]
        public void CreatePreRankingString_順位が下がった場合()
        {
            var db = new DataBase();
            db.Initialize();

            // 前回1位(idx=0)から今回3位(idx=2)へ下降
            string result = Logic.CreatePreRankingString(db, 2, 0);

            StringAssert.StartsWith(result, "↓");
        }

        [TestMethod]
        public void CreatePreRankingString_順位が変わらない場合()
        {
            var db = new DataBase();
            db.Initialize();

            string result = Logic.CreatePreRankingString(db, 1, 1);

            StringAssert.StartsWith(result, "－");
        }

        [TestMethod]
        public void CreateCountNumTotalは各行のCountLineを合計する()
        {
            var db = new DataBase();
            db.Initialize();
            db.CreateArray(
                "FileName,CountLine,CountCode,Cyclomatic\n" +
                "\"a.cs\",100,80,5\n" +
                "\"b.cs\",50,40,10\n",
                "week1");

            var array = db.GetData(0);
            int total = Logic.CreateCountNumTotal(db, array);

            Assert.AreEqual(150, total, "短い(空)行は除外して100+50のはず");
        }

        [TestMethod]
        public void CreateRankingStringはヘッダと各行を整形して並べる()
        {
            var db = new DataBase();
            db.Initialize();
            db.CreateArray(
                "FileName,CountLine,CountCode,Cyclomatic\n" +
                "\"a.cs\",100,80,5\n",
                "week1");

            var array = db.GetData(0);
            string result = Logic.CreateRankingString(db, -1, array, 10);

            StringAssert.Contains(result, "Rank");
            StringAssert.Contains(result, "a.cs");
            StringAssert.Contains(result, "New!", "前回データが無い(PreArrayIdx=-1)ので New! になる");
        }
    }
}
