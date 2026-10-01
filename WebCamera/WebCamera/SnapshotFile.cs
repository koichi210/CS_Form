using System;
using System.Globalization;

namespace WebCamera
{
    /// <summary>
    /// スナップショット(静止画)保存まわりの、カメラやUIに依存しない小さなロジック。
    /// Form1 から切り出してあるのは、カメラ無しの環境でもテストできるようにするため。
    /// </summary>
    public static class SnapshotFile
    {
        public const string Extension = "png";
        public const string DialogFilter = "PNG画像 (*.png)|*.png";

        /// <summary>
        /// 既定の保存ファイル名(例: 20261002_134501.png)を作る。
        /// 和暦などのカルチャに左右されないよう、日付書式はインバリアントカルチャで固定する。
        /// </summary>
        public static string CreateDefaultFileName(DateTime now)
        {
            return now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + "." + Extension;
        }

        /// <summary>
        /// 保存ダイアログの初期フォルダ。マイピクチャが取れない環境ではマイドキュメントにする。
        /// </summary>
        public static string GetInitialDirectory()
        {
            string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!string.IsNullOrEmpty(pictures))
            {
                return pictures;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
