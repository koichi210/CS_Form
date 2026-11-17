using System;
using System.Collections.Generic;

namespace PictMerge
{
    // 切断位置(帯の境目になる高さ)の入力を読む
    internal static class TrimHeights
    {
        // 改行とカンマのどちらで区切ってもよい。空の項目は無視し、小さい順に並べる
        // (上から順に帯の境目として使うので、入力した順番には左右されない)。
        // 「50,100」のような範囲指定は「50と100で切る」と同じ結果になるので、どちらの書き方でも同じように結合できる。
        // 数字でない値があればFormatException、intに収まらなければOverflowExceptionを投げる
        internal static List<int> Parse(String text)
        {
            List<int> trimHeights = new List<int>();
            foreach (String token in text.Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (String.IsNullOrWhiteSpace(token))
                {
                    continue;
                }
                trimHeights.Add(int.Parse(token.Trim()));
            }
            trimHeights.Sort();
            return trimHeights;
        }
    }
}
