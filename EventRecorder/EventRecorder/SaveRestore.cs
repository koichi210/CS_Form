using System;
using StandardTemplate;

namespace EventRecorder
{
    // マクロ(記録したイベント一覧+ループ回数)のXML保存/読込
    // ※ いずれJSON保存に置き換えたい(やりたいことリスト)
    class SaveRestore : StcSaveRestore
    {
        public void RegistItem(Form1 Parent)
        {
            SetElement("Setting");

            RegistCtrl("Record", "textBox_Loop", Parent.textBox_Loop, "1");
            RegistCtrl("Record", "Cell", "RowCount", Parent.dataGridView_Events);

            // タブ2のプレイリスト(実行順・チェック状態・行ごとのループ回数・全体ループ回数)も
            // 同じ設定ファイルに保存する。1つのファイルにマクロとプレイリストの両方を持たせる形
            RegistCtrl("Playlist", "textBox_PlaylistLoop", Parent.textBox_PlaylistLoop, "1");
            RegistCtrl("Playlist", "Cell", "RowCount", Parent.dataGridView_Playlist);
        }

        // プレイリスト再生時、各行の設定ファイルを1つずつ読み込む専用。
        // 記録データ(タブ1)だけを登録し、Playlist側は一切登録しない。
        // (RegistItemを使い回すと、各ファイルに保存されている「そのファイルを保存した時点の
        // プレイリストのスナップショット」がLoadXmlFile内部の仕組みで強制的に反映されてしまい、
        // 再生中に今操作中のプレイリストが勝手に上書き/消去される不具合になるため、専用インスタンスで分離する)
        public void RegistItemForPlayback(Form1 Parent)
        {
            SetElement("Setting");

            RegistCtrl("Record", "textBox_Loop", Parent.textBox_Loop, "1");
            RegistCtrl("Record", "Cell", "RowCount", Parent.dataGridView_Events);
        }

        // 通常の「設定値読込」等、ファイルの中身(記録データ+プレイリスト)を
        // まるごと反映したい場合はこちら(既定でプレイリストもクリアする)
        public Boolean LoadProc(String LoadFileName, Form1 Parent)
        {
            return LoadProc(LoadFileName, Parent, true);
        }

        // clearPlaylist=falseにすると、プレイリスト再生中に各行の設定ファイルを
        // 1つずつ読み込む時のように、記録データ(タブ1)だけ差し替えてプレイリスト自体(タブ2)は
        // 触らずに残せる
        public Boolean LoadProc(String LoadFileName, Form1 Parent, Boolean clearPlaylist)
        {
            if (LoadFileName == String.Empty)
            {
                return false;
            }

            Parent.dataGridView_Events.Rows.Clear();
            if (clearPlaylist)
            {
                Parent.dataGridView_Playlist.Rows.Clear();
            }
            return LoadXmlFile(LoadFileName);
        }
    }
}
