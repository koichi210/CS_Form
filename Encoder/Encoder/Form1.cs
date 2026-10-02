using System;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace Encoder
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();
            this.Icon = Properties.Resources.Encoder;
            radioButton_Utf8ToSjis.Checked = true;
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(DropBox, "ファイルをドロップすると選んだ向きで変換する。UTF-8 ⇒ Sjisでは同じフォルダに「元の名前_sjis.拡張子」で保存し、同名のファイルは上書きする。フォルダは対象外");
            toolTip.SetToolTip(radioButton_SjisToUtf8, "未実装。この向きを選んでファイルをドロップしても変換しない");
        }

        private void Execute(String inputPathName)
        {
            if (Directory.Exists(inputPathName))
            {
                MessageBox.Show("フォルダは対応外です。" + Environment.NewLine + inputPathName);
                return;
            }

            if (!File.Exists(inputPathName))
            {
                MessageBox.Show("ファイルパスを確認してください。" + Environment.NewLine + inputPathName);
                return;
            }

            if (radioButton_Utf8ToSjis.Checked)
            {
                StcFileInputOutput fio = new StcFileInputOutput();
                String outputFileName = Path.GetDirectoryName(inputPathName)
                    + @"\" + Path.GetFileNameWithoutExtension(inputPathName)
                    + "_sjis"
                    + Path.GetExtension(inputPathName);
                fio.ChangeStringCodeUTF2SJIS(inputPathName, outputFileName);
            }
            else
            {
                MessageBox.Show("未実装です");
            }
        }

        private void DropBox_DragEnter(object sender, DragEventArgs e)
        {
            util.SetDragFile(e);
        }

        private void DropBox_DragDrop(object sender, DragEventArgs e)
        {
            String[] fileList = (String[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (String filePath in fileList)
            {
                Execute(filePath);
            }
        }
    }
}
