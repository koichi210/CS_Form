using System;
using System.Windows.Forms;
using StandardTemplate;

namespace ToyingData
{
    public partial class Form1 : Form
    {
        private readonly StcUtils _util = new StcUtils();

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();
            this.Icon = Properties.Resources.ToyingData;
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_Source, "1行を1件として処理する。ファイルをドロップすると、そのファイルのパスが入る(中身ではない)");
            toolTip.SetToolTip(radioButton_DeleteDuplicate, "同じ内容の行を、最初の1行だけ残して削除する");
            toolTip.SetToolTip(radioButton_ChangeWide2Narrow, "下のチェックで選んだ種類の全角文字だけを半角にする");
            toolTip.SetToolTip(button_Execute, "選んだメニューで変換し、結果をクリップボードにもコピーする。空行は取り除かれる");
        }

        private void textBox_Source_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private void textBox_Dest_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private void textBox_Source_DragEnter(object sender, DragEventArgs e)
        {
            _util.SetDragFile(e);
        }

        private void textBox_Source_DragDrop(object sender, DragEventArgs e)
        {
            // typeof(string)ではドロップされたファイル名を取得できず常にnullになっていた
            textBox_Source.Text = _util.GetDropListLinear(e);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            string[] sourceLines = _util.ChangeStrLinear2Array(textBox_Source.Text, Environment.NewLine);
            string[] destLines = null;
            if (radioButton_DeleteDuplicate.Checked)
            {
                destLines = _util.TrimDuplication(sourceLines);
            }
            else if (radioButton_ChangeWide2Narrow.Checked)
            {
                destLines = ChangeWide2Narrow(sourceLines);
            }
            string destText = _util.ChangeStrArray2Linear(destLines, Environment.NewLine);
            textBox_Dest.Text = destText;

            if (destText != string.Empty)
            {
                _util.SetClipboardText(destText);
            }
        }

        private string[] ChangeWide2Narrow(string[] lines)
        {
            string regexPattern;
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
