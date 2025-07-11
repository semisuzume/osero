using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    enum State
    {
        Test,
        Debug,

        //piecePosition�ɏ����l������
        Init,
        //�}�E�X�ŋ��u���ꏊ��I��
        SelectionPlayer,
        SelectionCPU,
        //�I�����ꂽ�ꏊ�ɋ�u���邩�𔻒肷��
        Judgement,
        //piecePosition�̑I�����ꂽ�ꏊ�ɋ�̏���������,�Ֆʂ̍X�V
        Arrangement,
        //����v���C���[��ύX
        Change,
        //���s���~�߂�
        Result
    }
    State state = State.Init;
    private bool DebugMode = false;
    private BoardManagement boardManagement;
    private CPU cpu;
    private DebugScript debugScript;
    public Vector2Int cellpos;
    private int playerTurn = 0;
    private int blockageCounter = 0;
    private bool callConfirmation;
    public bool isPlayerFirst = true;

    // Start is called before the first frame update
    void Start()
    {
        //state = State.Test;
        boardManagement = GetComponent<BoardManagement>();
        cpu = GetComponent<CPU>();
        debugScript = GetComponent<DebugScript>();
        StartCoroutine("ModeratorFacilitator");
    }

    IEnumerator ModeratorFacilitator()
    {
        while (true)
        {
            yield return null;
            switch (state)
            {
                case State.Init:
                    Debug.Log(state);
                    boardManagement.Init();
                    boardManagement.GeneratePiece();
                    playerTurn = 0;
                    callConfirmation = true;
                    state = State.SelectionPlayer;
                    break;
                case State.SelectionPlayer:
                    if (!boardManagement.BlockageJudgment(playerTurn, blockageCounter))
                    {
                        Debug.Log("手番交代");
                        boardManagement.index = new Vector2Int(-1, -1);
                        state = State.Change;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        Debug.Log(state);
                        Vector3? cellPos3 = null;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                        {
                            cellPos3 = hit.collider.gameObject.transform.position;
                        }
                        if (cellPos3 is null)
                        {
                            break;
                        }
                        cellpos = FunctionStorage.Vector3ToVector2(cellPos3.Value);
                        boardManagement.Intermediary(cellpos);
                        state = State.Judgement;
                    }
                    break;
                case State.SelectionCPU:
                    cpu.Action(isPlayerFirst, playerTurn, boardManagement.piecePosition);
                    cellpos = cpu.ChoiceBranch(isPlayerFirst, playerTurn);
                    if(cellpos == new Vector2Int(-1, -1))
                    {
                        blockageCounter++;
                        boardManagement.index = new Vector2Int(-1, -1);
                        state = State.Change;
                        break;
                    }
                    boardManagement.index = cellpos;
                    yield return new WaitForSeconds(1);
                    state = State.Arrangement;
                    if (DebugMode)
                    {
                        state = State.Debug; break;
                    }
                    break;
                case State.Judgement:
                    if (boardManagement.Judge(playerTurn))
                    {
                        Debug.Log("judge通ったよ");
                        cpu.RemovePlayerUnnecessary(playerTurn, boardManagement.ReturnConversion(cellpos));
                        state = State.Arrangement;
                    }
                    else
                    {
                        Debug.Log("judge通ってないよ");
                        state = State.SelectionPlayer;
                    }
                    break;
                case State.Arrangement:
                    /*
                     * Arrangenment�֐�������Ď��s����
                     */
                    boardManagement.Arrangement(playerTurn);
                    boardManagement.GeneratePiece();
                    state = State.Change;
                    break;
                case State.Change:
                    yield return null;//前回追加したところ
                    if (boardManagement.EndJudge())
                    {
                        playerTurn += 1;
                        if (playerTurn % 2 == 0)
                        {
                            Debug.Log("プレイヤー");
                            state = State.SelectionPlayer;
                        }
                        else
                        {
                            Debug.Log("CPU");
                            state = State.SelectionCPU;
                        }
                    }
                    else
                    {
                        state = State.Result;
                    }
                    break;
                case State.Result:
                    if (callConfirmation)
                    {
                        SceneManager.LoadScene("Result", LoadSceneMode.Additive);
                        callConfirmation = false;
                    }
                    break;
                case State.Debug:
                    debugScript.DebugFunction();
                    state = State.Arrangement;
                    break;
            }
        }
    }
}
