using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisualStudioBuilder
{
    public partial class FormSelectDirectory : Form
    {
        // 親へ渡すパラメータ
        public String DirectoryPath = "";

        public FormSelectDirectory()
        {
            InitializeComponent();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            DirectoryPath = textBox_DirectoryPath.Text;
        }
    }
}
