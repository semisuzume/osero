using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public struct SearchMove : IJob
{
    public BoardUpdate jobBoardUpdate;
    public CPU jobCpu;
    public FunctionStorage jobStorage;

    public NativeHashMap<FixedString512Bytes, MaxProfitPosition> searchResults;
    public Dictionary<string, MaxProfitPosition> profitPositionListCopy;

    public int[,] currentBoard;
    public List<Vector2Int> directVector;
    public string key;
    public string originKeyInformation;

    public int whichTurn;
    public int turn;
    public int progress;

    public string startPointKey;

    public void Execute()
    {
        int[,] commonBoard = UpdatePiecePositionCopy(turn, startPointKey, currentBoard);// コピー盤面を用意
        bool DoSkip = true;

        for (int i = 0; i < currentBoard.GetLength(0); i++)
        {
            for (int j = 0; j < currentBoard.GetLength(1); j++)// 64マス全探索
            {
                MaxProfitPosition valueInformation = new MaxProfitPosition();
                int tempCount = whichTurn * Judge(commonBoard, key == ",-1" ? profitPositionListCopy[key].Turn : searchResults[key].Turn + 1, new Vector2Int(i, j)); //ポイントi,jに駒を置いた場合にひっくり返せる枚数を探索
                int keyToSpecify = 1;//Dictionaryのkeyの初期値
                if (tempCount != 0)
                {
                    string keyInformation;
                    DoSkip = false;
                    do
                    {
                        keyInformation = originKeyInformation + "," + keyToSpecify.ToString();
                        keyToSpecify++;
                    } while (searchResults.ContainsKey(keyInformation));
                    if (turn == 0 || turn == 1) valueInformation.Turn = turn + progress;
                    else valueInformation.Turn = searchResults[key].Turn + 1;
                    valueInformation.MaxFlipCount = tempCount;
                    valueInformation.SelectedPosition = new Vector2Int(i, j);
                    searchResults.Add(keyInformation, valueInformation);
                }
            }
        }
        if (DoSkip)
        {
            searchResults.Add(originKeyInformation + "," + "N", new MaxProfitPosition() { MaxFlipCount = 0, Turn = turn + progress });
        }
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

        for (int i = 0; i <= (this.jobCpu.GetProfitPositionList()[key].Turn - turn); i++)
        {
            if (this.jobCpu.ReturnKeyElement(key, ",").Count <= (turn + i))
            {
                Debug.Log("<color=green>" + "keyの要素数が足りない" + "</color>" + (turn + i));
                continue;
            }
            if (jobCpu.ReturnKeyElement(key, ",")[turn + i] == null) continue;
            string stac = null;
            for (int j = 0; j <= turn + i; j++)
            {
                stac += "," + jobCpu.ReturnKeyElement(key, ",")[j];
            }
            if (jobCpu.GetProfitPositionList().ContainsKey(stac))
            {
                if (stac.Substring(stac.Length - 1, 1) == "N")
                {
                    stac = stac.Remove(stac.Length - 2, 2);
                }
                if (!(jobCpu.GetProfitPositionList()[stac].SelectedPosition.x < 0 && jobCpu.GetProfitPositionList()[stac].SelectedPosition.y < 0))
                {
                    Arrangement(ref board, turn + i, jobCpu.GetProfitPositionList()[stac].SelectedPosition);
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
        foreach (Vector2Int d in jobStorage.directVector)
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

    // 指定された位置に駒を置いた時ひっくり返せる枚数を返す。
    public int Judge(int[,] sourceBoard, int turn, Vector2Int selectedPosition)
    {
        int player = -2 * (turn % 2) + 1;
        if (sourceBoard[selectedPosition.y, selectedPosition.x] != 0)
        {
            return 0;
        }

        return Direct(sourceBoard, player, selectedPosition);
    }

    public int Direct(int[,] sourceBoard, int player, Vector2Int index)
    {
        int points = 0;
        foreach (Vector2Int d in directVector)
        {
            Vector2Int now = index + d;
            int directionPoints = 0;
            while (0 <= now.x && now.x < 8 && 0 <= now.y && now.y < 8)
            {
                if (sourceBoard[now.y, now.x] == 0)
                {
                    break;
                }
                else if (sourceBoard[now.y, now.x] == player)
                {
                    points += directionPoints;
                    break;
                }
                else if (sourceBoard[now.y, now.x] != player)
                {
                    directionPoints++;
                }
                now += d;
            }
        }
        // Debug.Log(allResults);
        return points;
    }
}
