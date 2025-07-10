using System;
using UnityEngine;

[Serializable]
public struct FlexibleVector2 : IEquatable<FlexibleVector2>
{
    // 内部はint2つで保存
    public int x;
    public int y;

    // Vector2Intとして扱うプロパティ
    public Vector2Int AsVector2Int => new Vector2Int(x, y);

    // int2つとして扱うプロパティ
    public (int x, int y) AsInts => (x, y);

    // コンストラクタ
    public FlexibleVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public FlexibleVector2(Vector2Int vector)
    {
        this.x = vector.x;
        this.y = vector.y;
    }

    // 暗黙的変換
    public static implicit operator Vector2Int(FlexibleVector2 pos)
    {
        return new Vector2Int(pos.x, pos.y);
    }

    public static implicit operator FlexibleVector2(Vector2Int vector)
    {
        return new FlexibleVector2(vector.x, vector.y);
    }

    // IEquatable実装
    public bool Equals(FlexibleVector2 other)
    {
        return x == other.x && y == other.y;
    }

    public override bool Equals(object obj)
    {
        return obj is FlexibleVector2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}

