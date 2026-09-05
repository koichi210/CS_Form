using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using StandardTemplate;
using Picture;

namespace Cheetos
{
    /// <summary>
    /// もともと Cheetos フォームの各タブ（DistOrient.cs / CapureWindow.cs /
    /// RotationPreview.cs）に private メソッドとして埋め込まれていた純粋なロジックを、
    /// テストできる形に切り出したもの。
    ///
    /// コードは元のファイルにあったものをそのまま移しただけで、中身の書き換えはしていない。
    /// 呼び出し側の Form 側フィールド参照（util / fio）は、状態を持たないユーティリティ
    /// インスタンスなのでこのクラス内で個別に new し直している。
    /// GetFileBaseFormat だけは Form のチェックボックスを直接参照していたので、
    /// bool のパラメータに置き換えた（呼び出し側で .Checked を渡す）。
    /// </summary>
    internal static class Logic
    {
        /// <summary>
        /// 画像の左右の白フチの太さを比較し、縦長（Portrait）向けの画像かどうかを判定する。
        /// IsSample=true のときは判定結果をポップアップ表示する（もとの実装のまま）。
        /// </summary>
        public static bool IsPortrait(String TargetFileName, int WhiteWidth, int WhiteCoef, Boolean IsSample = false)
        {
            StcUtils util = new StcUtils();

            Boolean IsPort = true;
            Size pict_sz = util.GetPictSize(TargetFileName);

            // 指定幅より画像サイズが小さければ、画像サイズの幅に合わせる
            int width = Math.Min(WhiteWidth, pict_sz.Width);

            // WhiteCoefは、WhiteAreaを算出するための係数(実測値)
            int BaseSize = width * pict_sz.Height / WhiteCoef;

            // 左端
            long LeftPictSize = 0;
            if (IsPort == true)
            {
                Rectangle CutParam = new Rectangle(0, 0, width, pict_sz.Height);
                LeftPictSize = GetBinSize(TargetFileName, CutParam);
                if (BaseSize < LeftPictSize)
                {
                    IsPort = false;
                }
            }

            // 右端
            long RightPictSize = 0;
            if (IsPort == true)
            {
                Rectangle CutParam = new Rectangle(pict_sz.Width - width, 0, width, pict_sz.Height);
                RightPictSize = GetBinSize(TargetFileName, CutParam);
                if (BaseSize < RightPictSize)
                {
                    IsPort = false;
                }
            }

            if (IsSample)
            {
                String ResultStr = "IsPortrait=" + IsPort.ToString() + Environment.NewLine +
                    "BaseSize=" + BaseSize.ToString() + Environment.NewLine +
                    "LeftPictSize=" + LeftPictSize.ToString() + Environment.NewLine +
                    "RightPictSize=" + RightPictSize.ToString();
                MessageBox.Show(ResultStr, "画像情報");
            }

            return IsPort;
        }

        /// <summary>画像の指定範囲を切り出して一時PNGに保存し、そのファイルサイズを返す。</summary>
        public static long GetBinSize(String FileName, Rectangle CutParam)
        {
            StcFileInputOutput fio = new StcFileInputOutput();

            // TODO:tempファイルを作らずにファイルサイズを知りたい
            String TempFileName = fio.CreateTempFile("png");
            PicEdit trm = new PicEdit(CutParam.Width, CutParam.Height);

            // 切り取り
            Point PutParam = new Point(0, 0);
            trm.TrimExec(FileName, CutParam, PutParam);

            // キャンバス保存
            trm.SaveCanvas(TempFileName);

            FileInfo fi = new FileInfo(TempFileName);
            long Length = fi.Length;

            // Tempファイルを削除
            File.Delete(TempFileName);

            trm.Dispose();
            return Length;
        }

        /// <summary>
        /// キャプチャ画像のファイル名の先頭部分（保存先＋接頭辞＋任意でタイムスタンプ）を組み立てる。
        /// 元は cw_checkBox_AddTimeStump.Checked を直接参照していたので、AddTimeStamp 引数に置き換えた。
        /// </summary>
        public static String GetFileBaseFormat(String DirectoryPath, String Prifix, Boolean AddTimeStamp)
        {
            String FileBaseFormat = DirectoryPath + @"\";
            if (Prifix != String.Empty)
            {
                FileBaseFormat += Prifix + "_";
            }
            if (AddTimeStamp)
            {
                FileBaseFormat += System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_");
            }

            return FileBaseFormat;
        }

        /// <summary>
        /// テキストボックスの数値を、上下キーで+1/-1する。数値でなければ変更しない。
        /// </summary>
        public static String UpdateValue(String base_value, KeyEventArgs e)
        {
            int add_value = 0;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    add_value = 1;
                    break;
                case Keys.Down:
                    add_value = -1;
                    break;
                case Keys.Enter:
                    break;
                default:
                    break;
            }

            int val;
            if (Int32.TryParse(base_value.ToString(), out val))
            {
                val += add_value;
                return val.ToString();
            }
            return base_value;
        }
    }
}
