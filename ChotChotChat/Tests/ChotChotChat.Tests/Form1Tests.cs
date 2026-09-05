using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChotChotChat.Tests
{
    /// <summary>
    /// Form1（簡易TCPチャットのサンプル）のテスト。
    ///
    /// ⚠️ button_Server_Click / button_Client_Click / button_Send_Click は、
    /// 内部のServer/Clientクラスを通じて実際のTCPソケット(ポート8080固定)を
    /// 開き、Server.Recv/Client.SendはAcceptTcpClient/NetworkStream.Readで
    /// 相手からのデータを実際にブロック待ちする作りになっている。
    /// 相手側を用意しない限りテストがハングし、また接続失敗時や切断検知時には
    /// MessageBox.Showも呼ばれる。安全にテストできないため、これら3つの
    /// ハンドラと内部のServer/Clientクラスはテスト対象から完全に除外し、
    /// フォームの生成のみを検証する。
    /// </summary>
    [TestClass]
    public class Form1Tests
    {
        [TestMethod]
        public void コンストラクタで例外なく生成できる()
        {
            using (var form = new Form1())
            {
                Assert.IsNotNull(form);
            }
        }
    }
}
