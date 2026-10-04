using System;
using System.Windows.Forms;

namespace FizzBuzz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializePlaceholders();
            InitializeToolTips();
            textBox_Number.Text = "100";
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 結果の出力欄(textBox_Result、Multilineのため表示もされない)は対象外
        private void InitializePlaceholders()
        {
            textBox_Number.PlaceholderText = "例: 100";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_Number, "1からこの数までを判定する。3の倍数はFizz、5の倍数はBuzz、7の倍数はWoofになり、重なる場合はつなげて表示する");
        }

        private void button_execute_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox_Number.Text))
            {
                MessageBox.Show("pls set number");
                return;
            }
            textBox_Result.Text = Logic.FizzBuzz(int.Parse(textBox_Number.Text));
        }
    }
}
