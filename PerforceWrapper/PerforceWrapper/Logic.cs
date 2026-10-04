namespace PerforceWrapper
{
    /// <summary>
    /// もともと Form1.cs に private メソッドとして埋め込まれていた、UIの状態を
    /// ドメインの値へ変換するだけの判定ロジックを、テストできる形に切り出したもの。
    ///
    /// ラジオボタンやタブの選択状態を直接参照する代わりに、
    /// 呼び出し側でその値(bool/int)を渡す形にした。
    /// </summary>
    internal static class Logic
    {
        /// <summary>タブの種類(タブコントロールの並び順と同じ)。</summary>
        internal enum TabId
        {
            BaseOperation,
            SetLabel,
            DiffLabel,
            ApplyLabel,
        }

        /// <summary>選択されているラジオボタンから、実行するPerforce操作の種類を判定する。</summary>
        public static Perforce.OperatorType GetOperatorType(
            bool checkoutChecked, bool restoreChecked, bool deleteChecked, bool getLatestChecked)
        {
            // 「最新を取得」と何も選ばれていない場合はどちらもSync
            if (checkoutChecked)
            {
                return Perforce.OperatorType.Edit;
            }
            if (restoreChecked)
            {
                return Perforce.OperatorType.Revert;
            }
            if (deleteChecked)
            {
                return Perforce.OperatorType.Delete;
            }
            return Perforce.OperatorType.Sync;
        }

        /// <summary>タブコントロールの選択インデックスから、現在のタブIDを判定する(範囲外はBaseOperation)。</summary>
        public static TabId GetCurrentTabId(int selectedTabIndex)
        {
            switch (selectedTabIndex)
            {
                case 1:
                    return TabId.SetLabel;
                case 2:
                    return TabId.DiffLabel;
                case 3:
                    return TabId.ApplyLabel;
                default:
                    return TabId.BaseOperation;
            }
        }
    }
}
