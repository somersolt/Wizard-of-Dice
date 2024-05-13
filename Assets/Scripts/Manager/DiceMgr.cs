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

    }    // �̱��� ����


    public bool onDiceRoll;
    [SerializeField]
    private SpinControl[] SpinControlers = new SpinControl[5];
    private bool[] buttonToggle = new bool[5];

    public Button[] dices = new Button[constant.diceMax]; // �ֻ�����
    private int[] dicesValues = new int[constant.diceMax]; // �ֻ��� �� �����

    private int[] numbersCount = new int[constant.diceNumberMax]; // ��ũ üũ list

    private List<int> diceNumbers = new List<int>(); // �ֻ��� �� 1~6 ����Ʈ (��������)

    private List<int> selectedDice = new List<int>();

    private RanksFlag checkedRanksList; // ��ũ üũ �� 9�� ���� Ȱ��ȭ ���� ����

    [SerializeField]
    private Button reRoll; // �籼��
    [SerializeField]
    private Button confirm; // Ȯ��

    private int countToResult;
    private bool onResult;

    private void Start()
    {
        GameMgr.Instance.currentDiceCount = GameMgr.DiceCount.five;

        for (int i = 0; i < constant.diceNumberMax; i++)
        {
            diceNumbers.Add(i + 1);  // 1���� 6���� �߰�
        }

        for (int i = 0; i < constant.diceMax; i++)
        {
            int index = i;
            dices[index].onClick.AddListener(() => ButtonSelect(index));  // ��ư ����
        }
        reRoll.onClick.AddListener(() => DiceRoll());

        for (int i = 0; i < constant.diceMax; i++)
        {
            selectedDice.Add(i);     // �����Ҷ� �⺻ ���� ����
        }
        DiceRoll(true);
    }

    private void Update()
    {
        if (countToResult == selectedDice.Count && onResult)
        {
            checkedRanksList = RankCheckSystem.RankCheck(numbersCount);
            Debug.Log(checkedRanksList);

            selectedDice.Clear();
            onResult = false;

            // ��� ���. TO-DO ���� �Ŵ����� ��� ����
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
            selectedDiceCount = constant.diceMax;
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

        for (int i = 0; i < dicesValues.Length; i++)
        {
            numbersCount[dicesValues[i] - 1]++;
        }

    }

    private IEnumerator SelectDiceRoll(int index , bool starting, Action<int> callback)
    {
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

}
