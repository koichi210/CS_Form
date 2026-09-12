using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        // 記録中/再生中フラグ
        private Boolean isRecording = false;
        private Boolean isPlaying = false;
        private Boolean stopPlayRequested = false;

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

        // 記録開始/停止のホットキー。ボタンクリックだとクリック自体のマウスイベントが
        // 記録に混ざってしまうため、キー操作で完結できるようにしている
        private const Keys HotkeyToggleRecord = Keys.F1;

        // 再生開始/停止のホットキー。IME変換キー単独、またはShift+F2のどちらでも発動する
        private const Keys HotkeyTogglePlay = Keys.IMEConvert;
        private const Keys HotkeyTogglePlayAlt = Keys.F2;

        // 記録したがまだDataGridViewに反映していない行(フックのコールバックを
        // 描画待ちで塞がないよう、一旦ここに貯めてタイマーでまとめて反映する)
        private List<String[]> pendingRows = new List<String[]>();
        private System.Windows.Forms.Timer gridFlushTimer;

        // マウスカーソル座標の常時表示用タイマー
        private System.Windows.Forms.Timer mousePosTimer;

        // 起動時に自動読込するデフォルトの設定ファイル名(Cheetosに倣う)。
        // 今後はJSON保存が主流になっていく想定なので、JSON版が存在すればそちらを優先する
        private readonly String SettingFileNameXml = @"EventRecorder.xml";
        private readonly String SettingFileNameJson = @"EventRecorder.json";

        // マクロ・プレイリストのユーザーデータ置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\EventRecorder\配下(既定)にあり、exe直下には
        // その場所を示す小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common\UserDataLocation.cs]])
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder("EventRecorder");

        // 最小化する瞬間、OSから一時的にクライアント領域が極小サイズのリサイズ通知が来ることがあり、
        // Anchor/Fillでの再レイアウトがその極小サイズを基準に確定してしまい、元に戻した時に
        // コントロールが重なる/見切れる不具合の原因になる。最小化中はレイアウト計算自体をスキップし、
        // 元のサイズに戻った時のOnSizeChangedで正しく再計算させる
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            base.OnSizeChanged(e);
        }

        public Form1()
        {
            InitializeComponent();

            // ダイアログのサイズを前回終了時の状態で復元する(マクロのXMLとは別の、
            // .NET標準のユーザー設定ファイルに保存してある値)。MinimumSizeより小さい値が
            // 保存されていてもWinForms側で自動的に補正される
            this.Size = Properties.Settings.Default.WindowSize;

            this.Icon = Properties.Resources.EventRecorder;
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

            // 起動時にデフォルト設定を読み込み、コンボボックスに設定ファイル一覧を表示する(Cheetosと同じ手順)。
            // どちらもuserDataFolder(exe直下ではない)を対象にする
            String defaultJsonPath = System.IO.Path.Combine(userDataFolder, SettingFileNameJson);
            String defaultXmlPath = System.IO.Path.Combine(userDataFolder, SettingFileNameXml);
            String defaultSettingPath = System.IO.File.Exists(defaultJsonPath) ? defaultJsonPath : defaultXmlPath;
            LoadProfile(defaultSettingPath);
            UpdateProfileListAll("");

            // プレイリストの設定ファイル列(col_PlaylistFile)は、comboBox_Profileと全く同じ
            // *.xml一覧を表示する。ファイルシステムへの問い合わせをcomboBox_Profile側の
            // 更新タイミングに一本化し、その結果(=キャッシュ)をそのままコピーするだけにする
            SyncPlaylistFileItems();

            // プレイリストが空のままだと使うたびに毎回「行の追加」を押す羽目になるため、
            // 起動時点で編集開始しやすいよう空行を2行用意しておく。
            // ただし、直前のsr.LoadProc(SettingFileName, this)でデフォルト設定ファイルから
            // 既にプレイリストの中身が読み込まれていた場合は、その内容を優先する
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
            // 最大化中はthis.Sizeが画面いっぱいのサイズになってしまうので、
            // 最大化前の通常サイズ(RestoreBounds)を保存する
            Properties.Settings.Default.WindowSize =
                (this.WindowState == FormWindowState.Normal) ? this.Size : this.RestoreBounds.Size;
            Properties.Settings.Default.Save();

            gridFlushTimer.Stop();
            mousePosTimer.Stop();
            GlobalHook.MouseHook.Stop();
            GlobalHook.KeyboardHook.Stop();
        }

        // *******************************************************************************
        // 記録

        private void button_Record_Click(object sender, EventArgs e)
        {
            ToggleRecording();
        }

        // 現在の記録中/再生中の状態をタイトルバーに反映する
        private void UpdateTitle()
        {
            if (isRecording)
            {
                this.Text = BaseTitle + " - 記録中";
            }
            else if (isPlaying)
            {
                // (表示例)プレイバック中：2 / 5：34/100
                // 「2 / 5」=全体ループ(プレイリストの全体周回。単発再生では常に1/1)の今の実行数/最大数、
                // 「34/100」=ループ(PlayRows呼び出し1回あたりの繰り返し)の今の実行回数/最大数
                this.Text = BaseTitle + " - プレイバック中："
                    + playbackOverallLoopNo + " / " + playbackOverallLoopMax + "："
                    + playbackInnerLoopNo + "/" + playbackInnerLoopMax;
            }
            else
            {
                this.Text = BaseTitle;
            }
        }

        // 単発再生とプレイリスト実行は1つのbutton_Play(表示名「再生」)に統合したので、
        // isPlayingが変わるたびにその表示を同期させるだけでよい
        private void UpdatePlayButtons()
        {
            button_Play.Text = isPlaying ? "停止" : "再生";
        }

        // 記録開始/停止を切り替える。ボタンクリックからもF1ホットキーからも呼ばれる
        private void ToggleRecording()
        {
            if (isRecording)
            {
                GlobalHook.MouseHook.Stop();
                gridFlushTimer.Stop();
                FlushPendingRows();
                isRecording = false;
                button_Record.Text = "記録";
                UpdateTitle();
                return;
            }

            if (isPlaying)
            {
                return;
            }

            // 記録開始時に既存の行はクリアしない(既存のマクロに追記で記録したい場合があるため)
            pendingRows.Clear();
            pressedKeys.Clear();
            lastEventTick = Environment.TickCount;

            GlobalHook.MouseHook.AddEvent(OnMouseEvent);
            GlobalHook.MouseHook.Start();

            gridFlushTimer.Start();
            isRecording = true;
            button_Record.Text = "記録中…";
            UpdateTitle();
        }

        private void OnMouseEvent(ref GlobalHook.MouseHook.StateMouse s)
        {
            // カーソル移動そのものは記録しない(HiMacroExと同様、クリック単位のみ記録)
            if (s.Stroke == GlobalHook.MouseHook.Stroke.MOVE || s.Stroke == GlobalHook.MouseHook.Stroke.UNKNOWN)
            {
                return;
            }

            QueueRow(s.Stroke.ToString(), s.X.ToString(), s.Y.ToString(), "", ElapsedMs());
        }

        private void OnKeyboardEvent(ref GlobalHook.KeyboardHook.StateKeyboard s)
        {
            if (s.Stroke == GlobalHook.KeyboardHook.Stroke.UNKNOWN)
            {
                return;
            }

            Boolean isDown = s.Stroke == GlobalHook.KeyboardHook.Stroke.KEY_DOWN || s.Stroke == GlobalHook.KeyboardHook.Stroke.SYSKEY_DOWN;
            Boolean isUp = s.Stroke == GlobalHook.KeyboardHook.Stroke.KEY_UP || s.Stroke == GlobalHook.KeyboardHook.Stroke.SYSKEY_UP;

            // ホットキーはアプリ操作用の予約キーなので、マクロの記録データには含めない。
            // Shift併用が必要なもの(Shift+F2)は、押した瞬間にShiftが押されていた
            // 場合だけホットキー扱いにする。素のF2は本来の用途やマクロ記録にそのまま使える
            if (isDown)
            {
                Boolean isShiftDown = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                Boolean isRecordHotkey = s.Key == HotkeyToggleRecord;
                Boolean isPlayHotkey = s.Key == HotkeyTogglePlay
                    || (s.Key == HotkeyTogglePlayAlt && isShiftDown);

                if (isRecordHotkey || isPlayHotkey)
                {
                    // リピート抑制。対応するKeyUpが来るまでは連続トグルさせない
                    if (!pressedHotkeys.Add(s.Key))
                    {
                        return;
                    }

                    if (isRecordHotkey)
                    {
                        ToggleRecording();
                    }
                    else
                    {
                        PlayOrStop();
                    }

                    return;
                }
            }
            else if (isUp)
            {
                // Down時にホットキーとして処理したキーのUpは、マクロ記録に含めず消費する。
                // (Up時点で先にShiftを離していても、Down時の判定結果に合わせて対称に扱う)
                if (pressedHotkeys.Remove(s.Key))
                {
                    return;
                }
            }

            if (!isRecording)
            {
                return;
            }

            if (isDown)
            {
                // すでに押されている状態なら、OSのキーリピートによるKeyDown連発なので無視する。
                // 対応するKeyUpが来るまでは「押されたまま」として扱う
                if (!pressedKeys.Add(s.Key))
                {
                    return;
                }
            }
            else if (isUp)
            {
                pressedKeys.Remove(s.Key);
            }

            QueueRow(s.Stroke.ToString(), "", "", s.Key.ToString(), ElapsedMs());
        }

        // 直前のイベントからの経過ms。記録開始直後の1件目は「記録開始からの待機」になる
        private int ElapsedMs()
        {
            int now = Environment.TickCount;
            int wait = now - lastEventTick;
            lastEventTick = now;
            return wait < 0 ? 0 : wait;
        }

        // 記録した1件をバッファに貯めるだけ(フックのコールバックを描画待ちで塞がないため)。
        // 実際のDataGridViewへの反映はgridFlushTimerがまとめて行う
        // waitが正の場合、待機だけを表す独立したWAIT行を先に積んでから、実際のイベント行を積む
        // (待機時間はイベント自体の属性ではなく、シーケンス上の別行として表現する方式にしたため)
        private void QueueRow(String type, String x, String y, String key, int wait)
        {
            if (wait > 0)
            {
                pendingRows.Add(new String[] { WaitEventType, "", "", "", wait.ToString() });
            }

            pendingRows.Add(new String[] { type, x, y, key, "0" });
        }

        // 貯まった行をまとめてDataGridViewに反映する。1件ずつ反映するより
        // 再描画・スクロール処理の回数を大幅に減らせる
        private void FlushPendingRows()
        {
            if (pendingRows.Count == 0)
            {
                return;
            }

            dataGridView_Events.SuspendLayout();

            int lastIdx = -1;
            foreach (String[] r in pendingRows)
            {
                lastIdx = dataGridView_Events.Rows.Add();
                DataGridViewRow row = dataGridView_Events.Rows[lastIdx];
                EventRowMapper.ApplyToRow(row, col_Type, col_X, col_Y, col_Key, col_Wait, r);
            }

            pendingRows.Clear();

            dataGridView_Events.ResumeLayout();

            // 記録中も、今追加された最新行を薄い黄色でハイライト+自動スクロールする
            // (単発再生中のハイライトと同じ見た目・仕組みを流用)
            if (lastIdx >= 0)
            {
                HighlightPlayingRow(lastIdx);
            }
        }

        // 旧バージョンの保存形式(各行が自分自身の待機時間をWait列に持つ)を、
        // 新形式(待機を独立したWAIT行として挿入する形式)に変換する。
        // すでに新形式(各行のWaitが0)の場合は何もしない、何度呼んでも安全な処理。
        // ファイル読込のたびにSaveRestore.csから呼ばれる
        internal void MigrateWaitColumnToRows()
        {
            dataGridView_Events.SuspendLayout();
            try
            {
                // 後ろから処理すれば、Insertしても未処理の行のインデックスに影響しない
                for (int i = dataGridView_Events.Rows.Count - 1; i >= 0; i--)
                {
                    DataGridViewRow row = dataGridView_Events.Rows[i];
                    String type = Convert.ToString(row.Cells[col_Type.Index].Value);

                    if (type == WaitEventType)
                    {
                        continue;
                    }

                    int wait = util.GetInteger(Convert.ToString(row.Cells[col_Wait.Index].Value));
                    if (wait <= 0)
                    {
                        continue;
                    }

                    dataGridView_Events.Rows.Insert(i, 1);
                    DataGridViewRow waitRow = dataGridView_Events.Rows[i];
                    waitRow.Cells[col_Type.Index].Value = WaitEventType;
                    waitRow.Cells[col_Wait.Index].Value = wait.ToString();

                    // 元の行のWaitは移し替えたので0にしておく(でないと再生時に二重に待ってしまう)
                    row.Cells[col_Wait.Index].Value = "0";
                }
            }
            finally
            {
                dataGridView_Events.ResumeLayout();
            }
        }

        // *******************************************************************************
        // 再生

        private void button_Play_Click(object sender, EventArgs e)
        {
            PlayOrStop();
        }

        // 単発再生とプレイリスト実行を1つのbutton_Play(表示名「再生」)に統合したので、
        // 開始時にどちらを動かすかはラジオボタンで選択中のモードで振り分ける。
        // 停止は共通(今動いている方が何であれ、そのままstopPlayRequestedで止める)。
        // ボタンクリックからも再生ホットキー(変換キー/Shift+F2)からも呼ばれる
        private void PlayOrStop()
        {
            if (isPlaying)
            {
                stopPlayRequested = true;
                return;
            }

            if (isRecording)
            {
                return;
            }

            if (radioButton_Playback.Checked)
            {
                StartPlaylistRun();
            }
            else
            {
                StartSinglePlayback();
            }
        }

        // レコードグループの記録データ(dataGridView_Events)を単発再生する
        private void StartSinglePlayback()
        {
            if (!ValidateEventsForPlayback())
            {
                return;
            }

            int loopCount = util.GetInteger(textBox_Loop.Text);
            if (loopCount <= 0)
            {
                loopCount = 1;
            }

            List<String[]> rows = SnapshotRows();
            if (rows.Count == 0)
            {
                return;
            }

            // 再生終了後にカーソルを戻せるよう、今の位置を覚えておく
            cursorPositionBeforePlay = Cursor.Position;

            // 単発再生には「全体ループ」の概念が無いので常に1/1固定にする
            playbackOverallLoopNo = 1;
            playbackOverallLoopMax = 1;
            playbackInnerLoopNo = 0;
            playbackInnerLoopMax = loopCount;

            isPlaying = true;
            stopPlayRequested = false;
            UpdatePlayButtons();
            UpdateTitle();
            MinimizeIfRequested();

            Task.Run(() => PlayLoop(rows, loopCount));
        }

        // checkBox_MinimizeOnPlayがチェックされていたら、再生開始と同時にウィンドウを最小化する。
        // 再生対象のアプリの操作を邪魔しないようにするための機能
        private void MinimizeIfRequested()
        {
            if (checkBox_MinimizeOnPlay.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        // MinimizeIfRequestedで最小化した場合、再生終了時に元の表示状態へ戻す
        private void RestoreIfMinimizedByPlay()
        {
            if (checkBox_MinimizeOnPlay.Checked && this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private List<String[]> SnapshotRows()
        {
            List<String[]> list = new List<String[]>();
            foreach (DataGridViewRow row in dataGridView_Events.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                // PlayRows/PlayOneEventは[Type, X, Y, Key, Wait]の順を前提にしている
                list.Add(EventRowMapper.ReadFromRow(row, col_Type, col_X, col_Y, col_Key, col_Wait));
            }
            return list;
        }

        private void PlayLoop(List<String[]> rows, int loopCount)
        {
            try
            {
                PlayRows(rows, loopCount);

                // マウスカーソルを再生開始前の位置に戻す
                Cursor.Position = cursorPositionBeforePlay;
            }
            finally
            {
                // 途中で例外が起きても、必ず「再生中」状態を解除する。
                // ここを素通りしてしまうとisPlayingがtrueのまま固まり、記録も再生も
                // 二度とできなくなる(アプリが固まって見える不具合の原因になっていた)
                isPlaying = false;
                stopPlayRequested = false;
                this.Invoke((MethodInvoker)(() =>
                {
                    UpdatePlayButtons();
                    UpdateTitle();
                    HighlightPlayingRow(-1);
                    RestoreIfMinimizedByPlay();
                }));
            }
        }

        // rowsをloopCount回再生する処理そのもの(前後の状態管理は呼び出し元の責務)。
        // タブ1の単発再生・タブ2のプレイリスト再生の両方から共通で使う
        private void PlayRows(List<String[]> rows, int loopCount)
        {
            for (int i = 0; i < loopCount && !stopPlayRequested; i++)
            {
                playbackInnerLoopNo = i + 1;
                playbackInnerLoopMax = loopCount;

                for (int idx = 0; idx < rows.Count && !stopPlayRequested; idx++)
                {
                    String[] r = rows[idx];

                    this.Invoke((MethodInvoker)(() =>
                    {
                        HighlightPlayingRow(idx);
                        UpdateTitle();
                    }));

                    int wait = util.GetInteger(r[4]);
                    if (wait > 0)
                    {
                        Thread.Sleep(wait);
                    }

                    PlayOneEvent(r[0], r[1], r[2], r[3]);
                }
            }
        }

        // 指定したグリッドのidx行を薄い黄色でハイライトし、見えるようにスクロールする。
        // idxに-1を渡すとハイライトを消す。記録中の最新行・単発再生中の実行中行・
        // プレイリスト実行中の実行中行(グリッドは呼び出し元次第)、どれも共通のこの処理を使う
        private void HighlightRow(StandardTemplate.DataGridViewEx grid, int idx, ref int highlightedIndexField)
        {
            if (highlightedIndexField >= 0 && highlightedIndexField < grid.Rows.Count)
            {
                grid.Rows[highlightedIndexField].DefaultCellStyle.BackColor = Color.Empty;
            }

            if (idx >= 0 && idx < grid.Rows.Count)
            {
                grid.Rows[idx].DefaultCellStyle.BackColor = Color.LightYellow;

                if (grid.Visible)
                {
                    try
                    {
                        grid.FirstDisplayedScrollingRowIndex = idx;
                    }
                    catch (InvalidOperationException)
                    {
                        // グリッドが未表示/高さ0等で行を表示できないタイミングでは
                        // スクロール位置合わせを諦めて処理自体は継続する(既知のWinForms挙動)
                    }
                }
            }

            highlightedIndexField = idx;
        }

        // 記録中の最新行・単発再生中の実行中行のハイライト(dataGridView_Events側)
        private void HighlightPlayingRow(int idx)
        {
            HighlightRow(dataGridView_Events, idx, ref highlightedRowIndex);
        }

        // プレイリスト実行中、今どのファイル(行)を再生しているかのハイライト(dataGridView_Playlist側)
        private void HighlightPlaylistRow(int idx)
        {
            HighlightRow(dataGridView_Playlist, idx, ref highlightedPlaylistRowIndex);
        }

        // Ctrl+V。DataGridViewはCtrl+A(全選択)/Ctrl+C(コピー)は標準対応してるけど、
        // 貼り付けだけは無いので自前で実装する
        private void dataGridView_Events_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                ClearSelectedCells(dataGridView_Events);
                e.Handled = true;
            }
        }

        // プレイリストのグリッドでもDeleteキーでセルの中身を空にできるようにする
        private void dataGridView_Playlist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ClearSelectedCells(dataGridView_Playlist);
                e.Handled = true;
            }
        }

        // 選択中のセルの中身を空にする(行そのものは削除しない。行削除は右クリックメニューの担当)。
        // 記録中/再生中は誤操作防止のため無効にする。複数セルをまとめて1回のCtrl+Zで
        // 戻せるよう、DataGridViewExのUndoバッチでまとめる
        private void ClearSelectedCells(StandardTemplate.DataGridViewEx grid)
        {
            if (isRecording || isPlaying)
            {
                return;
            }

            grid.BeginUndoBatch();
            try
            {
                foreach (DataGridViewCell cell in grid.SelectedCells)
                {
                    // ComboBoxセル(プレイリストの設定ファイル列)は選択肢(Items)に無い値を許容しないため、
                    // nullを入れようとするとDataError(既定のエラーダイアログ)になる。クリア対象から除外する
                    if (cell.ReadOnly || cell is DataGridViewComboBoxCell)
                    {
                        continue;
                    }

                    cell.Value = null;
                }
            }
            finally
            {
                grid.EndUndoBatch();
            }
        }

        // 値の設定などで想定外のDataErrorが起きても、既定のエラーダイアログを出さずに無視して
        // 処理を継続する(ClearSelectedCellsで防いだ問題以外にも同種の不具合が今後起きうるための保険)
        private void dataGridView_Events_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Playlist_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        // クリップボードのタブ区切りテキスト(Excelや他のDataGridViewからのコピーと同じ形式)を貼り付ける。
        // 列がグリッドの列数をはみ出る分は切り捨て、行が足りない場合は全部貼り付けられるように追加する
        private void PasteFromClipboard()
        {
            if (isRecording || isPlaying || !Clipboard.ContainsText())
            {
                return;
            }

            String[] lines = Clipboard.GetText().Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            if (lines.Length == 0)
            {
                return;
            }

            // X/Y/Key列は非表示なので、実際に見えている列(DisplayIndex順)だけを貼り付け対象にする
            List<DataGridViewColumn> visibleColumns = GetVisibleColumnsInDisplayOrder(dataGridView_Events);

            DataGridViewCell startCell = dataGridView_Events.CurrentCell;
            int startRow = (startCell != null) ? startCell.RowIndex : 0;
            int startVisibleCol = (startCell != null) ? visibleColumns.FindIndex(c => c.Index == startCell.ColumnIndex) : 0;
            if (startVisibleCol < 0)
            {
                startVisibleCol = 0;
            }

            // 複数セルへの貼り付けをまとめて1回のCtrl+Zで戻せるよう、Undoバッチでまとめる
            dataGridView_Events.BeginUndoBatch();
            try
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    int rowIndex = startRow + i;

                    // 行が足りなければ、貼り付け分を全部入れられるように追加する
                    while (rowIndex >= dataGridView_Events.Rows.Count)
                    {
                        dataGridView_Events.Rows.Add();
                    }

                    String[] values = lines[i].Split('\t');
                    for (int j = 0; j < values.Length; j++)
                    {
                        int visibleColIndex = startVisibleCol + j;

                        // 見えている列数をはみ出る分は切り捨てる
                        if (visibleColIndex >= visibleColumns.Count)
                        {
                            break;
                        }

                        dataGridView_Events.Rows[rowIndex].Cells[visibleColumns[visibleColIndex].Index].Value = values[j];
                    }
                }
            }
            finally
            {
                dataGridView_Events.EndUndoBatch();
            }
        }

        // gridの表示列(Visible=true)だけを、DisplayIndex順に並べて返す
        private static List<DataGridViewColumn> GetVisibleColumnsInDisplayOrder(DataGridView grid)
        {
            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Visible)
                {
                    columns.Add(col);
                }
            }

            columns.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));
            return columns;
        }

        // 右クリックしたセルの行(手動で行の追加/削除をする対象)。右クリック無しの状態(-1)なら末尾扱い
        private int contextMenuRowIndex = -1;

        // 右クリック時に、そのセルの行を選択状態にしつつ対象行を覚えておく
        private void dataGridView_Events_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            contextMenuRowIndex = e.RowIndex;

            // 左クリックで複数行選択済みの状態から右クリックでメニューを開きたいケースがあるため、
            // 右クリックした行がすでに選択済みならその選択状態を維持する。
            // 未選択の行を右クリックした場合だけ、その行の単一選択に切り替える。
            // 既定のRowHeaderSelectモードでは、行ヘッダーではなくセルのドラッグで複数選択すると
            // Rows[].Selectedはfalseのままになるため、右クリックしたセル自体の選択状態もあわせて見る
            Boolean isAlreadySelected = e.RowIndex >= 0 &&
                ((e.ColumnIndex >= 0 && dataGridView_Events.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
                || dataGridView_Events.Rows[e.RowIndex].Selected);

            if (e.RowIndex >= 0 && !isAlreadySelected)
            {
                dataGridView_Events.ClearSelection();
                dataGridView_Events.Rows[e.RowIndex].Selected = true;
            }
        }

        // 行が無い場所を右クリックした場合は「行の削除」を選べないようにする
        private void contextMenuStrip_Grid_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isRecording || isPlaying)
            {
                e.Cancel = true;
                return;
            }

            menuItem_DeleteRow.Enabled = contextMenuRowIndex >= 0 && contextMenuRowIndex < dataGridView_Events.Rows.Count;
        }

        // 右クリックした行のすぐ下に空行を1件挿入する(何も無い場所を右クリックした場合は末尾に追加)
        private void menuItem_AddRow_Click(object sender, EventArgs e)
        {
            int insertAt = (contextMenuRowIndex >= 0) ? contextMenuRowIndex + 1 : dataGridView_Events.Rows.Count;
            dataGridView_Events.Rows.Insert(insertAt, 1);
        }

        // 選択されている行(複数選択時は全行、未選択なら右クリックした行)を削除する。
        // KeyDown/SysKeyDownの行なら、非表示になっている対応するKeyUp/SysKeyUp行も
        // 一緒に探して削除する(片方だけ残って孤立するのを防ぐため)
        private void menuItem_DeleteRow_Click(object sender, EventArgs e)
        {
            List<int> targetIndexes = GetSelectedOrContextMenuRowIndexes(dataGridView_Events, contextMenuRowIndex);
            if (targetIndexes.Count == 0)
            {
                return;
            }

            // ペアのKeyUp行も含めて、重複なく集める
            HashSet<int> indexesToRemove = new HashSet<int>(targetIndexes);
            foreach (int idx in targetIndexes)
            {
                int pairedUpRowIndex = FindPairedKeyUpRowIndex(idx);
                if (pairedUpRowIndex >= 0)
                {
                    indexesToRemove.Add(pairedUpRowIndex);
                }
            }

            // 後ろのインデックスから消していけば、前方のインデックスがずれる心配がない
            foreach (int idx in indexesToRemove.OrderByDescending(x => x))
            {
                dataGridView_Events.Rows.RemoveAt(idx);
            }
        }

        // 複数行選択済みならその全行、未選択なら右クリックした1行(fallbackRowIndex)を対象にする。
        // 記録グリッド・プレイリストグリッドの両方の「行の削除」から共通で使う
        private List<int> GetSelectedOrContextMenuRowIndexes(DataGridView grid, int fallbackRowIndex)
        {
            HashSet<int> indexes = new HashSet<int>();

            foreach (DataGridViewRow row in grid.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    indexes.Add(row.Index);
                }
            }

            // 既定のRowHeaderSelectモードでは、行ヘッダーではなくセルをドラッグして複数選択することが
            // 多く、その場合SelectedRowsには何も入らないため、SelectedCellsからも対象行を拾う
            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                if (cell.RowIndex >= 0 && !grid.Rows[cell.RowIndex].IsNewRow)
                {
                    indexes.Add(cell.RowIndex);
                }
            }

            if (indexes.Count == 0 && fallbackRowIndex >= 0 && fallbackRowIndex < grid.Rows.Count)
            {
                indexes.Add(fallbackRowIndex);
            }

            return indexes.OrderBy(x => x).ToList();
        }

        // downRowIndexがKeyDown/SysKeyDownの行なら、それより後ろで最初に見つかる
        // 「同じKeyの対応するKeyUp/SysKeyUp行」のインデックスを返す。見つからなければ-1
        private int FindPairedKeyUpRowIndex(int downRowIndex)
        {
            DataGridViewRow downRow = dataGridView_Events.Rows[downRowIndex];
            String downType = Convert.ToString(downRow.Cells[col_Type.Index].Value);
            String key = Convert.ToString(downRow.Cells[col_Key.Index].Value);

            String upType;
            if (downType == GlobalHook.KeyboardHook.Stroke.KEY_DOWN.ToString())
            {
                upType = GlobalHook.KeyboardHook.Stroke.KEY_UP.ToString();
            }
            else if (downType == GlobalHook.KeyboardHook.Stroke.SYSKEY_DOWN.ToString())
            {
                upType = GlobalHook.KeyboardHook.Stroke.SYSKEY_UP.ToString();
            }
            else
            {
                return -1;
            }

            for (int i = downRowIndex + 1; i < dataGridView_Events.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView_Events.Rows[i];
                String type = Convert.ToString(row.Cells[col_Type.Index].Value);
                String rowKey = Convert.ToString(row.Cells[col_Key.Index].Value);

                if (type == upType && rowKey == key)
                {
                    return i;
                }
            }

            return -1;
        }

        // 行ヘッダー(左端)に行番号(0始まりのインデックス)を描画する。
        // DataGridViewには自動で行番号を出す機能が無いので、定番のRowPostPaintで自前描画する。
        // dataGridView_Events・dataGridView_Playlist両方から共用するので、対象はsenderから取る
        private void dataGridView_Events_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            String rowIndexStr = e.RowIndex.ToString();
            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIndexStr, grid.Font, SystemBrushes.ControlText, headerBounds, format);
        }

        // マウスイベントの行なのにKey列にも値が入っているか判定する。
        // (PlayOneEventはEvent列でマウス/キーボードを判定するので、この場合Key列は再生時に無視される)
        private Boolean IsKeyIgnoredOnMouseRow(DataGridViewRow row, out String message)
        {
            String eventType = Convert.ToString(row.Cells[col_Type.Index].Value);
            String keyValue = Convert.ToString(row.Cells[col_Key.Index].Value);

            InputSimulation.InputSimulator.MouseStroke mouseStroke;
            Boolean isMouseRow = Enum.TryParse<InputSimulation.InputSimulator.MouseStroke>(eventType, out mouseStroke);

            if (isMouseRow && !String.IsNullOrEmpty(keyValue))
            {
                message = "マウスイベント(" + eventType + ")の行なので、Key(" + keyValue + ")は再生時には使われず無視されるよ";
                return true;
            }

            message = "";
            return false;
        }

        // 再生時にint.Parse/Enum.Parse相当が失敗して、値が読めないまま無言で再生が止まって
        // しまう行を検出する(実際に例外の原因になりうる値だけをチェックする)。
        // 認識できないEvent種別の行(例: 打ち間違い)は、再生時に静かにスキップされるだけで
        // 例外は起きないため、ここではチェック対象にしない
        private Boolean IsRowInvalidForPlayback(DataGridViewRow row, out String message)
        {
            String type = Convert.ToString(row.Cells[col_Type.Index].Value);

            if (type == WaitEventType)
            {
                String wait = Convert.ToString(row.Cells[col_Wait.Index].Value);
                int waitValue;
                if (!int.TryParse(wait, out waitValue) || waitValue < 0)
                {
                    message = "WAIT行の待機時間(" + wait + ")が数値として読み取れないよ";
                    return true;
                }

                message = "";
                return false;
            }

            InputSimulation.InputSimulator.MouseStroke mouseStroke;
            if (Enum.TryParse<InputSimulation.InputSimulator.MouseStroke>(type, out mouseStroke))
            {
                String x = Convert.ToString(row.Cells[col_X.Index].Value);
                String y = Convert.ToString(row.Cells[col_Y.Index].Value);
                int xValue, yValue;
                if (!int.TryParse(x, out xValue) || !int.TryParse(y, out yValue))
                {
                    message = "マウスイベント(" + type + ")のX/Y(" + x + ", " + y + ")が数値として読み取れないよ";
                    return true;
                }

                message = "";
                return false;
            }

            InputSimulation.InputSimulator.KeyboardStroke keyStroke;
            if (Enum.TryParse<InputSimulation.InputSimulator.KeyboardStroke>(type, out keyStroke))
            {
                String key = Convert.ToString(row.Cells[col_Key.Index].Value);
                Keys keyCode;
                if (!Enum.TryParse<Keys>(key, out keyCode))
                {
                    message = "キーボードイベント(" + type + ")のKey(" + key + ")が認識できないよ";
                    return true;
                }

                message = "";
                return false;
            }

            message = "";
            return false;
        }

        // マウスイベントの行なのにKey列にも値が入っている場合や、再生時に読み取れない値が
        // 入っている場合に、Detail列の背景を薄いピンクにして知らせる。
        // 加えてErrorTextにも同じ内容を入れて、セルのエラーアイコン+ホバー時の吹き出しでも分かるようにする
        // (Key列自体は非表示になったため、警告表示はDetail列(実際に見えている列)に出す)
        private void dataGridView_Events_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != col_Detail.Index || e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dataGridView_Events.Rows[e.RowIndex];

            String errorMessage;
            Boolean isInvalid = IsRowInvalidForPlayback(row, out errorMessage);

            String warningMessage;
            Boolean hasWarning = IsKeyIgnoredOnMouseRow(row, out warningMessage);

            if (isInvalid || hasWarning)
            {
                e.CellStyle.BackColor = Color.MistyRose;
            }

            row.Cells[col_Detail.Index].ErrorText = isInvalid ? errorMessage : warningMessage;
        }

        // 再生開始前に全行のパラメータをチェックする。不正な値が見つかった行があれば、
        // そのセルをピンクでハイライトした上でエラーメッセージをまとめてポップアップ表示し、
        // 再生を開始させない(false を返す)
        private Boolean ValidateEventsForPlayback()
        {
            List<String> errors = new List<String>();

            foreach (DataGridViewRow row in dataGridView_Events.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                String message;
                if (IsRowInvalidForPlayback(row, out message))
                {
                    errors.Add("行" + row.Index + ": " + message);
                }
            }

            // CellFormattingによるピンクのハイライトをすぐ反映させる
            dataGridView_Events.Refresh();

            if (errors.Count == 0)
            {
                return true;
            }

            MessageBox.Show(
                "再生できない行が見つかったよ(該当セルはピンク色で表示してるよ)" + Environment.NewLine + Environment.NewLine
                    + String.Join(Environment.NewLine, errors),
                "EventRecorder - 再生前チェック",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        // Detail列とX/Y/Key列(非表示)を相互に同期させている最中、再帰的な同期を防ぐためのフラグ
        private Boolean isSyncingDetailColumn = false;

        // Event列を書き換えた直後もDetail列の警告表示(CellFormatting)がすぐ反映されるように、
        // 明示的にDetail列セルを再描画する(別列の値変更ではCellFormattingが自動では呼ばれないため)。
        // 記録・貼り付け・XML読込・手動編集、どの経路でEvent列がセットされてもここを通るので、
        // KeyUp行を非表示にする処理もまとめてここでやる。
        // さらに、実データ(X/Y/Key、非表示)とDetail列(表示・編集用)を相互に同期させる
        private void dataGridView_Events_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || isSyncingDetailColumn)
            {
                return;
            }

            if (e.ColumnIndex == col_Type.Index)
            {
                dataGridView_Events.InvalidateCell(col_Detail.Index, e.RowIndex);
                UpdateRowVisibility(e.RowIndex);
                // WAIT行への/からの切り替え等、Event列の変更でDetail列の意味も変わるので再計算する
                SyncDetailFromHiddenColumns(e.RowIndex);
                return;
            }

            if (e.ColumnIndex == col_X.Index || e.ColumnIndex == col_Y.Index || e.ColumnIndex == col_Key.Index
                || e.ColumnIndex == col_Wait.Index)
            {
                // 記録・XML読込・貼り付け等で実データ(X/Y/Key、WAIT行ならWait)が更新されたら、
                // Detail列の表示も追従させる
                SyncDetailFromHiddenColumns(e.RowIndex);
                return;
            }

            if (e.ColumnIndex == col_Detail.Index)
            {
                // ユーザーがDetail列を直接編集した時は、逆に実データへ書き戻す
                SyncHiddenColumnsFromDetail(e.RowIndex);
                return;
            }
        }

        // Event列が"WAIT"の行は、待機のためだけの行(実際の操作を伴わない)を表す
        private const String WaitEventType = "WAIT";

        private void SyncDetailFromHiddenColumns(int rowIndex)
        {
            DataGridViewRow row = dataGridView_Events.Rows[rowIndex];
            String type = Convert.ToString(row.Cells[col_Type.Index].Value);

            isSyncingDetailColumn = true;
            try
            {
                if (type == WaitEventType)
                {
                    String wait = Convert.ToString(row.Cells[col_Wait.Index].Value);
                    row.Cells[col_Detail.Index].Value = FormatWaitDetail(wait);
                }
                else
                {
                    String x = Convert.ToString(row.Cells[col_X.Index].Value);
                    String y = Convert.ToString(row.Cells[col_Y.Index].Value);
                    String key = Convert.ToString(row.Cells[col_Key.Index].Value);
                    row.Cells[col_Detail.Index].Value = FormatDetail(x, y, key);
                }
            }
            finally
            {
                isSyncingDetailColumn = false;
            }
        }

        private void SyncHiddenColumnsFromDetail(int rowIndex)
        {
            DataGridViewRow row = dataGridView_Events.Rows[rowIndex];
            String type = Convert.ToString(row.Cells[col_Type.Index].Value);
            String detail = Convert.ToString(row.Cells[col_Detail.Index].Value);

            isSyncingDetailColumn = true;
            try
            {
                if (type == WaitEventType)
                {
                    String wait;
                    TryParseWaitDetail(detail, out wait);
                    row.Cells[col_Wait.Index].Value = wait;
                }
                else
                {
                    String x, y, key;
                    ParseDetail(detail, out x, out y, out key);
                    row.Cells[col_X.Index].Value = x;
                    row.Cells[col_Y.Index].Value = y;
                    row.Cells[col_Key.Index].Value = key;
                }
            }
            finally
            {
                isSyncingDetailColumn = false;
            }
        }

        // マウス行は"X:123 Y:456"、キーボード行は"Key:A"の形式でDetail列に表示する
        private static String FormatDetail(String x, String y, String key)
        {
            if (!String.IsNullOrEmpty(key))
            {
                return "Key:" + key;
            }

            if (!String.IsNullOrEmpty(x) || !String.IsNullOrEmpty(y))
            {
                return "X:" + x + " Y:" + y;
            }

            return String.Empty;
        }

        // FormatDetailの逆変換。"X:123 Y:456"や"Key:A"の形式から値を取り出す
        private static void ParseDetail(String detail, out String x, out String y, out String key)
        {
            x = String.Empty;
            y = String.Empty;
            key = String.Empty;

            if (String.IsNullOrEmpty(detail))
            {
                return;
            }

            foreach (String token in detail.Split(' '))
            {
                if (token.StartsWith("X:"))
                {
                    x = token.Substring(2);
                }
                else if (token.StartsWith("Y:"))
                {
                    y = token.Substring(2);
                }
                else if (token.StartsWith("Key:"))
                {
                    key = token.Substring(4);
                }
            }
        }

        // WAIT行のDetail表示("500ms"のような形式)
        private static String FormatWaitDetail(String waitMs)
        {
            return waitMs + "ms";
        }

        // FormatWaitDetailの逆変換。末尾の"ms"を取り除いてミリ秒の数値文字列を取り出す
        private static Boolean TryParseWaitDetail(String detail, out String waitMs)
        {
            waitMs = String.Empty;

            if (String.IsNullOrEmpty(detail) || !detail.EndsWith("ms"))
            {
                return false;
            }

            waitMs = detail.Substring(0, detail.Length - 2);
            return true;
        }

        // KeyUp/SysKeyUpの行は、リピート抑制のための内部データとしては保持したまま、
        // テーブルの見た目からは隠す(データは残るので保存/再生には引き続き使われる)
        private void UpdateRowVisibility(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView_Events.Rows.Count)
            {
                return;
            }

            DataGridViewRow row = dataGridView_Events.Rows[rowIndex];
            String eventType = Convert.ToString(row.Cells[col_Type.Index].Value);

            Boolean isKeyUp = eventType == GlobalHook.KeyboardHook.Stroke.KEY_UP.ToString()
                || eventType == GlobalHook.KeyboardHook.Stroke.SYSKEY_UP.ToString();

            row.Visible = !isKeyUp;
        }

        // 記録した1イベント分をSendInputで再現する
        // ※ マウスのX(サイド)ボタン/ホイールとキーボードのSYSKEYは、
        //    HiMacroEx相当を作る段階では未対応(将来の機能拡張項目)
        private void PlayOneEvent(String type, String x, String y, String key)
        {
            List<InputSimulation.InputSimulator.Input> inputs = new List<InputSimulation.InputSimulator.Input>();

            InputSimulation.InputSimulator.MouseStroke mouseStroke;
            if (Enum.TryParse<InputSimulation.InputSimulator.MouseStroke>(type, out mouseStroke))
            {
                List<InputSimulation.InputSimulator.MouseStroke> flags = new List<InputSimulation.InputSimulator.MouseStroke>();
                flags.Add(InputSimulation.InputSimulator.MouseStroke.MOVE);
                flags.Add(mouseStroke);

                int ix = util.GetInteger(x);
                int iy = util.GetInteger(y);
                InputSimulation.InputSimulator.AddMouseInput(ref inputs, flags, 0, true, ix, iy);
                InputSimulation.InputSimulator.SendInput(inputs);
                return;
            }

            InputSimulation.InputSimulator.KeyboardStroke keyStroke;
            Keys keyCode;
            if (Enum.TryParse<InputSimulation.InputSimulator.KeyboardStroke>(type, out keyStroke)
                && Enum.TryParse<Keys>(key, out keyCode))
            {
                InputSimulation.InputSimulator.AddKeyboardInput(ref inputs, keyStroke, keyCode);
                InputSimulation.InputSimulator.SendInput(inputs);
            }
        }

        // *******************************************************************************
        // クリア/保存/読込

        private void button_Clear_Click(object sender, EventArgs e)
        {
            if (isRecording || isPlaying)
            {
                return;
            }

            dataGridView_Events.Rows.Clear();
        }

        // コンボボックスで設定ファイルを選び直したら、そのままそれを読み込む(Cheetosと同じ挙動)
        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = System.IO.Path.Combine(userDataFolder, comboBox_Profile.Text);
            LoadProfile(LoadFileName);
        }

        // *******************************************************************************
        // JSON保存/読込([[_Common/JsonFileStorage.cs]])。設定値はこれまでXML(StcSaveRestore)
        // 一本だったが、今後はJSONへ段階的に移行していく方針のため、拡張子で振り分ける。
        // 「既存のXMLをJSONで保存し直す」機能は、専用の変換ボタンを別途作るのではなく、
        // XMLを読み込んだ状態のままファイル保存ダイアログで.json拡張子を選ぶだけで実現できる
        // (BuildProfileFromGridsは読込元の形式を問わず、今グリッドにある内容をそのまま使うため)

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(System.IO.Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 設定ファイル(記録データ+プレイリスト)をまるごと読み込む。拡張子がjsonならJSON、
        // それ以外は従来通りXMLとして読み込む(既定でプレイリストもクリアする、sr.LoadProc相当)
        private void LoadProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                LoadProfileFromJson(filePath, true);
            }
            else
            {
                sr.LoadProc(filePath, this);
            }
        }

        // プレイリスト再生時、各行の設定ファイルを1つずつ読み込む専用(記録データのみ差し替え、
        // プレイリスト自体は触らない。playbackLoader.LoadProc(..., false)のJSON対応版)
        private void LoadProfileForPlayback(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                LoadProfileFromJson(filePath, false);
            }
            else
            {
                playbackLoader.LoadProc(filePath, this, false);
            }
        }

        // JSONファイルを読み込み、記録データ(+clearPlaylistがtrueならプレイリストも)グリッドへ反映する
        private void LoadProfileFromJson(String filePath, Boolean clearPlaylist)
        {
            EventRecorderProfile profile = JsonFileStorage.Load<EventRecorderProfile>(filePath);
            if (profile == null)
            {
                return;
            }

            dataGridView_Events.Rows.Clear();
            if (clearPlaylist)
            {
                dataGridView_Playlist.Rows.Clear();
            }

            textBox_Loop.Text = String.IsNullOrEmpty(profile.LoopCount) ? "1" : profile.LoopCount;

            foreach (MacroEventData ev in profile.Events ?? new List<MacroEventData>())
            {
                int idx = dataGridView_Events.Rows.Add();
                DataGridViewRow row = dataGridView_Events.Rows[idx];
                row.Cells[col_Type.Index].Value = ev.Type;
                row.Cells[col_X.Index].Value = ev.X;
                row.Cells[col_Y.Index].Value = ev.Y;
                row.Cells[col_Key.Index].Value = ev.Key;
                row.Cells[col_Wait.Index].Value = ev.Wait;
            }

            if (clearPlaylist)
            {
                foreach (PlaylistEntryData pl in profile.Playlist ?? new List<PlaylistEntryData>())
                {
                    int idx = dataGridView_Playlist.Rows.Add();
                    DataGridViewRow row = dataGridView_Playlist.Rows[idx];
                    row.Cells[col_PlaylistEnabled.Index].Value = pl.Enabled;
                    row.Cells[col_PlaylistFile.Index].Value = pl.FileName;
                    row.Cells[col_PlaylistLoopCount.Index].Value = pl.LoopCount;
                }
            }

            // 万一、旧XMLをそのままJSON化しただけ(各行が自分のWaitを持つ旧形式相当)のデータを
            // 読み込んでも安全なように、XML読込時と同じ変換を通しておく
            MigrateWaitColumnToRows();

            // ファイル読込は「ユーザーの編集操作」ではないので、Ctrl+Zで戻せないようにする
            dataGridView_Events.ClearUndoHistory();
            if (clearPlaylist)
            {
                dataGridView_Playlist.ClearUndoHistory();
            }
        }

        // 今のグリッドの中身(記録データ+プレイリスト)をJSON保存用のPOCOに詰め替える
        private EventRecorderProfile BuildProfileFromGrids()
        {
            EventRecorderProfile profile = new EventRecorderProfile();
            profile.LoopCount = textBox_Loop.Text;

            foreach (DataGridViewRow row in dataGridView_Events.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                profile.Events.Add(new MacroEventData
                {
                    Type = Convert.ToString(row.Cells[col_Type.Index].Value),
                    X = Convert.ToString(row.Cells[col_X.Index].Value),
                    Y = Convert.ToString(row.Cells[col_Y.Index].Value),
                    Key = Convert.ToString(row.Cells[col_Key.Index].Value),
                    Wait = Convert.ToString(row.Cells[col_Wait.Index].Value),
                });
            }

            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                profile.Playlist.Add(new PlaylistEntryData
                {
                    Enabled = Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false),
                    FileName = Convert.ToString(row.Cells[col_PlaylistFile.Index].Value),
                    LoopCount = Convert.ToString(row.Cells[col_PlaylistLoopCount.Index].Value),
                });
            }

            return profile;
        }

        // filePathの拡張子で振り分けて保存する(JSONならJsonFileStorage、XMLなら従来のsr.SaveSetting)
        private Boolean SaveProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                try
                {
                    JsonFileStorage.Save(filePath, BuildProfileFromGrids());
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "保存に失敗したよ: " + ex.Message,
                        "EventRecorder - 保存エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }

            return sr.SaveSetting(filePath);
        }

        // comboBox_Profile(プレイリストのcol_PlaylistFileも含む)へ、userDataFolder配下の
        // *.xmlと*.jsonの両方をまとめてリストアップする。util.UpdateProfileListは拡張子を
        // 1パターンしか指定できないため、ここでは2回検索した結果をマージして直接セットする
        private void UpdateProfileListAll(String defaultProfileName)
        {
            String[] xmlFiles = System.IO.Directory.GetFiles(userDataFolder, "*.xml", System.IO.SearchOption.AllDirectories);
            String[] jsonFiles = System.IO.Directory.GetFiles(userDataFolder, "*.json", System.IO.SearchOption.AllDirectories);
            String[] files = xmlFiles.Concat(jsonFiles).ToArray();

            util.SetComboBoxFromArray(comboBox_Profile, files, userDataFolder);
            util.SetComboBoxText(comboBox_Profile, defaultProfileName);
        }

        // 読込ボタンと同じ感覚で使えるよう、「現在のファイルに上書きしますか?」の確認は挟まず、
        // 常にダイアログを直接開く(SelectSaveFileNameのCheetos流の確認ステップはあえて使わない)
        private void button_ProfileSave_Click(object sender, EventArgs e)
        {
            // プルダウンで既存ファイルが選ばれている時は、毎回ダイアログを開かず
            // 「上書きしますか?」の確認だけで済ませられるようにする。
            // プルダウンが空の時は、従来通りファイル選択ダイアログを出す
            if (!String.IsNullOrEmpty(comboBox_Profile.Text))
            {
                DialogResult overwriteResult = MessageBox.Show(
                    comboBox_Profile.Text + " を上書きしますか?",
                    "上書き確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (overwriteResult == DialogResult.Yes)
                {
                    String overwriteFileName = System.IO.Path.Combine(userDataFolder, comboBox_Profile.Text);
                    if (SaveProfile(overwriteFileName))
                    {
                        UpdateProfileListAll(System.IO.Path.GetFileName(overwriteFileName));
                        SyncPlaylistFileItems();
                    }
                    return;
                }
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = comboBox_Profile.Text;
            dlg.InitialDirectory = userDataFolder;
            // 今後はJSON保存を主流にしていく方針なので、フィルタの先頭(既定)をJSONにしてある。
            // 既存のXMLプロファイルを開いた状態でここに来て.jsonを選べば、そのままXML→JSON変換になる
            dlg.Filter = "JSONファイル(*.json)|*.json|XMLファイル(*.xml)|*.xml|すべてのファイル(*.*)|*.*";
            dlg.Title = "保存するプロファイルを選択してください";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            String SaveFileName = dlg.FileName;
            if (SaveProfile(SaveFileName))
            {
                UpdateProfileListAll(System.IO.Path.GetFileName(SaveFileName));
                SyncPlaylistFileItems();
            }
        }

        // *******************************************************************************
        // プレイリスト(タブ2): 複数の設定ファイルを指定した順番で連続再生する

        // 右クリックしたセルの行(プレイリスト側)。右クリック無しの状態(-1)なら末尾扱い
        private int contextMenuPlaylistRowIndex = -1;

        // trueなら、実行チェック(col_PlaylistEnabled)がONの行だけをプレイリストに表示する
        private Boolean showOnlyCheckedPlaylistRows = false;

        // プレイリストの設定ファイル列(col_PlaylistFile)を、comboBox_Profileと同じ内容に揃える。
        // ファイルシステムへの問い合わせはcomboBox_Profile側(util.UpdateProfileList)だけで行い、
        // その結果をそのままコピーするだけにすることで、同じ一覧を二重に取得しないようにしている。
        // comboBox_Profileが更新されるタイミング(起動時・設定値保存時)で必ずこれも呼ぶこと
        private void SyncPlaylistFileItems()
        {
            col_PlaylistFile.Items.Clear();
            foreach (Object item in comboBox_Profile.Items)
            {
                col_PlaylistFile.Items.Add(item);
            }
        }

        // プルダウン(col_PlaylistFile)に表示される全ファイルを、プレイリストに1行ずつまとめて
        // 追加する。既存の行は全部作り直す(手動で1件ずつ追加する手間を省くための一括操作)
        private void button_PlaylistListAll_Click(object sender, EventArgs e)
        {
            if (isRecording || isPlaying)
            {
                return;
            }

            dataGridView_Playlist.Rows.Clear();

            int insertAt = 0;
            foreach (Object item in col_PlaylistFile.Items)
            {
                AddPlaylistRow(insertAt, isEnabled: true);
                dataGridView_Playlist.Rows[insertAt].Cells[col_PlaylistFile.Index].Value = Convert.ToString(item);
                insertAt++;
            }
        }

        private void dataGridView_Playlist_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            contextMenuPlaylistRowIndex = e.RowIndex;

            // 記録グリッドと同様、右クリックしたセル/行がすでに選択済みなら選択状態を維持する
            Boolean isAlreadySelected = e.RowIndex >= 0 &&
                ((e.ColumnIndex >= 0 && dataGridView_Playlist.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
                || dataGridView_Playlist.Rows[e.RowIndex].Selected);

            if (e.RowIndex >= 0 && !isAlreadySelected)
            {
                dataGridView_Playlist.ClearSelection();
                dataGridView_Playlist.Rows[e.RowIndex].Selected = true;
            }
        }

        // *******************************************************************************
        // プレイリストの行のドラッグ&ドロップによる並び替え

        // 掴んだ(左ボタンを押した)行のインデックス。ドラッグ開始の起点にもする
        private int dragStartPlaylistRowIndex = -1;
        private Point dragStartPlaylistPoint;

        private void dataGridView_Playlist_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || isRecording || isPlaying)
            {
                return;
            }

            DataGridView.HitTestInfo hit = dataGridView_Playlist.HitTest(e.X, e.Y);
            dragStartPlaylistRowIndex = hit.RowIndex;
            dragStartPlaylistPoint = new Point(e.X, e.Y);
        }

        // マウスを一定距離動かして初めてドラッグとみなす(チェックボックスのクリック等が
        // 誤ってドラッグ扱いにならないようにするため)
        private void dataGridView_Playlist_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || dragStartPlaylistRowIndex < 0)
            {
                return;
            }

            if (Math.Abs(e.X - dragStartPlaylistPoint.X) < SystemInformation.DragSize.Width
                && Math.Abs(e.Y - dragStartPlaylistPoint.Y) < SystemInformation.DragSize.Height)
            {
                return;
            }

            dataGridView_Playlist.DoDragDrop(dragStartPlaylistRowIndex, DragDropEffects.Move);
            dragStartPlaylistRowIndex = -1;
        }

        private void dataGridView_Playlist_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(int)) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void dataGridView_Playlist_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(int)))
            {
                return;
            }

            int sourceIndex = (int)e.Data.GetData(typeof(int));

            Point clientPoint = dataGridView_Playlist.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo hit = dataGridView_Playlist.HitTest(clientPoint.X, clientPoint.Y);
            int targetIndex = hit.RowIndex;

            MovePlaylistRow(sourceIndex, targetIndex);
        }

        // dataGridView_PlaylistのsourceIndex行をtargetIndexの位置へ移動する。
        // DataGridViewは行そのものを並べ替える機能を持たないため、セルの値を丸ごとコピーして
        // 元の行を消し、新しい位置に作り直す形で実現する
        private void MovePlaylistRow(int sourceIndex, int targetIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= dataGridView_Playlist.Rows.Count
                || targetIndex < 0 || targetIndex >= dataGridView_Playlist.Rows.Count
                || sourceIndex == targetIndex)
            {
                return;
            }

            DataGridViewRow sourceRow = dataGridView_Playlist.Rows[sourceIndex];
            Object[] values = new Object[dataGridView_Playlist.Columns.Count];
            for (int c = 0; c < values.Length; c++)
            {
                values[c] = sourceRow.Cells[c].Value;
            }

            dataGridView_Playlist.Rows.RemoveAt(sourceIndex);

            // Insert(index)は「削除後の並びのindex番目の手前に挿入する」動きなので、
            // targetIndexをそのまま使えばドロップ先の行の位置に割り込む形になる
            // (下方向への移動時は元の配列基準の値をそのまま使うのが正しく、
            // 1引く補正を入れると1行分行き過ぎてしまっていた)
            int insertAt = targetIndex;

            dataGridView_Playlist.Rows.Insert(insertAt, 1);
            DataGridViewRow newRow = dataGridView_Playlist.Rows[insertAt];
            for (int c = 0; c < values.Length; c++)
            {
                newRow.Cells[c].Value = values[c];
            }

            dataGridView_Playlist.ClearSelection();
            newRow.Selected = true;
        }

        private void contextMenuStrip_Playlist_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isRecording || isPlaying)
            {
                e.Cancel = true;
                return;
            }

            menuItem_PlaylistDeleteRow.Enabled = contextMenuPlaylistRowIndex >= 0 && contextMenuPlaylistRowIndex < dataGridView_Playlist.Rows.Count;

            // 今どちらの表示モードか一目で分かるように、選択中の方にチェックを付ける
            menuItem_PlaylistShowCheckedOnly.Checked = showOnlyCheckedPlaylistRows;
            menuItem_PlaylistShowAll.Checked = !showOnlyCheckedPlaylistRows;
        }

        private void menuItem_PlaylistShowCheckedOnly_Click(object sender, EventArgs e)
        {
            showOnlyCheckedPlaylistRows = true;
            ApplyPlaylistRowFilter();
        }

        private void menuItem_PlaylistShowAll_Click(object sender, EventArgs e)
        {
            showOnlyCheckedPlaylistRows = false;
            ApplyPlaylistRowFilter();
        }

        // 実行列(col_PlaylistEnabled)のチェック状態を見て、プレイリストの行の表示/非表示を切り替える
        private void ApplyPlaylistRowFilter()
        {
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                Boolean isEnabled = Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false);
                row.Visible = !showOnlyCheckedPlaylistRows || isEnabled;
            }
        }

        private void menuItem_PlaylistAddRow_Click(object sender, EventArgs e)
        {
            int insertAt = (contextMenuPlaylistRowIndex >= 0) ? contextMenuPlaylistRowIndex + 1 : dataGridView_Playlist.Rows.Count;
            // 右クリックで能動的に追加した行は、すぐ使うつもりのはずなので実行チェックはONにしておく
            AddPlaylistRow(insertAt, isEnabled: true);
        }

        // プレイリストに1行追加する(ループ回数=1がデフォルト。実行チェックの初期値はisEnabledで指定)。
        // 右クリックメニューの「行の追加」と、起動時の初期空行の両方から使う
        private void AddPlaylistRow(int insertAt, Boolean isEnabled)
        {
            dataGridView_Playlist.Rows.Insert(insertAt, 1);

            DataGridViewRow newRow = dataGridView_Playlist.Rows[insertAt];
            newRow.Cells[col_PlaylistEnabled.Index].Value = isEnabled;
            newRow.Cells[col_PlaylistLoopCount.Index].Value = "1";
            newRow.Visible = !showOnlyCheckedPlaylistRows || isEnabled;
        }

        private void menuItem_PlaylistDeleteRow_Click(object sender, EventArgs e)
        {
            List<int> targetIndexes = GetSelectedOrContextMenuRowIndexes(dataGridView_Playlist, contextMenuPlaylistRowIndex);

            foreach (int idx in targetIndexes.OrderByDescending(x => x))
            {
                dataGridView_Playlist.Rows.RemoveAt(idx);
            }
        }

        private void menuItem_PlaylistCheckAll_Click(object sender, EventArgs e)
        {
            SetAllPlaylistChecks(true);
        }

        private void menuItem_PlaylistUncheckAll_Click(object sender, EventArgs e)
        {
            SetAllPlaylistChecks(false);
        }

        private void SetAllPlaylistChecks(Boolean isChecked)
        {
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                row.Cells[col_PlaylistEnabled.Index].Value = isChecked;
            }

            ApplyPlaylistRowFilter();
        }

        // チェックボックス列は、クリックしただけだと確定(コミット)されず見た目が変わらないことがあるので、
        // 値が変化した直後に明示的にコミットして即座に反映させる
        private void dataGridView_Playlist_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // チェックボックスと設定ファイルのプルダウンは、選んだ瞬間に値を確定させたい
            // (プルダウンの方は、選択直後にCellValueChangedでループ回数の初期値セットに使うため)
            Boolean isImmediateCommitTarget = dataGridView_Playlist.CurrentCell is DataGridViewCheckBoxCell
                || dataGridView_Playlist.CurrentCell is DataGridViewComboBoxCell;

            if (isImmediateCommitTarget && dataGridView_Playlist.IsCurrentCellDirty)
            {
                dataGridView_Playlist.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // プレイリストの行で設定ファイルを選ぶと、そのファイル自身に保存されている
        // ループ回数を、行のループ回数セルへ初期値として自動セットする
        // (すでに手で入力済みの値を上書きしたくはないので、あくまで選択した瞬間の初期値扱い)
        private void dataGridView_Playlist_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == col_PlaylistEnabled.Index)
            {
                // チェックON/OFFが変わったら、表示フィルタが有効な時はその場で表示/非表示を切り替える
                DataGridViewRow row = dataGridView_Playlist.Rows[e.RowIndex];
                Boolean isEnabled = Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false);
                row.Visible = !showOnlyCheckedPlaylistRows || isEnabled;
                return;
            }

            if (e.ColumnIndex != col_PlaylistFile.Index)
            {
                return;
            }

            String fileName = Convert.ToString(dataGridView_Playlist.Rows[e.RowIndex].Cells[col_PlaylistFile.Index].Value);
            if (String.IsNullOrEmpty(fileName))
            {
                return;
            }

            String fullPath = System.IO.Path.Combine(userDataFolder, fileName);
            String savedLoopCount = ReadSavedLoopCount(fullPath);
            if (savedLoopCount != null)
            {
                dataGridView_Playlist.Rows[e.RowIndex].Cells[col_PlaylistLoopCount.Index].Value = savedLoopCount;
            }
        }

        // マクロファイル自身が保存しているループ回数(textBox_Loopの値)だけを、
        // dataGridView_Events等には一切触れずに読み取る
        // (LoadProfileを使うとタブ1の記録データがまるごとクリア/差し替えられてしまうため使わない)
        private String ReadSavedLoopCount(String filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            if (IsJsonFile(filePath))
            {
                try
                {
                    EventRecorderProfile profile = JsonFileStorage.Load<EventRecorderProfile>(filePath);
                    return profile?.LoopCount;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            try
            {
                System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(filePath);
                System.Xml.Linq.XElement el = doc.Root?.Elements("Setting")
                    .FirstOrDefault(x => (String)x.Attribute("Record") == "textBox_Loop");
                return el != null ? el.Value.Trim() : null;
            }
            catch (System.Xml.XmlException)
            {
                return null;
            }
        }

        // プレイリストの1行分(ファイル名+そのファイル専用のループ回数)
        private class PlaylistEntry
        {
            public String FileName;
            public int LoopCount;

            // dataGridView_Playlist上の元の行インデックス。実行中にその行をハイライトするために使う
            // (チェックが外れている行等は除外されるため、entries内のインデックスとは一致しない)
            public int RowIndex;
        }

        // プレイリストの各行のうち、チェックが入っていてファイルが選ばれている行だけを、
        // 上から順番に取り出す(チェックが外れている行・ファイル未選択の行は実行対象から除外)
        private List<PlaylistEntry> GetPlaylistEntries()
        {
            List<PlaylistEntry> entries = new List<PlaylistEntry>();
            foreach (DataGridViewRow row in dataGridView_Playlist.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                Boolean isEnabled = Convert.ToBoolean(row.Cells[col_PlaylistEnabled.Index].Value ?? false);
                if (!isEnabled)
                {
                    continue;
                }

                String fileName = Convert.ToString(row.Cells[col_PlaylistFile.Index].Value);
                if (String.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                int loopCount = util.GetInteger(Convert.ToString(row.Cells[col_PlaylistLoopCount.Index].Value));
                if (loopCount <= 0)
                {
                    loopCount = 1;
                }

                entries.Add(new PlaylistEntry() { FileName = fileName, LoopCount = loopCount, RowIndex = row.Index });
            }

            return entries;
        }

        // プレイリストグループの内容を実行する(単発再生とbutton_Playを共有しているため、
        // 呼び出し元はPlayOrStop。isPlaying/isRecordingのチェックは呼び出し元で済んでいる)
        private void StartPlaylistRun()
        {
            List<PlaylistEntry> entries = GetPlaylistEntries();
            if (entries.Count == 0)
            {
                return;
            }

            // 「全体ループ」も単発再生の「ループ回数」とtextBox_Loopを共有している
            int overallLoopCount = util.GetInteger(textBox_Loop.Text);
            if (overallLoopCount <= 0)
            {
                overallLoopCount = 1;
            }

            cursorPositionBeforePlay = Cursor.Position;

            playbackOverallLoopNo = 0;
            playbackOverallLoopMax = overallLoopCount;
            playbackInnerLoopNo = 0;
            playbackInnerLoopMax = 0;

            isPlaying = true;
            stopPlayRequested = false;
            UpdatePlayButtons();
            UpdateTitle();
            MinimizeIfRequested();

            Task.Run(() => PlaylistPlayLoop(entries, overallLoopCount));
        }

        // プレイリスト全体をoverallLoopCount回繰り返す。各周回の中で、
        // リストの各行を上から順に読み込み→その行のループ回数分だけ再生、を繰り返す
        private void PlaylistPlayLoop(List<PlaylistEntry> entries, int overallLoopCount)
        {
            try
            {
                for (int loopNo = 0; loopNo < overallLoopCount && !stopPlayRequested; loopNo++)
                {
                    int loopDisplayNo = loopNo + 1;
                    playbackOverallLoopNo = loopDisplayNo;

                    for (int i = 0; i < entries.Count && !stopPlayRequested; i++)
                    {
                        PlaylistEntry entry = entries[i];
                        int fileNo = i + 1;
                        List<String[]> rows = null;

                        this.Invoke((MethodInvoker)(() =>
                        {
                            label_PlaylistStatus.Text = "実行中(全体" + loopDisplayNo + "/" + overallLoopCount + "): "
                                + entry.FileName + " (" + fileNo + "/" + entries.Count + ")";

                            // 今どのファイル(行)を再生しているか、プレイリスト側もハイライトする
                            HighlightPlaylistRow(entry.RowIndex);

                            String fullPath = System.IO.Path.Combine(userDataFolder, entry.FileName);
                            // 再生対象の記録データ(タブ1)だけ差し替える。プレイリスト自体(タブ2)には
                            // 影響しない(XML/JSONどちらの形式でも、記録データのみを反映する)
                            LoadProfileForPlayback(fullPath);

                            // 読み込んだファイルに再生できない行が無いか確認する。あればプレイリスト
                            // 全体を停止する(rowsはnullのまま=このエントリは再生しない)
                            if (!ValidateEventsForPlayback())
                            {
                                stopPlayRequested = true;
                                return;
                            }

                            rows = SnapshotRows();
                        }));

                        if (rows != null && rows.Count > 0)
                        {
                            PlayRows(rows, entry.LoopCount);
                        }
                    }
                }

                Cursor.Position = cursorPositionBeforePlay;
            }
            finally
            {
                // 途中で例外が起きても、必ず「再生中」状態を解除する。
                // ここを素通りしてしまうとisPlayingがtrueのまま固まり、記録も再生も
                // 二度とできなくなる(アプリが固まって見える不具合の原因になっていた)
                isPlaying = false;
                stopPlayRequested = false;
                this.Invoke((MethodInvoker)(() =>
                {
                    UpdatePlayButtons();
                    label_PlaylistStatus.Text = "";
                    UpdateTitle();
                    HighlightPlayingRow(-1);
                    HighlightPlaylistRow(-1);
                    RestoreIfMinimizedByPlay();
                }));
            }
        }
    }
}
