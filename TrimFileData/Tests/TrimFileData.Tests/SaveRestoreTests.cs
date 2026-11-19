using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrimFileData.Tests
{
    /// <summary>
    /// SaveRestore（プロファイルの保存対象の登録）のテスト。
    /// </summary>
    [TestClass]
    public class SaveRestoreTests
    {
        [TestMethod]
        public void 全角半角を区別しないのチェックが保存して読み直すと戻る()
        {
            string path = Path.Combine(Path.GetTempPath(), "TrimFileDataSaveRestoreTests_" + Guid.NewGuid().ToString("N") + ".xml");
            try
            {
                using (Form1 writer = new Form1())
                {
                    SaveRestore writerSr = new SaveRestore();
                    writerSr.RegisterItem(writer);
                    writer.checkBox_IgnoreWidth.Checked = true;
                    Assert.IsTrue(writerSr.SaveXmlFile(path));
                    Assert.IsTrue(writerSr.BuildGenericProfile().Values.ContainsKey("Common|checkBox_IgnoreWidth"));
                }

                using (Form1 reader = new Form1())
                {
                    SaveRestore readerSr = new SaveRestore();
                    readerSr.RegisterItem(reader);
                    reader.checkBox_IgnoreWidth.Checked = false;
                    Assert.IsTrue(readerSr.LoadXmlFile(path));
                    Assert.IsTrue(reader.checkBox_IgnoreWidth.Checked);
                }
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
