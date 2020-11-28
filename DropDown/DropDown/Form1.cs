using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
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
