﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using StandardTemplate;
using Picture;

namespace Cheetos
{
    public partial class Cheetos : Form
    {
        private enum DataGridType
        {
            EDIT_BOX,
            CHECK_BOX,
            DROP_DOWN,
        } ;

        private struct DataGridClass
        {
            public String HeaderName;
            public DataGridType Type;
        }

        private readonly String SettingFileName = @"Cheetos.xml";

        private const String GridHeaderSleepStr = "Sleep(msec)";
        private const String GridHeaderMouseXStr = "MouseX";
        private const String GridHeaderMouseYStr = "MouseY";
        private const String GridHeaderMouseActionStr = "MouseAction";
        private const String GridHeaderCaptureStr = "Capture";

        private const String MouseEventMoveStr = "Move";
        private const String MouseEventLeftDownStr = "LeftDown";

        private const String ExecuteStr = "〇";
        private const String NotExecuteStr = "×";

        private readonly DataGridClass[] DataGridParam = new DataGridClass[]{
            new DataGridClass() { HeaderName = GridHeaderSleepStr, Type = DataGridType.EDIT_BOX },
            new DataGridClass() { HeaderName = GridHeaderMouseXStr, Type = DataGridType.EDIT_BOX },
            new DataGridClass() { HeaderName = GridHeaderMouseYStr, Type = DataGridType.EDIT_BOX },
            new DataGridClass() { HeaderName = GridHeaderMouseActionStr, Type = DataGridType.DROP_DOWN },
            new DataGridClass() { HeaderName = GridHeaderCaptureStr, Type = DataGridType.DROP_DOWN }
        };

        private readonly String[] MouseEvent = new String[] {
            MouseEventMoveStr,
            MouseEventLeftDownStr
        };

        private readonly String[] CaptureEvent = new String[] {
            ExecuteStr,
            NotExecuteStr
        };

        private StcFileInputOutput fio = new StcFileInputOutput();
        private SaveRestore sr = new SaveRestore();
        private StcUtils util = new StcUtils();
        private StcFileInputOutput FileIO = new StcFileInputOutput();
        private StcDebug Debug = new StcDebug();
        private bool IsTaskRun = false;

        public Cheetos()
        {
            InitializeComponent();

            // アイコン設定
            this.Icon = Properties.Resources.Cheetos;

            // カレントディレクトリ移動
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            // デバッグログに時間を表示
            Debug.SetWriteTime(true);

            // DataGridViewの初期設定
            InitializeDataGridView();

            sr.RegistItem(this);
            sr.LoadProc(SettingFileName, this);
            util.UpdateProfileList(ref Profile);
        }

        private int GetDataGridColumnIdx(String ColumnName)
        {
            int ColumnIdx = 0;
            for (int i = 0; i < DataGridParam.Length; i++)
            {
                if (ColumnName.Equals(DataGridParam[i].HeaderName))
                {
                    ColumnIdx = i;
                    break;
                }
            }

            return ColumnIdx;
        }

        private CaptWindow.MOUSE_EVENT GetMouseEvent(String MouseEventStr)
        {
            CaptWindow.MOUSE_EVENT Event = CaptWindow.MOUSE_EVENT.LEFT_CLICK;
            switch (MouseEventStr)
            {
                case MouseEventMoveStr:
                    Event = CaptWindow.MOUSE_EVENT.MOVE;
                    break;

                case MouseEventLeftDownStr:
                    Event = CaptWindow.MOUSE_EVENT.LEFT_CLICK;
                    break;
            }
            return Event;
        }

        private Boolean IsCaptureEvent(String CaptureEventStr)
        {
            Boolean IsCapture = true;
            switch (CaptureEventStr)
            {
                case ExecuteStr:
                    IsCapture = true;
                    break;

                default: // nobreak
                case NotExecuteStr:
                    IsCapture = false;
                    break;
            }
            return IsCapture;
        }

        private void Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + Profile.Text;
            sr.LoadProc(LoadFileName, this);
        }

        private void ProfileLoad_Click(object sender, EventArgs e)
        {
            String LoadFileName = fio.SelectLoadFileName(SettingFileName);
            if (sr.LoadProc(LoadFileName, this))
            {
                Profile.Text = Path.GetFileName(LoadFileName);
            }
        }

        private void ProfileSave_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(Profile.Text);
            if (sr.SaveXmlFile(SaveFileName))
            {
                util.UpdateProfileList(ref Profile, Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
            }
        }

        private void fc_SourceFolderPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(fc_SourceFolderPath.Text, e);
        }

        private void fc_DestFolderPath_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(fc_DestFolderPath.Text, e);
        }

        private void fc_Button_Collect_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(fc_SourceFolderPath.Text))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }

            if (!fio.IsExistDirectory(fc_DestFolderPath.Text))
            {
                return;
            }

            // ファイルを一つ一つ移動する
            string[] files = Directory.GetFiles(fc_SourceFolderPath.Text, fc_TargetFileName.Text, SearchOption.TopDirectoryOnly);

            InitProgressBar(files.Length);
            for (int i = 0; i <= files.Length - 1; i++)
            {
                String DestName = fc_DestFolderPath.Text + @"\" + Path.GetFileName(files[i]);
                File.Move(files[i], DestName);

                // 進捗率の表示
                int ProgressVal = i + 1;
                TextBox_Status.Text = ProgressVal.ToString() + "/" + ProgressBar_Status.Maximum;
                ProgressBar_Status.Value = ProgressVal;
            }
        }

        private void label_DebugMode_DoubleClick(object sender, EventArgs e)
        {
            Boolean IsDebugMode = Debug.GetDebugMode();
            Debug.SetDebugMode(!IsDebugMode);
            MessageBox.Show("DebugMode=" + Debug.GetDebugMode().ToString());
        }

        public void SetStartTime()
        {
            textBox_StartTime.Text = DateTime.Now.ToString();
            textBox＿ExpectEndTime.Text = "";
        }

        public void SetExpectEndTime(int TotalNum)
        {
            // 開始時間
            DateTime dtStart = DateTime.Parse(textBox_StartTime.Text);

            // 終了時間(一個目)
            DateTime dtEnd = DateTime.Now;

            // 一個分の処理時間
            long ProcTime = dtEnd.Ticks - dtStart.Ticks;
            DateTime dtExpect = new DateTime(dtStart.Ticks + ProcTime * TotalNum);

            textBox＿ExpectEndTime.Text = dtExpect.ToString();
        }

        private void pm_TrimingHeight_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void pt_Radio_SelectPointOfEnd_Click(object sender, EventArgs e)
        {
            UpdatePitTrimSize();
        }

        private void pt_Radio_SelectSizeOfEnd_Click(object sender, EventArgs e)
        {
            UpdatePitTrimSize();
        }

        private void pt_ListBox_ListUp_DoubleClick(object sender, EventArgs e)
        {
            for (int i = 0; i < pt_ListBox_ListUp.SelectedItems.Count; i++)
            {
                String FilePath = pt_SourceFolderPath.Text + @"\" + pt_ListBox_ListUp.SelectedItems[i].ToString();
                util.ExecutePath(FilePath);
            }
        }

        private void pm_ListBox_ListUp_DoubleClick(object sender, EventArgs e)
        {
            for (int i = 0; i < pm_ListBox_ListUp.SelectedItems.Count; i++)
            {
                String FilePath = pm_SourceFolderPath.Text + @"\" + pm_ListBox_ListUp.SelectedItems[i].ToString();
                util.ExecutePath(FilePath);
            }
        }

        private void InitProgressBar(int Maximum, int Minimum = 0)
        {
            ProgressBar_Status.Maximum = Maximum;
            ProgressBar_Status.Minimum = 0;
            ProgressBar_Status.Value = 0;
        }

        private void InitializeDataGridView()
        {
            // 左端プロパティを非表示
            cw_dataGridView.RowHeadersVisible = false;

            // 最下部プロパティを非表示
            cw_dataGridView.AllowUserToAddRows = false;

            // 個別に挿入していないColumn項目数（ComboBox等を別途Insertしているので除外したい）
            int EditColumn = 0;
            for (int i = 0; i < DataGridParam.Length; i++)
            {
                if (DataGridParam[i].Type == DataGridType.EDIT_BOX)
                {
                    EditColumn++;
                }
            }
            cw_dataGridView.ColumnCount = EditColumn;

            {
                // ComboBoxのリスト作成
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
                for (int i = 0; i < MouseEvent.Length; i++)
                {
                    column.Items.Add(MouseEvent[i]);
                }
                cw_dataGridView.Columns.Add(column);

                //DataGridViewCheckBoxColumn CheckColumn = new DataGridViewCheckBoxColumn();
                //cw_dataGridView.Columns.Add(CheckColumn);

                column = new DataGridViewComboBoxColumn();
                for (int i = 0; i < CaptureEvent.Length; i++)
                {
                    column.Items.Add(CaptureEvent[i]);
                }
                cw_dataGridView.Columns.Add(column);

                // ヘッダ作成
                for (int i = 0; i < DataGridParam.Length; i++)
                {
                    cw_dataGridView.Columns[i].HeaderText = DataGridParam[i].HeaderName;
                }
            }

            // 幅設定
            cw_dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            cw_dataGridView.RowCount = 1;
        }

        private void cw_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            switch(DataGridParam[e.ColumnIndex].Type)
            {
                case  DataGridType.DROP_DOWN :
                    dgv.BeginEdit(false);
                    var edt = cw_dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                    edt.DroppedDown = true;
                    break;

                case DataGridType.CHECK_BOX:
                    // TODO：ダブルクリックで値を設定したい
                    break;

                default :
                    break;
            }
        }

        private void cw_Button_AddLine_Click(object sender, EventArgs e)
        {
            int InsertIndex = cw_dataGridView.CurrentRow.Index + 1;
            cw_dataGridView.Rows.Insert(InsertIndex);

            int ColumnIdx = GetDataGridColumnIdx(GridHeaderMouseActionStr);
            util.SetDataGridCell(cw_dataGridView, InsertIndex, ColumnIdx, MouseEventMoveStr);

            ColumnIdx = GetDataGridColumnIdx(GridHeaderCaptureStr);
            util.SetDataGridCell(cw_dataGridView, InsertIndex, ColumnIdx, NotExecuteStr);
        }

        private void cw_Button_DelLine_Click(object sender, EventArgs e)
        {
            if (cw_dataGridView.RowCount > 1)
            {
                cw_dataGridView.Rows.RemoveAt(cw_dataGridView.CurrentRow.Index);
            }
        }

        private void MergeExec_Click(object sender, EventArgs e)
        {
            MergeExec();
        }

        private void Button_MergeListup_Click(object sender, EventArgs e)
        {
            ListupPictMerge();
        }

        private void pm_ListBox_ListUp_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void pm_ListBox_ListUp_SelectedIndexChanged(object sender, EventArgs e)
        {
            pm_TextBox_Status.Text = "ファイル数：" + pm_ListBox_ListUp.SelectedItems.Count.ToString();
        }

        private void pr_ListBox_ListUp_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void pt_Label_SourceFolderPath_DoubleClick(object sender, EventArgs e)
        {
            pt_SourceFolderPath.ReadOnly = !pt_SourceFolderPath.ReadOnly;
        }

        private void pr_Label_SourceFolderPath_DoubleClick(object sender, EventArgs e)
        {
            pr_SourceFolderPath.ReadOnly = !pr_SourceFolderPath.ReadOnly;
        }

        private void do_Label_SourceFolderPath_DoubleClick(object sender, EventArgs e)
        {
            do_SourceFolderPath.ReadOnly = !do_SourceFolderPath.ReadOnly;
        }

        private void do_Label_DestPortFolderPath_DoubleClick(object sender, EventArgs e)
        {
            do_DestPortFolderPath.ReadOnly = !do_DestPortFolderPath.ReadOnly;
        }

        private void do_Label_DestLandFolderPath_DoubleClick(object sender, EventArgs e)
        {
            do_DestLandFolderPath.ReadOnly = !do_DestLandFolderPath.ReadOnly;
        }

        private void pm_Label_SourceFolderPath_DoubleClick(object sender, EventArgs e)
        {
            pm_SourceFolderPath.ReadOnly = !pm_SourceFolderPath.ReadOnly;
        }

        private void fc_Label_SourceFolderPath_DoubleClick(object sender, EventArgs e)
        {
            fc_SourceFolderPath.ReadOnly = !fc_SourceFolderPath.ReadOnly;
        }
    }
}
