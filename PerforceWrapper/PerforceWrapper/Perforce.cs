using System;
using StandardTemplate;

namespace PerforceWrapper
{
    class Perforce
    {
        public enum OPERATOR_TYPE
        {
            EDIT,
            REVERT,
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
        String m_Charset = "";
        String m_TargetTree = "";
        String m_Revision = "";
        String m_LabelName = "";
        String m_BranchMapName = "";
        OPERATOR_TYPE m_OperatorType = OPERATOR_TYPE.SYNC;

        public void SetDebugMode( Boolean isDebug )
        {
            m_IsDebug = isDebug;
        }
        public void SetServerName(String serverName)
        {
            m_ServerName = serverName;
        }

        public void SetWorkspace(String workspace)
        {
            m_Workspace = workspace;
        }

        public void SetUserName(String userName)
        {
            m_UserName = userName;
        }

        public void SetUserPass(String userPass)
        {
            m_UserPass = userPass;
        }

        public void SetCharset(String charset)
        {
            m_Charset = charset;
        }

        public void SetOperatorType(OPERATOR_TYPE operatorType)
        {
            m_OperatorType = operatorType;
        }

        public void SetRevision(String revision)
        {
            m_Revision = revision;
        }

        public void SetLabelName(String labelName)
        {
            m_LabelName = labelName;
        }

        public void SetBranchMapName(String branchMapName)
        {
            m_BranchMapName = branchMapName;
        }

        public void SetTargetTree(String targetTree)
        {
            m_TargetTree = targetTree;
        }

        public String CreateCommandUseTree()
        {
            String command = CreateEnvCommand();
            String operatorCommand = GetOperatorCommand();

            // 指定ツリーすべてに対してコマンド生成
            String[] trees = m_TargetTree.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (String tree in trees)
            {
                if (tree == String.Empty)
                {
                    continue;
                }

                command += operatorCommand + AppendTreeSymbol(tree);
                command += GetRevision() + Environment.NewLine;
            }
            command += CreatePostCommand();

            return command;
        }

        private String GetRevision()
        {
            if (m_Revision != String.Empty)
            {
                return "@" + m_Revision;
            }

            return "#head";
        }

        public String CreateCommandDefined(String definedCommand)
        {
            String command = CreateEnvCommand();
            command += GetOperatorCommand() + definedCommand + Environment.NewLine;
            command += CreatePostCommand();
            return command;
        }

        public String GetLabelDesignationPathName(String pathName, String labelName )
        {
            return AppendTreeSymbol(pathName) + "@" + labelName;
        }

        private String AppendTreeSymbol(String filePath)
        {
            const String DirSpec = "...";
            if (filePath.EndsWith(DirSpec))
            {
                return filePath;
            }

            return filePath + DirSpec;
        }

        private String GetOperatorCommand()
        {
            String operatorCommand = "p4 ";

            // Operationごとにコマンド切替
            switch(m_OperatorType)
            {
            case  OPERATOR_TYPE.EDIT:
                operatorCommand += "edit ";
                break;

            case  OPERATOR_TYPE.REVERT:
                operatorCommand += "revert ";
                break;

            case  OPERATOR_TYPE.DELETE:
                operatorCommand += "delete ";
                break;

            case  OPERATOR_TYPE.SYNC:
                operatorCommand += "sync ";
                break;

            case OPERATOR_TYPE.SET_LABEL:
                operatorCommand += "tag -l " + m_LabelName + " ";
                break;

            case OPERATOR_TYPE.DIFF:
                operatorCommand += "diff2 -qt ";
                break;

            case OPERATOR_TYPE.COPY:
                operatorCommand += "copy -b " + m_BranchMapName + " -s ";
                break;

            case OPERATOR_TYPE.MERGE:
                operatorCommand += "integrate -b " + m_BranchMapName + " -s ";
                break;
            }

            return operatorCommand;
        }

        private String CreateEnvCommand()
        {
            String command = "";

            if (m_ServerName != String.Empty)
            {
                command += "set P4PORT=" + m_ServerName + Environment.NewLine;
            }

            if (m_Workspace != String.Empty)
            {
                command += "set P4CLIENT=" + m_Workspace + Environment.NewLine;
            }

            if (m_Charset != String.Empty)
            {
                command += "set P4CHARSET=" + m_Charset + Environment.NewLine;
            }

            if (m_UserName != String.Empty)
            {
                command += "set P4USER=" + m_UserName + Environment.NewLine;
            }

            // Perforceログイン設定
            if (m_UserName != String.Empty && m_UserPass != String.Empty)
            {
                command += "cat " + CreatePasswordFile(m_UserPass) + " | ";
                command += "p4 -u " + m_UserName + " login -a" + Environment.NewLine;
            }

            return command;
        }

        private String CreatePasswordFile(String password)
        {
            StcFileInputOutput fio = new StcFileInputOutput();
            m_UserPassFile = fio.CreateTempFile();
            fio.CreateFile(m_UserPassFile, password);

            return m_UserPassFile;
        }

        private String CreatePostCommand()
        {
            String command = "";

            if (m_UserPassFile != String.Empty)
            {
                command += "del " + m_UserPassFile + Environment.NewLine;
            }

            if (m_IsDebug)
            {
                command += "PAUSE" + Environment.NewLine;
            }

            return command;
        }
    }
}
