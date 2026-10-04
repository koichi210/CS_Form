using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChotChotChat
{
    class Client
    {
        private const int _port = 8080;
        private const int _timeoutMsec = 100000;
        private TcpClient _tcp;

        public bool IsConnected { get; private set; }

        public String Connect(String hostName)
        {
            if (IsConnected)
            {
                MessageBox.Show("すでにサーバーと接続済みです");
                return "";
            }

            //TcpClientを作成し、サーバーと接続する
            _tcp = new TcpClient(hostName, _port);
            IsConnected = true;

            IPEndPoint remote = (IPEndPoint)_tcp.Client.RemoteEndPoint;
            IPEndPoint local = (IPEndPoint)_tcp.Client.LocalEndPoint;
            return String.Format("サーバー({0}:{1})と接続しました({2}:{3})。",
                remote.Address, remote.Port, local.Address, local.Port);
        }

        public String Disconnect()
        {
            if (IsConnected)
            {
                IsConnected = false;
                _tcp.Close();
            }
            return "サーバーと切断しました。";
        }

        public void Send(String sendText)
        {
            //TODO:Try～CatchでサーバーDownを回避
            //NetworkStreamを取得
            using (NetworkStream ns = _tcp.GetStream())
            {
                // タイムアウト
                ns.ReadTimeout = _timeoutMsec;
                ns.WriteTimeout = _timeoutMsec;

                //データ送信
                byte[] sendBytes = Encoding.UTF8.GetBytes(sendText + Environment.NewLine);
                ns.Write(sendBytes, 0, sendBytes.Length);

                //サーバーから送られたデータ(応答)を受信する。中身は使わない
                if (NetworkStreamReader.ReceiveLine(ns) == null)
                {
                    MessageBox.Show("Disconnect Server",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }
    }
}
