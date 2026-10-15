using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrimHtmlData.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、HTMLソースから検索ワードにヒットする行を
    /// 抽出するロジック）のテスト。
    ///
    /// ⚠️ button_Execute_Click は WebClient で実サイトへ通信するため、
    /// Form1 レベルの統合テストは行わない（ネットワーク依存、失敗時に
    /// MessageBox.Show も呼ばれる）。ここでは切り出した Logic のみを対象にする。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void TryGetTrimLineは数値文字列をそのまま返す()
        {
            int trimLineNum;
            Assert.IsTrue(Logic.TryGetTrimLine("3", out trimLineNum));
            Assert.AreEqual(3, trimLineNum);
        }

        [TestMethod]
        public void TryGetTrimLineは0や空文字なら1を返す()
        {
            int trimLineNum;
            Assert.IsTrue(Logic.TryGetTrimLine("0", out trimLineNum));
            Assert.AreEqual(1, trimLineNum);
            Assert.IsTrue(Logic.TryGetTrimLine("", out trimLineNum));
            Assert.AreEqual(1, trimLineNum);
            Assert.IsTrue(Logic.TryGetTrimLine("  ", out trimLineNum));
            Assert.AreEqual(1, trimLineNum);
        }

        [TestMethod]
        public void TryGetTrimLineは数字以外や負の数なら例外にせずfalseを返す()
        {
            int trimLineNum;
            Assert.IsFalse(Logic.TryGetTrimLine("abc", out trimLineNum));
            Assert.IsFalse(Logic.TryGetTrimLine("1.5", out trimLineNum));
            Assert.IsFalse(Logic.TryGetTrimLine("３", out trimLineNum));
            Assert.IsFalse(Logic.TryGetTrimLine("-1", out trimLineNum));
            Assert.IsFalse(Logic.TryGetTrimLine("99999999999", out trimLineNum));
        }

        [TestMethod]
        public void SplitLinesはCRLFとLFとCRのどれでも行に分け空行は捨てる()
        {
            CollectionAssert.AreEqual(new[] { "a", "b", "c", "d", "e" },
                Logic.SplitLines("a\r\nb\nc\rd\r\n\r\n\n\re"));
        }

        [TestMethod]
        public void LFだけやCRだけのHTMLでも行ごとに検索できる()
        {
            string expected = "<title>A</title>" + Environment.NewLine + "<body>" + Environment.NewLine + Environment.NewLine + Environment.NewLine;

            Assert.AreEqual(expected, Logic.GetSearchString("<html>\n<title>A</title>\n<body>\n</html>", "<title>", 2, StringComparison.OrdinalIgnoreCase, firstWordOnly: false));
            Assert.AreEqual(expected, Logic.GetSearchString("<html>\r<title>A</title>\r<body>\r</html>", "<title>", 2, StringComparison.OrdinalIgnoreCase, firstWordOnly: false));
        }

        [TestMethod]
        public void ヒットした行を抽出する()
        {
            string source = "line0" + Environment.NewLine + "hit target here" + Environment.NewLine + "line2";

            string result = Logic.GetSearchString(source, "target", 1, StringComparison.OrdinalIgnoreCase, firstWordOnly: false);

            StringAssert.Contains(result, "hit target here");
        }

        [TestMethod]
        public void TrimLineNumで前後の行もまとめて取得する()
        {
            string source = "hit target here" + Environment.NewLine + "next line" + Environment.NewLine + "extra line";

            string result = Logic.GetSearchString(source, "target", 2, StringComparison.OrdinalIgnoreCase, firstWordOnly: false);

            StringAssert.Contains(result, "hit target here");
            StringAssert.Contains(result, "next line");
        }

        [TestMethod]
        public void firstWordOnlyがtrueなら最初のヒットだけで打ち切る()
        {
            string source = "hit target one" + Environment.NewLine + "hit target two";

            string result = Logic.GetSearchString(source, "target", 1, StringComparison.OrdinalIgnoreCase, firstWordOnly: true);

            StringAssert.Contains(result, "hit target one");
            Assert.IsFalse(result.Contains("hit target two"));
        }

        [TestMethod]
        public void firstWordOnlyがfalseなら全てのヒットを取得する()
        {
            string source = "hit target one" + Environment.NewLine + "hit target two";

            string result = Logic.GetSearchString(source, "target", 1, StringComparison.OrdinalIgnoreCase, firstWordOnly: false);

            StringAssert.Contains(result, "hit target one");
            StringAssert.Contains(result, "hit target two");
        }

        [TestMethod]
        public void CmpOptがOrdinalなら大文字小文字を区別する()
        {
            string source = "TARGET line";

            string result = Logic.GetSearchString(source, "target", 1, StringComparison.Ordinal, firstWordOnly: false);

            Assert.IsFalse(result.Contains("TARGET line"));
        }

        [TestMethod]
        public void CmpOptがOrdinalIgnoreCaseなら大文字小文字を区別しない()
        {
            string source = "TARGET line";

            string result = Logic.GetSearchString(source, "target", 1, StringComparison.OrdinalIgnoreCase, firstWordOnly: false);

            StringAssert.Contains(result, "TARGET line");
        }

        [TestMethod]
        public void ヒットしなければ空行のみ返す()
        {
            string source = "nothing here";

            string result = Logic.GetSearchString(source, "target", 1, StringComparison.OrdinalIgnoreCase, firstWordOnly: false);

            Assert.AreEqual(Environment.NewLine, result);
        }
    }
}
