using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            String[] FileList = (String[])e.Data.GetData(typeof(String));
            textBox_Source.Text = util.ChangeStrArray2Linear(FileList, Environment.NewLine);
        }
        
        private void button_Execute_Click(object sender, EventArgs e)
        {
            String[] SourceArray = util.ChangeStrLinear2Array(textBox_Source.Text, Environment.NewLine);
            String[] TrimDupArray = util.TrimDuplication(SourceArray);
            textBox_Dest.Text = util.ChangeStrArray2Linear(TrimDupArray, Environment.NewLine);
        }
    }
}
