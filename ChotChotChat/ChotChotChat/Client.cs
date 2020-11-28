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
    class Client
    {
        private readonly int port = 8080;
        private readonly int TimeOutMsec = 100000;
        private bool IsConnect = false;
        private TcpClient tcp;

        public String Connect(String HostName)
        {
            if (IsConnect)
            {
                MessageBox.Show("すでにサーバーと接続済みです");
                return "";
            }
            
            //TcpClientを作成し、サーバーと接続する
            tcp = new TcpClient(HostName, port);
            IsConnect = true;

            return String.Format("サーバー({0}:{1})と接続しました({2}:{3})。",
                ((IPEndPoint)tcp.Client.RemoteEndPoint).Address,
                ((IPEndPoint)tcp.Client.RemoteEndPoint).Port,
                ((IPEndPoint)tcp.Client.LocalEndPoint).Address,
                ((IPEndPoint)tcp.Client.LocalEndPoint).Port);
        }

        public String Diconnect()
        {
            if (IsConnect)
            {
                IsConnect = false;
                tcp.Close();
            }
            return  "サーバーと切断しました。";
        }

        public void Send(String SendText)
        {
            //TODO:Try～CatchでサーバーDownを回避
            //NetworkStreamを取得
            NetworkStream ns = tcp.GetStream();

            // タイムアウト
            ns.ReadTimeout = TimeOutMsec;
            ns.WriteTimeout = TimeOutMsec;

            //データ送信
            Encoding enc = Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(SendText + Environment.NewLine);
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

            //受信したデータを文字列に変換
            String resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            ms.Close();

            //閉じる
            ns.Close();
        }
    }
}
