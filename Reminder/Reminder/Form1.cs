using System;
using System.Windows.Forms;

namespace Reminder
{
    public partial class Form1 : Form
    {
        // 「毎月の繰り返し」欄の先頭の選択肢(繰り返さない)。2番目以降が第1〜第4・最終(WeekOfMonthの値と同じ並び)
        private const int _repeatNoneIdx = 0;

        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
            InitializeRepeatSetting();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(textBox5の本文、Windowsの仕様で表示されない)は対象外
        private void InitializePlaceholders()
        {
            textBox1.PlaceholderText = "例: user@example.com";
            textBox2.PlaceholderText = "例: cc@example.com";
            textBox3.PlaceholderText = "例: bcc@example.com";
            textBox4.PlaceholderText = "例: 定例会議のリマインド";
        }

        // 「毎月 第1 火曜日」のように、毎月決まった週・曜日で繰り返す設定欄。
        // 日付の計算は[[MonthlyNthWeekday.cs]]にあり、ここでは選択肢の用意と「次回」の表示だけを行う
        private void InitializeRepeatSetting()
        {
            comboBox_RepeatWeek.Items.Add("しない");
            foreach (WeekOfMonth week in Enum.GetValues(typeof(WeekOfMonth)))
            {
                comboBox_RepeatWeek.Items.Add(MonthlyNthWeekday.GetWeekName(week));
            }

            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                comboBox_RepeatDayOfWeek.Items.Add(MonthlyNthWeekday.GetDayOfWeekName(dayOfWeek));
            }

            // 初期値は「繰り返さない」、曜日は起動日の曜日
            comboBox_RepeatWeek.SelectedIndex = _repeatNoneIdx;
            comboBox_RepeatDayOfWeek.SelectedIndex = (int)dateTimePicker_Days.Value.DayOfWeek;

            UpdateRepeatSetting();
        }

        private void RepeatSetting_Changed(object sender, EventArgs e)
        {
            UpdateRepeatSetting();
        }

        // 曜日欄の有効/無効と「次回」の表示を、いまの設定に合わせて更新する
        private void UpdateRepeatSetting()
        {
            // InitializeComponentの途中(選択肢を入れる前)に呼ばれた場合は何もしない
            if (comboBox_RepeatWeek.SelectedIndex < 0 || comboBox_RepeatDayOfWeek.SelectedIndex < 0)
            {
                return;
            }

            bool isRepeat = comboBox_RepeatWeek.SelectedIndex != _repeatNoneIdx;
            comboBox_RepeatDayOfWeek.Enabled = isRepeat;

            // 起動時刻 = 日付欄の日付 + 時刻欄の時刻
            DateTime start = dateTimePicker_Days.Value.Date + dateTimePicker_Time.Value.TimeOfDay;
            DateTime now = DateTime.Now;

            if (!isRepeat)
            {
                label_NextReminder.Text = "次回：" + Environment.NewLine + MonthlyNthWeekday.FormatDateTime(start)
                    + (start < now ? Environment.NewLine + "(過ぎています)" : "");
                return;
            }

            // 起動時刻より前・過ぎた回は出さず、これから来る最初の回を出す
            WeekOfMonth week = (WeekOfMonth)comboBox_RepeatWeek.SelectedIndex;
            DayOfWeek dayOfWeek = (DayOfWeek)comboBox_RepeatDayOfWeek.SelectedIndex;
            DateTime from = start > now ? start : now;
            DateTime next = MonthlyNthWeekday.GetNextOccurrence(from, week, dayOfWeek, start.TimeOfDay);
            label_NextReminder.Text = "次回：" + Environment.NewLine + MonthlyNthWeekday.FormatDateTime(next);
        }
    }
}
