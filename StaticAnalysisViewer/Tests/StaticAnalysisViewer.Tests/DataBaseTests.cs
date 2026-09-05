using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StaticAnalysisViewer.Tests
{
    /// <summary>
    /// DataBase（CSVを取り込んでランキング用に保持する、Form非依存の public class）のテスト。
    ///
    /// CreateArray は "ヘッダ行\nデータ行1\nデータ行2\n"（各行カンマ区切り、末尾に改行）
    /// という形式を前提にしている。末尾の改行によって Split('\n') の最後に空文字列の要素が
    /// でき、それが1要素だけの配列としてデータに混ざる（＝短い行＝空行として後段で除外される
    /// 前提）。テストデータもこの実際の入力形式に合わせている。
    /// </summary>
    [TestClass]
    public class DataBaseTests
    {
        private const string SampleCsv =
            "FileName,CountLine,CountCode,Cyclomatic\n" +
            "\"a.cs\",100,80,5\n" +
            "\"b.cs\",50,40,10\n" +
            "\"c.cs\",30,20,3\n";

        [TestMethod]
        public void CreateArrayでヘッダがCategoryとして取得できる()
        {
            var db = new DataBase();
            db.Initialize();

            db.CreateArray(SampleCsv, "week1");

            CollectionAssert.AreEqual(new[] { "FileName", "CountLine", "CountCode", "Cyclomatic" }, db.GetCategory());
        }

        [TestMethod]
        public void CreateArrayで配列数が1つ増える()
        {
            var db = new DataBase();
            db.Initialize();

            Assert.AreEqual(0, db.GetArrayNum());
            db.CreateArray(SampleCsv, "week1");
            Assert.AreEqual(1, db.GetArrayNum());
            db.CreateArray(SampleCsv, "week2");
            Assert.AreEqual(2, db.GetArrayNum());
        }

        [TestMethod]
        public void GetRowNumは最初のデータ行の列数になる()
        {
            var db = new DataBase();
            db.Initialize();

            db.CreateArray(SampleCsv, "week1");

            Assert.AreEqual(4, db.GetRowNum(), "FileName,CountLine,CountCode,Cyclomatic の4列");
        }

        [TestMethod]
        public void SortDataは指定列の降順に並び替える()
        {
            var db = new DataBase();
            db.Initialize();
            db.CreateArray(SampleCsv, "week1");

            // Cyclomatic(index=3) で並び替え： 5, 10, 3 → 降順で 10, 5, 3
            db.SortData(0, 3);

            var array = db.GetData(0);
            Assert.AreEqual("\"b.cs\"", array.Data[0][0]);
            Assert.AreEqual("\"a.cs\"", array.Data[1][0]);
            Assert.AreEqual("\"c.cs\"", array.Data[2][0]);
        }

        [TestMethod]
        public void GetIdxは部分一致するファイル名の行番号を返す()
        {
            var db = new DataBase();
            db.Initialize();
            db.CreateArray(SampleCsv, "week1");

            int idx = db.GetIdx(0, 0, "b.cs");

            Assert.AreEqual(1, idx, "2行目(index=1)がb.csのはず");
        }

        [TestMethod]
        public void GetIdxは見つからなければUNKNOWN_IDXを返す()
        {
            var db = new DataBase();
            db.Initialize();
            db.CreateArray(SampleCsv, "week1");

            int idx = db.GetIdx(0, 0, "not_exist.cs");

            Assert.AreEqual(db.UNKNOWN_IDX, idx);
        }

        [TestMethod]
        public void GetIdxはArrayIdxが負ならUNKNOWN_IDXを返す()
        {
            var db = new DataBase();
            db.Initialize();
            db.CreateArray(SampleCsv, "week1");

            int idx = db.GetIdx(-1, 0, "a.cs");

            Assert.AreEqual(db.UNKNOWN_IDX, idx, "初回データ(前回データが無い)の判定に使われる");
        }

        [TestMethod]
        public void Initializeで状態がリセットされる()
        {
            var db = new DataBase();
            db.Initialize();
            db.CreateArray(SampleCsv, "week1");
            Assert.AreEqual(1, db.GetArrayNum());

            db.Initialize();

            Assert.AreEqual(0, db.GetArrayNum());
            Assert.IsNull(db.GetCategory());
        }

        [TestMethod]
        public void GetColumnNumはCreateArrayしたデータ行数になる()
        {
            var db = new DataBase();
            db.Initialize();
            db.CreateArray(SampleCsv, "week1");

            // ヘッダを除く3行 + 末尾の空行1つ = 4
            Assert.AreEqual(4, db.GetColumnNum(0));
        }
    }
}
