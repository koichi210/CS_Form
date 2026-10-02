using System;
using System.Windows.Forms;

namespace Encryption
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
            InitializeToolTips();

            textBox_Table.Text = "8 1 4 7 2 3 9 5 6 0";
            textBox_Key.Text = "257";
            radioButton_Decode.Checked = true;

            // encode
            // 0 1 2 3 4 5 6 7 8 9 ↓
            // 8 1 4 7 2 3 9 5 6 0

            // decode
            // 0 1 2 3 4 5 6 7 8 9
            // 8 3 7 1 2 5 6 0 9 4 ↑
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnlyの欄(textBox_Result、結果の出力欄)は対象外
        private void InitializePlaceholders()
        {
            textBox_Table.PlaceholderText = "例: 8 1 4 7 2 3 9 5 6 0";
            textBox_Key.PlaceholderText = "例: 257";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_Table, "数字0〜9それぞれの置き換え先を、0の分から順に半角スペース区切りで10個並べる");
            toolTip.SetToolTip(textBox_Key, "変換する数字(整数)。1桁ずつ暗号テーブルで置き換える");
            toolTip.SetToolTip(radioButton_Decode, "暗号テーブルを逆向きに使い、暗号化した数字を元に戻す");
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            textBox_Result.Text = Logic.Execute(textBox_Table.Text, radioButton_Decode.Checked, textBox_Key.Text);
        }
    }
}
