using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace FileArranger
{
    partial class FileArranger : StcBaseForm<SaveRestore>
    {
        private const String _settingFileNameXml = @"FileArranger.xml";
        private const String _settingFileNameJson = @"FileArranger.json";

        // プロファイル(FileArranger.xml/.json)の置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\FileArranger\配下(既定)にあり、exe直下にはその場所を示す
        // 小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common/UserDataLocation.cs]]、EventRecorderと同じ仕組み)
        private const String _appName = "FileArranger";
        private readonly String _userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(_appName);
        private static readonly String[] _profileExtensions = { "*.json", "*.xml" };

        private static readonly String[] _renameDirColumns = { "変更前", "変更後" };
        private static readonly String[] _partitionFileColumns = { "対象", "移動前名称", "移動後名称" };

        // rd_listView_Targetの列Idx
        private const int _renameSrcIdx = 0;
        private const int _renameDestIdx = 1;

        // pf_listView_Targetの列Idx
        private const int _partitionTargetIdx = 0;
        private const int _partitionMoveSrcIdx = 1;
        private const int _partitionMoveDestIdx = 2;

        // リファレンス名の候補
        public String[] ReferenceCandidateFolders { get; set; }

        private readonly StcFileInputOutput _fio = new StcFileInputOutput();
        // StcBaseForm<SaveRestore>のprotected StcUtils _utilを、FileArranger固有の拡張
        // メソッド(AvoidFolderNameConflict等)を持つUtilsで意図的に隠す。
        // UtilsはStcUtilsを継承しているだけなので、既存の_util.ExecutePath()等の呼び出しは
        // そのまま継承元のメソッドとして動く。
        private new Utils _util = new Utils();
        private readonly StcProcessMemory _renameDirMemory = new StcProcessMemory();    // フォルダ名変更(rdタブ)の復元用
        private readonly FileSorter _sorter = new FileSorter();

        public FileArranger()
        {
            InitializeComponent();
            InitializeCommonSettings(Properties.Resources.FileArranger);

            //ListView初期設定
            RecreateRenameColumnsEvenly();
            RecreatePartitionColumnsEvenly();

            InitializePlaceholders();
            InitializeToolTips();

            _sr.RegisterLoadItem(this);

            // 起動時はJSONを読む。旧XMLしか無ければ読み込んでJSONへ保存し直し、旧XMLは削除する
            // ([[_Common/JsonSaveRestore.cs]])
            String defaultJsonPath = Path.Combine(_userDataFolder, _settingFileNameJson);
            String defaultXmlPath = Path.Combine(_userDataFolder, _settingFileNameXml);
            JsonSaveRestore.LoadWithMigration(_sr, defaultJsonPath, defaultXmlPath, LoadProfileFromXml);

            UpdateProfileListAll("");
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnlyの欄(rd_textBox_ExistItemDir・pf_textBox_ReferenceFile、リファレンスフォルダから自動で入る)と、
        // Multilineの欄(cmn_textBox_AddList、Windowsの仕様で表示されない)、進捗表示のprogressTextは対象外
        private void InitializePlaceholders()
        {
            cmn_textBox_Reference.PlaceholderText = @"例: C:\Work\Reference";
            cmn_textBox_AddListSuffix.PlaceholderText = "例: _new";

            md_textBox_SourceDir.PlaceholderText = @"例: C:\Work\Source";

            rd_textBox_SplitWord3.PlaceholderText = "例: _";
            rd_textBox_AddTitlePreWord.PlaceholderText = "例: vol";
            rd_textBox_SearchTitleLine.PlaceholderText = "例: 3";
            rd_textBox_SearchTitleLength.PlaceholderText = "例: 2";

            sf_textBox_TargetFile.PlaceholderText = @"例: C:\Work\Files";

            mf_textBox_SourceDir.PlaceholderText = @"例: C:\Work\Source";
            mf_textBox_TargetDir.PlaceholderText = @"例: C:\Work\Dest";

            pf_textBox_TargetFile.PlaceholderText = @"例: C:\Work\Files";
            pf_textBox_TargetSeparator.PlaceholderText = "例: _";
            pf_textBox_SearchTitleLine.PlaceholderText = "例: 3";
            pf_textBox_SearchTitleLength.PlaceholderText = "例: 2";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            const String historyTip = "設定値保存の時に、今の値がプルダウンの履歴に追加される。Enterでフォルダを開く";
            const String restoreTip = "押すたびに1回分さかのぼる(アプリを閉じると戻せない)";

            // 共通
            toolTip.SetToolTip(SaveSetting, "現在の設定をファイルに保存する(Ctrl+Sでも可)。プロファイル選択中なら、まずそのファイルへ上書きするか確認する");
            toolTip.SetToolTip(comboBox_LoadSetting, "選ぶとその設定ファイルを読み込む");

            // Common
            toolTip.SetToolTip(cmn_textBox_Reference, "振り分け・リネームの候補になるフォルダが並んでいる親フォルダ。入力するとRenameDir/PartitionFileタブのフォルダ格納先にも同じパスが入る");
            toolTip.SetToolTip(cmn_textBox_AddList, "リファレンスフォルダにまだ無いフォルダ名を1行に1つ書く。リストアップ時に候補へ加える(既存フォルダのパスに含まれる名前は除く)。フォルダはここでは作られない");
            toolTip.SetToolTip(cmn_textBox_AddListSuffix, "新規追加リストの各名前の末尾に付ける文字列。PartitionFileでフォルダを新規作成する時の名前にも付く");
            const String referenceListupTip = "リファレンスフォルダ直下のフォルダと新規追加リストを候補として読み込み、RenameDirの結合文字・PartitionFileの移動後名称のプルダウンを更新する";
            toolTip.SetToolTip(cmn_button_Listup, referenceListupTip);
            toolTip.SetToolTip(rd_button_Listup_Item, referenceListupTip);
            toolTip.SetToolTip(pf_button_Listup_Reference, referenceListupTip);

            // MoveDir
            toolTip.SetToolTip(md_listBox_Listup, "格納元以下(サブフォルダも含む)で、直下にファイルがあるフォルダの一覧。Ctrl+Aで全選択、Enterでサブディレクトリを移動、ダブルクリックでフォルダを開く");
            toolTip.SetToolTip(md_comboBox_TargetDir, "移動先のフォルダ。" + historyTip);
            toolTip.SetToolTip(md_button_MoveTopDir, "選択したフォルダを、格納元直下の最上位フォルダごと格納先へ移動する。格納先に同名フォルダがあれば、名前の末尾に「_Cnt番号_日時」を付ける");
            toolTip.SetToolTip(md_button_MoveSubDir, "選択したフォルダそのものを格納先の直下へ移動する(途中の階層は持っていかない)。格納先に同名フォルダがあれば、名前の末尾に「_Cnt番号_日時」を付ける");
            toolTip.SetToolTip(md_button_Delete, "選択したフォルダを中身ごと削除する。確認は出ず、ごみ箱にも入らない");

            // RenameDir
            toolTip.SetToolTip(rd_comboBox_RenameDir, "リネームするフォルダが並んでいる親フォルダ(直下のフォルダが対象)。" + historyTip);
            toolTip.SetToolTip(rd_textBox_ExistItemDir, "Commonのリファレンスフォルダと連動する欄。結合文字の候補は、候補フォルダのパスからこの部分を除いた名前になる。左のラベルをダブルクリックすると直接編集できる。Enterでフォルダを開く");
            toolTip.SetToolTip(rd_label_ExistItemDir, "ダブルクリックで右の欄の直接編集を切り替える");
            toolTip.SetToolTip(rd_listView_Target, "選択したフォルダの変更後の名前が[変更後]に表示される。Ctrl+Enterでリネーム実行、Ctrl+Cでフォルダ名をコピー、ダブルクリックでフォルダを開く");
            toolTip.SetToolTip(rd_comboBox_MergeWord, "変更後の名前の先頭部分。プルダウンには、リファレンスのフォルダ名(最後の区切り文字より前)のうち入力中の文字を含むものが出る");
            toolTip.SetToolTip(rd_textBox_SplitWord3, "結合文字の候補を作る時、リファレンスのフォルダ名をこの文字の最後の出現位置で切る");
            toolTip.SetToolTip(rd_textBox_SearchTitleLine, "元のフォルダ名から番号を探し始める位置を、名前の末尾から数えた文字数で指定する。空欄なら先頭から(全角数字は半角とみなす)");
            toolTip.SetToolTip(rd_textBox_SearchTitleLength, "探し始めた位置から何文字を番号として読むか(中の数字だけを使う)。空欄なら末尾まで。数字が無ければ1になる");
            toolTip.SetToolTip(rd_comboBox_AddTitlePostWord, "番号の後ろに付ける文字列。設定値保存の時に、今の値がプルダウンの履歴に追加される");
            toolTip.SetToolTip(rd_checkBox_FileOpen, "オンの時、一覧をダブルクリックするとフォルダではなく、中の先頭ファイル(サブフォルダも含めて最初に見つかったもの)を開く");
            toolTip.SetToolTip(rd_button_RenameDir, "選択したフォルダを「結合文字+番号前に追加+番号(2桁以上にゼロ埋め)+番号後に追加」にリネームする。同名があれば末尾に「_Cnt番号_日時」を付ける。各入力欄でCtrl+Enterでも実行");
            toolTip.SetToolTip(rd_button_RenameDirRestore, "直前のリネームを元に戻す。" + restoreTip);

            // SortFileName
            toolTip.SetToolTip(sf_listBox_Target, "格納元直下のフォルダの一覧。Ctrl+Aで全選択、Enterでソート実行");
            toolTip.SetToolTip(sf_button_Sort, "選択したフォルダ内のファイル(サブフォルダ内は対象外)を、000、001…の3桁の連番名にリネームする(拡張子はそのまま)");
            toolTip.SetToolTip(sf_button_SortRestore, "直前のソートを元の名前に戻す。" + restoreTip);

            // PartitionFile
            toolTip.SetToolTip(pf_textBox_ReferenceFile, "振り分け先のフォルダが並んでいる親フォルダ(Commonのリファレンスフォルダと連動)。左のラベルをダブルクリックすると直接編集できる。Enterでフォルダを開く");
            toolTip.SetToolTip(pf_label_ReferenceFile, "ダブルクリックで右の欄の直接編集を切り替える");
            toolTip.SetToolTip(pf_textBox_TargetSeparator, "ファイル名をこの文字の最後の出現位置で切り、その前の部分を名前に含むフォルダを振り分け先の候補から探す");
            toolTip.SetToolTip(pf_textBox_SearchTitleLine, "振り分け先のフォルダ名から番号を探し始める位置を、名前の末尾から数えた文字数で指定する。空欄なら先頭から。Ctrl+Enterでファイル移動");
            toolTip.SetToolTip(pf_textBox_SearchTitleLength, "探し始めた位置から何文字を番号として読むか(中の数字だけを使う)。空欄なら末尾まで。Ctrl+Enterでファイル移動");
            toolTip.SetToolTip(pf_checkBox_CreateNewDir, "オンの時、候補に一致するフォルダが無いファイルには、ファイル名(区切り文字より前)+新規追加Suffixを元にした新しいフォルダ名を移動後名称に入れる。切り替えると一覧を読み直す");
            toolTip.SetToolTip(pf_listView_Target, "選択したファイルに移動前/移動後名称が自動で入る(移動後はフォルダ名の番号に、同じ名前で選択中のファイル数を足したもの)。Enterでファイル移動、Deleteで移動後名称を空に、ダブルクリックで移動前フォルダを開く");
            toolTip.SetToolTip(pf_comboBox_MoveDestDirName, "キー入力すると、選択中の全行の移動後名称をこの値にする。Ctrl+Enterでファイル移動");
            toolTip.SetToolTip(pf_button_ClearSelect, "全行の移動前/移動後名称を空にする(一覧の選択状態はそのまま)");
            toolTip.SetToolTip(pf_button_CreateFolderAndMoveFile, "選択した行のうち移動後名称がある行について、移動前名称のフォルダを移動後名称にリネーム(無ければ新規作成)してから、ファイルをそこへ移動する。同名ファイルがあればスキップする");
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
        private Boolean LoadProfile(String filePath)
        {
            if (IsJsonFile(filePath))
            {
                return _sr.LoadJsonFile(filePath, this);
            }

            return _sr.LoadProc(filePath, this);
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
                return _sr.SaveJsonFile(filePath, this);
            }

            return _sr.SaveSetting(filePath, this);
        }

        // comboBox_LoadSettingへ、userDataFolder配下の*.xmlと*.jsonの両方をまとめてリストアップする。
        // _util.UpdateProfileListは拡張子を1パターンしか指定できないため、2回検索した結果をマージする
        private void UpdateProfileListAll(String defaultProfileName)
        {
            String[] xmlFiles = Directory.GetFiles(_userDataFolder, "*.xml", SearchOption.AllDirectories);
            String[] jsonFiles = Directory.GetFiles(_userDataFolder, "*.json", SearchOption.AllDirectories);
            String[] files = xmlFiles.Concat(jsonFiles).ToArray();

            _util.SetComboBoxFromArray(comboBox_LoadSetting, files, _userDataFolder);
            _util.SetComboBoxText(comboBox_LoadSetting, defaultProfileName);
        }

        private void cmn_textBox_AddList_KeyDown(object sender, KeyEventArgs e)
        {
            _util.SelectAll(e);
        }

        // リストアップ前のフォルダ確認。各タブで同じ確認をしていたためまとめた
        // (showErrorPopup=falseなら、無効でもメッセージを出さずに中断する)
        private static Boolean IsValidFolderPath(String folderPath, Boolean showErrorPopup = true)
        {
            if (Directory.Exists(folderPath))
            {
                return true;
            }

            if (showErrorPopup)
            {
                MessageBox.Show("フォルダパスが不正です。" + folderPath);
            }
            return false;
        }

        // フルパスから基準フォルダの分を取り除いて、表示用の名前にする
        // (区切り文字の1文字分を足す処理が各タブに散らばっていたためまとめた)
        private static String GetDisplayName(String fullPath, String baseFolderPath)
        {
            return fullPath.Remove(0, baseFolderPath.Length + 1);
        }

        // パス一覧を表示用の名前にしてListBoxへ並べ直す(各タブのリストアップで共通)
        private static void FillListBox(ListBox listBox, String[] paths, String baseFolderPath)
        {
            listBox.BeginUpdate();
            listBox.Items.Clear();
            listBox.Items.AddRange(paths.Select(path => (object)GetDisplayName(path, baseFolderPath)).ToArray());
            listBox.EndUpdate();
        }

        // パス一覧を表示用の名前にしてListViewへ並べ直す。1列目に名前を入れ、残りの列は空欄にする
        private static void FillListView(ListView listView, String[] paths, String baseFolderPath, int columnCount)
        {
            listView.BeginUpdate();
            listView.Items.Clear();
            foreach (String path in paths)
            {
                String[] item = new String[columnCount];
                item[0] = GetDisplayName(path, baseFolderPath);
                for (int i = 1; i < columnCount; i++)
                {
                    item[i] = "";
                }
                listView.Items.Add(new ListViewItem(item));
            }
            listView.EndUpdate();
        }

        // ListViewの列を作り直し、幅を均等に割り振る
        private static void RecreateColumnsEvenly(ListView listView, String[] columnNames)
        {
            listView.Columns.Clear();

            // ListViewコントロールのプロパティを設定
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.Sorting = SortOrder.Ascending;
            listView.View = View.Details;

            // 列（コラム）ヘッダの作成
            int columnWidth = listView.Width / columnNames.Length;
            foreach (String columnName in columnNames)
            {
                listView.Columns.Add(new ColumnHeader { Text = columnName, Width = columnWidth });
            }
        }

        // 選択項目が無ければメッセージを出してfalseを返す(各タブの実行ボタンで共通)
        private static Boolean HasSelectedItems(int selectedCount)
        {
            if (selectedCount > 0)
            {
                return true;
            }

            MessageBox.Show("項目が選択されていません。");
            return false;
        }

        private static String FormatSelectedCount(int selectedCount)
        {
            return "選択数：" + selectedCount.ToString();
        }

        // BackgroundWorker実行前の進捗バー初期化(mf/pfタブで共通)
        private void ResetProgressBar(int maximum)
        {
            progressBar.Maximum = maximum;
            progressBar.Minimum = 0;
            progressBar.Value = 0;
        }

        // BackgroundWorkerの進捗表示(mf/pfタブで共通)
        private void ShowProgress(int doneCount)
        {
            progressText.Text = doneCount + "/" + progressBar.Maximum + " 完了";
            progressBar.Value = doneCount;
        }

        private void SaveSetting_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(_util, _fio, comboBox_LoadSetting, _profileExtensions, SaveProfile, _userDataFolder);
        }

        // Ctrl+Sで「設定値保存」ボタンと同じ動作にする(テキストボックス等にフォーカスがあっても拾える)
        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                SaveSetting_Click(this, EventArgs.Empty);
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

        private void comboBox_LoadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            String loadFileName = Path.Combine(_userDataFolder, comboBox_LoadSetting.Text);
            LoadProfile(loadFileName);
        }

        private void FileArranger_ResizeEnd(object sender, EventArgs e)
        {
            rd_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            RecreatePartitionColumnsEvenly();
        }

        private void cmn_textBox_Reference_TextChanged(object sender, EventArgs e)
        {
            rd_textBox_ExistItemDir.Text = cmn_textBox_Reference.Text;
            pf_textBox_ReferenceFile.Text = cmn_textBox_Reference.Text;
        }

        private void cmn_button_Listup_Click(object sender, EventArgs e)
        {
            if (!IsValidFolderPath(cmn_textBox_Reference.Text))
            {
                return;
            }

            // フォルダをリストアップ
            ReferenceCandidateFolders = Directory.GetDirectories(cmn_textBox_Reference.Text);

            // 新規追加
            if (cmn_textBox_AddList.Text != String.Empty)
            {
                String[] addReferenceList = cmn_textBox_AddList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                Logic.DeleteDuplicate(ReferenceCandidateFolders, ref addReferenceList, rd_textBox_SplitWord3.Text);
                addReferenceList = addReferenceList.Select(str => cmn_textBox_Reference.Text + @"\" + str + cmn_textBox_AddListSuffix.Text).ToArray();

                ReferenceCandidateFolders = ReferenceCandidateFolders.Concat(addReferenceList).ToArray();
            }

            // コンボボックス更新
            UpdateRenameComboBox();
            UpdateMoveDestDirComboBox();
        }

    }
}
