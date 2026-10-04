using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text;
using StandardTemplate;

namespace TrimHtmlData
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private bool _isDebug = false;
        private const string _settingFileName = @"TrimHtmlData.json";

        // 設定ファイルはJSONが基本。旧XML(TrimHtmlData.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const string _legacySettingFileName = @"TrimHtmlData.xml";
        private static readonly string[] _profileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\TrimHtmlData\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const string _appName = "TrimHtmlData";
        private readonly string _userDataFolder = UserDataLocation.GetUserDataFolder(_appName);

        private static bool IsJsonFile(string filePath)
        {
            return string.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private bool LoadProfile(string filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(_sr, filePath) : _sr.LoadProc(filePath);
        }

        private bool SaveProfile(string filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(_sr, filePath) : _sr.SaveSetting(filePath);
        }

        private readonly StcFileInputOutput _fio = new StcFileInputOutput();

        public Form1()
        {
            InitializeComponent();

            InitializeCommonSettings(Properties.Resources.TrimHtmlData);

            InitializePlaceholders();
            InitializeToolTips();

            _sr.RegisterItem(this);
            string defaultJsonPath = Path.Combine(_userDataFolder, _settingFileName);
            string defaultXmlPath = Path.Combine(_userDataFolder, _legacySettingFileName);
            JsonSaveRestore.LoadWithMigration(_sr, defaultJsonPath, defaultXmlPath,
                path => _sr.LoadProc(path));
            _util.UpdateProfileList(comboBox_LoadSetting, _profileExtensions, _settingFileName, _userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(textBox_SourceList、Windowsの仕様で表示されない)と、
        // 取得結果の出力欄(textBox_DestList、Multilineでもある)、
        // 非表示の欄(textBox_DelimiterWord、Visible=falseで処理にも未使用)は対象外
        private void InitializePlaceholders()
        {
            textBox_SearchWord.PlaceholderText = "例: <title>";
            textBox_TrimLineNum.PlaceholderText = "例: 1";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_SourceList, "HTMLを取得するページのURL。1行に1つずつ書く(空行は無視)");
            toolTip.SetToolTip(textBox_SearchWord, "この文字列を含む行を探す。行内のどこにあってもヒットする");
            toolTip.SetToolTip(textBox_TrimLineNum, "ヒットした行から数えて取り出す行数(ヒット行を含む)。空欄・0は1行として扱う");
            toolTip.SetToolTip(checkBox_FirstWordOnly, "各URLで最初にヒットした箇所だけを取り出す");
            toolTip.SetToolTip(button_Execute, "各URLのHTMLを取得して該当行を取り出し、結果をクリップボードにもコピーする");
            toolTip.SetToolTip(comboBox_LoadSetting, "選ぶと保存済みの設定を読み込み、各欄の内容が置き換わる");
        }

        // *******************************************************************************
        // データ保存先フォルダの変更(システムメニューから呼び出す)
        // ([[Cheetos/Form1.cs]]の同名機能と同じ考え方)
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DataFolderMenu.AppendToSystemMenu(this);
        }

        protected override void WndProc(ref Message m)
        {
            if (DataFolderMenu.IsChangeDataFolderCommand(m))
            {
                DataFolderMenu.ChangeDataFolder(_appName, _userDataFolder,
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, _appName));
                return;
            }

            base.WndProc(ref m);
        }

        private void textBox_SourceList_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_SourceList, e);
        }

        private void textBox_DestList_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_DestList, e);
        }

        private void textBox_SearchWord_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(textBox_SearchWord, e);
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            _isDebug = !_isDebug;
            MessageBox.Show("IsDebug=" + _isDebug.ToString());
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            // 出力先をクリア
            textBox_DestList.Text = "";

            // Text +=はURLごとにテキストボックス全体を作り直すため、StringBuilderに溜めて最後に1回だけセットする
            StringBuilder destText = new StringBuilder();

            int trimLineNum = Logic.GetTrimLine(textBox_TrimLineNum.Text);

            StringComparison comparison = checkBox_OrdinalCase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            StcDebug dbg = new StcDebug();
            dbg.IsDebugMode = _isDebug;

            string[] urls = textBox_SourceList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string url in urls)
            {
                destText.Append("◆").Append(url).Append(Environment.NewLine);

                string htmlSource = GetHtmlSource(url);
                dbg.WriteDataInNewFile(htmlSource, "_1_source");

                string result = Logic.GetSearchString(htmlSource, textBox_SearchWord.Text, trimLineNum, comparison, checkBox_FirstWordOnly.Checked);
                dbg.WriteDataInNewFile(result, "_2_search");

                destText.Append(result);
            }
            textBox_DestList.Text = destText.ToString();

            _util.SetClipboardText(textBox_DestList.Text);
        }

        /// <summary>
        /// Htmlのソースを取得
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetHtmlSource(string url)
        {
            string htmlSource = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    htmlSource = client.DownloadString(url);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("ソース取得に失敗しました。" + Environment.NewLine + url);
            }

            return _util.ChangeNewLineCodeLf2Crlf(htmlSource);
        }

        private void comboBox_LoadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            string loadFilePath = Path.Combine(_userDataFolder, comboBox_LoadSetting.Text);
            if (File.Exists(loadFilePath))
            {
                LoadProfile(loadFilePath);
            }
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(_util, _fio, comboBox_LoadSetting, _profileExtensions, SaveProfile, _userDataFolder);
        }
    }
}
