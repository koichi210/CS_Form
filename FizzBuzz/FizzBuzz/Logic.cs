using System;
using System.Text;

namespace FizzBuzz
{
    /// <summary>
    /// もともと Form1.cs の private メソッド FizzBuzz に実装されていた、
    /// FizzBuzz(3の倍数)+Woof(7の倍数)拡張版のロジックをテストできる形に
    /// 切り出したもの。コードはそのまま移しただけで書き換えていない。
    /// </summary>
    internal static class Logic
    {
        public static String FizzBuzz(int number)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 1; i <= number; i++)
            {
                result.Append(i).Append(" : ");

                String label = "";
                if (i % 3 == 0)
                {
                    label += "Fizz";
                }
                if (i % 5 == 0)
                {
                    label += "Buzz";
                }
                if (i % 7 == 0)
                {
                    label += "Woof";
                }

                if (label == String.Empty)
                {
                    label = i.ToString();
                }

                result.Append(label).Append(Environment.NewLine);
            }

            return result.ToString();
        }
    }
}
