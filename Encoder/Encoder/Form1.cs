using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private void Execute(String InputPathName)
        {
            if (Directory.Exists(InputPathName))
            {
                MessageBox.Show("フォルダは対応外です。" + Environment.NewLine + InputPathName);
                return;
            }

            if (!File.Exists(InputPathName))
            {
                MessageBox.Show("ファイルパスを確認してください。" + Environment.NewLine + InputPathName);
                return;
            }

            if (radioButton_Utf8ToSjis.Checked)
            {
                StcFileInputOutput fio = new StcFileInputOutput();
                String OutFileName = Path.GetDirectoryName(InputPathName);
                OutFileName += @"\" + Path.GetFileNameWithoutExtension(InputPathName);
                OutFileName += "_sjis";
                OutFileName += Path.GetExtension(InputPathName);
                fio.ChangeStringCodeUTF2SJIS(InputPathName, OutFileName);
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
            String[] FileList = (String[])e.Data.GetData(DataFormats.FileDrop, false);
            for(int i=0; i < FileList.Length; i++)
            {
                Execute(FileList[i]);
            }
        }
    }
}
