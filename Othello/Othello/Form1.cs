using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Othello
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 対戦モード。C++版 PLAYER_PLAYER/PLAYER_COM/COM_PLAYER/COM_COM 相当。
        /// PC/CPは「先に書かれている方が黒(先手)」という命名。
        /// </summary>
        private enum PlayMode
        {
            PlayerPlayer,
            PlayerCom,
            ComPlayer,
            ComCom,
        }

        /// <summary>
        /// 対局の進行状態。C++版 GAME_INIT/GAME_PLAY/GAME_STOP/GAME_END 相当。
        /// 持ち時間のカウントダウンとCOMの自動着手は Play の間だけ動く。
        /// </summary>
        private enum GameState
        {
            Init,
            Play,
            Stop,
            End,
        }

        private GameState _gameState = GameState.Init;

        private readonly BoardRenderer _renderer = new BoardRenderer();
        private readonly GameMaster _gm = new GameMaster();
        private PlayMode _playMode = PlayMode.PlayerPlayer;
        private int _comLevel = 1;

        // 持ち時間(秒)。-1は「なし」。C++版 time_limit 相当。
        private int _timeLimitSeconds = -1;
        private int _blackTimeMs;
        private int _whiteTimeMs;
        // 時間切れで終局したかどうか(GameMaster.IsGameEndとは別に管理する)
        private bool _isTimedOut;

        // COMの手を少し間を置いてから打つためのタイマー(人間の手と同じ速さで即打つと
        // 何が起きたか分かりにくいため)。
        private readonly Timer _comMoveTimer = new Timer { Interval = 150 };

        // 持ち時間のカウントダウン用タイマー。C++版 COUNT_DOWN_CYC(100ms)相当。
        // 常に「今の手番」の残り時間を減らす(手番が変わればおのずと対象も切り替わる)。
        private readonly Timer _countdownTimer = new Timer { Interval = 100 };

        // pictureBoxFieldの「上下の余白の合計」と「左右の余白の合計」の差。
        // リサイズ中にウィンドウの高さを幅から逆算し、盤面(pictureBoxField)が
        // 常に正方形になるようにするために使う(WM_SIZINGで利用)。
        private int _boardHeightOffset;

        private const string _kifuFileFilter = "棋譜ファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*";

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();

            // 初期デザインのサイズを下限にする(C++版のWIN_MIN_SIZE相当)。
            // これより小さくはリサイズできないようにし、盤面が0サイズになる事態を防ぐ。
            this.MinimumSize = this.Size;

            int leftGap = pictureBoxField.Left;
            int rightGap = this.ClientSize.Width - pictureBoxField.Right;
            int topGap = pictureBoxField.Top;
            int bottomGap = this.ClientSize.Height - pictureBoxField.Bottom;
            _boardHeightOffset = (topGap + bottomGap) - (leftGap + rightGap);

            _comMoveTimer.Tick += ComMoveTimer_Tick;
            _countdownTimer.Tick += CountdownTimer_Tick;

            ResetGame();

            _renderer.SetDrawArea(pictureBoxField);
            RedrawBoard();
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(button_ReStart, "確認のうえ初期局面に戻す。対戦モード・COMレベル・持ち時間の設定はそのまま");

            menuStrip1.ShowItemToolTips = true;

            menuItem_Start.ToolTipText = "開始前・停止中なら対局を(再)開する。終局後なら新しい対局を始める";
            menuItem_Stop.ToolTipText = "持ち時間のカウントダウンとCOMの着手を止める。再開は「開始」";
            menuItem_Undo.ToolTipText = "1手戻して停止状態にする。続けるには「開始」";
            menuItem_Redo.ToolTipText = "戻した手を1手やり直して停止状態にする。続けるには「開始」";

            menuItem_PP.ToolTipText = "選ぶと新しい対局になる";
            menuItem_PC.ToolTipText = "人が黒(先手)、COMが白。選ぶと新しい対局になる";
            menuItem_CP.ToolTipText = "COMが黒(先手)、人が白。選ぶと新しい対局になる";
            menuItem_CC.ToolTipText = "選ぶと新しい対局になる";

            menuItem_ComLevel1.ToolTipText = "盤の中心に近い手を優先する";
            menuItem_ComLevel2.ToolTipText = "序盤は中心寄り、中盤は返す石が少ない手、終盤は多い手を優先する";
            menuItem_ComLevel3.ToolTipText = "打った後に周りの空きマスが少ない(開放度が低い)手を優先する";

            menuItem_TimeLimit.ToolTipText = "黒白それぞれの1局全体の持ち時間。先に0になった側の負け。対局中(開始〜終局)は変更できず、選び直すと新しい対局になる";

            menuItem_KifuSave.ToolTipText = "ここまでの手順を「手数 : 列.行 色」形式(例: 1 : C.4 黒)のテキストで保存する";
            menuItem_KifuLoad.ToolTipText = "棋譜テキストを最初から再生した局面にする。置けない手があればそこで打ち切る";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const int WM_SIZING = 0x0214;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;

        /// <summary>
        /// ドラッグでのリサイズ中(WM_SIZING)に、幅から高さを逆算して盤面(pictureBoxField)が
        /// 常に正方形になるよう、OSがこれから適用しようとしているウィンドウ矩形を書き換える。
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SIZING)
            {
                AdjustResizingRectForSquareBoard(m);
            }

            base.WndProc(ref m);
        }

        private void AdjustResizingRectForSquareBoard(Message m)
        {
            RECT rect = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));

            int borderWidth = this.Width - this.ClientSize.Width;
            int borderHeight = this.Height - this.ClientSize.Height;

            int outerWidth = rect.Right - rect.Left;
            int clientWidth = outerWidth - borderWidth;

            int desiredClientHeight = clientWidth + _boardHeightOffset;
            int desiredOuterHeight = desiredClientHeight + borderHeight;

            // 最小サイズより小さくはしない
            desiredOuterHeight = Math.Max(desiredOuterHeight, this.MinimumSize.Height);

            int edge = m.WParam.ToInt32();
            bool draggingTopEdge = edge == WMSZ_TOP || edge == WMSZ_TOPLEFT || edge == WMSZ_TOPRIGHT;

            if (draggingTopEdge)
            {
                // 上端をドラッグしている時は下端を固定し、上端の位置を合わせる
                rect.Top = rect.Bottom - desiredOuterHeight;
            }
            else
            {
                // それ以外(左右・下・左下・右下)は上端を固定し、下端の位置を合わせる
                rect.Bottom = rect.Top + desiredOuterHeight;
            }

            Marshal.StructureToPtr(rect, m.LParam, true);
        }

        /// <summary>
        /// 新規対局を始める(盤面・手番・持ち時間・時間切れ状態を初期化する)。
        /// </summary>
        private void ResetGame()
        {
            _comMoveTimer.Stop();
            _countdownTimer.Stop();

            _gm.Initialize();
            _isTimedOut = false;
            _gameState = GameState.Init;
            ResetRemainingTime();
            UpdateMenuState();
        }

        /// <summary>
        /// 黒・白の残り時間を持ち時間の設定値に戻す(持ち時間「なし」なら0)。
        /// </summary>
        private void ResetRemainingTime()
        {
            int initialTimeMs = _timeLimitSeconds < 0 ? 0 : _timeLimitSeconds * 1000;
            _blackTimeMs = initialTimeMs;
            _whiteTimeMs = initialTimeMs;
        }

        /// <summary>
        /// メニュー「ゲーム」→「一手戻す」。C++版 VersProc/OnMenuitemVers 相当
        /// (このプロジェクトではUndoに相当する側)。
        /// </summary>
        private void menuItem_Undo_Click(object sender, EventArgs e)
        {
            _comMoveTimer.Stop();
            if (_gm.Undo())
            {
                _isTimedOut = false;
                _gameState = GameState.Stop;
                RedrawBoard();
            }
        }

        /// <summary>
        /// メニュー「ゲーム」→「一手進める」。Undoで戻した手をやり直す(Redo)。
        /// C++版 ReVersProc/OnMenuitemRevers 相当。
        /// </summary>
        private void menuItem_Redo_Click(object sender, EventArgs e)
        {
            _comMoveTimer.Stop();
            if (_gm.Redo())
            {
                _isTimedOut = false;
                _gameState = GameState.Stop;
                RedrawBoard();
            }
        }

        private void button_ReStart_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
                "初期画面にもどります。よろしいですか？",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                ResetGame();
                RedrawBoard();
            }
        }

        /// <summary>
        /// メニュー「ゲーム」→「開始」。開始前・停止中なら(再)開し、終局後なら新規対局を始める。
        /// C++版 OnMenuitemStart/StartProc 相当。
        /// </summary>
        private void menuItem_Start_Click(object sender, EventArgs e)
        {
            if (_gameState == GameState.Play)
            {
                return;
            }

            if (_gameState == GameState.End)
            {
                ResetGame();
            }

            _gameState = GameState.Play;
            RedrawBoard();
        }

        /// <summary>
        /// メニュー「ゲーム」→「停止」。持ち時間のカウントダウンとCOMの着手を止める。
        /// 再開は「開始」。C++版 OnMenuitemStop 相当。
        /// </summary>
        private void menuItem_Stop_Click(object sender, EventArgs e)
        {
            if (_gameState != GameState.Play)
            {
                return;
            }

            _gameState = GameState.Stop;
            RedrawBoard();
        }

        private void menuItem_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuItem_HowToPlay_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "・黒が先手です。\n" +
                "・自分の石で相手の石を挟める場所をクリックすると石を置けます。\n" +
                "・挟まれた相手の石は自分の色にひっくり返ります。\n" +
                "・置ける場所が無い場合は自動的にパスされます。\n" +
                "・双方とも置ける場所が無くなったら終局、石数の多い方が勝ちです。",
                "遊び方",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void menuItem_Version_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Othello (C#/WinForms版)\nC++版(MFC)からの移植",
                "バージョン情報",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// メニュー「対戦モード」。選び直したら新規対局から始める。
        /// </summary>
        private void menuItem_PlayMode_Click(object sender, EventArgs e)
        {
            var clicked = (ToolStripMenuItem)sender;

            if (clicked == menuItem_PP)
            {
                _playMode = PlayMode.PlayerPlayer;
            }
            else if (clicked == menuItem_PC)
            {
                _playMode = PlayMode.PlayerCom;
            }
            else if (clicked == menuItem_CP)
            {
                _playMode = PlayMode.ComPlayer;
            }
            else if (clicked == menuItem_CC)
            {
                _playMode = PlayMode.ComCom;
            }

            menuItem_PP.Checked = clicked == menuItem_PP;
            menuItem_PC.Checked = clicked == menuItem_PC;
            menuItem_CP.Checked = clicked == menuItem_CP;
            menuItem_CC.Checked = clicked == menuItem_CC;

            ResetGame();
            RedrawBoard();
        }

        /// <summary>
        /// メニュー「COMレベル」。
        /// </summary>
        private void menuItem_ComLevel_Click(object sender, EventArgs e)
        {
            var clicked = (ToolStripMenuItem)sender;

            if (clicked == menuItem_ComLevel1)
            {
                _comLevel = 1;
            }
            else if (clicked == menuItem_ComLevel2)
            {
                _comLevel = 2;
            }
            else if (clicked == menuItem_ComLevel3)
            {
                _comLevel = 3;
            }

            menuItem_ComLevel1.Checked = clicked == menuItem_ComLevel1;
            menuItem_ComLevel2.Checked = clicked == menuItem_ComLevel2;
            menuItem_ComLevel3.Checked = clicked == menuItem_ComLevel3;
        }

        /// <summary>
        /// メニュー「持ち時間」。選び直したら新規対局から始める。
        /// </summary>
        private void menuItem_TimeLimit_Click(object sender, EventArgs e)
        {
            var clicked = (ToolStripMenuItem)sender;

            if (clicked == menuItem_TimeNone)
            {
                _timeLimitSeconds = -1;
            }
            else if (clicked == menuItem_Time30s)
            {
                _timeLimitSeconds = 30;
            }
            else if (clicked == menuItem_Time1m)
            {
                _timeLimitSeconds = 60;
            }
            else if (clicked == menuItem_Time2m)
            {
                _timeLimitSeconds = 120;
            }
            else if (clicked == menuItem_Time3m)
            {
                _timeLimitSeconds = 180;
            }
            else if (clicked == menuItem_Time5m)
            {
                _timeLimitSeconds = 300;
            }
            else if (clicked == menuItem_Time10m)
            {
                _timeLimitSeconds = 600;
            }
            else if (clicked == menuItem_Time15m)
            {
                _timeLimitSeconds = 900;
            }

            menuItem_TimeNone.Checked = clicked == menuItem_TimeNone;
            menuItem_Time30s.Checked = clicked == menuItem_Time30s;
            menuItem_Time1m.Checked = clicked == menuItem_Time1m;
            menuItem_Time2m.Checked = clicked == menuItem_Time2m;
            menuItem_Time3m.Checked = clicked == menuItem_Time3m;
            menuItem_Time5m.Checked = clicked == menuItem_Time5m;
            menuItem_Time10m.Checked = clicked == menuItem_Time10m;
            menuItem_Time15m.Checked = clicked == menuItem_Time15m;

            label_Time.Visible = _timeLimitSeconds >= 0;

            ResetGame();
            RedrawBoard();
        }

        /// <summary>
        /// メニュー「棋譜」→「表示」。ここまでの手順をメッセージボックスに表示する。
        /// </summary>
        private void menuItem_KifuShow_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Kifu.ToText(_gm.History), "棋譜", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// メニュー「棋譜」→「保存」。ここまでの手順をテキストファイルに保存する。
        /// </summary>
        private void menuItem_KifuSave_Click(object sender, EventArgs e)
        {
            if (_gm.History.Count == 0)
            {
                MessageBox.Show("まだ1手も打たれていないよ。", "棋譜の保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = _kifuFileFilter;
                dialog.FileName = "Othello_kifu.txt";

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(dialog.FileName, Kifu.ToText(_gm.History));
                }
            }
        }

        /// <summary>
        /// メニュー「棋譜」→「読込」。テキストファイルから手順を読み込み、最初から再生する。
        /// </summary>
        private void menuItem_KifuLoad_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = _kifuFileFilter;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                string text;
                try
                {
                    text = System.IO.File.ReadAllText(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("棋譜ファイルを読み込めなかったよ。\n" + ex.Message, "棋譜の読込", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _comMoveTimer.Stop();
                _countdownTimer.Stop();

                bool ok = Kifu.TryReplay(text, _gm, out string errorMessage);
                _isTimedOut = false;
                _gameState = GameState.Init;
                ResetRemainingTime();

                RedrawBoard();

                if (!ok)
                {
                    MessageBox.Show(errorMessage ?? "棋譜を読み込めなかったよ。", "棋譜の読込", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// 現在の対戦モードで、colorがCOM操作かどうか。
        /// </summary>
        private bool IsComTurn(StoneColor color)
        {
            switch (_playMode)
            {
                case PlayMode.PlayerCom:
                    return color == StoneColor.White;
                case PlayMode.ComPlayer:
                    return color == StoneColor.Black;
                case PlayMode.ComCom:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 現在の手番がCOMなら、少し間を置いてからCOMに一手打たせる。
        /// </summary>
        private void MaybeTriggerComMove()
        {
            if (_gameState == GameState.Play && !_gm.IsGameEnd && IsComTurn(_gm.CurrentTurn))
            {
                _comMoveTimer.Start();
            }
        }

        private void ComMoveTimer_Tick(object sender, EventArgs e)
        {
            _comMoveTimer.Stop();

            if (_gameState != GameState.Play || _isTimedOut || _gm.IsGameEnd || !IsComTurn(_gm.CurrentTurn))
            {
                return;
            }

            if (ComPlayer.TryGetMove(_gm, _gm.CurrentTurn, _comLevel, out int x, out int y))
            {
                _gm.TryPut(x, y);
            }

            RedrawBoard();
        }

        /// <summary>
        /// 持ち時間のカウントダウン。今の手番の残り時間を100msずつ減らし、
        /// 0になったらその手番の時間切れ負けとして対局を止める(C++版 OnTimer/TimeOutProc)。
        /// </summary>
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (_gm.CurrentTurn == StoneColor.Black)
            {
                _blackTimeMs -= _countdownTimer.Interval;
            }
            else
            {
                _whiteTimeMs -= _countdownTimer.Interval;
            }

            UpdateTimeLabel();

            if (_blackTimeMs <= 0 || _whiteTimeMs <= 0)
            {
                _countdownTimer.Stop();
                _comMoveTimer.Stop();
                _isTimedOut = true;
                UpdateStatusLabel();
            }
        }

        private void pictureBoxField_MouseClick(object sender, MouseEventArgs e)
        {
            if (_isTimedOut || _gm.IsGameEnd)
            {
                return;
            }

            // COMの手番中(タイマー待ち)は人間のクリックを受け付けない
            if (IsComTurn(_gm.CurrentTurn))
            {
                return;
            }

            if (pictureBoxField.Width <= 0 || pictureBoxField.Height <= 0)
            {
                return;
            }

            // BoardRenderer側の罫線描画(「全体*i/8」で位置を計算)と同じ考え方でマス目を求める。
            // 先に1マス分の幅を割ってしまうと、盤面のサイズによっては罫線とクリック判定の
            // マス目がわずかにズレることがあるため、掛け算してから割る。
            int x = e.X * GameMaster.BoardSize / pictureBoxField.Width;
            int y = e.Y * GameMaster.BoardSize / pictureBoxField.Height;
            if (x < 0 || x >= GameMaster.BoardSize || y < 0 || y >= GameMaster.BoardSize)
            {
                return;
            }

            if (_gameState == GameState.Stop)
            {
                MessageBox.Show("「ゲーム」→「開始」を押してね", "停止中", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_gm.TryPut(x, y))
            {
                // 開始前に最初の手を打ったら、そのまま対局開始(C++版 GAME_INIT 時の挙動)
                if (_gameState == GameState.Init)
                {
                    _gameState = GameState.Play;
                }

                RedrawBoard();
            }
        }

        // pictureBoxField.Imageを差し替える(CreateCanvas/DrawField内)と、その場で
        // PictureBoxのAnchorレイアウトが再計算され、コントロールのサイズが一瞬古い値に
        // 巻き戻ってしまうことがある(WinFormsのレイアウトエンジンの癖)。
        // Resizeイベントの中で即座に描画し直すとこの再計算に巻き込まれてしまうため、
        // BeginInvokeで「今回のリサイズ処理が完全に終わった後」まで描画を遅延させる。
        private bool _redrawScheduled = false;

        private void pictureBoxField_Resize(object sender, EventArgs e)
        {
            if (_redrawScheduled)
            {
                return;
            }

            _redrawScheduled = true;
            BeginInvoke((MethodInvoker)delegate
            {
                _redrawScheduled = false;

                if (pictureBoxField.Width <= 0 || pictureBoxField.Height <= 0)
                {
                    return;
                }

                _renderer.CreateCanvas();
                RedrawBoard();
            });
        }

        /// <summary>
        /// 盤面・置ける場所のマーク・手番表示をまとめて再描画し、COMの手番なら次の一手を予約する。
        /// 終局している場合や、これから打つのがCOMの場合は置ける場所のマークは出さない
        /// (マークは「あなたがここに置けるよ」という案内なので)。
        /// </summary>
        private void RedrawBoard()
        {
            if (_isTimedOut || _gm.IsGameEnd)
            {
                _gameState = GameState.End;
            }

            bool showNotice = !_isTimedOut && !_gm.IsGameEnd && !IsComTurn(_gm.CurrentTurn);
            bool[,] validMoves = showNotice ? _gm.GetValidMoves(_gm.CurrentTurn) : null;
            _renderer.DrawField(_gm.Table, validMoves);
            UpdateStatusLabel();
            UpdateTimeLabel();

            menuItem_Undo.Enabled = _gm.CanUndo;
            menuItem_Redo.Enabled = _gm.CanRedo;

            UpdateMenuState();

            if (_gameState == GameState.Play && _timeLimitSeconds >= 0)
            {
                _countdownTimer.Start();
            }
            else
            {
                _countdownTimer.Stop();
            }

            MaybeTriggerComMove();
        }

        /// <summary>
        /// 対局の進行状態に合わせて、開始/停止/持ち時間メニューの有効無効を切り替える。
        /// 持ち時間は対局中(進行中・停止中)は変更できない(C++版 Constraints 相当)。
        /// </summary>
        private void UpdateMenuState()
        {
            menuItem_Start.Enabled = _gameState != GameState.Play;
            menuItem_Stop.Enabled = _gameState == GameState.Play;
            menuItem_TimeLimit.Enabled = _gameState == GameState.Init || _gameState == GameState.End;
        }

        /// <summary>
        /// 持ち時間の残りをlabel_Timeに表示する(持ち時間「なし」なら何も表示しない)。
        /// </summary>
        private void UpdateTimeLabel()
        {
            if (_timeLimitSeconds < 0)
            {
                return;
            }

            int blackSec = Math.Max(0, _blackTimeMs) / 1000;
            int whiteSec = Math.Max(0, _whiteTimeMs) / 1000;
            label_Time.Text = string.Format("残り時間 黒:{0}秒 白:{1}秒", blackSec, whiteSec);
        }

        /// <summary>
        /// 手番・石数・終局結果をlabel_Statusに表示する
        /// </summary>
        private void UpdateStatusLabel()
        {
            _gm.CountStones(out int blackCount, out int whiteCount);

            if (_isTimedOut)
            {
                string loserName = _blackTimeMs <= 0 ? "黒" : "白";
                string winnerName = _blackTimeMs <= 0 ? "白" : "黒";
                label_Status.Text = string.Format("時間切れ({0}) {1}の勝ち 黒:{2} 白:{3}", loserName, winnerName, blackCount, whiteCount);
                return;
            }

            if (_gm.IsGameEnd)
            {
                string winner;
                if (blackCount > whiteCount)
                {
                    winner = "黒の勝ち";
                }
                else if (whiteCount > blackCount)
                {
                    winner = "白の勝ち";
                }
                else
                {
                    winner = "引き分け";
                }

                label_Status.Text = string.Format("終局 黒:{0} 白:{1} {2}", blackCount, whiteCount, winner);
            }
            else
            {
                string turnName = _gm.CurrentTurn == StoneColor.Black ? "黒" : "白";
                string stopped = _gameState == GameState.Stop ? " [停止中]" : "";
                label_Status.Text = string.Format("{0}の番 (黒:{1} 白:{2}){3}", turnName, blackCount, whiteCount, stopped);
            }
        }
    }
}
