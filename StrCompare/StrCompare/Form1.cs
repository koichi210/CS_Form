using System;
using System.Windows.Forms;

namespace StrCompare
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBoxSource.Text = "SampleString";
            textBoxTarget.Text = "samplestring";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Logic.Compare(textBoxSource.Text, textBoxTarget.Text));
            //MessageBox.Show(Logic.SampleCompare());
        }
    }
}
