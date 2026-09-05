using System;
using System.Drawing;
using System.Windows.Forms;

namespace MoveDialog
{
    public partial class Form1 : Form
    {
        private readonly FormChild child = new FormChild();

        public Form1()
        {
            InitializeComponent();

            // 子ウィンドウの「幅・高さ」の設定上限は画面サイズとする
            trackBarWindowWidth.Maximum = Screen.PrimaryScreen.Bounds.Width;
            trackBarWindowHeight.Maximum = Screen.PrimaryScreen.Bounds.Height;

            // 子ウィンドウ表示
            child.Show();
        }

        private void updatePosition(object sender, EventArgs e)
        {
            //if (sender.Equals(this.buttonMoveDirectionUp))
            switch ((sender as Button).Name)
            {
            case "buttonMoveDirectionUp":
                child.Top = Logic.GetSubValue(child.Top, trackBarMoveDistance.Value);
                break;
            case "buttonMoveDirectionDown":
                child.Top = Logic.GetAddValue(trackBarWindowHeight.Maximum, child.Top, trackBarMoveDistance.Value, child.Height);
                break;
            case "buttonMoveDirectionLeft":
                child.Left = Logic.GetSubValue(child.Left, trackBarMoveDistance.Value);
                break;
            case "buttonMoveDirectionRight":
                child.Left = Logic.GetAddValue(trackBarWindowWidth.Maximum, child.Left, trackBarMoveDistance.Value, child.Width);
                break;
            default:
                // buttonMoveDirectionCentor
                child.Left = (trackBarWindowWidth.Maximum - child.Width) / 2;
                child.Top = (trackBarWindowHeight.Maximum - child.Height) / 2;
                break;
            }
        }

        private void checkBoxVisible_CheckedChanged(object sender, EventArgs e)
        {
            child.Visible = checkBoxVisible.Checked;
        }

        private void trackBarMoveDistance_Scroll(object sender, EventArgs e)
        {
            labelMoveDistanceValue.Text = trackBarMoveDistance.Value.ToString();
        }

        private void updateRectSize(object sender, EventArgs e)
        {
            child.Width = trackBarWindowWidth.Value;
            child.Height = trackBarWindowHeight.Value;

            labelWindowWidthValue.Text = trackBarWindowWidth.Value.ToString();
            labelWindowHeightValue.Text = trackBarWindowHeight.Value.ToString();
        }

        private void updateColor(object sender, EventArgs e)
        {
            child.BackColor = Color.FromArgb(trackBarWindowColorRed.Value, trackBarWindowColorGreen.Value, trackBarWindowColorBlue.Value);

            labelWindowColorRedValue.Text = trackBarWindowColorRed.Value.ToString();
            labelWindowColorGreenValue.Text = trackBarWindowColorGreen.Value.ToString();
            labelWindowColorBlueValue.Text = trackBarWindowColorBlue.Value.ToString();
        }
    }
}
