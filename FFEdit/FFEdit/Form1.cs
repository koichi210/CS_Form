using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace FFEdit
{
    public partial class Form1 : Form
    {
        private readonly String SettingFileName = @"FFEdit.xml";
        private readonly String[] IncrCycleArray = { "無し", "秒", "分", "時間", "日" };
        private readonly String[] DigitArray = { "自動", "1桁", "2桁", "3桁", "4桁", "5桁", "6桁" };
        private const int TabIdxChangeName = 0;
        private const int TabIdxTimeStump = 1;
        private const int TabIdxFuntion = 2;

        private StcUtils util = new StcUtils();
        private FileMng fm = new FileMng();
        private SaveRestore sr = new SaveRestore();

        private Rename rename = new Rename();
        private Function fs = new Function();

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.FFEdit;
            util.SetCurrentDirectory();

            sr.RegistItem(this);
            sr.LoadProc(SettingFileName, this);

            // 桁の選択肢を生成
            comboBox_ChangeNumber_Digit.Items.Clear();
            for (int i = 0; i < DigitArray.Length; i++)
            {
                comboBox_ChangeNumber_Digit.Items.Add(DigitArray[i]);
            }
            comboBox_ChangeNumber_Digit.SelectedIndex = 0;

            // 桁の選択肢を生成
            comboBox_TimeSpan.Items.Clear();
            for (int i = 0; i < IncrCycleArray.Length; i++)
            {
                comboBox_TimeSpan.Items.Add(IncrCycleArray[i]);
            }
            comboBox_TimeSpan.SelectedIndex = 1;
        }

        private void comboBox_TargetDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                util.ExecutePath(comboBox_TargetDir.Text);
            }
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control == true)
            {
                util.SelectAll(e);
            }
            else if (e.KeyCode == Keys.C && e.Control == true)
            {
                util.CopyToClipboard(e, listBox);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DialogResult DlgResult = MessageBox.Show(
                    "削除しますか？",
                    "情報",
                    MessageBoxButtons.YesNo);
                if (DlgResult == DialogResult.No)
                {
                    return ;
                }

                String TargetName = util.GetSelectName(listBox, comboBox_TargetDir.Text);

                StcFileInputOutput fio = new StcFileInputOutput();
                String[] TargetArray = TargetName.Split(new[] {Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
                for ( int i= 0; i < TargetArray.Length; i++)
                {
                    fio.DeleteDirectoryAndFile(TargetArray[i]);
                }
                UpdateListBox();
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            textBox_StatusBar.Text = String.Format("総数={0} 選択数={1}",
                listBox.Items.Count, listBox.SelectedItems.Count);
        }

        private void button_Listup_Click(object sender, EventArgs e)
        {
            UpdateListBox();
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            String ErrorList = "";
            switch (tabControl.SelectedIndex)
            {
                case TabIdxChangeName:
                    ErrorList = ChangeName();
                    break;

                case TabIdxTimeStump:
                    ChangeTimeStump();
                    break;

                case TabIdxFuntion:
                    ErrorList = ChangeOtherFunction();
                    break;
            }

            if (ErrorList != String.Empty)
            {
                using (ErrorMsg dlg = new ErrorMsg(ErrorList))
                {
                    dlg.ShowDialog();
                }
            }
            UpdateListBox();
        }

        private void button_Restore_Click(object sender, EventArgs e)
        {
            Boolean IsSuccess = true;
            switch (tabControl.SelectedIndex)
            {
                case TabIdxChangeName:
                    IsSuccess = rename.Restore();
                    break;
                case TabIdxFuntion:
                    IsSuccess = fs.Restore();
                    break;
            }

            if (!IsSuccess)
            {
                MessageBox.Show("これ以上復元できません");
                return;
            }
            UpdateListBox();
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            if (sr.SaveSetting(SettingFileName, this))
            {
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SettingFileName);
            }
        }

        private void textBox_Target_Extension_KeyUp(object sender, KeyEventArgs e)
        {
            // このタイミングでリストを更新すると、対象ファイルが多いときに重くなってしまう
            // 入力途中でもリストアップされるのは非効率なので、Enterキーをトリガに動作するようにしておく
            if (e.KeyCode == Keys.Enter)
            {
                UpdateListBox();
            }
        }

        private void textBox_Function_Target_Directory_Enter(object sender, EventArgs e)
        {
            this.textBox_Function_Any_Directory.SelectAll();
        }

        private void textBox_Function_Target_Directory_MouseClick(object sender, MouseEventArgs e)
        {
            this.textBox_Function_Any_Directory.SelectAll();
        }

        private void UpdateListBox(object sender, EventArgs e)
        {
            UpdateListBox();
        }

        private void UpdateListBox()
        {
            if (comboBox_TargetDir.Text == String.Empty)
            {
                return;
            }

            int TrimLength; // ファイルリストを生成するときに、基準となるディレクトリパスは削除する
            if (comboBox_TargetDir.Text.Length > 3)
            {
                // C:\ よりも長い場合は、終端の\を削除。ドライブレターの次の\は残す
                comboBox_TargetDir.Text = comboBox_TargetDir.Text.TrimEnd('\\');

                // 「+1」はフォルダ区切り文字
                TrimLength = comboBox_TargetDir.Text.Length + 1;
            }
            else
            {
                if (comboBox_TargetDir.Text.Length == 1)
                {
                    comboBox_TargetDir.Text += @":\";
                }
                else if (comboBox_TargetDir.Text.Length == 2)
                {
                    comboBox_TargetDir.Text += @"\";
                }
                TrimLength = comboBox_TargetDir.Text.Length;
            }

            if (!Directory.Exists(comboBox_TargetDir.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + comboBox_TargetDir.Text);
                return;
            }

            SearchOption opt = SearchOption.TopDirectoryOnly;
            if (checkBox_Target_SubDirectory.Checked)
            {
                opt = SearchOption.AllDirectories;
            }

            String SerchPattern = "*";
            if (textBox_Target_Extension.Text != String.Empty)
            {
                SerchPattern = textBox_Target_Extension.Text;
            }

            String[] elements;
            if (radioButton_Target_File.Checked)
            {
                elements = Directory.GetFiles(comboBox_TargetDir.Text, SerchPattern, opt);
            }
            else
            {
                elements = Directory.GetDirectories(comboBox_TargetDir.Text, SerchPattern, opt);
            }

            listBox.Items.Clear();
            for (int i = 0; i < elements.Length; i++)
            {
                listBox.Items.Add(elements[i].Substring(TrimLength));
            }
            UpdateStatusBar();
        }

        private List<String> GetFileList()
        {
            var FileList = new List<String>();

            int TargetNum = GetListTargetNum();
            for (int i = 0; i < TargetNum; i++)
            {
                FileList.Add(GetTargetName(i));
            }
            return FileList;
        }

        private String GetTargetName(int idx)
        {
            String TargetName = "";
            if (checkBox_Target_SelectFile.Checked)
            {
                TargetName = listBox.SelectedItems[idx].ToString();
            }
            else
            {
                TargetName = listBox.Items[idx].ToString();
            }
            return TargetName;
        }

        private String ChangeName()
        {
            rename._base_dir = comboBox_TargetDir.Text.TrimEnd('\\');
            rename._file_list = GetFileList();
            rename._change_type = GetChangedNameType();

            rename._param1 = comboBox_String1.Text;
            rename._param2 = comboBox_String2.Text;

            rename._first_number = int.Parse(textBox_ChangeNumber_FirstVal.Text);
            rename._keep_org_name = checkBox_ChangeNumber_OrgName.Checked;
            rename._pad_number = GetPaddingNum();

            return rename.Execute();
        }

        private void ChangeTimeStump()
        {
            var ts = new TimeStump();

            DateTime dt = new DateTime(
                dateTimePicker_Days.Value.Year,
                dateTimePicker_Days.Value.Month,
                dateTimePicker_Days.Value.Day,
                dateTimePicker_Time.Value.Hour,
                dateTimePicker_Time.Value.Minute,
                dateTimePicker_Time.Value.Second,
                0);
            ts._base_tick_time = dt.Ticks;
            ts._update_tick_time = GetTickTime(comboBox_TimeSpan.SelectedIndex);

            ts._base_dir = comboBox_TargetDir.Text.TrimEnd('\\');
            ts._file_list = GetFileList();

            ts._target_time_create = checkBox_CreationTime.Checked;
            ts._target_time_last_write = checkBox_LastWriteTime.Checked;
            ts._target_time_access = checkBox_LastAccessTime.Checked;

            ts.Execute();
        }

        private int GetPaddingNum()
        {
            int Padding = 0;

            if (comboBox_ChangeNumber_Digit.SelectedIndex > 0)
            {
                // 自動桁数じゃない場合
                Padding = comboBox_ChangeNumber_Digit.SelectedIndex;
            }
            else
            {
                // [自動桁数]の場合
                if (checkBox_Target_SelectFile.Checked)
                {
                    Padding = listBox.SelectedItems.Count.ToString().Length;
                }
                else
                {
                    Padding = listBox.Items.Count.ToString().Length;
                }
            }

            return Padding;
        }

        private long GetTickTime(int TimeSpanIdx)
        {
            long tick = 0;

            // 加算時間
            TimeSpan Span = TimeSpan.Zero;
            switch (TimeSpanIdx)
            {
                case 0:     // [無し]
                default:
                    break;

                case 1:     // [秒]
                    tick = TimeSpan.TicksPerSecond;
                    break;

                case 2:     // [分]
                    tick = TimeSpan.TicksPerMinute;
                    break;

                case 3:     // [時間]
                    tick = TimeSpan.TicksPerHour;
                    break;

                case 4:     // [日]
                    tick = TimeSpan.TicksPerDay;
                    break;
            }

            return tick;
        }

        private Rename.ChangeType GetChangedNameType()
        {
            if (radioButton_ChangeNumber.Checked)
            {
                return Rename.ChangeType.Number;
            }
            else if (radioButton_ChangeDelNum.Checked)
            {
                return Rename.ChangeType.DelNum;
            }
            else if (radioButton_ChangeAdd.Checked)
            {
                return Rename.ChangeType.Add;
            }
            else if (radioButton_ChangeDelete.Checked)
            {
                return Rename.ChangeType.Delete;
            }
            else if (radioButton_ChangeReplace.Checked)
            {
                return Rename.ChangeType.Replace;
            }
            else if (radioButton_ChangeExt.Checked)
            {
                return Rename.ChangeType.OnlyExt;
            }
            //else if (radioButton_ChangeAddDirName.Checked)
            {
                return Rename.ChangeType.AddDirName;
            }
        }
        private void SetNameChangeControlLabel()
        {
            String TextVal1 = "";
            String TextVal2 = "";

            // 状態取得
            if (radioButton_ChangeNumber.Checked)
            {
            }
            else if (radioButton_ChangeDelNum.Checked)
            {
                TextVal1 = "先頭から";
                TextVal2 = "後方から";
            }
            else if (radioButton_ChangeAdd.Checked)
            {
                TextVal1 = "先頭に追加";
                TextVal2 = "後方に追加";
            }
            else if (radioButton_ChangeDelete.Checked)
            {
                TextVal1 = "削除文字";
            }
            else if (radioButton_ChangeReplace.Checked)
            {
                TextVal1 = "置換前";
                TextVal2 = "置換後";
            }
            else if (radioButton_ChangeExt.Checked)
            {
                TextVal1 = "拡張子";
            }
            else if (radioButton_ChangeAddDirName.Checked)
            {
                // 何も無し
            }

            label_String1.Text = TextVal1;
            label_String2.Text = TextVal2;
        }

        private void UpdateNameChangeControl(object sender, EventArgs e)
        {
            bool IsChangeNumber = radioButton_ChangeNumber.Checked;
            SetNameChangeControlLabel();

            // TextBoxの表示
            {
                comboBox_String1.Enabled = true;
                if (label_String1.Text == String.Empty)
                {
                    comboBox_String1.Enabled = false;
                }

                comboBox_String2.Enabled = true;
                if (label_String2.Text == String.Empty)
                {
                    comboBox_String2.Enabled = false;
                }
            }

            // ChangeNumber用の設定
            {
                label_ChangeNumber_FirstVal.Enabled = IsChangeNumber;
                textBox_ChangeNumber_FirstVal.Enabled = IsChangeNumber;

                comboBox_ChangeNumber_Digit.Enabled = IsChangeNumber;
                checkBox_ChangeNumber_OrgName.Enabled = IsChangeNumber;
            }
        }

        private void UpdateFunctionControl(object sender, EventArgs e)
        {
            Boolean IsOperatoinEn = true;
            if (radioButton_Delete_BlankDir.Checked)
            {
                IsOperatoinEn = false;
            }
            checkBox_Operatoin_AnyDir.Enabled = IsOperatoinEn;

            Boolean IsAnyDirEn = false;
            if ( IsOperatoinEn && checkBox_Operatoin_AnyDir.Checked)
            {
                IsAnyDirEn = true;
            }
            textBox_Function_Any_Directory.Enabled = IsAnyDirEn;
        }

        private String GetDestDirOtherFunction()
        {
            String DestDirName = comboBox_TargetDir.Text;
            if (!radioButton_Delete_BlankDir.Checked &&
                checkBox_Operatoin_AnyDir.Checked)
            {
                DestDirName = textBox_Function_Any_Directory.Text;
            }

            return DestDirName;
        }

        private Function.FunctionType GetFunctionType()
        {
            if (radioButton_Delete_BlankDir.Checked)
            {
                return Function.FunctionType.DelEmptyDir;
            }
            else if (radioButton_Move_Target.Checked)
            {
                return Function.FunctionType.Move;
            }
            //else if (radioButton_Copy_Target.Checked)
            {
                return Function.FunctionType.Copy;
            }
        }

        private String ChangeOtherFunction()
        {
            fs._base_dir = comboBox_TargetDir.Text.TrimEnd('\\');
            fs._target_dir = GetDestDirOtherFunction().TrimEnd('\\');

            fs._file_list = GetFileList();
            fs._function_type = GetFunctionType();

            return fs.Execute();
        }

        private int GetListTargetNum()
        {
            int TargetNum = 0;
            if (checkBox_Target_SelectFile.Checked)
            {
                TargetNum = listBox.SelectedItems.Count;
            }
            else
            {
                TargetNum = listBox.Items.Count;
            }
            return TargetNum;
        }
    }
}
