using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

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

    private void Awake()
    {
        for (int i = 0; i < (int)Ranks.TwoPair; i++)
        {
            RankList[i] = 1; // �����, Ʈ����, 3��Ʈ����Ʈ 1���� ���
        }
        currentDiceCount = DiceCount.three;
    }

    public int[] GetRankList()
    {
        int[] copy = new int[RankList.Length];
        Array.Copy(RankList, copy, RankList.Length);
        return copy; // �迭 Ȯ�ο� ���纻 ��ȯ
    }

    public void SetRankList(int i)
    {
        if( RankList[i] == 3) 
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

    public void GetDice4Ranks ()
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

}
