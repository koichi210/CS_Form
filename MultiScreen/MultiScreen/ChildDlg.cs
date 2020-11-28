using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MouseTrainingWithMultiScreen
{
    public partial class ChildDlg : Form
    {
        public ChildDlg(String ButtonName)
        {
            InitializeComponent();

            ChildDlg_button.Text = ButtonName;

            //this.Size = new Size(200, 200); // ウィンドウのサイズ
        }

        private void buttonAllPopup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
