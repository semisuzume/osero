using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    enum State
    {
        //piecePositionに初期値を入れる
        Init,
        //マウスで駒を置く場所を選択
        Selection,
        //選択された場所に駒が置けるかを判定する
        Judgement,
        //piecePositionの選択された場所に駒の情報を代入する,盤面の更新
        Arrangement,
        //操作プレイヤーを変更
        Change
    }
    State state = State.Init;
    private BoardManagement boardManagement;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(state);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Init:
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log(state);
                    boardManagement.Init();
                    state = State.Selection;
                }
                break;
            case State.Selection:
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log(state);
                    state = State.Judgement;
                }
                break;
            case State.Judgement:
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log(state);
                    state = State.Arrangement;
                }
                break;
            case State.Arrangement:
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log(state);
                    state = State.Change;
                }
                break;
            case State.Change:
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log(state);
                    state = State.Selection;
                }
                break;
        }

    }

    
}
