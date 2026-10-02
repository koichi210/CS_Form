using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace FFEdit
{
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private readonly String SettingFileName = @"FFEdit.json";

        // 設定ファイルはJSONが基本。旧XML(FFEdit.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])
        private const String LegacySettingFileName = @"FFEdit.xml";

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // internal: テストから保存ボタンと同じ経路で保存を確かめるため
        internal Boolean SaveProfile(String filePath)
        {
            if (!IsJsonFile(filePath))
            {
                return sr.SaveSetting(filePath, this);
            }

            sr.UpdateComboHistory(this);
            return JsonSaveRestore.Save(sr, filePath);
        }

        private readonly String[] TimeSpanItems = { "無し", "秒", "分", "時間", "日" };
        private readonly String[] DigitItems = { "自動", "1桁", "2桁", "3桁", "4桁", "5桁", "6桁" };
        private const int TabIdxChangeName = 0;
        private const int TabIdxTimeStamp = 1;
        private const int TabIdxFunction = 2;

        private Rename rename = new Rename();
        private Function function = new Function();

        // 設定ファイル(FFEdit.xml)の置き場。exe直下(bin/Debug、bin/Release)は
        // ビルド出力の掃除等で丸ごと消される事故が起きうるため、そこには置かない。
        // 実データは%LOCALAPPDATA%\FFEdit\配下(既定)にあり、exe直下にはその場所を示す
        // 小さな案内板ファイル(DataFolder.txt)だけを置く2段構成にしてある
        // ([[_Common/UserDataLocation.cs]]、EventRecorderと同じ仕組み)
        private const String AppName = "FFEdit";
        private readonly String userDataFolder = StandardTemplate.UserDataLocation.GetUserDataFolder(AppName);

        public Form1()
        {
            InitializeComponent();
            InitializeCommonSettings(Properties.Resources.FFEdit);

            InitializePlaceholders();
            InitializeToolTips();

            sr.RegisterItem(this);
            JsonSaveRestore.LoadWithMigration(sr,
                Path.Combine(userDataFolder, SettingFileName),
                Path.Combine(userDataFolder, LegacySettingFileName),
                path => sr.LoadProc(path, this));

            // 桁の選択肢を生成
            comboBox_ChangeNumber_Digit.Items.Clear();
            for (int i = 0; i < DigitItems.Length; i++)
            {
                comboBox_ChangeNumber_Digit.Items.Add(DigitItems[i]);
            }
            comboBox_ChangeNumber_Digit.SelectedIndex = 0;

            // 加算間隔の選択肢を生成
            comboBox_TimeSpan.Items.Clear();
            for (int i = 0; i < TimeSpanItems.Length; i++)
            {
                comboBox_TimeSpan.Items.Add(TimeSpanItems[i]);
            }
            comboBox_TimeSpan.SelectedIndex = 1;
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // ReadOnlyの欄(textBox_StatusBar、総数/選択数の表示欄)と、
        // エラー画面(ErrorMsg)のMultiline+ReadOnlyのメッセージ欄は対象外
        private void InitializePlaceholders()
        {
            textBox_Target_Extension.PlaceholderText = "例: *.jpg";
            textBox_ChangeNumber_FirstVal.PlaceholderText = "例: 1";
            textBox_Function_Any_Directory.PlaceholderText = @"例: C:\Work";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(comboBox_TargetDir, "処理対象のフォルダ。Enterキーでこのフォルダを開く");
            toolTip.SetToolTip(textBox_Target_Extension, "一覧に出す名前の絞り込み(ワイルドカード可)。空欄なら全件。Enterキーで一覧を更新する");
            toolTip.SetToolTip(checkBox_Target_SelectFile, "オンなら一覧で選択した項目だけを処理する。オフなら一覧の全項目が対象");
            toolTip.SetToolTip(listBox, "Ctrl+A: 全選択、Ctrl+C: 表示名をコピー、Ctrl+Shift+C: フルパスでコピー、Delete: 選択項目を削除(ごみ箱を経由せず元に戻せない)");
            toolTip.SetToolTip(button_Execute, "表示中のタブ(名称変換/タイムスタンプ/機能)の処理を対象項目に実行する");
            toolTip.SetToolTip(button_Restore, "名称変換・移動の直前の実行を1回分ずつ元に戻す。タイムスタンプ変更・コピー・空フォルダ削除は戻せない。履歴はアプリ終了で消える");
            toolTip.SetToolTip(button_SaveSetting, "現在の入力内容を設定ファイルに保存する(Ctrl+Sでも可)");

            toolTip.SetToolTip(radioButton_ChangeNumber, "名前を「連番+拡張子」に変える。番号は一覧の並び順で振る");
            toolTip.SetToolTip(comboBox_ChangeNumber_Digit, "連番を0埋めする桁数。自動は対象件数の桁数に合わせる");
            toolTip.SetToolTip(checkBox_ChangeNumber_OrgName, "連番の後ろに元の名前(拡張子を除く)を続ける");
            toolTip.SetToolTip(radioButton_ChangeDelNum, "名前の先頭から、または拡張子の手前から、指定した文字数を削除する");
            toolTip.SetToolTip(radioButton_ChangeAdd, "先頭欄の文字を名前の頭に、後方欄の文字を拡張子の手前に追加する");
            toolTip.SetToolTip(radioButton_ChangeDelete, "指定した文字列を名前から全て取り除く。拡張子部分も対象になる");
            toolTip.SetToolTip(radioButton_ChangeReplace, "置換前の文字列を全て置換後の文字列に置き換える。拡張子部分も対象になる");
            toolTip.SetToolTip(radioButton_ChangeExt, "拡張子を指定した文字に変える。「.」は付けずに入力する");
            toolTip.SetToolTip(radioButton_ChangeAddDirName, "対象フォルダからの相対パスの「\\」を「_」に置き換えた名前にする(例: sub\\a.txt → sub_a.txt)。置き場所は変わらない");

            toolTip.SetToolTip(dateTimePicker_Days, "1件目に設定する日付");
            toolTip.SetToolTip(dateTimePicker_Time, "1件目に設定する時刻");
            toolTip.SetToolTip(comboBox_TimeSpan, "2件目以降は1件ごとにこの単位で1ずつ日時を進める。無しなら全件同じ日時");

            toolTip.SetToolTip(radioButton_Copy_Target, "対象ファイルをコピーする。コピー先に同名ファイルがあると失敗する。フォルダのコピーは未対応");
            toolTip.SetToolTip(radioButton_Move_Target, "対象を移動する。サブフォルダ内の項目も移動先の直下にまとめて置く");
            toolTip.SetToolTip(radioButton_Delete_BlankDir, "一覧の各フォルダの配下にある空のサブフォルダを削除する。元に戻せない");
            toolTip.SetToolTip(checkBox_Operation_AnyDir, "オンなら移動/コピー先を入力欄のフォルダにする。オフなら対象フォルダ直下");
            toolTip.SetToolTip(textBox_Function_Any_Directory, "移動/コピー先のフォルダ。存在しなければ作成する");
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
            else if (e.KeyCode == Keys.C && e.Control == true && e.Shift == true)
            {
                util.CopyToClipboard(e, listBox, comboBox_TargetDir.Text);
            }
            else if (e.KeyCode == Keys.C && e.Control == true)
            {
                util.CopyToClipboard(e, listBox);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DialogResult dlgResult = MessageBox.Show(
                    "削除しますか？",
                    "情報",
                    MessageBoxButtons.YesNo);
                if (dlgResult == DialogResult.No)
                {
                    return ;
                }

                String targetName = util.GetSelectName(listBox, comboBox_TargetDir.Text);

                StcFileInputOutput fio = new StcFileInputOutput();
                String[] targetArray = targetName.Split(new[] {Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
                for ( int i= 0; i < targetArray.Length; i++)
                {
                    fio.DeleteDirectoryAndFile(targetArray[i]);
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
            String errorList = "";
            switch (tabControl.SelectedIndex)
            {
                case TabIdxChangeName:
                    errorList = ChangeName();
                    break;

                case TabIdxTimeStamp:
                    ChangeTimeStamp();
                    break;

                case TabIdxFunction:
                    errorList = ChangeOtherFunction();
                    break;
            }

            if (errorList != String.Empty)
            {
                using (ErrorMsg dlg = new ErrorMsg(errorList))
                {
                    dlg.ShowDialog();
                }
            }
            UpdateListBox();
        }

        private void button_Restore_Click(object sender, EventArgs e)
        {
            Boolean isSuccess = true;
            switch (tabControl.SelectedIndex)
            {
                case TabIdxChangeName:
                    isSuccess = rename.Restore();
                    break;
                case TabIdxFunction:
                    isSuccess = function.Restore();
                    break;
            }

            if (!isSuccess)
            {
                MessageBox.Show("これ以上復元できません");
                return;
            }
            UpdateListBox();
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            String saveFilePath = Path.Combine(userDataFolder, SettingFileName);
            if (!SaveProfile(saveFilePath))
            {
                MessageBox.Show("設定の保存に失敗しました" + Environment.NewLine + saveFilePath,
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + saveFilePath);
        }

        // Ctrl+Sで「設定保存」ボタンと同じ動作にする(テキストボックス等にフォーカスがあっても拾える)
        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                button_SaveSetting_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // *******************************************************************************
        // データ保存先フォルダの変更(システムメニューから呼び出す)
        // ([[EventRecorder/Form1.cs]]の同名機能と同じ考え方。FFEditは設定ファイルが
        // FFEdit.xml1つだけなので、複数プロファイルの引っ越し処理までは不要)

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DataFolderMenu.AppendToSystemMenu(this);
        }

        protected override void WndProc(ref Message m)
        {
            if (DataFolderMenu.IsChangeDataFolderCommand(m))
            {
                DataFolderMenu.ChangeDataFolder(AppName, userDataFolder, MoveSettingFile);
                return;
            }

            base.WndProc(ref m);
        }

        // FFEditは設定ファイルが1つだけなので、他プロジェクトのような全プロファイルの引っ越しではなく
        // この1ファイルだけを移す
        private void MoveSettingFile(String oldFolder, String newFolder)
        {
            String oldSettingPath = Path.Combine(oldFolder, SettingFileName);
            String newSettingPath = Path.Combine(newFolder, SettingFileName);

            if (!File.Exists(oldSettingPath) || File.Exists(newSettingPath))
            {
                return;
            }

            DialogResult moveResult = MessageBox.Show(
                "既存の設定ファイルを新しい保存先に移動しますか？" + Environment.NewLine + Environment.NewLine
                    + "移動元: " + oldSettingPath + Environment.NewLine
                    + "移動先: " + newSettingPath,
                AppName + " - 設定ファイルの引っ越し",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (moveResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                File.Move(oldSettingPath, newSettingPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "移動に失敗したよ: " + ex.Message,
                    AppName + " - 設定ファイルの引っ越し",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
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

            int trimLength; // ファイルリストを生成するときに、基準となるディレクトリパスは削除する
            if (comboBox_TargetDir.Text.Length > 3)
            {
                // C:\ よりも長い場合は、終端の\を削除。ドライブレターの次の\は残す
                comboBox_TargetDir.Text = comboBox_TargetDir.Text.TrimEnd('\\');

                // 「+1」はフォルダ区切り文字
                trimLength = comboBox_TargetDir.Text.Length + 1;
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
                trimLength = comboBox_TargetDir.Text.Length;
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

            String searchPattern = "*";
            if (textBox_Target_Extension.Text != String.Empty)
            {
                searchPattern = textBox_Target_Extension.Text;
            }

            String[] elements;
            if (radioButton_Target_File.Checked)
            {
                elements = Directory.GetFiles(comboBox_TargetDir.Text, searchPattern, opt);
            }
            else
            {
                elements = Directory.GetDirectories(comboBox_TargetDir.Text, searchPattern, opt);
            }

            // 標準のstring比較だと"HOGE_2"より"HOGE_10"が先に来てしまうため、
            // 数字部分を数値として比較する自然順ソート(NaturalStringComparer)で並べ替える
            Array.Sort(elements, StandardTemplate.NaturalStringComparer.Instance);

            listBox.Items.Clear();
            for (int i = 0; i < elements.Length; i++)
            {
                listBox.Items.Add(elements[i].Substring(trimLength));
            }
            UpdateStatusBar();
        }

        private List<String> GetFileList()
        {
            var fileList = new List<String>();

            int targetCount = GetTargetCount();
            for (int i = 0; i < targetCount; i++)
            {
                fileList.Add(GetTargetName(i));
            }
            return fileList;
        }

        private String GetTargetName(int idx)
        {
            if (checkBox_Target_SelectFile.Checked)
            {
                return listBox.SelectedItems[idx].ToString();
            }
            return listBox.Items[idx].ToString();
        }

        private String ChangeName()
        {
            rename.BaseDir = comboBox_TargetDir.Text.TrimEnd('\\');
            rename.FileList = GetFileList();
            rename.Type = GetChangeType();

            rename.Param1 = comboBox_String1.Text;
            rename.Param2 = comboBox_String2.Text;

            // 連番モード以外ではtextBox_ChangeNumber_FirstValは空欄のままなので、
            // 他のモード用ガード(isChangeNumber等)と同じ考え方でモード判定してからParseする
            rename.FirstNumber = radioButton_ChangeNumber.Checked
                ? int.Parse(textBox_ChangeNumber_FirstVal.Text)
                : 0;
            rename.KeepOriginalName = checkBox_ChangeNumber_OrgName.Checked;
            rename.PadDigits = GetPadDigits();

            return rename.Execute();
        }

        private void ChangeTimeStamp()
        {
            var ts = new TimeStamp();

            DateTime dt = new DateTime(
                dateTimePicker_Days.Value.Year,
                dateTimePicker_Days.Value.Month,
                dateTimePicker_Days.Value.Day,
                dateTimePicker_Time.Value.Hour,
                dateTimePicker_Time.Value.Minute,
                dateTimePicker_Time.Value.Second,
                0);
            ts.BaseTicks = dt.Ticks;
            ts.IntervalTicks = GetIntervalTicks(comboBox_TimeSpan.SelectedIndex);

            ts.BaseDir = comboBox_TargetDir.Text.TrimEnd('\\');
            ts.FileList = GetFileList();

            ts.UpdateCreationTime = checkBox_CreationTime.Checked;
            ts.UpdateLastWriteTime = checkBox_LastWriteTime.Checked;
            ts.UpdateLastAccessTime = checkBox_LastAccessTime.Checked;

            ts.Execute();
        }

        private int GetPadDigits()
        {
            if (comboBox_ChangeNumber_Digit.SelectedIndex > 0)
            {
                // 自動桁数じゃない場合
                return comboBox_ChangeNumber_Digit.SelectedIndex;
            }

            // [自動桁数]の場合
            return GetTargetCount().ToString().Length;
        }

        private long GetIntervalTicks(int timeSpanIdx)
        {
            // 加算時間
            switch (timeSpanIdx)
            {
                case 1:     // [秒]
                    return TimeSpan.TicksPerSecond;

                case 2:     // [分]
                    return TimeSpan.TicksPerMinute;

                case 3:     // [時間]
                    return TimeSpan.TicksPerHour;

                case 4:     // [日]
                    return TimeSpan.TicksPerDay;

                case 0:     // [無し]
                default:
                    return 0;
            }
        }

        private Rename.ChangeType GetChangeType()
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
            // radioButton_ChangeAddDirName
            return Rename.ChangeType.AddDirName;
        }

        private void SetNameChangeControlLabel()
        {
            String label1 = "";
            String label2 = "";

            // 状態取得(連番・フォルダ名付与のときはラベル無し)
            if (radioButton_ChangeDelNum.Checked)
            {
                label1 = "先頭から";
                label2 = "後方から";
            }
            else if (radioButton_ChangeAdd.Checked)
            {
                label1 = "先頭に追加";
                label2 = "後方に追加";
            }
            else if (radioButton_ChangeDelete.Checked)
            {
                label1 = "削除文字";
            }
            else if (radioButton_ChangeReplace.Checked)
            {
                label1 = "置換前";
                label2 = "置換後";
            }
            else if (radioButton_ChangeExt.Checked)
            {
                label1 = "拡張子";
            }

            label_String1.Text = label1;
            label_String2.Text = label2;
        }

        private void UpdateNameChangeControl(object sender, EventArgs e)
        {
            bool isChangeNumber = radioButton_ChangeNumber.Checked;
            SetNameChangeControlLabel();

            // TextBoxの表示
            comboBox_String1.Enabled = (label_String1.Text != String.Empty);
            comboBox_String2.Enabled = (label_String2.Text != String.Empty);

            // ChangeNumber用の設定
            label_ChangeNumber_FirstVal.Enabled = isChangeNumber;
            textBox_ChangeNumber_FirstVal.Enabled = isChangeNumber;

            comboBox_ChangeNumber_Digit.Enabled = isChangeNumber;
            checkBox_ChangeNumber_OrgName.Enabled = isChangeNumber;
        }

        private void UpdateFunctionControl(object sender, EventArgs e)
        {
            Boolean isOperationEnabled = !radioButton_Delete_BlankDir.Checked;
            checkBox_Operation_AnyDir.Enabled = isOperationEnabled;

            textBox_Function_Any_Directory.Enabled = isOperationEnabled && checkBox_Operation_AnyDir.Checked;
        }

        private String GetDestDirOtherFunction()
        {
            if (!radioButton_Delete_BlankDir.Checked &&
                checkBox_Operation_AnyDir.Checked)
            {
                return textBox_Function_Any_Directory.Text;
            }

            return comboBox_TargetDir.Text;
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
            // radioButton_Copy_Target
            return Function.FunctionType.Copy;
        }

        private String ChangeOtherFunction()
        {
            function.BaseDir = comboBox_TargetDir.Text.TrimEnd('\\');
            function.DestDir = GetDestDirOtherFunction().TrimEnd('\\');

            function.FileList = GetFileList();
            function.Type = GetFunctionType();

            return function.Execute();
        }

        private int GetTargetCount()
        {
            if (checkBox_Target_SelectFile.Checked)
            {
                return listBox.SelectedItems.Count;
            }
            return listBox.Items.Count;
        }
    }
}
