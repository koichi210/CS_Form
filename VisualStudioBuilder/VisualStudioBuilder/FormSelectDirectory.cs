using System;
using System.Windows.Forms;

namespace VisualStudioBuilder
{
    public partial class FormSelectDirectory : Form
    {
        // 親へ渡すパラメータ
        public String DirectoryPath { get; private set; } = "";

        public FormSelectDirectory()
        {
            InitializeComponent();

            InitializePlaceholders();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnly・Multilineの欄は無いので全欄が対象
        private void InitializePlaceholders()
        {
            textBox_DirectoryPath.PlaceholderText = @"例: C:\Work\Source";
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            DirectoryPath = textBox_DirectoryPath.Text;
        }
    }
}
