using System;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalClockDisplayTitle
{
    public partial class Form1 : Form
    {
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;

        public Form1()
        {
            InitializeComponent();

            // アイコン設定
            this.Icon = Properties.Resources.DigitalClockDisplayTitle;

            UpdateTime();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(75,20);
            this.Location = new Point(100, 100);

            // サイズは固定なので、復元するのは位置だけ。
            // user.configが壊れていると設定へのアクセスで例外になるため、位置の復元・保存は諦めて動作を優先する
            try
            {
                this.Location = Properties.Settings.Default.FormPoint;
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
            this.Text = Logic.FormatTime(DateTime.Now);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                this.Close();
            }

            base.WndProc(ref m);
        }
    }
}
