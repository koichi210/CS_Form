using System;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalClockDisplayTitle
{
    public partial class Form1 : Form
    {
        const int WM_NCLBUTTONDBLCLK = 0x00A3;

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
            if (Properties.Settings.Default.FormSize.Width == 0 || Properties.Settings.Default.FormSize.Height == 0)
            {
                this.Location = new Point(100, 100);
            }
            else
            {
                this.Location = Properties.Settings.Default.FormPoint;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.FormPoint = this.Location;
            //Properties.Settings.Default.FormSize = this.Size;
            Properties.Settings.Default.Save();
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }
        
        private void UpdateTime()
        {
            DateTime d = DateTime.Now;
            this.Text = String.Format("{0:00}:{1:00}:{2:00}", d.Hour, d.Minute, d.Second);
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
