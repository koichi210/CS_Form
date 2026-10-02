using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace EventRecorder.Tests
{
    /// <summary>
    /// SessionGuard(画面ロック・サインイン・シャットダウン時の扱い)のテスト。
    /// 実際にロックやシャットダウンはできないので、OSの通知を受けた後の「判定」と
    /// 「再生スレッドの停止待ち」だけを確かめる(フック・SendInputには触らない)
    /// </summary>
    [TestClass]
    public class SessionGuardTests
    {
        [TestMethod]
        public void 画面ロックやユーザー切替やリモート切断は記録再生を止める側に振り分ける()
        {
            Assert.AreEqual(SessionChange.Suspend, SessionGuard.Classify(SessionSwitchReason.SessionLock));
            Assert.AreEqual(SessionChange.Suspend, SessionGuard.Classify(SessionSwitchReason.SessionLogoff));
            Assert.AreEqual(SessionChange.Suspend, SessionGuard.Classify(SessionSwitchReason.ConsoleDisconnect));
            Assert.AreEqual(SessionChange.Suspend, SessionGuard.Classify(SessionSwitchReason.RemoteDisconnect));
        }

        [TestMethod]
        public void ロック解除やサインインは立て直す側に振り分ける()
        {
            Assert.AreEqual(SessionChange.Resume, SessionGuard.Classify(SessionSwitchReason.SessionUnlock));
            Assert.AreEqual(SessionChange.Resume, SessionGuard.Classify(SessionSwitchReason.SessionLogon));
            Assert.AreEqual(SessionChange.Resume, SessionGuard.Classify(SessionSwitchReason.ConsoleConnect));
            Assert.AreEqual(SessionChange.Resume, SessionGuard.Classify(SessionSwitchReason.RemoteConnect));
        }

        [TestMethod]
        public void リモート操作の切替は何もしない()
        {
            Assert.AreEqual(SessionChange.None, SessionGuard.Classify(SessionSwitchReason.SessionRemoteControl));
        }

        [TestMethod]
        public void 既に終わっていればpumpを呼ばずにすぐtrueを返す()
        {
            int pumpCount = 0;
            Boolean result = SessionGuard.WaitWhilePumping(() => true, () => pumpCount++, 1000);

            Assert.IsTrue(result);
            Assert.AreEqual(0, pumpCount);
        }

        [TestMethod]
        public void 終わらなければ時間切れでfalseを返す()
        {
            Boolean result = SessionGuard.WaitWhilePumping(() => false, () => { }, 100);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// 実際の終了処理で起きる形の再現: 再生スレッドは停止要求を受けた後、後片付けとして
        /// UIスレッドへの処理依頼(Invoke相当)が通るのを待ってから終わる。
        /// 待つ側がpump(メッセージ処理)を回さずに待つとお互い待ち合ってデッドロックするので、
        /// pumpの中で依頼を処理しながら待てば、時間切れにならずに終わることを確かめる
        /// </summary>
        [TestMethod]
        public void UIスレッドへの依頼を待つ再生スレッドもpumpを回しながら待てば止まる()
        {
            ManualResetEventSlim requestPosted = new ManualResetEventSlim(false);
            ManualResetEventSlim requestDone = new ManualResetEventSlim(false);

            Task worker = Task.Run(() =>
            {
                requestPosted.Set();
                requestDone.Wait();
            });

            Boolean result = SessionGuard.WaitWhilePumping(
                () => worker.IsCompleted,
                () =>
                {
                    if (requestPosted.IsSet)
                    {
                        requestDone.Set();
                    }
                },
                5000);

            Assert.IsTrue(result);
            Assert.IsTrue(worker.IsCompleted);
        }
    }
}
