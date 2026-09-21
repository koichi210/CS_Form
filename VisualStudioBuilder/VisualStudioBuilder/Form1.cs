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
    partial class Form1 : StcBaseForm<SaveRestore>
    {
        private readonly String DefaultSettingFileName = "VisualStudioBuilder.json";

        // 設定ファイルはJSONが基本。旧XML(VisualStudioBuilder.xml)しか無い場合は起動時に読み込んでJSONへ移行し、
        // 旧XMLは削除する([[_Common/JsonSaveRestore.cs]])。プロファイル一覧は移行途中でも
        // 両方見えるよう、*.jsonと*.xmlの両方をリストアップする
        private const String LegacySettingFileName = @"VisualStudioBuilder.xml";
        private static readonly String[] ProfileExtensions = { "*.json", "*.xml" };

        private static Boolean IsJsonFile(String filePath)
        {
            return String.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        // 拡張子で振り分けて読み込む(旧XMLのプロファイルも引き続き開ける)
        private Boolean LoadProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Load(sr, filePath) : sr.LoadProc(filePath, this);
        }

        private Boolean SaveProfile(String filePath)
        {
            return IsJsonFile(filePath) ? JsonSaveRestore.Save(sr, filePath) : sr.SaveXmlFile(filePath);
        }


        private readonly String StrDataGridBuildListEnable = "○";
        private readonly String StrDataGridBuildListDisable = "×";

        private readonly String StrDataGridHeaderBuild = "ビルド";
        private readonly String StrDataGridHeaderSolution = "ソリューションファイル";
        private readonly String StrDataGridHeaderProjectPath = "プロジェクトフルパス";

        private readonly String ExtSln = ".sln";

        private const int BuildEnableIdx = 0;
        private const int SolutionNameIdx = 1;
        private const int ProjectPathIdx = 2;

        private StcFileInputOutput fio = new StcFileInputOutput();

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

            InitializeCommonSettings(Properties.Resources.VisualStudioBuilder);

            // DataGridViewの初期設定
            InitializeDataGridView();

            sr.RegistItem(this);
            JsonSaveRestore.LoadWithMigration(sr, DefaultSettingFileName, LegacySettingFileName,
                path => sr.LoadProc(path, this));
            UpdateBuildGUI();
            UpdateOutputGUI();
            util.UpdateProfileList(ref comboBox_Profile, ProfileExtensions, DefaultSettingFileName);

        }

        private void comboBox_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + comboBox_Profile.Text;
            LoadProfile(LoadFileName);
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            JsonSaveRestore.SaveProfileWithDialog(util, fio, comboBox_Profile, ProfileExtensions, SaveProfile);
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
                    DetectTargetLogList += Logic.GetLogPathName(textBox_LogDirectory.Text, SolutionName);
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
                String SolutionPath = Logic.GetSolutionPathName(ProjectPath, SolutionName);

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
            // ビルド中に押されると、実行中のビルドのログフォルダを消してしまうため先に弾く
            if (BuildWorker.IsBusy)
            {
                MessageBox.Show("ビルド実行中です。終わってからもう一度押してください");
                return;
            }

            if (!CheckSolutionPath())
            {
                return;
            }

            // ビルドログを削除
            StcFileInputOutput fio = new StcFileInputOutput();
            fio.DeleteDirectoryAndFile(textBox_LogDirectory.Text);

            // 以前はList<object>へ順番に詰めてDoWork側で位置で取り出していたが、
            // 並び順がずれると気づきにくいため、専用クラスをそのまま渡す
            BuildWorkerInfo bwi = new BuildWorkerInfo
            {
                BuildScript = CreateAllBuildScript(),
                IsDetectError = checkBox_DetectBuildError.Checked,
                DetectBuildErrorWord = textBox_DetectBuildErrorWord.Text,
                IsExclude = checkBox_IsExclude.Checked,
                IgnoreExecuteFile = textBox_ExcludeWord.Text,
                IsDeleteDirectory = checkBox_DeleteDirectory.Checked,
                DeleteDirectoryName = textBox_DeleteDirectoryName.Text,
                DetectTargetLogList = CreateDetectTargetList(),
            };

            BuildWorker.RunWorkerAsync(bwi);
        }

        private void BuildWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            BuildWorkerInfo bwi = (BuildWorkerInfo)e.Argument;

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

            // ビルドエラー検出。振り分けの判定はLogic側(テスト可能)に置き、
            // ログファイルの中身を見る部分だけここから渡す
            if (bwi.IsDetectError)
            {
                String[] TargetArray = util.ChangeStrLinear2Array(bwi.DetectTargetLogList, Environment.NewLine);
                e.Result = Logic.ClassifyBuildResult(TargetArray, bwi.DetectBuildErrorWord,
                                                     bwi.IsExclude, bwi.IgnoreExecuteFile,
                                                     fio.DetectFileData);
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
                MessageBox.Show("処理が中断されました" + Environment.NewLine + e.Error.Message);
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
                fio.EnsureDirectory(textBox_LogDirectory.Text, true);
                IsExportLog = true;
            }

            // ヘッダー生成
            String Script = Logic.CreateScriptHeader(textBox_VisualStudioExePath.Text, textBox_BuildOption.Text);

            // GridDataからビルド設定コマンド生成
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                Script += CreateBuildScript(i, IsExportLog);
            }

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

        private String CreateBuildScript(int RowCount, Boolean IsExportLog)
        {
            String BuildEnable = GetCellData(RowCount, BuildEnableIdx);
            String SolutionName = GetCellData(RowCount, SolutionNameIdx);
            String ProjectPath = GetCellData(RowCount, ProjectPathIdx);

            return Logic.CreateBuildScript(BuildEnable, SolutionName, ProjectPath, textBox_LogDirectory.Text, IsExportLog);
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
            // キーの種類を見ておらず、どのキーを押してもクリップボードを上書きしていた
            if (!e.Control || e.KeyCode != Keys.C)
            {
                return;
            }

            // 自前でコピー
            util.SetClipboardData(dataGridView.GetClipboardContent());

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

                    ExecPath = Logic.GetSolutionPathName(
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
