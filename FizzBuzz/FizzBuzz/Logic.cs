using System;

namespace FizzBuzz
{
    /// <summary>
    /// もともと Form1.cs の private メソッド FizzBuzz に実装されていた、
    /// FizzBuzz(3の倍数)+Woof(7の倍数)拡張版のロジックをテストできる形に
    /// 切り出したもの。コードはそのまま移しただけで書き換えていない。
    /// </summary>
    internal static class Logic
    {
        public static String FizzBuzz(int Number)
        {
            String Result = "";

            for (int i = 1; i <= Number; i++)
            {
                Result += i.ToString() + " : ";

                String Add = "";
                if (i % 3 == 0)
                {
                    Add += "Fizz";
                }
                if (i % 5 == 0)
                {
                    Add += "Buzz";
                }
                if (i % 7 == 0)
                {
                    Add += "Woof";
                }

                if (Add == String.Empty)
                {
                    Add = i.ToString();
                }

                Result += Add + Environment.NewLine;
            }

            return Result;
        }
    }
}
