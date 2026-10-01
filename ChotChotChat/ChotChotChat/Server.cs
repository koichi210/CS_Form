using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChotChotChat
{
    class Server
    {
        private readonly int port = 8080;
        private readonly int TimeOutMsec = 100000;
        private bool isConnected = false;
        private TcpListener listener;
        private TcpClient client;

        public String Connect(String listenAddress)
        {
            if (isConnected)
            {
                MessageBox.Show("すでにクライアントと接続済みです");
                return "";
            }

            //ListenするIPアドレス
            IPAddress ipAddress = IPAddress.Parse(listenAddress);

            //ホスト名からIPアドレスに変換
            //String host = "localhost";
            //IPAddress ipAddress = Dns.GetHostEntry(host).AddressList[0];

            listener = new TcpListener(ipAddress, port);

            //Listenを開始
            listener.Start();

            //接続要求があったら受け入れる
            client = listener.AcceptTcpClient();
            isConnected = true;

            return  String.Format("クライアント({0}:{1})と接続しました。",
                ((IPEndPoint)client.Client.RemoteEndPoint).Address,
                ((IPEndPoint)client.Client.RemoteEndPoint).Port);
        }

        public String Disconnect()
        {
            // 接続待ちの途中で失敗した場合もポートを解放するため、isConnectedに関係なく閉じる
            isConnected = false;
            if (client != null)
            {
                client.Close();
                client = null;
            }
            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }
            return "クライアントとの接続を閉じました。";
        }

        public void Receive(Form1 parent)
        {
            // データ受信
            Encoding enc = Encoding.UTF8;

            //NetworkStreamを取得
            NetworkStream ns = client.GetStream();

            // タイムアウト
            ns.ReadTimeout = TimeOutMsec;
            ns.WriteTimeout = TimeOutMsec;

            //while (true)
            {
                //クライアントから送られたデータ受信
                MemoryStream ms = new MemoryStream();
                byte[] resBytes = new byte[256];
                int resSize = 0;

                do
                {
                    //データの一部を受信
                    resSize = ns.Read(resBytes, 0, resBytes.Length);
                    if (resSize == 0)
                    {
                        isConnected = false;
                        MessageBox.Show("Disconnect Client",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        // 暫定
                        ms.Close();
                        ns.Close();
                        return;
                    }

                    //受信したデータを蓄積し、データの最後が\nでない時は受信を続ける
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

                //受信したデータを文字列に変換
                String resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                ms.Close();

                // ログ画面更新
                parent.textBox_Log.Text += resMsg;

                if (isConnected)
                {
                    //クライアントにデータ送信
                    String sendMsg = resMsg.Length.ToString();
                    byte[] sendBytes = enc.GetBytes(sendMsg + '\n');

                    //データ送信
                    ns.Write(sendBytes, 0, sendBytes.Length);
                }
            }

            //閉じる
            ns.Close();
        }
    }
}
