using System;
using System.Windows.Forms;

namespace MultiScreen
{
    public partial class Form1 : Form
    {
        private readonly int _screenWidth;    // 画面サイズ
        private readonly int _screenHeight;   // 画面サイズ
        private readonly Random _random = new Random();   // ダイアログの表示座標用(都度生成すると同じ値が続くため使い回す)
        private const int _defaultDlgNum = 10;    // ダイアログ生成数の既定値

        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
            InitializeToolTips();

            // ダイアログ生成数のDefault値を設定
            textBox_DlgNum.Text = _defaultDlgNum.ToString();
            textBox_ButtonName.Text = "Click Me!!";

            // モニタの解像度取得
            _screenWidth = Screen.PrimaryScreen.Bounds.Width;
            _screenHeight = Screen.PrimaryScreen.Bounds.Height;
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
            CreateDialogs(false);
        }

        private void buttonSequencePopup_Click(object sender, EventArgs e)
        {
            CreateDialogs(true);
        }

        private void CreateDialogs(bool isModal)
        {
            int dlgNum = int.Parse(textBox_DlgNum.Text);
            for (int i = 0; i < dlgNum; i++)
            {
                CreateDialog(i, isModal);
            }
        }

        private void CreateDialog(int dlgIndex, bool isModal = false)
        {
            // ランダムな表示座標を生成
            int dlgX = _random.Next(_screenWidth);
            int dlgY = _random.Next(_screenHeight);

            // ダイアログの表示座標はマルチモニタを考慮する
            Screen[] screens = Screen.AllScreens;
            System.Drawing.Point monitorLocation = screens[dlgIndex % screens.Length].Bounds.Location;
            dlgX += monitorLocation.X;
            dlgY += monitorLocation.Y;

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
