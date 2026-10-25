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

        // モード切替ラジオボタン(true=レコード、false=プレイバック)・最小化チェックボックスの
        // 状態。プレイリスト再生時の各行のファイル読込(LoadProfileForPlayback)では、
        // 途中でモードが切り替わってしまわないようこの2つは適用しない
        public Boolean IsRecordMode { get; set; } = true;
        public Boolean MinimizeOnPlay { get; set; }
    }

    // dataGridView_Eventsの1行分(Type/X/Y/Key/Wait/備考)
    public class MacroEventData
    {
        public String Type { get; set; }
        public String X { get; set; }
        public String Y { get; set; }
        public String Key { get; set; }
        public String Wait { get; set; }

        // ユーザーが自由に書けるコメント欄。記録・再生には一切使わない
        public String Remarks { get; set; }

        // ホイール行の回転量(WHEEL_DELTA=120が1ノッチ。向きはTypeのWHEEL_UP/WHEEL_DOWNで表すので正の数)。
        // 以前は記録していなかったので、この項目が無いファイル(空の行)は従来どおり1行=1ノッチで再生する
        public String Delta { get; set; }

        // 空なら書き出さない(ホイール以外の行や、回転量の無い行は今までと同じ形のまま保存する)。
        // Newtonsoft.JsonのShouldSerialize{プロパティ名}の決まりで呼ばれる
        public Boolean ShouldSerializeDelta()
        {
            return !String.IsNullOrEmpty(Delta);
        }
    }

    // dataGridView_Playlistの1行分(実行チェック/設定ファイル名/その行のループ回数)
    public class PlaylistEntryData
    {
        public Boolean Enabled { get; set; }
        public String FileName { get; set; }
        public String LoopCount { get; set; }
    }
}
