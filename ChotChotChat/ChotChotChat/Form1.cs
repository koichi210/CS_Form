using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChotChotChat
{
    public partial class Form1 : Form
    {
        Server srv = new Server();
        Client clnt = new Client();
        public Form1()
        {
            InitializeComponent();
        }

        private void button_Server_Click(object sender, EventArgs e)
        {
            String ClientName = "127.0.0.1";
            try
            {
                label_StatusBar.Text = srv.Connect(ClientName);
                srv.Recv(this);
            }
            finally
            {
                // 暫定：毎回コネクト(受信に失敗しても接続中のまま残らないよう必ず閉じる)
                label_StatusBar.Text = srv.Diconnect();
            }
        }

        private void button_Client_Click(object sender, EventArgs e)
        {
            String HostName = "127.0.0.1";
            label_StatusBar.Text = clnt.Connect(HostName);
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            if (!clnt.IsConnected)
            {
                MessageBox.Show("先にサーバーと接続してください");
                return;
            }

            try
            {
                clnt.Send(textBox_Message.Text);
                textBox_Log.Text += textBox_Message.Text + Environment.NewLine;
            }
            finally
            {
                // 暫定：毎回コネクト(送信に失敗しても接続中のまま残らないよう必ず切断する)
                label_StatusBar.Text = clnt.Diconnect();
            }
        }
    }
}
