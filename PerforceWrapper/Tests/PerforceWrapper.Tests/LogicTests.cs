using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PerforceWrapper.Tests
{
    /// <summary>
    /// Logic（Form1.cs から切り出した、UI状態をドメイン値に変換するだけの判定ロジック）
    /// のテスト。抽出前と挙動が変わっていないことを、抽出後のコードに対して確認する。
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        // ------------------------------------------------------------------
        // GetOperatorType
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetOperatorType_チェックアウトが選択されているとEDIT()
        {
            var result = Logic.GetOperatorType(checkoutChecked: true, restoreChecked: false, deleteChecked: false, getLatestChecked: false);

            Assert.AreEqual(Perforce.OPERATOR_TYPE.EDIT, result);
        }

        [TestMethod]
        public void GetOperatorType_復元が選択されているとREVENT()
        {
            var result = Logic.GetOperatorType(checkoutChecked: false, restoreChecked: true, deleteChecked: false, getLatestChecked: false);

            Assert.AreEqual(Perforce.OPERATOR_TYPE.REVENT, result);
        }

        [TestMethod]
        public void GetOperatorType_削除が選択されているとDELETE()
        {
            var result = Logic.GetOperatorType(checkoutChecked: false, restoreChecked: false, deleteChecked: true, getLatestChecked: false);

            Assert.AreEqual(Perforce.OPERATOR_TYPE.DELETE, result);
        }

        [TestMethod]
        public void GetOperatorType_最新取得が選択されているとSYNC()
        {
            var result = Logic.GetOperatorType(checkoutChecked: false, restoreChecked: false, deleteChecked: false, getLatestChecked: true);

            Assert.AreEqual(Perforce.OPERATOR_TYPE.SYNC, result);
        }

        [TestMethod]
        public void GetOperatorType_何も選択されていなければ既定値のSYNC()
        {
            var result = Logic.GetOperatorType(checkoutChecked: false, restoreChecked: false, deleteChecked: false, getLatestChecked: false);

            Assert.AreEqual(Perforce.OPERATOR_TYPE.SYNC, result);
        }

        [TestMethod]
        public void GetOperatorType_複数選択されていても判定順の先頭が優先される()
        {
            // 実装は if / else if の連鎖なので、チェックアウトが最優先になる
            // （実際のラジオボタンは単一選択だが、判定順の仕様として記録しておく）。
            var result = Logic.GetOperatorType(checkoutChecked: true, restoreChecked: true, deleteChecked: true, getLatestChecked: true);

            Assert.AreEqual(Perforce.OPERATOR_TYPE.EDIT, result);
        }

        // ------------------------------------------------------------------
        // GetCurrentTabId
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetCurrentTabId_0番目はBASE_OPERATION()
        {
            Assert.AreEqual(Logic.TAB_ID.BASE_OPERATION, Logic.GetCurrentTabId(0));
        }

        [TestMethod]
        public void GetCurrentTabId_1番目はSET_LABEL()
        {
            Assert.AreEqual(Logic.TAB_ID.SET_LABEL, Logic.GetCurrentTabId(1));
        }

        [TestMethod]
        public void GetCurrentTabId_2番目はDIFF_LABEL()
        {
            Assert.AreEqual(Logic.TAB_ID.DIFF_LABEL, Logic.GetCurrentTabId(2));
        }

        [TestMethod]
        public void GetCurrentTabId_3番目はAPPLY_LABEL()
        {
            Assert.AreEqual(Logic.TAB_ID.APPLY_LABEL, Logic.GetCurrentTabId(3));
        }

        [TestMethod]
        public void GetCurrentTabId_範囲外の値は既定値のBASE_OPERATION()
        {
            Assert.AreEqual(Logic.TAB_ID.BASE_OPERATION, Logic.GetCurrentTabId(99));
            Assert.AreEqual(Logic.TAB_ID.BASE_OPERATION, Logic.GetCurrentTabId(-1));
        }
    }
}
