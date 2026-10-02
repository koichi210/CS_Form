using System;
using System.Windows.Forms;

namespace VariableArgument
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();

            textBox_Input.Text = "Santa_%d.raw";
            textBox_Replace_Digit.Text = "1";
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnlyの出力欄(textBox_Output、結果をプログラムが書き込む)は対象外
        private void InitializePlaceholders()
        {
            textBox_Input.PlaceholderText = "例: image_%d.raw";
            textBox_Replace_Digit.PlaceholderText = "例: 1";
        }

        private void button_Execute_C_Click(object sender, EventArgs e)
        {
            MessageBox.Show("C#ではsprintfとか使えませんでした。。");
        }

        private void button_Execute_CS_Click(object sender, EventArgs e)
        {
            textBox_Output.Text = textBox_Input.Text.Replace("%d", textBox_Replace_Digit.Text);
        }
    }
}
