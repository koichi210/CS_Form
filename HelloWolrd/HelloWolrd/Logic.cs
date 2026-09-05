using System;

namespace HelloWolrd
{
    /// <summary>
    /// もともと Main メソッドに直接書かれていた挨拶文字列をテストできる形に
    /// 切り出したもの。Console.WriteLine / Console.ReadKey (キー入力待ちで
    /// 自動テストをハングさせてしまう)は Main 側に残し、ここには文字列を
    /// 返すだけの部分を切り出した。
    /// </summary>
    internal static class Logic
    {
        public static String GetGreeting()
        {
            return "Hello World!";
        }
    }
}
