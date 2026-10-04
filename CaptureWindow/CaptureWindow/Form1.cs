using System;
using System.Drawing;
using System.Windows.Forms;
using StandardTemplate;

namespace CaptureWindow
{
    public partial class Form1 : Form
    {
        // 以前はSendInput/INPUT/MOUSEINPUTのP/Invoke一式、MouseProc()、
        // Full/Current/CurrentWindowの3種のキャプチャ処理をこのファイルで個別に
        // 持っていたが、_Common.CaptWindowに全く同じ機能が既にあった(Cheetosは
        // 元からこちらを使っていた)ので、そちらへ統一した(重複撲滅#3)。
        // ※実際にマウスカーソルを動かして物理クリックを送る/画面を撮る処理なので、
        // 自動テストでは検証できない。挙動が変わっていないか、Captureボタンを押しての
        // 実機確認が必要。
        private readonly CaptWindow _captWindow = new CaptWindow();

        private const String _settingFilePath = @"CaptureWindow.json";

        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
            InitializeToolTips();

            TextBox_SavePath.Text = @"c:\tmp";
            Radio_FullScreen.Checked = true;
            TextBox_MouseX.Text = @"500";
            TextBox_MouseY.Text = @"500";
            TextBox_Sleep.Text = @"3";

            // 以前のMouseProc()と同じ「指定座標をクリックしてから元の位置に戻す」動作にする設定
            _captWindow.SetMouseMove(true);
            _captWindow.SetRestoreMousePosition(true);
            // CurrentScreenキャプチャで「このウィンドウが今あるモニタ」を判定できるようにする
            _captWindow.TargetWindow = this;
            _captWindow.SetCaptureCase(true);

            LoadSetting();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnlyの欄(TextBox_MousePoint、マウス座標の表示欄)は対象外
        private void InitializePlaceholders()
        {
            TextBox_SavePath.PlaceholderText = @"例: C:\tmp";
            TextBox_MouseX.PlaceholderText = "例: 500";
            TextBox_MouseY.PlaceholderText = "例: 500";
            TextBox_Sleep.PlaceholderText = "例: 3";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(Button_Capture, "画面を撮って「保存フォルダ\\日時_1.png」に保存する。X・Yが両方入っていればその座標を左クリックし、Sleep時間待ってから2枚目(_2.png)を撮る");
            toolTip.SetToolTip(TextBox_SavePath, "画像の保存先フォルダ。無ければ確認のうえ作成する");
            toolTip.SetToolTip(TextBox_MouseX, "クリックする画面上のX座標(ピクセル)。X・Yどちらかが空欄ならクリックせず1枚だけ撮る");
            toolTip.SetToolTip(TextBox_MouseY, "クリックする画面上のY座標(ピクセル)。X・Yどちらかが空欄ならクリックせず1枚だけ撮る");
            toolTip.SetToolTip(TextBox_Sleep, "クリックしてから2枚目を撮るまでの待ち時間(整数の秒)。空欄なら待たない");
            toolTip.SetToolTip(Radio_FullScreen, "画面全体を撮る(Ctrl+PrintScreen相当)");
            toolTip.SetToolTip(Radio_CurrentScreen, "このツールのウィンドウがあるモニタ1枚だけを撮る");
            toolTip.SetToolTip(Radio_CurrentWindow, "その時点でアクティブなウィンドウを撮る(Alt+PrintScreen相当)");
        }

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            if (!EnsureSavePathExists())
            {
                return;
            }

            String fileFormat = TextBox_SavePath.Text + @"\" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

            _captWindow.SetFileFormat(fileFormat);
            _captWindow.SetFileIdx(1);
            _captWindow.SetCaptureTarget(GetSelectedCaptureTarget());

            _captWindow.CaptureProc();   // "_1.png" として保存、呼ぶたびにFileIdxが自動で進む

            if (TextBox_MouseX.Text != "" && TextBox_MouseY.Text != "")
            {
                _captWindow.MouseProc(TextBox_MouseX.Text, TextBox_MouseY.Text, CaptWindow.MouseEventType.LeftClick);

                if (TextBox_Sleep.Text != "")
                {
                    System.Threading.Thread.Sleep(int.Parse(TextBox_Sleep.Text)*1000);
                }
                _captWindow.CaptureProc();   // "_2.png" として保存
            }
        }

        private CaptWindow.CaptureTargetType GetSelectedCaptureTarget()
        {
            if (Radio_FullScreen.Checked)
            {
                return CaptWindow.CaptureTargetType.FullScreen;
            }
            if (Radio_CurrentScreen.Checked)
            {
                return CaptWindow.CaptureTargetType.CurrentScreen;
            }
            return CaptWindow.CaptureTargetType.CurrentWindow;
        }

        private bool EnsureSavePathExists()
        {
            if (System.IO.Directory.Exists(TextBox_SavePath.Text))
            {
                return true;
            }

            DialogResult result = MessageBox.Show("ディレクトリは存在しません。作成しますか？",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);

            if (result != DialogResult.Yes)
            {
                return false;
            }

            System.IO.Directory.CreateDirectory(TextBox_SavePath.Text);
            return true;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = Cursor.Position;
            TextBox_MousePoint.Text = position.X + "," + position.Y;
        }

        private void SaveSetting_Click(object sender, EventArgs e)
        {
            Logic.SaveSetting(_settingFilePath, TextBox_SavePath.Text, TextBox_MouseX.Text, TextBox_MouseY.Text, TextBox_Sleep.Text);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            Logic.Settings settings = Logic.LoadSetting(_settingFilePath);
            if (settings == null)
            {
                return;
            }

            SetTextIfNotNull(TextBox_SavePath, settings.SavePath);
            SetTextIfNotNull(TextBox_MouseX, settings.MouseX);
            SetTextIfNotNull(TextBox_MouseY, settings.MouseY);
            SetTextIfNotNull(TextBox_Sleep, settings.Sleep);
        }

        // 保存されていなかった項目(null)は既定値のまま残す
        private static void SetTextIfNotNull(Control control, String value)
        {
            if (value != null)
            {
                control.Text = value;
            }
        }
    }
}
