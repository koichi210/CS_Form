using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ChotChotChat
{
    /// <summary>
    /// サーバーへ1件だけメッセージを送るTCPクライアント。
    ///
    /// 以前は中で MessageBox を出していて、サーバーが起動していない・途中で落ちた時は例外がそのまま上がって
    /// アプリごと落ちていた。WPF版(CS_Wpf/ChotChotChat)・Serverと同じく、UIには触らず結果を戻り値・例外で返し、
    /// 呼び出し側(Form1)が通信エラーを受け止めてメッセージを出す形にした。
    /// </summary>
    internal sealed class Client
    {
        private const int _timeoutMsec = 100000;

        private readonly int _port;
        private TcpClient _tcp;

        public Client()
            : this(Server.DefaultPort)
        {
        }

        // テストでは8080以外(空いているポート)を使うため
        internal Client(int port)
        {
            _port = port;
        }

        public bool IsConnected
        {
            get { return _tcp != null; }
        }

        /// <summary>
        /// サーバーと接続する。サーバーが待っていなければ SocketException(接続していない状態のまま)。
        /// </summary>
        public String Connect(String hostName)
        {
            if (IsConnected)
            {
                throw new InvalidOperationException("すでにサーバーと接続済みです");
            }

            // TcpClientを作成し、サーバーと接続する(失敗したら _tcp は null のまま)
            TcpClient tcp = new TcpClient(hostName, _port);
            _tcp = tcp;

            IPEndPoint remote = (IPEndPoint)tcp.Client.RemoteEndPoint;
            IPEndPoint local = (IPEndPoint)tcp.Client.LocalEndPoint;
            return String.Format("サーバー({0}:{1})と接続しました({2}:{3})。",
                remote.Address, remote.Port, local.Address, local.Port);
        }

        public String Disconnect()
        {
            if (_tcp != null)
            {
                _tcp.Close();
                _tcp = null;
            }
            return "サーバーと切断しました。";
        }

        /// <summary>
        /// メッセージを送信し、サーバーの応答(受信した文字数)を受け取って返す。
        /// 応答の前にサーバーが切断したら null。
        /// サーバーが落ちた・接続が切れた等の通信エラーは IOException / SocketException / ObjectDisposedException。
        /// </summary>
        public String Send(String sendText)
        {
            if (_tcp == null)
            {
                throw new InvalidOperationException("先にサーバーと接続してください");
            }

            //NetworkStreamを取得(相手に切られてソケットが閉じていると InvalidOperationException になるので、通信エラーとして返す)
            NetworkStream ns;
            try
            {
                ns = _tcp.GetStream();
            }
            catch (InvalidOperationException ex)
            {
                throw new IOException("サーバーとの接続が切れています", ex);
            }

            using (ns)
            {
                // タイムアウト
                ns.ReadTimeout = _timeoutMsec;
                ns.WriteTimeout = _timeoutMsec;

                //データ送信(改行を含むメッセージでも1件と分かるよう、バイト数を付けて送る)
                byte[] sendBytes = Encoding.UTF8.GetBytes(sendText + Environment.NewLine);
                NetworkStreamReader.WriteMessage(ns, sendBytes);

                //サーバーから送られたデータ(応答)を受信する
                byte[] reply = NetworkStreamReader.ReceiveLine(ns);
                return reply == null ? null : Encoding.UTF8.GetString(reply);
            }
        }
    }
}
