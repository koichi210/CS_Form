﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StandardTemplate;

namespace PerforceWrapper
{
    class Perforce
    {
        public enum OPERATOR_TYPE
        {
            EDIT,
            REVENT,
            DELETE,
            SYNC,
            SET_LABEL,
            DIFF,
            COPY,
            MERGE,
        }

        Boolean m_IsDebug = false;
        String m_ServerName = "";
        String m_Workspace = "";
        String m_UserName = "";
        String m_UserPass = "";
        String m_UserPassFile = "";
        String m_Character = "";
        String m_TargetTree = "";
        String m_Revision = "";
        String m_LabelName = "";
        String m_BranchMapName = "";
        OPERATOR_TYPE m_OperatorType = OPERATOR_TYPE.SYNC;

        public void SetDebugMode( Boolean IsDebug )
        {
            m_IsDebug = IsDebug;
        }
        public void SetServerName(String ServerName)
        {
            m_ServerName = ServerName;
        }

        public void SetWorkspace(String Workspace)
        {
            m_Workspace = Workspace;
        }

        public void SetUserName(String UserName)
        {
            m_UserName = UserName;
        }

        public void SetUserPass(String UserPass)
        {
            m_UserPass = UserPass;
        }

        public void SetCharacter(String Character)
        {
            m_Character = Character;
        }

        public void SetOperatorType(OPERATOR_TYPE OperatorType)
        {
            m_OperatorType = OperatorType;
        }

        public void SetRevision(String Revision)
        {
            m_Revision = Revision;
        }

        public void SetLabelName(String LabelName)
        {
            m_LabelName = LabelName;
        }

        public void SetBranchMapName(String BranchMapName)
        {
            m_BranchMapName = BranchMapName;
        }

        public void SetTargetTree(String TargetTree)
        {
            m_TargetTree = TargetTree;
        }

        public String CreateCommandUseTree()
        {
            String Command = CreateEnvCommand();
            String Operator = GetOperatorCommand();

            // 指定ツリーすべてに対してコマンド生成
            String[] TreeArray = m_TargetTree.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (String Tree in TreeArray)
            {
                if (Tree == String.Empty)
                {
                    continue;
                }

                Command += Operator + AppendTreeSymbol(Tree);
                Command += GetRevision() + Environment.NewLine;
            }
            Command += CreatePostCommand();

            return Command;
        }

        private String GetRevision()
        {
            String Revision = "";
            if (m_Revision != String.Empty)
            {
                Revision = "@" + m_Revision;
            }
            else
            {
                Revision = "#head";
            }

            return Revision;
        }

        public String CreateCommandDefined(String DefinedCommand)
        {
            String Command = CreateEnvCommand();
            String Operator = GetOperatorCommand();

            Command += Operator + DefinedCommand + Environment.NewLine;
            Command += CreatePostCommand();
            return Command;
        }

        public String GetLabelDesignationPathName(String PathName, String LabelName )
        {
            return AppendTreeSymbol(PathName) + "@" + LabelName;
        }

        private String AppendTreeSymbol(String FilePath)
        {
            String Command = FilePath;

            String DirSpec = "...";
            if (!FilePath.EndsWith(DirSpec))
            {
                Command += DirSpec;
            }

            return Command;
        }

        private String GetOperatorCommand()
        {
            String Operator = "p4 ";

            // Operationごとにコマンド切替
            switch(m_OperatorType)
            {
            case  OPERATOR_TYPE.EDIT:
                Operator += "edit ";
                break;

            case  OPERATOR_TYPE.REVENT:
                Operator += "revert ";
                break;

            case  OPERATOR_TYPE.DELETE:
                Operator += "delete ";
                break;

            case  OPERATOR_TYPE.SYNC:
                Operator += "sync ";
                break;

            case OPERATOR_TYPE.SET_LABEL:
                Operator += "tag -l " + m_LabelName + " ";
                break;

            case OPERATOR_TYPE.DIFF:
                Operator += "diff2 -qt ";
                break;

            case OPERATOR_TYPE.COPY:
                Operator += "copy -b " + m_BranchMapName + " -s ";
                break;

            case OPERATOR_TYPE.MERGE:
                Operator += "integrate -b " + m_BranchMapName + " -s ";
                break;
            }

            return Operator;
        }

        private String CreateEnvCommand()
        {
            String Command = "";

            if (m_ServerName != String.Empty)
            {
                Command += "set P4PORT=" + m_ServerName + Environment.NewLine;
            }

            if (m_Workspace != String.Empty)
            {
                Command += "set P4CLIENT=" + m_Workspace + Environment.NewLine;
            }

            if (m_Character != String.Empty)
            {
                Command += "set P4CHARSET=" + m_Character + Environment.NewLine;
            }

            if (m_UserName != String.Empty)
            {
                Command += "set P4USER=" + m_UserName + Environment.NewLine;
            }

            // Perforceログイン設定
            if (m_UserName != String.Empty && m_UserPass != String.Empty)
            {
                Command += "cat " + CreatePasswordFile(m_UserPass) + " | ";
                Command += "p4 -u " + m_UserName + " login -a" + Environment.NewLine;
            }

            return Command;
        }

        private String CreatePasswordFile(String Password)
        {
            StcFileInputOutput fio = new StcFileInputOutput();
            m_UserPassFile = fio.CreateTempFile();
            fio.CreateFile(m_UserPassFile, Password);

            return m_UserPassFile;
        }

        private String CreatePostCommand()
        {
            String Command = "";

            if (m_UserPassFile != String.Empty)
            {
                Command += "del " + m_UserPassFile + Environment.NewLine;
            }

            if (m_IsDebug)
            {
                Command += "PAUSE" + Environment.NewLine;
            }

            return Command;
        }
    }
}
