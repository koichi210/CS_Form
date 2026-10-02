using System;
using System.Windows.Forms;

namespace WebBrowser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.WebBrowser;

            InitializePlaceholders();
            InitializeToolTips();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnly・Multilineの欄は無いので全欄が対象
        private void InitializePlaceholders()
        {
            textBox_Url.PlaceholderText = "例: https://example.com";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_Url, "開くページのURL。「https://」などから始まる完全な形で入力する(省略すると読み込めない)");
            toolTip.SetToolTip(button_Test, "表示中のページのHTMLソースをメッセージで表示する");
        }

        private void button_Go_Click(object sender, EventArgs e)
        {
            if ( textBox_Url.Text.Length == 0 )
            {
                // URLが指定されていない
                return;
            }

            //URL読み込み
            try
            {
                webBrowser.Navigate(new Uri(textBox_Url.Text));
            }
            catch (System.UriFormatException)
            {
                MessageBox.Show("ページが読み込めませんでした" + Environment.NewLine + textBox_Url.Text,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button_Test_Click(object sender, EventArgs e)
        {
            String documentText = webBrowser.DocumentText;
            MessageBox.Show(documentText, "Source Text");
        }
    }
}
