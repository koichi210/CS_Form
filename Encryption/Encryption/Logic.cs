using System;
using System.Linq;

namespace Encryption
{
    /// <summary>
    /// もともと Form1.cs の button_Execute_Click に埋め込まれていた、置換テーブルに
    /// よる数字の暗号化/復号ロジックをテストできる形に切り出したもの。コードは
    /// そのまま移しただけで書き換えていない。textBox_Table.Text などのコントロール
    /// 参照は、呼び出し元(Form1)で読み取った値を引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static String Execute(String tableText, Boolean isDecode, String keyText)
        {
            string[] tableTokens = tableText.Split(' ');
            int[] table = tableTokens.Select(str => int.Parse(str)).ToArray();

            // Key
            int keyNumber = int.Parse(keyText);

            // decodeのときはテーブルを反転
            if (isDecode)
            {
                int[] encodeTable = tableTokens.Select(str => int.Parse(str)).ToArray();
                for (int i = 0; i < encodeTable.Length; i++)
                {
                    table[encodeTable[i]] = i;
                }
            }

            string result = "";
            while (keyNumber != 0)
            {
                int digit = keyNumber % 10;
                keyNumber /= 10;
                result = table[digit].ToString() + result;
            }

            return result;
        }
    }
}
