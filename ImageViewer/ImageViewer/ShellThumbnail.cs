using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ImageViewer
{
    // Windowsのシェル(エクスプローラーと同じ仕組み)からファイルのサムネイルを取り出す。
    // 動画はImage.FromFileで読めないため、シェルが作る先頭付近のフレームのサムネイルで代用する
    // (外部dllやNuGetを足さずに済むよう、IShellItemImageFactoryを直接P/Invokeで呼んでいる)
    public static class ShellThumbnail
    {
        [Flags]
        private enum SiigbfFlags
        {
            ResizeToFit = 0x00,
            BiggerSizeOk = 0x01,
            MemoryOnly = 0x02,
            IconOnly = 0x04,
            ThumbnailOnly = 0x08,
            InCacheOnly = 0x10,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeSize
        {
            public int Width;
            public int Height;
        }

        [ComImport]
        [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItemImageFactory
        {
            [PreserveSig]
            int GetImage(NativeSize size, SiigbfFlags flags, out IntPtr phbm);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        private static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] String path,
            IntPtr pbc,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItemImageFactory ppv);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);

        // サムネイルを取り出す。取れないとき(コーデックが無い・壊れた動画・ファイルが消えた等)はnullを返す。
        // アイコンで代用すると画像の一覧に紛れて分かりにくいので、本物のサムネイルだけを対象にする(ThumbnailOnly)
        public static Bitmap GetThumbnail(String filePath, int width, int height)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                return null;
            }

            IShellItemImageFactory factory = null;
            IntPtr hBitmap = IntPtr.Zero;
            try
            {
                Guid iid = typeof(IShellItemImageFactory).GUID;
                if (SHCreateItemFromParsingName(filePath, IntPtr.Zero, ref iid, out factory) != 0 || factory == null)
                {
                    return null;
                }

                NativeSize size = new NativeSize { Width = Math.Max(1, width), Height = Math.Max(1, height) };
                // ResizeToFit: 縦横比を保ったまま指定サイズに収めた画像をシェル側で作ってもらう
                // (BiggerSizeOkだとキャッシュ済みの大きな画像がそのまま返り、無駄にメモリを使う)
                if (factory.GetImage(size, SiigbfFlags.ResizeToFit | SiigbfFlags.ThumbnailOnly, out hBitmap) != 0
                    || hBitmap == IntPtr.Zero)
                {
                    return null;
                }

                // FromHbitmapは中身をコピーした別のBitmapを作るので、元のHBITMAPはfinallyで解放してよい
                return Image.FromHbitmap(hBitmap);
            }
            catch (Exception ex) when (ex is ExternalException || ex is ArgumentException
                || ex is InvalidCastException || ex is OutOfMemoryException)
            {
                // COMの呼び出し失敗(COMExceptionはExternalExceptionの派生)・GDI+の変換失敗は「取れなかった」扱い
                return null;
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    DeleteObject(hBitmap);
                }
                if (factory != null)
                {
                    Marshal.ReleaseComObject(factory);
                }
            }
        }
    }
}
