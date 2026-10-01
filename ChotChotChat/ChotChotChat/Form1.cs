using System;
using System.Windows.Forms;

namespace ChotChotChat
{
    public partial class Form1 : Form
    {
        Server server = new Server();
        Client client = new Client();

        public Form1()
        {
            InitializeComponent();
        }

        private void button_Server_Click(object sender, EventArgs e)
        {
            String listenAddress = "127.0.0.1";
            try
            {
                label_StatusBar.Text = server.Connect(listenAddress);
                server.Receive(this);
            }
            finally
            {
                // 暫定：毎回コネクト(受信に失敗しても接続中のまま残らないよう必ず閉じる)
                label_StatusBar.Text = server.Disconnect();
            }
        }

        private void button_Client_Click(object sender, EventArgs e)
        {
            String hostName = "127.0.0.1";
            label_StatusBar.Text = client.Connect(hostName);
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            if (!client.IsConnected)
            {
                MessageBox.Show("先にサーバーと接続してください");
                return;
            }

            try
            {
                client.Send(textBox_Message.Text);
                textBox_Log.Text += textBox_Message.Text + Environment.NewLine;
            }
            finally
            {
                // 暫定：毎回コネクト(送信に失敗しても接続中のまま残らないよう必ず切断する)
                label_StatusBar.Text = client.Disconnect();
            }
        }
    }
}
