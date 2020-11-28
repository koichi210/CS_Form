using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FFEdit
{
    public partial class ErrorMsg : Form
    {
        public ErrorMsg(String msg)
        {
            InitializeComponent();
            textBox_ErrorMessage.Text = msg;
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
