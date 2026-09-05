using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileArranger.Tests
{
    /// <summary>
    /// FileArranger.Utils（StcUtils を継承した独自ユーティリティ、internal）のテスト。
    ///
    /// Form1.cs（1,284行）はロジックがイベントハンドラに埋め込まれていて、テストするには
    /// private メソッドの切り出し（本体コードの書き換え）が要る。今回はそこまで踏み込まず、
    /// もともと独立したクラスとして分離されている Utils.cs だけを対象にする。
    /// Form1.cs は一切変更していない。
    /// </summary>
    [TestClass]
    public class UtilsTests
    {
        private string tempDirectory;
        private Utils util;

        [TestInitialize]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "FileArrangerUtilsTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            util = new Utils();
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                if (Directory.Exists(tempDirectory)) Directory.Delete(tempDirectory, true);
            }
            catch (IOException)
            {
                // 後片付けの失敗はテストの成否に関係ないので黙って流す
            }
        }

        // ------------------------------------------------------------------
        // CreateFolderNameOverLapShirk
        // ------------------------------------------------------------------

        [TestMethod]
        public void CreateFolderNameOverLapShirk_フォルダが存在しなければ変更しない()
        {
            string path = Path.Combine(tempDirectory, "not_exist");

            util.CreateFolderNameOverLapShirk(ref path, 1);

            Assert.AreEqual(Path.Combine(tempDirectory, "not_exist"), path);
        }

        [TestMethod]
        public void CreateFolderNameOverLapShirk_フォルダが存在すれば連番付きの名前にする()
        {
            string original = Path.Combine(tempDirectory, "exists");
            Directory.CreateDirectory(original);
            string path = original;

            util.CreateFolderNameOverLapShirk(ref path, 3);

            Assert.AreNotEqual(original, path);
            StringAssert.StartsWith(path, original + "_Cnt3_");
        }

        [TestMethod]
        public void CreateFolderNameOverLapShirk_LoopIdxが0でも連番0として埋め込む()
        {
            string original = Path.Combine(tempDirectory, "exists_zero");
            Directory.CreateDirectory(original);
            string path = original;

            util.CreateFolderNameOverLapShirk(ref path, 0);

            StringAssert.StartsWith(path, original + "_Cnt0_");
        }

        // ------------------------------------------------------------------
        // CreateFileNameOverLapShirk
        // ------------------------------------------------------------------

        [TestMethod]
        public void CreateFileNameOverLapShirk_何も無ければtrueを返し名前も変えない()
        {
            string path = Path.Combine(tempDirectory, "new.txt");

            bool result = util.CreateFileNameOverLapShirk(ref path, 1);

            Assert.IsTrue(result);
            Assert.AreEqual(Path.Combine(tempDirectory, "new.txt"), path);
        }

        [TestMethod]
        public void CreateFileNameOverLapShirk_ファイルが存在すればfalseを返し連番を付ける()
        {
            string original = Path.Combine(tempDirectory, "dup.txt");
            File.WriteAllText(original, "dummy");
            string path = original;

            bool result = util.CreateFileNameOverLapShirk(ref path, 2);

            Assert.IsFalse(result);
            Assert.AreNotEqual(original, path);
            StringAssert.StartsWith(path, original + "_Cnt2_");
        }

        [TestMethod]
        public void CreateFileNameOverLapShirk_LoopIdxが0でも連番0として埋め込む()
        {
            string original = Path.Combine(tempDirectory, "dup_zero.txt");
            File.WriteAllText(original, "dummy");
            string path = original;

            util.CreateFileNameOverLapShirk(ref path, 0);

            StringAssert.StartsWith(path, original + "_Cnt0_");
        }

        [TestMethod]
        public void CreateFileNameOverLapShirk_同名のフォルダがあってもfalseを返す()
        {
            // ファイルではなくフォルダとの重複も検知する
            string original = Path.Combine(tempDirectory, "dup_dir");
            Directory.CreateDirectory(original);
            string path = original;

            bool result = util.CreateFileNameOverLapShirk(ref path, 1);

            Assert.IsFalse(result);
        }

        // ------------------------------------------------------------------
        // CreateNewFolderName
        // ------------------------------------------------------------------

        [TestMethod]
        public void CreateNewFolderName_区切り文字未指定なら元の名前のまま()
        {
            string result = util.CreateNewFolderName("abc_def_ghi");

            Assert.AreEqual("abc_def_ghi", result);
        }

        [TestMethod]
        public void CreateNewFolderName_最初に見つかった区切りより前を切り出す()
        {
            string result = util.CreateNewFolderName("abc_def_ghi", "_");

            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void CreateNewFolderName_Reverse指定で最後に見つかった区切りより前を切り出す()
        {
            string result = util.CreateNewFolderName("abc_def_ghi", "_", true);

            Assert.AreEqual("abc_def", result);
        }

        [TestMethod]
        public void CreateNewFolderName_区切り文字が見つからなければ元の名前のまま()
        {
            string result = util.CreateNewFolderName("abcdefghi", "_");

            Assert.AreEqual("abcdefghi", result);
        }

        [TestMethod]
        public void CreateNewFolderName_区切り文字が先頭にあれば空文字になる()
        {
            string result = util.CreateNewFolderName("_abc", "_");

            Assert.AreEqual("", result);
        }

        // ------------------------------------------------------------------
        // GetStringFromListViewInSelect
        // ------------------------------------------------------------------

        private static ListView NewListViewWithItems(params string[] texts)
        {
            var lv = new ListView();
            lv.Columns.Add("col0");
            foreach (string t in texts)
            {
                lv.Items.Add(new ListViewItem(t));
            }

            // ウィンドウハンドルが未生成のうちは Item.Selected への代入が SelectedItems に
            // 反映されない（WinForms 特有の挙動）。Handle を触ってハンドル生成を強制する。
            IntPtr forceHandleCreation = lv.Handle;

            return lv;
        }

        [TestMethod]
        public void GetStringFromListViewInSelect_選択項目の中から部分一致するものを探す()
        {
            using (ListView lv = NewListViewWithItems("apple_1.txt", "banana_2.txt", "cherry_3.txt"))
            {
                lv.Items[0].Selected = true;
                lv.Items[2].Selected = true;

                int idx = util.GetStringFromListViewInSelect(lv, 0, "cherry");

                Assert.AreEqual(2, idx);
            }
        }

        [TestMethod]
        public void GetStringFromListViewInSelect_選択されていない項目はヒットしない()
        {
            using (ListView lv = NewListViewWithItems("apple_1.txt", "banana_2.txt"))
            {
                lv.Items[0].Selected = true;
                // banana は選択していない

                int idx = util.GetStringFromListViewInSelect(lv, 0, "banana");

                Assert.AreEqual(-1, idx);
            }
        }

        [TestMethod]
        public void GetStringFromListViewInSelect_見つからなければマイナス1()
        {
            using (ListView lv = NewListViewWithItems("apple_1.txt"))
            {
                lv.Items[0].Selected = true;

                int idx = util.GetStringFromListViewInSelect(lv, 0, "not_found");

                Assert.AreEqual(-1, idx);
            }
        }

        [TestMethod]
        public void GetStringFromListViewInSelect_TrimNameで区切ってから比較する()
        {
            // 検索対象の文字列側を区切ってから、リストの項目に部分一致するか見る
            using (ListView lv = NewListViewWithItems("report", "summary"))
            {
                lv.Items[0].Selected = true;
                lv.Items[1].Selected = true;

                int idx = util.GetStringFromListViewInSelect(lv, 0, "report_v2.txt", "_");

                Assert.AreEqual(0, idx);
            }
        }
    }
}
