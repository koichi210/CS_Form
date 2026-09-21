using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using StandardTemplate;

namespace Cheetos
{
    partial class Cheetos : StcBaseForm<SaveRestore>
    {
        private enum DataGridType
        {
            EDIT_BOX,
            CHECK_BOX,
            DROP_DOWN,
        };

        private struct DataGridClass
        {
            public String HeaderName;
            public DataGridType Type;
        }

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
        private StcDebug Debug = new StcDebug();
        private bool IsTaskRun = false;

        // プロファイル(Cheetos.xml/Cheetos.json)の置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\Cheetos\配下(既定)にあり、exe直下にはその場所を示す
        // 小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common/UserDataLocation.cs]]、EventRecorderと同じ仕組み)
        private const String AppName = "Cheetos";
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(AppName);
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        private readonly String SettingFileNameXml = @"Cheetos.xml";
        private readonly String SettingFileNameJson = @"Cheetos.json";

        public Cheetos()
        {
            InitializeComponent();

            InitializeCommonSettings(Properties.Resources.Cheetos);

            // CURRENT_SCREENキャプチャで「このウィンドウが今あるモニタ」を判定できるようにする
            cw.TargetWindow = this;

            // デバッグログに時間を表示
            Debug.SetWriteTime(true);

            // DataGridViewの初期設定
            InitializeDataGridView();

            sr.RegistItem(this);

            // 起動時はJSONを読む。旧XMLしか無ければ読み込んでJSONへ保存し直し、旧XMLは削除する
            // ([[_Common/JsonSaveRestore.cs]])
            String defaultJsonPath = Path.Combine(userDataFolder, SettingFileNameJson);
            String defaultXmlPath = Path.Combine(userDataFolder, SettingFileNameXml);
            JsonSaveRestore.LoadWithMigration(sr, defaultJsonPath, defaultXmlPath, LoadProfileFromXml);

            UpdateProfileListAll("");
        }

        // *******************************************************************************
        // JSON保存/読込([[_Common/JsonFileStorage.cs]])。設定値はこれまでXML(StcSaveRestore)
        // 一本だったが、今後はJSONへ段階的に移行していく方針のため、拡張子で振り分ける
        // (EventRecorderと同じ考え方)

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 設定ファイルを拡張子で振り分けて読み込む。拡張子がjsonならJSON、それ以外は従来通りXML
        private void LoadProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                sr.LoadJsonFile(filePath);
            }
            else
            {
                sr.LoadProc(filePath, this);
            }
        }

        // 旧XMLの読み込み(移行用)。JsonSaveRestore.LoadWithMigrationへ渡す
        private Boolean LoadProfileFromXml(String path)
        {
            return sr.LoadProc(path, this);
        }

        // 設定ファイルを拡張子で振り分けて保存する
        private Boolean SaveProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                return sr.SaveJsonFile(filePath);
            }

            return sr.SaveXmlFile(filePath);
        }

        // Profile(コンボボックス)へ、userDataFolder配下の*.xmlと*.jsonの両方をまとめてリストアップする。
        // util.UpdateProfileListは拡張子を1パターンしか指定できないため、2回検索した結果をマージする
        private void UpdateProfileListAll(String defaultProfileName)
        {
            String[] xmlFiles = Directory.GetFiles(userDataFolder, "*.xml", SearchOption.AllDirectories);
            String[] jsonFiles = Directory.GetFiles(userDataFolder, "*.json", SearchOption.AllDirectories);
            String[] files = xmlFiles.Concat(jsonFiles).ToArray();

            util.SetComboBoxFromArray(Profile, files, userDataFolder);
            util.SetComboBoxText(Profile, defaultProfileName);
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
            String LoadFileName = Path.Combine(userDataFolder, Profile.Text);
            LoadProfile(LoadFileName);
        }

        // プルダウンで既存ファイルが選ばれている時は、毎回ダイアログを開かず
        // 「上書きしますか?」の確認だけで済ませられるようにする(EventRecorderと同じ挙動)
        private void ProfileSave_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, Profile, ProfileExtensions, SaveProfile, userDataFolder);
        }

        // Ctrl+Sで「設定値保存」ボタンと同じ動作にする(テキストボックス等にフォーカスがあっても拾える)
        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                ProfileSave_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // *******************************************************************************
        // データ保存先フォルダの変更(システムメニューから呼び出す)
        // ([[EventRecorder/Form1.cs]]の同名機能と同じ考え方)

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DataFolderMenu.AppendToSystemMenu(this);
        }

        protected override void WndProc(ref Message m)
        {
            if (DataFolderMenu.IsChangeDataFolderCommand(m))
            {
                DataFolderMenu.ChangeDataFolder(AppName, userDataFolder,
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, AppName));
                return;
            }

            base.WndProc(ref m);
        }


        private void fc_Button_Collect_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(fc_SourceFolderPath.Text))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }

            if (!fio.EnsureDirectory(fc_DestFolderPath.Text))
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

        // 各タブのBackgroundWorker(Trim/Merge/Rotation/Orient)は進捗表示がまったく同じだったため、
        // 1つのハンドラを4つのProgressChangedから共有する(結線はDesigner側)
        private void BkgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            TextBox_Status.Text = e.ProgressPercentage + "/" + ProgressBar_Status.Maximum;
            ProgressBar_Status.Value = e.ProgressPercentage;

            // 一回目の更新時に、予想終了時間を表示
            if (e.ProgressPercentage == 0)
            {
                SetExpectEndTime(ProgressBar_Status.Maximum);
            }
        }

        // 指定フォルダ直下のファイル名をリストボックスへ並べる(Trim/Merge/Rotationの各タブで共通)
        private void ListupFolderFiles(TextBox FolderPathCtrl, ListBox ListCtrl)
        {
            if (!Directory.Exists(FolderPathCtrl.Text))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }

            ListCtrl.Items.Clear();

            string[] files = Directory.GetFiles(FolderPathCtrl.Text, "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                ListCtrl.Items.Add(Path.GetFileName(files[i]));
            }
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

            switch (DataGridParam[e.ColumnIndex].Type)
            {
                case DataGridType.DROP_DOWN:
                    dgv.BeginEdit(false);
                    var edt = cw_dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                    edt.DroppedDown = true;
                    break;

                case DataGridType.CHECK_BOX:
                    // TODO：ダブルクリックで値を設定したい
                    break;

                default:
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

        private void UpdateReadOnly(object sender, EventArgs e)
        {
            switch ((sender as Label).Name)
            {
                case "pt_Label_SourceFolderPath":
                    pt_SourceFolderPath.ReadOnly = !pt_SourceFolderPath.ReadOnly;
                    break;
                case "pr_Label_SourceFolderPath":
                    pr_SourceFolderPath.ReadOnly = !pr_SourceFolderPath.ReadOnly;
                    break;
                case "do_Label_SourceFolderPath":
                    do_SourceFolderPath.ReadOnly = !do_SourceFolderPath.ReadOnly;
                    break;
                case "do_Label_DestPortFolderPath":
                    do_DestPortFolderPath.ReadOnly = !do_DestPortFolderPath.ReadOnly;
                    break;
                case "do_Label_DestLandFolderPath":
                    do_DestLandFolderPath.ReadOnly = !do_DestLandFolderPath.ReadOnly;
                    break;
                case "pm_Label_SourceFolderPath":
                    pm_SourceFolderPath.ReadOnly = !pm_SourceFolderPath.ReadOnly;
                    break;
                case "fc_Label_SourceFolderPath":
                    fc_SourceFolderPath.ReadOnly = !fc_SourceFolderPath.ReadOnly;
                    break;
            }
        }
        private void ExecutePath(object sender, KeyEventArgs e)
        {
            util.ExecutePath((sender as TextBox).Text, e);
        }
    }
}