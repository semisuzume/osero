using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{
    enum DebugState
    {
        Init,
        DisplayUpdate
    }
    DebugState state = DebugState.Init;
    private CPU cpu;
    private GameObject DebugButton;
    public GameObject ViewContent;
    private int DisplayHierarchy = 1;
    // Start is called before the first frame update
    void Start()
    {
        DebugButton = (GameObject)Resources.Load("DebugButton");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator DebugFunction()
    {
        List<string> stringKeys = new List<string>();
        Dictionary<string, List<string>> DebugDic = new Dictionary<string, List<string>>();
        bool EndDebug = true;
        while (EndDebug)
        {
            yield return null;
            switch (state)
            {
                case DebugState.Init:
                    stringKeys = cpu.ReturnString();
                    break;
                case DebugState.DisplayUpdate:
                    UpdateDic(stringKeys);
                    UIUpdate(stringKeys);
                    break;

            }
        }
    }

    private void UpdateDic(List<string> keys)
    {
        List<string> expectation = new List<string>();
        foreach (string key in keys)
        {
            if (cpu.ReturnKeyElement(key, ",").Count() == DisplayHierarchy)
            {
                expectation.Add(key);
            }

        }
    }

    //Bottunを表示・編集
    private void UIUpdate(List<string> strings)
    {
        //一個目をまず表示する
        //分解して最初の一個を取り出す

        GameObject ButtonInstance = Instantiate(DebugButton);
        ButtonInstance.transform.SetParent(ViewContent.transform);
    }
}
