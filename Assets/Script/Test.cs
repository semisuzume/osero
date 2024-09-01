using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    BoardManagement boardManagement;

    private void Start()
    {
        //boardManagement = GetComponent<BoardManagement>();
        //Debug.Assert(Test1(),   "Test01");
        //Debug.Assert(Test2(),   "Test02");
        //Debug.Assert(Test3(),   "Test03");
        //Debug.Assert(Test4(),   "Test04");
        //Debug.Assert(Test5(),   "Test05");
        //Debug.Assert(Test6(),   "Test06");
        //Debug.Assert(Test7(),   "Test07");
        //Debug.Assert(Test8(),   "Test08");
        //Debug.Assert(Test9(),   "Test09");
        //Debug.Assert(Test10(),  "Test10");
        //Debug.Assert(Test11(),  "Test11");
        //Debug.Assert(Test12(),  "Test12");
        //Debug.Assert(Test13(),  "Test13");
        //Debug.Assert(Test14(),  "Test14");
        //Debug.Assert(Test15(),  "Test15");
        //Debug.Assert(Test16(),  "Test16");
        //Debug.Assert(Test17(),  "Test17");
        //Debug.Assert(Test18(),  "Test18");
        //Debug.Assert(Test19(),  "Test19");
        //Debug.Assert(Test20(),  "Test20");
        //Debug.Assert(Test21(),  "Test21");
        //Debug.Assert(Test22(),  "Test22");
        //Debug.Assert(Test23(),  "Test23");
        //Debug.Assert(Test24(),  "Test24");
        //Debug.Assert(Test25(),  "Test25");
        //Debug.Assert(Test26(),  "Test26");
        //Debug.Assert(Test27(),  "Test27");
        //Debug.Assert(Test28(),  "Test28");
        //Debug.Assert(Test29(),  "Test29");
        
        //ArrayTest();
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

    /// <summary>
    /// ������Z���~�Œu��
    /// �z��Ftrue
    /// </summary>
    /// <returns></returns>
    private bool Test1()
    {
        Init();
        boardManagement.Assignment(0, 0, 1);
        boardManagement.Assignment(1, 0, -1);

        boardManagement.index = new Vector2Int(2, 0);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// �����灜���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test2()
    {
        Init();
        boardManagement.Assignment(0, 0, -1);
        boardManagement.Assignment(1, 0, -1);

        boardManagement.index = new Vector2Int(2, 0);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// ������n���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test3()
    {
        Init();
        boardManagement.Assignment(0, 0, 0);
        boardManagement.Assignment(1, 0, -1);

        boardManagement.index = new Vector2Int(2, 0);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// ������Z�����~�Œu��
    /// �z��Ftrue
    /// </summary>
    /// <returns></returns>
    private bool Test4()
    {
        Init();
        boardManagement.Assignment(0, 0, 1);
        boardManagement.Assignment(1, 0, -1);
        boardManagement.Assignment(2, 0, -1);
        
        boardManagement.index = new Vector2Int(3, 0);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// ������Z��N�~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test5() 
    {
        Init();
        boardManagement.Assignment(0, 0, 1);
        boardManagement.Assignment(1, 0, -1);
        boardManagement.Assignment(2, 0, 0);

        boardManagement.index = new Vector2Int(3, 0);
        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// ���ォ��Z���~�Œu��
    /// �z��Ftrue
    /// </summary>
    /// <returns></returns>
    private bool Test6()
    {
        Init();
        boardManagement.Assignment(0, 0, 1);
        boardManagement.Assignment(1, 1, -1);

        boardManagement.index = new Vector2Int(2, 2);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// ���ォ�灜���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test7()
    {
        Init();
        boardManagement.Assignment(0, 0, -1);
        boardManagement.Assignment(1, 1, -1);

        boardManagement.index = new Vector2Int(2, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// ���ォ��n���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test8()
    {
        Init();
        boardManagement.Assignment(0, 0, 0);
        boardManagement.Assignment(1, 1, -1);

        boardManagement.index = new Vector2Int(2, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    ///�ォ��Z���~�Œu��
    /// �z��Ftrue 
    /// </summary>
    /// <returns></returns>
    private bool Test9()
    {
        Init();
        boardManagement.Assignment(0, 0, 1);
        boardManagement.Assignment(0, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// �ォ�灜���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test10()
    {
        Init();
        boardManagement.Assignment(0, 0, -1);
        boardManagement.Assignment(0, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �ォ��n���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test11()
    {
        Init();
        boardManagement.Assignment(0, 0, 0);
        boardManagement.Assignment(0, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �E�ォ��Z���~�Œu��
    /// �z��Ftrue
    /// </summary>
    /// <returns></returns>
    private bool Test12()
    {
        Init();
        boardManagement.Assignment(2, 0, 1);
        boardManagement.Assignment(1, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// �E�ォ�灜���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test13()
    {
        Init();
        boardManagement.Assignment(2, 0, -1);
        boardManagement.Assignment(1, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �E�ォ��n���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test14()
    {
        Init();
        boardManagement.Assignment(2, 0, 0);
        boardManagement.Assignment(1, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �E����Z���~�Œu��
    /// �z��Ftrue
    /// </summary>
    /// <returns></returns>
    private bool Test15()
    {
        Init();
        boardManagement.Assignment(1, 2, -1);
        boardManagement.Assignment(2, 2, 1);

        boardManagement.index = new Vector2Int(0, 2);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// �E���灜���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test16()
    {
        Init();
        boardManagement.Assignment(1, 2, -1);
        boardManagement.Assignment(2, 2, -1);

        boardManagement.index = new Vector2Int(0, 1);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �E����n���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test17()
    {
        Init();
        boardManagement.Assignment(1, 2, 0);
        boardManagement.Assignment(2, 2, -1);

        boardManagement.index = new Vector2Int(0, 1);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �E������Z���~�Œu��
    /// �z��Ftrue
    /// </summary>
    /// <returns></returns>
    private bool Test18()
    {
        Init();
        boardManagement.Assignment(1, 3, -1);
        boardManagement.Assignment(2, 4, 1);

        boardManagement.index = new Vector2Int(0, 2);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// �E�����灜���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test19()
    {
        Init();
        boardManagement.Assignment(1, 3, -1);
        boardManagement.Assignment(2, 4, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �E������n���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test20()
    {
        Init();
        boardManagement.Assignment(2, 4, 0);
        boardManagement.Assignment(1, 3, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// ������Z���~�Œu��
    /// �z��Ftrue
    /// </summary>
    /// <returns></returns>
    private bool Test21()
    {
        Init();
        boardManagement.Assignment(0, 3, -1);
        boardManagement.Assignment(0, 4, 1);

        boardManagement.index = new Vector2Int(0, 2);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// �����灜���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test22()
    {
        Init();
        boardManagement.Assignment(0, 3, -1);
        boardManagement.Assignment(0, 4, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// ������n���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test23()
    {
        Init();
        boardManagement.Assignment(0, 4, 0);
        boardManagement.Assignment(0, 3, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// ��������Z���~�Œu��
    /// �z��Ftrue
    /// </summary>
    /// <returns></returns>
    private bool Test24()
    {
        Init();
        boardManagement.Assignment(0, 3, -1);
        boardManagement.Assignment(0, 4, 1);

        boardManagement.index = new Vector2Int(0, 2);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// �������灜���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test25()
    {
        Init();
        boardManagement.Assignment(0, 3, -1);
        boardManagement.Assignment(0, 4, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// ��������n���~�Œu��
    /// �z��Ffalse
    /// </summary>
    /// <returns></returns>
    private bool Test26()
    {
        Init();
        boardManagement.Assignment(0, 4, 0);
        boardManagement.Assignment(0, 3, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �E����Z���~�A�ォ�灜���~
    /// �z��Ftrue
    /// </summary>
    private bool Test27()
    {
        Init();
        boardManagement.Assignment(1, 2, -1);
        boardManagement.Assignment(2, 2, 1);

        boardManagement.Assignment(0, 0, -1);
        boardManagement.Assignment(0, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return boardManagement.Judge(2);
    }

    /// <summary>
    /// �E���灜���~�A�ォ�灜���~
    /// �z��Ffalse
    /// </summary>
    private bool Test28()
    {
        Init();
        boardManagement.Assignment(1, 2, -1);
        boardManagement.Assignment(2, 2, -1);

        boardManagement.Assignment(0, 0, -1);
        boardManagement.Assignment(0, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return !boardManagement.Judge(2);
    }

    /// <summary>
    /// �E����Z���~�A�ォ��Z���~
    /// �z��Ftrue
    /// </summary>
    private bool Test29()
    {
        Init();
        boardManagement.Assignment(2, 2, 1);
        boardManagement.Assignment(1, 2, -1);

        boardManagement.Assignment(0, 0, 1);
        boardManagement.Assignment(0, 1, -1);

        boardManagement.index = new Vector2Int(0, 2);

        return boardManagement.Judge(2);
    }

    private void ArrayTest()
    {
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                List<Vector2Int> ints = new List<Vector2Int>
                {
                    new Vector2Int(y, x)
                };
                Debug.Log(ints);
            }
        }
    }

    private void Test30()
    {
        Test31 temp1 = new Test31();
        temp1.Child = new Test31();
        temp1.Child.Child = new Test31();
    }

    private Test31 Test31(Test31 test31)
    {
        return Test31(test31.Child);
    }

    private Test31 Test32(Test31 test31)
    {
        Test31 temp1 = test31;
        while (true)
        {
            temp1 = temp1.Child;
            if (temp1.Child == null)
            {
                break;
            }
        }
        return temp1;
    }
}
public class Test31
{
    public Test31 Child;
}