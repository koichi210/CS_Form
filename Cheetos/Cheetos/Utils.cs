using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace Cheetos
{
    public class Utils : StcUtils
    {
        public bool SelectLoadFileName(ref String LoadFileName)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = LoadFileName;
            ofd.Filter = "XMLファイル(*.xml)|*.xml|すべてのファイル(*.*)|*.*";
            ofd.Title = "読み込む設定ファイルを選択してください";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadFileName = ofd.FileName;
                return true;
            }
            return false;
        }

        public bool SelectSaveFileName(ref String SaveFileName)
        {
            DialogResult DlgResult = MessageBox.Show(
                "現在の設定ファイルに保存しますか",
                "保存ファイル名の選択",
                MessageBoxButtons.YesNo);

            // 任意のファイル名を指定
            if (DlgResult == DialogResult.No)
            {
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.FileName = SaveFileName;
                ofd.Filter = "XMLファイル(*.xml)|*.xml|すべてのファイル(*.*)|*.*";
                ofd.Title = "保存する設定ファイルを選択してください";

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                SaveFileName = ofd.FileName;
            }
            return true;
        }

        public void ExecutePathWithWait(String Path)
        {
            System.Diagnostics.Process hProcess = System.Diagnostics.Process.Start(Path);
            hProcess.WaitForExit(); // 処理が終わるまで待つ
            hProcess.Close();
            hProcess.Dispose();
        }

        // ConboBoxにアイテム追加
        public void AddComboBoxItem(ComboBox ComboBoxControl, String TopDir, String SerchDir, String TargetFile, String ExcludeFile)
        {
            String TargetDir = TopDir;

            if (SerchDir != String.Empty)
            {
                TargetDir  += @"\" + SerchDir;
            }

            if (Directory.Exists(TargetDir) == false)
            {
                return;
            }

            // ファイルをリストアップ
            String[] files = Directory.GetFiles(TargetDir, TargetFile, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                // 除外ファイルが見つかったらスキップ
                if (FoundFile(files[i], ExcludeFile))
                {
                    continue;
                }

                String FileName = files[i].Remove(0, TopDir.Length + 1);   // "\\"の分を1加算
                ComboBoxControl.Items.Add(FileName);
            }
        }

        // Linuxパスか判定
        public bool IsLinuxPath(String Path)
        {
            // "\"が見つかったらWindowsPathなので、置換対象と判断
            if (Path.IndexOf(@"\") != -1)
            {
                return false;
            }
            return true;
        }

        // パス指定方法を変更 C:\ → /cygdrive/c
        public String ChangeWindowsPath2CygwinPath(String Path)
        {
            // "\"が見つかったらWindowsPathなので、置換対象と判断
            if (Path.IndexOf(@"\") != -1)
            {
                bool IsSetDrv = false;
                if (Path.IndexOf(@":") != -1)
                {
                    IsSetDrv = true;
                }
                Path = Path.Replace(@":", @"");

                if (IsSetDrv)
                {
                    Path = "/cygdrive/" + Path;
                }

                // \ → / に置換
                Path = Path.Replace(@"\", @"/");
            }

            return Path;
        }

        // パス指定方法を変更 /cygdrive/c → C:\
        public String ChangeCygwinPath2WindowsPath(String Path)
        {
            // "/"が見つかったらCygwinPathなので、置換対象と判断
            if (Path.IndexOf(@"/") != -1)
            {
                bool IsSetDrv = false;
                if (Path.IndexOf(@"/cygdrive/") != -1)
                {
                    IsSetDrv = true;
                }
                Path = Path.Replace(@"/cygdrive/", @"");

                if (IsSetDrv)
                {
                    Path.Insert(1, @":");
                }

                // / → \ に置換
                Path = Path.Replace(@"/", @"\");
            }

            return Path;
        }

        public String GetConfigFilePath(String InPath)
        {
            String OutPath = "";
            if (InPath != String.Empty)
            {
                OutPath = "../" + InPath;
            }
            return OutPath;
        }

        // ファイル削除
        public bool DeleteAnyFile(String TargetName)
        {
            String DirName = Path.GetDirectoryName(TargetName);
            String FileName = Path.GetFileName(TargetName);

            if (FileName.IndexOf("*") == -1)
            {
                // ワイルドカードの指定が無かった終了
                return true;
            }

            if (!Directory.Exists(DirName))
            {
                // 無効なディレクトリだったら終了
                return true;
            }

            // ファイルリストアップ
            bool DeleteComplete = true;
            String[] files = Directory.GetFiles(DirName, FileName);
            for (int i = 0; i < files.Length; i++)
            {
                if (!DeleteFileWithRemoveReadonlyAttribute(files[i]))
                {
                    DeleteComplete = false;
                }
            }

            return DeleteComplete;
        }

        // テンポラリファイル作成
        public String CreateTempFile(String Ext= "")
        {
            String str = Path.GetTempFileName();
            if (Ext != String.Empty)
            {
                str = str.Replace(".tmp", "." + Ext);
            }

            return str;
        }

        // フォルダ削除
        public bool DeleteDirectoryAndFile(String DeletePath)
        {
            bool DeleteComplete = true;

            if (DeletePath.IndexOf("*") != -1)
            {
                // ワイルドカードの指定があった
                DeleteComplete = DeleteAnyFile(DeletePath);
            }
            else if (Directory.Exists(DeletePath))
            {
                System.IO.DirectoryInfo delDir = new System.IO.DirectoryInfo(DeletePath);
                try
                {
                    RemoveReadonlyAttribute(delDir);
                    delDir.Delete(true);
                }
                catch (Exception)
                {
                    DeleteComplete = false;
                }
            }
            else if (File.Exists(DeletePath))
            {
                DeleteComplete = DeleteFileWithRemoveReadonlyAttribute(DeletePath);
            }
            //else// Any
            //{
            //    DeleteComplete = DeleteAnyFile(DeletePath);
            //}

            return DeleteComplete;
        }

        // ファイル削除（読み取り属性解除）
        public bool DeleteFileWithRemoveReadonlyAttribute(String DeleteFile)
        {
            bool DeleteComplete = true;

            FileInfo fi = new FileInfo(DeleteFile);
            if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                fi.Attributes = FileAttributes.Normal;
            }

            try
            {
                File.Delete(DeleteFile);
            }
            catch (Exception)
            {
                DeleteComplete = false;
            }

            return DeleteComplete;
        }

        // 読み取り属性解除
        public void RemoveReadonlyAttribute(DirectoryInfo dirInfo)
        {
            //フォルダ属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                dirInfo.Attributes = FileAttributes.Normal;
            }

            //フォルダ内のファイル属性を変更
            foreach (FileInfo fi in dirInfo.GetFiles())
            {
                if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    fi.Attributes = FileAttributes.Normal;
                }
            }

            //サブフォルダの属性も変更
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
            {
                RemoveReadonlyAttribute(di);
            }
        }

        // 目的のファイルか判断
        public bool FoundFile(String SourceName, String SerchName)
        {
            // 検索対象が無いので、見つからない
            if (SerchName == String.Empty)
            {
                return false;
            }

            // 検索したけど、見つからない
            if (SourceName.IndexOf(SerchName) == -1)
            {
                return false;
            }

            return true;
       }

        public String ChangeNewLineCodeLF2CRLF(String Source)
        {
            String Dest = Source.Replace("\n", "\r\n");
            return Dest;
        }

        public void ChangeStringCodeEUC2SJIS(String InFileName, String OutFileName)
        {
            // TODO：本関数を汎用的かつシンプルにしたい
            Byte[] dat = new Byte[] { };
            System.IO.Stream sr = null;
            System.IO.Stream sw = null;
            System.IO.BinaryReader br = null;
            System.IO.BinaryWriter bw = null;

            // 入力ファイルをバイナリ形式で入力
            sr = System.IO.File.Open(InFileName,
                System.IO.FileMode.Open, System.IO.FileAccess.Read);
            br = new System.IO.BinaryReader(sr);
            Array.Resize<Byte>(ref dat, (int)sr.Length);
            dat = br.ReadBytes((int)sr.Length);
            br.Close();
            sr.Close();

            String uni;
            uni = System.Text.Encoding.GetEncoding("EUC-JP").GetString(dat);
            dat = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(uni);

            sw = System.IO.File.Open(OutFileName,
                System.IO.FileMode.Create, System.IO.FileAccess.Write);
            bw = new System.IO.BinaryWriter(sw);
            bw.Write(dat);

            bw.Close();
            sw.Close();
            br.Close();
            sr.Close();
        }
    }
}
