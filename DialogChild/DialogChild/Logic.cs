using System;

namespace DialogChild
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
            int result = currentValue - distanceValue - offsetValue;
            if (result < 0)
            {
                // 座標が負値になる場合、画面端に張り付く
                result = 0;
            }

            return result;
        }

        public static int GetAddValue(int maxValue, int currentValue, int distanceValue, int offsetValue = 0)
        {
            int result = currentValue + distanceValue;
            if (result > maxValue - offsetValue)
            {
                // 座標が画角を超える場合、画面端に張り付く
                result = maxValue - offsetValue;
            }

            return result;
        }
    }
}
