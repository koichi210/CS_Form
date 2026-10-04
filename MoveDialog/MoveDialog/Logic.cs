using System;

namespace MoveDialog
{
    /// <summary>
    /// もともと Form1.cs の getSubValue / getAddValue に実装されていた、子ウィンドウの
    /// 移動先座標を画面端でクランプするロジックをテストできる形に切り出したもの。
    /// 引数はもともと private メソッドの時点でFormコントロールに依存していなかったため、
    /// そのまま踏襲した。
    /// </summary>
    internal static class Logic
    {
        public static int GetSubValue(int currentValue, int distanceValue, int offsetValue = 0)
        {
            // 座標が負値になる場合、画面端に張り付く
            return Math.Max(currentValue - distanceValue - offsetValue, 0);
        }

        public static int GetAddValue(int maxValue, int currentValue, int distanceValue, int offsetValue = 0)
        {
            // 座標が画角を超える場合、画面端に張り付く
            return Math.Min(currentValue + distanceValue, maxValue - offsetValue);
        }
    }
}
