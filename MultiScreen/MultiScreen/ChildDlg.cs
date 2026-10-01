using System;
using System.Windows.Forms;

namespace MultiScreen
{
    public partial class ChildDlg : Form
    {
        public ChildDlg(String buttonName)
        {
            InitializeComponent();

            ChildDlg_button.Text = buttonName;
        }

        private void ChildDlg_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
