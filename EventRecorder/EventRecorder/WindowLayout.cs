using System;

namespace EventRecorder
{
    // ウィンドウサイズ+splitContainer_Mainの境界線位置(レコード/プレイバックの幅配分)を
    // 保存するための小さな設定データ。
    //
    // 以前はProperties.Settings.Default(.NET標準のユーザー設定。バージョンごとのフォルダ
    // (%LOCALAPPDATA%\EventRecorder\EventRecorder.exe_Url_xxxx\...\user.config)に
    // 分散して保存され、マクロ・プレイリストの保存先([[_Common/UserDataLocation.cs]]の
    // userDataFolder)とは別の管理になっていて分かりにくかった。この設定もJSON
    // ([[_Common/JsonFileStorage.cs]])にしてuserDataFolder配下に置き、
    // 「設定はすべてuserDataFolderにまとまっている」状態にした
    public class WindowLayout
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int SplitterDistance { get; set; }
    }
}
