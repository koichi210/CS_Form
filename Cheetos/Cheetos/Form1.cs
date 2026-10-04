using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace Cheetos
{
    partial class Cheetos : StcBaseForm<SaveRestore>
    {
        private enum DataGridType
        {
            EditBox,
            CheckBox,
            DropDown,
        };

        private struct DataGridColumnDef
        {
            public String HeaderName;
            public DataGridType Type;
        }

        private const String _gridHeaderSleepStr = "Sleep(msec)";
        private const String _gridHeaderMouseXStr = "MouseX";
        private const String _gridHeaderMouseYStr = "MouseY";
        private const String _gridHeaderMouseActionStr = "MouseAction";
        private const String _gridHeaderCaptureStr = "Capture";

        private const String _mouseEventMoveStr = "Move";
        private const String _mouseEventLeftDownStr = "LeftDown";

        private const String _executeStr = "〇";
        private const String _notExecuteStr = "×";

        private readonly DataGridColumnDef[] _dataGridColumns = new DataGridColumnDef[]{
            new DataGridColumnDef() { HeaderName = _gridHeaderSleepStr, Type = DataGridType.EditBox },
            new DataGridColumnDef() { HeaderName = _gridHeaderMouseXStr, Type = DataGridType.EditBox },
            new DataGridColumnDef() { HeaderName = _gridHeaderMouseYStr, Type = DataGridType.EditBox },
            new DataGridColumnDef() { HeaderName = _gridHeaderMouseActionStr, Type = DataGridType.DropDown },
            new DataGridColumnDef() { HeaderName = _gridHeaderCaptureStr, Type = DataGridType.DropDown }
        };

        private readonly String[] _mouseEventItems = new String[] {
            _mouseEventMoveStr,
            _mouseEventLeftDownStr
        };

        private readonly String[] _captureEventItems = new String[] {
            _executeStr,
            _notExecuteStr
        };

        private readonly StcFileInputOutput _fio = new StcFileInputOutput();
        private readonly StcDebug _debugLog = new StcDebug();
        private bool _isCaptureRunning = false;

        // プロファイル(Cheetos.xml/Cheetos.json)の置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\Cheetos\配下(既定)にあり、exe直下にはその場所を示す
        // 小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common/UserDataLocation.cs]]、EventRecorderと同じ仕組み)
        private const String _appName = "Cheetos";
        private readonly String _userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(_appName);
        private static readonly String[] _profileExtensions = { "*.json", "*.xml" };

        private const String _settingFileNameXml = @"Cheetos.xml";
        private const String _settingFileNameJson = @"Cheetos.json";

        public Cheetos()
        {
            InitializeComponent();

            InitializeCommonSettings(Properties.Resources.Cheetos);

            // CurrentScreenキャプチャで「このウィンドウが今あるモニタ」を判定できるようにする
            _captWindow.TargetWindow = this;

            // デバッグログに時間を表示
            _debugLog.UseTimeInLog = true;

            // DataGridViewの初期設定
            InitializeDataGridView();

            InitializePlaceholders();
            InitializeToolTips();

            _sr.RegisterItem(this);

            // 起動時はJSONを読む。旧XMLしか無ければ読み込んでJSONへ保存し直し、旧XMLは削除する
            // ([[_Common/JsonSaveRestore.cs]])
            String defaultJsonPath = Path.Combine(_userDataFolder, _settingFileNameJson);
            String defaultXmlPath = Path.Combine(_userDataFolder, _settingFileNameXml);
            JsonSaveRestore.LoadWithMigration(_sr, defaultJsonPath, defaultXmlPath, LoadProfileFromXml);

            UpdateProfileListAll("");
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnlyの欄(フォルダ選択ボタンで入る欄・ステータス欄)と、
        // Multilineの欄(pm_TrimmingHeight、Windowsの仕様で表示されない)は対象外
        private void InitializePlaceholders()
        {
            cw_TextBox_SavePath.PlaceholderText = @"例: C:\Capture";
            cw_TextBox_SaveFilePrefix.PlaceholderText = "例: capture_";
            cw_TextBox_Sleep.PlaceholderText = "例: 2000";
            cw_TextBox_Loop.PlaceholderText = "例: 2";

            pt_BaseX.PlaceholderText = "例: 0";
            pt_BaseY.PlaceholderText = "例: 0";
            pt_TargetX.PlaceholderText = "例: 1920";
            pt_TargetY.PlaceholderText = "例: 1080";

            pr_BaseX.PlaceholderText = "例: 0";
            pr_BaseY.PlaceholderText = "例: 0";
            pr_Angle.PlaceholderText = "例: 90";

            do_TargetFileName.PlaceholderText = "例: *.jpg";
            do_WhiteLength.PlaceholderText = "例: 10";
            do_WhiteCoef.PlaceholderText = "例: 30";
            do_SampleFilePath.PlaceholderText = @"例: C:\Sample\001.jpg";

            pm_SourceFile1Prefix.PlaceholderText = "例: _1";
            pm_SourceFile2Prefix.PlaceholderText = "例: _2";

            fc_TargetFileName.PlaceholderText = "例: *.png";
            fc_DestFolderPath.PlaceholderText = @"例: C:\Collect";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            const String readOnlyFolderTip = "フォルダのドラッグ&ドロップで設定する。左のラベルをダブルクリックすると直接編集できる。Enterでフォルダを開く";
            const String readOnlyLabelTip = "ダブルクリックで右の欄の直接編集を切り替える";

            // 共通
            toolTip.SetToolTip(Profile, "選ぶとその設定ファイルを読み込む");
            toolTip.SetToolTip(Button_ProfileSave, "現在の設定をファイルに保存する(Ctrl+Sでも可)。プロファイル選択中なら、まずそのファイルへ上書きするか確認する");

            // CaptureWindow
            toolTip.SetToolTip(cw_TextBox_SavePath, "キャプチャ画像の保存先(無ければ自動で作成)。手入力すると他タブのフォルダ欄にも同じパスが入る(DistOrientの移動先は末尾に_port/_landを付けたもの)");
            toolTip.SetToolTip(cw_TextBox_SaveFilePrefix, "保存ファイル名の先頭に付ける文字列。ファイル名は「接頭辞_[タイムスタンプ_]繰り返し番号4桁_撮影番号.png」になる");
            toolTip.SetToolTip(cw_checkBox_AddTimeStamp, "ファイル名の接頭辞の後に、Capture開始時刻(yyyy_MM_dd_HH_mm_ss)を付ける");
            toolTip.SetToolTip(cw_TextBox_Sleep, "Capture開始前に待つ時間(ミリ秒)。空欄なら待たない。実際は1秒単位に切り上げて待つ");
            toolTip.SetToolTip(cw_TextBox_Loop, "グリッドの全行を繰り返す回数。空欄なら1回。↑↓キーで1ずつ増減できる");
            toolTip.SetToolTip(cw_Radio_FullScreen, "Ctrl+PrintScreenで画面全体を撮る");
            toolTip.SetToolTip(cw_Radio_CurrentScreen, "このウィンドウが表示されているモニタ1枚だけを撮る");
            toolTip.SetToolTip(cw_Radio_CurrentWindow, "Alt+PrintScreenで、撮影時点でアクティブなウィンドウを撮る");
            toolTip.SetToolTip(cw_Button_Capture, "グリッドの各行を上から順に実行(マウス操作→Sleep→キャプチャ)し、それを繰り返し数だけ行う。終了後はマウスカーソルを押した時の位置へ戻す。処理中に押すと中断");
            toolTip.SetToolTip(cw_Button_AddLine, "選択中の行の下に1行挿入する(MouseAction=Move、Capture=×)");
            toolTip.SetToolTip(cw_TextBox_Status, "このタブ上でマウスを動かすと、現在のマウス座標(画面座標)を表示する");
            cw_dataGridView.Columns[GetDataGridColumnIdx(_gridHeaderSleepStr)].ToolTipText = "マウス操作の後、キャプチャ前に待つ時間(ミリ秒、1秒単位に切り上げ)。空欄なら直前に使った待ち時間を引き継ぐ";
            cw_dataGridView.Columns[GetDataGridColumnIdx(_gridHeaderMouseXStr)].ToolTipText = "マウスを動かす先のX座標(画面座標、px)。MouseX/MouseYのどちらかが空欄ならマウス操作をしない";
            cw_dataGridView.Columns[GetDataGridColumnIdx(_gridHeaderMouseYStr)].ToolTipText = "マウスを動かす先のY座標(画面座標、px)。MouseX/MouseYのどちらかが空欄ならマウス操作をしない";
            cw_dataGridView.Columns[GetDataGridColumnIdx(_gridHeaderMouseActionStr)].ToolTipText = "Move: 移動だけ。LeftDown(未設定も含む): 移動して左クリック(押して離す)";
            cw_dataGridView.Columns[GetDataGridColumnIdx(_gridHeaderCaptureStr)].ToolTipText = "〇の行だけSleepの後にキャプチャする(×・未設定は撮らない)";

            // PictTrim
            toolTip.SetToolTip(pt_SourceFolderPath, readOnlyFolderTip);
            toolTip.SetToolTip(pt_Label_SourceFolderPath, readOnlyLabelTip);
            toolTip.SetToolTip(pt_BaseX, "切り取る範囲の左上のX座標(px)");
            toolTip.SetToolTip(pt_BaseY, "切り取る範囲の左上のY座標(px)");
            toolTip.SetToolTip(pt_Radio_SelectPointOfEnd, "下のX/Yを右下の座標(px、その位置自体は含まない)として扱う。切り替えると入力中の値を換算し直す");
            toolTip.SetToolTip(pt_Radio_SelectSizeOfEnd, "下のX/Yを切り取る幅/高さ(px)として扱う。切り替えると入力中の値を換算し直す");
            toolTip.SetToolTip(pt_TargetX, "「座標で指定」なら右端のX座標、「サイズで指定」なら幅(px)");
            toolTip.SetToolTip(pt_TargetY, "「座標で指定」なら下端のY座標、「サイズで指定」なら高さ(px)");
            toolTip.SetToolTip(pt_ListBox_ListUp, "選択したファイルだけが処理対象(Ctrl+Aで全選択)。ダブルクリックで選択中のファイルを開く");
            toolTip.SetToolTip(pt_Button_Trim, "選択したファイルを切り取って上書き保存する。元ファイルはフォルダ内のBk_Trimへコピーしておく。処理中に押すと中断");

            // Rotation
            toolTip.SetToolTip(pr_SourceFolderPath, readOnlyFolderTip);
            toolTip.SetToolTip(pr_Label_SourceFolderPath, readOnlyLabelTip);
            toolTip.SetToolTip(pr_BaseX, "回転後の画像で、元画像の左上隅を置くX座標(px)。この点を中心に回転する");
            toolTip.SetToolTip(pr_BaseY, "回転後の画像で、元画像の左上隅を置くY座標(px)。この点を中心に回転する");
            toolTip.SetToolTip(pr_Angle, "回転角度(度、整数)。正の値で時計回り");
            toolTip.SetToolTip(pr_Button_RotationPreview, "画像を読み込んで原点・角度を試せるプレビューを開く。OKで閉じるとその値をこの画面に反映する");
            toolTip.SetToolTip(pr_ListBox_ListUp, "選択したファイルだけが処理対象(Ctrl+Aで全選択)");
            toolTip.SetToolTip(pr_Button_Rotation, "選択したファイルを回転して上書き保存する(画像サイズは元画像の対角線長の正方形になる)。元ファイルはBk_Rotateへコピーしておく。処理中に押すと中断");

            // DistOrient
            toolTip.SetToolTip(do_SourceFolderPath, readOnlyFolderTip);
            toolTip.SetToolTip(do_Label_SourceFolderPath, readOnlyLabelTip);
            toolTip.SetToolTip(do_DestPortFolderPath, "縦と判定した画像の移動先。" + readOnlyFolderTip);
            toolTip.SetToolTip(do_Label_DestPortFolderPath, readOnlyLabelTip);
            toolTip.SetToolTip(do_DestLandFolderPath, "横と判定した画像の移動先。" + readOnlyFolderTip);
            toolTip.SetToolTip(do_Label_DestLandFolderPath, readOnlyLabelTip);
            toolTip.SetToolTip(do_TargetFileName, "振り分けるファイルの検索パターン(*や?が使える)。移動元フォルダの直下だけが対象");
            toolTip.SetToolTip(do_WhiteLength, "白フチかどうかを調べる、画像の左右両端の帯の幅(px)。画像の幅より大きい場合は画像の幅になる");
            toolTip.SetToolTip(do_WhiteCoef, "白と見なすしきい値の係数(既定30)。しきい値=帯の幅×画像の高さ÷係数で、大きいほど白と判定されにくく横に振り分けられやすい");
            toolTip.SetToolTip(do_Distribute, "左右両端の帯をPNGにした時のサイズが両方ともしきい値以下(白フチあり)なら縦、そうでなければ横の移動先へファイルを移動する。処理中に押すと中断");
            toolTip.SetToolTip(do_SampleFilePath, "長さ/係数を試すための画像ファイルのパス");
            toolTip.SetToolTip(do_GetSampleParam, "サンプル画像を今の長さ/係数で判定し、結果・しきい値・左右の帯のサイズを表示する(ファイルは移動しない)");

            // PictMerge
            toolTip.SetToolTip(pm_SourceFolderPath, readOnlyFolderTip);
            toolTip.SetToolTip(pm_Label_SourceFolderPath, readOnlyLabelTip);
            toolTip.SetToolTip(pm_SourceFile1Prefix, "結合先のファイルを見分ける、拡張子の直前の文字列(例: 001_1.jpgなら_1)。このファイルに上書き保存し、元はBk_Mergeへコピーしておく");
            toolTip.SetToolTip(pm_SourceFile2Prefix, "切り出し元のファイルを見分ける、拡張子の直前の文字列。結合先の名前のこの部分を置き換えたファイルを使い、結合後はBk_Mergeへ移動する");
            toolTip.SetToolTip(pm_TrimmingHeight, "マージ元から結合先の同じ位置へ写す縦の範囲(px)を、1行に1つ「開始,終了」で書く。「-」は開始なら0、終了なら画像の下端の意味");
            toolTip.SetToolTip(pm_ListBox_ListUp, "選択したファイルのうち、マージ先の識別子を含むものだけ処理する(Ctrl+Aで全選択)。ダブルクリックで選択中のファイルを開く");
            toolTip.SetToolTip(pm_Button_Merge, "選択したファイルを結合する。処理中に押すと中断");

            // FileCollect
            toolTip.SetToolTip(fc_SourceFolderPath, readOnlyFolderTip);
            toolTip.SetToolTip(fc_Label_SourceFolderPath, readOnlyLabelTip);
            toolTip.SetToolTip(fc_TargetFileName, "移動するファイルの検索パターン(*や?が使える)。移動元フォルダの直下だけが対象");
            toolTip.SetToolTip(fc_MoveFile, "移動元直下の対象ファイルを移動先へ移動する(コピーではない)。移動先が無ければ作成するか確認する");
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
                _sr.LoadJsonFile(filePath);
            }
            else
            {
                _sr.LoadProc(filePath, this);
            }
        }

        // 旧XMLの読み込み(移行用)。JsonSaveRestore.LoadWithMigrationへ渡す
        private Boolean LoadProfileFromXml(String path)
        {
            return _sr.LoadProc(path, this);
        }

        // 設定ファイルを拡張子で振り分けて保存する
        private Boolean SaveProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                return _sr.SaveJsonFile(filePath);
            }

            return _sr.SaveXmlFile(filePath);
        }

        // Profile(コンボボックス)へ、userDataFolder配下の*.xmlと*.jsonの両方をまとめてリストアップする。
        // _util.UpdateProfileListは拡張子を1パターンしか指定できないため、2回検索した結果をマージする
        private void UpdateProfileListAll(String defaultProfileName)
        {
            String[] xmlFiles = Directory.GetFiles(_userDataFolder, "*.xml", SearchOption.AllDirectories);
            String[] jsonFiles = Directory.GetFiles(_userDataFolder, "*.json", SearchOption.AllDirectories);
            String[] files = xmlFiles.Concat(jsonFiles).ToArray();

            _util.SetComboBoxFromArray(Profile, files, _userDataFolder);
            _util.SetComboBoxText(Profile, defaultProfileName);
        }

        // 見つからない場合は0(先頭列)を返す
        private int GetDataGridColumnIdx(String columnName)
        {
            int index = Array.FindIndex(_dataGridColumns, c => c.HeaderName == columnName);
            return Math.Max(index, 0);
        }

        // "Move"以外(未設定含む)はLeftClick扱い
        private CaptWindow.MouseEventType GetMouseEvent(String mouseEventStr)
        {
            if (mouseEventStr == _mouseEventMoveStr)
            {
                return CaptWindow.MouseEventType.Move;
            }
            return CaptWindow.MouseEventType.LeftClick;
        }

        // "〇"のときだけキャプチャする(未設定含む、それ以外はキャプチャしない)
        private Boolean IsCaptureEvent(String captureEventStr)
        {
            return captureEventStr == _executeStr;
        }

        private void Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProfile(Path.Combine(_userDataFolder, Profile.Text));
        }

        // プルダウンで既存ファイルが選ばれている時は、毎回ダイアログを開かず
        // 「上書きしますか?」の確認だけで済ませられるようにする(EventRecorderと同じ挙動)
        private void ProfileSave_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(_util, _fio, Profile, _profileExtensions, SaveProfile, _userDataFolder);
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
                DataFolderMenu.ChangeDataFolder(_appName, _userDataFolder,
                    (oldFolder, newFolder) => DataFolderMenu.MoveProfiles(oldFolder, newFolder, _appName));
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

            if (!_fio.EnsureDirectory(fc_DestFolderPath.Text))
            {
                return;
            }

            // ファイルを一つ一つ移動する
            string[] files = Directory.GetFiles(fc_SourceFolderPath.Text, fc_TargetFileName.Text, SearchOption.TopDirectoryOnly);

            InitProgressBar(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                String destName = fc_DestFolderPath.Text + @"\" + Path.GetFileName(files[i]);
                File.Move(files[i], destName);

                // 進捗率の表示
                int progressVal = i + 1;
                TextBox_Status.Text = progressVal.ToString() + "/" + ProgressBar_Status.Maximum;
                ProgressBar_Status.Value = progressVal;
            }
        }

        private void label_DebugMode_DoubleClick(object sender, EventArgs e)
        {
            _debugLog.IsDebugMode = !_debugLog.IsDebugMode;
            MessageBox.Show("DebugMode=" + _debugLog.IsDebugMode.ToString());
        }

        public void SetStartTime()
        {
            textBox_StartTime.Text = DateTime.Now.ToString();
            textBox_ExpectEndTime.Text = "";
        }

        public void SetExpectEndTime(int totalNum)
        {
            // 開始時間
            DateTime dtStart = DateTime.Parse(textBox_StartTime.Text);

            // 終了時間(一個目)
            DateTime dtEnd = DateTime.Now;

            // 一個分の処理時間
            long procTime = dtEnd.Ticks - dtStart.Ticks;
            DateTime dtExpect = new DateTime(dtStart.Ticks + procTime * totalNum);

            textBox_ExpectEndTime.Text = dtExpect.ToString();
        }

        private void pm_TrimmingHeight_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private void pt_Radio_SelectPointOfEnd_Click(object sender, EventArgs e)
        {
            UpdatePictTrimSize();
        }

        private void pt_Radio_SelectSizeOfEnd_Click(object sender, EventArgs e)
        {
            UpdatePictTrimSize();
        }

        private void pt_ListBox_ListUp_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedFiles(pt_SourceFolderPath, pt_ListBox_ListUp);
        }

        private void pm_ListBox_ListUp_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedFiles(pm_SourceFolderPath, pm_ListBox_ListUp);
        }

        // リストボックスで選択中のファイルを関連付けで開く(Trim/Mergeの各タブで共通)
        private void OpenSelectedFiles(TextBox folderPathCtrl, ListBox listCtrl)
        {
            for (int i = 0; i < listCtrl.SelectedItems.Count; i++)
            {
                String filePath = folderPathCtrl.Text + @"\" + listCtrl.SelectedItems[i].ToString();
                _util.ExecutePath(filePath);
            }
        }

        private void InitProgressBar(int maximum)
        {
            ProgressBar_Status.Maximum = maximum;
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
        private void ListUpFolderFiles(TextBox folderPathCtrl, ListBox listCtrl)
        {
            if (!Directory.Exists(folderPathCtrl.Text))
            {
                MessageBox.Show("フォルダパスが不正です");
                return;
            }

            listCtrl.Items.Clear();

            // 1件ずつAddすると都度再描画が走るため、まとめてAddRangeする
            string[] files = Directory.GetFiles(folderPathCtrl.Text, "*", SearchOption.TopDirectoryOnly);
            listCtrl.Items.AddRange(files.Select(Path.GetFileName).ToArray<object>());
        }

        private void InitializeDataGridView()
        {
            // 左端プロパティを非表示
            cw_dataGridView.RowHeadersVisible = false;

            // 最下部プロパティを非表示
            cw_dataGridView.AllowUserToAddRows = false;

            // 個別に挿入していないColumn項目数（ComboBox等を別途Insertしているので除外したい）
            cw_dataGridView.ColumnCount = _dataGridColumns.Count(c => c.Type == DataGridType.EditBox);

            // ComboBoxのリスト作成
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            column.Items.AddRange(_mouseEventItems);
            cw_dataGridView.Columns.Add(column);

            column = new DataGridViewComboBoxColumn();
            column.Items.AddRange(_captureEventItems);
            cw_dataGridView.Columns.Add(column);

            // ヘッダ作成
            for (int i = 0; i < _dataGridColumns.Length; i++)
            {
                cw_dataGridView.Columns[i].HeaderText = _dataGridColumns[i].HeaderName;
            }

            // 幅設定
            cw_dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            cw_dataGridView.RowCount = 1;
        }

        private void cw_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            switch (_dataGridColumns[e.ColumnIndex].Type)
            {
                case DataGridType.DropDown:
                    dgv.BeginEdit(false);
                    var edt = cw_dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                    edt.DroppedDown = true;
                    break;

                case DataGridType.CheckBox:
                    // TODO：ダブルクリックで値を設定したい
                    break;

                default:
                    break;
            }
        }

        private void cw_Button_AddLine_Click(object sender, EventArgs e)
        {
            int insertIndex = cw_dataGridView.CurrentRow.Index + 1;
            cw_dataGridView.Rows.Insert(insertIndex);

            int columnIdx = GetDataGridColumnIdx(_gridHeaderMouseActionStr);
            _util.SetDataGridCell(cw_dataGridView, insertIndex, columnIdx, _mouseEventMoveStr);

            columnIdx = GetDataGridColumnIdx(_gridHeaderCaptureStr);
            _util.SetDataGridCell(cw_dataGridView, insertIndex, columnIdx, _notExecuteStr);
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
            ListUpPictMerge();
        }

        private void pm_ListBox_ListUp_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        private void pm_ListBox_ListUp_SelectedIndexChanged(object sender, EventArgs e)
        {
            pm_TextBox_Status.Text = "ファイル数：" + pm_ListBox_ListUp.SelectedItems.Count.ToString();
        }

        private void pr_ListBox_ListUp_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
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
            _util.ExecutePath((sender as TextBox).Text, e);
        }
    }
}