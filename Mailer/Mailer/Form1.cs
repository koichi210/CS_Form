using System;
using System.IO;
using System.Windows.Forms;
using StandardTemplate;

namespace Mailer
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        readonly String SettingFileName = @"Mailer.json";

        // 設定ファイルはJSONが基本。旧XML(Mailer.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"Mailer.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        // プロファイルの置き場。exe直下(bin/Debug、bin/Release)はビルド出力の掃除等で
        // 丸ごと消される事故が起きうるため、そこには置かない。実データは%LOCALAPPDATA%\Mailer\配下
        // (既定)にあり、exe直下にはその場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く
        // 2段構成にしてある([[_Common/UserDataLocation.cs]]、Cheetos/FileArrangerと同じ仕組み)
        private const String AppName = "Mailer";
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(AppName);

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private Boolean LoadProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(sr, filePath) : sr.LoadProc(filePath);
        }

        private Boolean SaveProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(sr, filePath) : sr.SaveSetting(filePath);
        }

        readonly String MailUrl = @"https://mail.google.com/mail/?view=cm&fs=1";

        private StcFileInputOutput fio = new StcFileInputOutput();

        private ExecParam param = new ExecParam();

        class ExecParam
        {
            public int CreateNum = 0;
            public int IntervalMsec = 0;
            public DateTime UserDate;
        }

        public Form1()
        {
            InitializeComponent();
            InitializeCommonSettings(Properties.Resources.Mailer);

            InitializePlaceholders();
            InitializeToolTips();

            sr.RegisterItem(this);
            String defaultJsonPath = Path.Combine(userDataFolder, SettingFileName);
            String defaultXmlPath = Path.Combine(userDataFolder, LegacySettingFileName);
            JsonSaveRestore.LoadWithMigration(sr, defaultJsonPath, defaultXmlPath,
                path => sr.LoadProc(path));
            util.UpdateProfileList(comboBox_LoadSetting, ProfileExtensions, "", userDataFolder);
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(textBox_MailBody、Windowsの仕様で表示されない)は対象外
        private void InitializePlaceholders()
        {
            textBox_BrowserPath.PlaceholderText = @"例: C:\Program Files\Google\Chrome\Application\chrome.exe";
            textBox_MailTo.PlaceholderText = "例: user@example.com";
            textBox_MailCc.PlaceholderText = "例: cc@example.com";
            textBox_MailBcc.PlaceholderText = "例: bcc@example.com";
            textBox_MailSubject.PlaceholderText = "例: 日報 %%today%%";

            textBox_CreateNum.PlaceholderText = "例: 5";
            textBox_IntervalMsec.PlaceholderText = "例: 5000";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_BrowserPath, "Gmailの作成画面を開くブラウザのexe。PATHが通っていればファイル名だけでも可。Enterキーで起動する");
            toolTip.SetToolTip(textBox_MailSubject, "%%today%% %%tomorrow%% %%weekend%%(今週の金曜)は実行日、%%usersday%% %%dayofweek%%は起点日の日付・曜日に置き換わる。大文字で書くと年付きの日付/曜日の正式名になる");
            toolTip.SetToolTip(textBox_MailBody, "件名の置換記号(%%today%%等)は本文では置き換わらない");
            toolTip.SetToolTip(button_OpenBrowse, "入力内容を埋めたGmailの新規作成画面をブラウザで開く。送信まではしない");
            toolTip.SetToolTip(comboBox_LoadSetting, "保存済みのプロファイル。選ぶとその内容を読み込む");
            toolTip.SetToolTip(dateTimePicker_Calendar, "%%usersday%%と%%dayofweek%%の基準日。一括表示では1通目がこの日で、以降1日ずつ進む");
            toolTip.SetToolTip(textBox_CreateNum, "一括表示で開く通数(=日数)");
            toolTip.SetToolTip(textBox_IntervalMsec, "一括表示で1通開くごとに待つ時間。待っている間は画面を操作できない");
            toolTip.SetToolTip(check_BoxReverse, "オンなら一括表示を日付の遅い順に開く");
            toolTip.SetToolTip(button_OpenBrowse_OneWeek, "起点日から生成数の日数分、日付をずらした作成画面を順に開く");
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
                DataFolderMenu.ChangeDataFolder(AppName, userDataFolder,
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, AppName));
                return;
            }

            base.WndProc(ref m);
        }

        private void comboBox_LoadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            String loadFileName = Path.Combine(userDataFolder, comboBox_LoadSetting.Text);
            LoadProfile(loadFileName);
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            // プロファイル未選択のときは既定の設定ファイル名で保存ダイアログを出す
            if (comboBox_LoadSetting.Text == String.Empty)
            {
                comboBox_LoadSetting.Text = SettingFileName;
            }

            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_LoadSetting, ProfileExtensions, SaveProfile, userDataFolder);
        }
 
        private void button_OpenBrowse_Click(object sender, EventArgs e)
        {
            if (!TryReadUIParam())
            {
                return;
            }

            OpenBrowser();
        }

        private void button_OpenBrowse_OneWeek_Click(object sender, EventArgs e)
        {
            if (!TryReadUIParam())
            {
                return;
            }
            var dayOffsets = Logic.GetLoopList(param.CreateNum, check_BoxReverse.Checked);
            foreach (var dayOffset in dayOffsets)
            {
                OpenBrowser(dayOffset);
                System.Threading.Thread.Sleep(param.IntervalMsec);
            }
        }

        private void OpenBrowser(int daysOffset = 0)
        {
            String browseUrl = MailUrl;
            if (textBox_MailTo.Text != String.Empty)
            {
                browseUrl += "&to=" + textBox_MailTo.Text;
            }
            if (textBox_MailCc.Text != String.Empty)
            {
                browseUrl += "&cc=" + textBox_MailCc.Text;
            }
            if (textBox_MailBcc.Text != String.Empty)
            {
                browseUrl += "&bcc=" + textBox_MailBcc.Text;
            }

            if (textBox_MailSubject.Text != String.Empty)
            {
                DateTime userDate = param.UserDate.AddDays(daysOffset);
                String chromeFormatText = textBox_MailSubject.Text.Replace(" ", "+");
                browseUrl += "&su=" + Logic.GetReplaceDay(chromeFormatText, userDate);
            }

            if (textBox_MailBody.Text != String.Empty)
            {
                browseUrl += "&body=" + textBox_MailBody.Text.Replace("\r\n", "%0D%0A").Replace(" ", "+");
            }

            util.ExecuteProcess(textBox_BrowserPath.Text, browseUrl);
        }

        // 画面の入力値をparamへ読み込む。ブラウザが見つからなければfalse
        private Boolean TryReadUIParam()
        {
            if( !util.IsExistFileNameInEnvironment(textBox_BrowserPath.Text) )
            {
                MessageBox.Show("ファイルが存在しません" + Environment.NewLine + textBox_BrowserPath.Text);
                return false;
            }

            int.TryParse(textBox_CreateNum.Text, out param.CreateNum);
            int.TryParse(textBox_IntervalMsec.Text, out param.IntervalMsec);

            param.UserDate = new DateTime(
                dateTimePicker_Calendar.Value.Year,
                dateTimePicker_Calendar.Value.Month,
                dateTimePicker_Calendar.Value.Day,
                dateTimePicker_Calendar.Value.Hour,
                dateTimePicker_Calendar.Value.Minute,
                dateTimePicker_Calendar.Value.Second,
                0);
            return true;
        }

        private void textBox_BrowserPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(textBox_BrowserPath.Text, e);

        }

        private void button_Help_Click(object sender, EventArgs e)
        {
            var helpText = "USAGE:" + Environment.NewLine +
                "  %%today%% ・・・・        1/1" + Environment.NewLine +
                "  %%TODAY%% ・・・・   2024/1/1" + Environment.NewLine +
                "  %%tomorrow%% ・・・       1/2" + Environment.NewLine +
                "  %%TOMORROW%% ・・・  2024/1/2" + Environment.NewLine +
                "  %%weekend%% ・・・  (金曜日の日付）" + Environment.NewLine +
                "  %%WEEKEND%% ・・・  (金曜日の日付）" + Environment.NewLine +
                "  %%usersday%% ・・・       2/3 (select day)" + Environment.NewLine +
                "  %%USERSDAY%% ・・・  2024/2/3 (select day)" + Environment.NewLine +
                "  %%dayofweek%% ・・・ 月       (is selected 2024/1/1)" + Environment.NewLine +
                "  %%DAYOFWEEK%% ・・・ 月曜日   (is selected 2024/1/1)";
            MessageBox.Show(helpText);
        }
    }
}
