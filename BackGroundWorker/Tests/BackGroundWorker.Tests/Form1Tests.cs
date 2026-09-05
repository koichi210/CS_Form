using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackGroundWorker.Tests
{
    /// <summary>
    /// Form1（BackgroundWorkerの使い方を示すサンプル）のテスト。
    ///
    /// ⚠️ buttonStart_Click は実際に別スレッドで非同期実行(RunWorkerAsync)を
    /// 開始するため、タイミング依存でテストが不安定になる。bgWorker_RunWorkerCompleted_1
    /// はキャンセル時・正常完了時のどちらでも必ずMessageBox.Showを呼ぶ。
    /// この2つはテスト対象から除外し、bgWorker_DoWork_1(実処理本体)と
    /// bgWorker_ProgressChanged_1(進捗表示)を直接呼び出して検証する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void DoWorkは指定回数繰り返して完了結果を返す()
        {
            using (var form = new Form1())
            using (var worker = new BackgroundWorker())
            {
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                var arguments = new List<object> { 2 }; // 2回だけ繰り返す(高速化のため小さい値)
                var e = new DoWorkEventArgs(arguments);

                FormReflection.InvokeHandler(form, "bgWorker_DoWork_1", worker, e);

                Assert.IsFalse(e.Cancel);
                Assert.AreEqual("すべて完了", e.Result);
            }
        }

        [TestMethod]
        public void DoWorkはキャンセル済みならCancelをtrueにして早期終了する()
        {
            using (var form = new Form1())
            using (var worker = new BackgroundWorker())
            {
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.CancelAsync(); // CancellationPendingをtrueにする

                var arguments = new List<object> { 100 };
                var e = new DoWorkEventArgs(arguments);

                FormReflection.InvokeHandler(form, "bgWorker_DoWork_1", worker, e);

                Assert.IsTrue(e.Cancel);
                Assert.IsNull(e.Result);
            }
        }

        [TestMethod]
        public void ProgressChangedで進捗率がタイトルとプログレスバーに反映される()
        {
            using (var form = new Form1())
            {
                var e = new ProgressChangedEventArgs(50, null);

                FormReflection.InvokeHandler(form, "bgWorker_ProgressChanged_1", form, e);

                StringAssert.Contains(form.Text, "50％完了");
                ProgressBar progressBar = (ProgressBar)FormReflection.GetControl(form, "progressBar");
                Assert.AreEqual(50, progressBar.Value);
            }
        }
    }
}
