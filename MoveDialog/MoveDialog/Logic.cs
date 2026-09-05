using System;

namespace MoveDialog
{
    /// <summary>
    /// もともと Form1.cs の getSubValue / getAddValue に実装されていた、子ウィンドウの
    /// 移動先座標を画面端でクランプするロジックをテストできる形に切り出したもの。
    /// コードはそのまま移しただけで書き換えていない。引数はもともと private メソッドの
    /// 時点でFormコントロールに依存していなかったため、そのまま踏襲した。
    /// </summary>
    internal static class Logic
    {
        public static int GetSubValue(int currentValue, int distanceValue, int offsetValue = 0)
        {
            int determiningValue = currentValue - distanceValue - offsetValue;
            if (determiningValue < 0)
            {
                // 座標が負値になる場合、画面端に張り付く
                determiningValue = 0;
            }

            return determiningValue;
        }

        public static int GetAddValue(int maxValue, int currentValue, int distanceValue, int offsetValue = 0)
        {
            int determiningValue = currentValue + distanceValue;
            if (determiningValue > maxValue - offsetValue)
            {
                // 座標が画角を超える場合、画面端に張り付く
                determiningValue = maxValue - offsetValue;
            }

            return determiningValue;
        }
    }
}
