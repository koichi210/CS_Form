using System;
using System.Windows.Forms;
using StandardTemplate;
using System.IO;

namespace ToyingFile
{
    public partial class Form1 : Form
    {
        StcUtils util = new StcUtils();
        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.ToyingFile;

            InitializePlaceholders();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(textBox_DeleteString、Windowsの仕様で表示されない)は対象外
        private void InitializePlaceholders()
        {
            textBox_Directory.PlaceholderText = @"例: C:\Work";
            textBox_File.PlaceholderText = "例: *.txt";
        }

        private void textBox_Directory_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void textBox_File_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void textBox_DeleteString_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            if (textBox_Directory.Text == String.Empty)
            {
                MessageBox.Show("対象ファイルのディレクトリが設定されていません");
                return;
            }

            //リストアップ
            String[] targetFiles = GetTargetFiles();

            // メニュー
            if (radioButton_DeleteString.Checked)
            {
                DeleteStringFromFiles(targetFiles);
            }
        }

        private String[] GetTargetFiles()
        {
            String searchPattern = "*";
            if (textBox_File.Text != String.Empty)
            {
                searchPattern = textBox_File.Text;
            }

            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if (checkBox_SubDirectory.Checked)
            {
                searchOption = SearchOption.AllDirectories;
            }

            return Directory.GetFiles(textBox_Directory.Text, searchPattern, searchOption);
        }

        private void DeleteStringFromFiles(String[] filePaths)
        {
            String[] deleteStrings = textBox_DeleteString.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            StcFileInputOutput fio = new StcFileInputOutput();
            foreach (String filePath in filePaths)
            {
                String fileData = fio.LoadFile(filePath);
                String resultData = Logic.DeleteStringFromContent(fileData, deleteStrings, checkBox_CaseSensitive.Checked, checkBox_DeleteLine.Checked);
                fio.SaveFile(filePath, resultData);
            }
        }
    }
}
