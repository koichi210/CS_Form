using System;
using System.Drawing;
using System.Windows.Forms;
using Picture;

namespace Cheetos
{
    /// <summary>
    /// もともと Cheetos フォームの各タブ（DistOrient.cs / CaptureWindow.cs /
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
        public static bool IsPortrait(String targetFileName, int whiteWidth, int whiteCoef, Boolean isSample = false)
        {
            Boolean isPortrait = true;

            // 以前はサイズ取得(GetPictSize)・左端切り出し・右端切り出しのそれぞれで
            // 同じ元ファイルをフルデコードしており、1枚の画像につき都合3回デコードしていた。
            // ここで1回だけデコードしたBitmapを使い回すことでデコード回数を1回に減らす。
            using (Bitmap sourceImg = new Bitmap(targetFileName))
            {
                Size pictSize = sourceImg.Size;

                // 指定幅より画像サイズが小さければ、画像サイズの幅に合わせる
                int width = Math.Min(whiteWidth, pictSize.Width);

                // whiteCoefは、WhiteAreaを算出するための係数(実測値)
                int baseSize = width * pictSize.Height / whiteCoef;

                // 左端
                Rectangle leftCutParam = new Rectangle(0, 0, width, pictSize.Height);
                long leftPictSize = GetBinSize(sourceImg, leftCutParam);
                if (baseSize < leftPictSize)
                {
                    isPortrait = false;
                }

                // 右端(左端で横長と判定済みなら計測しない)
                long rightPictSize = 0;
                if (isPortrait)
                {
                    Rectangle rightCutParam = new Rectangle(pictSize.Width - width, 0, width, pictSize.Height);
                    rightPictSize = GetBinSize(sourceImg, rightCutParam);
                    if (baseSize < rightPictSize)
                    {
                        isPortrait = false;
                    }
                }

                if (isSample)
                {
                    String resultStr = "IsPortrait=" + isPortrait.ToString() + Environment.NewLine +
                        "BaseSize=" + baseSize.ToString() + Environment.NewLine +
                        "LeftPictSize=" + leftPictSize.ToString() + Environment.NewLine +
                        "RightPictSize=" + rightPictSize.ToString();
                    MessageBox.Show(resultStr, "画像情報");
                }

                return isPortrait;
            }
        }

        /// <summary>画像の指定範囲を切り出して、PNGエンコードした場合のバイト数を返す。</summary>
        public static long GetBinSize(String fileName, Rectangle cutParam)
        {
            // ファイルパスからは1回だけデコードし、実際の計測は共通処理(Bitmap版)に委ねる。
            using (Bitmap sourceImg = new Bitmap(fileName))
            {
                return GetBinSize(sourceImg, cutParam);
            }
        }

        /// <summary>既にデコード済みのBitmapから指定範囲を切り出し、PNGエンコードした場合のバイト数を返す。</summary>
        private static long GetBinSize(Bitmap sourceImg, Rectangle cutParam)
        {
            PicEdit trm = new PicEdit(cutParam.Width, cutParam.Height);

            // 切り取り
            trm.TrimExec(sourceImg, cutParam, new Point(0, 0));

            // 以前は一時PNGファイルをディスクに書いてFileInfo.Lengthを見ていたが、
            // ディスクI/O(書き込み+削除)自体が無駄なので、メモリ上でPNGエンコードして
            // そのバイト数を見るだけにした。
            long length = trm.GetCanvasPngByteLength();

            trm.Dispose();
            return length;
        }

        /// <summary>
        /// キャプチャ画像のファイル名の先頭部分（保存先＋接頭辞＋任意でタイムスタンプ）を組み立てる。
        /// 元は cw_checkBox_AddTimeStamp.Checked を直接参照していたので、AddTimeStamp 引数に置き換えた。
        /// </summary>
        public static String GetFileBaseFormat(String directoryPath, String prefix, Boolean addTimeStamp)
        {
            String fileBaseFormat = directoryPath + @"\";
            if (prefix != String.Empty)
            {
                fileBaseFormat += prefix + "_";
            }
            if (addTimeStamp)
            {
                fileBaseFormat += DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_");
            }

            return fileBaseFormat;
        }

        /// <summary>
        /// テキストボックスの数値を、上下キーで+1/-1する。数値でなければ変更しない。
        /// </summary>
        public static String UpdateValue(String baseValue, KeyEventArgs e)
        {
            int addValue = 0;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    addValue = 1;
                    break;
                case Keys.Down:
                    addValue = -1;
                    break;
            }

            int val;
            if (Int32.TryParse(baseValue, out val))
            {
                return (val + addValue).ToString();
            }
            return baseValue;
        }
    }
}
