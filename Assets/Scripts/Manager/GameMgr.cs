using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    private static GameMgr instance;

    public static GameMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameMgr>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameMgr");
                    instance = obj.AddComponent<GameMgr>();
                }
            }
            return instance;
        }

    }    // �̱��� ����

    public enum DiceCount
    {
        three = 3,
        four = 4,
        five = 5,
    }

    public DiceCount currentDiceCount;
    int[] RankList = new int[(int)Ranks.count]; // �÷��̾��� ���� ����Ʈ
    private RanksFlag currentRanks; //���� �ֻ����� ���� ����
    private int currentValue;
    [SerializeField]
    private TextMeshProUGUI damageInfo;
    private int currentDamage;
    [SerializeField]
    private Button[] scrolls = new Button[3];
    [SerializeField]
    private Button totalScrolls;

    private void Awake()
    {
        for (int i = 0; i < (int)Ranks.TwoPair; i++)
        {
            RankList[i] = 1; // �����, Ʈ����, 3��Ʈ����Ʈ 1���� ���
        }
        currentDiceCount = DiceCount.three;

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentDiceCount = DiceCount.three;
            DiceMgr.Instance.DiceThree();
            DiceMgr.Instance.DiceRoll(true);
            Debug.Log("three");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GetDice4Ranks();
            DiceMgr.Instance.DiceFour();
            DiceMgr.Instance.DiceRoll(true);

            Debug.Log("four");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GetDice4Ranks();
            GetDice5Ranks();
            DiceMgr.Instance.DiceFive();
            DiceMgr.Instance.DiceRoll(true);

            Debug.Log("five");
        }

        // �׽�Ʈ �ڵ�
    }


    public int[] GetRankList()
    {
        int[] copy = new int[RankList.Length];
        Array.Copy(RankList, copy, RankList.Length);
        return copy; // �迭 Ȯ�ο� ���纻 ��ȯ
    }

    public void SetRankList(int i)
    {
        if (RankList[i] == 3)
        {
            return;
            //max level
        }
        else if (RankList[i] == 0)
        {
            return;
            //���� ��ȹ��
        }
        RankList[i] += 1;
    }

    public void GetDice4Ranks()
    {
        for (int i = (int)Ranks.TwoPair; i < (int)Ranks.FullHouse; i++)
        {
            RankList[i] = 1; // �����, ī�ε�4, 4��Ʈ����Ʈ 1���� ���
        }
        currentDiceCount = DiceCount.four;
    }
    public void GetDice5Ranks()
    {
        for (int i = (int)Ranks.FullHouse; i < (int)Ranks.count; i++)
        {
            RankList[i] = 1; // Ǯ�Ͽ콺 5��Ʈ����Ʈ, ī�ε�5 1���� ���
        }
        currentDiceCount = DiceCount.five;
    }

    public void SetResult(RanksFlag ranks, int value)
    {
        currentRanks = ranks;
        currentValue = value;

        currentDamage = DamageCheckSystem.DamageCheck(currentValue, RankList, currentRanks);
        damageInfo.text = currentDamage.ToString();

        int r = 0;
        for (int i = 0; i < scrolls.Length; i++)
        {
            scrolls[i].GetComponentInChildren<TextMeshProUGUI>().text = "�ֹ��� ����";
            for (int j = 8 - r; j >= 0; j--)
            {
                RanksFlag currentFlag = (RanksFlag)(1 << j);

                r++;

                if ((currentRanks & currentFlag) != 0)
                {
                        scrolls[i].GetComponentInChildren<TextMeshProUGUI>().text =
                            DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[j]).GetName;
                        break;
                }
            }
        }
    }

}
