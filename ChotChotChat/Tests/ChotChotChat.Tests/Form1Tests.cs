using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChotChotChat.Tests
{
    /// <summary>
    /// Form1（簡易TCPチャットのサンプル）のテスト。
    ///
    /// ⚠️ button_Server_Click / button_Client_Click / button_Send_Click は、
    /// 実際のTCPソケット(ポート8080固定)を開き、接続失敗時や切断検知時には
    /// MessageBox.Showも呼ばれる。安全にテストできないため、これら3つの
    /// ハンドラとClientクラスはテスト対象から除外し、フォームの生成のみを検証する。
    /// Serverクラスは画面に触らない作りにしたので、ServerTestsで空いているポートを使って確かめている。
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
