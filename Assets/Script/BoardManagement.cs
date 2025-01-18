using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

public class BoardManagement : MonoBehaviour
{
    /// <summary>
    /// 白 = 1,
    /// 黒 = -1,
    /// null=0
    /// </summary>
    public int[,] piecePosition = new int[8, 8];
    GameManagement gameManagement;
    public GameObject piece;
    public GameObject nullObject;
    FunctionStorage storage;
    public Vector2Int index;

    // Start is called before the first frame update
    void Start()
    {
        storage = GetComponent<FunctionStorage>();
        piece = Resources.Load("osero_piece") as GameObject;
        nullObject = Resources.Load("null") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        for (int y = 0; y < piecePosition.GetLength(0); y++)
        {
            for (int x = 0; x < piecePosition.GetLength(1); x++)
            {
                if (x == y && (x == 3 || x == 4))
                {
                    Assignment(x, y, -1);
                }
                else if (y == 7 - x && (y == 3 || y == 4))
                {
                    Assignment(x, y, 1);
                }
                else
                {
                    Assignment(x, y, 0);
                }
            }
        }
        //BoardPrint();
    }

    public void BoardPrint()
    {
        for (int Y = 0; Y < piecePosition.GetLength(0); Y++)
        {
            string printString = "";
            for (int X = 0; X < piecePosition.GetLength(1); X++)
            {
                printString += piecePosition[Y, X] + ",";
            }
            Debug.Log(printString);
        }
    }

    public void ListPrint(int[,] list)
    {
        for (int i = 0; i < list.GetLength(0); i++)
        {
            string str = "";
            for (int j = 0; j < list.GetLength(1); j++)
            {
                str = str + list[i, j] + " ";
            }
            Debug.Log(str);
        }
    }

    public void Intermediary(Vector2Int cellpos)
    {
        index = FunctionStorage.PosToIndex(cellpos);
        Debug.Log(index);
    }

    public Vector2Int ReturnConversion(Vector2Int cellpos)
    {
        return FunctionStorage.PosToIndex(cellpos);
    }

    /// <summary>
    /// �����P,����-1,null��0
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    public void Assignment(int x, int y, int color)
    {
        piecePosition[y, x] = color;
    }

    public void Assignment(Vector2Int location, int color)
    {
        Assignment(location.x, location.y, color);
    }

    public void Assignment(Vector3Int locationAndcolor)
    {
        Assignment(locationAndcolor);
    }

    /// <summary>
    /// �u���鎞(true)
    /// �E���łɋ�u����Ă��Ȃ��i�z�����null)
    /// �E����̋�����߂�
    /// </summary>
    /// <param name="turn"></param>
    /// <returns></returns>
    public bool Judge(int turn)
    {
        int player = -2 * (turn % 2) + 1;
        if (piecePosition[index.y, index.x] != 0)
        {
            return false;
        }

        return Direct(player);
    }

    public bool Direct(int player)
    {
        bool allResults = false;
        foreach (Vector2Int d in storage.directVector)
        {

            Vector2Int now = index + d;
            bool sandwiching = false;

            while (0 <= now.x && now.x < 8 && 0 <= now.y && now.y < 8)
            {
                if (piecePosition[now.y, now.x] == 0)
                {
                    allResults |= false;
                    break;
                }
                else if (piecePosition[now.y, now.x] == player)
                {
                    allResults |= sandwiching;
                    break;
                }
                else if (piecePosition[now.y, now.x] != player)
                {
                    sandwiching = true;
                }
                now += d;
            }
        }
        //Debug.Log(allResults);
        return allResults;
    }

    //�u����ꏊ������Ȃ�true
    public bool BlockageJudgment(int turn, int Counter)
    {
        if (Counter == 1)
        {

        }
        for (int i = 0; i < piecePosition.GetLength(0); i++)
        {
            for (int j = 0; j < piecePosition.GetLength(1); j++)
            {
                if (Judge(turn, new Vector2Int(i, j)) > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// indexを渡すとひっくり返せる枚数を返してくれる
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public int Judge(int turn, Vector2Int index)
    {
        int player = -2 * (turn % 2) + 1;
        if (piecePosition[index.y, index.x] != 0)
        {
            return 0;
        }
        return Direct(player, index);
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
        //Debug.Log(points);
        return points;
    }

    // ArrangementDirectから帰ってきた情報をまとめて反映する
    public void Arrangement(int turn)
    {
        int player = -2 * (turn % 2) + 1;
        List<Vector2Int> Temporarily = ArrangementDirect(player);
        Assignment(index, player);
        for (int i = 0; i < Temporarily.Count; i++)
        {
            Assignment(Temporarily[i], player);
        }
    }

    // ひっくり返す方向と枚数を探索する。
    public List<Vector2Int> ArrangementDirect(int player)
    {
        List<Vector2Int> allResults = new List<Vector2Int>();
        foreach (Vector2Int d in storage.directVector)
        {
            List<Vector2Int> candidate = new List<Vector2Int>();
            Vector2Int now = index + d;

            while (0 <= now.x && now.x < 8 && 0 <= now.y && now.y < 8)
            {
                if (piecePosition[now.y, now.x] == 0)
                {
                    //�I���
                    break;
                }
                else if (piecePosition[now.y, now.x] == player)
                {
                    //�����m�肷��
                    allResults.AddRange(candidate);
                    break;
                }
                else if (piecePosition[now.y, now.x] != player)
                {
                    //���̒ǉ�
                    candidate.Add(now);
                }
                now += d;
            }
        }
        return allResults;
    }

    public void GeneratePiece()
    {
        GameObject obj;
        foreach (GameObject destroyPiece in GameObject.FindGameObjectsWithTag("osero"))
        {
            Destroy(destroyPiece);
        }
        for (int y = 0; y < piecePosition.GetLength(0); y++)
        {
            for (int x = 0; x < piecePosition.GetLength(1); x++)
            {
                if (piecePosition[y, x] != 0)
                {
                    obj = Instantiate(piece);
                    obj.tag = "osero";
                    obj.transform.position = FunctionStorage.IndexToPos(new Vector2Int(x, y));
                    if (piecePosition[y, x] == -1)
                    {
                        obj.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.black;
                    }
                    else
                    {
                        obj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.black;
                    }
                }
                else
                {
                    obj = Instantiate(nullObject);
                }
            }
        }
    }

    public bool EndJudge()
    {
        for (int y = 0; y < piecePosition.GetLength(0); y++)
        {
            for (int x = 0; x < piecePosition.GetLength(1); x++)
            {
                if (piecePosition[y, x] == 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
