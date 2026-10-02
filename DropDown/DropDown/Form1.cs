using System;
using System.Windows.Forms;
using StandardTemplate;

namespace DropDown
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox1, "ファイルをドロップすると、そのパスを1行に1つずつ表示する");
        }

        // ドラッグ
        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            util.SetDragFile(e);
        }

        // ドロップ
        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            textBox1.Text = util.GetDropListLinear(e);
        }
    }
}
