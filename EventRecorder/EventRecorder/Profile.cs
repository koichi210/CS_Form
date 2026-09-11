using System;
using System.Collections.Generic;

namespace EventRecorder
{
    // マクロ(記録データ+プレイリスト)をJSON([[_Common/JsonFileStorage.cs]])で保存する際の
    // データ構造。プロパティ名がそのままJSONのキー名になるので、リネームすると
    // 過去に保存したJSONファイルが読めなくなる点に注意。
    //
    // 従来のXML版(SaveRestore.cs)はStcSaveRestoreのセル位置(Cell_行-列)方式で
    // DataGridViewへ直接出し入れしていたが、こちらは意味の分かる名前を持つ
    // 素直なPOCOとしてやり取りする(XML→JSON移行の足場)
    public class EventRecorderProfile
    {
        public String LoopCount { get; set; } = "1";
        public List<MacroEventData> Events { get; set; } = new List<MacroEventData>();
        public List<PlaylistEntryData> Playlist { get; set; } = new List<PlaylistEntryData>();
    }

    // dataGridView_Eventsの1行分(Type/X/Y/Key/Wait)
    public class MacroEventData
    {
        public String Type { get; set; }
        public String X { get; set; }
        public String Y { get; set; }
        public String Key { get; set; }
        public String Wait { get; set; }
    }

    // dataGridView_Playlistの1行分(実行チェック/設定ファイル名/その行のループ回数)
    public class PlaylistEntryData
    {
        public Boolean Enabled { get; set; }
        public String FileName { get; set; }
        public String LoopCount { get; set; }
    }
}
