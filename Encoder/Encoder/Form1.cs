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
            this.Icon = Properties.Resources.Encoder;
            radioButton_Utf8ToSjis.Checked = true;
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
