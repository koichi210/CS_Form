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
            List<InputSimulation.InputSimulator.Input> inputs = new List<InputSimulation.InputSimulator.Input>();
            List<InputSimulation.InputSimulator.MouseStroke> flags = new List<InputSimulation.InputSimulator.MouseStroke>();

            flags.Add(InputSimulation.InputSimulator.MouseStroke.LEFT_DOWN);
            flags.Add(InputSimulation.InputSimulator.MouseStroke.LEFT_UP);
            flags.Add(InputSimulation.InputSimulator.MouseStroke.MOVE);

            InputSimulation.InputSimulator.AddMouseInput(ref inputs, flags, 0, false, 0, 50);
            InputSimulation.InputSimulator.AddKeyboardInput(ref inputs, "ゆっくりしていってね！！");

            InputSimulation.InputSimulator.SendInput(inputs);
        }

    }
}
