using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using StandardTemplate;

namespace FFEdit
{
    class FileMng : StcProcessMemory
    {
        private readonly StcUtils _util = new StcUtils();
        private readonly StcFileInputOutput _fileIO = new StcFileInputOutput();

        // 直前の操作1回分を、記録しておいた移動元へ戻す。
        // Rename/Functionの両方に同じ実装が置かれていたためここへ集約した
        public Boolean RestoreAll()
        {
            if (!DecrementSerialNumber())
            {
                return false;
            }

            while (HasRestoreItem())
            {
                String srcName = "";
                String destName = "";
                PopRestoreItem(ref srcName, ref destName);
                Move(destName, srcName);
            }

            return true;
        }

        public Boolean Move(String srcName, String destName, Boolean showErrorPopup = false)
        {
            try
            {
                if (File.Exists(srcName))
                {
                    File.Move(srcName, destName);
                    return true;
                }
                if (Directory.Exists(srcName))
                {
                    Directory.Move(srcName, destName);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                if (showErrorPopup)
                {
                    ShowPathMessage("指定パスが移動できませんでした。", srcName, destName);
                }
                return false;
            }
        }

        public Boolean Copy(String srcName, String destName, Boolean showErrorPopup = false)
        {
            try
            {
                if (File.Exists(srcName))
                {
                    File.Copy(srcName, destName);
                    return true;
                }
                if (Directory.Exists(srcName))
                {
                    ShowPathMessage("ディレクトリコピーは未対応です。", srcName, destName);
                }
                return false;
            }
            catch (Exception)
            {
                if (showErrorPopup)
                {
                    ShowPathMessage("指定パスがコピーできませんでした。", srcName, destName);
                }
                return false;
            }
        }

        private static void ShowPathMessage(String message, String srcName, String destName)
        {
            MessageBox.Show(message + Environment.NewLine +
                srcName + Environment.NewLine +
                destName);
        }

        // 失敗した移動/コピーの移動元・移動先を、エラー画面(ErrorMsg)に出す一覧へ追記する
        // (Rename/Functionで同じ書式を使う)
        public static void AppendError(StringBuilder errorList, String srcName, String destName)
        {
            errorList.Append("Src=").Append(srcName).Append(Environment.NewLine);
            errorList.Append("Dst=").Append(destName).Append(Environment.NewLine);
            errorList.Append(Environment.NewLine);
        }

        public void DeleteBlankDir(String dirPath)
        {
            // dir /s は親フォルダを子より先に列挙するため、そのままだと「空フォルダだけを含むフォルダ」
            // (a\b が両方空など)は、rd の時点ではまだ子が残っていて消えなかった。
            // 逆順に並べ替えて、子(深い方)から先に rd する。sortはWindows標準のものをフルパスで使う
            // (PATHでGitのsort.exe(GNU sort)が先に見つかると /r が効かず、何も消えなくなるため)
            String command = @"for /f ""delims="" %%d in ('dir """ + dirPath + @""" /ad /b /s ^| %SystemRoot%\System32\sort.exe /r') do rd ""%%d""" + Environment.NewLine;

            String batchFile = _fileIO.CreateTempFile("bat");
            _fileIO.CreateFile(batchFile, command);

            // 以前は完了を待たずに戻っていたため、直後の一覧更新に削除前のフォルダが残り、
            // 一時バッチファイルとプロセスのハンドルも残ったままだった
            _util.ExecutePathWithWait(batchFile, "", true);
            File.Delete(batchFile);
        }
    }
}
