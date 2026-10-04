using System;
using System.Windows.Forms;

namespace TabControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabPage1.Controls.Add(new Button());
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPage1)
            {
                MessageBox.Show("page1");
            }
            else if (e.TabPage == tabPage2)
            {
                MessageBox.Show("page2");
            }
        }
    }
}
