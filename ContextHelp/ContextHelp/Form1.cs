using System;
using System.Windows.Forms;

namespace ContextHelp
{
    public partial class Form1 : Form
    {
        private HelpProvider popupHelp;

        public Form1()
        {
            InitializeComponent();

            popupHelp = new HelpProvider();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            popupHelp.SetHelpString(label1, "ラベルのポップアップヘルプだよ");
            popupHelp.SetHelpString(button1, "ボタンのポップアップヘルプだよ");
            popupHelp.SetHelpString(checkBox1, "チェックボックスのポップアップヘルプだよ");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("HelpProviderのテストだよ");
        }
    }
}
