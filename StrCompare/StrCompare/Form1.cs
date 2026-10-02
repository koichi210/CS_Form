using System;
using System.Windows.Forms;

namespace StrCompare
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializePlaceholders();
            textBoxSource.Text = "SampleString";
            textBoxTarget.Text = "samplestring";
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnly・Multilineの欄は無いので全欄が対象
        private void InitializePlaceholders()
        {
            textBoxSource.PlaceholderText = "例: SampleString";
            textBoxTarget.PlaceholderText = "例: samplestring";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Logic.Compare(textBoxSource.Text, textBoxTarget.Text));
            //MessageBox.Show(Logic.SampleCompare());
        }
    }
}
