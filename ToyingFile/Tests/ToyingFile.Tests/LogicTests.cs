using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToyingFile.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、ファイル内容から指定文字列を含む行を処理する
    /// ロジック）のテスト。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void 該当行だけ文字列を削除する()
        {
            string content = "keep this" + Environment.NewLine + "delete THIS word" + Environment.NewLine + "keep too";

            string result = Logic.DeleteStringFromContent(content, new[] { "THIS" }, exactMatch: true, deleteWholeLine: false);

            Assert.AreEqual("keep this" + Environment.NewLine + "delete  word" + Environment.NewLine + "keep too", result);
        }

        [TestMethod]
        public void deleteWholeLineがtrueなら行ごと空行になる()
        {
            string content = "keep this" + Environment.NewLine + "delete this line" + Environment.NewLine + "keep too";

            string result = Logic.DeleteStringFromContent(content, new[] { "delete" }, exactMatch: true, deleteWholeLine: true);

            Assert.AreEqual("keep this" + Environment.NewLine + "" + Environment.NewLine + "keep too", result);
        }

        [TestMethod]
        public void exactMatchがtrueなら大文字小文字を区別して検索する()
        {
            string content = "target line" + Environment.NewLine + "TARGET line";

            string result = Logic.DeleteStringFromContent(content, new[] { "target" }, exactMatch: true, deleteWholeLine: true);

            // 1行目だけ完全一致するので空行に、2行目(大文字)は対象外でそのまま残る
            Assert.AreEqual("" + Environment.NewLine + "TARGET line", result);
        }

        [TestMethod]
        public void exactMatchがfalseなら大文字小文字を区別せず検索する()
        {
            string content = "target line" + Environment.NewLine + "TARGET line";

            string result = Logic.DeleteStringFromContent(content, new[] { "target" }, exactMatch: false, deleteWholeLine: true);

            // どちらも検索には一致するので両方空行になる
            Assert.AreEqual("" + Environment.NewLine + "", result);
        }

        [TestMethod]
        public void 置換のReplace自体は大文字小文字を区別する既知の仕様()
        {
            // exactMatch=false で検索は一致しても、実際の置換(String.Replace)は
            // 常に大文字小文字を区別するため、大文字側の文字列は消えずに残る。
            string content = "TARGET line";

            string result = Logic.DeleteStringFromContent(content, new[] { "target" }, exactMatch: false, deleteWholeLine: false);

            Assert.AreEqual("TARGET line", result, "検索はヒットするが、Replaceが一致せず削除されない");
        }

        [TestMethod]
        public void 空行は判定をスキップする()
        {
            string content = "" + Environment.NewLine + "keep this";

            string result = Logic.DeleteStringFromContent(content, new[] { "keep" }, exactMatch: true, deleteWholeLine: true);

            Assert.AreEqual("" + Environment.NewLine + "", result);
        }

        [TestMethod]
        public void 複数の削除対象文字列を扱える()
        {
            string content = "foo bar baz";

            string result = Logic.DeleteStringFromContent(content, new[] { "foo", "baz" }, exactMatch: true, deleteWholeLine: false);

            Assert.AreEqual(" bar ", result);
        }
    }
}
