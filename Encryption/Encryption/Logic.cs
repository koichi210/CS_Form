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
            var line = tableText;

            string[] key_s = line.Split(' ');
            int[] key_i = key_s.Select(str => int.Parse(str)).ToArray();

            bool encdec = true;
            if (isDecode)
            {
                encdec = false;
            }

            // Key
            int word = int.Parse(keyText);

            // decodeのときはテーブルを反転

            if (!encdec)
            {
                int[] key_ii = key_s.Select(str => int.Parse(str)).ToArray();
                for (int i = 0; i < key_ii.Length; i++)
                {
                    int idx = key_ii[i];
                    key_i[idx] = i;
                }
            }

            string ans = "";
            while (word != 0)
            {
                int idx = word % 10;
                word /= 10;
                ans = key_i[idx].ToString() + ans;
            }

            return ans;
        }
    }
}
