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
