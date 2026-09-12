using System;
using System.Collections.Generic;
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
        private StcFileInputOutput FileIO = new StcFileInputOutput();
        private StcDebug Debug = new StcDebug();
        private bool IsTaskRun = false;

        // プロファイル(Cheetos.xml/Cheetos.json)の置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\Cheetos\配下(既定)にあり、exe直下にはその場所を示す
        // 小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common/UserDataLocation.cs]]、EventRecorderと同じ仕組み)
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder("Cheetos");

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

            // 起動時はJSON版があればそちらを優先して読み込む(今後はJSON保存が主流になっていく方針のため)
            String defaultJsonPath = Path.Combine(userDataFolder, SettingFileNameJson);
            String defaultXmlPath = Path.Combine(userDataFolder, SettingFileNameXml);
            LoadProfile(File.Exists(defaultJsonPath) ? defaultJsonPath : defaultXmlPath);

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

        private void ProfileLoad_Click(object sender, EventArgs e)
        {
            String LoadFileName = fio.SelectLoadFileName(SettingFileNameXml, userDataFolder);
            if (String.IsNullOrEmpty(LoadFileName))
            {
                return;
            }

            LoadProfile(LoadFileName);
            Profile.Text = Path.GetFileName(LoadFileName);
        }

        // プルダウンで既存ファイルが選ばれている時は、毎回ダイアログを開かず
        // 「上書きしますか?」の確認だけで済ませられるようにする(EventRecorderと同じ挙動)
        private void ProfileSave_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(Profile.Text, userDataFolder);
            if (String.IsNullOrEmpty(SaveFileName))
            {
                return;
            }

            if (SaveProfile(SaveFileName))
            {
                UpdateProfileListAll(Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
            }
        }

        // *******************************************************************************
        // データ保存先フォルダの変更(システムメニューから呼び出す)
        // ([[EventRecorder/Form1.cs]]の同名機能と同じ考え方)

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, Boolean bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Boolean AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, String lpNewItem);

        private const uint MF_SEPARATOR = 0x800;
        private const uint MF_STRING = 0x0;
        private const int WM_SYSCOMMAND = 0x112;

        // システムコマンドのIDは下位4bitをWindowsが予約しているため、16の倍数かつ
        // 0xF000未満にする必要がある(MSDN既定のルール)
        private const int SysMenuId_ChangeDataFolder = 0x1000;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            IntPtr systemMenu = GetSystemMenu(this.Handle, false);
            AppendMenu(systemMenu, MF_SEPARATOR, 0, String.Empty);
            AppendMenu(systemMenu, MF_STRING, SysMenuId_ChangeDataFolder, "データ保存先を変更(&D)...");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & 0xFFF0) == SysMenuId_ChangeDataFolder)
            {
                ChangeDataFolder();
                return;
            }

            base.WndProc(ref m);
        }

        // 保存先フォルダを選び直し、exe直下のポインタファイル(DataFolder.txt)を書き換える。
        // 実行中のuserDataFolder(readonly)はその場では切り替えない。
        // 変更は次回起動時から反映される、シンプルで安全な方式にしている
        private void ChangeDataFolder()
        {
            // フォルダ選択ダイアログの実装は[[_Common/DataFolderChooser.cs]]に集約してある
            // (「データ保存先を変更」機能を持つプロジェクト全部で見た目・挙動を統一するため)
            String selectedFolder = StandardTemplate.DataFolderChooser.ChooseFolder(
                "プロファイルの保存先フォルダを選んでください", userDataFolder);

            if (String.IsNullOrEmpty(selectedFolder))
            {
                return;
            }

            if (String.Equals(
                Path.GetFullPath(selectedFolder).TrimEnd('\\'),
                Path.GetFullPath(userDataFolder).TrimEnd('\\'),
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            DialogResult moveResult = MessageBox.Show(
                "既存のプロファイルを新しい保存先に移動しますか？" + Environment.NewLine + Environment.NewLine
                    + "移動元: " + userDataFolder + Environment.NewLine
                    + "移動先: " + selectedFolder,
                "Cheetos - プロファイルの引っ越し",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (moveResult == DialogResult.Yes)
            {
                MoveExistingProfiles(userDataFolder, selectedFolder);
            }

            StandardTemplate.UserDataLocation.SetUserDataFolder("Cheetos", selectedFolder);

            MessageBox.Show(
                "保存先を変更したよ" + Environment.NewLine + selectedFolder + Environment.NewLine + Environment.NewLine
                    + "今のセッションはこれまで通り" + Environment.NewLine + userDataFolder + Environment.NewLine
                    + "を使うよ。新しい保存先は次回起動時から反映されるよ",
                "Cheetos - データ保存先の変更",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // oldFolder直下(サブフォルダは対象外)にある*.xml/*.jsonプロファイルをnewFolderへ移動する。
        // 移動先に同名ファイルが既にある場合は、上書きせずスキップする(データ消失を避けるため)
        private void MoveExistingProfiles(String oldFolder, String newFolder)
        {
            List<String> profileFiles = Directory.GetFiles(oldFolder, "*.xml")
                .Concat(Directory.GetFiles(oldFolder, "*.json"))
                .ToList();

            List<String> movedFiles = new List<String>();
            List<String> skippedFiles = new List<String>();

            foreach (String sourcePath in profileFiles)
            {
                String fileName = Path.GetFileName(sourcePath);
                String destPath = Path.Combine(newFolder, fileName);

                if (File.Exists(destPath))
                {
                    skippedFiles.Add(fileName);
                    continue;
                }

                try
                {
                    File.Move(sourcePath, destPath);
                    movedFiles.Add(fileName);
                }
                catch (Exception)
                {
                    skippedFiles.Add(fileName);
                }
            }

            String message = movedFiles.Count + "件のプロファイルを移動したよ";
            if (skippedFiles.Count > 0)
            {
                message += Environment.NewLine + Environment.NewLine
                    + skippedFiles.Count + "件は移動先に同名ファイルが既にあった(または移動に失敗した)ためスキップしたよ:"
                    + Environment.NewLine + String.Join(Environment.NewLine, skippedFiles);
            }

            MessageBox.Show(
                message,
                "Cheetos - プロファイルの引っ越し",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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