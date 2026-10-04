using System;
using StandardTemplate;

namespace CaptureWindow
{
    /// <summary>
    /// もともと Form1.cs の SaveSetting_Click / LoadSetting に実装されていた、
    /// 設定値をファイルに保存/読み込みするロジック(現在はJSON。旧XMLは読み込み時に移行する)。
    ///
    /// XMLの組み立てと読み取りは、同じ形式を手書きしていた4プロジェクト
    /// (CaptureWindow/PictTrimming/PictMerge/PictMerge2)で共通だったため
    /// [[_Common/SimpleSettings.cs]]へ集約した。ここに残っているのは
    /// 「どの項目をどのキー名で保存するか」というCaptureWindow固有の対応だけ。
    /// ファイル形式は従来と同じなので、これまでの設定ファイルもそのまま読める。
    /// </summary>
    internal static class Logic
    {
        private const String _keySavePath = "TextBox_SavePath";
        private const String _keyMouseX = "TextBox_MouseX";
        private const String _keyMouseY = "TextBox_MouseY";
        private const String _keySleep = "TextBox_Sleep";

        public class Settings
        {
            public String SavePath { get; set; }
            public String MouseX { get; set; }
            public String MouseY { get; set; }
            public String Sleep { get; set; }
        }

        public static void SaveSetting(String path, String savePath, String mouseX, String mouseY, String sleep)
        {
            StcSimpleSettings settings = new StcSimpleSettings();
            settings.Set(_keySavePath, savePath);
            settings.Set(_keyMouseX, mouseX);
            settings.Set(_keyMouseY, mouseY);
            settings.Set(_keySleep, sleep);
            settings.SaveJson(path);
        }

        // 旧形式(CaptureWindow.xml)しか無い場合は、読み込んだ内容をJSONで保存し直して旧XMLを削除する
        public static Settings LoadSetting(String path)
        {
            StcSimpleSettings loaded = StcSimpleSettings.LoadWithMigration(path);
            if (loaded == null)
            {
                return null;
            }

            // 保存されていない項目はnullのままにする(呼び出し元が既定値を使う)
            return new Settings
            {
                SavePath = GetOrNull(loaded, _keySavePath),
                MouseX = GetOrNull(loaded, _keyMouseX),
                MouseY = GetOrNull(loaded, _keyMouseY),
                Sleep = GetOrNull(loaded, _keySleep),
            };
        }

        private static String GetOrNull(StcSimpleSettings settings, String key)
        {
            return settings.IsExist(key) ? settings.Get(key) : null;
        }
    }
}
