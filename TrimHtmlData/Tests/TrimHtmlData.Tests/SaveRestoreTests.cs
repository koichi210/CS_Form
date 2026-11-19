using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrimHtmlData.Tests
{
    /// <summary>
    /// SaveRestore（プロファイルの保存対象の登録）のテスト。
    /// </summary>
    [TestClass]
    public class SaveRestoreTests
    {
        // 取得元・取得値は毎回入れ替える「データ」なので、プロファイルの保存対象に入れない
        [TestMethod]
        public void 取得元と取得値はプロファイルの保存対象に含めない()
        {
            using (Form1 form = new Form1())
            {
                SaveRestore sr = new SaveRestore();
                sr.RegisterItem(form);

                CollectionAssert.AreEquivalent(new[]
                {
                    "Common|textBox_SearchWord",
                    "Common|textBox_DelimiterWord",
                    "Common|textBox_TrimLineNum",
                    "Common|checkBox_OrdinalCase",
                }, new List<string>(sr.BuildGenericProfile().Values.Keys));
            }
        }
    }
}
