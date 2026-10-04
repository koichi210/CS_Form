using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFEdit
{
    class Function
    {
        public enum FunctionType
        {
            DelEmptyDir,
            Copy,
            Move,
        }

        public String BaseDir { get; set; } = "";
        public String DestDir { get; set; } = "";
        public List<String> FileList { get; set; }
        public FunctionType Type { get; set; }

        private readonly FileMng _fileMng = new FileMng();

        public Boolean Restore()
        {
            return _fileMng.RestoreAll();
        }

        public String Execute()
        {
            StringBuilder errorList = new StringBuilder();

            for (int i = 0; i < FileList.Count; i++)
            {
                String srcName = BaseDir + '\\' + FileList[i];
                if (Type == FunctionType.DelEmptyDir)
                {
                    _fileMng.DeleteBlankDir(srcName);
                    continue;
                }

                // 移動/コピー(サブフォルダ内の項目も移動先の直下に置く)
                String destName = DestDir + '\\' + Path.GetFileName(FileList[i]);
                if (srcName == destName)
                {
                    // 同一だったら処理しない
                    continue;
                }

                Directory.CreateDirectory(DestDir);
                Boolean isSuccess;
                if (Type == FunctionType.Move)
                {
                    isSuccess = _fileMng.Move(srcName, destName);
                    if (isSuccess)
                    {
                        // 復元用に設定を覚えておく(コピーのときは処理を覚えない)
                        _fileMng.AddRestoreItem(srcName, destName);
                    }
                }
                else
                {
                    isSuccess = _fileMng.Copy(srcName, destName);
                }

                if (!isSuccess)
                {
                    FileMng.AppendError(errorList, srcName, destName);
                }
            }
            _fileMng.IncrementSerialNumber();

            return errorList.ToString();
        }
    }
}
