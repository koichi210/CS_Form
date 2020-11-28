using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
        private bool IsConnect = false;
        private TcpListener listener;
        private TcpClient client;

        public String Connect(String ClientName)
        {
            if (IsConnect)
            {
                MessageBox.Show("すでにクライアントと接続済みです");
                return "";
            }

            //ListenするIPアドレス
            IPAddress ipAdd = IPAddress.Parse(ClientName);

            //ホスト名からIPアドレスに変換
            //String host = "localhost";
            //IPAddress ipAdd = Dns.GetHostEntry(host).AddressList[0];

            listener = new TcpListener(ipAdd, port);

            //Listenを開始
            listener.Start();
            //Parent.label_StatusBar.Text = String.Format("Listenを開始しました({0}:{1})。",
            //    ((IPEndPoint)listener.LocalEndpoint).Address,
            //    ((IPEndPoint)listener.LocalEndpoint).Port);

            //接続要求があったら受け入れる
            client = listener.AcceptTcpClient();
            IsConnect = true;

            return  String.Format("クライアント({0}:{1})と接続しました。",
                ((IPEndPoint)client.Client.RemoteEndPoint).Address,
                ((IPEndPoint)client.Client.RemoteEndPoint).Port);
        }

        public String Diconnect()
        {
            if (IsConnect)
            {
                IsConnect = false;
                client.Close();
                listener.Stop();
            }
            return "クライアントとの接続を閉じました。";
        }

        public void Recv(Form1 Parent)
        {
            // データ受信
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

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
                        IsConnect = false;
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
                Parent.textBox_Log.Text += resMsg;

                if (IsConnect)
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

        public String Listen()
        {
            //IPv4とIPv6の全てのIPアドレスをListen
            TcpListener listener = new TcpListener(IPAddress.IPv6Any, 2001);

            //IPv6OnlyをOFF
            listener.Server.SetSocketOption(
                SocketOptionLevel.IPv6,
                SocketOptionName.IPv6Only,
                0);

            //Listenを開始
            listener.Start();

            //接続要求があったら受け入れる
            TcpClient client = listener.AcceptTcpClient();

            return String.Format("IPアドレス:{0} ポート番号:{1})。",
                ((IPEndPoint)client.Client.LocalEndPoint).Address,
                ((IPEndPoint)client.Client.LocalEndPoint).Port);
        }
    }
}
