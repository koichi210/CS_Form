using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Xml;

namespace CaptureWindow
{
    public partial class Form1 : Form
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        extern static uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        const int MOUSEEVENTF_MOVE      = 0x0001;   // 移動
        const int MOUSEEVENTF_LEFTDOWN  = 0x0002;   // 左ボタン Down
        const int MOUSEEVENTF_LEFTUP    = 0x0004;   // 左ボタン Up
        const int MOUSEEVENTF_ABSOLUTE  = 0x8000;   // 絶対値指定
        ///////////////////////////////////////////////////////////////////////////////////////////////

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
                MouseProc();

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

        // マウス処理
        private void MouseProc()
        {
            Point MousePt = new Point(Cursor.Position.X, Cursor.Position.Y);

            //マウス移動
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
                                                    int.Parse(TextBox_MouseX.Text),
                                                    int.Parse(TextBox_MouseY.Text));

            //struct 配列の宣言
            INPUT[] input = new INPUT[2];

            //左ボタン Down
            //input[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN;
            input[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            //input[0].mi.dx = int.Parse(TextBox_MouseX.Text);
            //input[0].mi.dy = int.Parse(TextBox_MouseY.Text);

            //左ボタン Up
            input[1].mi.dwFlags = MOUSEEVENTF_LEFTUP;
            //input[1].mi.dx = input[0].mi.dx;
            //input[1].mi.dy = input[0].mi.dy;

            //イベントの一括生成
            SendInput(2, input, Marshal.SizeOf(input[0]));

            // マウスの位置をもとに戻す
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(MousePt.X, MousePt.Y);
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
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                    Screen.PrimaryScreen.Bounds.Height);

            //Graphicsの作成
            using (Graphics g = Graphics.FromImage(bmp))
            {
                //画面全体をコピーする
                g.CopyFromScreen(new Point(0, 0), new Point(0, 0), bmp.Size);

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
            XmlDocument document = new XmlDocument();

            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            XmlElement root = document.CreateElement("root");  // ルート要素

            document.AppendChild(declaration);
            document.AppendChild(root);

            XmlElement element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "TextBox_SavePath");
            element.InnerText = TextBox_SavePath.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "TextBox_MouseX");
            element.InnerText = TextBox_MouseX.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "TextBox_MouseY");
            element.InnerText = TextBox_MouseY.Text;
            root.AppendChild(element);

            element = document.CreateElement("Setting");
            element.SetAttribute("attribute", "TextBox_Sleep");
            element.InnerText = TextBox_Sleep.Text;
            root.AppendChild(element);

            // ファイルに保存する
            document.Save(SaveXmlFile);

            MessageBox.Show("設定値を保存しました♪");
        }

        private void LoadSetting()
        {
            if (!File.Exists(SaveXmlFile))
            {
                return;
            }

            // ファイルから読み込む
            XmlDocument document = new XmlDocument();
            document.Load(SaveXmlFile);

            foreach (XmlElement element in document.DocumentElement)
            {
                string attribute = element.GetAttribute("attribute");   // 属性
                string text = element.InnerText;                        // 要素の内容

                if (attribute.Equals("TextBox_SavePath"))
                {
                    TextBox_SavePath.Text = text;
                }
                else if (attribute.Equals("TextBox_MouseX"))
                {
                    TextBox_MouseX.Text = text;
                }
                else if (attribute.Equals("TextBox_MouseY"))
                {
                    TextBox_MouseY.Text = text;
                }
                else if (attribute.Equals("TextBox_Sleep"))
                {
                    TextBox_Sleep.Text = text;
                }
            }
        }
    }
}
