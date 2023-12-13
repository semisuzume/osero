using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.UI;
using UnityEngine;

public class ArrangementTest : MonoBehaviour
{
    private BoardManagement boardManagement;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        boardManagement = GetComponent<BoardManagement>();

        //Debug.Assert(Test1(), "Test1");
    }

    public void Init()
    {
        for (int x = 0; x < boardManagement.piecePosition.GetLength(0); x++)
        {
            for (int y = 0; y < boardManagement.piecePosition.GetLength(1); y++)
            {
                boardManagement.Assignment(x, y, 0);
            }
        }
    }

    public void BoardPrint(int[,] piecePosition)
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

    /// <summary>
    /// ç∂Ç©ÇÁÅZÅúÅ~Ç≈íuÇ≠
    /// ëzíËÅFtrue
    /// </summary>
    /// <returns></returns>
    private bool Test1()
    {
        bool count = true;

        int[,] idealPosition = new int[8, 8];
        idealPosition[0, 0] = 1;
        idealPosition[0, 1] = 1;
        idealPosition[0, 2] = 1;

        Init();
        boardManagement.Assignment(0, 0, 1);
        boardManagement.Assignment(1, 0, -1);

        boardManagement.index = new Vector2Int(2, 0);

        boardManagement.Arrangement(2);

        BoardPrint(boardManagement.piecePosition);
        BoardPrint(idealPosition);

        for (int Y = 0;Y < idealPosition.GetLength(0); Y++)
        {
            for (int X = 0;X < idealPosition.GetLength(1); X++)
            {
                Debug.Log(count);
                count &= idealPosition[Y, X] == boardManagement.piecePosition[Y, X];

                //if(idealPosition[Y, X] != boardManagement.piecePosition[Y, X])
                //{
                //    return false;
                //}
            }
        }

        return count;
    }
}
