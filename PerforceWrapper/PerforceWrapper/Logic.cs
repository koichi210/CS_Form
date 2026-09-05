namespace PerforceWrapper
{
    /// <summary>
    /// もともと Form1.cs に private メソッドとして埋め込まれていた、UIの状態を
    /// ドメインの値へ変換するだけの判定ロジックを、テストできる形に切り出したもの。
    ///
    /// コードは元のファイルにあったものをそのまま移しただけで、判定の中身は
    /// 書き換えていない。ラジオボタンやタブの選択状態を直接参照する代わりに、
    /// 呼び出し側でその値(bool/int)を渡す形にした。
    /// </summary>
    internal static class Logic
    {
        /// <summary>Form1 の private だった TAB_ID をそのまま移設したもの。</summary>
        internal enum TAB_ID
        {
            BASE_OPERATION,
            SET_LABEL,
            DIFF_LABEL,
            APPLY_LABEL,
        }

        /// <summary>選択されているラジオボタンから、実行するPerforce操作の種類を判定する。</summary>
        public static Perforce.OPERATOR_TYPE GetOperatorType(
            bool checkoutChecked, bool restoreChecked, bool deleteChecked, bool getLatestChecked)
        {
            Perforce.OPERATOR_TYPE OperatorType = Perforce.OPERATOR_TYPE.SYNC;
            if (checkoutChecked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.EDIT;
            }
            else if (restoreChecked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.REVENT;
            }
            else if (deleteChecked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.DELETE;
            }
            else if (getLatestChecked)
            {
                OperatorType = Perforce.OPERATOR_TYPE.SYNC;
            }

            return OperatorType;
        }

        /// <summary>タブコントロールの選択インデックスから、現在のタブIDを判定する。</summary>
        public static TAB_ID GetCurrentTabId(int selectedTabIndex)
        {
            TAB_ID TabId = TAB_ID.BASE_OPERATION;
            switch (selectedTabIndex)
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
    }
}
