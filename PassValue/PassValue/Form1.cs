using System;
using System.Windows.Forms;

namespace PassValue
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_Click_PopupWindow(object sender, EventArgs e)
        {
            FormSub fs = new FormSub();
            DialogResult dr = fs.ShowDialog();
            if (dr == DialogResult.OK)
            {
                label1.Text = fs.value;
            }
        }

    }
}
