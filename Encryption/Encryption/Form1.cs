using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Encryption
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            textBox_Table.Text = "8 1 4 7 2 3 9 5 6 0";
            textBox_Key.Text = "257";
            radioButton_Decode.Checked = true;

            // encode
            // 0 1 2 3 4 5 6 7 8 9 ↓
            // 8 1 4 7 2 3 9 5 6 0

            // decode
            // 0 1 2 3 4 5 6 7 8 9
            // 8 3 7 1 2 5 6 0 9 4 ↑
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            var line = textBox_Table.Text;

            string[] key_s = line.Split(' ');
            int[] key_i = key_s.Select(str => int.Parse(str)).ToArray();

            bool encdec = true;
            if (radioButton_Decode.Checked)
            {
                encdec = false;
            }

            // Key
            int word = int.Parse(textBox_Key.Text);

            // decodeのときはテーブルを反転

            if (!encdec)
            {
                int[] key_ii = key_s.Select(str => int.Parse(str)).ToArray();
                for (int i = 0; i < key_ii.Length; i++)
                {
                    int idx = key_ii[i];
                    key_i[idx] = i;
                }
            }

            string ans = "";
            while (word != 0)
            {
                int idx = word % 10;
                word /= 10;
                ans = key_i[idx].ToString() + ans;
            }
            textBox_Result.Text = ans;
        }
    }
}
