using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StandardTemplate;

namespace EventRecorder
{
    public partial class Form1 : Form
    {
        private StcUtils util = new StcUtils();

        public Form1()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.EventRecorder;
            util.SetCurrentDirectory();
        }

        private void button_Setting_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ごめんね。作成中。。");
        }

        void hookMouseTest(ref GlobalHook.MouseHook.StateMouse s)
        {
            textBox2.Text = s.X + ", " + s.Y;

            if (s.Stroke == GlobalHook.MouseHook.Stroke.X1_DOWN)
            {
                GlobalHook.MouseHook.Cancel();
                textBox1.Text = "Disable X1_DOWN" + "\r\n" + textBox1.Text;
                return;
            }

            if (s.Stroke != GlobalHook.MouseHook.Stroke.MOVE)
            {
                textBox1.Text = s.Stroke + "\r\n" + textBox1.Text;
            }
        }

        void hookKeyboardTest(ref GlobalHook.KeyboardHook.StateKeyboard s)
        {
            textBox1.Text = s.Stroke + " : " + s.Key + "\r\n" + textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (GlobalHook.MouseHook.IsHooking)
            {
                GlobalHook.MouseHook.Stop();
                return;
            }

            GlobalHook.MouseHook.AddEvent(hookMouseTest);
            GlobalHook.MouseHook.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (GlobalHook.KeyboardHook.IsHooking)
            {
                GlobalHook.KeyboardHook.Stop();
                return;
            }

            GlobalHook.KeyboardHook.AddEvent(hookKeyboardTest);
            GlobalHook.KeyboardHook.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<EventRecorder.Windows.Simulation.InputSimulator.Input> inputs = new List<EventRecorder.Windows.Simulation.InputSimulator.Input>();
            List<EventRecorder.Windows.Simulation.InputSimulator.MouseStroke> flags = new List<EventRecorder.Windows.Simulation.InputSimulator.MouseStroke>();

            flags.Add(EventRecorder.Windows.Simulation.InputSimulator.MouseStroke.LEFT_DOWN);
            flags.Add(EventRecorder.Windows.Simulation.InputSimulator.MouseStroke.LEFT_UP);
            flags.Add(EventRecorder.Windows.Simulation.InputSimulator.MouseStroke.MOVE);

            EventRecorder.Windows.Simulation.InputSimulator.AddMouseInput(ref inputs, flags, 0, false, 0, 50);
            EventRecorder.Windows.Simulation.InputSimulator.AddKeyboardInput(ref inputs, "ゆっくりしていってね！！");

            EventRecorder.Windows.Simulation.InputSimulator.SendInput(inputs);
        }

    }
}
