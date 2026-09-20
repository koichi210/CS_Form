using System;
using System.Drawing;
using System.IO;

namespace PictTriming
{
    /// <summary>
    /// もともと MainWindow.xaml.cs の Triming / SaveSetting_Click / LoadSetting に
    /// 実装されていたロジックをテストできる形に切り出したもの。コードはそのまま
    /// 移しただけで書き換えていない。BaseX.Text などのコントロール参照は、
    /// 呼び出し元(MainWindow)で読み取った値を引数として渡す/戻り値として
    /// 受け取る形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static void Triming(String TargetFilePath, String SourceFilePath, int BaseX, int BaseY, int Target_Width, int Target_Height)
        {
            //描画先とするImageオブジェクトを作成
            Bitmap canvas = new Bitmap(Target_Width, Target_Height);

            //画像ファイルのImageオブジェクトを作成
            Bitmap img = new Bitmap(SourceFilePath);

            //切り取る部分の範囲を決定
            Rectangle srcRect = new Rectangle(BaseX, BaseY, Target_Width, Target_Height);

            //描画する部分の範囲を決定
            Rectangle desRect = new Rectangle(0, 0, Target_Width, Target_Height);

            //ImageオブジェクトのGraphicsオブジェクトを作成
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.DrawImage(img, desRect, srcRect, GraphicsUnit.Pixel);
                g.Dispose();
            }
            img.Dispose();

            canvas.Save(TargetFilePath);
            canvas.Dispose();
        }

        public class Settings
        {
            public String SourceFolderPath;
            public String BaseX;
            public String BaseY;
            public String TargetX;
            public String TargetY;
        }

        private static readonly String[] Keys = { "SourceFolderPath", "BaseX", "BaseY", "TargetX", "TargetY" };

        // XMLの組み立て/読み取りは同じ形式を手書きしていた4プロジェクトで共通だったため
        // [[_Common/SimpleSettings.cs]]へ集約した。ここにはPictTriming固有の項目名の対応だけ残す
        public static void SaveSettingXml(String Path, String SourceFolderPath, String BaseX, String BaseY, String TargetX, String TargetY)
        {
            StandardTemplate.StcSimpleSettings settings = new StandardTemplate.StcSimpleSettings();
            String[] values = { SourceFolderPath, BaseX, BaseY, TargetX, TargetY };
            for (int i = 0; i < Keys.Length; i++)
            {
                settings.Set(Keys[i], values[i]);
            }
            settings.Save(Path);
        }

        public static Settings LoadSettingXml(String Path)
        {
            StandardTemplate.StcSimpleSettings loaded = StandardTemplate.StcSimpleSettings.Load(Path);
            if (loaded == null)
            {
                return null;
            }

            // 保存されていない項目はnullのままにする(呼び出し元が既定値を使う)
            return new Settings
            {
                SourceFolderPath = loaded.IsExist(Keys[0]) ? loaded.Get(Keys[0]) : null,
                BaseX = loaded.IsExist(Keys[1]) ? loaded.Get(Keys[1]) : null,
                BaseY = loaded.IsExist(Keys[2]) ? loaded.Get(Keys[2]) : null,
                TargetX = loaded.IsExist(Keys[3]) ? loaded.Get(Keys[3]) : null,
                TargetY = loaded.IsExist(Keys[4]) ? loaded.Get(Keys[4]) : null,
            };
        }
    }
}
