using System;
using System.Windows.Forms;

namespace VariableArgument
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            textBox_Input.Text = "Santa_%d.raw";
            textBox_Replace_Digit.Text = "1";
        }

        private void button_Exceute_C_Click(object sender, EventArgs e)
        {
            MessageBox.Show("C#ではsprintfとか使えませんでした。。");
        }

        private void button_Exceute_CS_Click(object sender, EventArgs e)
        {
            textBox_Output.Text = textBox_Input.Text.Replace("%d", textBox_Replace_Digit.Text);
        }
    }
}
