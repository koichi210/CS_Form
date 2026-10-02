using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PerforceWrapper.Tests
{
    /// <summary>
    /// Form1.backgroundWorker_DoWork（p4コマンドのバッチファイルを実行する箇所）のテスト。
    ///
    /// このバッチファイルは、ユーザーがパスワードを入力した場合に p4 login 用のパスワード
    /// ファイルを読む "cat <path> | p4 login" を含むことがある。以前は実行後の
    /// File.Delete(batchFile) がコメントアウトされていたため、パスワード絡みの一時ファイルが
    /// ディスクに残り続けるバグがあった。ここでは「実行後に必ず削除される」ことだけを確認する
    /// （バッチの中身自体はパスワードを含まない無害なものにして、実際の p4 サーバーには依存しない）。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void バッチ実行後は一時バッチファイルが削除される()
        {
            using (Form1 form = new Form1())
            {
                String batchFile = Path.Combine(Path.GetTempPath(), "PerforceWrapperTest_" + Guid.NewGuid().ToString("N") + ".bat");
                File.WriteAllText(batchFile, "echo test" + Environment.NewLine);

                try
                {
                    var args = new DoWorkEventArgs(batchFile);
                    MethodInfo method = typeof(Form1).GetMethod("backgroundWorker_DoWork", BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(form, new object[] { form, args });

                    Assert.IsFalse(File.Exists(batchFile), "パスワードが含まれうる一時バッチファイルは実行後に削除されるはず");
                }
                finally
                {
                    // テスト失敗時に備えた後片付け(フィックス後は既に削除済みのはず)
                    if (File.Exists(batchFile)) File.Delete(batchFile);
                }
            }
        }
    }
}
