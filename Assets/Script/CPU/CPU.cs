using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

/// <summary>
/// 置ける手の探索と選択
/// </summary>
public class CPU : MonoBehaviour
{
    // AssighmentとCopyからのみ代入可能
    public int[,] piecePositionCopy = new int[8, 8];

    Dictionary<string, MaxProfitPosition> profitPositionList = new Dictionary<string, MaxProfitPosition>();
    Dictionary<string, MaxProfitPosition> searchResults = new Dictionary<string, MaxProfitPosition>();

    GameManagement gameManagement;
    BoardManagement boardManagement;
    BoardUpdate boardUpdate;
    FunctionStorage storage;

    int difficulty = 2;

    // Start is called before the first frame update
    void Start()
    {
        gameManagement = GetComponent<GameManagement>();
        boardManagement = GetComponent<BoardManagement>();
        boardUpdate = GetComponent<BoardUpdate>();
        storage = GetComponent<FunctionStorage>();
    }

    /// <summary>
    /// Dictionaryの初期化と探索用関数を指定回数呼び出して結果を結合する
    /// </summary>
    /// <param name="turn">経過ターン数</param>
    /// <param name="originalData">現在の盤面</param>
    public void Action(bool isPlayerFirst, int turn, int[,] originalData)
    {
        searchResults.Clear();
        for (int progress = 0; progress < difficulty; progress++)
        {
            profitPositionList = profitPositionList.Concat(FindValidMoves(turn, progress, profitPositionList, originalData)
                                                    .Where(pair => !profitPositionList.ContainsKey(pair.Key)))
                                                    .ToDictionary(dicPair => dicPair.Key, dicPair => dicPair.Value);
            if (progress == 0)
            {
                profitPositionList.Remove(",-1");
            }
            Debug.Log("<color=green>" + "探索したターン数: " + (CountThisTurn(turn) * difficulty + progress - difficulty) + "</color>");
        }
        EvaluationAssignment(turn, difficulty, originalData);
    }

    /// <summary>
    /// profitPositionListから最適な手を選択する
    /// </summary>
    /// <param name="turn">経過ターン数</param>
    /// <returns></returns>
    public Vector2Int ChoiceBranch(bool isPlayerFirst, int turn)
    {
        string selectedKey = "a";
        bool DoRun = false;
        MaxProfitPosition temp = new MaxProfitPosition();
        temp.RatingValue = 0;
        foreach (string key in profitPositionList.Keys)
        {
            string keyCode = "";
            int finalResult = 0;
            if (ReturnKeyElement(key, ",").Count != (turn == 0 ? turn : CountThisTurn(turn)) * difficulty + 1)
            {
                Debug.Log("element: " + ReturnKeyElement(key, ",").Count + "比較対象: " + ((turn == 0 ? turn : CountThisTurn(turn)) * difficulty + 1) + $"式: {turn} == 0 ? {turn} : {CountThisTurn(turn)} * {difficulty} + 1");
                continue;
            }
            for (int i = 0; i < (turn == 0 ? turn : CountThisTurn(turn)) * difficulty + 1; i++)
            {
                keyCode = null;
                for (int j = 0; j <= i; j++)
                {
                    keyCode += "," + ReturnKeyElement(key, ",")[j];
                }
                finalResult += profitPositionList[keyCode].RatingValue;
                Debug.Log("keyCode: " + keyCode + " finalResult: " + finalResult);
            }
            //SubStringされるとき失敗するときがある
            if ((temp.RatingValue < finalResult) || (finalResult == 0 && temp.RatingValue == 0) || !DoRun)
            {
                Debug.Log("<color=green>" + "新しいkeyが選択されました" + "</color>");
                selectedKey = "";
                for (int i = 0; i < (isPlayerFirst ? turn + 1 : turn + 2); i++)
                {
                    selectedKey = selectedKey + "," + ReturnKeyElement(keyCode, ",")[i];
                }
                temp = profitPositionList[selectedKey];
                DoRun = true;
            }
        }
        Debug.Log("<color=green>" + "選択されたkey: " + selectedKey + "</color>");
        RemoveUnnecessary(selectedKey);
        if (string.Join(",", ListSlice(ReturnKeyElement(selectedKey, ","), ReturnKeyElement(selectedKey, ",").Count - 1, 1)) == "N")
        {
            Debug.Log("<color=red>" + "選択されたkeyがN" + "</color>");
            return new Vector2Int(-1, -1);
        }
        return temp.SelectedPosition;
    }

    //引数:現在のターン数、何手目まで探索したか、n手目で置ける場所の保存されているDictionary、現在の盤面情報
    //foreachを使っている為、直接profitPositionListの編集ができない、そのためCopyに候補を入れ戻り値で返す
    //profitPositionListからkeyのprogressの値と同じkeyの長さを持つ現在探索できている候補地を取り出す
    //112行目で取り出した候補地に駒を実際に置いた場合の盤面の再現を114行目のUpdatePiecePositionCopyで行う
    //総当たりで打てる場所を探す116行～118行、121行目:座標（i,j）に打った場合にひっくり返せる枚数をtempCountに保存する
    //一枚でもひっくり返せるのなら（124行）、AssignToListを用いてprofitPositionLsitCopyに保存する
    //全ての探索が終わったらprofitPositionListに統合する
    //ターン数を経過させる
    /// <summary>
    /// 置ける手を全探索
    /// </summary>
    /// <param name="turn">経過ターン数</param>
    /// <param name="progress">何手まで探索したか</param>
    /// <param name="profitPositionList">今まで何処に置いたかの保存に使う</param>
    /// <param name="defaultBoard">プレイヤーが見てる盤面</param>
    /// <returns>ｎ+1手目で置ける場所の保存されているDictionary</returns>
    public Dictionary<string, MaxProfitPosition> FindValidMoves(int turn, int progress, Dictionary<string, MaxProfitPosition> profitPositionList, int[,] defaultBoard)
    {
        string originKeyInformation;
        List<JobHandle> jobHandles = new List<JobHandle>();
        List<NativeHashMap<FixedString512Bytes, MaxProfitPosition>> nativeResults = new List<NativeHashMap<FixedString512Bytes, MaxProfitPosition>>();

        foreach (string key in profitPositionList.Keys) //keyから一つ選択
        {
            int whichTurn = (2 * (profitPositionList[key].Turn % 2)) - 1;
            originKeyInformation = ",1"; //keyの初期値
            if (turn + progress <= 1) { }
            else if (ReturnKeyElement(key, ",").Count != CountThisTurn(turn) * difficulty + progress - difficulty + 1) continue; //探索済みのkeyを弾く
            else { originKeyInformation = key;/*末尾にKeyToSpecifyを追加する*/}
            NativeHashMap<FixedString512Bytes, MaxProfitPosition> nativeSearchResults = new NativeHashMap<FixedString512Bytes, MaxProfitPosition>(64, Allocator.TempJob);

            SearchMove searchMove = new SearchMove
            {
                jobBoardUpdate = boardUpdate,
                jobCpu = this,
                jobStorage = storage,
                searchResults = nativeSearchResults,
                profitPositionListCopy = profitPositionList,
                currentBoard = defaultBoard,
                key = key,
                originKeyInformation = originKeyInformation,
                whichTurn = whichTurn,
                turn = turn,
                progress = progress,
                startPointKey = originKeyInformation
            };

            JobHandle jobHandle = searchMove.Schedule();
            jobHandles.Add(jobHandle);
            nativeResults.Add(nativeSearchResults);
        }
        NativeArray<JobHandle> jobHandlesArray = new NativeArray<JobHandle>(jobHandles.ToArray(), Allocator.Temp);
        // 全てのジョブが完了するのを待つ
        JobHandle.CompleteAll(jobHandlesArray);

        // 結果をマージしてクリーンアップ
        for (int i = 0; i < nativeResults.Count; i++)
        {
            var nativeResult = nativeResults[i];
            foreach (var kvp in nativeResult)
            {
                searchResults[kvp.Key.ToString()] = kvp.Value;
            }
            nativeResult.Dispose(); // メモリリークを防ぐ
        }

        // 一時保存したデータを返す
        return searchResults;
    }

    void EvaluationAssignment(int turn, int progress, int[,] defaultBoard)
    {
        Dictionary<string, MaxProfitPosition> collection = new Dictionary<string, MaxProfitPosition>(profitPositionList);
        MaxProfitPosition value;

        foreach (string key in profitPositionList.Keys)
        {
            piecePositionCopy = boardUpdate.UpdatePiecePositionCopy(turn, key, defaultBoard);

            value = collection[key];
            value.RatingValue = EvaluationFunction(piecePositionCopy, key, profitPositionList[key].SelectedPosition);
            collection[key] = value; // 評価値を更新
        }

        profitPositionList = collection; // 評価値をprofitPositionListに反映
    }

    /// <summary>
    /// 評価関数の一番上
    /// </summary>
    /// <param name="evaluationTarget">評価する盤面</param>
    /// <param name="difficultyCopy">何手先まで評価するか</param>
    /// <returns></returns>
    private int EvaluationFunction(int[,] evaluationTarget, string key, Vector2Int selectedPosition)
    {
        return CheckBoardPoint(key, selectedPosition) * 2 + ConfirmedStoneCount(evaluationTarget, key) * 5 - CandidateNumberSearch(key);
    }

    /// <summary>
    /// 確定石の探索
    /// </summary>
    /// <param name="evaluationTarget">評価対象（盤面）</param>
    /// <returns>評価</returns>
    private int ConfirmedStoneCount(int[,] evaluationTarget, string key)
    {
        if (ReturnKeyElement(key, ",").Count == difficulty - 1) return 0;
        int confirmedStone = 0;
        bool[,] verificationFlag = new bool[8, 8];
        Array.Clear(verificationFlag, 0, verificationFlag.Length);
        int pieceColor = -2;
        foreach (Vector2Int cornerPos in storage.cornerPos)
        {
            pieceColor = evaluationTarget[cornerPos.x, cornerPos.y];
            if (pieceColor == 0) continue;
            int colorSwitch = pieceColor;
            confirmedStone += 1 * colorSwitch;
            SearchInformationData searchInformation = new SearchInformationData { };
            searchInformation.searchDirect = false;
            //x座標方向に探索
            for (int i = 1; i <= 7; i++)
            {
                searchInformation.additionDirection.x = cornerPos.x == 0 ? 1 : -1;
                if (verificationFlag[cornerPos.x + (searchInformation.additionDirection.x * i), cornerPos.y]) continue;
                else if (evaluationTarget[cornerPos.x + (searchInformation.additionDirection.x * i), cornerPos.y] == pieceColor)
                {
                    searchInformation.vertical++;
                    verificationFlag[cornerPos.x + (searchInformation.additionDirection.x * i), cornerPos.y] = true;
                }
                else break;
            }
            //y座標方向に探索
            for (int i = 1; i <= 7; i++)
            {
                searchInformation.additionDirection.y = cornerPos.y == 0 ? 1 : -1;
                if (verificationFlag[cornerPos.x, cornerPos.y + (searchInformation.additionDirection.y * i)]) continue;
                if (evaluationTarget[cornerPos.x, cornerPos.y + (searchInformation.additionDirection.y * i)] == pieceColor)
                {
                    searchInformation.horizontal++;
                    verificationFlag[cornerPos.x, cornerPos.y + (searchInformation.additionDirection.y * i)] = true;
                }
                else break;
            }
            confirmedStone += (searchInformation.vertical + searchInformation.horizontal) * colorSwitch;
            if (searchInformation.horizontal >= searchInformation.vertical)
            {
                searchInformation.occupancyRate = searchInformation.horizontal;
                searchInformation.searchDirect = true;
            }
            else
            {
                searchInformation.occupancyRate = searchInformation.vertical;
                searchInformation.searchDirect = false;
            }
            int searchLimit = searchInformation.occupancyRate;
            Vector2Int searchPos = new Vector2Int(0, 0);
            if (searchInformation.searchDirect == true)// serchInformation.horizontalがverticalよりも大きい
            {
                for (int i = 0; i < searchInformation.vertical; i++)
                {
                    searchPos = new Vector2Int(cornerPos.x + (searchInformation.vertical * searchInformation.additionDirection.x) + (-searchInformation.additionDirection.x * i),
                                               cornerPos.y + (searchInformation.horizontal * searchInformation.additionDirection.y) + (-searchInformation.additionDirection.y * i));
                    if (evaluationTarget[searchPos.x, searchPos.y] == pieceColor)
                    {
                        searchLimit = searchLimit > searchPos.y ? searchPos.y : searchLimit;
                        for (int searchFocus = 0; searchFocus < searchLimit; ++searchFocus)
                        {
                            if (evaluationTarget[searchPos.x, searchFocus] != pieceColor) searchLimit = searchFocus - 1;
                            else if (verificationFlag[searchPos.x, searchFocus]) continue;
                            else
                            {
                                verificationFlag[searchPos.x, searchFocus] = true;
                                confirmedStone += 1 * colorSwitch;
                            }
                        }
                    }
                }
            }
            else if (searchInformation.searchDirect == false)
            {
                for (int i = 0; i < searchInformation.horizontal; i++)
                {
                    searchPos = new Vector2Int(cornerPos.x + (searchInformation.vertical * searchInformation.additionDirection.x) + (-searchInformation.additionDirection.x * i),
                                               cornerPos.y + (searchInformation.horizontal * searchInformation.additionDirection.y) + (-searchInformation.additionDirection.y * i));
                    // Debug.Log("searchPos :" + searchPos);
                    if (evaluationTarget[searchPos.x, searchPos.y] == pieceColor)
                    {
                        searchLimit = searchLimit > searchPos.x ? searchPos.x : searchLimit; //searchLimit > searchPos.yならsearchLimit = searchPos.y;
                        for (int searchFocus = 0; searchFocus < searchLimit; ++searchFocus)
                        {
                            if (evaluationTarget[searchFocus, searchPos.y] != pieceColor) searchLimit = searchFocus - 1;
                            else if (verificationFlag[searchFocus, searchPos.y]) continue;
                            else
                            {
                                verificationFlag[searchFocus, searchPos.y] = true;
                                confirmedStone += 1 * colorSwitch;
                            }
                        }
                    }
                }
            }
        }
        // switch (pieceColor)
        // {
        //     case 0:
        //         Debug.Log("<color=green>" + confirmedStone + "</color>");
        //         break;
        //     case 1:
        //         Debug.Log("<color=white>" + confirmedStone + "</color>");
        //         break;
        //     case -1:
        //         Debug.Log("<color=brack>" + confirmedStone + "</color>");
        //         break;
        // }
        return confirmedStone;
    }

    /// <summary>
    /// 指定した手から相手が取れる手の数を探索する
    /// </summary>
    /// <param name="key">keyを一つ渡す</param>
    /// <returns>取れる手の数</returns>
    public int CandidateNumberSearch(string key)
    {
        int whichTurn = (2 * (profitPositionList[key].Turn % 2)) - 1;
        int count = 0;
        for (int i = 1; i <= profitPositionList.Count(); i++)
        {
            if (profitPositionList.ContainsKey(key + "," + i))
            {
                count++;
            }
        }
        return count * whichTurn;
    }

    public int CheckBoardPoint(string key, Vector2Int checkTarget)
    {
        int whichTurn = (2 * (profitPositionList[key].Turn % 2)) - 1;
        int[] BP = new int[64]{45,-11, 4,-1,-1, 4,-11, 45
                                ,-11,-16,-1,-3,-3, 2,-16,-11
                                ,  4, -1, 2,-1,-1, 2, -1,  4
                                , -1, -3,-1, 0, 0,-1, -3, -1
                                , -1, -3,-1, 0, 0,-1, -3, -1
                                ,  4, -1, 2,-1,-1, 2, -1,  4
                                ,-11,-16,-1,-3,-3,-1,-16,-11
                                , 45,-11, 4,-1,-1, 4,-11, 45};
        return BP[checkTarget.y + checkTarget.x * 8] * -whichTurn;
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
        foreach (Vector2Int d in storage.directVector)
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

    public string ListPrint(IReadOnlyList<int> ints)
    {
        string a = "";
        foreach (int keyCopy in ints)
        {
            a += " " + keyCopy;
        }
        return a;
    }

    public List<string> ReturnString()
    {
        List<string> result = new List<string>(profitPositionList.Keys);
        return result;
    }

    public List<string> ReturnKeyElement(string checkSource, string separatorString)
    {
        List<string> Items = checkSource.Split(separatorString).ToList();
        Items.RemoveAt(0);
        return Items;
    }

    /// <summary>
    /// Listから切り出す
    /// </summary>
    /// <param name="list">切り出し元</param>
    /// <param name="startIndex">切り出しを始める最初の要素数</param>
    /// <param name="Quantity">切り出す個数</param>
    /// <returns>切り出した結果</returns>
    public List<string> ListSlice(List<string> list, int startIndex, int Quantity, [CallerMemberName] string caller = "")
    {
        List<string> result = new List<string>();
        if (startIndex < 0 || startIndex >= list.Count)
        {
            Debug.LogError($"startIndex is out of range.{startIndex}, method: {caller}");
            return null; // 戻り値をnullにしてエラーを示す;
        }
        for (int i = startIndex; result.Count < Quantity; i++)
        {
            result.Add(list[i]);
        }
        return result;
    }

    public Dictionary<string, MaxProfitPosition> GetProfitPositionList()
    {
        return profitPositionList;
    }

    public int CountThisTurn(int turn)
    {
        if (turn / 2f < 1) return 1;
        return (int)Mathf.Ceil(turn / 2f);
    }

    /// <summary>
    /// CPUのターンが何回あったかを計算する
    /// </summary>
    /// <param name="allowZero"></param>
    /// <param name="turn"></param>
    /// <returns></returns>
    public int CountMyTurn(int turn)
    {
        return (int)Mathf.Ceil((float)turn / 2);
    }

    private void RemoveUnnecessary(string requiredElements)
    {
        Dictionary<string, MaxProfitPosition> profitPositionListCopy = new Dictionary<string, MaxProfitPosition>(profitPositionList);
        foreach (string key in profitPositionListCopy.Keys)
        {
            if (key == requiredElements)
            {
                Debug.Log("対象を発見しました");
            }
            else if (key.Length < requiredElements.Length)
            {
                if ("," + string.Join(",", ListSlice(ReturnKeyElement(requiredElements, ","), 0, ReturnKeyElement(key, ",").Count())) != key)
                {
                    profitPositionList.Remove(key);
                }
            }
            else if ("," + string.Join(",", ListSlice(ReturnKeyElement(key, ","), 0, ReturnKeyElement(requiredElements, ",").Count())) != requiredElements)
            {
                profitPositionList.Remove(key);
            }
        }
        if (!profitPositionList.ContainsKey(requiredElements))
        {
            Debug.LogError("<color=red>" + "必要な要素が削除されました" + "key: " + requiredElements + "</color>");
        }
    }

    public void RemovePlayerUnnecessary(int turn, Vector2Int selectedPos)
    {
        Dictionary<string, MaxProfitPosition> profitPositionListCopy = new Dictionary<string, MaxProfitPosition>(profitPositionList);
        string removeTarget = "";
        bool changeCheck = false;
        foreach (string key in profitPositionListCopy.Keys)
        {
            if (key.Substring(0, removeTarget.Length) != removeTarget && removeTarget != "")
            {
                profitPositionList.Remove(key);
                changeCheck = true;
            }
            else if (profitPositionListCopy[key].SelectedPosition != selectedPos && ReturnKeyElement(key, ",").Count() == turn + 1)
            {
                profitPositionList.Remove(key);
                changeCheck = true;
            }
            else if (profitPositionListCopy[key].SelectedPosition == selectedPos && ReturnKeyElement(key, ",").Count() == turn + 1)
            {
                removeTarget = key;
            }
        }
        if (!changeCheck)
        {
            if (turn == 0)
            {
                profitPositionList.Add(",1", new MaxProfitPosition() { SelectedPosition = selectedPos, Turn = turn });
            }
            Debug.Log("<color=red>" + "削除対象が見つかりませんでした" + "</color>");
        }
    }
}
