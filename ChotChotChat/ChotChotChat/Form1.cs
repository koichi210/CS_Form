using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ChotChotChat
{
    public partial class Form1 : Form
    {
        private readonly Server _server = new Server();
        private readonly Client _client = new Client();

        // 閉じた後にワーカースレッドから届いた画面更新は捨てる
        private volatile bool _closed;

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(button_Server, "このPC(127.0.0.1)のポート8080で接続を待ち、メッセージを1件受信したら切断する。待っている間も画面は操作できる(同じ画面のclientからも送れる)");
            toolTip.SetToolTip(button_Client, "このPC(127.0.0.1)のポート8080で待っているサーバーに接続する");
            toolTip.SetToolTip(button_Send, "メッセージを送信し、サーバーの応答を受け取ったら切断する。次に送る前にclientで接続し直す必要がある");
        }

        private void button_Server_Click(object sender, EventArgs e)
        {
            String listenAddress = "127.0.0.1";
            if (_server.IsListening)
            {
                MessageBox.Show("すでにクライアントと接続済みです");
                return;
            }

            // 待ち受けの開始はすぐ終わるのでUIスレッドで行い、ポート使用中などの失敗をその場で知らせる
            try
            {
                label_StatusBar.Text = _server.Listen(listenAddress);
            }
            catch (SocketException ex)
            {
                label_StatusBar.Text = _server.Disconnect();
                MessageBox.Show("接続を待てませんでした" + Environment.NewLine + ex.Message);
                return;
            }

            button_Server.Enabled = false;

            // 接続要求と受信は相手しだいでいつまでも待つため、UIスレッドではなくワーカースレッドで待つ
            // (以前はUIスレッドでAcceptTcpClientしていて、待っている間は画面が固まっていた)
            RunInBackground(() =>
            {
                try
                {
                    String connected = _server.Accept();
                    PostToUi(() => label_StatusBar.Text = connected);

                    String received = _server.Receive();
                    PostToUi(() =>
                    {
                        if (received == null)
                        {
                            MessageBox.Show("Disconnect Client",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                        // ログ画面更新
                        textBox_Log.Text += received;
                    });
                }
                catch (Exception ex) when (IsNetworkError(ex))
                {
                    PostToUi(() => MessageBox.Show("メッセージを受信できませんでした" + Environment.NewLine + ex.Message));
                }
                finally
                {
                    // 暫定：毎回コネクト(受信に失敗しても接続中のまま残らないよう必ず閉じる)
                    String closed = _server.Disconnect();
                    PostToUi(() =>
                    {
                        label_StatusBar.Text = closed;
                        button_Server.Enabled = true;
                    });
                }
            });
        }

        private void button_Client_Click(object sender, EventArgs e)
        {
            String hostName = "127.0.0.1";
            label_StatusBar.Text = _client.Connect(hostName);
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show("先にサーバーと接続してください");
                return;
            }

            try
            {
                _client.Send(textBox_Message.Text);
                textBox_Log.Text += textBox_Message.Text + Environment.NewLine;
            }
            finally
            {
                // 暫定：毎回コネクト(送信に失敗しても接続中のまま残らないよう必ず切断する)
                label_StatusBar.Text = _client.Disconnect();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // 接続待ち・受信中のワーカーがあれば、ソケットを閉じて抜けさせる(ポート8080も解放される)
            _closed = true;
            _server.Disconnect();
            base.OnFormClosed(e);
        }

        // 接続待ちを途中で止めた(画面を閉じた)時や、相手が途中で切断した時に出る例外
        private static bool IsNetworkError(Exception ex)
        {
            return ex is SocketException || ex is IOException || ex is ObjectDisposedException;
        }

        // ワーカースレッドで実行する。待ち時間が長い(接続待ち等)のでスレッドプールではなく専用スレッドにし、
        // 画面を閉じた時に待ち続けていてもプロセスが終われるよう IsBackground にする。
        // 想定外の例外はUIスレッドで包んで投げ直す(UIスレッドで起きた未処理例外と同じ扱いにする)
        private void RunInBackground(Action work)
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    work();
                }
                catch (Exception ex)
                {
                    PostToUi(() => { throw new InvalidOperationException("通信中にエラーが発生しました", ex); });
                }
            });
            thread.IsBackground = true;
            thread.Name = "ChotChotChat.Socket";
            thread.Start();
        }

        // ワーカースレッドからUIスレッドへ処理を渡す。閉じた後なら何もしない
        private void PostToUi(Action action)
        {
            if (_closed || IsDisposed || !IsHandleCreated)
            {
                return;
            }

            try
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    if (!_closed)
                    {
                        action();
                    }
                }));
            }
            catch (InvalidOperationException)
            {
                // 判定の直後に閉じられてウィンドウハンドルが無くなった
            }
        }
    }
}
