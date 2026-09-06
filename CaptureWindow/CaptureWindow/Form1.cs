using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
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
        private readonly CaptWindow cw = new CaptWindow();

        readonly String SaveXmlFile = @"CaptureWindow.xml";

        public Form1()
        {
            InitializeComponent();

            TextBox_SavePath.Text = @"c:\tmp";
            Radio_FullScreen.Checked = true;
            TextBox_MouseX.Text = @"500";
            TextBox_MouseY.Text = @"500";
            TextBox_Sleep.Text = @"3";

            // 以前のMouseProc()と同じ「指定座標をクリックしてから元の位置に戻す」動作にする設定
            cw.SetMouseMove(true);
            cw.RestoreMousePosition(true);
            // CURRENT_SCREENキャプチャで「このウィンドウが今あるモニタ」を判定できるようにする
            cw.TargetWindow = this;
            cw.SetCaptureCase(true);

            LoadSetting();
        }

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            if (!IsExistSavePath())
            {
                return;
            }

            String FileFormat = TextBox_SavePath.Text + @"\" + System.DateTime.Now.ToString("yyyy_dd_mm_HH_mm_ss");

            cw.SetFileFormat(FileFormat);
            cw.SetFileIdx(1);
            cw.SetCaptureTarget(GetSelectedCaptureTarget());

            cw.CaptureProc();   // "_1.png" として保存、呼ぶたびにFileIdxが自動で進む

            if (!TextBox_MouseX.Text.Equals("") && !TextBox_MouseY.Text.Equals(""))
            {
                cw.MouseProc(TextBox_MouseX.Text, TextBox_MouseY.Text, CaptWindow.MOUSE_EVENT.LEFT_CLICK);

                if (!TextBox_Sleep.Text.Equals(""))
                {
                    System.Threading.Thread.Sleep(int.Parse(TextBox_Sleep.Text)*1000);
                }
                cw.CaptureProc();   // "_2.png" として保存
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

        private bool IsExistSavePath()
        {
            bool IsExist = true;
            if (! System.IO.Directory.Exists(TextBox_SavePath.Text))
            {
                DialogResult result = MessageBox.Show("ディレクトリは存在しません。作成しますか？",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes)
                {
                    System.IO.Directory.CreateDirectory(TextBox_SavePath.Text);
                }
                else
                {
                    IsExist = false;
                }
            }

            return IsExist;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            TextBox_MousePoint.Text = Cursor.Position.X.ToString() + "," + Cursor.Position.Y.ToString();
        }

        private void SaveSetting_Click(object sender, EventArgs e)
        {
            Logic.SaveSettingXml(SaveXmlFile, TextBox_SavePath.Text, TextBox_MouseX.Text, TextBox_MouseY.Text, TextBox_Sleep.Text);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            Logic.Settings settings = Logic.LoadSettingXml(SaveXmlFile);
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
