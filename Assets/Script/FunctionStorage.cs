using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FunctionStorage : MonoBehaviour
{
    private BoardManagement boardManagement;
    public List<Vector2Int> directVector = new List<Vector2Int>();
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
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
