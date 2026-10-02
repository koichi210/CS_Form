using System;
using System.Windows.Forms;

namespace VariableArgument
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
            InitializeToolTips();

            textBox_Input.Text = "Santa_%d.raw";
            textBox_Replace_Digit.Text = "1";
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnlyの出力欄(textBox_Output、結果をプログラムが書き込む)は対象外
        private void InitializePlaceholders()
        {
            textBox_Input.PlaceholderText = "例: image_%d.raw";
            textBox_Replace_Digit.PlaceholderText = "例: 1";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(button_Execute_CS, "入力文字列中のすべての「%d」を、置き換える値の文字列でそのまま置換する");
            toolTip.SetToolTip(button_Execute_C, "未実装。押すとメッセージを表示するだけ");
        }

        private void button_Execute_C_Click(object sender, EventArgs e)
        {
            MessageBox.Show("C#ではsprintfとか使えませんでした。。");
        }

        private void button_Execute_CS_Click(object sender, EventArgs e)
        {
            textBox_Output.Text = textBox_Input.Text.Replace("%d", textBox_Replace_Digit.Text);
        }
    }
}
