using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using StandardTemplate;

namespace EventRecorder
{
    public partial class Form1 : Form
    {
        private StcUtils _util = new StcUtils();
        private SaveRestore _sr = new SaveRestore();

        // プレイリスト再生時、各行の設定ファイルを読み込む専用のインスタンス。
        // _srを使い回すと各ファイルに埋め込まれたプレイリストのスナップショットで
        // 今操作中のプレイリストが上書きされてしまうため、記録データだけを登録した別インスタンスで分離する
        private SaveRestore _playbackLoader = new SaveRestore();

        // 記録中/再生中フラグ。UIスレッド(ボタンクリック・グローバルホットキー)と
        // 再生用バックグラウンドスレッド(Task.Run側)の両方から読み書きされるため、
        // volatileでスレッド間の可視性を保証する(付けないと、最適化次第でバックグラウンド
        // スレッド側が_stopPlayRequestedの変化に気づかないまま待機し続ける可能性があった)
        private volatile Boolean _isRecording;
        private volatile Boolean _isPlaying;
        private volatile Boolean _stopPlayRequested;

        // 実行中の再生スレッド(単発再生/プレイリスト実行のTask.Run)。終了時に停止を伝えた後、
        // 本当に止まるまで待つために持っておく(止まる前にフォームを破棄すると、再生スレッドが
        // 閉店後の店内で動き回る=破棄済みのフォームを触ったり入力を送り続けたりしてしまう)
        private Task _playbackTask;

        // 終了処理(Form1_FormClosing)に入ったらtrue。以後は記録/再生を新しく始めない
        private Boolean _isExiting;

        // タイトルバーに表示する再生中のループ進捗。「全体ループ」はプレイリストの
        // 全体周回(単発再生では常に1/1)、「ループ」はPlayRows呼び出し1回あたりの
        // 繰り返し(単発再生ならtextBox_Loopの回数、プレイリストなら各行のループ回数)。
        // 再生用の別スレッドから書き込み、UI側はUpdateTitleで読むだけなので
        // (単純なint代入・多少の表示タイミングのズレは許容)、特にロックはしていない
        private int _playbackOverallLoopNo;
        private int _playbackOverallLoopMax;
        private int _playbackInnerLoopNo;
        private int _playbackInnerLoopMax;

        // 直前のイベント時刻(記録の待機ms算出用)
        private int _lastEventTick;

        // 記録中/再生中にハイライトしている行のインデックス(-1ならハイライト無し)
        private int _highlightedEventRowIndex = -1;

        // プレイリスト実行中にハイライトしている行(dataGridView_Playlist側)のインデックス
        private int _highlightedPlaylistRowIndex = -1;

        // 再生開始時のマウスカーソル位置(再生終了後に戻すため)
        private Point _cursorPositionBeforePlay;

        // 記録中に押下中のキー(OSのキーリピートによるKeyDown連発を抑制するため、
        // KeyUpが来るまで「押されている」とみなす)。記録セッションの開始のたびにクリアする
        private HashSet<Keys> _pressedKeys = new HashSet<Keys>();

        // ホットキー(F1=記録開始/停止、変換キー=再生開始/停止)の押下状態。
        // 記録セッションとは独立して管理する(_pressedKeysをClearしても消えないように)
        private HashSet<Keys> _pressedHotkeys = new HashSet<Keys>();

        // タイトルバーの基本文字列。記録中/再生中はここに状態を追記する
        private const String _baseTitle = "EventRecorder";

        // 記録/再生の切り替えホットキー(ボタンクリックだとクリック自体のマウスイベントが
        // 記録に混ざってしまうため、キー操作で完結できるようにしている)。
        // ユーザーが設定画面([[HotkeySettingsForm]]、システムメニューから開く)で変更でき、
        // 変更内容はEventRecorder.json(_userDataFolder配下、ウィンドウサイズ等と共通の
        // アプリ設定ファイル)に保存される。既定値はHotkeyDefaults([[HotkeyDefaults.cs]])に
        // 集約してあり、設定画面の「初期値に戻す」も同じ値を参照する
        private Keys _hotkeyToggleRecord = HotkeyDefaults.Record;
        private Keys _hotkeyTogglePlay = HotkeyDefaults.Play;

        // キーバインド設定画面([[HotkeySettingsForm]])を開いている間だけtrueにする。
        // 開いている間はOnKeyboardEventの処理そのものを止め、テストで押したキーが
        // 記録/再生のホットキーとして誤発動しないようにする(ChangeHotkeys参照)
        private Boolean _isHotkeySettingsOpen;

        // 記録したがまだDataGridViewに反映していない行(フックのコールバックを
        // 描画待ちで塞がないよう、一旦ここに貯めてタイマーでまとめて反映する)
        private List<String[]> _pendingRows = new List<String[]>();
        private System.Windows.Forms.Timer _gridFlushTimer;

        // マウスカーソル座標の常時表示用タイマー
        private System.Windows.Forms.Timer _mousePosTimer;

        // マクロ・プレイリストのユーザーデータ置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\EventRecorder\配下(既定)にあり、exe直下には
        // その場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common\UserDataLocation.cs]])
        private const String _appName = "EventRecorder";
        private readonly String _userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(_appName);

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
            BringWindowToFront();
        }

        // 再生中は再生対象のアプリが前面(フォアグラウンド)にいるため、WindowStateを戻すだけだと
        // Windowsのフォアグラウンドロックにより対象アプリの裏に隠れたままになり、戻ったことに気付けない。
        // 一瞬だけ最前面にしてZオーダーの先頭へ持ってきてから、アクティブにする
        private void BringWindowToFront()
        {
            this.TopMost = true;
            this.TopMost = false;
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

            InitializePlaceholders();
            InitializeToolTips();

            // ウィンドウサイズ+splitContainer_Mainの境界線位置+ホットキーを、前回終了時の
            // 状態(EventRecorder.json、無ければ既定値のまま)で復元する
            LoadAppSettings();

            this.Icon = Properties.Resources.EventRecorder;
            notifyIcon_Tray.Icon = Properties.Resources.EventRecorder;
            _util.SetCurrentDirectory();

            // DataGridViewの標準実装は行追加のたびにチラつき/再描画コストが出やすいため、
            // ダブルバッファを有効化する(DoubleBufferedはprotectedなのでリフレクション経由)
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null,
                dataGridView_Events,
                new object[] { true });

            _gridFlushTimer = new System.Windows.Forms.Timer();
            _gridFlushTimer.Interval = 150;
            _gridFlushTimer.Tick += (s, e) => FlushPendingRows();

            // マウスカーソルの座標を起動中ずっと表示しておく(記録/再生の状態と関係なく常時更新)
            _mousePosTimer = new System.Windows.Forms.Timer();
            _mousePosTimer.Interval = 100;
            _mousePosTimer.Tick += (s, e) =>
            {
                Point p = Cursor.Position;
                label_MousePos.Text = "Mouse: " + p.X + ", " + p.Y;
            };
            _mousePosTimer.Start();

            // キーボードフックはホットキー(F1/変換キー)監視のためアプリ起動中ずっと張っておく。
            // マウスフックは記録中だけでよいのでToggleRecording側で開始/停止する
            GlobalHook.KeyboardHook.AddEvent(OnKeyboardEvent);
            GlobalHook.KeyboardHook.Start();

            this.FormClosing += Form1_FormClosing;

            // 画面ロック・サインイン(セッション切替)の通知。SystemEventsは静的イベントなので、
            // フォームが破棄された後に呼ばれないよう、閉じた時に必ず解除する
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            this.FormClosed += (s, e) => SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;

            _sr.RegisterItem(this);
            _playbackLoader.RegisterItemForPlayback(this);

            // _userDataFolder(exe直下ではない)配下のプロファイル一覧をコンボボックスに表示する。
            // 「デフォルトで読み込むプロファイル」という特別な予約ファイル名は用意しておらず、
            // 一覧に一致するものが無い(=""を渡す)と_util.SetComboBoxTextが一覧の先頭を選ぶ
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

            // 起動時のデフォルトモードは「レコード」。ただし直前のUpdateProfileListAll("")で
            // プロファイルが読み込まれていれば、そのプロファイルのIsRecordModeが既に
            // radioButton_Record/Playback.Checkedへ反映済み(Form1.Profile.cs LoadProfileFromJson)
            // なので、ここで無条件に上書きするとプレイリスト読込直後にモードが「レコード」へ
            // 戻ってしまう。プロファイルが1つも無い(=何も読み込まれなかった)時だけ適用する
            if (comboBox_Profile.Items.Count == 0)
            {
                radioButton_Record.Checked = true;
            }

            // 各グループボックス内のコントロールを触ったら、同じ名前(Record/Playback)の
            // ラジオボタンへ自動でモードを切り替える
            SetupGroupBoxRadioSync();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // キーバインド設定画面(HotkeySettingsForm)の欄は、押したキーを読み取って表示する
        // ReadOnlyの欄なので対象外
        private void InitializePlaceholders()
        {
            textBox_Loop.PlaceholderText = "例: 1";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている。
        // ホットキーはLoadAppSettings(この後)やキーバインド設定画面で変わりうるため、
        // 実際のキー名ではなく既定値として書いている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            const String hotkeySettingTip = "キーはウィンドウ左上のアイコンのメニュー「キーバインドを設定」で変更できる";

            toolTip.SetToolTip(radioButton_Record, "再生ボタン(ホットキー)で、左の記録データを再生するモード");
            toolTip.SetToolTip(radioButton_Playback, "再生ボタン(ホットキー)で、右のプレイリストを上から順に再生するモード");
            toolTip.SetToolTip(button_Record, "記録の開始/停止。既存の行の後ろに追記する。ホットキー(既定F1)でも切り替えられ、ボタンで止めるとそのクリックも記録される。" + hotkeySettingTip);
            toolTip.SetToolTip(button_Play, "レコードモードなら記録データ、プレイバックモードならプレイリストを再生する。再生中に押すと停止。ホットキー(既定は変換キー)でも開始/停止できる。" + hotkeySettingTip);
            toolTip.SetToolTip(textBox_Loop, "レコードモード: 記録データを繰り返す回数。プレイバックモード: プレイリスト全体を繰り返す回数(各ファイルの回数は表のループ数)。空欄・0以下は1回。↑↓キーで増減できる");
            toolTip.SetToolTip(checkBox_MinimizeOnPlay, "再生開始時にこのウィンドウを最小化し、終了したら元に戻す。オンの間は最小化中にタスクバーから消え、通知領域のアイコン(ダブルクリックで復元)になる");
            toolTip.SetToolTip(comboBox_Profile, "選ぶとそのファイル(記録データ・ループ数・プレイリスト・モード等)を読み込み、今の表の内容を置き換える");
            toolTip.SetToolTip(button_ProfileSave, "記録データ・ループ数・プレイリスト・モード等をまとめて1つのファイルに保存する(Ctrl+Sでも可)。プルダウンで選択中なら、まずそのファイルへ上書きするか確認する");

            col_Type.ToolTipText = "操作の種類(LEFT_DOWN、KEY_DOWN等)。WAIT_MSは待機するだけの行";
            col_Detail.ToolTipText = "マウスは「X:123 Y:456」(画面座標)、キーは「Key:A」、WAIT_MS行は待機時間(ミリ秒)。直接編集できる";
            col_Remarks.ToolTipText = "自由に書けるメモ。記録・再生には使わない";

            col_PlaylistEnabled.ToolTipText = "チェックした行だけ再生する";
            col_PlaylistFile.ToolTipText = "再生するファイル(プロファイル一覧と同じもの)。見つからないファイルはピンクで表示する。行はドラッグで並び替えできる";
            col_PlaylistLoopCount.ToolTipText = "このファイルを続けて再生する回数。ファイル自身に保存されたループ数より優先する(ファイルを選んだ時に初期値として入る)。↑↓キーで増減できる";

            menuItem_DeleteRow.ToolTipText = "KEY_DOWN/SYSKEY_DOWNの行を消すと、対応するKEY_UP/SYSKEY_UPの行も一緒に消える";
            menuItem_BulkChangeEventWait.ToolTipText = "指定したイベントの直前にあるWAIT_MS行の待機時間をまとめて変更する";
            menuItem_PlaylistRefresh.ToolTipText = "プロファイル一覧にあってプレイリストに無いファイルを末尾に追加する(既存の行はそのまま残す)";
        }

        // グループボックス自体(枠・余白部分)や、グループボックス内のコントロールをクリックしたら、
        // そのグループボックスと同じ名前のラジオボタンをCheckedにする(Record⇔Playbackのモード切り替え)
        private void SetupGroupBoxRadioSync()
        {
            CheckRadioOnClick(groupBox_Record, radioButton_Record);
            CheckRadioOnClick(groupBox_Playback, radioButton_Playback);
        }

        private static void CheckRadioOnClick(GroupBox groupBox, RadioButton radioButton)
        {
            EventHandler check = (s, e) => radioButton.Checked = true;
            groupBox.Click += check;
            foreach (Control c in groupBox.Controls)
            {
                c.Click += check;
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
        private const int _sysMenuIdChangeHotkeys = 0x1010;

        // ウィンドウハンドルが確定したタイミングでシステムメニューに項目を追加する
        // (コンストラクタの時点ではまだthis.Handleが未確定のため、ここで行う)
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            DataFolderMenu.AppendToSystemMenu(this);
            DataFolderMenu.AppendMenuItem(this, _sysMenuIdChangeHotkeys, "キーバインドを設定(&K)...");
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
        private FindReplaceForm _findReplaceForm;

        private void ShowFindReplaceDialog(FindReplaceMode mode)
        {
            if (_isRecording || _isPlaying)
            {
                return;
            }

            if (_findReplaceForm == null || _findReplaceForm.IsDisposed)
            {
                String initialText = (mode == FindReplaceMode.Find && dataGridView_Events.CurrentCell != null)
                    ? Convert.ToString(dataGridView_Events.CurrentCell.Value)
                    : "";
                _findReplaceForm = new FindReplaceForm(dataGridView_Events, mode, initialText);
                _findReplaceForm.Show(this);
            }
            else
            {
                _findReplaceForm.SetMode(mode);
                _findReplaceForm.Activate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (DataFolderMenu.IsChangeDataFolderCommand(m))
            {
                ChangeDataFolder();
                return;
            }
            if (DataFolderMenu.IsSysCommand(m, _sysMenuIdChangeHotkeys))
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
            if (_isRecording || _isPlaying)
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
            // _isHotkeySettingsOpen中はOnKeyboardEventの先頭で処理そのものを止めてこれを防ぐ
            _isHotkeySettingsOpen = true;
            try
            {
                using (HotkeySettingsForm form = new HotkeySettingsForm(_hotkeyToggleRecord, _hotkeyTogglePlay))
                {
                    if (form.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    _hotkeyToggleRecord = form.RecordHotkey;
                    _hotkeyTogglePlay = form.PlayHotkey;

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
                _isHotkeySettingsOpen = false;
            }
        }

        // 保存先フォルダを選び直し、exe直下のポインタファイル(DataFolder.txt)を書き換える。
        // 実行中の_userDataFolder(readonly)はその場では切り替えない
        // (記録中のプレイリスト等、今のセッションの状態と食い違うと事故のもとになるため)。
        // 変更は次回起動時から反映される、シンプルで安全な方式にしている
        private void ChangeDataFolder()
        {
            if (_isRecording || _isPlaying)
            {
                MessageBox.Show(
                    "記録中/再生中は変更できないよ。停止してから試してね",
                    _appName + " - データ保存先の変更",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // EventRecorder.json(アプリの設定ファイル、プロファイルではない)は引っ越し対象から外す
            DataFolderMenu.ChangeDataFolder(_appName, _userDataFolder,
                (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, _appName, IsAppSettingsFile));
        }

        // 今選択中のモードのグループボックスだけ背景色をハイライトする。
        // ボタンをDisableにする方式は分かりにくいという理由でやめ、色分けだけにした
        // 実行中の行のハイライト(LightYellow)と被らないよう、別の色にしてある
        private static readonly Color _modeHighlightColor = Color.FromArgb(205, 255, 230);

        private void radioButton_Mode_CheckedChanged(object sender, EventArgs e)
        {
            groupBox_Playback.BackColor = radioButton_Playback.Checked ? _modeHighlightColor : SystemColors.Control;
            groupBox_Record.BackColor = radioButton_Record.Checked ? _modeHighlightColor : SystemColors.Control;
        }

        // 閉じるボタン・トレイの「終了」だけでなく、Windowsのシャットダウン/再起動/サインアウト
        // (CloseReason.WindowsShutDown)の時もここを通る。
        // 以前は記録/再生を止めずにフォームを閉じていたため、再生スレッドがフォーム破棄後も
        // 入力を送り続けたり、破棄済みのフォームへInvokeしたりしていた。
        // 「店じまい」の順番: ①ホットキーを受け付けない ②記録を止める ③再生に停止を伝えて止まるまで待つ
        // ④設定を保存 ⑤タイマー・フックを片付ける
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isExiting)
            {
                // 下の「再生の停止待ち」の最中(メッセージ処理を回している間)に閉じる操作がもう一度来た場合。
                // 1回目の終了処理がまだ途中なので、2回目は受け流して1回目に任せる
                e.Cancel = true;
                return;
            }
            _isExiting = true;

            // ①止めている最中にホットキーで記録/再生が再開されないよう、先にキーボードフックを外す
            GlobalHook.KeyboardHook.Stop();

            // ②記録中なら、溜まっている行を表へ反映してから止める(マウスフックもここで外れる)
            if (_isRecording)
            {
                ToggleRecording();
            }

            // ③再生中なら停止を伝え、再生スレッドが後片付け(押しっぱなしのキーを離す等)を終えるまで待つ
            if (_isPlaying)
            {
                _stopPlayRequested = true;
                Task task = _playbackTask;
                if (task != null)
                {
                    SessionGuard.WaitWhilePumping(() => task.IsCompleted, Application.DoEvents, SessionGuard.ExitWaitTimeoutMs);
                }
            }

            // ④
            SaveAppSettings();

            // ⑤
            _gridFlushTimer.Stop();
            _mousePosTimer.Stop();
            GlobalHook.MouseHook.Stop();
            GlobalHook.KeyboardHook.Stop();
        }

        // *******************************************************************************
        // 画面ロック・サインイン(セッション切替)

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (_isExiting || IsDisposed || !IsHandleCreated)
            {
                return;
            }

            // 通常はUIスレッドで通知されるが、念のため別スレッドから来た場合はUIスレッドへ回す
            if (InvokeRequired)
            {
                SessionSwitchReason reason = e.Reason;
                BeginInvoke((MethodInvoker)(() => HandleSessionChange(SessionGuard.Classify(reason))));
                return;
            }

            HandleSessionChange(SessionGuard.Classify(e.Reason));
        }

        private void HandleSessionChange(SessionChange change)
        {
            if (_isExiting || IsDisposed)
            {
                return;
            }

            switch (change)
            {
                case SessionChange.Suspend:
                    SuspendForSessionLock();
                    break;
                case SessionChange.Resume:
                    ResumeAfterSessionUnlock();
                    break;
            }
        }

        // 画面ロック等でこのセッションの画面から離れる時、記録/再生を止める。
        // ・再生: ロック中のSendInputはロック画面に届かず空振りする上、ロック解除した瞬間に
        //   途中の行から勝手に再開してしまうため、ここで停止する(押しっぱなしのキーは再生スレッドが離す)
        // ・記録: ロック中は入力がフックに来ず、ロック操作(Win+L等)のKeyUpも取りこぼして
        //   「押しっぱなし」扱いのキーが残るため、ここで記録を終える(記録済みの行はそのまま残る)
        private void SuspendForSessionLock()
        {
            if (_isRecording)
            {
                ToggleRecording();
            }

            if (_isPlaying)
            {
                _stopPlayRequested = true;
            }

            _pressedHotkeys.Clear();
        }

        // ロック解除・サインインでこのセッションの画面に戻ってきた時の立て直し。
        // ・ロック直前に押したホットキーのKeyUpを取りこぼしていると、次の1回が「リピート」扱いで無視されるため押下状態を消す
        // ・低レベルキーボードフックは、コールバックが一定時間内に戻らないとWindowsに黙って外される
        //   (ロック解除直後は画面の再描画などでUIスレッドが詰まりやすい)。外れたかどうかは知る手段が無いので、
        //   張り直してホットキーが効かなくなるのを防ぐ
        private void ResumeAfterSessionUnlock()
        {
            _pressedHotkeys.Clear();

            try
            {
                GlobalHook.KeyboardHook.Stop();
                GlobalHook.KeyboardHook.ClearEvent();
                GlobalHook.KeyboardHook.AddEvent(OnKeyboardEvent);
                GlobalHook.KeyboardHook.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // 張り直しに失敗してもエラー表示はしない(ロック解除のたびにダイアログが出るのを避ける)。
                // ホットキーは効かなくなるが、ボタン操作は使えるし、次のロック解除でもう一度張り直しを試みる
            }
        }

        // アプリ本体の設定(ウィンドウサイズ+splitContainer_Mainの境界線位置+ホットキー)を
        // まとめて保存するファイル名。ツール名そのものにしてあるので、ユーザーがこの名前で
        // プロファイルを保存することはまず無い、という前提の名前(意図的な予約名)。
        // プロファイル(マクロ)一覧には出したくないので、UpdateProfileListAllで除外している
        private const String _appSettingsFileName = "EventRecorder.json";

        // _userDataFolder直下にプロファイルと混在して置かれる、アプリ自体の設定ファイル
        // (EventRecorder.json)かどうかを判定する。プロファイル一覧(UpdateProfileListAll)や
        // 引っ越し(DataFolderMenu.MoveProfiles)で誤って対象にしてしまわないよう、両方から共通で参照する
        private static Boolean IsAppSettingsFile(String filePath)
        {
            String fileName = System.IO.Path.GetFileName(filePath);
            return String.Equals(fileName, _appSettingsFileName, StringComparison.OrdinalIgnoreCase);
        }

        // EventRecorder.jsonのフルパス(読込/保存で共通)
        private String AppSettingsFilePath
        {
            get { return System.IO.Path.Combine(_userDataFolder, _appSettingsFileName); }
        }

        // 起動時、前回終了時のウィンドウサイズ+境界線位置+ホットキーを復元する。保存ファイルが
        // 無い/壊れている場合は何もしない(Designer既定のサイズ・境界線位置、HotkeyDefaultsの
        // ままで動く)。
        // なお、以前はここで_userDataFolder直下の"EventRecorder.json/xml"を「起動時デフォルトで
        // 読み込むプロファイル」として特別扱いする仕組みがあったが、アプリ設定ファイル自体に
        // 同じ名前(EventRecorder.json)を使うことにしたため廃止した。今後、起動時に読み込まれる
        // プロファイルは常に「プルダウン一覧の先頭」になる(コンストラクタのUpdateProfileListAll
        // 呼び出し側のコメント参照)
        private void LoadAppSettings()
        {
            String path = AppSettingsFilePath;

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
                _hotkeyToggleRecord = settings.RecordHotkey;
            }

            if (settings.PlayHotkey != Keys.None)
            {
                _hotkeyTogglePlay = settings.PlayHotkey;
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
                RecordHotkey = _hotkeyToggleRecord,
                PlayHotkey = _hotkeyTogglePlay,
            };

            String path = AppSettingsFilePath;
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
