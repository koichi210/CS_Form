using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StandardTemplate;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace ToyingData
{
    public partial class Form1 : Form
    {
        StcUtils util = new StcUtils();
        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.ToyingData;
        }

        private void textBox_Source_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void textBox_Dest_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private void textBox_Source_DragEnter(object sender, DragEventArgs e)
        {
            util.SetDragFile(e);
        }

        private void textBox_Source_DragDrop(object sender, DragEventArgs e)
        {
            String[] FileList = (String[])e.Data.GetData(typeof(String));
            textBox_Source.Text = util.ChangeStrArray2Linear(FileList, Environment.NewLine);
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {
            String[] SourceArray = util.ChangeStrLinear2Array(textBox_Source.Text, Environment.NewLine);
            String DestLinear = "";
            if (radioButton_DeleteDuplicate.Checked)
            {
                DestLinear = FunctionDeleteDuplicate(SourceArray);
            }
            else if (radioButton_ChangeWide2Narrow.Checked)
            {
                DestLinear = FunctionChangeWide2Narrow(SourceArray);
            }
            textBox_Dest.Text = DestLinear;

            if (DestLinear != String.Empty)
            {
                Clipboard.SetText(textBox_Dest.Text);
            }
        }

        private String FunctionDeleteDuplicate(String[] SourceArray)
        {
            String[] DestArray = util.TrimDuplication(SourceArray);
            return util.ChangeStrArray2Linear(DestArray, Environment.NewLine);
        }

        private String FunctionChangeWide2Narrow(String[] SourceArray)
        {
            String[] DestArray = ChangeWide2Narrow(SourceArray);
            return util.ChangeStrArray2Linear(DestArray, Environment.NewLine);
        }

        private String[] ChangeWide2Narrow(String[] StrArray)
        {
            String RegesStr = "";
            if (!GetRegesStr(out RegesStr))
            {
                MessageBox.Show("変換対象が選ばれませんでした");
                return null;
            }

            Regex re = new Regex(RegesStr);
            return StrArray.Select(str => re.Replace(str, myReplacer)).ToArray();

            // 全置換（カナ含む）
            //return StrArray.Select(str => Strings.StrConv(str, VbStrConv.Narrow)).ToArray();
        }

        private Boolean GetRegesStr(out String RegesStr)
        {
            Boolean IsSuccess = false;

            RegesStr = "[";
            if (checkBox_Wide2Narrow_Number.Checked)
            {
                RegesStr += "０-９";
                IsSuccess = true;
            }

            if (checkBox_Wide2Narrow_Alpha_Large.Checked)
            {
                RegesStr += "Ａ-Ｚ";
                IsSuccess = true;
            }

            if (checkBox_Wide2Narrow_Alpha_Small.Checked)
            {
                RegesStr += "ａ-ｚ";
                IsSuccess = true;
            }
 
            if (checkBox_Wide2Narrow_Space.Checked)
            {
                RegesStr += "　";
                IsSuccess = true;
            }
            RegesStr += "]";

            return IsSuccess;
        }

        private String myReplacer(Match m)
        {
            // Memo: 参照設定に「Microsoft.VisualBasic」が必要
            return Strings.StrConv(m.Value, VbStrConv.Narrow);
        }
    }
}
