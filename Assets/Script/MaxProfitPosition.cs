using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaxProfitPosition : IEquatable<MaxProfitPosition>
{
    public FlexibleVector2 SelectedPosition; // 選択された位置を表す（x, y座標）
    public int Turn;
    public int MaxFlipCount;
    public int StaticStoneCount;
    public int RatingValue;

    public bool Equals(MaxProfitPosition value)
    {
        return SelectedPosition.Equals(value.SelectedPosition) && Turn == value.Turn && MaxFlipCount == value.MaxFlipCount && StaticStoneCount == value.StaticStoneCount && RatingValue == value.RatingValue;
    }

    // 2. object.Equals()のオーバーライド（汎用性）
    public override bool Equals(object obj)
    {
        // 型チェックしてからIEquatable<T>に委譲
        return obj is MaxProfitPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SelectedPosition, Turn, MaxFlipCount, StaticStoneCount, RatingValue);
    }
}
