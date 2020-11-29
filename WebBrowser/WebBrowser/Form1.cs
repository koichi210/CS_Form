using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
                return;
            }
        }

        private void button_Test_Click(object sender, EventArgs e)
        {
            String str = webBrowser.DocumentText;
            MessageBox.Show(str, "Source Text");
        }
    }
}
