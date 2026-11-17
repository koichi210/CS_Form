using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChotChotChat.Tests
{
    /// <summary>
    /// Client(サーバーへ1件だけメッセージを送るTCPクライアント)のテスト。
    /// Client は画面に触らず結果を戻り値・例外で返すので、サーバーが落ちている・途中で切れた時に
    /// 「例外で落ちる」のではなく、Form1 が受け止められる種類の例外か null になることを確かめる。
    /// ServerTests と同じく、毎回空いているポートを使い、127.0.0.1 だけで完結させる。
    /// </summary>
    [TestClass]
    public class ClientTests
    {
        private const int _waitMsec = 5000;

        private int _port;
        private Client _client;
        // サーバーの代わりに素のTcpListenerで待つテスト用(.NET Framework の TcpListener は using できないので Cleanup で止める)
        private TcpListener _listener;

        [TestInitialize]
        public void Setup()
        {
            _port = FindFreePort();
            _client = new Client(_port);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Disconnect();
            if (_listener != null)
            {
                _listener.Stop();
            }
        }

        [TestMethod]
        public void サーバーが起動していなければSocketExceptionで未接続のまま()
        {
            Assert.ThrowsException<SocketException>(() => _client.Connect("127.0.0.1"));

            Assert.IsFalse(_client.IsConnected, "接続に失敗したのに接続済みになっている");
        }

        [TestMethod]
        public void 接続に失敗した後でもサーバーが起動すれば接続できる()
        {
            Assert.ThrowsException<SocketException>(() => _client.Connect("127.0.0.1"));

            Server server = new Server(_port);
            try
            {
                server.Listen("127.0.0.1");
                Task<String> accept = Task.Run(() => server.Accept());

                StringAssert.StartsWith(_client.Connect("127.0.0.1"), "サーバー(127.0.0.1:" + _port + ")と接続しました");
                Assert.IsTrue(_client.IsConnected);
                Assert.IsTrue(accept.Wait(_waitMsec), "接続を受け入れない");
            }
            finally
            {
                server.Disconnect();
            }
        }

        [TestMethod]
        public void 送信するとサーバーの応答を返す()
        {
            Server server = new Server(_port);
            try
            {
                server.Listen("127.0.0.1");
                Task<String> accept = Task.Run(() => server.Accept());
                _client.Connect("127.0.0.1");
                Assert.IsTrue(accept.Wait(_waitMsec), "接続を受け入れない");

                Task<String> receive = Task.Run(() => server.Receive());
                String reply = _client.Send("こんにちは");

                Assert.IsTrue(receive.Wait(_waitMsec), "受信が終わらない");
                Assert.AreEqual("こんにちは" + Environment.NewLine, receive.Result);
                // 受信した文字数(改行込み)
                Assert.AreEqual((5 + Environment.NewLine.Length) + "\n", reply);
            }
            finally
            {
                server.Disconnect();
            }
        }

        [TestMethod]
        public void 応答する前にサーバーが切断したらnull()
        {
            TcpListener listener = StartListener();
            {
                Task<TcpClient> accept = listener.AcceptTcpClientAsync();
                _client.Connect("127.0.0.1");
                Assert.IsTrue(accept.Wait(_waitMsec), "接続を受け入れない");

                Task<String> send = Task.Run(() => _client.Send("こんにちは"));
                using (TcpClient server = accept.Result)
                {
                    // メッセージは受け取るが、応答を返さずに切断する
                    NetworkStream ns = server.GetStream();
                    Assert.IsNotNull(NetworkStreamReader.ReceiveMessage(ns));
                }

                Assert.IsTrue(send.Wait(_waitMsec), "送信が終わらない");
                Assert.IsNull(send.Result);
            }
        }

        [TestMethod]
        public void 接続した後にサーバーが落ちていたら通信エラーの例外()
        {
            TcpListener listener = StartListener();
            {
                Task<TcpClient> accept = listener.AcceptTcpClientAsync();
                _client.Connect("127.0.0.1");
                Assert.IsTrue(accept.Wait(_waitMsec), "接続を受け入れない");

                // サーバー側を強制的に切断(RST)してから送る
                using (TcpClient server = accept.Result)
                {
                    server.LingerState = new LingerOption(true, 0);
                }
                listener.Stop();
                System.Threading.Thread.Sleep(100);

                // 送信自体はOS側で受け付けられることがあり、その場合は応答の受信で切断に気づく(null)。
                // どちらの場合も Form1 が受け止められる結果になればよい
                try
                {
                    Assert.IsNull(_client.Send("こんにちは"));
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is IOException || ex is SocketException || ex is ObjectDisposedException,
                        ex.GetType().Name);
                }
            }
        }

        [TestMethod]
        public void 接続せずに送ると例外()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _client.Send("こんにちは"));
        }

        [TestMethod]
        public void 接続済みでもう一度接続すると例外()
        {
            TcpListener listener = StartListener();
            {
                Task<TcpClient> accept = listener.AcceptTcpClientAsync();
                _client.Connect("127.0.0.1");
                Assert.IsTrue(accept.Wait(_waitMsec), "接続を受け入れない");
                accept.Result.Close();

                Assert.ThrowsException<InvalidOperationException>(() => _client.Connect("127.0.0.1"));
            }
        }

        private TcpListener StartListener()
        {
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();
            return _listener;
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
    }
}
