using System;
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
        private readonly CaptWindow captWindow = new CaptWindow();

        private readonly String SettingFilePath = @"CaptureWindow.json";

        public Form1()
        {
            InitializeComponent();

            TextBox_SavePath.Text = @"c:\tmp";
            Radio_FullScreen.Checked = true;
            TextBox_MouseX.Text = @"500";
            TextBox_MouseY.Text = @"500";
            TextBox_Sleep.Text = @"3";

            // 以前のMouseProc()と同じ「指定座標をクリックしてから元の位置に戻す」動作にする設定
            captWindow.SetMouseMove(true);
            captWindow.SetRestoreMousePosition(true);
            // CURRENT_SCREENキャプチャで「このウィンドウが今あるモニタ」を判定できるようにする
            captWindow.TargetWindow = this;
            captWindow.SetCaptureCase(true);

            LoadSetting();
        }

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            if (!EnsureSavePathExists())
            {
                return;
            }

            String fileFormat = TextBox_SavePath.Text + @"\" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

            captWindow.SetFileFormat(fileFormat);
            captWindow.SetFileIdx(1);
            captWindow.SetCaptureTarget(GetSelectedCaptureTarget());

            captWindow.CaptureProc();   // "_1.png" として保存、呼ぶたびにFileIdxが自動で進む

            if (!TextBox_MouseX.Text.Equals("") && !TextBox_MouseY.Text.Equals(""))
            {
                captWindow.MouseProc(TextBox_MouseX.Text, TextBox_MouseY.Text, CaptWindow.MOUSE_EVENT.LEFT_CLICK);

                if (!TextBox_Sleep.Text.Equals(""))
                {
                    System.Threading.Thread.Sleep(int.Parse(TextBox_Sleep.Text)*1000);
                }
                captWindow.CaptureProc();   // "_2.png" として保存
            }
        }

        private CaptWindow.CAPTURE_TARGET GetSelectedCaptureTarget()
        {
            if (Radio_FullScreen.Checked)
            {
                return CaptWindow.CAPTURE_TARGET.FULL_SCREEN;
            }
            if (Radio_CurrentScreen.Checked)
            {
                return CaptWindow.CAPTURE_TARGET.CURRENT_SCREEN;
            }
            return CaptWindow.CAPTURE_TARGET.CURRENT_WINDOW;
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
            TextBox_MousePoint.Text = Cursor.Position.X.ToString() + "," + Cursor.Position.Y.ToString();
        }

        private void SaveSetting_Click(object sender, EventArgs e)
        {
            Logic.SaveSetting(SettingFilePath, TextBox_SavePath.Text, TextBox_MouseX.Text, TextBox_MouseY.Text, TextBox_Sleep.Text);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            Logic.Settings settings = Logic.LoadSetting(SettingFilePath);
            if (settings == null)
            {
                return;
            }

            if (settings.SavePath != null)
            {
                TextBox_SavePath.Text = settings.SavePath;
            }
            if (settings.MouseX != null)
            {
                TextBox_MouseX.Text = settings.MouseX;
            }
            if (settings.MouseY != null)
            {
                TextBox_MouseY.Text = settings.MouseY;
            }
            if (settings.Sleep != null)
            {
                TextBox_Sleep.Text = settings.Sleep;
            }
        }
    }
}
