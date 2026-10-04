using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dialog
{
    public partial class Form1 : Form
    {
        private readonly Button _buttonDynamic = new Button()
        {
            Text = "DialogDynamic",
            Location = new Point(20, 20),
        };

        public Form1()
        {
            InitializeComponent();

            _buttonDynamic.Click += buttonDynamic_Click;
            this.Controls.Add(_buttonDynamic);
            this.Text = "Form1";
        }

        private void buttonDynamic_Click(object sender, EventArgs e)
        {
            // モーダルダイアログとして表示(ShowDialogで出したフォームは閉じても破棄されないのでusingで破棄する)
            using (DialogDynamic dialog = new DialogDynamic())
            {
                dialog.ShowDialog();
            }
        }

        private void button_static_modal_Click(object sender, EventArgs e)
        {
            using (FormStatic fs = new FormStatic())
            {
                fs.ShowDialog();
            }
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
