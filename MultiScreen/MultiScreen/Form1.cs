using System;
using System.Windows.Forms;

namespace MultiScreen
{
    public partial class Form1 : Form
    {
        private int ScreenWidth;    // 画面サイズ
        private int ScreenHeight;   // 画面サイズ
        private const int DefaultDlgNum = 10;    // ダイアログ生成数の既定値

        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
            InitializeToolTips();

            // ダイアログ生成数のDefault値を設定
            textBox_DlgNum.Text = DefaultDlgNum.ToString();
            textBox_ButtonName.Text = "Click Me!!";

            // モニタの解像度取得
            ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 対象外の欄は無し(2つとも手入力する欄)
        private void InitializePlaceholders()
        {
            textBox_DlgNum.PlaceholderText = "例: 10";
            textBox_ButtonName.PlaceholderText = "例: Click Me!!";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_DlgNum, "表示するダイアログの数。モニタが複数あれば1つずつ順番に振り分け、ランダムな位置に表示する");
            toolTip.SetToolTip(textBox_ButtonName, "各ダイアログのボタンに表示する文字。そのボタンを押すとダイアログが閉じる");
            toolTip.SetToolTip(buttonAllPopup, "指定数のダイアログをモードレスでまとめて表示する");
            toolTip.SetToolTip(buttonSequencePopup, "ダイアログをモーダルで1つずつ表示する。閉じると次が表示される");
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

        private void CreateDialog(int dlgIndex, bool isModal = false)
        {
            // ランダムな表示座標を生成
            Random random = new System.Random();
            int dlgX = random.Next(ScreenWidth);
            int dlgY = random.Next(ScreenHeight);

            // ダイアログの表示座標はマルチモニタを考慮する
            int monitorIdx = dlgIndex % Screen.AllScreens.Length;
            dlgX += Screen.AllScreens[monitorIdx].Bounds.Location.X;
            dlgY += Screen.AllScreens[monitorIdx].Bounds.Location.Y;

            // ダイアログ生成
            ChildDlg dlg = new ChildDlg(textBox_ButtonName.Text);
            dlg.StartPosition = FormStartPosition.Manual;
            dlg.Owner = this; // 常に親ウィンドウの手前に表示
            dlg.Left = dlgX;
            dlg.Top = dlgY;

            if (isModal)
            {
                dlg.ShowDialog(); // モーダル・ダイアログとして表示
            }
            else
            {
                dlg.Show(); // モードレス・ダイアログとして表示 
            }
        }
    }
}
