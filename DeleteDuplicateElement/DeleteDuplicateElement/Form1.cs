using System;
using System.Windows.Forms;
using StandardTemplate;

namespace DeleteDuplicateElement
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.DeleteDuplicateElement;
        }

        private void textBox_Source_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_Source, e);
        }

        private void textBox_Dest_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_Dest, e);
        }

        private void textBox_Source_DragEnter(object sender, DragEventArgs e)
        {
            util.SetDragFile(e);
        }

        private void textBox_Source_DragDrop(object sender, DragEventArgs e)
        {
            // typeof(String)ではドロップされたファイル名を取得できず常にnullになっていた
            textBox_Source.Text = util.GetDropListLinear(e);
        }
        
        private void button_Execute_Click(object sender, EventArgs e)
        {
            String[] sourceArray = util.ChangeStrLinear2Array(textBox_Source.Text, Environment.NewLine);
            String[] uniqueArray = util.TrimDuplication(sourceArray);
            textBox_Dest.Text = util.ChangeStrArray2Linear(uniqueArray, Environment.NewLine);
        }
    }
}
