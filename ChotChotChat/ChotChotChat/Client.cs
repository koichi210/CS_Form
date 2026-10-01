using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChotChotChat
{
    class Client
    {
        private readonly int port = 8080;
        private readonly int TimeOutMsec = 100000;
        private bool isConnected = false;
        private TcpClient tcp;

        public bool IsConnected { get { return isConnected; } }

        public String Connect(String hostName)
        {
            if (isConnected)
            {
                MessageBox.Show("すでにサーバーと接続済みです");
                return "";
            }
            
            //TcpClientを作成し、サーバーと接続する
            tcp = new TcpClient(hostName, port);
            isConnected = true;

            return String.Format("サーバー({0}:{1})と接続しました({2}:{3})。",
                ((IPEndPoint)tcp.Client.RemoteEndPoint).Address,
                ((IPEndPoint)tcp.Client.RemoteEndPoint).Port,
                ((IPEndPoint)tcp.Client.LocalEndPoint).Address,
                ((IPEndPoint)tcp.Client.LocalEndPoint).Port);
        }

        public String Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.Close();
            }
            return  "サーバーと切断しました。";
        }

        public void Send(String sendText)
        {
            //TODO:Try～CatchでサーバーDownを回避
            //NetworkStreamを取得
            NetworkStream ns = tcp.GetStream();

            // タイムアウト
            ns.ReadTimeout = TimeOutMsec;
            ns.WriteTimeout = TimeOutMsec;

            //データ送信
            Encoding enc = Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(sendText + Environment.NewLine);
            ns.Write(sendBytes, 0, sendBytes.Length);

            //サーバーから送られたデータ受信する
            MemoryStream ms = new MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize = 0;

            do
            {
                //データの一部を受信する
                resSize = ns.Read(resBytes, 0, resBytes.Length);
                if (resSize == 0)
                {
                    MessageBox.Show("Disconnect Server",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    break;
                }

                //受信したデータを蓄積し、データの最後が\nでない時は受信を続ける
                ms.Write(resBytes, 0, resSize);
            } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

            ms.Close();

            //閉じる
            ns.Close();
        }
    }
}
