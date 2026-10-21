using System;
using System.Windows.Forms;
using StandardTemplate;
using System.IO;

namespace ToyingFile
{
    public partial class Form1 : Form
    {
        private readonly StcUtils _util = new StcUtils();

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.ToyingFile;

            InitializePlaceholders();
            InitializeToolTips();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(textBox_DeleteString、Windowsの仕様で表示されない)は対象外
        private void InitializePlaceholders()
        {
            textBox_Directory.PlaceholderText = @"例: C:\Work";
            textBox_File.PlaceholderText = "例: *.txt";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_File, "対象にするファイル名のパターン(*や?のワイルドカード可)。空欄なら全ファイル");
            toolTip.SetToolTip(textBox_DeleteString, "削除する文字列。1行に1つずつ書く(空行は無視)");
            toolTip.SetToolTip(checkBox_DeleteLine, "指定文字を含む行を行ごと空行にする(行は詰めない)。改行コード(CRLF/LF/CR)は元のまま残す");
            toolTip.SetToolTip(button_Execute, "対象ファイルをShift_JISとして読み込み、結果で直接上書きする。バックアップは作らない");
        }

        private void textBox_Directory_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private void textBox_File_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private void textBox_DeleteString_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            string error = Logic.ValidateDirectory(textBox_Directory.Text);
            if (error != null)
            {
                MessageBox.Show(error);
                return;
            }

            //リストアップ
            // ディレクトリがあっても、アクセス権の無いサブフォルダやパターンに使えない文字があると列挙の途中で失敗する
            string[] targetFiles;
            try
            {
                targetFiles = GetTargetFiles();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException)
            {
                MessageBox.Show("対象ファイルを列挙できませんでした" + Environment.NewLine + ex.Message);
                return;
            }

            // メニュー
            if (radioButton_DeleteString.Checked)
            {
                DeleteStringFromFiles(targetFiles);
            }
        }

        private string[] GetTargetFiles()
        {
            string searchPattern = (textBox_File.Text != string.Empty) ? textBox_File.Text : "*";
            SearchOption searchOption = checkBox_SubDirectory.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            return Directory.GetFiles(textBox_Directory.Text, searchPattern, searchOption);
        }

        private void DeleteStringFromFiles(string[] filePaths)
        {
            string[] deleteStrings = textBox_DeleteString.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            StcFileInputOutput fio = new StcFileInputOutput();
            foreach (string filePath in filePaths)
            {
                string fileData = fio.LoadFile(filePath);
                string resultData = Logic.DeleteStringFromContent(fileData, deleteStrings, checkBox_CaseSensitive.Checked, checkBox_DeleteLine.Checked);

                // 削除する文字列が無かったファイルは書き戻さない。以前は対象の全ファイルをShift_JISで書き直していたため、
                // 一致しなかったUTF-8のテキストや画像等のバイナリ(ファイル名パターン空欄=全ファイル)まで化けて壊れていた
                if (resultData == fileData)
                {
                    continue;
                }
                fio.SaveFile(filePath, resultData);
            }
        }
    }
}
