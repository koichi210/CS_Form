using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StandardTemplate;

namespace EventRecorder
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();
        private SaveRestore sr = new SaveRestore();

        // プレイリスト再生時、各行の設定ファイルを読み込む専用のインスタンス。
        // srを使い回すと各ファイルに埋め込まれたプレイリストのスナップショットで
        // 今操作中のプレイリストが上書きされてしまうため、記録データだけを登録した別インスタンスで分離する
        private SaveRestore playbackLoader = new SaveRestore();

        // 記録中/再生中フラグ。UIスレッド(ボタンクリック・グローバルホットキー)と
        // 再生用バックグラウンドスレッド(Task.Run側)の両方から読み書きされるため、
        // volatileでスレッド間の可視性を保証する(付けないと、最適化次第でバックグラウンド
        // スレッド側がstopPlayRequestedの変化に気づかないまま待機し続ける可能性があった)
        private volatile Boolean isRecording = false;
        private volatile Boolean isPlaying = false;
        private volatile Boolean stopPlayRequested = false;

        // タイトルバーに表示する再生中のループ進捗。「全体ループ」はプレイリストの
        // 全体周回(単発再生では常に1/1)、「ループ」はPlayRows呼び出し1回あたりの
        // 繰り返し(単発再生ならtextBox_Loopの回数、プレイリストなら各行のループ回数)。
        // 再生用の別スレッドから書き込み、UI側はUpdateTitleで読むだけなので
        // (単純なint代入・多少の表示タイミングのズレは許容)、特にロックはしていない
        private int playbackOverallLoopNo = 0;
        private int playbackOverallLoopMax = 0;
        private int playbackInnerLoopNo = 0;
        private int playbackInnerLoopMax = 0;

        // 直前のイベント時刻(記録の待機ms算出用)
        private int lastEventTick = 0;

        // 記録中/再生中にハイライトしている行のインデックス(-1ならハイライト無し)
        private int highlightedRowIndex = -1;

        // プレイリスト実行中にハイライトしている行(dataGridView_Playlist側)のインデックス
        private int highlightedPlaylistRowIndex = -1;

        // 再生開始時のマウスカーソル位置(再生終了後に戻すため)
        private Point cursorPositionBeforePlay;

        // 記録中に押下中のキー(OSのキーリピートによるKeyDown連発を抑制するため、
        // KeyUpが来るまで「押されている」とみなす)。記録セッションの開始のたびにクリアする
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();

        // ホットキー(F1=記録開始/停止、変換キー=再生開始/停止)の押下状態。
        // 記録セッションとは独立して管理する(pressedKeysをClearしても消えないように)
        private HashSet<Keys> pressedHotkeys = new HashSet<Keys>();

        // タイトルバーの基本文字列。記録中/再生中はここに状態を追記する
        private const String BaseTitle = "EventRecorder";

        // 記録/再生の切り替えホットキー(ボタンクリックだとクリック自体のマウスイベントが
        // 記録に混ざってしまうため、キー操作で完結できるようにしている)。
        // ユーザーが設定画面([[HotkeySettingsForm]]、システムメニューから開く)で変更でき、
        // 変更内容はEventRecorder.json(userDataFolder配下、ウィンドウサイズ等と共通の
        // アプリ設定ファイル)に保存される。既定値はHotkeyDefaults([[HotkeyDefaults.cs]])に
        // 集約してあり、設定画面の「初期値に戻す」も同じ値を参照する
        private Keys hotkeyToggleRecord = HotkeyDefaults.Record;
        private Keys hotkeyTogglePlay = HotkeyDefaults.Play;

        // キーバインド設定画面([[HotkeySettingsForm]])を開いている間だけtrueにする。
        // 開いている間はOnKeyboardEventの処理そのものを止め、テストで押したキーが
        // 記録/再生のホットキーとして誤発動しないようにする(ChangeHotkeys参照)
        private Boolean isHotkeySettingsOpen = false;

        // 記録したがまだDataGridViewに反映していない行(フックのコールバックを
        // 描画待ちで塞がないよう、一旦ここに貯めてタイマーでまとめて反映する)
        private List<String[]> pendingRows = new List<String[]>();
        private System.Windows.Forms.Timer gridFlushTimer;

        // マウスカーソル座標の常時表示用タイマー
        private System.Windows.Forms.Timer mousePosTimer;

        // マクロ・プレイリストのユーザーデータ置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\EventRecorder\配下(既定)にあり、exe直下には
        // その場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common\UserDataLocation.cs]])
        private const String AppName = "EventRecorder";
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(AppName);

        // 最小化する瞬間、OSから一時的にクライアント領域が極小サイズのリサイズ通知が来ることがあり、
        // Anchor/Fillでの再レイアウトがその極小サイズを基準に確定してしまい、元に戻した時に
        // コントロールが重なる/見切れる不具合の原因になる。最小化中はレイアウト計算自体をスキップし、
        // 元のサイズに戻った時のOnSizeChangedで正しく再計算させる
        protected override void OnSizeChanged(EventArgs e)
        {
            // checkBox_MinimizeOnPlayがオンなら、最小化/復元のたびにタスクバー表示と
            // システムトレイ表示を切り替える(手動最小化・再生時の自動最小化のどちらでも働く)
            UpdateTaskbarVisibility();

            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            base.OnSizeChanged(e);
        }

        // checkBox_MinimizeOnPlay(実行時にウィンドウを最小化する)がオンかつ最小化中の時だけ、
        // タスクバーから消してシステムトレイアイコンを表示する。それ以外(オフ、または最小化
        // されていない)は通常通りタスクバーに表示しトレイアイコンは消す。OnSizeChanged(最小化/
        // 復元の切り替え時)とcheckBox_MinimizeOnPlay_CheckedChanged(最小化中にチェックを変えた時)の
        // 両方から呼ぶ
        private void UpdateTaskbarVisibility()
        {
            Boolean shouldHideFromTaskbar = checkBox_MinimizeOnPlay.Checked
                && this.WindowState == FormWindowState.Minimized;

            this.ShowInTaskbar = !shouldHideFromTaskbar;
            notifyIcon_Tray.Visible = shouldHideFromTaskbar;
        }

        private void checkBox_MinimizeOnPlay_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTaskbarVisibility();
        }

        // トレイアイコンのダブルクリック/右クリックメニュー「元に戻す」共通の復元処理
        private void RestoreFromTray()
        {
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void notifyIcon_Tray_DoubleClick(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void menuItem_TrayRestore_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void menuItem_TrayExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public Form1()
        {
            InitializeComponent();

            // ウィンドウサイズ+splitContainer_Mainの境界線位置+ホットキーを、前回終了時の
            // 状態(EventRecorder.json、無ければ既定値のまま)で復元する
            LoadAppSettings();

            this.Icon = Properties.Resources.EventRecorder;
            notifyIcon_Tray.Icon = Properties.Resources.EventRecorder;
            util.SetCurrentDirectory();

            // DataGridViewの標準実装は行追加のたびにチラつき/再描画コストが出やすいため、
            // ダブルバッファを有効化する(DoubleBufferedはprotectedなのでリフレクション経由)
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null,
                dataGridView_Events,
                new object[] { true });

            gridFlushTimer = new System.Windows.Forms.Timer();
            gridFlushTimer.Interval = 150;
            gridFlushTimer.Tick += (s, e) => FlushPendingRows();

            // マウスカーソルの座標を起動中ずっと表示しておく(記録/再生の状態と関係なく常時更新)
            mousePosTimer = new System.Windows.Forms.Timer();
            mousePosTimer.Interval = 100;
            mousePosTimer.Tick += (s, e) =>
            {
                Point p = Cursor.Position;
                label_MousePos.Text = "Mouse: " + p.X + ", " + p.Y;
            };
            mousePosTimer.Start();

            // キーボードフックはホットキー(F1/変換キー)監視のためアプリ起動中ずっと張っておく。
            // マウスフックは記録中だけでよいのでToggleRecording側で開始/停止する
            GlobalHook.KeyboardHook.AddEvent(OnKeyboardEvent);
            GlobalHook.KeyboardHook.Start();

            this.FormClosing += Form1_FormClosing;

            sr.RegistItem(this);
            playbackLoader.RegistItemForPlayback(this);

            // userDataFolder(exe直下ではない)配下のプロファイル一覧をコンボボックスに表示する。
            // 「デフォルトで読み込むプロファイル」という特別な予約ファイル名は用意しておらず、
            // 一覧に一致するものが無い(=""を渡す)とutil.SetComboBoxTextが一覧の先頭を選ぶ
            // 仕様になっているため、それがそのまま「起動時は一覧の先頭を読み込む」動作になる
            UpdateProfileListAll("");

            // プレイリストの設定ファイル列(col_PlaylistFile)は、comboBox_Profileと全く同じ
            // *.xml一覧を表示する。ファイルシステムへの問い合わせをcomboBox_Profile側の
            // 更新タイミングに一本化し、その結果(=キャッシュ)をそのままコピーするだけにする
            SyncPlaylistFileItems();

            // SyncPlaylistFileItemsがcol_PlaylistFile.Itemsを作り直す際、DataGridViewComboBoxCellの
            // 内部処理でセルの見た目がリセットされることがあるため、直後に再計算する
            // (LoadProfile内で一度計算済みだが、その後のSyncPlaylistFileItemsで上書きされてしまう)
            UpdatePlaylistMissingFileHighlights();

            // プレイリストが空のままだと使うたびに毎回「行の追加」を押す羽目になるため、
            // 起動時点で編集開始しやすいよう空行を2行用意しておく。
            // ただし、直前のUpdateProfileListAll("")が(一覧の先頭を選ぶことで)自動的に読み込んだ
            // プロファイルに、既にプレイリストの中身が入っていた場合は、その内容を優先する
            // (プルダウンが空=何も読み込まれなかった時だけ、無条件の空2行を追加する)
            if (dataGridView_Playlist.Rows.Count == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    AddPlaylistRow(i, isEnabled: false);
                }
            }

            // 起動時のデフォルトモードは「レコード」
            radioButton_Record.Checked = true;

            // 各グループボックス内のコントロールを触ったら、同じ名前(Record/Playback)の
            // ラジオボタンへ自動でモードを切り替える
            SetupGroupBoxRadioSync();
        }

        // グループボックス内のコントロールをクリックしたら、そのグループボックスと
        // 同じ名前のラジオボタンをCheckedにする(Record⇔Playbackのモード切り替え)
        private void SetupGroupBoxRadioSync()
        {
            foreach (Control c in groupBox_Record.Controls)
            {
                c.Click += (s, e) => radioButton_Record.Checked = true;
            }
            foreach (Control c in groupBox_Playback.Controls)
            {
                c.Click += (s, e) => radioButton_Playback.Checked = true;
            }
        }

        // *******************************************************************************
        // データ保存先フォルダの変更(システムメニューから呼び出す)
        //
        // 保存先パスはそうそう変えるものではない設定なので、常時見えるボタンは置かず、
        // ウィンドウ左上のアイコンをクリックした時のシステムメニュー(最小化・最大化・閉じる等が
        // 並ぶメニュー)に項目を追加する形にした

        // システムメニューへの追加と「データ保存先を変更」は[[_Common/DataFolderMenu.cs]]に集約済み。
        // ここではEventRecorder独自の項目のIDだけ持つ(16の倍数かつ0xF000未満、0x1000は共通側が使用)
        private const int SysMenuId_ChangeHotkeys = 0x1010;

        // ウィンドウハンドルが確定したタイミングでシステムメニューに項目を追加する
        // (コンストラクタの時点ではまだthis.Handleが未確定のため、ここで行う)
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            DataFolderMenu.AppendToSystemMenu(this);
            DataFolderMenu.AppendMenuItem(this, SysMenuId_ChangeHotkeys, "キーバインドを設定(&K)...");
        }

        // Ctrl+Sでプロファイル保存(button_ProfileSave_Click)を呼ぶ。
        // テキストボックス等にフォーカスがあっても拾えるよう、個別のKeyDownではなくここで処理する
        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                button_ProfileSave_Click(this, EventArgs.Empty);
                return true;
            }

            if (keyData == (Keys.Control | Keys.F))
            {
                ShowFindReplaceDialog(FindReplaceMode.Find);
                return true;
            }

            if (keyData == (Keys.Control | Keys.H))
            {
                ShowFindReplaceDialog(FindReplaceMode.Replace);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Ctrl+F/Ctrl+Hのダイアログ(検索・置換は1つのダイアログをラジオボタンで切り替える)は、
        // 記録データ(dataGridView_Events)を編集/参照している最中にだけ意味があるので、
        // 既に開いていれば作り直さずモードだけ切り替えて前面に出す(重複オープン防止)
        private FindReplaceForm findReplaceForm;

        private void ShowFindReplaceDialog(FindReplaceMode mode)
        {
            if (isRecording || isPlaying)
            {
                return;
            }

            if (findReplaceForm == null || findReplaceForm.IsDisposed)
            {
                String initialText = (mode == FindReplaceMode.Find && dataGridView_Events.CurrentCell != null)
                    ? Convert.ToString(dataGridView_Events.CurrentCell.Value)
                    : "";
                findReplaceForm = new FindReplaceForm(dataGridView_Events, mode, initialText);
                findReplaceForm.Show(this);
            }
            else
            {
                findReplaceForm.SetMode(mode);
                findReplaceForm.Activate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (DataFolderMenu.IsChangeDataFolderCommand(m))
            {
                ChangeDataFolder();
                return;
            }
            if (DataFolderMenu.IsSysCommand(m, SysMenuId_ChangeHotkeys))
            {
                ChangeHotkeys();
                return;
            }

            base.WndProc(ref m);
        }

        // キーバインド設定画面([[HotkeySettingsForm]])を開き、「保存」で閉じられたら
        // 現在有効なホットキーとEventRecorder.jsonの両方を更新する
        private void ChangeHotkeys()
        {
            if (isRecording || isPlaying)
            {
                MessageBox.Show(
                    "記録中/再生中は変更できないよ。停止してから試してね",
                    "EventRecorder - キーバインド設定",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 設定画面を開いている間は、テキストボックスに試しに入力したキーが
            // グローバルフック(OnKeyboardEvent)側にも同時に届いて、記録/再生の
            // ホットキーとして誤発動してしまう(ダイアログはモーダルでも、低レベルの
            // キーボードフックはウィンドウのフォーカスと無関係に効き続けるため)。
            // isHotkeySettingsOpen中はOnKeyboardEventの先頭で処理そのものを止めてこれを防ぐ
            isHotkeySettingsOpen = true;
            try
            {
                using (HotkeySettingsForm form = new HotkeySettingsForm(hotkeyToggleRecord, hotkeyTogglePlay))
                {
                    if (form.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    hotkeyToggleRecord = form.RecordHotkey;
                    hotkeyTogglePlay = form.PlayHotkey;

                    if (!SaveAppSettings())
                    {
                        MessageBox.Show(
                            "キーバインド設定の保存に失敗したよ。今回のセッションだけは新しい設定のまま動くよ",
                            "EventRecorder - キーバインド設定",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            }
            finally
            {
                isHotkeySettingsOpen = false;
            }
        }

        // 保存先フォルダを選び直し、exe直下のポインタファイル(DataFolder.txt)を書き換える。
        // 実行中のuserDataFolder(readonly)はその場では切り替えない
        // (記録中のプレイリスト等、今のセッションの状態と食い違うと事故のもとになるため)。
        // 変更は次回起動時から反映される、シンプルで安全な方式にしている
        private void ChangeDataFolder()
        {
            if (isRecording || isPlaying)
            {
                MessageBox.Show(
                    "記録中/再生中は変更できないよ。停止してから試してね",
                    AppName + " - データ保存先の変更",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // EventRecorder.json(アプリの設定ファイル、プロファイルではない)は引っ越し対象から外す
            DataFolderMenu.ChangeDataFolder(AppName, userDataFolder,
                (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, AppName, IsNonProfileSettingFile));
        }

        // 今選択中のモードのグループボックスだけ背景色をハイライトする。
        // ボタンをDisableにする方式は分かりにくいという理由でやめ、色分けだけにした
        // 実行中の行のハイライト(LightYellow)と被らないよう、別の色にしてある
        private static readonly Color ModeHighlightColor = Color.FromArgb(205, 255, 230);

        private void radioButton_Mode_CheckedChanged(object sender, EventArgs e)
        {
            groupBox_Playback.BackColor = radioButton_Playback.Checked ? ModeHighlightColor : SystemColors.Control;
            groupBox_Record.BackColor = radioButton_Record.Checked ? ModeHighlightColor : SystemColors.Control;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAppSettings();

            gridFlushTimer.Stop();
            mousePosTimer.Stop();
            GlobalHook.MouseHook.Stop();
            GlobalHook.KeyboardHook.Stop();
        }

        // アプリ本体の設定(ウィンドウサイズ+splitContainer_Mainの境界線位置+ホットキー)を
        // まとめて保存するファイル名。ツール名そのものにしてあるので、ユーザーがこの名前で
        // プロファイルを保存することはまず無い、という前提の名前(意図的な予約名)。
        // プロファイル(マクロ)一覧には出したくないので、UpdateProfileListAllで除外している
        private const String AppSettingsFileName = "EventRecorder.json";

        // userDataFolder直下にプロファイルと混在して置かれる、アプリ自体の設定ファイル
        // (EventRecorder.json)かどうかを判定する。プロファイル一覧(UpdateProfileListAll)や
        // 引っ越し(MoveExistingProfiles)で誤って対象にしてしまわないよう、両方から共通で参照する
        private static Boolean IsNonProfileSettingFile(String filePath)
        {
            String fileName = System.IO.Path.GetFileName(filePath);
            return String.Equals(fileName, AppSettingsFileName, StringComparison.OrdinalIgnoreCase);
        }

        // 起動時、前回終了時のウィンドウサイズ+境界線位置+ホットキーを復元する。保存ファイルが
        // 無い/壊れている場合は何もしない(Designer既定のサイズ・境界線位置、HotkeyDefaultsの
        // ままで動く)。
        // なお、以前はここでuserDataFolder直下の"EventRecorder.json/xml"を「起動時デフォルトで
        // 読み込むプロファイル」として特別扱いする仕組みがあったが、アプリ設定ファイル自体に
        // 同じ名前(EventRecorder.json)を使うことにしたため廃止した。今後、起動時に読み込まれる
        // プロファイルは常に「プルダウン一覧の先頭」になる(コンストラクタのUpdateProfileListAll
        // 呼び出し側のコメント参照)
        private void LoadAppSettings()
        {
            String path = System.IO.Path.Combine(userDataFolder, AppSettingsFileName);

            AppSettings settings;
            try
            {
                settings = JsonFileStorage.Load<AppSettings>(path);
            }
            catch (Exception)
            {
                return;
            }

            if (settings == null)
            {
                return;
            }

            if (settings.Width > 0 && settings.Height > 0)
            {
                // MinimumSizeより小さい値が保存されていてもWinForms側で自動的に補正される
                this.Size = new Size(settings.Width, settings.Height);
            }

            if (settings.SplitterDistance > 0)
            {
                try
                {
                    splitContainer_Main.SplitterDistance = settings.SplitterDistance;
                }
                catch (ArgumentException)
                {
                    // 保存時と画面サイズが大きく変わった等で範囲外になった場合は、
                    // 境界線位置だけDesigner既定のまま諦める(ウィンドウサイズの復元は活かす)
                }
            }

            if (settings.RecordHotkey != Keys.None)
            {
                hotkeyToggleRecord = settings.RecordHotkey;
            }

            if (settings.PlayHotkey != Keys.None)
            {
                hotkeyTogglePlay = settings.PlayHotkey;
            }
        }

        // ウィンドウサイズ+境界線位置+ホットキーをまとめて保存する。終了時と、
        // キーバインド設定画面で「保存」した直後の両方から呼ばれる。
        // 保存に失敗したかどうかをBooleanで返す(呼び出し側でエラー表示するかどうかを決められるように。
        // 終了処理中は落としたくないので握りつぶすが、設定画面からの保存では失敗をユーザーに知らせたい)
        private Boolean SaveAppSettings()
        {
            // 最大化中はthis.Sizeが画面いっぱいのサイズになってしまうので、
            // 最大化前の通常サイズ(RestoreBounds)を保存する
            Size sizeToSave = (this.WindowState == FormWindowState.Normal) ? this.Size : this.RestoreBounds.Size;

            AppSettings settings = new AppSettings
            {
                Width = sizeToSave.Width,
                Height = sizeToSave.Height,
                SplitterDistance = splitContainer_Main.SplitterDistance,
                RecordHotkey = hotkeyToggleRecord,
                PlayHotkey = hotkeyTogglePlay,
            };

            String path = System.IO.Path.Combine(userDataFolder, AppSettingsFileName);
            try
            {
                JsonFileStorage.Save(path, settings);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
