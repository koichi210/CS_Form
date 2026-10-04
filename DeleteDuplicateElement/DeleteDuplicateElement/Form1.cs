using System;
using System.Windows.Forms;
using StandardTemplate;

namespace DeleteDuplicateElement
{
    public partial class Form1 : Form
    {
        private readonly StcUtils _util = new StcUtils();

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();
            this.Icon = Properties.Resources.DeleteDuplicateElement;
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_Source, "1行に1要素を入力する。ファイルをドロップするとそのパス一覧が入る。Ctrl+Aで全選択");
            toolTip.SetToolTip(button_Execute, "左の欄から重複した行と空行を除き、最初に出てきた順のまま右の欄に出す");
        }

        private void textBox_Source_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_Source, e);
        }

        private void textBox_Dest_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_Dest, e);
        }

        private void textBox_Source_DragEnter(object sender, DragEventArgs e)
        {
            _util.SetDragFile(e);
        }

        private void textBox_Source_DragDrop(object sender, DragEventArgs e)
        {
            // typeof(String)ではドロップされたファイル名を取得できず常にnullになっていた
            textBox_Source.Text = _util.GetDropListLinear(e);
        }
        
        private void button_Execute_Click(object sender, EventArgs e)
        {
            String[] sourceArray = _util.ChangeStrLinear2Array(textBox_Source.Text, Environment.NewLine);
            String[] uniqueArray = _util.TrimDuplication(sourceArray);
            textBox_Dest.Text = _util.ChangeStrArray2Linear(uniqueArray, Environment.NewLine);
        }
    }
}
