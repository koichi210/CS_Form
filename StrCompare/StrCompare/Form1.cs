using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            Compare(textBoxSource.Text, textBoxTarget.Text);
            //SampleCompare();
        }

        private void Compare(String Source, String Target)
        {
            String ResultStr = "";

            // 大文字・小文字は区別される（完全一致）
            Boolean ret = Source.Equals(Target);
            ResultStr += "大文字小文字区別する（完全一致） =" + ret.ToString() + Environment.NewLine;

            // 大文字・小文字を区別しない（それ以外は完全一致）
            ret = Source.Equals(Target, StringComparison.OrdinalIgnoreCase);
            ResultStr += "大文字小文字区別しない（完全一致） =" + ret.ToString() + Environment.NewLine;

            // 大文字・小文字を区別しない（前方一致で比較）
            ret = Source.StartsWith(Target, StringComparison.OrdinalIgnoreCase);
            ResultStr += "大文字小文字区別しない（前方一致） =" + ret.ToString() + Environment.NewLine;

            MessageBox.Show(ResultStr);
        }

        private void SampleCompare()
        {
            String Source = "sampleString";
            String ResultStr = "";

            // 大文字・小文字は区別される（完全一致）
            Boolean ret = Source.Equals("sampleString");
            ResultStr += "[" + Source + "][" + "sampleString" + "]" + Environment.NewLine;
            ResultStr += "大文字小文字区別する（完全一致） =" + ret.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字は区別される（完全一致）
            ret = Source.Equals("sampleSTRING");
            ResultStr += "[" + Source + "][" + "sampleSTRING" + "]" + Environment.NewLine;
            ResultStr += "大文字小文字区別する（完全一致） =" + ret.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字を区別しない（それ以外は完全一致）
            ret = Source.Equals("sampleSTRING", StringComparison.OrdinalIgnoreCase);
            ResultStr += "[" + Source + "][" + "sampleSTRING" + "]" + Environment.NewLine;
            ResultStr += "大文字小文字区別しない（完全一致） =" + ret.ToString() + Environment.NewLine + Environment.NewLine;

            // 大文字・小文字を区別しない（前方一致で比較）
            ret = Source.StartsWith("SAMPLE", StringComparison.OrdinalIgnoreCase);
            ResultStr += "[" + Source + "][" + "SAMPLE" + "]" + Environment.NewLine;
            ResultStr += "大文字小文字区別しない（前方一致） =" + ret.ToString() + Environment.NewLine + Environment.NewLine;

            MessageBox.Show(ResultStr);
        }
    }
}
