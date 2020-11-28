using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

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

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
