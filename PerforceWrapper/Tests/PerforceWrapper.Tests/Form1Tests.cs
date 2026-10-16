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
    /// 以前このバッチファイルは、p4 login 用に平文のパスワードファイルを読む "cat <path> | p4 login" を
    /// 含んでいて、実行後の File.Delete(batchFile) がコメントアウトされていたため、パスワード絡みの
    /// 一時ファイルがディスクに残り続けるバグがあった。今はログインをバッチの外で行うのでパスワードは
    /// 含まないが、一時ファイルを残さないことは引き続き確認する
    /// （ログインなしの無害なバッチにして、実際の p4 サーバーには依存しない）。
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
                    var args = new DoWorkEventArgs(new Form1.ExecuteRequest(batchFile, null));
                    MethodInfo method = typeof(Form1).GetMethod("backgroundWorker_DoWork", BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(form, new object[] { form, args });

                    Assert.IsFalse(File.Exists(batchFile), "一時バッチファイルは実行後に削除されるはず");
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
