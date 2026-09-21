using System;
using StandardTemplate;

namespace CaptureWindow
{
    /// <summary>
    /// もともと Form1.cs の SaveSetting_Click / LoadSetting に実装されていた、
    /// 設定値をXMLファイルに保存/読み込みするロジック。
    ///
    /// XMLの組み立てと読み取りは、同じ形式を手書きしていた4プロジェクト
    /// (CaptureWindow/PictTriming/PictMerge/PictMerge2)で共通だったため
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
            public String SavePath;
            public String MouseX;
            public String MouseY;
            public String Sleep;
        }

        public static void SaveSetting(String Path, String SavePath, String MouseX, String MouseY, String Sleep)
        {
            StcSimpleSettings settings = new StcSimpleSettings();
            settings.Set(KeySavePath, SavePath);
            settings.Set(KeyMouseX, MouseX);
            settings.Set(KeyMouseY, MouseY);
            settings.Set(KeySleep, Sleep);
            settings.SaveJson(Path);
        }

        // 旧形式(CaptureWindow.xml)しか無い場合は、読み込んだ内容をJSONで保存し直して旧XMLを削除する
        public static Settings LoadSetting(String Path)
        {
            StcSimpleSettings loaded = StcSimpleSettings.LoadWithMigration(Path);
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
