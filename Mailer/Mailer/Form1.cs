using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StandardTemplate;

namespace Mailer
{
    public partial class Form1 : Form
    {
        readonly String SettingFileName = @"Mailer.xml";
        readonly String MailUrl = @"https://mail.google.com/mail/?view=cm&fs=1";

        private StcFileInputOutput fio = new StcFileInputOutput();
        private SaveRestore sr = new SaveRestore();
        private StcUtils util = new StcUtils();

        private ExecParam param = new ExecParam();

        class ExecParam
        {
            public int CreateNum = 0;
            public int IntervalMsec = 0;
        }

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Mailer;

            // カレントディレクトリ移動
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            sr.RegistLoadItem(this);
            sr.LoadProc(SettingFileName, this);
            util.UpdateProfileList(ref comboBox_LoadSetting);
        }

        private void comboBox_LoadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + comboBox_LoadSetting.Text;
            sr.LoadProc(LoadFileName, this);
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            String FileName = comboBox_LoadSetting.Text;
            if (FileName == String.Empty)
            {
                FileName = SettingFileName;
            }
            String SaveFileName = fio.SelectSaveFileName(FileName);
            if (sr.SaveSetting(SaveFileName, this))
            {
                util.UpdateProfileList(ref comboBox_LoadSetting, Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
            }
        }
        
        private void button_OpenBrowse_Click(object sender, EventArgs e)
        {
            if (!GetUIParam())
            {
                return;
            }

            OpenBrowse(DateTime.Now);
        }

        private void button_OpenBrowse_OneWeek_Click(object sender, EventArgs e)
        {
            if (!GetUIParam())
            {
                return;
            }

            DateTime now = new DateTime(
                dateTimePicker_Calendar.Value.Year,
                dateTimePicker_Calendar.Value.Month,
                dateTimePicker_Calendar.Value.Day,
                dateTimePicker_Calendar.Value.Hour,
                dateTimePicker_Calendar.Value.Minute,
                dateTimePicker_Calendar.Value.Second,
                0);

            var offsetDay = GetLoopList();
            foreach (var ofs in offsetDay)
            {
                OpenBrowse(now, ofs);
            }
        }

        private void OpenBrowse(DateTime srcDt, int daysOffset = 0)
        {
            String BrowseUrl = MailUrl;
            if (textBox_MailTo.Text != String.Empty)
            {
                BrowseUrl += "&to=" + textBox_MailTo.Text;
            }
            if (textBox_MailCc.Text != String.Empty)
            {
                BrowseUrl += "&cc=" + textBox_MailCc.Text;
            }
            if (textBox_MailBcc.Text != String.Empty)
            {
                BrowseUrl += "&bcc=" + textBox_MailBcc.Text;
            }

            if (textBox_MailSubject.Text != String.Empty)
            {
                DateTime dt = srcDt.AddDays(daysOffset);
                String ChromeFormatText = textBox_MailSubject.Text.Replace(" ", "+");
                BrowseUrl += "&su=" + GetReplaceDay(ChromeFormatText, dt);
            }

            if (textBox_MailBody.Text != String.Empty)
            {
                BrowseUrl += "&body=" + textBox_MailBody.Text.Replace("\r\n", "%0D%0A").Replace(" ", "+");
            }

            util.ExecuteProcess(textBox_BrowserPath.Text, BrowseUrl);
            System.Threading.Thread.Sleep(param.IntervalMsec);
        }

        private String GetReplaceDay(String SrcText, DateTime dt)
        {
            var NewText = GetUsersDay(SrcText, dt);
            return GetDateText(NewText, dt);
        }

        private String GetUsersDay(String SrcText, DateTime dt)
        {
            String DestText = "";
            DestText = ReplaceDay(dt, SrcText, "%%USERSDAY%%");
            DestText = ReplaceDay(dt, DestText, "%%usersday%%", false);

            return DestText;
        }

        private String GetDateText(String SrcText, DateTime dt)
        {
            String DestText = "";
            DestText = ReplaceDay(dt, SrcText, "%%TODAY%%");
            DestText = ReplaceDay(dt, DestText, "%%today%%", false);

            var nextDay = dt.AddDays(1);
            DestText = ReplaceDay(nextDay, DestText, "%%TOMORROW%%");
            DestText = ReplaceDay(nextDay, DestText, "%%tomorrow%%", false);

            return DestText; 
        }

        private String ReplaceDay(DateTime dt, String SrcText, String KeyName, bool IsYear = true)
        {
            String DateString = "";
            if ( IsYear )
            {
                DateString += dt.Year.ToString() + "/";
            }
            DateString += dt.Month.ToString() +"/";
            DateString += dt.Day.ToString();
 
            return SrcText.Replace(KeyName, DateString);
        }

        private Boolean GetUIParam()
        {
            if( !util.IsExistFileNameInEnvironment(textBox_BrowserPath.Text) )
            {
                MessageBox.Show("ファイルが存在しません" + Environment.NewLine + textBox_BrowserPath.Text);
                return false;
            }

            int.TryParse(textBox_CreateNum.Text, out param.CreateNum);
            int.TryParse(textBox_IntervalMsec.Text, out param.IntervalMsec);

            return true;
        }

        private List<int> GetLoopList()
        {
            var offsetDay = new List<int>();
            for (var i = 0; i < param.CreateNum; i++)
            {
                offsetDay.Add(i);
            }
            if (check_BoxReverse.Checked)
            {
                offsetDay.Sort((x, y) => y - x);
            }
            return offsetDay;
        }
        
        private void textBox_BrowserPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(textBox_BrowserPath.Text, e);

        }

        private void button_Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("to be Update");
        }
    }
}
