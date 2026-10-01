using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dialog
{
    public partial class Form1 : Form
    {
        private Button buttonDynamic;

        public Form1()
        {
            InitializeComponent();

            buttonDynamic = new Button()
            {
                Text = "DialogDynamic",
                Location = new Point(20, 20),
            };

            buttonDynamic.Click += new EventHandler(buttonDynamic_Click);
            this.Controls.Add(buttonDynamic);
            this.Text = "Form1";
        }

        void buttonDynamic_Click(object sender, EventArgs e)
        {
            DialogDynamic dialog = new DialogDynamic();

            // モーダルダイアログとして表示
            dialog.ShowDialog();
        }

        private void button_static_modal_Click(object sender, EventArgs e)
        {
            FormStatic fs = new FormStatic();
            fs.ShowDialog();
        }

        private void button_static_modeless_Click(object sender, EventArgs e)
        {
            FormStatic fs = new FormStatic();
            fs.Show();
        }
    }

    // 動的生成するダイアログ
    class DialogDynamic : Form
    {
        public DialogDynamic()
        {
            this.Text = "DialogDynamic";
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
        }
    }
}
