using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Class_List
{
    public partial class Form1 : Form
    {
        public struct Table
        {
            public int Group;
            public String SrcName;
            public String DestName;

            public Table(int group, string srcName, string destName)
            {
                Group = group;
                SrcName = srcName;
                DestName = destName;
            }
        }

        public enum INDEX_COUNTER
        {
            INCREMENT,
            DECREMENT
        };


        public void UpdateIdx(INDEX_COUNTER counter)
        {
            switch (counter)
            {
                case INDEX_COUNTER.INCREMENT:
                    LastIdx++;
                    break;

                case INDEX_COUNTER.DECREMENT:
                    if (LastIdx > 0)
                    {
                        LastIdx--;
                    }
                    break;
            }
        }

        private int LastIdx = 0;
        private List<Table> AllList = new List<Table>();

        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
            InitializeToolTips();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // 対象外の欄は無し(回数の入力欄1つだけ)
        private void InitializePlaceholders()
        {
            textBox_num.PlaceholderText = "例: 3";
        }

        // マウスを乗せた時に出す説明(ツールチップ)。ラベルやボタン名だけでは
        // 意味・単位・書式・注意点が分かりにくい所にだけ付けている
        private void InitializeToolTips()
        {
            ToolTip toolTip = new ToolTip { AutoPopDelay = 15000 };
            this.Disposed += (s, e) => toolTip.Dispose();

            toolTip.SetToolTip(textBox_num, "Addを1回押した時にリストへ追加する要素の数(整数)。まとめて1つのグループになる");
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < int.Parse(textBox_num.Text); i++)
            {
                AddList(AllList, i);
            }
            UpdateIdx(INDEX_COUNTER.INCREMENT);

            ResultDump();
        }

        private void buttonRestore_Click(object sender, EventArgs e)
        {
            RemoveLastGroup();
            ResultDump();
        }

        // 直前に追加した1グループ分の要素を削除する(Restoreボタンの本体ロジック)。
        // ループは末尾からインデックス0まで見る必要がある(先頭グループだけで追加した直後に
        // Restoreすると、i > 0ではインデックス0が除外されて何も消えないバグがあったため、
        // i >= 0 にして0番目も対象に含める。RemoveAtで前方から削除するのでインデックスが
        // ずれないよう、ループは後ろから前へ進める
        private void RemoveLastGroup()
        {
            UpdateIdx(INDEX_COUNTER.DECREMENT);
            for (int i = AllList.Count - 1; i >= 0; i--)
            {
                if (!RemoveIfLastGroup(AllList, i))
                {
                    break;
                }
            }
        }

        private void AddList(List<Table> list, int num)
        {
            list.Add(new Table(LastIdx, "Source" + num.ToString(), "Destination" + num.ToString()));
        }

        private Boolean RemoveIfLastGroup(List<Table> list, int index)
        {
            if (list[index].Group == LastIdx)
            {
                list.RemoveAt(index);
                return true;
            }
            return false;
        }

        private void ResultDump()
        {
            String result = "";
            for (int i = 0; i < AllList.Count; i++)
            {
                result += "[" + i.ToString() + "]" + Environment.NewLine;
                result += "  Group=    " + AllList[i].Group + Environment.NewLine;
                result += "  SrcName= " + AllList[i].SrcName + Environment.NewLine;
                result += "  DestName=" + AllList[i].DestName + Environment.NewLine;
            }
            MessageBox.Show(result, "Tableの中身");
        }
    }
}
