using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

            public Table(int idx, string srcName, string destName)
            {
                Group = idx;
                SrcName = srcName;
                DestName = destName;
            }
        }

        public enum INDEX_COUNTER
        {
            INCREMENT,
            DECREMENT
        };


        public void UpdateIdx(INDEX_COUNTER cnt)
        {
            switch (cnt)
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
            UpdateIdx(INDEX_COUNTER.DECREMENT);
            for (int i = AllList.Count - 1; i > 0; i--)
            {
                if (!SubList(AllList, i))
                {
                    break;
                }
            }

            ResultDump();
        }

        private void AddList(List<Table> arr, int num)
        {
            arr.Add(new Table(LastIdx, "Source" + num.ToString(), "Destination" + num.ToString()));
        }

        private Boolean SubList(List<Table> arr, int num)
        {
            if (arr[num].Group == LastIdx)
            {
                arr.RemoveAt(num);
                return true;
            }
            return false;
        }

        private void ResultDump()
        {
            String Result = "";
            for (int i = 0; i < AllList.Count; i++)
            {
                Result += "[" + i.ToString() + "]" + Environment.NewLine;
                Result += "  Group=    " + AllList[i].Group + Environment.NewLine;
                Result += "  SrcName= " + AllList[i].SrcName + Environment.NewLine;
                Result += "  DestName=" + AllList[i].DestName + Environment.NewLine;
            }
            MessageBox.Show(Result, "Tableの中身");
        }
    }
}
