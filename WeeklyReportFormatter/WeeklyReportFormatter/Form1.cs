using System;
using System.Windows.Forms;
using StandardTemplate;

namespace WeeklyReportFormatter
{
    public partial class Form1 : Form
    {
        private readonly StcUtils _util = new StcUtils();
        private readonly StcFileInputOutput _fileInputOutput = new StcFileInputOutput();

        private const string _userNameFileName = "WhoAmI.txt";

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.WeeklyReportFormatter;

            InitializePlaceholders();
            InitializeToolTips();

            // カレントディレクトリ移動
            _util.SetCurrentDirectory();

            textBox_UserName.Text = _fileInputOutput.LoadFile(_userNameFileName);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 変換元・変換先の欄(textBox_ThisWeek*/NextWeek*/Perforce*)はすべてMultilineで
        // Windowsの仕様で表示されないため対象外
        private void InitializePlaceholders()
        {
            textBox_UserName.PlaceholderText = "例: Taro Yamada";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_UserName, "整形時に取り除く自分の名前。起動時にexeと同じフォルダのWhoAmI.txtから読み込む(画面で変えても保存はされない)");
            toolTip.SetToolTip(textBox_ThisWeekBefore, "1行で1件として扱う。各行の「名前+スペース」より後ろを括弧で囲み、先頭にタブを付ける");
            toolTip.SetToolTip(textBox_NextWeekBefore, "3行で1件として扱う(課題No、課題名、名前とストーリーポイントの行)。空行は無視");
            toolTip.SetToolTip(textBox_PerforceBefore, "2行で1件として扱う(ProjectID、Summary)。全件を改行なしで1行につなげる");
            toolTip.SetToolTip(button_ThisWeekChange, "整形結果を右の欄に出し、クリップボードにもコピーする");
            toolTip.SetToolTip(button_NextWeekChange, "整形結果を右の欄に出し、クリップボードにもコピーする");
            toolTip.SetToolTip(button_PerforceChange, "整形結果を右の欄に出し、クリップボードにもコピーする");
        }

        private void textBox_ThisWeekBefore_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_ThisWeekBefore, e);
        }

        private void textBox_ThisWeekAfter_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_ThisWeekAfter, e);
        }

        private void textBox_NextWeekBefore_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_NextWeekBefore, e);
        }

        private void textBox_NextWeekAfter_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_NextWeekAfter, e);
        }

        private void textBox_PerforceBefore_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_PerforceBefore, e);
        }

        private void textBox_PerforceAfter_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_PerforceAfter, e);
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
        private void ConvertText(TextBox beforeTextBox, TextBox afterTextBox, Func<string, string> format)
        {
            afterTextBox.Clear();
            if (beforeTextBox.Text == string.Empty)
            {
                MessageBox.Show("変換元データが入力されていません");
                return;
            }

            afterTextBox.Text = format(beforeTextBox.Text);
            _util.SetClipboardText(afterTextBox.Text);
        }
    }
}
