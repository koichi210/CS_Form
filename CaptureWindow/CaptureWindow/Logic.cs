using System;
using StandardTemplate;

namespace CaptureWindow
{
    /// <summary>
    /// もともと Form1.cs の SaveSetting_Click / LoadSetting に実装されていた、
    /// 設定値をXMLファイルに保存/読み込みするロジック。
    ///
    /// XMLの組み立てと読み取りは、同じ形式を手書きしていた4プロジェクト
    /// (CaptureWindow/PictTrimming/PictMerge/PictMerge2)で共通だったため
    /// [[_Common/SimpleSettings.cs]]へ集約した。ここに残っているのは
    /// 「どの項目をどのキー名で保存するか」というCaptureWindow固有の対応だけ。
    /// ファイル形式は従来と同じなので、これまでの設定ファイルもそのまま読める。
    /// </summary>
    internal static class Logic
    {
        private const String KeySavePath = "TextBox_SavePath";
        private const String KeyMouseX = "TextBox_MouseX";
        private const String KeyMouseY = "TextBox_MouseY";
        private const String KeySleep = "TextBox_Sleep";

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
            settings.Set(KeySavePath, savePath);
            settings.Set(KeyMouseX, mouseX);
            settings.Set(KeyMouseY, mouseY);
            settings.Set(KeySleep, sleep);
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
                SavePath = loaded.IsExist(KeySavePath) ? loaded.Get(KeySavePath) : null,
                MouseX = loaded.IsExist(KeyMouseX) ? loaded.Get(KeyMouseX) : null,
                MouseY = loaded.IsExist(KeyMouseY) ? loaded.Get(KeyMouseY) : null,
                Sleep = loaded.IsExist(KeySleep) ? loaded.Get(KeySleep) : null,
            };
        }
    }
}
