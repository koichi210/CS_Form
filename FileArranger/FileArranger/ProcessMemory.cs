using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FileArranger
{
    class ProcessMemory
    {
        public struct RestoreStruct
        {
            public int SerialNumber;    // 管理番号
            public String SrcName;      // 記憶するデータ
            public String DestName;     // 記憶するデータ

            public RestoreStruct(int serial_number, string src_name, string dest_name)
            {
                SerialNumber = serial_number;
                SrcName = src_name;
                DestName = dest_name;
            }
        }

        private int CurrentIdx = 0;
        private List<RestoreStruct> RestoreList = new List<RestoreStruct>();

        public void IncrementRegistNumber()
        {
            CurrentIdx++;
        }

        public Boolean DecrementRegistNumber()
        {
            Boolean IsSuccess = false;
            if (CurrentIdx > 0)
            {
                CurrentIdx--;
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
            }

            return IsSuccess;
        }

        public void SetRestoreList(String SrcName, String DestName)
        {
            RestoreList.Add(new RestoreStruct(CurrentIdx, SrcName, DestName));
        }

        public void GetRestoreList(ref String SrcName, ref String DestName)
        {
            int LastIdx = RestoreList.Count - 1;

            SrcName = RestoreList[LastIdx].SrcName;
            DestName = RestoreList[LastIdx].DestName;
            RestoreList.RemoveAt(LastIdx);
        }

        public Boolean IsExistRestoreList()
        {
            Boolean IsExist = false;

            if (RestoreList.Count > 0)
            {
                int ListEndIdx = RestoreList.Count - 1;
                if (RestoreList[ListEndIdx].SerialNumber == CurrentIdx)
                {
                    IsExist = true;
                }
            }

            return IsExist;
        }
    }
}
