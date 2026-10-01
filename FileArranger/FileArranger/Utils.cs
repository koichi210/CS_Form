using System;
using System.IO;
using System.Windows.Forms;
using StandardTemplate;

namespace FileArranger
{
    class Utils : StcUtils
    {
        // フォルダ名の重複回避(同名フォルダがあれば、名前の末尾に連番と日時を付ける)
        public void AvoidFolderNameConflict(ref String targetPath, int loopIdx)
        {
            // フォルダが存在しなければ何もしない
            if (!Directory.Exists(targetPath))
            {
                return;
            }
            targetPath = targetPath + "_Cnt" + loopIdx.ToString() + "_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }

        // ファイル名の重複回避(同名のファイル/フォルダが無ければtrue。あれば名前を変えてfalse)
        public Boolean AvoidFileNameConflict(ref String targetPath, int loopIdx)
        {
            if (!File.Exists(targetPath) && !Directory.Exists(targetPath))
            {
                return true;
            }
            targetPath = targetPath + "_Cnt" + loopIdx.ToString() + "_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            return false;
        }

        public String CreateNewFolderName(String srcName, String trimName = "", Boolean isReverse = false)
        {
            return TrimAtSeparator(srcName, trimName, isReverse);
        }

        // 選択されているリストビューの項目の中から目的の文字列を含むものを探し、そのIdxを返す(無ければ-1)
        public int FindSelectedRowIndex(ListView listView, int subItemIdx, String srcName, String trimName = "", Boolean isReverse = false)
        {
            String searchName = TrimAtSeparator(srcName, trimName, isReverse);

            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                // 参照しているListViewのIdx
                int idx = listView.SelectedItems[i].Index;
                String itemText = listView.Items[idx].SubItems[subItemIdx].Text;

                if (itemText.IndexOf(searchName) != -1)
                {
                    return idx;
                }
            }

            return -1;
        }

        // trimNameが設定されていたら、その文字列より前の部分を切り出す
        // (見つからない・未設定なら元の文字列のまま)
        private static String TrimAtSeparator(String srcName, String trimName, Boolean isReverse)
        {
            if (trimName == String.Empty)
            {
                return srcName;
            }

            int trimIdx = isReverse ? srcName.LastIndexOf(trimName) : srcName.IndexOf(trimName);
            if (0 <= trimIdx)
            {
                return srcName.Substring(0, trimIdx);
            }

            return srcName;
        }
    }
}
