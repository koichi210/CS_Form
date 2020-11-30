﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StandardTemplate;

namespace PerforceWrapper
{
    public partial class Form1 : Form
    {
        private readonly String SettingFileName = "PerforceWrapper.xml";

        private StcFileInputOutput fio = new StcFileInputOutput();
        private SaveRestore sr = new SaveRestore();
        private StcUtils util = new StcUtils();
        private Boolean m_IsDebug = false;

        enum TAB_ID{
            BASE_OPERATION,
            SET_LABEL,
            DIFF_LABEL,
            APPLY_LABEL,
        }

        public Form1()
        {
            InitializeComponent();

            // アイコン設定
            this.Icon = Properties.Resources.PerforceWrapper;

            // カレントディレクトリ移動
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            sr.RegistItem(this);
            sr.LoadProc(SettingFileName);
            util.UpdateProfileList(ref comboBox_profile);
        }

        private void button_profile_save_Click(object sender, EventArgs e)
        {
            String SaveFileName = fio.SelectSaveFileName(comboBox_profile.Text);

            if (sr.SaveSetting(SaveFileName, this))
            {
                util.UpdateProfileList(ref comboBox_profile, Path.GetFileName(SaveFileName));
                MessageBox.Show("設定値を保存しました♪" + Environment.NewLine + SaveFileName);
            }
        }

        private void comboBox_profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LoadFileName = Directory.GetCurrentDirectory() + @"\" + comboBox_profile.Text;
            sr.LoadProc(LoadFileName);
        }

        private Perforce.OPERATOR_TYPE GetOperatorType()
        {
            Perforce.OPERATOR_TYPE OperatorType = Perforce.OPERATOR_TYPE.SYNC;
            if (radioButton_so_menu_checkout.Checked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.EDIT;
            }
            else if (radioButton_so_menu_restore.Checked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.REVENT;
            }
            else if (radioButton_so_menu_delete.Checked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.DELETE;
            }
            else if (radioButton_so_menu_get_latest.Checked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.SYNC;
            }

            return OperatorType;            
        }

        private void textBox_tree_list_KeyDown(object sender, KeyEventArgs e)
        {
            util.SelectAll(e);
        }

        private String ExecuteForeground(String Script)
        {
            String BatchFile = fio.CreateTempFile("bat");
            fio.CreateFile(BatchFile, Script);

            String Output = "";
            util.ExecuteProcess(out Output, BatchFile);
            return Output;
        }

        private void Execute(String Script)
        {
            String BatchFile = fio.CreateTempFile("bat");
            fio.CreateFile(BatchFile, Script);

            // 実行
            List<object> arguments = new List<object>();
            arguments.Add(BatchFile);
            backgroundWorker.RunWorkerAsync(arguments);   // ⇒DoWork()
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            // このメソッドへのパラメータ
            List<object> genericlist = e.Argument as List<object>;
            String BatchFile = (String)genericlist[0];

            // コマンド実行
            util.ExecuteProcess(BatchFile);

            // パスワードが含まれるのでファイルを削除する
            //File.Delete(BatchFile);

            // このメソッドからの戻り値
            e.Result = "SUCCESS";
        }


        private void SetPerforceEnv(Perforce pf)
        {
            pf.SetServerName(comboBox_perforce_server.Text);
            pf.SetWorkspace(comboBox_perforce_workspace.Text);
            pf.SetCharacter(comboBox_perforce_charset.Text);
            pf.SetUserName(comboBox_perforce_user.Text);
            pf.SetUserPass(textbox_perforce_password.Text);
        }

        private TAB_ID GetCurrentTabId()
        {
            TAB_ID TabId = TAB_ID.BASE_OPERATION;
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    TabId = TAB_ID.BASE_OPERATION;
                    break;

                case 1:
                    TabId = TAB_ID.SET_LABEL;
                    break;

                case 2:
                    TabId = TAB_ID.DIFF_LABEL;
                    break;
                
                case 3:
                    TabId = TAB_ID.APPLY_LABEL;
                    break;
                
                default:
                    break;
            }

            return TabId;
        }


        private void UpdateControlUI(object sender, EventArgs e)
        {
            Boolean IsStandardChangeListEnable = false;
            if (radioButton_so_menu_get_latest.Checked)
            {
                IsStandardChangeListEnable = true;
            }
            textBox_so_changelist.Enabled = IsStandardChangeListEnable;

            Boolean IsTreeEnable = true;
            TAB_ID CurrentTabId = GetCurrentTabId();
            if (CurrentTabId == TAB_ID.DIFF_LABEL)
            {
                IsTreeEnable = false;
            }
            textBox_tree_list.Enabled = IsTreeEnable;
        }

        private void button_execute_Click(object sender, EventArgs e)
        {
            TAB_ID TabId = GetCurrentTabId();

            switch(TabId)
            {
            case TAB_ID.BASE_OPERATION:
                BaseOperationExecute();
                break;

            case TAB_ID.SET_LABEL:
                SetLabelExecute();
                break;

            case TAB_ID.DIFF_LABEL:
                DiffLabelExecute();
                break;

            case TAB_ID.APPLY_LABEL:
                ApplyLabelExecute();
                break;

                default:
                break;
            }
        }

        private void BaseOperationExecute()
        {
            if (textBox_tree_list.Text == String.Empty)
            {
                MessageBox.Show("ツリーが指定されていません");
                return;
            }

            Perforce pf = new Perforce();
            SetPerforceEnv(pf);

            // 選択された操作のコマンド生成
            Perforce.OPERATOR_TYPE OperatorType = GetOperatorType();
            pf.SetOperatorType(OperatorType);
            if (OperatorType == Perforce.OPERATOR_TYPE.SYNC)
            {
                pf.SetRevision(textBox_so_changelist.Text);
            }
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetDebugMode(m_IsDebug);

            Execute(pf.CreateCommandUseTree());
        }

        private void SetLabelExecute()
        {
            if (textBox_tree_list.Text == String.Empty)
            {
                MessageBox.Show("ツリーが指定されていません");
                return;
            }
            if (textBox_sl_label_name.Text == String.Empty)
            {
                MessageBox.Show("ラベル名が指定されていません");
                return;
            }

            Perforce pf = new Perforce();
            SetPerforceEnv(pf);

            // 選択された操作のコマンド生成
            pf.SetOperatorType(Perforce.OPERATOR_TYPE.SET_LABEL);
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetRevision(textBox_sl_base_changelist.Text);
            pf.SetLabelName(textBox_sl_label_name.Text);
            pf.SetDebugMode(m_IsDebug);

            Execute(pf.CreateCommandUseTree());
        }

        private void DiffLabelExecute()
        {
            if (textBox_dl_src_label_name.Text == String.Empty ||
                textBox_dl_src_tree.Text == String.Empty ||
                textBox_dl_dest_label_name.Text == String.Empty ||
                textBox_dl_dest_tree.Text == String.Empty)
            {
                MessageBox.Show("設定されていない項目があります。" + Environment.NewLine + 
                    "比較元 ラベル名：" + textBox_dl_src_label_name + Environment.NewLine + 
                    "比較元 ツリー：" + textBox_dl_src_tree + Environment.NewLine + 
                    "比較先 ラベル名：" + textBox_dl_dest_label_name + Environment.NewLine + 
                    "比較先 ツリー：" + textBox_dl_dest_tree + Environment.NewLine );
                return;
            }

            Perforce pf = new Perforce();
            SetPerforceEnv(pf);

            // 選択された操作のコマンド生成
            pf.SetOperatorType(Perforce.OPERATOR_TYPE.DIFF);
            pf.SetDebugMode(m_IsDebug);

            // 比較対象
            String Command = pf.GetLabelDesignationPathName(textBox_dl_src_tree.Text, textBox_dl_src_label_name.Text);
            Command += " ";
            Command += pf.GetLabelDesignationPathName(textBox_dl_dest_tree.Text, textBox_dl_dest_label_name.Text);

            // 比較結果格納
            StcFileInputOutput fio = new StcFileInputOutput();
            String DiffFile = fio.CreateTempFile();
            Command += " > " + DiffFile;

            ExecuteForeground(pf.CreateCommandDefined(Command));

            // 比較結果検証
            FileInfo fi = new FileInfo(DiffFile);
            if ( 0 < fi.Length)
            {
                MessageBox.Show("差分があります", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("差分はありません");
            }
        }

        private void ApplyLabelExecute()
        {
            if (textBox_ak_branch_map.Text == String.Empty)
            {
                MessageBox.Show("ブランチマップが指定されていません");
                return;
            }

            Perforce pf = new Perforce();
            SetPerforceEnv(pf);

            // 選択された操作のコマンド生成
            Perforce.OPERATOR_TYPE OperatorType = Perforce.OPERATOR_TYPE.COPY;
            if (radioButton_al_merge.Checked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.MERGE;
            }
            pf.SetOperatorType(OperatorType);
            pf.SetRevision(textBox_ak_label_name.Text);
            pf.SetBranchMapName(textBox_ak_branch_map.Text);
            pf.SetTargetTree(textBox_tree_list.Text);
            pf.SetDebugMode(m_IsDebug);

            Execute(pf.CreateCommandUseTree());
        }

        private void label16_DoubleClick(object sender, EventArgs e)
        {
            m_IsDebug = !m_IsDebug;
            MessageBox.Show("DebugMode=" + m_IsDebug.ToString());
        }
    }
}
