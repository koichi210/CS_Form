using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        // ウィンドウが閉じられたか。取得はUIスレッドの外で行うので、取得中に閉じられた後で
        // 結果が届いても、破棄済みの画面やクリップボードに触らないよう途中で打ち切るために見る
        private bool _isClosed = false;

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _isClosed = true;
            base.OnFormClosed(e);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            // 取得中はボタンを無効にしてあるが、念のため二重に走らせない
            if (!button_Execute.Enabled)
            {
                return;
            }

            int trimLineNum;
            if (!Logic.TryGetTrimLine(textBox_TrimLineNum.Text, out trimLineNum))
            {
                MessageBox.Show(this, Logic.TrimLineNumErrorMessage);
                textBox_TrimLineNum.Focus();
                textBox_TrimLineNum.SelectAll();
                return;
            }

            // 出力先をクリア
            textBox_DestList.Text = "";

            // 取得中も画面は操作できる(設定の読み込みで各欄が書き換わることもある)ので、
            // 処理に使う値は開始時点でまとめて読み取っておく
            TrimJob job = new TrimJob
            {
                Urls = Logic.SplitLines(textBox_SourceList.Text),
                SearchWord = textBox_SearchWord.Text,
                TrimLineNum = trimLineNum,
                Comparison = checkBox_OrdinalCase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase,
                FirstWordOnly = checkBox_FirstWordOnly.Checked,
                Debug = new StcDebug { IsDebugMode = _isDebug },
                UiScheduler = TaskScheduler.FromCurrentSynchronizationContext(),
            };

            SetFetching(true);
            FetchNext(job, 0);
        }

        // 実行1回分の入力値と途中経過
        private class TrimJob
        {
            public string[] Urls;
            public string SearchWord;
            public int TrimLineNum;
            public StringComparison Comparison;
            public bool FirstWordOnly;
            public StcDebug Debug;
            public TaskScheduler UiScheduler;

            // Text +=はURLごとにテキストボックス全体を作り直すため、StringBuilderに溜めて最後に1回だけセットする
            public readonly StringBuilder DestText = new StringBuilder();
        }

        // index番目のURLを取得する。以前はUIスレッドでそのまま取得していたため、取得が終わるまで画面が固まっていた。
        // .NET Framework 4.0なのでasync/awaitは使えず、取得だけをTaskで裏に回し、
        // 結果の処理は1件ずつUIスレッドに戻してから次のURLへ進む
        // (失敗時のメッセージをURLの順に1件ずつ出す、以前の挙動を保つため)
        private void FetchNext(TrimJob job, int index)
        {
            if (index >= job.Urls.Length)
            {
                FinishFetch(job);
                return;
            }

            string url = job.Urls[index];
            job.DestText.Append("◆").Append(url).Append(Environment.NewLine);
            this.Text = _appName + " - 取得中 (" + (index + 1) + "/" + job.Urls.Length + ")";

            Task.Factory.StartNew(() => DownloadHtmlSource(url))
                .ContinueWith(task =>
                {
                    if (_isClosed)
                    {
                        return;
                    }

                    string htmlSource = "";
                    if (task.IsFaulted)
                    {
                        // Exceptionを読んで「確認済み」にしておく(未確認のまま捨てられたTaskの例外として扱われないように)
                        Exception unused = task.Exception;
                        MessageBox.Show(this, "ソース取得に失敗しました。" + Environment.NewLine + url);

                        // メッセージを出している間に閉じられることもある
                        if (_isClosed)
                        {
                            return;
                        }
                    }
                    else
                    {
                        htmlSource = task.Result;
                    }

                    htmlSource = _util.ChangeNewLineCodeLf2Crlf(htmlSource);
                    job.Debug.WriteDataInNewFile(htmlSource, "_1_source");

                    string result = Logic.GetSearchString(htmlSource, job.SearchWord, job.TrimLineNum, job.Comparison, job.FirstWordOnly);
                    job.Debug.WriteDataInNewFile(result, "_2_search");

                    job.DestText.Append(result);

                    FetchNext(job, index + 1);
                }, CancellationToken.None, TaskContinuationOptions.None, job.UiScheduler);
        }

        private void FinishFetch(TrimJob job)
        {
            textBox_DestList.Text = job.DestText.ToString();
            SetFetching(false);

            _util.SetClipboardText(textBox_DestList.Text);
        }

        // 取得中は実行ボタンを押せなくし、ボタンとタイトルで取得中であることを示す
        private void SetFetching(bool isFetching)
        {
            button_Execute.Enabled = !isFetching;
            button_Execute.Text = isFetching ? "取得中..." : "実行";
            if (!isFetching)
            {
                this.Text = _appName;
            }
        }

        /// <summary>
        /// Htmlのソースを取得する(UIスレッドの外で呼ばれる)。失敗時は例外をそのまま投げ、呼び出し元がメッセージを出す
        /// </summary>
        private static string DownloadHtmlSource(string url)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(url);
            }
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
