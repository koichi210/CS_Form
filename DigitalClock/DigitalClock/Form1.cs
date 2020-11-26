using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DigitalClock
{
    public partial class Form1 : Form
    {
        private Point mouseDownPoint;
        public Form1()
        {
            InitializeComponent();

            // アイコン設定
            this.Icon = Properties.Resources.DigitalClock;

            UpdateTime();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.FormSize.Width == 0 || Properties.Settings.Default.FormSize.Height == 0)
            {
                this.Location = new Point(100, 100);
            }
            else
            {
                this.Location = Properties.Settings.Default.FormPoint;
                this.Size = Properties.Settings.Default.FormSize;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.FormPoint = this.Location;
            Properties.Settings.Default.FormSize = this.Size;
            Properties.Settings.Default.Save();
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }
        
        private void UpdateTime()
        {
            DateTime d = DateTime.Now;
            label_time.Text = String.Format("{0:00}:{1:00}:{2:00}", d.Hour, d.Minute, d.Second);
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
