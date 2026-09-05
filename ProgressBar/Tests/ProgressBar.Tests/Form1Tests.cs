using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProgressBar.Tests
{
    /// <summary>
    /// Form1（各種スレッド方式でのプログレスバー更新サンプル）のテスト。
    ///
    /// ⚠️ StartMultiThreadTask / StartMultiThreadTaskInBkgWork は、別スレッドから
    /// Control.Invoke でUIスレッドへマーシャリングする実装になっている。
    /// Control.Invoke はメッセージポンプ(Application.Run)が無いと永久にブロックし、
    /// テストをハングさせてしまうため、これらとそのStopハンドラはテスト対象から
    /// 除外する。StartMultiThreadBkgWorkはBackgroundWorkerの非同期実行に依存し
    /// タイミングが不安定なため同様に除外する。
    /// StartSingleThread/StopSingleThreadは呼び出し元スレッドをブロックするだけで
    /// Invokeを使わないため、安全にテストできる。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void 単一スレッド版はStop要求で途中終了する()
        {
            using (var form = new Form1())
            {
                var thread = new Thread(() =>
                {
                    FormReflection.InvokeMethod(form, "StartSingleThread");
                });
                thread.Start();

                // 少し進行させてから停止要求を出す
                Thread.Sleep(120);
                FormReflection.InvokeMethod(form, "StopSingleThread");

                bool finished = thread.Join(3000);
                Assert.IsTrue(finished, "StartSingleThreadがタイムアウト内に終了しなかった");

                var progressBar = (System.Windows.Forms.ProgressBar)FormReflection.GetControl(form, "progressBar_SingleThread");
                Assert.IsTrue(progressBar.Value < progressBar.Maximum, "途中で停止せず最後まで進んでしまった");
            }
        }
    }
}
