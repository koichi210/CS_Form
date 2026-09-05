using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Encryption
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            textBox_Table.Text = "8 1 4 7 2 3 9 5 6 0";
            textBox_Key.Text = "257";
            radioButton_Decode.Checked = true;

            // encode
            // 0 1 2 3 4 5 6 7 8 9 ↓
            // 8 1 4 7 2 3 9 5 6 0

            // decode
            // 0 1 2 3 4 5 6 7 8 9
            // 8 3 7 1 2 5 6 0 9 4 ↑
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            textBox_Result.Text = Logic.Execute(textBox_Table.Text, radioButton_Decode.Checked, textBox_Key.Text);
        }
    }
}
