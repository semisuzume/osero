using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxProfitPosition
{
    public bool[,] ConfirmedStone { get; set; } = new bool[8, 8];
    public Vector2Int SelectedPosition { get; set; }
    public int Turn { get; set; }
    public int MaxFlipCount { get; set; }
    /// <summary>
    /// 確定石
    /// </summary>
    public int StaticStoneCount { get; set; }
    public int RatingValue { get; set; }
}
