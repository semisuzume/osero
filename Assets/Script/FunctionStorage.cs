using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FunctionStorage : MonoBehaviour
{
    private BoardManagement boardManagement;
    public List<Vector2Int> directVector = new List<Vector2Int>();
    public Vector2Int[] cornerPos = new Vector2Int[4];
    // Start is called before the first frame update
    void Start()
    {
        InitDirectVector();
        InitCornerPos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitDirectVector()
    {
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!(x == y && x == 0))
                {
                    directVector.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    public void InitCornerPos()
    {
        int counter = 0;
        for(int x = 0; x <= 7; x += 7)
        {
            for (int y = 0; y <= 7; y += 7)
            {
                cornerPos[counter] = new Vector2Int(x,y);
                counter++;
            }
        }
    }

    public static Vector2Int Vector3ToVector2(Vector3 vector3)
    {
        return new Vector2Int((int)(vector3.x + 0.5f), -(int)(-vector3.z + 0.5f));
    }

    public static Vector2Int PosToIndex(Vector2Int pos)
    {
        return new Vector2Int((pos.x + 7) / 2, -(pos.y - 7) / 2);
    }

    public static Vector3 IndexToPos(Vector2Int index)
    {
        return new Vector3(index.x * 2 - 7, 0.3f, -(index.y * 2 - 7));
    }
}
