using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            targetPath = AppendConflictSuffix(targetPath, loopIdx);
        }

        // ファイル名の重複回避(同名のファイル/フォルダが無ければtrue。あれば名前を変えてfalse)
        public Boolean AvoidFileNameConflict(ref String targetPath, int loopIdx)
        {
            if (!File.Exists(targetPath) && !Directory.Exists(targetPath))
            {
                return true;
            }
            targetPath = AppendConflictSuffix(targetPath, loopIdx);
            return false;
        }

        // 重複回避用に、名前の末尾へ「_Cnt連番_日時」を付ける
        private static String AppendConflictSuffix(String targetPath, int loopIdx)
        {
            return targetPath + "_Cnt" + loopIdx.ToString() + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }

        public String CreateNewFolderName(String srcName, String trimName = "", Boolean isReverse = false)
        {
            return TrimAtSeparator(srcName, trimName, isReverse);
        }

        // 選択されているリストビューの項目の中から目的の文字列を含むものを探し、そのIdxを返す(無ければ-1)
        public int FindSelectedRowIndex(ListView listView, int subItemIdx, String srcName, String trimName = "", Boolean isReverse = false)
        {
            ListViewItem found = FindItem(GetSelectedItems(listView), subItemIdx, srcName, trimName, isReverse);
            return found != null ? found.Index : -1;
        }

        // items(選択項目のスナップショット等)の中から目的の文字列を含む最初の項目を探す(無ければnull)
        public ListViewItem FindItem(IEnumerable<ListViewItem> items, int subItemIdx, String srcName, String trimName = "", Boolean isReverse = false)
        {
            String searchName = TrimAtSeparator(srcName, trimName, isReverse);
            return items.FirstOrDefault(item => item.SubItems[subItemIdx].Text.IndexOf(searchName) != -1);
        }

        // ListViewの選択項目を一度に取り出す。
        // .NET FrameworkのSelectedItemsは、Countや[i]にアクセスするたびに選択項目の配列を
        // Win32から丸ごと取り直すため、for (i < SelectedItems.Count) { SelectedItems[i] }と書くと
        // 選択数の2乗の処理量になる(数百件選ぶと目に見えて重い)。列挙なら1回で済む
        public static List<ListViewItem> GetSelectedItems(ListView listView)
        {
            return listView.SelectedItems.Cast<ListViewItem>().ToList();
        }

        // ListBoxの選択項目の表示名を一度に取り出す。
        // SelectedItems[i]は毎回先頭から選択項目を数え直すため、インデックスでのループは選択数の2乗になる
        public static List<String> GetSelectedNames(ListBox listBox)
        {
            return listBox.SelectedItems.Cast<object>().Select(item => item.ToString()).ToList();
        }

        // 指定した項目の指定列にまとめて同じ文字列を入れる(1件ずつ再描画しないようBeginUpdateで囲む)
        public static void SetSubItemText(ListView listView, IEnumerable<ListViewItem> items, String text, params int[] subItemIdxes)
        {
            listView.BeginUpdate();
            foreach (ListViewItem item in items)
            {
                foreach (int subItemIdx in subItemIdxes)
                {
                    item.SubItems[subItemIdx].Text = text;
                }
            }
            listView.EndUpdate();
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
