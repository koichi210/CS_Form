using System;
using System.Drawing;
using System.Windows.Forms;

namespace DialogChild
{
    public partial class Form1 : Form
    {
        private readonly FormChild _child = new FormChild();

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();

            // 子ウィンドウの「幅・高さ」の設定上限は画面サイズとする
            trackBarWindowWidth.Maximum = Screen.PrimaryScreen.Bounds.Width;
            trackBarWindowHeight.Maximum = Screen.PrimaryScreen.Bounds.Height;

            // 子ウィンドウ表示
            _child.Show();
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(buttonMoveDirectionCenter, "子ウィンドウを画面の中央へ移動する");
            toolTip.SetToolTip(trackBarMoveDistance, "矢印ボタン1回で子ウィンドウを動かす量(ピクセル)。画面の端を越える場合は端で止まる");
        }

        private void UpdatePosition(object sender, EventArgs e)
        {
            switch ((sender as Button).Name)
            {
            case "buttonMoveDirectionUp":
                _child.Top = Logic.GetSubValue(_child.Top, trackBarMoveDistance.Value);
                break;
            case "buttonMoveDirectionDown":
                _child.Top = Logic.GetAddValue(trackBarWindowHeight.Maximum, _child.Top, trackBarMoveDistance.Value, _child.Height);
                break;
            case "buttonMoveDirectionLeft":
                _child.Left = Logic.GetSubValue(_child.Left, trackBarMoveDistance.Value);
                break;
            case "buttonMoveDirectionRight":
                _child.Left = Logic.GetAddValue(trackBarWindowWidth.Maximum, _child.Left, trackBarMoveDistance.Value, _child.Width);
                break;
            default:
                // buttonMoveDirectionCenter
                // 中央寄せは(領域 - 子のサイズ)/2。+では中央からずれていた
                _child.Left = (trackBarWindowWidth.Maximum - _child.Width) / 2;
                _child.Top = (trackBarWindowHeight.Maximum - _child.Height) / 2;
                break;
            }
        }

        private void checkBoxVisible_CheckedChanged(object sender, EventArgs e)
        {
            _child.Visible = checkBoxVisible.Checked;
        }

        private void trackBarMoveDistance_Scroll(object sender, EventArgs e)
        {
            labelMoveDistanceValue.Text = trackBarMoveDistance.Value.ToString();
        }

        private void UpdateRectSize(object sender, EventArgs e)
        {
            _child.Width = trackBarWindowWidth.Value;
            _child.Height = trackBarWindowHeight.Value;

            labelWindowWidthValue.Text = trackBarWindowWidth.Value.ToString();
            labelWindowHeightValue.Text = trackBarWindowHeight.Value.ToString();
        }

        private void UpdateColor(object sender, EventArgs e)
        {
            _child.BackColor = Color.FromArgb(trackBarWindowColorRed.Value, trackBarWindowColorGreen.Value, trackBarWindowColorBlue.Value);

            labelWindowColorRedValue.Text = trackBarWindowColorRed.Value.ToString();
            labelWindowColorGreenValue.Text = trackBarWindowColorGreen.Value.ToString();
            labelWindowColorBlueValue.Text = trackBarWindowColorBlue.Value.ToString();
        }
    }
}
