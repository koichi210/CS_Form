using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChotChotChat
{
    class Server
    {
        private const int _port = 8080;
        private const int _timeoutMsec = 100000;
        private bool _isConnected = false;
        private TcpListener _listener;
        private TcpClient _client;

        public String Connect(String listenAddress)
        {
            if (_isConnected)
            {
                MessageBox.Show("すでにクライアントと接続済みです");
                return "";
            }

            //ListenするIPアドレス
            IPAddress ipAddress = IPAddress.Parse(listenAddress);

            //ホスト名からIPアドレスに変換
            //String host = "localhost";
            //IPAddress ipAddress = Dns.GetHostEntry(host).AddressList[0];

            _listener = new TcpListener(ipAddress, _port);

            //Listenを開始
            _listener.Start();

            //接続要求があったら受け入れる
            _client = _listener.AcceptTcpClient();
            _isConnected = true;

            IPEndPoint remote = (IPEndPoint)_client.Client.RemoteEndPoint;
            return String.Format("クライアント({0}:{1})と接続しました。", remote.Address, remote.Port);
        }

        public String Disconnect()
        {
            // 接続待ちの途中で失敗した場合もポートを解放するため、_isConnectedに関係なく閉じる
            _isConnected = false;
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
            if (_listener != null)
            {
                _listener.Stop();
                _listener = null;
            }
            return "クライアントとの接続を閉じました。";
        }

        public void Receive(Form1 parent)
        {
            Encoding enc = Encoding.UTF8;

            //NetworkStreamを取得
            using (NetworkStream ns = _client.GetStream())
            {
                // タイムアウト
                ns.ReadTimeout = _timeoutMsec;
                ns.WriteTimeout = _timeoutMsec;

                //クライアントから送られたデータ受信
                byte[] received = NetworkStreamReader.ReceiveLine(ns);
                if (received == null)
                {
                    _isConnected = false;
                    MessageBox.Show("Disconnect Client",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                //受信したデータを文字列に変換
                String resMsg = enc.GetString(received);

                // ログ画面更新
                parent.textBox_Log.Text += resMsg;

                if (_isConnected)
                {
                    //クライアントにデータ送信(受信した文字数を返す)
                    byte[] sendBytes = enc.GetBytes(resMsg.Length.ToString() + '\n');
                    ns.Write(sendBytes, 0, sendBytes.Length);
                }
            }
        }
    }
}
