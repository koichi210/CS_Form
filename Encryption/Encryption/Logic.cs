using System;
using System.Linq;
using System.Text;

namespace Encryption
{
    /// <summary>
    /// もともと Form1.cs の button_Execute_Click に埋め込まれていた、置換テーブルに
    /// よる数字の暗号化/復号ロジックをテストできる形に切り出したもの。
    /// textBox_Table.Text などのコントロール参照は、呼び出し元(Form1)で読み取った値を
    /// 引数として渡す形に変えた。
    /// </summary>
    internal static class Logic
    {
        public static String Execute(String tableText, Boolean isDecode, String keyText)
        {
            int[] encodeTable = tableText.Split(' ').Select(int.Parse).ToArray();

            // Key
            int keyNumber = int.Parse(keyText);

            // decodeのときはテーブルを反転
            int[] table = encodeTable;
            if (isDecode)
            {
                table = (int[])encodeTable.Clone();
                for (int i = 0; i < encodeTable.Length; i++)
                {
                    table[encodeTable[i]] = i;
                }
            }

            // 下の桁から順に置換し、先頭へ挿入していく
            StringBuilder result = new StringBuilder();
            while (keyNumber != 0)
            {
                int digit = keyNumber % 10;
                keyNumber /= 10;
                result.Insert(0, table[digit]);
            }

            return result.ToString();
        }
    }
}
