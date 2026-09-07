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
        private Draw draw = new Draw();
        private GameMaster gm = new GameMaster();

        public Form1()
        {
            InitializeComponent();

            // 初期デザインのサイズを下限にする(C++版のWIN_MIN_SIZE相当)。
            // これより小さくはリサイズできないようにし、盤面が0サイズになる事態を防ぐ。
            this.MinimumSize = this.Size;

            gm.Initialize();

            draw.SetDrawArea(pictureBoxField);
            draw.DrawField(gm.Table);
            UpdateStatusLabel();
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
                gm.Initialize();
                draw.DrawField(gm.Table);
                UpdateStatusLabel();
            }
        }

        /// <summary>
        /// メニュー「ゲーム」→「開始」。確認なしで新規対局を始める。
        /// </summary>
        private void menuItem_Start_Click(object sender, EventArgs e)
        {
            gm.Initialize();
            draw.DrawField(gm.Table);
            UpdateStatusLabel();
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
        /// メニュー「対戦モード」。COM対戦(AI)は未実装のため、
        /// 人 vs 人以外を選ぶと案内を出して選択を人 vs 人に戻す。
        /// </summary>
        private void menuItem_PlayMode_Click(object sender, EventArgs e)
        {
            var clicked = (ToolStripMenuItem)sender;

            if (clicked != menuItem_PP)
            {
                MessageBox.Show(
                    "COM対戦(AI)はまだ実装されていないよ。\n人 vs 人で遊んでね。",
                    "対戦モード",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                clicked = menuItem_PP;
            }

            menuItem_PP.Checked = clicked == menuItem_PP;
            menuItem_PC.Checked = clicked == menuItem_PC;
            menuItem_CP.Checked = clicked == menuItem_CP;
            menuItem_CC.Checked = clicked == menuItem_CC;
        }

        private void pictureBoxField_MouseClick(object sender, MouseEventArgs e)
        {
            if (gm.IsGameEnd)
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
                draw.DrawField(gm.Table);
                UpdateStatusLabel();
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
                draw.DrawField(gm.Table);
            });
        }

        /// <summary>
        /// 手番・石数・終局結果をlabel_Statusに表示する
        /// </summary>
        private void UpdateStatusLabel()
        {
            gm.CountStone(out int blackCount, out int whiteCount);

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
