using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUpdate : MonoBehaviour
{
    private CPU cpu;
    private FunctionStorage storage;

    void Awake()
    {
        cpu = GetComponent<CPU>();
        storage = GetComponent<FunctionStorage>();
    }

    /// <summary>
    /// BordManagementが保持している盤面CPU内でいじる用の盤面にコピーする
    /// </summary>
    /// <param name="originalData">元データ</param>
    public int[,] Copy(int[,] originalData)
    {
        int[,] boardPieceCopy = new int[8, 8];

        for (int i = 0; i < originalData.GetLength(0); i++)
        {
            string str = "";
            for (int j = 0; j < boardPieceCopy.GetLength(1); j++)
            {
                boardPieceCopy[i, j] = originalData[i, j];
                str = str + boardPieceCopy[i, j] + " ";
            }
        }

        return boardPieceCopy;
    }

    //引数：現在のターン数、何手目まで探索したか、探索したい枝のkey
    //keyの要素を順番に取り出しその要素を持つListを作成する・・・⓵
    //⓵で作ったListをkeyに持つMaxProfitPosition.SelectedPositionを取得しそこに打った場合の盤面を再現する
    /// <summary>
    /// 渡されたkeyの盤面を再現する
    /// </summary>
    /// <param name="turn">プレイヤーの見ているターン</param>
    /// <param name="key">再現したいkey</param>
    /// <param name="defaultBoard">プレイヤーの見ている盤面</param>
    public int[,] UpdatePiecePositionCopy(int turn, string key, int[,] defaultBoard)
    {
        int[,] board = Copy(defaultBoard);

        for (int i = 0; i <= (cpu.GetProfitPositionList()[key].Turn - turn); i++)
        {
            if (cpu.ReturnKeyElement(key, ",").Count <= (turn + i))
            {
                Debug.Log("<color=green>" + "keyの要素数が足りない" + "</color>" + (turn + i));
                continue;
            }
            if (cpu.ReturnKeyElement(key, ",")[turn + i] == null) continue;
            string stac = null;
            for (int j = 0; j <= turn + i; j++)
            {
                stac += "," + cpu.ReturnKeyElement(key, ",")[j];
            }
            if (cpu.GetProfitPositionList().ContainsKey(stac))
            {
                if (stac.Substring(stac.Length - 1, 1) == "N")
                {
                    stac = stac.Remove(stac.Length - 2, 2);
                }
                if (!(cpu.GetProfitPositionList()[stac].SelectedPosition.x < 0 && cpu.GetProfitPositionList()[stac].SelectedPosition.y < 0))
                {
                    Arrangement(ref board, turn + i, cpu.GetProfitPositionList()[stac].SelectedPosition);
                }
            }
            else
            {
                Debug.Log("違う！");
            }
        }

        return board;
    }

    // ArrangementDirectから帰ってきた情報をまとめて反映する
    public int[,] Arrangement(ref int[,] board, int turn, Vector2Int index)
    {
        int player = -2 * (turn % 2) + 1;
        List<Vector2Int> Temporarily = ArrangementDirect(board, player, index);
        board[index.y, index.x] = player;
        for (int i = 0; i < Temporarily.Count; i++)
        {
            board[Temporarily[i].y, Temporarily[i].x] = player;
        }

        return board;
    }

    // ひっくり返す方向と枚数を探索する。
    public List<Vector2Int> ArrangementDirect(int[,] checkSource, int player, Vector2Int index)
    {
        List<Vector2Int> allResults = new List<Vector2Int>();
        foreach (Vector2Int d in storage.directVector)
        {
            List<Vector2Int> candidate = new List<Vector2Int>();
            Vector2Int now = index + d;

            while (0 <= now.x && now.x < 8 && 0 <= now.y && now.y < 8)
            {
                if (checkSource[now.y, now.x] == 0)
                {
                    break;
                }
                else if (checkSource[now.y, now.x] == player)
                {
                    allResults.AddRange(candidate);
                    break;
                }
                else if (checkSource[now.y, now.x] != player)
                {
                    candidate.Add(now);
                }
                now += d;
            }
        }
        return allResults;
    }
}
