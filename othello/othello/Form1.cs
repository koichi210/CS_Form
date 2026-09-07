using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace othello
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

        private Draw draw = new Draw();
        private GameMaster gm = new GameMaster();
        private PlayMode playMode = PlayMode.PlayerPlayer;
        private int comLevel = 1;

        // 持ち時間(秒)。-1は「なし」。C++版 time_limit 相当。
        private int timeLimitSeconds = -1;
        private int blackTimeMs;
        private int whiteTimeMs;
        // 時間切れで終局したかどうか(GameMaster.IsGameEndとは別に管理する)
        private bool isTimedOut;

        // COMの手を少し間を置いてから打つためのタイマー(人間の手と同じ速さで即打つと
        // 何が起きたか分かりにくいため)。
        private readonly Timer comMoveTimer = new Timer { Interval = 500 };

        // 持ち時間のカウントダウン用タイマー。C++版 COUNT_DOWN_CYC(100ms)相当。
        // 常に「今の手番」の残り時間を減らす(手番が変わればおのずと対象も切り替わる)。
        private readonly Timer countdownTimer = new Timer { Interval = 100 };

        public Form1()
        {
            InitializeComponent();

            // 初期デザインのサイズを下限にする(C++版のWIN_MIN_SIZE相当)。
            // これより小さくはリサイズできないようにし、盤面が0サイズになる事態を防ぐ。
            this.MinimumSize = this.Size;

            comMoveTimer.Tick += ComMoveTimer_Tick;
            countdownTimer.Tick += CountdownTimer_Tick;

            ResetGame();

            draw.SetDrawArea(pictureBoxField);
            RedrawBoard();
        }

        /// <summary>
        /// 新規対局を始める(盤面・手番・持ち時間・時間切れ状態を初期化する)。
        /// </summary>
        private void ResetGame()
        {
            comMoveTimer.Stop();
            countdownTimer.Stop();

            gm.Initialize();
            isTimedOut = false;
            blackTimeMs = timeLimitSeconds < 0 ? 0 : timeLimitSeconds * 1000;
            whiteTimeMs = timeLimitSeconds < 0 ? 0 : timeLimitSeconds * 1000;
        }

        private void button_ReStart_Click(object sender, EventArgs e)
        {
            DialogResult DlgResult = MessageBox.Show(
                "初期画面にもどります。よろしいですか？",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (DlgResult == DialogResult.Yes)
            {
                ResetGame();
                RedrawBoard();
            }
        }

        /// <summary>
        /// メニュー「ゲーム」→「開始」。確認なしで新規対局を始める。
        /// </summary>
        private void menuItem_Start_Click(object sender, EventArgs e)
        {
            ResetGame();
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
                "othello (C#/WinForms版)\nC++版(MFC)からの移植",
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
                playMode = PlayMode.PlayerPlayer;
            }
            else if (clicked == menuItem_PC)
            {
                playMode = PlayMode.PlayerCom;
            }
            else if (clicked == menuItem_CP)
            {
                playMode = PlayMode.ComPlayer;
            }
            else if (clicked == menuItem_CC)
            {
                playMode = PlayMode.ComCom;
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
                comLevel = 1;
            }
            else if (clicked == menuItem_ComLevel2)
            {
                comLevel = 2;
            }
            else if (clicked == menuItem_ComLevel3)
            {
                comLevel = 3;
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
                timeLimitSeconds = -1;
            }
            else if (clicked == menuItem_Time30s)
            {
                timeLimitSeconds = 30;
            }
            else if (clicked == menuItem_Time1m)
            {
                timeLimitSeconds = 60;
            }
            else if (clicked == menuItem_Time2m)
            {
                timeLimitSeconds = 120;
            }
            else if (clicked == menuItem_Time3m)
            {
                timeLimitSeconds = 180;
            }
            else if (clicked == menuItem_Time5m)
            {
                timeLimitSeconds = 300;
            }
            else if (clicked == menuItem_Time10m)
            {
                timeLimitSeconds = 600;
            }
            else if (clicked == menuItem_Time15m)
            {
                timeLimitSeconds = 900;
            }

            menuItem_TimeNone.Checked = clicked == menuItem_TimeNone;
            menuItem_Time30s.Checked = clicked == menuItem_Time30s;
            menuItem_Time1m.Checked = clicked == menuItem_Time1m;
            menuItem_Time2m.Checked = clicked == menuItem_Time2m;
            menuItem_Time3m.Checked = clicked == menuItem_Time3m;
            menuItem_Time5m.Checked = clicked == menuItem_Time5m;
            menuItem_Time10m.Checked = clicked == menuItem_Time10m;
            menuItem_Time15m.Checked = clicked == menuItem_Time15m;

            label_Time.Visible = timeLimitSeconds >= 0;

            ResetGame();
            RedrawBoard();
        }

        /// <summary>
        /// 現在の対戦モードで、colorがCOM操作かどうか。
        /// </summary>
        private bool IsComTurn(StoneColor color)
        {
            switch (playMode)
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
            if (!gm.IsGameEnd && IsComTurn(gm.CurrentTurn))
            {
                comMoveTimer.Start();
            }
        }

        private void ComMoveTimer_Tick(object sender, EventArgs e)
        {
            comMoveTimer.Stop();

            if (isTimedOut || gm.IsGameEnd || !IsComTurn(gm.CurrentTurn))
            {
                return;
            }

            if (ComPlayer.TryGetMove(gm, gm.CurrentTurn, comLevel, out int x, out int y))
            {
                gm.TryPut(x, y);
            }

            RedrawBoard();
        }

        /// <summary>
        /// 持ち時間のカウントダウン。今の手番の残り時間を100msずつ減らし、
        /// 0になったらその手番の時間切れ負けとして対局を止める(C++版 OnTimer/TimeOutProc)。
        /// </summary>
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (gm.CurrentTurn == StoneColor.Black)
            {
                blackTimeMs -= countdownTimer.Interval;
            }
            else
            {
                whiteTimeMs -= countdownTimer.Interval;
            }

            UpdateTimeLabel();

            if (blackTimeMs <= 0 || whiteTimeMs <= 0)
            {
                countdownTimer.Stop();
                comMoveTimer.Stop();
                isTimedOut = true;
                UpdateStatusLabel();
            }
        }

        private void pictureBoxField_MouseClick(object sender, MouseEventArgs e)
        {
            if (isTimedOut || gm.IsGameEnd)
            {
                return;
            }

            // COMの手番中(タイマー待ち)は人間のクリックを受け付けない
            if (IsComTurn(gm.CurrentTurn))
            {
                return;
            }

            if (pictureBoxField.Width <= 0 || pictureBoxField.Height <= 0)
            {
                return;
            }

            // Draw側の罫線描画(「全体*i/8」で位置を計算)と同じ考え方でマス目を求める。
            // 先に1マス分の幅を割ってしまうと、盤面のサイズによっては罫線とクリック判定の
            // マス目がわずかにズレることがあるため、掛け算してから割る。
            int x = e.X * GameMaster.BoardSize / pictureBoxField.Width;
            int y = e.Y * GameMaster.BoardSize / pictureBoxField.Height;
            if (x < 0 || x >= GameMaster.BoardSize || y < 0 || y >= GameMaster.BoardSize)
            {
                return;
            }

            if (gm.TryPut(x, y))
            {
                RedrawBoard();
            }
        }

        // pictureBoxField.Imageを差し替える(CreateCanvas/DrawField内)と、その場で
        // PictureBoxのAnchorレイアウトが再計算され、コントロールのサイズが一瞬古い値に
        // 巻き戻ってしまうことがある(WinFormsのレイアウトエンジンの癖)。
        // Resizeイベントの中で即座に描画し直すとこの再計算に巻き込まれてしまうため、
        // BeginInvokeで「今回のリサイズ処理が完全に終わった後」まで描画を遅延させる。
        private bool redrawScheduled = false;

        private void pictureBoxField_Resize(object sender, EventArgs e)
        {
            if (redrawScheduled)
            {
                return;
            }

            redrawScheduled = true;
            BeginInvoke((MethodInvoker)delegate
            {
                redrawScheduled = false;

                if (pictureBoxField.Width <= 0 || pictureBoxField.Height <= 0)
                {
                    return;
                }

                draw.CreateCanvas();
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
            bool showNotice = !isTimedOut && !gm.IsGameEnd && !IsComTurn(gm.CurrentTurn);
            bool[,] validMoves = showNotice ? gm.GetValidMoves(gm.CurrentTurn) : null;
            draw.DrawField(gm.Table, validMoves);
            UpdateStatusLabel();
            UpdateTimeLabel();

            if (!isTimedOut && !gm.IsGameEnd && timeLimitSeconds >= 0)
            {
                countdownTimer.Start();
            }
            else
            {
                countdownTimer.Stop();
            }

            MaybeTriggerComMove();
        }

        /// <summary>
        /// 持ち時間の残りをlabel_Timeに表示する(持ち時間「なし」なら何も表示しない)。
        /// </summary>
        private void UpdateTimeLabel()
        {
            if (timeLimitSeconds < 0)
            {
                return;
            }

            int blackSec = Math.Max(0, blackTimeMs) / 1000;
            int whiteSec = Math.Max(0, whiteTimeMs) / 1000;
            label_Time.Text = string.Format("残り時間 黒:{0}秒 白:{1}秒", blackSec, whiteSec);
        }

        /// <summary>
        /// 手番・石数・終局結果をlabel_Statusに表示する
        /// </summary>
        private void UpdateStatusLabel()
        {
            gm.CountStone(out int blackCount, out int whiteCount);

            if (isTimedOut)
            {
                string loserName = blackTimeMs <= 0 ? "黒" : "白";
                string winnerName = blackTimeMs <= 0 ? "白" : "黒";
                label_Status.Text = string.Format("時間切れ({0}) {1}の勝ち 黒:{2} 白:{3}", loserName, winnerName, blackCount, whiteCount);
                return;
            }

            if (gm.IsGameEnd)
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
                string turnName = gm.CurrentTurn == StoneColor.Black ? "黒" : "白";
                label_Status.Text = string.Format("{0}の番 (黒:{1} 白:{2})", turnName, blackCount, whiteCount);
            }
        }
    }
}
