using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FizzBuzz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox_Number.Text = "100";
        }

        private void button_execute_Click(object sender, EventArgs e)
        {
            if (textBox_Number.Text == String.Empty)
            {
                MessageBox.Show("pls set number");
                return;
            }
            textBox_Result.Text = Logic.FizzBuzz(int.Parse(textBox_Number.Text));

        }
    }
}
