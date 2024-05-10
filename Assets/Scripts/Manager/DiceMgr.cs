using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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


    public Button[] dices = new Button[constant.diceMax]; // �ֻ�����
    private int[] dicesValues = new int[constant.diceNumberMax]; // �ֻ��� �� �����

    private int[] numbersCount = new int[constant.diceNumberMax]; // ��ũ üũ list

    private List<int> diceNumbers = new List<int>(); // �ֻ��� �� 1~6 ����Ʈ (��������)

    private List<int> selectedDice = new List<int>();

    private RanksFlag checkedRanksList; // ��ũ üũ �� 9�� ���� Ȱ��ȭ ���� ����

    private void Start()
    {
        for (int i = 1; i < constant.diceNumberMax + 1; i++)
        {
            diceNumbers.Add(i);  // 1���� 6���� �߰�
        }
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
    }

    public void SelectDiceRoll(int index)
    {
        dicesValues[index] = diceNumbers[Random.Range(0, diceNumbers.Count)];

        dices[index].GetComponentInChildren<TextMeshProUGUI>().text = dicesValues[index].ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameMgr.Instance.currentDiceCount = GameMgr.DiceCount.five;
            for (int i = 0; i < 5; i++)
            {
                selectedDice.Add(i);
            }
            DiceRoll();
            Debug.Log(checkedRanksList);
        } // �׽�Ʈ �ڵ�
    }
}
