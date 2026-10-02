using System;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalClock
{
    public partial class Form1 : Form
    {
        private Point mouseDownPoint;

        public Form1()
        {
            InitializeComponent();
            InitializeToolTips();

            // アイコン設定
            this.Icon = Properties.Resources.DigitalClock;

            UpdateTime();
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(label_time, "時刻の部分を左ドラッグするとウィンドウを移動できる");
        }

        // user.configが壊れていると設定へのアクセスで例外になるため、位置の復元・保存は諦めて動作を優先する
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(100, 100);
            try
            {
                if (Properties.Settings.Default.FormSize.Width != 0 && Properties.Settings.Default.FormSize.Height != 0)
                {
                    this.Location = Properties.Settings.Default.FormPoint;
                    this.Size = Properties.Settings.Default.FormSize;
                }
            }
            catch (System.Configuration.ConfigurationException)
            {
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Properties.Settings.Default.FormPoint = this.Location;
                Properties.Settings.Default.FormSize = this.Size;
                Properties.Settings.Default.Save();
            }
            catch (System.Configuration.ConfigurationException)
            {
            }
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }
        
        private void UpdateTime()
        {
            label_time.Text = Logic.FormatTime(DateTime.Now);
        }

        private void label_time_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                mouseDownPoint = new Point(e.X, e.Y);
            }
        }

        private void label_time_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.Left += e.X - mouseDownPoint.X;
                this.Top += e.Y - mouseDownPoint.Y;
            }
        }
    }
}
