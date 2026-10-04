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
            using (FormSub formSub = new FormSub())
            {
                if (formSub.ShowDialog() == DialogResult.OK)
                {
                    label1.Text = formSub.Value;
                }
            }
        }
    }
}
