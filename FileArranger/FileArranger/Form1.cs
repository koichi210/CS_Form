using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;
using StandardTemplate;
using Microsoft.VisualBasic;

namespace FileArranger
{
    public partial class FileArranger : Form
    {
        readonly String SettingFileName = @"FileArranger.xml";

        private readonly String[] RenameDirColumn = { "変更前", "変更後" };
        private readonly String[] PartitionFileColumn = { "対象", "移動前名称", "移動後名称" };

        private readonly int PaddingMinNum = 2;
        private readonly int RenameSrcIdx = 0;
        private readonly int RenameDestIdx = 1;

        private readonly int CreateFolderTargetIdx = 0;
        private readonly int CreateFolderMoveSrcIdx = 1;
        private readonly int CreateFolderMoveDestIdx = 2;

        private readonly int SortFileRenameTargetIdx = 0;

        public String[] RefrenceCandidateFolders;      // リファレンス名の候補

        private StcFileInputOutput fio = new StcFileInputOutput();
        private SaveRestore sr = new SaveRestore();
        private Utils util = new Utils();
        private ProcessMemory pmd = new ProcessMemory();
        private ProcessMemory pmf = new ProcessMemory();

        public FileArranger()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.FileArranger;

            // カレントディレクトリ移動
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            //ListView初期設定
            rd_listView_Target_Update();
            pf_listView_Target_Update();

            sr.RegistLoadItem(this);
            sr.LoadProc(SettingFileName, this);
            util.UpdateProfileList(ref comboBox_LoadSetting);
        }

        private void mf_textBox_SourceDir_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(md_textBox_SourceDir.Text, e);
        }

        private void md_button_Listup_Click(object sender, EventArgs e)
        {
            ListupMoveDirectory();
        }

        private void mf_listBox_Listup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mf_button_Move_SubDir_Click(sender, e);
            }
            else
            {
                util.SelectAll(e);
            }
        }

        private void cmn_textBox_AddList_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void rd_button_Listup_Target_Click(object sender, EventArgs e)
        {
            ListupRenameTargetDirectory();
        }

        private void rd_textBox_ExistItemDir_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(rd_textBox_ExistItemDir.Text, e);
        }

        private void pf_textBox_TargetFile_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(pf_textBox_TargetFile.Text, e);
        }

        private void pf_textBox_RefrenceFile_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(pf_textBox_RefrenceFile.Text, e);
        }

        private void sf_textBox_TargetFile_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(sf_textBox_TargetFile.Text, e);
        }

        private void mf_textBox_TargetDir_KeyDown(object sender, KeyEventArgs e)
        {
            util.ExecutePath(mf_textBox_TargetDir.Text, e);
        }

        private void mf_listBox_Listup_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (md_listBox_Listup.SelectedItems.Count > 0)
            {
                String TargetPath = md_textBox_SourceDir.Text + @"\" + md_listBox_Listup.SelectedItem.ToString();
                util.ExecutePath(TargetPath);
            }
        }

        private void md_button_MoveTopDir_Click(object sender, EventArgs e)
        {
            Move_Directory(true);
        }

        private void mf_button_Move_SubDir_Click(object sender, EventArgs e)
        {
            Move_Directory(false);
        }

        private void Move_Directory(Boolean IsMoveTopDir)
        {
            if (!fio.IsExistDirectory(md_comboBox_TargetDir.Text))
            {
                return;
            }

            if (md_listBox_Listup.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            for (int i = 0; i < md_listBox_Listup.SelectedItems.Count; i++)
            {
                String SourceTargetName;
                String DestTargetName;
                if (IsMoveTopDir)
                {

                    SourceTargetName = fio.GetFirstPathName(md_listBox_Listup.SelectedItems[i].ToString());
                    DestTargetName = SourceTargetName;
                }
                else
                {
                    SourceTargetName = md_listBox_Listup.SelectedItems[i].ToString();
                    DestTargetName = fio.GetLastPathName(md_listBox_Listup.SelectedItems[i].ToString());
                }

                String SourcePath = md_textBox_SourceDir.Text + @"\" + SourceTargetName;
                String DestPath = md_comboBox_TargetDir.Text + @"\" + DestTargetName;

                // Top階層ごと移動した場合などで、すでにDirectoryが存在しないケースをcare
                if (!Directory.Exists(SourcePath))
                {
                    continue;
                }

                // 移動先にすでにフォルダがある場合は重複回避
                util.CreateFolderNameOverLapShirk(ref DestPath, i);

                fio.MoveDirectory(SourcePath, DestPath);
            }

            // リストを更新
            ListupMoveDirectory(true);
        }

        private void mf_button_Delete_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < md_listBox_Listup.SelectedItems.Count; i++)
            {
                String DelPath = md_textBox_SourceDir.Text + @"\" + md_listBox_Listup.SelectedItems[i].ToString();
                DirectoryInfo DelDir = new DirectoryInfo(DelPath);
                DelDir.Delete(true);
            }

            // リストを更新
            ListupMoveDirectory();
        }

        private void ListupMoveDirectory(Boolean IsRestoreScrollBarPos = false)
        {
            if (!Directory.Exists(md_textBox_SourceDir.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + md_textBox_SourceDir.Text );
                return;
            }

            int RegistNum = 0;
            // フォルダパスの末尾に'\\'があったら削除
            char[] chTrims = { '\\', '/' };
            md_textBox_SourceDir.Text = md_textBox_SourceDir.Text.TrimEnd(chTrims);

            int ScrollBarPos = 0;
            if (IsRestoreScrollBarPos)
            {
                ScrollBarPos = md_listBox_Listup.TopIndex;
            }
            md_listBox_Listup.Items.Clear();
            String[] files = Directory.GetDirectories(md_textBox_SourceDir.Text, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                // ファイル or フォルダをリストアップ
                String[] entries = Directory.GetFileSystemEntries(files[i]);
                if (entries.Length == 0)
                {
                    continue;
                }

                for (int j = 0; j < entries.Length; j++)
                {
                    if (File.Exists(entries[j]))
                    {
                        String FileName = files[i].Remove(0, md_textBox_SourceDir.Text.Length + 1);   // "\\"の分を1加算
                        md_listBox_Listup.Items.Add(FileName);
                        RegistNum++;
                        break;
                    }
                }
            }
            md_listBox_Listup.TopIndex = ScrollBarPos;
            md_label_TotalNum.Text = "フォルダ数：" + RegistNum.ToString();
        }

        private void rd_button_Execute_Rename_Click(object sender, EventArgs e)
        {
            RenameFolderExecute();
        }

        private void rd_button_RenameFolderRestore_Click(object sender, EventArgs e)
        {
            if (!pmd.DecrementRegistNumber())
            {
                MessageBox.Show("これ以上復元できません");
                return;
            }

            while (pmd.IsExistRestoreList())
            {
                String SrcName = "";
                String DestName = "";
                pmd.GetRestoreList(ref SrcName, ref DestName);
                Directory.Move(DestName, SrcName);

            }
            ListupRenameTargetDirectory();
        }

        private void RenameFolderExecute()
        {
            if (rd_listView_Target.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            int idx = 0;
            for (int i = 0; i < rd_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                idx = rd_listView_Target.SelectedItems[i].Index;

                // 変更するファイル名
                String SrcName = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.Items[idx].SubItems[RenameSrcIdx].Text;
                String DestName = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.Items[idx].SubItems[RenameDestIdx].Text;

                // ファイル名の重複回避
                util.CreateFileNameOverLapShirk(ref DestName, i);
                fio.MoveDirectory(SrcName, DestName);
                pmd.SetRestoreList(SrcName, DestName);
            }
            pmd.IncrementRegistNumber();

            ListupRenameTargetDirectory(idx);
        }

        private void ListupRenameTargetDirectory(int ScrollbarPos = 0)
        {
            if (!Directory.Exists(rd_comboBox_RenameDir.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + rd_comboBox_RenameDir.Text );
                return;
            }

            rd_listView_Target.Items.Clear();

            // フォルダをリストアップ
            String[] Folders = Directory.GetDirectories(rd_comboBox_RenameDir.Text);
            for (int i = 0; i < Folders.Length; i++)
            {
                String FileName = Folders[i].Remove(0, rd_comboBox_RenameDir.Text.Length + 1);   // "\\"の分を1加算

                String[] item = { FileName, "" };
                rd_listView_Target.Items.Add(new ListViewItem(item));
            }

            if (ScrollbarPos > Folders.Length)
            {
                ScrollbarPos = Folders.Length - 1;
            }
            rd_listView_Target.EnsureVisible(ScrollbarPos);

            rd_label_TotalNum.Text = "フォルダ数：" + Folders.Length.ToString();
            rd_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void UpdateRenameComboBox()
        {
            util.SetComboBoxFromArraySubString(
                rd_comboBox_MergeWord,
                RefrenceCandidateFolders,
                rd_textBox_ExistItemDir.Text.Length + 1,
                rd_textBox_SplitWord3.Text,
                rd_comboBox_MergeWord.Text,
                true);
        }

        public void UpdateMoveDestDirComboBox()
        {
            util.SetComboBoxFromArray(pf_comboBox_MoveDestDirName, RefrenceCandidateFolders, pf_textBox_RefrenceFile.Text);
        }

        private void rd_listView_Rename_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 選択解除
            for (int i = 0; i < rd_listView_Target.Items.Count; i++)
            {
                rd_listView_Target.Items[i].SubItems[RenameDestIdx].Text = "";
            }

            rd_listView_Rename_UpdteListBox();
        }

        private void rd_comboBox_MergeWord_TextChanged(object sender, EventArgs e)
        {
            rd_listView_Rename_UpdteListBox();
        }

        private String ChangeWide2Narrow(String SrcString)
        {
            String RegesStr = "[０-９Ａ-Ｚａ-ｚ　]";
            Regex re = new Regex(RegesStr);
            return re.Replace(SrcString, myReplacer);
        }

        private String myReplacer(Match m)
        {
            // Memo: 参照設定に「Microsoft.VisualBasic」が必要
            return Strings.StrConv(m.Value, VbStrConv.Narrow);
        }

        private void rd_listView_Rename_UpdteListBox()
        {
            rd_label_SelectNum.Text = "選択数：" + rd_listView_Target.SelectedItems.Count.ToString();
            for (int i = 0; i < rd_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = rd_listView_Target.SelectedItems[i].Index;

                //文字列から数値を取得
                String SrcString = ChangeWide2Narrow(rd_listView_Target.Items[idx].SubItems[RenameSrcIdx].Text);
                long DestNumber = util.GetNumberFromRear(SrcString,
                    rd_textBox_SearchTitleLine.Text,
                    rd_textBox_SearchTitleLength.Text);

                String Number = GetNumber(DestNumber);

                // 変更後ファイル名を生成
                String DestName = rd_comboBox_MergeWord.Text + rd_textBox_AddTitlePreWord.Text + Number + rd_comboBox_AddTitlePostWord.Text;
                rd_listView_Target.Items[idx].SubItems[RenameDestIdx].Text = DestName;
            }
        }

        private void rd_listView_Rename_DoubleClick(object sender, EventArgs e)
        {
            String FilePath = rd_comboBox_RenameDir.Text + @"\" + rd_listView_Target.SelectedItems[SortFileRenameTargetIdx].SubItems[RenameSrcIdx].Text;
            if (Directory.Exists(FilePath))
            {
                if (rd_checkBox_FileOpen.Checked)
                {
                    String[] files = Directory.GetFiles(FilePath, "*", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        FilePath = files[0];
                    }
                }
                System.Diagnostics.Process.Start(FilePath);
                rd_comboBox_MergeWord.Focus();
            }
        }

        private void rd_listView_Target_Update()
        {
            rd_listView_Target.Columns.Clear();

            // ListViewコントロールのプロパティを設定
            rd_listView_Target.FullRowSelect = true;
            rd_listView_Target.GridLines = true;
            rd_listView_Target.Sorting = SortOrder.Ascending;
            rd_listView_Target.View = View.Details;

            // 列（コラム）ヘッダの作成
            ColumnHeader columnSource = new ColumnHeader();
            columnSource.Text = RenameDirColumn[0];
            columnSource.Width = rd_listView_Target.Width / RenameDirColumn.Length;

            ColumnHeader columnTarget = new ColumnHeader();
            columnTarget.Text = RenameDirColumn[1];
            columnTarget.Width = rd_listView_Target.Width / RenameDirColumn.Length;
            
            ColumnHeader[] colHeaderRegValue = { columnSource, columnTarget };
            rd_listView_Target.Columns.AddRange(colHeaderRegValue);
        }

        private void pf_button_Listup_Target_Click(object sender, EventArgs e)
        {
            ListupTargetMoveDirectory();
        }

        private void pf_listView_Target_Update()
        {
            pf_listView_Target.Columns.Clear();

            // ListViewコントロールのプロパティを設定
            pf_listView_Target.FullRowSelect = true;
            pf_listView_Target.GridLines = true;
            pf_listView_Target.Sorting = SortOrder.Ascending;
            pf_listView_Target.View = View.Details;

            // 列（コラム）ヘッダの作成
            ColumnHeader columnTarget = new ColumnHeader();
            columnTarget.Text = PartitionFileColumn[0];
            columnTarget.Width = pf_listView_Target.Width / PartitionFileColumn.Length;

            ColumnHeader columnMoveSrc = new ColumnHeader();
            columnMoveSrc.Text = PartitionFileColumn[1];
            columnMoveSrc.Width = pf_listView_Target.Width / PartitionFileColumn.Length;

            ColumnHeader columnMoveDest = new ColumnHeader();
            columnMoveDest.Text = PartitionFileColumn[2];
            columnMoveDest.Width = pf_listView_Target.Width / PartitionFileColumn.Length;

            ColumnHeader[] colHeaderRegValue = { columnTarget, columnMoveSrc, columnMoveDest };
            pf_listView_Target.Columns.AddRange(colHeaderRegValue);
        }

        private void ListupTargetMoveDirectory()
        {
            if (!Directory.Exists(pf_textBox_TargetFile.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + pf_textBox_TargetFile.Text );
                return;
            }

            // 移動元フォルダをリストアップ
            String[] Files = Directory.GetFiles(pf_textBox_TargetFile.Text);
            pf_listView_Target.Items.Clear();
            for (int i = 0; i < Files.Length; i++)
            {
                String FileName = Files[i].Remove(0, pf_textBox_TargetFile.Text.Length + 1);   // "\\"の分を1加算

                String[] item = { FileName, "", "" };
                pf_listView_Target.Items.Add(new ListViewItem(item));
            }
            pf_label_TotalNum.Text = "ファイル数：" + Files.Length.ToString();

            // 左端しかAutoResizeしないので、使い勝手悪い。。
            //pf_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            pf_listView_Target_Update();
        }

        private int GetAddCount(ListView lv, String FileName, String TrimName, Boolean IsReverse = false)
        {
            int Count = 0;
            String SerchName = "";

            int FileNameidx;
            if (!IsReverse)
            {
                FileNameidx = FileName.IndexOf(TrimName);
            }
            else
            {
                FileNameidx = FileName.LastIndexOf(TrimName);
            }

            if (0 <= FileNameidx)
            {
                SerchName = FileName.Substring(0, FileNameidx);
            }

            for (int i = 0; i < lv.SelectedItems.Count; i++)
            {
                int idx = lv.SelectedItems[i].Index;
                String SrcFileName = lv.Items[idx].SubItems[CreateFolderTargetIdx].Text;

                if (SrcFileName.IndexOf(SerchName) != -1)
                {
                    Count ++;
                }
            }

            if (Count == 0)
            {
               // 今回新規追加時の初期値
                Count = 1;
            }
            return Count;
        }

        private void pf_listView_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            pf_button_ClearSelect_Click(sender, e);
            pf_label_SelectNum.Text = "選択数：" + pf_listView_Target.SelectedItems.Count.ToString();

            UpdatePartitionFileList();

            // 単独ファイル選択時はコンボボックスに表示する
            if (pf_listView_Target.SelectedItems.Count != 0)
            {
                int Idx = pf_listView_Target.SelectedItems[0].Index;
                pf_comboBox_MoveDestDirName.Text = pf_listView_Target.Items[Idx].SubItems[CreateFolderMoveDestIdx].Text;
            }
        }

        private Boolean GetPartitionNameFromListView(ref String SrcFolderName, ref String TargetFolderName, String SrcFileName)
        {
            Boolean IsSuccess = true;

            int SameIdx = util.GetStringFromListViewInSelect(pf_listView_Target, CreateFolderTargetIdx, SrcFileName, pf_textBox_TargetSeprator.Text, true);
            if (0 <= SameIdx)
            {
                SrcFolderName = pf_listView_Target.Items[SameIdx].SubItems[CreateFolderMoveSrcIdx].Text;
                TargetFolderName = pf_listView_Target.Items[SameIdx].SubItems[CreateFolderMoveDestIdx].Text;
            }

            if (TargetFolderName == String.Empty)
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }

        private Boolean GetPartitionNameFromComboBox(ref String SrcFolderName, ref String TargetFolderName, String SrcFileName)
        {
            Boolean IsSuccess = true;

            TargetFolderName = util.FindStringFromComboBox(pf_comboBox_MoveDestDirName, SrcFileName, pf_textBox_TargetSeprator.Text, true);
            if (TargetFolderName != String.Empty)
            {
                // 期待するフォルダ名が見つかった
                SrcFolderName = TargetFolderName;

                // 数値をインクリした文字列
                TargetFolderName = GetPatitionTargetNameWithNumber(SrcFolderName, SrcFileName, "0");
            }

            if (TargetFolderName == String.Empty)
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }

        private void CreatePartitionName(ref String SrcFolderName, ref String TargetFolderName, String SrcFileName)
        {
            // 期待するフォルダ名が見つからなかった
            SrcFolderName = "";
            String SampleSrcFolderName = util.CreateNewFolderName(SrcFileName, pf_textBox_TargetSeprator.Text, true);
            SampleSrcFolderName += cmn_textBox_AddListSuffix.Text;

            // 数値を考慮した文字列
            TargetFolderName = GetPatitionTargetNameWithNumber(SampleSrcFolderName, SrcFileName, "0");    // 複数ファイル選択時にインクリしてくれる
        }

        private String GetPatitionTargetNameWithNumber(String SrcFolderName, String SrcFileName, String DefaultNumber)
        {
            long SrcNumber = util.GetNumberFromRear(SrcFolderName, pf_textBox_SearchTitleLine.Text, pf_textBox_SearchTitleLength.Text, DefaultNumber);
            int AddCount = GetAddCount(pf_listView_Target, SrcFileName, pf_textBox_TargetSeprator.Text, true);

            String Number = GetNumber(SrcNumber, AddCount);
            int SrcNumberDigit = GetPadding(SrcNumber);

            return SrcFolderName.Substring(0, SrcFolderName.Length - SrcNumberDigit) + Number;
        }

        private String GetNumber(long SrcNumber, int AddCount = 0)
        {
            long DestNumber = SrcNumber + AddCount;

            int PaddingDigit = GetPadding(DestNumber);
            return DestNumber.ToString().PadLeft(PaddingDigit, '0');
        }

        private int GetPadding(long Number, Boolean ThroughNumberZero = false)
        {
            int PaddingDigit = 0;

            if ( ThroughNumberZero && Number == 0)
            {
                // 数値が「0」のときは、桁数も「0」とする
            }
            else if (Number.ToString().Length <= PaddingMinNum)
            {
                PaddingDigit = PaddingMinNum;
            }
            return PaddingDigit;
        }

        private void UpdatePartitionFileList()
        {
            for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = pf_listView_Target.SelectedItems[i].Index;

                String SrcFileName = pf_listView_Target.Items[idx].SubItems[CreateFolderTargetIdx].Text;
                String SrcFolderName = "";
                String TargetFolderName = "";

                Boolean IsSuccess = false;
                // ListViewに既出であれば流量
                IsSuccess = GetPartitionNameFromListView(ref SrcFolderName, ref TargetFolderName, SrcFileName);

                if (!IsSuccess)
                {
                    // ListViewに無ければCombBoxから検索
                    IsSuccess = GetPartitionNameFromComboBox(ref SrcFolderName, ref TargetFolderName, SrcFileName);
                }

                if (!IsSuccess)
                {
                    if (pf_checkBox_CreateNewDir.Checked)
                    {
                        // CombBoxにもなかったら新規作成
                        CreatePartitionName(ref SrcFolderName, ref TargetFolderName, SrcFileName);
                    }
                }

                pf_listView_Target.Items[idx].SubItems[CreateFolderMoveSrcIdx].Text = SrcFolderName;
                pf_listView_Target.Items[idx].SubItems[CreateFolderMoveDestIdx].Text = TargetFolderName;
            }
        }

        private Boolean FindMoveFolderName(String FileName, ref String DestName, int SearchedIdx)
        {
            // ファイル名から、検索対象の文字列までのインデックスを取得
            int FileNameidx = FileName.IndexOf(pf_textBox_TargetSeprator.Text);
            if (FileNameidx <= 0)
            {
                DestName = FileName;
                return false;
            }
            DestName = FileName.Substring(0, FileNameidx);

            // すでに検索済みだったら、検索結果を再利用
            for (int i = 0; i < SearchedIdx; i++)
            {
                int idx = pf_listView_Target.SelectedItems[i].Index;
                String FolderName = pf_listView_Target.Items[idx].SubItems[CreateFolderMoveSrcIdx].Text;
                if (FolderName.IndexOf(DestName) != -1)
                {
                    DestName = FolderName;
                    return true;
                }
            }

            return false;
        }

        private void pf_button_ClearSelect_Click(object sender, EventArgs e)
        {
            // 選択解除
            for (int i = 0; i < pf_listView_Target.Items.Count; i++)
            {
                pf_listView_Target.Items[i].SubItems[CreateFolderMoveSrcIdx].Text = "";
                pf_listView_Target.Items[i].SubItems[CreateFolderMoveDestIdx].Text = "";
            }
        }

        private void pf_button_CreateFolderExecute_Click(object sender, EventArgs e)
        {
            MovePartitionFile();
        }

        private void MovePartitionFile()
        {
            if (pf_listView_Target.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            progressBar.Maximum = pf_listView_Target.SelectedItems.Count;
            progressBar.Minimum = 0;
            progressBar.Value = 0;

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(pf_textBox_TargetFile.Text);
            arguments.Add(pf_textBox_RefrenceFile.Text);
            arguments.Add(pf_listView_Target.SelectedItems.Count);

            for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
            {
                int idx = pf_listView_Target.SelectedItems[i].Index;
                arguments.Add(pf_listView_Target.Items[idx].SubItems[CreateFolderTargetIdx].Text);
                arguments.Add(pf_listView_Target.Items[idx].SubItems[CreateFolderMoveSrcIdx].Text);
                arguments.Add(pf_listView_Target.Items[idx].SubItems[CreateFolderMoveDestIdx].Text);
            }
            bgPartition.RunWorkerAsync(arguments);   // ⇒bgPartition_DoWork()
        }

        private void pf_listView_Target_DoubleClick(object sender, EventArgs e)
        {
            int idx = pf_listView_Target.SelectedItems[0].Index;
            String DirPath = pf_textBox_RefrenceFile.Text + @"\" + pf_listView_Target.Items[idx].SubItems[CreateFolderMoveSrcIdx].Text;
            if (Directory.Exists(DirPath))
            {
                util.ExecutePath(DirPath);
            }
        }

        private void sf_button_Listup_TargetFile_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(sf_textBox_TargetFile.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + sf_textBox_TargetFile.Text);
                return;
            }

            // フォルダをリストアップ
            String[] Folders = Directory.GetDirectories(sf_textBox_TargetFile.Text);
            sf_listBox_Target.Items.Clear();
            for (int i = 0; i < Folders.Length; i++)
            {
                String FolderName = Folders[i].Remove(0, sf_textBox_TargetFile.Text.Length + 1);   // "\\"の分を1加算
                sf_listBox_Target.Items.Add(FolderName);
            }

            sf_label_TotalNum.Text = "フォルダ数：" + Folders.Length.ToString();

        }

        private void sf_button_SortFileRename_Click(object sender, EventArgs e)
        {
            if (sf_listBox_Target.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            for (int i = 0; i < sf_listBox_Target.SelectedItems.Count; i++)
            {
                String FilePath = sf_textBox_TargetFile.Text + @"\" + sf_listBox_Target.SelectedItems[i].ToString();
                SortFile(FilePath);
            }
            pmf.IncrementRegistNumber();
        }

        private void sf_button_Sort_Restore_Click(object sender, EventArgs e)
        {
            if (!pmf.DecrementRegistNumber())
            {
                MessageBox.Show("これ以上復元できません");
                return;
            }

            while (pmf.IsExistRestoreList())
            {
                String SrcName = "";
                String DestName = "";
                pmf.GetRestoreList(ref SrcName, ref DestName);
                File.Move(DestName, SrcName);
            }
        }

        private void SortFile(String FilePath)
        {
            String[] Files = Directory.GetFiles(FilePath);
            for (int i = 0; i < Files.Length; i++)
            {
                String Ext = Path.GetExtension(Files[i]);
                String DestName = FilePath + @"\" + String.Format("{0:D3}", i) + Ext;
                File.Move(Files[i], DestName);
                pmf.SetRestoreList(Files[i], DestName);
            }
        }

        private void LoadSetting_Click(object sender, EventArgs e)
        {
            String LoadFileName = fio.SelectLoadFileName(SettingFileName);
            if (sr.LoadProc(LoadFileName, this))
            {
                comboBox_LoadSetting.Text = Path.GetFileName(LoadFileName);
            }
        }

        private void SaveSetting_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(comboBox_LoadSetting.Text);
            if (sr.SaveSetting(SaveFileName, this))
            {
                util.UpdateProfileList(ref comboBox_LoadSetting, Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
            }
        }

        private void comboBox_LoadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + comboBox_LoadSetting.Text;
            sr.LoadProc(LoadFileName, this);
        }

        private void pf_listView_Target_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    MovePartitionFile();
                    break;

                case Keys.Delete:
                    for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
                    {
                        // 参照しているListViewのIdx
                        int idx = pf_listView_Target.SelectedItems[i].Index;
                        pf_listView_Target.Items[idx].SubItems[CreateFolderMoveDestIdx].Text = "";
                    }
                    break;

                default:
                    break;
            }
        }

        private void rd_comboBox_MergeWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                RenameFolderExecute();
            }
        }

        private void pf_comboBox_MoveDestDirName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                MovePartitionFile();
            }
            else
            {
                for (int i = 0; i < pf_listView_Target.SelectedItems.Count; i++)
                {
                    int idx = pf_listView_Target.SelectedItems[i].Index;
                    pf_listView_Target.Items[idx].SubItems[CreateFolderMoveDestIdx].Text = pf_comboBox_MoveDestDirName.Text;
                }
            }
        }

        private void mf_listBox_Listup_SelectedIndexChanged(object sender, EventArgs e)
        {
            md_label_SelectNum.Text = "選択数：" + md_listBox_Listup.SelectedItems.Count.ToString();
        }

        private void FileArranger_ResizeEnd(object sender, EventArgs e)
        {
            rd_listView_Target.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            pf_listView_Target_Update();
        }

        private void mf_button_Listup_Click(object sender, EventArgs e)
        {
            MoveFileListup();
        }

        private void MoveFileListup()
        {
            if (!Directory.Exists(mf_textBox_SourceDir.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + mf_textBox_SourceDir.Text);
                return;
            }

            // 移動元フォルダをリストアップ
            String[] Files = Directory.GetFiles(mf_textBox_SourceDir.Text);
            mf_listBox_Target.Items.Clear();
            for (int i = 0; i < Files.Length; i++)
            {
                String FileName = Files[i].Remove(0, mf_textBox_SourceDir.Text.Length + 1);   // "\\"の分を1加算
                mf_listBox_Target.Items.Add(FileName);
            }
            mf_label_TotalNum.Text = "ファイル数：" + Files.Length.ToString();
        }

        private void sf_listBox_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            sf_label_SelectNum.Text = "選択数：" + sf_listBox_Target.SelectedItems.Count.ToString();
        }

        private void sf_listBox_Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sf_button_SortFileRename_Click(sender, e);
            }
            else
            {
                util.SelectAll(e);
            }
        }

        private void mf_listBox_Target_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void mf_listBox_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            mf_label_SelectNum.Text = "選択数：" + mf_listBox_Target.SelectedItems.Count.ToString();
        }

        private void mf_button_MoveFile_Click(object sender, EventArgs e)
        {
            if (!fio.IsExistDirectory(mf_textBox_TargetDir.Text))
            {
                return;
            }

            if (mf_listBox_Target.SelectedItems.Count == 0)
            {
                MessageBox.Show("項目が選択されていません。");
                return;
            }

            progressBar.Maximum = mf_listBox_Target.SelectedItems.Count;
            progressBar.Minimum = 0;
            progressBar.Value = 0;

            // 別スレッドを非同期実行
            List<object> arguments = new List<object>();
            arguments.Add(mf_textBox_SourceDir.Text);
            arguments.Add(mf_textBox_TargetDir.Text);
            arguments.Add(mf_listBox_Target.SelectedItems.Count);
            for (int i = 0; i < mf_listBox_Target.SelectedItems.Count; i++)
            {
                arguments.Add(mf_listBox_Target.SelectedItems[i].ToString() );
            }

            bgWorkerMove.RunWorkerAsync(arguments);   // ⇒bgWorker_DoWork()
        }

        private void mf_textBox_SourceDir_KeyDown_1(object sender, KeyEventArgs e)
        {
            util.ExecutePath(mf_textBox_SourceDir.Text, e);
        }

        private void mf_textBox_TargetDir_KeyDown_1(object sender, KeyEventArgs e)
        {
            util.ExecutePath(mf_textBox_TargetDir.Text, e);
        }

        private void rd_listView_Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                rd_button_Execute_Rename_Click(sender, e);
                rd_comboBox_MergeWord.Focus();
            }
            else if (e.Control == true && e.KeyCode == Keys.C)
            {
                util.CopyToClipboard(e, rd_listView_Target, "", RenameSrcIdx);
            }
        }

        private void bgWorkerMove_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;
            String Sourcedir = (String)genericlist[0];
            String TargetDir = (String)genericlist[1];
            int ItemCount = (int)genericlist[2];

            for (int i = 0; i < ItemCount; i++)
            {
                String TargetName = (String)genericlist[3 + i];     // 3個目以降が対象のファイル名
                String SourcePath = Sourcedir + @"\" + TargetName;
                String TargetPath = TargetDir + @"\" + fio.GetLastPathName(TargetName);

                // 移動先にすでにフォルダがある場合は重複回避
                util.CreateFolderNameOverLapShirk(ref TargetPath, i);
                fio.MoveDirectory(SourcePath, TargetPath);

                worker.ReportProgress(i);      // ⇒ProgressChanged()

                // キャンセルされてないかチェック
                //if (worker.CancellationPending)
                //{
                //    e.Cancel = true;
                //    return;
                //}
            }
            worker.ReportProgress(ItemCount);

            // このメソッドからの戻り値
            e.Result = "すべて完了";

            // ⇒RunWorkerCompleted()
        }

        private void bgWorkerMove_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            progressText.Text = e.ProgressPercentage + "/" + progressBar.Maximum + " 完了";
            progressBar.Value = e.ProgressPercentage;
        }

        private void bgWorkerMove_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // この場合はe.Resultにはアクセスできない
                MessageBox.Show("キャンセルされました");
            }

            // リストを更新
            MoveFileListup();
        }

        private void bgPartition_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;
            String TargetFilePath = (String)genericlist[0];
            String TargetDir = (String)genericlist[1];
            int ItemCount = (int)genericlist[2];

            int generic_index = 3;
            for (int i = 0; i < ItemCount; i++)
            {
                // TODO：いけてない。
                String TargetName = (String)genericlist[generic_index + i * generic_index + 0]; // 4個目以降が対象[3個おき]
                String MoveSrc = (String)genericlist[generic_index + i * generic_index + 1];    // 5個目以降が対象[3個おき]
                String MoveDest = (String)genericlist[generic_index + i * generic_index + 2];   // 6個目以降が対象[3個おき]

                if (MoveDest == String.Empty)
                {
                    // [移動後名称]が無い項目は処理対象外
                    continue;
                }

                // TODO：変数名整理したい
                String SrcDirName = TargetFilePath;
                String DestPreDirName = TargetDir + @"\" + MoveSrc;
                String DestPostDirName = TargetDir + @"\" + MoveDest;

                String FileName = TargetName;
                String SrcFileName = SrcDirName + @"\" + FileName;
                String DestFileName = DestPostDirName + @"\" + FileName;

                try
                {
                    // 移動先フォルダを生成
                    if (MoveSrc != String.Empty && // 移動元がカラじゃない
                         System.IO.Directory.Exists(DestPreDirName)     // フォルダが存在する
                         )
                    {
                        // 元フォルダと先フォルダが違うときだけ移動
                        if (DestPreDirName != DestPostDirName)
                        {
                            // フォルダ名が変わるのであればリネーム
                            System.IO.Directory.Move(DestPreDirName, DestPostDirName);
                        }
                    }
                    else
                    {
                        // 元フォルダが無かったら新規フォルダなので、先フォルダを作成
                        System.IO.Directory.CreateDirectory(DestPostDirName);
                    }

                    // ファイル名の重複回避
                    if (!util.CreateFileNameOverLapShirk(ref DestFileName, i))
                    {
                        MessageBox.Show("ファイル名が重複しました。処理をスキップします。" + Environment.NewLine +
                                        FileName,
                                        "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                        continue;
                    }
                    File.Move(SrcFileName, DestFileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("エラーが発生しました。処理を中断します。" + Environment.NewLine +
                                    "移動元：" + SrcFileName + Environment.NewLine +
                                    "移動先：" + DestFileName + Environment.NewLine,
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    break;
                }

                worker.ReportProgress(i);      // ⇒ProgressChanged()
            }
            worker.ReportProgress(ItemCount);

            // このメソッドからの戻り値
            e.Result = "すべて完了";

            // ⇒RunWorkerCompleted()
        }

        private void bgPartition_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率の表示
            progressText.Text = e.ProgressPercentage + "/" + progressBar.Maximum + " 完了";
            progressBar.Value = e.ProgressPercentage;
        }

        private void bgPartition_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // この場合はe.Resultにはアクセスできない
                MessageBox.Show("キャンセルされました");
            }

            // 選択解除
            for (int i = 0; i < pf_listView_Target.Items.Count; i++)
            {
                pf_listView_Target.Items[i].SubItems[CreateFolderMoveSrcIdx].Text = "";
                pf_listView_Target.Items[i].SubItems[CreateFolderMoveDestIdx].Text = "";
            }

            // リストを更新
            ListupTargetMoveDirectory();

            //リファレンスフォルダは自動更新しない
            //UpdateMoveDestDirComboBox();
        }

        private void rd_label_ExistItemDir_DoubleClick(object sender, EventArgs e)
        {
            rd_textBox_ExistItemDir.ReadOnly = !rd_textBox_ExistItemDir.ReadOnly;
        }

        private void pf_label_RefrenceFile_DoubleClick(object sender, EventArgs e)
        {
            pf_textBox_RefrenceFile.ReadOnly = !pf_textBox_RefrenceFile.ReadOnly;
        }

        private void cmn_textBox_Reference_TextChanged(object sender, EventArgs e)
        {
            rd_textBox_ExistItemDir.Text = cmn_textBox_Reference.Text;
            pf_textBox_RefrenceFile.Text = cmn_textBox_Reference.Text;
        }

        private void cmn_button_Listup_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(cmn_textBox_Reference.Text))
            {
                MessageBox.Show("フォルダパスが不正です。" + cmn_textBox_Reference.Text);
                return;
            }

            // フォルダをリストアップ
            RefrenceCandidateFolders = Directory.GetDirectories(cmn_textBox_Reference.Text);

            // 新規追加
            if ( !cmn_textBox_AddList.Text.Equals(String.Empty) )
            {
                String[] AddRefrenceList = cmn_textBox_AddList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                DeleteDuplicate(RefrenceCandidateFolders, ref AddRefrenceList, rd_textBox_SplitWord3.Text);
                AddRefrenceList = AddRefrenceList.Select(str => cmn_textBox_Reference.Text + @"\" + str + cmn_textBox_AddListSuffix.Text).ToArray();

                String[] SumRefrenceList = new String[RefrenceCandidateFolders.Length + AddRefrenceList.Length];
                RefrenceCandidateFolders.CopyTo(SumRefrenceList, 0);
                AddRefrenceList.CopyTo(SumRefrenceList, RefrenceCandidateFolders.Length);
                RefrenceCandidateFolders = SumRefrenceList;
            }

            // コンボボックス更新
            UpdateRenameComboBox();
            UpdateMoveDestDirComboBox();
        }

        private void DeleteDuplicate(String[] LegacyArray, ref String[] NewArray, String Delimiter)
        {
            var NewList = new List<String>();
            NewList.AddRange(NewArray);

            for (int i = 0; i < NewList.Count; i++)
            {
                for (int j = 0; j < LegacyArray.Length; j++)
                {
                    if (-1 != LegacyArray[j].IndexOf(NewList[i]))
                    {
                        NewList.RemoveAt(i);
                        i--;    // TODO：他にもっと良いやり方があるはず
                        break;
                    }
                }
            }

            //String[] OrganizeArray;
            //OrganizeArray = NewList.ToArray();
            NewArray = NewList.ToArray();
        }

        private void rd_comboBox_MergeWord_DropDown(object sender, EventArgs e)
        {
            UpdateRenameComboBox();
        }

        private void pf_comboBox_MoveDestDirName_DropDown(object sender, EventArgs e)
        {
            UpdateMoveDestDirComboBox();
        }

        private void pf_textBox_SearchTitleLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.Enter)
            {
                MovePartitionFile();
            }
        }

        private void pf_checkBox_CreateNewDir_CheckedChanged(object sender, EventArgs e)
        {
            ListupTargetMoveDirectory();
        }

        private void rd_comboBox_RenameDir_KeyDown(object sender, KeyEventArgs e)
        {

            util.ExecutePath(rd_comboBox_RenameDir.Text, e);
        }
    }
}
