using System;
using System.Windows.Forms;

namespace Reminder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(textBox5の本文、Windowsの仕様で表示されない)は対象外
        private void InitializePlaceholders()
        {
            textBox1.PlaceholderText = "例: user@example.com";
            textBox2.PlaceholderText = "例: cc@example.com";
            textBox3.PlaceholderText = "例: bcc@example.com";
            textBox4.PlaceholderText = "例: 定例会議のリマインド";
        }
    }
}
