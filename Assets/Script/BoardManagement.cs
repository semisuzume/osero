using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManagement : MonoBehaviour
{
    public int[,] piecePosition = new int[8, 8];
    public float[] dividedHeight = new float[9];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.mousePosition);
    }

    public void Init()
    {
        for(int x = 0; x < piecePosition.GetLength(0);x++)
        {
            for(int y = 0; y < piecePosition.GetLength(1); y++)
            {
                if(x == y && (x == 3 || x == 4))
                {
                    Assignment(x, y, 1);
                }
                if(y == 7 - x && (y == 3 || y == 4))
                {
                    Assignment(x, y, -1);
                }
                else
                {
                    Assignment(x, y, 0);
                }
            }
        }
    }

    public void Selection()
    {
        float sh = Screen.height;
        for(int i = 0; i <= 8; i++)
        {
            dividedHeight[i] = sh / 8 * i;
        }
    }

    private void Assignment(int x, int y, int color)
    {
        piecePosition[x, y] = color;
    }
}
