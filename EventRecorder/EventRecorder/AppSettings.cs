using System;
using System.Windows.Forms;

namespace EventRecorder
{
    // アプリ本体の設定値をまとめて1つのファイル(EventRecorder.json、userDataFolder配下)に
    // 保存するためのクラス。
    //
    // 以前はウィンドウサイズ等をWindowLayout.json、ホットキーをHotkeySettings.jsonと
    // ファイルを分けていたが、「ツールの設定ファイルは1つにまとめたい」という方針により統合した。
    // ファイル名をツール名そのものの"EventRecorder.json"にしたのに伴い、旧仕様にあった
    // 「userDataFolder直下のEventRecorder.json/xmlを起動時デフォルトプロファイルとして
    // 自動読み込みする」という予約ファイル名の慣習は廃止した([[Form1.cs]]のコンストラクタ参照)。
    // 今後、起動時に読み込まれるプロファイルは常に「プルダウン一覧の先頭」になる
    public class AppSettings
    {
        // ウィンドウサイズ+splitContainer_Mainの境界線位置
        public int Width { get; set; }
        public int Height { get; set; }
        public int SplitterDistance { get; set; }

        // 記録/再生の切り替えホットキー([[HotkeyDefaults.cs]]参照)
        public Keys RecordHotkey { get; set; }
        public Keys PlayHotkey { get; set; }
    }
}
