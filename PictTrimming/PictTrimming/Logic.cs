using System;
using System.Drawing;
using System.IO;

namespace PictTrimming
{
    /// <summary>
    /// もともと MainWindow.xaml.cs の Triming(現 Trim) / SaveSetting_Click / LoadSetting に
    /// 実装されていたロジックをテストできる形に切り出したもの。コードはそのまま
    /// 移しただけで書き換えていない。BaseX.Text などのコントロール参照は、
    /// 呼び出し元(MainWindow)で読み取った値を引数として渡す/戻り値として
    /// 受け取る形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static void Trim(String targetFilePath, String sourceFilePath, int baseX, int baseY, int targetWidth, int targetHeight)
        {
            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(baseX, baseY, targetWidth, targetHeight);

            //描画する部分の範囲を決定
            Rectangle destRect = new Rectangle(0, 0, targetWidth, targetHeight);

            // 途中で例外になっても画像が解放される(ファイルのロックが残らない)ようusingで囲む
            //描画先とするImageオブジェクトを作成
            using (Bitmap canvas = new Bitmap(targetWidth, targetHeight))
            {
                //画像ファイルのImageオブジェクトを作成
                using (Bitmap img = new Bitmap(sourceFilePath))
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);
                }

                canvas.Save(targetFilePath);
            }
        }

        public class Settings
        {
            public String SourceFolderPath { get; set; }
            public String BaseX { get; set; }
            public String BaseY { get; set; }
            public String TargetX { get; set; }
            public String TargetY { get; set; }
        }

        private static readonly String[] _settingKeys = { "SourceFolderPath", "BaseX", "BaseY", "TargetX", "TargetY" };

        // XMLの組み立て/読み取りは同じ形式を手書きしていた4プロジェクトで共通だったため
        // [[_Common/SimpleSettings.cs]]へ集約した。ここにはPictTrimming固有の項目名の対応だけ残す
        public static void SaveSetting(String filePath, String sourceFolderPath, String baseX, String baseY, String targetX, String targetY)
        {
            StandardTemplate.StcSimpleSettings settings = new StandardTemplate.StcSimpleSettings();
            String[] values = { sourceFolderPath, baseX, baseY, targetX, targetY };
            for (int i = 0; i < _settingKeys.Length; i++)
            {
                settings.Set(_settingKeys[i], values[i]);
            }
            settings.SaveJson(filePath);
        }

        public static Settings LoadSetting(String filePath)
        {
            StandardTemplate.StcSimpleSettings loaded = StandardTemplate.StcSimpleSettings.LoadWithMigration(filePath);
            if (loaded == null)
            {
                return null;
            }

            // 保存されていない項目はnullのままにする(呼び出し元が既定値を使う)
            Func<int, String> getOrNull = i => loaded.IsExist(_settingKeys[i]) ? loaded.Get(_settingKeys[i]) : null;
            return new Settings
            {
                SourceFolderPath = getOrNull(0),
                BaseX = getOrNull(1),
                BaseY = getOrNull(2),
                TargetX = getOrNull(3),
                TargetY = getOrNull(4),
            };
        }
    }
}
