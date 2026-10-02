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
            InitializeToolTips();
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

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(button1, "大文字小文字を区別する完全一致、区別しない完全一致、区別しない前方一致(上の文字列が下の文字列で始まるか)の3通りの結果を表示する");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Logic.Compare(textBoxSource.Text, textBoxTarget.Text));
            //MessageBox.Show(Logic.SampleCompare());
        }
    }
}
