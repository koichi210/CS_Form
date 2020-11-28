using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MouseTrainingWithMultiScreen
{
    public partial class Form1 : Form
    {
        private int ScreenWidth;    // 画面サイズ
        private int ScreenHeight;   // 画面サイズ
        private int DlgMax = 10;    // ループ回数

        public Form1()
        {
            InitializeComponent();

            // ダイアログ生成数のDefault値を設定
            textBox_DlgNum.Text = DlgMax.ToString();
            textBox_ButtonName.Text = "Click Me!!";

            // モニタの解像度取得
            ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        }

        private void buttonAllPopup_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < int.Parse(textBox_DlgNum.Text); i++)
            {
                CreateDialog(i);
            }
        }

        private void buttonSequencePopup_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < int.Parse(textBox_DlgNum.Text); i++)
            {
                CreateDialog(i, true);
            }
        }

        private void CreateDialog(int DlgNum, bool IsModal = false)
        {
            // ランダムな表示座標を生成
            Random Rand = new System.Random();
            int DlgWidth = Rand.Next(ScreenWidth);
            int DlgHeight = Rand.Next(ScreenHeight);

            // ダイアログの表示座標はマルチモニタを考慮する
            int MonitorIdx = DlgNum % Screen.AllScreens.Length;
            DlgWidth += Screen.AllScreens[MonitorIdx].Bounds.Location.X;
            DlgHeight += Screen.AllScreens[MonitorIdx].Bounds.Location.Y;

            // ダイアログ生成
            ChildDlg dlg = new ChildDlg(textBox_ButtonName.Text);
            //dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.StartPosition = FormStartPosition.Manual;
            dlg.Owner = this; // 常に親ウィンドウの手前に表示
            dlg.Left = DlgWidth;
            dlg.Top = DlgHeight;

            if (IsModal)
            {
                dlg.ShowDialog(); // モーダル・ダイアログとして表示
            }
            else
            {
                dlg.Show(); // モードレス・ダイアログとして表示 
            }

            //System.Threading.Thread.Sleep(1000);
        }
    }
}
