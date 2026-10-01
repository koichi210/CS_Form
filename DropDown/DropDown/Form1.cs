using System;
using System.Windows.Forms;
using StandardTemplate;

namespace DropDown
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();

        public Form1()
        {
            InitializeComponent();
        }

        // ドラッグ
        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            util.SetDragFile(e);
        }

        // ドロップ
        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            textBox1.Text = util.GetDropListLinear(e);
        }
    }
}
