using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

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
    [SerializeField]
    private SpinControl[] SpinControlers = new SpinControl[5];

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
        DiceRoll();
    }

    public void DiceRoll()
    {
        for (int i = 0; i < numbersCount.Length; i++)
        {
            numbersCount[i] = 0;
        }

        for (int i = 0; i < selectedDice.Count; i++)
        {
            SelectDiceRoll(selectedDice[i]);
        }
        selectedDice.Clear();

        for (int i = 0; i < dicesValues.Length; i++)
        {
            numbersCount[dicesValues[i] - 1]++;
        }
        checkedRanksList = RankCheckSystem.RankCheck(numbersCount);
        Debug.Log(checkedRanksList);
    }

    public void SelectDiceRoll(int index)
    {
        dicesValues[index] = diceNumbers[UnityEngine.Random.Range(0, diceNumbers.Count)];
        SpinControlers[index].DiceSpin(30, RotatePos.posList[dicesValues[index] - 1]);
        dices[index].GetComponentInChildren<TextMeshProUGUI>().text = dicesValues[index].ToString();
        dices[index].interactable = true;
    }

    public void ButtonSelect(int i)
    {
        selectedDice.Add(i);
        dices[i].interactable = false;
    }

}
