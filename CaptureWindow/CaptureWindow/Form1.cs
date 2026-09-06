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
        // 以前はSendInput/INPUT/MOUSEINPUTのP/Invoke一式とMouseProc()をこのファイルで
        // 個別に持っていたが、_Common.CaptWindowに全く同じ処理(座標指定クリック→元の
        // 位置へ戻す)が既にあったので、そちらを使う形に統一した(重複撲滅#3)。
        // ※実際にマウスカーソルを動かして物理クリックを送る処理なので、自動テストでは
        // 検証できない。挙動が変わっていないか、Captureボタンを押しての実機確認が必要。
        private readonly CaptWindow cw = new CaptWindow();

        String SaveFileName;
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

            LoadSetting();
        }

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            if (!IsExistSavePath())
            {
                return;
            }

            String FileFormat = TextBox_SavePath.Text + @"\" + System.DateTime.Now.ToString("yyyy_dd_mm_HH_mm_ss");

            SaveFileName = FileFormat + "_1.png";
            CaptureProc();

            if (!TextBox_MouseX.Text.Equals("") && !TextBox_MouseY.Text.Equals(""))
            {
                SaveFileName = FileFormat + "_2.png";
                cw.MouseProc(TextBox_MouseX.Text, TextBox_MouseY.Text, CaptWindow.MOUSE_EVENT.LEFT_CLICK);

                if (!TextBox_Sleep.Text.Equals(""))
                {
                    System.Threading.Thread.Sleep(int.Parse(TextBox_Sleep.Text)*1000);
                }
                CaptureProc();
            }
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

        // キャプチャ処理
        private void CaptureProc()
        {
            if (Radio_FullScreen.Checked == true)
            {
                SaveWithCaptureFullScreen();
            }
            else if (Radio_CurrentScreen.Checked == true)
            {
                SaveWithCaptureCurrentScreen();
            }
            else // Radio_CurrentWindow
            {
                SaveWithCaptureCurrentWindow();
            }
        }

        private void SaveWithCaptureFullScreen()
        {
            SendKeys.SendWait("^{PRTSC}");      // 全画面
            IDataObject d = Clipboard.GetDataObject();
            if (d != null)
            {
                //ビットマップデータ形式に関連付けられているデータを取得
                Image img = (Image)d.GetData(DataFormats.Bitmap);
                if (img != null)
                {
                    img.Save(SaveFileName);
                }
            }
        }

        private void SaveWithCaptureCurrentWindow()
        {
            SendKeys.SendWait("%{PRTSC}");      // Current Windowのみ
            IDataObject d = Clipboard.GetDataObject();
            if (d != null)
            {
                //ビットマップデータ形式に関連付けられているデータを取得
                Image img = (Image)d.GetData(DataFormats.Bitmap);
                if (img != null)
                {
                    img.Save(SaveFileName);
                }
            }
        }

        private void SaveWithCaptureCurrentScreen()
        {
            // 以前はScreen.PrimaryScreen(メイン画面)を決め打ちで参照していたため、
            // 「CurrentScreen」という名前なのに実質「常にメイン画面」を撮っていた
            // (マルチモニタ環境でこのウィンドウを拡張画面に置いても、メイン画面が
            // キャプチャされてしまうバグ)。このウィンドウが実際に今あるスクリーンを
            // Screen.FromControlで取得し、その範囲をキャプチャするように直した。
            Screen CurrentScreen = Screen.FromControl(this);

            Bitmap bmp = new Bitmap(CurrentScreen.Bounds.Width,
                                    CurrentScreen.Bounds.Height);

            //Graphicsの作成
            using (Graphics g = Graphics.FromImage(bmp))
            {
                //画面全体をコピーする(コピー元の起点は、そのスクリーンの左上座標)
                g.CopyFromScreen(CurrentScreen.Bounds.Location, new Point(0, 0), bmp.Size);

                //解放
                g.Dispose();
            }

            // ファイル保存
            bmp.Save(SaveFileName);
            bmp.Dispose();
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
