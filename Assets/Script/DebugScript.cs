using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{
    private CPU cpu;
    private GameObject debugScreen;
    private GameObject DebugButton;
    private GameObject InfoMember;
    private GameObject ViewInfoContent;
    private List<GameObject> DebugButtonList = new List<GameObject>();
    [SerializeField] private GameObject ViewListContent;
    [SerializeField] private GameObject canvas;
    [SerializeField] private int constant;

    // Start is called before the first frame update
    void Start()
    {
        cpu = GetComponent<CPU>();
        debugScreen = (GameObject)Resources.Load("DebugInfo");
        DebugButton = (GameObject)Resources.Load("DebugButton");
        InfoMember = (GameObject)Resources.Load("InfoMember");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DebugFunction()
    {
        List<string> stringKeys = new List<string>();
        stringKeys = cpu.ReturnString();
        stringKeys.Sort();
        UIUpdate(stringKeys);
    }

    //Bottunを表示・編集
    private void UIUpdate(List<string> strings)
    {
        foreach (GameObject button in DebugButtonList)
        {
            Destroy(button);
        }
        for (int i = 0; i < strings.Count; i++)
        {
            GameObject buttonObj = Instantiate(DebugButton, ViewListContent.transform);
            Button buttonComponent = buttonObj.GetComponent<Button>();
            TextMeshProUGUI TMP = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            TMP.text = strings[i];
            SetupButton(buttonComponent, TMP, strings[i]);
            DebugButtonList.Add(buttonObj);
        }
    }

    private void SetupButton(Button button, TextMeshProUGUI tmp, string text)
    {
        button.onClick.AddListener(() => DisplayDebugInfo(text));
    }

    public void DisplayDebugInfo(string key)
    {
        MaxProfitPosition copy = new MaxProfitPosition();
        GameObject ScreenObj = Instantiate(debugScreen, canvas.transform);
        ViewInfoContent = ScreenObj.transform.GetChild(0).gameObject;
        Button button = ScreenObj.transform.GetComponentInChildren<Button>();
        button.onClick.AddListener(() => Destroy(ScreenObj));
        MaxProfitPosition source = cpu.ReturnProfitPositionList()[key];
        foreach (var prop in typeof(MaxProfitPosition).GetProperties())
        {
            if (prop.GetType() == typeof(bool[,])) continue;
            if (prop.Name == "SelectedPosition")
            {
                copy.SelectedPosition = (Vector2Int)prop.GetValue(source);
            }
            if (prop.Name == "turn")
            {
                copy.turn = (int)prop.GetValue(source);
            }
            GameObject textObj = Instantiate(InfoMember, ViewInfoContent.transform);
            TextMeshProUGUI TMP = textObj.GetComponent<TextMeshProUGUI>();
            TMP.text = prop.Name + " : " + prop.GetValue(source).ToString();
        }
        GameObject keyTextObj = Instantiate(InfoMember, ViewInfoContent.transform);
        TextMeshProUGUI keyTMP = keyTextObj.GetComponent<TextMeshProUGUI>();
        keyTMP.text = "key : " + key;
    }
}
