using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Reflection;

public class DiceMgr : MonoBehaviour
{
    private static DiceMgr instance;

    public static DiceMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DiceMgr>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("DiceMgr");
                    instance = obj.AddComponent<DiceMgr>();
                }
            }
            return instance;
        }

    }    // 싱글톤 패턴


    public bool onDiceRoll;
    [SerializeField]
    private SpinControl[] SpinControlers = new SpinControl[5];
    private bool[] buttonToggle = new bool[5];

    public Button[] dices = new Button[constant.diceMax]; // 주사위들
    private int[] dicesValues = new int[constant.diceMax]; // 주사위 눈 결과값

    private int[] numbersCount = new int[constant.diceNumberMax]; // 랭크 체크 list

    private List<int> diceNumbers = new List<int>(); // 주사위 눈 1~6 리스트 (변동가능)

    private List<int> selectedDice = new List<int>();

    private RanksFlag checkedRanksList; // 랭크 체크 후 9개 족보 활성화 여부 저장

    [SerializeField]
    private Button reRoll; // 재굴림
    [SerializeField]
    private Button confirm; // 확정

    private int countToResult;
    private bool onResult;

    private int totalValue;

    private void Start()
    {

        for (int i = 0; i < constant.diceNumberMax; i++)
        {
            diceNumbers.Add(i + 1);  // 1부터 6까지 추가
        }

        for (int i = 0; i < constant.diceMax; i++)
        {
            int index = i;
            dices[index].onClick.AddListener(() => ButtonSelect(index));  // 버튼 세팅
        }
        reRoll.onClick.AddListener(() => DiceRoll());
        confirm.onClick.AddListener(() => GameMgr.Instance.AttackButtonClicked());
    }

    private void Update()
    {
        if (countToResult == selectedDice.Count && onResult)
        {
            checkedRanksList = RankCheckSystem.RankCheck(numbersCount);

            selectedDice.Clear();
            onResult = false;

            totalValue = 0;
            for (int i = 0; i < (int)GameMgr.Instance.currentDiceCount; i++)
            {
                totalValue += dicesValues[i];
            }
            GameMgr.Instance.SetResult(checkedRanksList, totalValue);
        }

        if (onDiceRoll)
        {
            for (int i = 0; i < constant.diceMax; i++)
            { dices[i].interactable = false; }
            confirm.interactable = false;
            reRoll.interactable = false;
        }
        else
        {
            for (int i = 0; i < constant.diceMax; i++)
            { dices[i].interactable = true; }
            confirm.interactable = true;

            if (selectedDice.Count == 0)
            {
                reRoll.interactable = false;
            }
            else
            {
                reRoll.interactable = true;
            }
        }
    }

    public void DiceRoll(bool starting = false)
    {
        onDiceRoll = true;
        onResult = true;
        countToResult = 0;
        for (int i = 0; i < numbersCount.Length; i++)
        {
            numbersCount[i] = 0;
        }

        int selectedDiceCount = selectedDice.Count;
        if (starting)
        {
            selectedDiceCount = (int)GameMgr.Instance.currentDiceCount;
        }

        for (int i = 0; i < selectedDiceCount; i++)
        {
            Action<int> spinCallback = (diceIndex) =>
            {
                dices[diceIndex].GetComponentInChildren<TextMeshProUGUI>().text = dicesValues[diceIndex].ToString();
                dices[diceIndex].interactable = true;
                onDiceRoll = false;
                countToResult++;
            };

            StartCoroutine(SelectDiceRoll(selectedDice[i], starting, spinCallback));
            dices[selectedDice[i]].GetComponent<Image>().color = new Color(0x214 / 255f, 0x214 / 255f, 0x214 / 255f);
            buttonToggle[selectedDice[i]] = false;
        }

        for (int i = 0; i < (int)GameMgr.Instance.currentDiceCount; i++)
        {
            numbersCount[dicesValues[i] - 1]++;
        }

    }

    private IEnumerator SelectDiceRoll(int index , bool starting, Action<int> callback)
    {
        if(GameMgr.Instance.currentDiceCount == GameMgr.DiceCount.three && (index == 3 || index == 4))
        {
            dicesValues[index] = 0;
            yield break;
        }
        if (GameMgr.Instance.currentDiceCount == GameMgr.DiceCount.four && index == 4)
        {
            dicesValues[index] = 0;
            yield break;
        }
        dices[index].GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
        dices[index].interactable = false;
        dicesValues[index] = diceNumbers[UnityEngine.Random.Range(0, diceNumbers.Count)];
        if (starting)
        {
            switch (GameMgr.Instance.currentDiceCount)
            {
                case GameMgr.DiceCount.three:
                    SpinControlers[index].DiceSpin(30 + index * 16, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
                    break;
                case GameMgr.DiceCount.four:
                    SpinControlers[index].DiceSpin(30 + index * 12, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
                    break;
                case GameMgr.DiceCount.five:
                    SpinControlers[index].DiceSpin(30 + index * 9, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
                    break;
            }
        }
        else
        {
            SpinControlers[index].DiceSpin(30, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
        }
        yield return null;
    }

    public void ButtonSelect(int i)
    {
        if (!onDiceRoll)
        {
            if (!buttonToggle[i])
            {
                selectedDice.Add(i);
                dices[i].GetComponent<Image>().color = new Color(0x57 / 255f, 0x57 / 255f, 0x57 / 255f);
                buttonToggle[i] = true;
            }
            else if (buttonToggle[i])
            {
                selectedDice.Remove(i);
                dices[i].GetComponent<Image>().color = new Color(0x214 / 255f, 0x214 / 255f, 0x214 / 255f);
                buttonToggle[i] = false;
            }
        }
    }

    public void DiceThree()
    {
        dices[3].gameObject.SetActive(false);
        dices[4].gameObject.SetActive(false);
        selectedDice.Clear();
        for (int i = 0; i < 3; i++)
        {
            selectedDice.Add(i);     // 시작할때 기본 숫자 세팅
        }
    }
    public void DiceFour()
    {
        dices[3].gameObject.SetActive(true);
        dices[4].gameObject.SetActive(false);
        selectedDice.Clear();
        for (int i = 0; i < 4; i++)
        {
            selectedDice.Add(i);     // 시작할때 기본 숫자 세팅
        }
    }
    public void DiceFive()
    {
        dices[3].gameObject.SetActive(true);
        dices[4].gameObject.SetActive(true);
        selectedDice.Clear();
        for (int i = 0; i < 5; i++)
        {
            selectedDice.Add(i);     // 시작할때 기본 숫자 세팅
        }
    }
}
