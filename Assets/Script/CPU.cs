using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CPU : MonoBehaviour
{
    public struct MaxProfitPosition
    {
        public Vector2Int selectedPosition;
        public int maxFlipCount;
    }
    public int[,] piecePosition = new int[8, 8];
    List<Vector2Int> computerMoveIndices = new List<Vector2Int>();
    FunctionStorage storage;
    // Start is called before the first frame update
    void Start()
    {
        storage = GetComponent<FunctionStorage>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Copy(int[,] originalData)
    {
        for (int i = 0; i < piecePosition.GetLength(0); i++)
        {
            for (int j = 0; j < piecePosition.GetLength(1); j++)
            {
                piecePosition[i, j] = originalData[i, j];
            }
        }

    }

    public Vector2Int Action(int turn)
    {
        MaxProfitPosition profitPosition = new MaxProfitPosition() { maxFlipCount = 0};

        for (int i = 0; i < piecePosition.GetLength(0); i++)
        {
            for (int j = 0; j < piecePosition.GetLength(1); j++)
            {
                int tempCount = Judge(turn, new Vector2Int(i, j));
                if(profitPosition.maxFlipCount < tempCount)
                {
                    profitPosition.maxFlipCount = tempCount;
                    profitPosition.selectedPosition = new Vector2Int(i, j);
                }
            }
        }
        Debug.Log(profitPosition.selectedPosition);
        return profitPosition.selectedPosition;
    }

    public int Judge(int turn, Vector2Int selectedPosition)
    {
        int player = -2 * (turn % 2) + 1;
        if (piecePosition[selectedPosition.y, selectedPosition.x] != 0)
        {
            return 0;
        }

        return Direct(player, selectedPosition);
    }

    public int Direct(int player, Vector2Int index)
    {
        int points = 0;
        foreach (Vector2Int d in storage.directVector)
        {

            Vector2Int now = index + d;
            int directionPoints = 0;

            while (0 <= now.x && now.x < 8 && 0 <= now.y && now.y < 8)
            {
                if (piecePosition[now.y, now.x] == 0)
                {
                    break;
                }
                else if (piecePosition[now.y, now.x] == player)
                {
                    points += directionPoints;
                    break;
                }
                else if (piecePosition[now.y, now.x] != player)
                {
                    directionPoints++;
                }
                now += d;
            }
        }
        //Debug.Log(allResults);
        return points;
    }

    //位置とひっくり返せる枚数を二回以上渡すと前回のflipCountと比べて数が大きい方のindexを返す
    // public Vector2Int compare_and_return_larger_set(Vector2Int index,int flipCount)
    // {

    // }
}
