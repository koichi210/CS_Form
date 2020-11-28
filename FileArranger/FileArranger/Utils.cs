using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using StandardTemplate;

namespace FileArranger
{
    class Utils : StcUtils
    {
        // フォルダ名の重複回避
        public void CreateFolderNameOverLapShirk(ref String TargetPath, int LoopIdx)
        {
            // フォルダが存在しなければ何もしない
            if (!Directory.Exists(TargetPath))
            {
                return;
            }
            TargetPath = TargetPath + "_Cnt" + LoopIdx.ToString() + "_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }

        // ファイル名の重複回避
        public Boolean CreateFileNameOverLapShirk(ref String TargetPath, int LoopIdx)
        {
            if (!File.Exists(TargetPath) && !Directory.Exists(TargetPath))
            {
                return true;
            }
            TargetPath = TargetPath + "_Cnt" + LoopIdx.ToString() + "_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            return false;
        }

        public String CreateNewFolderName(String SrcName, String TrimName = "", Boolean IsReverse = false)
        {
            String NewFolderName = SrcName;

            // SrcTrimNameが設定されていたら、特定の文字列で区切る
            if (TrimName != String.Empty)
            {
                int FileNameidx;
                if (!IsReverse)
                {
                    FileNameidx = SrcName.IndexOf(TrimName);
                }
                else
                {
                    FileNameidx = SrcName.LastIndexOf(TrimName);
                }

                if (0 <= FileNameidx)
                {
                    NewFolderName = SrcName.Substring(0, FileNameidx);
                }
            }

            return NewFolderName;
        }

        // 選択されているリストビューの中から目的の文字列を探す
        public int GetStringFromListViewInSelect(ListView LvCtrl, int SrcSubItemIdx, String SrcName, String SrcTrimName = "", Boolean IsReverse = false)
        {
            String SerchName = SrcName;
            int SameIdx = -1;

            // SrcTrimNameが設定されていたら、特定の文字列で区切る
            if (SrcTrimName != String.Empty)
            {
                int FileNameidx;
                if (!IsReverse)
                {
                    FileNameidx = SrcName.IndexOf(SrcTrimName);
                }
                else
                {
                    FileNameidx = SrcName.LastIndexOf(SrcTrimName);
                }

                if (0 <= FileNameidx)
                {
                    SerchName = SrcName.Substring(0, FileNameidx);
                }
            }

            for (int i = 0; i < LvCtrl.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = LvCtrl.SelectedItems[i].Index;
                String LvString = LvCtrl.Items[idx].SubItems[SrcSubItemIdx].Text;

                if (LvString.IndexOf(SerchName) != -1)
                {
                    SameIdx = idx;
                    break;
                }
            }

            return SameIdx;
        }
    }
}
