using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CPU : GameManagement
{
    public class MaxProfitPosition
    {
        public Vector2Int selectedPosition;
        public int color;
        public int maxFlipCount;
    }
    public int[,] piecePositionCopy = new int[8, 8];
    Dictionary<Vector3Int, MaxProfitPosition> profitPositionList = new Dictionary<Vector3Int, MaxProfitPosition>();
    FunctionStorage storage;
    int turnCopy;

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
        for (int i = 0; i < piecePositionCopy.GetLength(0); i++)
        {
            string str = "";
            for (int j = 0; j < piecePositionCopy.GetLength(1); j++)
            {
                piecePositionCopy[i, j] = originalData[i, j];
                str = str + piecePositionCopy[i, j] + " ";
            }
            Debug.Log(str);
        }
    }

    public Vector2Int Action(int turn)
    {
        turnCopy = turn;
        MaxProfitPosition temp = new MaxProfitPosition()
        {
            maxFlipCount = 0
        };
        int progress = 0;
        for (int i = 0; i < 3; i++)
        {
            foreach (Vector3Int key in profitPositionList.Keys)
            {
                if (key.x > 0)
                {
                    if (key.y > 0)
                    {
                        if (key.z > 0)
                        {
                            progress = 3;
                        }
                        else
                        {
                            progress = 2;
                        }
                    }
                    else
                    {
                        progress = 1;
                    }
                }
                else
                {
                    progress = 0;
                }
                Debug.Log("progress:" + progress);
                break;
            }
            FindValidMoves(turn, progress);
            turn++;
        }
        foreach (Vector3Int key in profitPositionList.Keys)
        {
            if (temp.maxFlipCount < profitPositionList[key].maxFlipCount)
            {
                temp = profitPositionList[key];
            }
        }
        Debug.Log("temp:" + temp.selectedPosition);
        return temp.selectedPosition;
    }

    public void FindValidMoves(int turn, int progress)
    {
        for (int i = 0; i < piecePositionCopy.GetLength(0); i++)
        {
            for (int j = 0; j < piecePositionCopy.GetLength(1); j++)
            {
                MaxProfitPosition profitPosition = new MaxProfitPosition();
                int tempCount = Judge(turn, new Vector2Int(i, j));
                int key = 1;
                if (tempCount > 0)
                {
                    profitPosition.maxFlipCount = tempCount;
                    profitPosition.selectedPosition = new Vector2Int(i, j);
                    Debug.Log("profitPosition:" + profitPosition.selectedPosition);
                    switch (progress)
                    {
                        case 0:
                            while (profitPositionList.ContainsKey(new Vector3Int(key, 0, 0)))
                            {
                                key++;
                            }
                            Debug.Log("key:" + key.ToString());
                            AssignToList(profitPositionList, new Vector3Int(key, 0, 0), profitPosition);
                            break;
                    }
                }
            }
        }
    }

    // ArrangementDirectから帰ってきた情報をまとめて反映する
    public void Arrangement(int turn, Vector2Int index)
    {
        int player = -2 * (turn % 2) + 1;
        List<Vector2Int> Temporarily = ArrangementDirect(player, index);
        Assignment(index, player);
        for (int i = 0; i < Temporarily.Count; i++)
        {
            Assignment(Temporarily[i], player);
        }
    }

    // ひっくり返す方向と枚数を探索する。
    public List<Vector2Int> ArrangementDirect(int player, Vector2Int index)
    {
        List<Vector2Int> allResults = new List<Vector2Int>();
        foreach (Vector2Int d in storage.directVector)
        {
            List<Vector2Int> candidate = new List<Vector2Int>();
            Vector2Int now = index + d;

            while (0 <= now.x && now.x < 8 && 0 <= now.y && now.y < 8)
            {
                if (piecePositionCopy[now.y, now.x] == 0)
                {
                    break;
                }
                else if (piecePositionCopy[now.y, now.x] == player)
                {
                    allResults.AddRange(candidate);
                    break;
                }
                else if (piecePositionCopy[now.y, now.x] != player)
                {
                    candidate.Add(now);
                }
                now += d;
            }
        }
        return allResults;
    }

    // 指定された位置に駒を置いた時ひっくり返せる枚数を返す。
    public int Judge(int turn, Vector2Int selectedPosition)
    {
        int player = -2 * (turn % 2) + 1;
        if (piecePositionCopy[selectedPosition.y, selectedPosition.x] != 0)
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
                if (piecePositionCopy[now.y, now.x] == 0)
                {
                    break;
                }
                else if (piecePositionCopy[now.y, now.x] == player)
                {
                    points += directionPoints;
                    break;
                }
                else if (piecePositionCopy[now.y, now.x] != player)
                {
                    directionPoints++;
                }
                now += d;
            }
        }
        // Debug.Log(allResults);
        return points;
    }

    private void AssignToList(Dictionary<Vector3Int, MaxProfitPosition> list, Vector3Int key, MaxProfitPosition element)
    {
        list.Add(key, element);
    }

    private void Assignment(int x, int y, int color)
    {
        piecePositionCopy[y, x] = color;
    }

    private void Assignment(Vector2Int location, int color)
    {
        Assignment(location.x, location.y, color);
    }

    private void Assignment(MaxProfitPosition maxProfitPosition)
    {
        Assignment(maxProfitPosition.selectedPosition, maxProfitPosition.color);
    }
}
