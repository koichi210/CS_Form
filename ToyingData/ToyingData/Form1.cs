using System;
using System.Windows.Forms;
using StandardTemplate;

namespace ToyingData
{
    public partial class Form1 : Form
    {
        StcUtils util = new StcUtils();
        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.ToyingData;
        }

        private void textBox_Source_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void textBox_Dest_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
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
            String[] sourceLines = util.ChangeStrLinear2Array(textBox_Source.Text, Environment.NewLine);
            String destText = "";
            if (radioButton_DeleteDuplicate.Checked)
            {
                destText = DeleteDuplicateLines(sourceLines);
            }
            else if (radioButton_ChangeWide2Narrow.Checked)
            {
                destText = ChangeWide2NarrowLines(sourceLines);
            }
            textBox_Dest.Text = destText;

            if (destText != String.Empty)
            {
                util.SetClipboardText(textBox_Dest.Text);
            }
        }

        private String DeleteDuplicateLines(String[] sourceLines)
        {
            String[] destLines = util.TrimDuplication(sourceLines);
            return util.ChangeStrArray2Linear(destLines, Environment.NewLine);
        }

        private String ChangeWide2NarrowLines(String[] sourceLines)
        {
            String[] destLines = ChangeWide2Narrow(sourceLines);
            return util.ChangeStrArray2Linear(destLines, Environment.NewLine);
        }

        private String[] ChangeWide2Narrow(String[] lines)
        {
            String regexPattern;
            if (!Logic.TryGetRegexPattern(
                checkBox_Wide2Narrow_Number.Checked,
                checkBox_Wide2Narrow_Alpha_Large.Checked,
                checkBox_Wide2Narrow_Alpha_Small.Checked,
                checkBox_Wide2Narrow_Space.Checked,
                out regexPattern))
            {
                MessageBox.Show("変換対象が選ばれませんでした");
                return null;
            }

            return Logic.ApplyWide2Narrow(lines, regexPattern);
        }
    }
}
