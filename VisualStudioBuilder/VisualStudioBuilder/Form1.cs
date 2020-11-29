﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace VisualStudioBuilder
{
    public partial class Form1 : Form
    {
        private readonly String DefaultSettingFileName = "VisualStudioBuilder.xml";

        private readonly String StrDataGridBuildListEnable = "○";
        private readonly String StrDataGridBuildListDisable = "×";

        private readonly String StrDataGridHeaderBuild = "ビルド";
        private readonly String StrDataGridHeaderSolution = "ソリューションファイル";
        private readonly String StrDataGridHeaderProjectPath = "プロジェクトフルパス";

        private readonly String ExtSln = ".sln";
        private readonly String ExtLog = ".log";

        private const int BuildEnableIdx = 0;
        private const int SolutionNameIdx = 1;
        private const int ProjectPathIdx = 2;

        private StcFileInputOutput fio = new StcFileInputOutput();
        private SaveRestore sr = new SaveRestore();
        private StcUtils util = new StcUtils();

        public class BuildWorkerInfo
        {
            public String BuildScript;
            public Boolean IsDetectError;
            public String DetectBuildErrorWord;
            public Boolean IsExclude;
            public String IgnoreExecuteFile;
            public Boolean IsDeleteDirectory;
            public String DeleteDirectoryName;
            public String DetectTargetLogList;
        }

        public Form1()
        {
            InitializeComponent();

            // アイコン設定
            this.Icon = Properties.Resources.VisualStudioBuilder; 

            // カレントディレクトリ移動
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            // DataGridViewの初期設定
            InitializeDataGridView();

            sr.RegistItem(this);
            sr.LoadProc(DefaultSettingFileName, this);
            UpdateBuildGUI();
            UpdateOutputGUI();
            util.UpdateProfileList(ref comboBox_Profile, DefaultSettingFileName);

        }

        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + comboBox_Profile.Text;
            sr.LoadProc(LoadFileName, this);
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(comboBox_Profile.Text);
            if (SaveFileName != String.Empty)
            {
                if (sr.SaveXmlFile(SaveFileName))
                {
                    util.UpdateProfileList(ref comboBox_Profile, Path.GetFileName(SaveFileName));
                    MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
                }
            }
        }

        private void button_RemoveRaw_Click(object sender, EventArgs e)
        {
            if (dataGridView.RowCount > 1)
            {
                dataGridView.Rows.RemoveAt(dataGridView.CurrentRow.Index);
            }
        }

        private String CreateDetectTargetList()
        {
            String DetectTargetLogList = "";

            if (checkBox_DetectBuildError.Checked)
            {
                for (int i = 0; i < dataGridView.RowCount; i++)
                {
                    if (GetCellData(i, BuildEnableIdx) != StrDataGridBuildListEnable)
                    {
                        continue;
                    }

                    String SolutionName = GetCellData(i, SolutionNameIdx);
                    DetectTargetLogList += GetLogPathName(textBox_LogDirectory.Text, SolutionName);
                    DetectTargetLogList += Environment.NewLine;
                }
            }

            return util.TrimEndGarbage(DetectTargetLogList);
        }

        private Boolean CheckSolutionPath()
        {
            Boolean IsSuccess = true;

            String FileNotFoundList = "";

            // ファイルパスチェック
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                String SolutionName = GetCellData(i, SolutionNameIdx);
                String ProjectPath = GetCellData(i, ProjectPathIdx);
                String SolutionPath = GetSolutionPathName(ProjectPath, SolutionName);

                if (!File.Exists(SolutionPath))
                {
                    // 存在しないパス
                    FileNotFoundList += "No" + i.ToString() + " " + SolutionName + Environment.NewLine;
                }
            }
            
            if (FileNotFoundList != String.Empty)
            {
                DialogResult DlgResult = MessageBox.Show(
                    "いくつかのソリューションが見つかりませんでした。" +
                    "処理を継続しますか？" + Environment.NewLine + FileNotFoundList,
                    "Warning",
                    MessageBoxButtons.YesNo);
                if (DlgResult == DialogResult.No)
                {
                    IsSuccess = false;
                }
            }
            return IsSuccess;
        }

        private void button_Build_Click(object sender, EventArgs e)
        {
            if (!CheckSolutionPath())
            {
                return;
            }
            
            // ビルドログを削除
            StcFileInputOutput fio = new StcFileInputOutput();
            fio.DeleteDirectoryAndFile(textBox_LogDirectory.Text);

            List<object> arguments = new List<object>();

            // Build実行Script生成
            String BuildScript = CreateAllBuildScript();
            arguments.Add(BuildScript);

            // エラー検知
            arguments.Add(checkBox_DetectBuildError.Checked);
            arguments.Add(textBox_DetectBuildErrorWord.Text);

            // エラー検知除外
            arguments.Add(checkBox_IsExclude.Checked);
            arguments.Add(textBox_ExcludeWord.Text);

            // ビルド後の削除ディレクトリ
            arguments.Add(checkBox_DeleteDirectory.Checked);
            arguments.Add(textBox_DeleteDirectoryName.Text);

            // ビルドログリスト生成
            String DetectTargetLogList = CreateDetectTargetList();
            arguments.Add(DetectTargetLogList);

            BuildWorker.RunWorkerAsync(arguments);
        }

        private void BuildWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;
            BuildWorkerInfo bwi = new BuildWorkerInfo();
            bwi.BuildScript = (String)genericlist[0];           // BuildScript
            bwi.IsDetectError = (Boolean)genericlist[1];        // checkBox_DetectBuildError
            bwi.DetectBuildErrorWord = (String)genericlist[2];  // DetectBuildErrorWord
            bwi.IsExclude = (Boolean)genericlist[3];            // checkBox_IsExclude
            bwi.IgnoreExecuteFile = (String)genericlist[4];     // textBox_ExcludeWord
            bwi.IsDeleteDirectory = (Boolean)genericlist[5];    // checkBox_DeleteDirectory
            bwi.DeleteDirectoryName = (String)genericlist[6];   // textBox_DeleteDirectoryName
            bwi.DetectTargetLogList = (String)genericlist[7];   // DetectTargetLogList

            // ビルド実行
            String BatchFile = fio.CreateTempFile("Bat");
            fio.CreateFile(BatchFile, bwi.BuildScript);
            util.ExecutePathWithWait(BatchFile);

            // TODO：ビルドリストはバッチで生成しているので、ここでは見えない。
            //       バッチ生成もタスクで実装する？
            // ディレクトリ削除
            //if (bwi.IsDeleteDirectory)
            //{
                //for (int i = 0; i < ファイルリスト; i++)
                //{
                    //StcFileInputOutput fio = new StcFileInputOutput();
                    //fio.DeleteDirectoryAndFile(ファイルリスト + bwi.DeleteDirectoryName);
                //}
            //}

            // ビルドエラー検出
            String SuccessList = "[ビルド成功]" + Environment.NewLine;
            String ErrorList = "[ビルド失敗]" + Environment.NewLine;
            String ExcludeList = "[実行ファイルの上書きに失敗]" + Environment.NewLine;
            if (bwi.IsDetectError)
            {
                String[] TargetArray = util.ChangeStrLinear2Array(bwi.DetectTargetLogList, Environment.NewLine);
                for (int i = 0; i < TargetArray.Length; i++)
                {
                    Boolean IsSuccess = true;
                    if (fio.DetectFileData(TargetArray[i], bwi.DetectBuildErrorWord))
                    {
                        // ビルド失敗
                        ErrorList += TargetArray[i] + Environment.NewLine;
                        IsSuccess = false;
                    }

                    if (bwi.IsExclude && (fio.DetectFileData(TargetArray[i], bwi.IgnoreExecuteFile)))
                    {
                        // 上書き不可
                        ExcludeList += TargetArray[i] + Environment.NewLine;
                        IsSuccess = false;
                    }

                    if (IsSuccess)
                    {
                        // ビルド成功
                        SuccessList += TargetArray[i] + Environment.NewLine;
                    }
                }

                e.Result = SuccessList + Environment.NewLine;
                if (bwi.IsExclude)
                {
                    e.Result += ExcludeList + Environment.NewLine;
                }
                e.Result += ErrorList + Environment.NewLine;
            }
            else
            {
                e.Result = "ビルド完了";
            }
        }

        private void BuildWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // この場合にはe.Resultにはアクセスできない
                MessageBox.Show("キャンセルされました");
            }
            else if (e.Error != null)
            {
                // この場合にはe.Resultにはアクセスできない
                MessageBox.Show("処理が中断されました");
            }
            else
            {
                // 処理結果の表示
                MessageBox.Show((String)e.Result);
            }
        }

        private void InitializeDataGridView()
        {
            // ComboBoxを除く項目数
            dataGridView.ColumnCount = 2;

            // 左端プロパティを非表示
            dataGridView.RowHeadersVisible = false;

            // Memo：下記を設定しない場合、最終行（☆がついている行）は削除できないので注意
            // 最下部プロパティを非表示
            dataGridView.AllowUserToAddRows = false;

            // ComboBoxのリスト作成
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            column.Items.Add(StrDataGridBuildListEnable);
            column.Items.Add(StrDataGridBuildListDisable);
            dataGridView.Columns.Insert(dataGridView.Columns[BuildEnableIdx].Index, column);

            // ヘッダ作成
            dataGridView.Columns[BuildEnableIdx].HeaderText = StrDataGridHeaderBuild;
            dataGridView.Columns[SolutionNameIdx].HeaderText = StrDataGridHeaderSolution;
            dataGridView.Columns[ProjectPathIdx].HeaderText = StrDataGridHeaderProjectPath;

            // 幅設定
            SetDataGridViewColumnWidth();
        }

        private String CreateAllBuildScript()
        {
            Boolean IsExportLog = false;
            if (textBox_LogDirectory.Text != String.Empty)
            {
                fio.IsExistDirectory(textBox_LogDirectory.Text, true);
                IsExportLog = true;
            }

            // ヘッダー生成
            String Script = CreateScriptHeader();

            // GridDataからビルド設定コマンド生成
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                Script += CreateBuildScript(i, IsExportLog);
            }

            return Script;
        }

        private String CreateScriptHeader()
        {
            String Script = "";

            //Script += @"set DEV_ENV=\""C:\Program Files (x86)\""Microsoft Visual Studio 10.0\""Common7\""IDE\""devenv.exe";
            Script += @"set DEV_ENV=""" + textBox_VisualStudioExePath.Text + @"""" + Environment.NewLine;

            //Script += @"set BUILD_OPT=/rebuild release";
            Script += "set BUILD_OPT=" + textBox_BuildOption.Text + Environment.NewLine;
            Script += Environment.NewLine;

            return Script;
        }

        private String GetCellData(int RowCount, int ColumnCount)
        {
            String CellData = "";
            if (dataGridView.Rows[RowCount].Cells[ColumnCount].Value != null)
            {
                CellData = dataGridView.Rows[RowCount].Cells[ColumnCount].Value.ToString();
            }

            return CellData;
        }

        private void SetCellData(int RowCount, int ColumnCount, String CellData = "")
        {
            dataGridView.Rows[RowCount].Cells[ColumnCount].Value = CellData;
        }

        private String GetFilePathName(String PathName, String FileName, String Ext = "")
        {
            return PathName.TrimEnd('\\') + '\\' + FileName + Ext;
        }

        private String GetSolutionPathName(String PathName, String FileName)
        {
            return GetFilePathName(PathName, FileName);
        }

        private String GetLogPathName(String PathName, String FileName)
        {
            return GetFilePathName(PathName, FileName.Replace(ExtSln, ExtLog));
        }

        private String CreateBuildScript(int RowCount, Boolean IsExportLog)
        {
            String BuildEnable = GetCellData(RowCount, BuildEnableIdx);
            String SolutionName = GetCellData(RowCount, SolutionNameIdx);
            String ProjectPath = GetCellData(RowCount, ProjectPathIdx);

            // パラメータチェック
            if (BuildEnable != StrDataGridBuildListEnable ||
                SolutionName == String.Empty ||
                ProjectPath == String.Empty)
            {
                return "";
            }

            String SolutionPath = GetSolutionPathName(ProjectPath, SolutionName);
            String LogName = GetLogPathName(textBox_LogDirectory.Text, SolutionName);

            String Script = "";
            if (IsExportLog)
            {
                // ファイルが存在したら削除
                if (File.Exists(LogName))
                {
                    Script += "del " + LogName + Environment.NewLine;
                }
                Script += @"%DEV_ENV% %BUILD_OPT% /out " + LogName + " " + SolutionPath + Environment.NewLine;
            }
            else
            {
                Script += @"%DEV_ENV% %BUILD_OPT% " + SolutionPath + Environment.NewLine;
            }
            Script += Environment.NewLine;

            return Script;
        }

        private void button_AddRaw_Click(object sender, EventArgs e)
        {
            // 選択されたRowの一つ下に追加
            dataGridView.Rows.Insert(dataGridView.CurrentRow.Index+1);
        }

        private void SetDataGridViewColumnWidth()
        {
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView.Columns[BuildEnableIdx].Width = 80;
            dataGridView.Columns[SolutionNameIdx].Width = 120;

            // Memo：AutoSizeしてもDataGridViewの全体サイズは取得できなかった
            //int AllColumnWidth = dataGridView.Columns[0].Width * dataGridView.ColumnCount;
            //dataGridView.Columns[2].Width = AllColumnWidth - dataGridView.Columns[0].Width - dataGridView.Columns[1].Width;
        }

        private void textBox_LogDirectory_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(textBox_LogDirectory.Text, e);
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < BuildEnableIdx)
            {
                return;
            }

            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridView dgv = (DataGridView)sender;
            if (dgv[e.ColumnIndex, e.RowIndex].GetType().Equals(typeof(DataGridViewComboBoxCell)))
            {
                dgv.BeginEdit(false);
                var edt = dgv.EditingControl as DataGridViewComboBoxEditingControl;
                edt.DroppedDown = true;
            }
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            // 自前でコピー
            Clipboard.SetDataObject(dataGridView.GetClipboardContent());

            // Memo：DataGridViewの機能でコピー
            //dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        }

        private void checkBox_UpdateGUI(object sender, EventArgs e)
        {
            UpdateOutputGUI();
        }

        private void UpdateOutputGUI()
        {
            Boolean DetectErrorEnable = true;
            Boolean ExcludeEnable = true;

            if (checkBox_DetectBuildError.Checked == false)
            {
                DetectErrorEnable = false;
                ExcludeEnable = false;
            }
            else
            {
                if (checkBox_IsExclude.Checked == false)
                {
                    ExcludeEnable = false;
                }
            }

            label_DetectBuildErrorWord.Enabled = DetectErrorEnable;
            textBox_DetectBuildErrorWord.Enabled = DetectErrorEnable;
            checkBox_IsExclude.Enabled = DetectErrorEnable;
            label_ExcludeWord.Enabled = ExcludeEnable;
            textBox_ExcludeWord.Enabled = ExcludeEnable;
        }

        private void checkBox_DeleteDirectory_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBuildGUI();
        }

        private void UpdateBuildGUI()
        {
            textBox_DeleteDirectoryName.Enabled = checkBox_DeleteDirectory.Checked;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < BuildEnableIdx)
            {
                return;
            }

            if (e.RowIndex < 0)
            {
                return;
            }

            String ExecPath = "";
            switch(e.ColumnIndex)
            {
                case SolutionNameIdx :

                    ExecPath = GetSolutionPathName(
                        GetCellData(e.RowIndex, ProjectPathIdx),
                        GetCellData(e.RowIndex, SolutionNameIdx));
                    break;

                case ProjectPathIdx:
                    ExecPath = GetCellData(e.RowIndex, ProjectPathIdx);
                    break;
                
                case BuildEnableIdx:
                    // no break
                default:
                    return;
            }

            util.ExecutePath(ExecPath);
        }

        private void button_AddAllSolution_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show(
                "既存リストをクリアし、ソリューションを一括で登録しますか？",
                "ソリューション一括登録",
                MessageBoxButtons.YesNo))
            {
                MessageBox.Show("処理を中断しました");
                return;
            }

            FormSelectDirectory dir = new FormSelectDirectory();
            DialogResult dr = dir.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                MessageBox.Show("処理を中断しました");
                return;
            }

            String RootDirectory = dir.DirectoryPath;
            if (!Directory.Exists(RootDirectory))
            {
                MessageBox.Show("ディレクトリパスが存在しません" + Environment.NewLine + RootDirectory);
                return;
            }

            // ソリューション追加
            AddAllSolution(RootDirectory);
        }

        private void AddAllSolution(String RootDirectory)
        {
            // リスト削除
            dataGridView.Rows.Clear();

            // リストアップ
            String[] SolutionPath;
            SolutionPath = Directory.GetFiles(RootDirectory, "*" + ExtSln, SearchOption.AllDirectories);

            // リスト登録
            dataGridView.Rows.Add(SolutionPath.Length);
            for (int i = 0; i < SolutionPath.Length; i++)
            {
                dataGridView.Rows[i].Cells[BuildEnableIdx].Value = StrDataGridBuildListEnable;
                dataGridView.Rows[i].Cells[SolutionNameIdx].Value = Path.GetFileName(SolutionPath[i]);
                dataGridView.Rows[i].Cells[ProjectPathIdx].Value = Path.GetDirectoryName(SolutionPath[i]);
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != ProjectPathIdx)
            {
                return;
            }

            if (e.RowIndex < 0)
            {
                return;
            }

            if ( dataGridView.Rows[e.RowIndex].Cells[ProjectPathIdx].Value == null )
            {
                // プロジェクトパスがblankだったら何もしない。
                return;
            }

            if (dataGridView.Rows[e.RowIndex].Cells[SolutionNameIdx].Value == null)
            {
                // ソリューション名が設定されていなかったら、自動設定
                String SolutionName = Path.GetFileName((String)dataGridView.Rows[e.RowIndex].Cells[ProjectPathIdx].Value) + ExtSln;
                dataGridView.Rows[e.RowIndex].Cells[SolutionNameIdx].Value = SolutionName;
            }

            if (dataGridView.Rows[e.RowIndex].Cells[BuildEnableIdx].Value == null)
            {
                // ビルド設定がされていなかったら、自動設定
                dataGridView.Rows[e.RowIndex].Cells[BuildEnableIdx].Value = StrDataGridBuildListEnable;
            }

        }
    }
}
