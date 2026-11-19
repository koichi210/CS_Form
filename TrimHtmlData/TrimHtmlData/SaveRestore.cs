using StandardTemplate;

namespace TrimHtmlData
{
    class SaveRestore : StcSaveRestore
    {
        public void RegisterItem(Form1 parent)
        {
            // コントロールを列挙。
            // 取得元(textBox_SourceList)・取得値(textBox_DestList)は毎回入れ替える「データ」で設定ではないので、
            // プロファイルの保存・読み込みの対象にしない(プロファイルを選び直しても入力中のURLや結果が消えない)。
            // 以前のプロファイルに残っているこの2項目の値は、読み込み時に無視される
            RegisterCtrl("Common", "textBox_SearchWord", parent.textBox_SearchWord);
            RegisterCtrl("Common", "textBox_DelimiterWord", parent.textBox_DelimiterWord);
            RegisterCtrl("Common", "textBox_TrimLineNum", parent.textBox_TrimLineNum);
            RegisterCtrl("Common", "checkBox_OrdinalCase", parent.checkBox_OrdinalCase);
        }

        // LoadProc(string) / SaveSetting(string) は StcSaveRestore 側の共通実装をそのまま使う
        // （「空なら何もしない→委譲」という中身が完全に同じだったため）。
    }
}
