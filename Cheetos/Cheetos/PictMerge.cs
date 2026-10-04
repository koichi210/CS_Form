using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using Picture;

namespace Cheetos
{
    public class PictMerge
    {
        public String BackUpDirPath { get; set; } = String.Empty;
        public String SourceFolderPath { get; set; } = String.Empty;
        public String SourceFile1Prefix { get; set; } = String.Empty;
        public String SourceFile2Prefix { get; set; } = String.Empty;
        public String[] TrimHeightAry { get; set; } = null;

        private String _targetFileName = String.Empty;
        private String _prefix1 = String.Empty;
        private String _prefix2 = String.Empty;
        private String _sourceFileFullName = String.Empty;
        private String _sourceBackUpFullName = String.Empty;
        private String _mergeFileFullName = String.Empty;
        private String _mergeBackUpFullName = String.Empty;
        private readonly StringBuilder _errorMessages = new StringBuilder();

        // 「開始,終了」の区切り文字(行ごとに配列を作り直さないよう共有する)
        private static readonly String[] _trimHeightSeparator = { "," };

        public bool SetTargetFileName(String targetFileName)
        {
            if (targetFileName == String.Empty)
            {
                // 空行だったら処理しない
                return false;
            }
            _targetFileName = targetFileName;
            return true;
        }

        public bool IsProcTarget()
        {
            // 文字列が部分一致したら処理
            _prefix1 = SourceFile1Prefix + Path.GetExtension(_targetFileName);
            _prefix2 = SourceFile2Prefix + Path.GetExtension(_targetFileName);
            // 含まれていなければ対象外のファイル
            return _targetFileName.IndexOf(_prefix1) != -1;
        }

        public bool BackUpSourceFile()
        {
            _sourceFileFullName = SourceFolderPath + @"\" + _targetFileName;
            _sourceBackUpFullName = BackUpDirPath + @"\" + _targetFileName;

            if (!File.Exists(_sourceFileFullName))
            {
                _errorMessages.Append("ファイルが存在しません。").Append(_sourceFileFullName).AppendLine();
                return false;
            }
            File.Copy(_sourceFileFullName, _sourceBackUpFullName, true);
            return true;
        }

        public bool ResolveMergeFilePath()
        {
            String mergeFileName = _targetFileName.Replace(_prefix1, _prefix2);
            _mergeFileFullName = SourceFolderPath + @"\" + mergeFileName;
            _mergeBackUpFullName = BackUpDirPath + @"\" + mergeFileName;

            if (!File.Exists(_mergeFileFullName))
            {
                _errorMessages.Append("ファイルが存在しません。").Append(_mergeFileFullName).AppendLine();
                return false;
            }
            return true;
        }

        public int GetHeight(String heightText, int defaultHeight = 0)
        {
            if (heightText == "-")
            {
                return defaultHeight;
            }
            return int.Parse(heightText);
        }

        // 「開始,終了」(各値は数値か"-")の形になっていない行を返す。空行は対象外。
        // 確認ダイアログはバックグラウンドから出せないため、結合開始前にUIスレッドで使う
        public static String[] FindInvalidTrimHeights(String[] trimHeights)
        {
            return trimHeights.Where(line => line != String.Empty && !IsValidTrimHeight(line)).ToArray();
        }

        private static bool IsValidTrimHeight(String line)
        {
            string[] heightRange = line.Split(_trimHeightSeparator, StringSplitOptions.None);
            int height;
            return heightRange.Length == 2
                && heightRange.All(text => text == "-" || int.TryParse(text, out height));
        }

        public bool MergeExecute()
        {
            // キャンバス作成
            using (PicEdit mrg = new PicEdit(_sourceBackUpFullName))
            {
                mrg.CreateSourceImg(_mergeFileFullName);
                Size sz = mrg.GetCanvasSize();

                foreach (String line in TrimHeightAry)
                {
                    // 不正な行は開始前にUIスレッドで確認済み(FindInvalidTrimHeights)なので、ここでは飛ばすだけ
                    if (line == String.Empty || !IsValidTrimHeight(line))
                    {
                        continue;
                    }

                    string[] heightRange = line.Split(_trimHeightSeparator, StringSplitOptions.None);
                    int startHeight = GetHeight(heightRange[0], 0);
                    int endHeight = GetHeight(heightRange[1], sz.Height);
                    if (sz.Height < startHeight)
                    {
                        // 画像サイズよりも指定されたサイズが大きい
                        break;
                    }
                    Rectangle cutParam = new Rectangle(0, startHeight, sz.Width, endHeight - startHeight);
                    mrg.MergeExec(cutParam);
                }

                mrg.ReleaseSourceImg();

                // キャンバス保存
                mrg.SaveCanvas(_sourceFileFullName);

                // マージ元ファイルをバックアップへ移動
                File.Move(_mergeFileFullName, _mergeBackUpFullName);
            }
            return true;
        }

        public String GetErrorMessage()
        {
            return _errorMessages.ToString();
        }
    }

    // PictMerge
    partial class Cheetos
    {
        private void MergeExec()
        {
            _debugLog.WriteData("MergeExec_Click" + Environment.NewLine, false);

            // キャンセル
            if (bkgWorkerMerge.IsBusy)
            {
                bkgWorkerMerge.CancelAsync();
                return;
            }

            String backUpDirPath = pm_SourceFolderPath.Text + @"\" + @"Bk_Merge";
            if (!_fio.EnsureDirectory(backUpDirPath))
            {
                MessageBox.Show("無効なフォルダパスです。\n" + backUpDirPath);
                return;
            }

            // 切断基準となる高さ。書式の確認はバックグラウンドでは聞けないため、開始前にここで1回だけ行う
            String[] trimHeights = pm_TrimmingHeight.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            String[] invalidTrimHeights = global::Cheetos.PictMerge.FindInvalidTrimHeights(trimHeights);
            if (invalidTrimHeights.Length > 0)
            {
                DialogResult dr = MessageBox.Show("フォーマットが不正な行があります。" + Environment.NewLine
                    + "[" + String.Join("] [", invalidTrimHeights) + "]" + Environment.NewLine
                    + "不正な行を飛ばして結合しますか？",
                    "Error",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);
                if (dr != DialogResult.Yes)
                {
                    return;
                }
            }

            InitProgressBar(pm_ListBox_ListUp.SelectedItems.Count);

            // 別スレッドを非同期実行
            MergeWorkerParam param = new MergeWorkerParam
            {
                BackUpDirPath = backUpDirPath,
                SourceFolderPath = pm_SourceFolderPath.Text,
                SourceFile1Prefix = pm_SourceFile1Prefix.Text,
                SourceFile2Prefix = pm_SourceFile2Prefix.Text,
            };

            _debugLog.WriteData("BackUpDirPath = " + backUpDirPath);
            _debugLog.WriteData("SourceFolderPath = " + pm_SourceFolderPath.Text);
            _debugLog.WriteData("Prefix1 = " + pm_SourceFile1Prefix.Text);
            _debugLog.WriteData("Prefix2 = " + pm_SourceFile2Prefix.Text);

            param.TrimHeightAry = trimHeights;

            // ListBoxの値を配列で取得
            param.TargetFileNames = _util.GetStrArrayFromListBox(pm_ListBox_ListUp.SelectedItems);

            SetStartTime();
            pm_Button_Merge.Text = "中断";
            bkgWorkerMerge.RunWorkerAsync(param);   // ⇒DoWork()
        }

        private void ListUpPictMerge()
        {
            ListUpFolderFiles(pm_SourceFolderPath, pm_ListBox_ListUp);
        }

        private void bkgWorkerMerge_DoWork(object sender, DoWorkEventArgs e)
        {
            // 別スレッドで実行されるため、このメソッドではGUIを操作してはいけない

            // senderの値はbgWorkerの値と同じ
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            MergeWorkerParam param = (MergeWorkerParam)e.Argument;

            PictMerge pm = new PictMerge
            {
                BackUpDirPath = param.BackUpDirPath,
                SourceFolderPath = param.SourceFolderPath,
                SourceFile1Prefix = param.SourceFile1Prefix,
                SourceFile2Prefix = param.SourceFile2Prefix,
                TrimHeightAry = param.TrimHeightAry,
            };
            String[] targetFileNames = param.TargetFileNames;

            for (int itemIdx = 0; itemIdx < targetFileNames.Length; itemIdx++)
            {
                if (!pm.SetTargetFileName(targetFileNames[itemIdx]))
                {
                    continue;
                }

                if (!pm.IsProcTarget())
                {
                    continue;
                }

                if (!pm.BackUpSourceFile())
                {
                    continue;
                }

                if (!pm.ResolveMergeFilePath())
                {
                    continue;
                }

                pm.MergeExecute();
                worker.ReportProgress(itemIdx);      // ⇒ProgressChanged()
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
            e.Result = pm.GetErrorMessage();
            worker.ReportProgress(targetFileNames.Length);      // ⇒ProgressChanged()
        }

        private void bkgWorkerMerge_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("キャンセルされました");
                // この場合はe.Resultにはアクセスできない
            }
            else if (e.Error != null)
            {
                MessageBox.Show("エラーが発生しました[" + e.Error.Message + "]");
            }
            else
            {
                String result = e.Result.ToString();
                if (result != String.Empty)
                {
                    MessageBox.Show("処理中にエラーが発生しました。" + Environment.NewLine + result,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            TextBox_Status.Text += " 完了";
            pm_Button_Merge.Text = "結合";

            // リスト更新
            ListUpPictMerge();
        }
    }
}
