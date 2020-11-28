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
            gm.Initialize();

            draw.SetDrawArea(pictureBoxField);
            draw.InitField();
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
                draw.InitField();
            }
        }
    }
}
