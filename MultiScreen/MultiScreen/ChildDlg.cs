using System;
using System.Windows.Forms;

namespace MouseTrainingWithMultiScreen
{
    public partial class ChildDlg : Form
    {
        public ChildDlg(String buttonName)
        {
            InitializeComponent();

            ChildDlg_button.Text = buttonName;
        }

        private void buttonAllPopup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
