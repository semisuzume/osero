using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ListComparer : IEqualityComparer<List<int>>
{
    public bool Equals(List<int> x, List<int> y)
    {
        if (x == null || y == null)
            return x == y;

        if (x.Count != y.Count)
            return false;

        for (int i = 0; i < x.Count; i++)
        {
            if (x[i] != y[i])
                return false;
        }

        return true;
    }

    public int GetHashCode(List<int> obj)
    {
        if (obj == null)
            return 0;

        int hash = 17;
        foreach (int item in obj)
        {
            hash = hash * 31 + item.GetHashCode();
        }

        return hash;
    }
}

public class CPU : MonoBehaviour
{
    public class MaxProfitPosition
    {
        public Vector2Int SelectedPosition;
        public int turn;
        public int MaxFlipCount;
        public MaxProfitPosition Parent;
    }
    public int[,] piecePositionCopy = new int[8, 8];
    // List<MaxProfitPosition> profitPositionList = new List<MaxProfitPosition>();
    // Vector2Int[] vector2Ints = new Vector2Int[64];
    // List<List<Vector2Int>> threeMoveAheadPatterns = new List<List<Vector2Int>>();
    Dictionary<List<int>, MaxProfitPosition> profitPositionList = new Dictionary<List<int>, MaxProfitPosition>(new ListComparer());
    FunctionStorage storage;

    // Start is called before the first frame update
    void Start()
    {
        storage = GetComponent<FunctionStorage>();
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

    //探索はFindValidMovesが行う（指定回数実行する）
    //FindValidMovesが探索した結果をprofitPositionListに入れその結果を基に一番利益が出る手をVector2Intで返す
    public Vector2Int Action(int turn)
    {
        MaxProfitPosition temp = new MaxProfitPosition()
        {
            MaxFlipCount = 0
        };
        profitPositionList.Clear();
        profitPositionList.Add(new MaxProfitPosition() { SelectedPosition = new Vector2Int(-1, -1) });
        for (int progress = 0; progress < 3 progress++)
        {
            Debug.Log("progress:" + progress);
            FindValidMoves(turn + progress, progress);
        }
        foreach (List<int> key in profitPositionList.Keys)
        {
            if (temp.MaxFlipCount < profitPositionList[key].MaxFlipCount)
            {
                Debug.Log(key);
                temp = profitPositionList[key];
            }
        }
        Debug.Log("temp:" + temp.SelectedPosition);
        return temp.SelectedPosition;
    }

    //引数:現在のターン数、何手目まで探索したか、n手目で置ける場所の保存されているDictionary、現在の盤面情報
    //ｎ+1手目で置ける場所の保存されているDictionary
    //foreachを使っている為、直接profitPositionListの編集ができない、そのためCopyに候補を入れ後でprofitPositionListに統合する
    //profitPositionListからkeyのprogressの値と同じkeyの長さを持つ現在探索できている候補地を取り出す
    //112行目で取り出した候補地に駒を実際に置いた場合の盤面の再現を114行目のUpdatePiecePositionCopyで行う
    //総当たりで打てる場所を探す116行～118行、121行目:座標（i,j）に打った場合にひっくり返せる枚数をtempCountに保存する
    //一枚でもひっくり返せるのなら（124行）、AssignToListを用いてprofitPositionLsitCopyに保存する
    //全ての探索が終わったらprofitPositionListに統合する
    //ターン数を経過させる
    public void FindValidMoves(int turn, int progress, Dictionary<List<int>, MaxProfitPosition> profitPositionList, int[,] defaultBoard)
    {
        // Dictionary<Vector3Int, MaxProfitPosition> profitPositionListCopy = new Dictionary<Vector3Int, MaxProfitPosition>();
        //List<MaxProfitPosition> profitPositionCopy = new List<MaxProfitPosition>();
        foreach (KeyValuePair<List<int>, MaxProfitPosition> keyValuePair in profitPositionList)
        {
            if (keyValuePair.Key.Count != progress) continue;
            UpdatePiecePositionCopy(turn, progress, keyValuePair.Key);// コピー盤面を用意
            for (int i = 0; i < piecePositionCopy.GetLength(0); i++)
            {
                for (int j = 0; j < piecePositionCopy.GetLength(1); j++)// 64マス全探索
                {
                    MaxProfitPosition profitPosition = new MaxProfitPosition();
                    int tempCount = Judge(turn, new Vector2Int(i, j));
                    int keyToSpecify = 1;
                    if (tempCount > 0)
                    {
                        profitPosition.MaxFlipCount = tempCount;
                        profitPosition.SelectedPosition = new Vector2Int(i, j);

                        AssignToList(profitPositionListCopy, keysCopy, profitPosition);
                    }
                }
            }
        }
        // 一時保存したデータをオリジナル（profitPositionList）に.Add
    }

    //引数：現在のターン数、何手目まで探索したか、探索したい枝のkey
    //keyの要素を順番に取り出しその要素を持つListを作成する・・・⓵
    //⓵で作ったListをkeyに持つMaxProfitPosition.SelectedPositionを取得しそこに打った場合の盤面を再現する
    private void UpdatePiecePositionCopy(int turn, int progress, List<int> key)
    {
        List<List<int>> keysCopy = new List<List<int>>();// 指定するためのList
        for (int i = 0; i <= progress; i++)// 現在のprogressの分だけkeyからコピーする
        {
            List<int> stac = new List<int>();
            for (int j = 0; j < i; j++)
            {
                stac.Add(key[j]);
            }
            if (profitPositionList[stac].SelectedPosition != new Vector2Int(-1, -1))
            {
                Arrangement(turn, profitPositionList[stac].SelectedPosition);
            }
        }
    }

    private void UpdatePiecePositionCopy(MaxProfitPosition maxProfitPosition)
    {
        if (maxProfitPosition.SelectedPosition != new Vector2Int(-1, -1))
        {
            Arrangement(maxProfitPosition.turn, maxProfitPosition.SelectedPosition);
        }
    }

    private void UpdatePiecePositionCopy(int turn, Vector2Int selectedPosition)
    {
        if (selectedPosition != new Vector2Int(-1, -1))
        {
            Arrangement(turn, selectedPosition);
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

    private void AssignToList(Dictionary<List<int>, MaxProfitPosition> list, List<int> key, MaxProfitPosition element)
    {
        list.Add(key, element);
    }

    private void AssignToList(Dictionary<List<int>, MaxProfitPosition> list, Dictionary<List<int>, MaxProfitPosition> list2)
    {
        foreach (List<int> keyCopy in list2.Keys)
        {
            AssignToList(list, keyCopy, list2[keyCopy]);
        }
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
        Assignment(maxProfitPosition.SelectedPosition, maxProfitPosition.Color);
    }

    private void AllDelete(Dictionary<List<int>, MaxProfitPosition> list)
    {
        List<List<int>> keys = new List<List<int>>();
        foreach (List<int> key in list.Keys)
        {
            keys.Add(key);
        }
        foreach (List<int> key in keys)
        {
            list.Remove(key);
        }
    }

    public string ListPrint(IReadOnlyList<int> ints)
    {
        string a = "";
        foreach (int keyCopy in ints)
        {
            a += " " + keyCopy;
        }
        return a;
    }
}
