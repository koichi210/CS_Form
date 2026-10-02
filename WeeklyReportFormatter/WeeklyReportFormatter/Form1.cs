using System;
using System.Windows.Forms;
using StandardTemplate;

namespace WeeklyReportFormatter
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();
        private StcFileInputOutput fileInputOutput = new StcFileInputOutput();

        private readonly String UserNameFileName = "WhoAmI.txt";

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.WeeklyReportFormatter;

            InitializePlaceholders();

            // カレントディレクトリ移動
            util.SetCurrentDirectory();

            textBox_UserName.Text = fileInputOutput.LoadFile(UserNameFileName);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 変換元・変換先の欄(textBox_ThisWeek*/NextWeek*/Perforce*)はすべてMultilineで
        // Windowsの仕様で表示されないため対象外
        private void InitializePlaceholders()
        {
            textBox_UserName.PlaceholderText = "例: Taro Yamada";
        }

        private void textBox_ThisWeekBefore_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_ThisWeekBefore, e);
        }

        private void textBox_ThisWeekAfter_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_ThisWeekAfter, e);
        }

        private void textBox_NextWeekBefore_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_NextWeekBefore, e);
        }

        private void textBox_NextWeekAfter_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_NextWeekAfter, e);
        }

        private void textBox_PerforceBefore_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_PerforceBefore, e);
        }

        private void textBox_PerforceAfter_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(textBox_PerforceAfter, e);
        }

        private void button_ThisWeekChange_Click(object sender, EventArgs e)
        {
            ConvertText(textBox_ThisWeekBefore, textBox_ThisWeekAfter,
                before => Logic.FormatThisWeek(before, textBox_UserName.Text));
        }

        private void button_NextWeekChange_Click(object sender, EventArgs e)
        {
            ConvertText(textBox_NextWeekBefore, textBox_NextWeekAfter,
                before => Logic.FormatNextWeek(before, textBox_UserName.Text));
        }

        private void button_PerforceChange_Click(object sender, EventArgs e)
        {
            ConvertText(textBox_PerforceBefore, textBox_PerforceAfter, Logic.FormatPerforce);
        }

        /// <summary>
        /// 変換元テキストを整形して変換先に表示し、クリップボードにもコピーする（3ボタン共通処理）
        /// </summary>
        private void ConvertText(TextBox beforeTextBox, TextBox afterTextBox, Func<String, String> format)
        {
            afterTextBox.Clear();
            if (beforeTextBox.Text == String.Empty)
            {
                MessageBox.Show("変換元データが入力されていません");
                return;
            }

            afterTextBox.Text = format(beforeTextBox.Text);
            util.SetClipboardText(afterTextBox.Text);
        }
    }
}
