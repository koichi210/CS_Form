using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChotChotChat.Tests
{
    /// <summary>
    /// Server(1件だけメッセージを受け取るTCPサーバー)のテスト。
    /// Server は画面(MessageBox・ログ欄)に触らず結果を戻り値で返すので、画面なしで確かめられる。
    /// 8080番は本物のアプリや他の作業で使われていることがあるので、テストでは毎回空いているポートを使い、
    /// 相手側は素のTcpClientで用意する(このPC内の127.0.0.1だけで完結する)。
    /// </summary>
    [TestClass]
    public class ServerTests
    {
        private const int _waitMsec = 5000;

        private Server _server;
        private int _port;

        [TestInitialize]
        public void Setup()
        {
            _port = FindFreePort();
            _server = new Server(_port);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server.Disconnect();
        }

        [TestMethod]
        public void 待ち受けを始めるとすぐ戻り接続待ちになる()
        {
            String message = _server.Listen("127.0.0.1");

            Assert.AreEqual("127.0.0.1:" + _port + "で接続を待っています…", message);
            Assert.IsTrue(_server.IsListening);
        }

        [TestMethod]
        public void 受け取ったメッセージを返し受信文字数を応答する()
        {
            _server.Listen("127.0.0.1");
            Task<String> accept = Task.Run(() => _server.Accept());

            using (TcpClient client = new TcpClient())
            {
                client.Connect(IPAddress.Loopback, _port);
                Assert.IsTrue(accept.Wait(_waitMsec), "接続を受け入れない");
                StringAssert.StartsWith(accept.Result, "クライアント(127.0.0.1:");

                Task<String> receive = Task.Run(() => _server.Receive());
                NetworkStream ns = client.GetStream();
                byte[] sendBytes = Encoding.UTF8.GetBytes("こんにちは\n");
                ns.Write(sendBytes, 0, sendBytes.Length);

                Assert.IsTrue(receive.Wait(_waitMsec), "受信が終わらない");
                Assert.AreEqual("こんにちは\n", receive.Result);

                using (StreamReader reader = new StreamReader(ns, Encoding.UTF8))
                {
                    Assert.AreEqual("6", reader.ReadLine(), "受信した文字数(改行込み)を応答する");
                }
            }
        }

        [TestMethod]
        public void 何も送らずに切断されたらnull()
        {
            _server.Listen("127.0.0.1");
            Task<String> accept = Task.Run(() => _server.Accept());

            using (TcpClient client = new TcpClient())
            {
                client.Connect(IPAddress.Loopback, _port);
                Assert.IsTrue(accept.Wait(_waitMsec), "接続を受け入れない");
            }

            Assert.IsNull(_server.Receive());
        }

        [TestMethod]
        public void 接続待ちの途中で切断すると待ちが例外で抜けてポートを解放する()
        {
            // 画面を閉じた時(OnFormClosed)に、ワーカースレッドの接続待ちを止められること
            _server.Listen("127.0.0.1");
            Task<String> accept = Task.Run(() => _server.Accept());
            // Acceptが接続待ちに入る前にDisconnectすると「待ち受けを開始していません」になってしまうので少し待つ
            Thread.Sleep(100);

            _server.Disconnect();

            try
            {
                accept.Wait(_waitMsec);
                Assert.Fail("接続待ちが抜けない、または例外にならない");
            }
            catch (AggregateException ex)
            {
                Exception inner = ex.InnerException;
                Assert.IsTrue(inner is SocketException || inner is ObjectDisposedException, inner.GetType().Name);
            }

            Assert.IsFalse(_server.IsListening);
            Assert.IsTrue(CanListen(_port), "ポートが解放されていない");
        }

        [TestMethod]
        public void 待ち受け中にもう一度待ち受けると例外()
        {
            _server.Listen("127.0.0.1");

            Assert.ThrowsException<InvalidOperationException>(() => _server.Listen("127.0.0.1"));
        }

        // いま空いているポート番号(一度借りてすぐ返す)
        private static int FindFreePort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }

        // そのポートで待ち受けできる(=誰も使っていない)か
        private static bool CanListen(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);
            try
            {
                listener.Start();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
