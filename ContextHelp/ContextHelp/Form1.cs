using System;
using System.Windows.Forms;

namespace ContextHelp
{
    public partial class Form1 : Form
    {
        private readonly HelpProvider _popupHelp = new HelpProvider();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _popupHelp.SetHelpString(label1, "ラベルのポップアップヘルプだよ");
            _popupHelp.SetHelpString(button1, "ボタンのポップアップヘルプだよ");
            _popupHelp.SetHelpString(checkBox1, "チェックボックスのポップアップヘルプだよ");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("HelpProviderのテストだよ");
        }
    }
}
