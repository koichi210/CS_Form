using System;
using System.Text;
using StandardTemplate;

namespace PerforceWrapper
{
    class Perforce
    {
        public enum OperatorType
        {
            Edit,
            Revert,
            Delete,
            Sync,
            SetLabel,
            Diff,
            Copy,
            Merge,
        }

        private Boolean _isDebug = false;
        private String _serverName = "";
        private String _workspace = "";
        private String _userName = "";
        private String _userPass = "";
        private String _userPassFile = "";
        private String _charset = "";
        private String _targetTree = "";
        private String _revision = "";
        private String _labelName = "";
        private String _branchMapName = "";
        private OperatorType _operatorType = OperatorType.Sync;

        public void SetDebugMode(Boolean isDebug)
        {
            _isDebug = isDebug;
        }

        public void SetServerName(String serverName)
        {
            _serverName = serverName;
        }

        public void SetWorkspace(String workspace)
        {
            _workspace = workspace;
        }

        public void SetUserName(String userName)
        {
            _userName = userName;
        }

        public void SetUserPass(String userPass)
        {
            _userPass = userPass;
        }

        public void SetCharset(String charset)
        {
            _charset = charset;
        }

        public void SetOperatorType(OperatorType operatorType)
        {
            _operatorType = operatorType;
        }

        public void SetRevision(String revision)
        {
            _revision = revision;
        }

        public void SetLabelName(String labelName)
        {
            _labelName = labelName;
        }

        public void SetBranchMapName(String branchMapName)
        {
            _branchMapName = branchMapName;
        }

        public void SetTargetTree(String targetTree)
        {
            _targetTree = targetTree;
        }

        public String CreateCommandUseTree()
        {
            var command = new StringBuilder(CreateEnvCommand());
            String operatorCommand = GetOperatorCommand();
            String revision = GetRevision();

            // 指定ツリーすべてに対してコマンド生成
            String[] trees = _targetTree.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (String tree in trees)
            {
                if (tree == String.Empty)
                {
                    continue;
                }

                command.Append(operatorCommand).Append(AppendTreeSymbol(tree));
                command.Append(revision).Append(Environment.NewLine);
            }
            command.Append(CreatePostCommand());

            return command.ToString();
        }

        private String GetRevision()
        {
            return _revision != String.Empty ? "@" + _revision : "#head";
        }

        public String CreateCommandDefined(String definedCommand)
        {
            return CreateEnvCommand()
                + GetOperatorCommand() + definedCommand + Environment.NewLine
                + CreatePostCommand();
        }

        public String GetLabelDesignationPathName(String pathName, String labelName)
        {
            return AppendTreeSymbol(pathName) + "@" + labelName;
        }

        private static String AppendTreeSymbol(String filePath)
        {
            const String dirSpec = "...";
            return filePath.EndsWith(dirSpec) ? filePath : filePath + dirSpec;
        }

        private String GetOperatorCommand()
        {
            // Operationごとにコマンド切替
            switch (_operatorType)
            {
                case OperatorType.Edit:
                    return "p4 edit ";
                case OperatorType.Revert:
                    return "p4 revert ";
                case OperatorType.Delete:
                    return "p4 delete ";
                case OperatorType.Sync:
                    return "p4 sync ";
                case OperatorType.SetLabel:
                    return "p4 tag -l " + _labelName + " ";
                case OperatorType.Diff:
                    return "p4 diff2 -qt ";
                case OperatorType.Copy:
                    return "p4 copy -b " + _branchMapName + " -s ";
                case OperatorType.Merge:
                    return "p4 integrate -b " + _branchMapName + " -s ";
                default:
                    return "p4 ";
            }
        }

        private String CreateEnvCommand()
        {
            var command = new StringBuilder();

            AppendSetCommand(command, "P4PORT", _serverName);
            AppendSetCommand(command, "P4CLIENT", _workspace);
            AppendSetCommand(command, "P4CHARSET", _charset);
            AppendSetCommand(command, "P4USER", _userName);

            // Perforceログイン設定
            if (_userName != String.Empty && _userPass != String.Empty)
            {
                command.Append("cat ").Append(CreatePasswordFile(_userPass)).Append(" | ");
                command.Append("p4 -u ").Append(_userName).Append(" login -a").Append(Environment.NewLine);
            }

            return command.ToString();
        }

        // 値が空でなければ「set 変数名=値」の行を追加する(空ならPC側の既定設定を使う)
        private static void AppendSetCommand(StringBuilder command, String variableName, String value)
        {
            if (value != String.Empty)
            {
                command.Append("set ").Append(variableName).Append('=').Append(value).Append(Environment.NewLine);
            }
        }

        private String CreatePasswordFile(String password)
        {
            StcFileInputOutput fio = new StcFileInputOutput();
            _userPassFile = fio.CreateTempFile();
            fio.CreateFile(_userPassFile, password);

            return _userPassFile;
        }

        private String CreatePostCommand()
        {
            String command = "";

            if (_userPassFile != String.Empty)
            {
                command += "del " + _userPassFile + Environment.NewLine;
            }

            if (_isDebug)
            {
                command += "PAUSE" + Environment.NewLine;
            }

            return command;
        }
    }
}
