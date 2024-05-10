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
        
    }    // 싱글톤 패턴

    public enum DiceCount
    {
        three = 3, 
        four = 4, 
        five = 5,
    }

    public DiceCount currentDiceCount;
    int[] RankList = new int[(int)Ranks.count]; // 플레이어의 족보 리스트

    private void Awake()
    {
        for (int i = 0; i < (int)Ranks.TwoPair; i++)
        {
            RankList[i] = 1; // 원페어, 트리플, 3스트레이트 1레벨 등록
        }
        currentDiceCount = DiceCount.three;
    }

    public int[] GetRankList()
    {
        int[] copy = new int[RankList.Length];
        Array.Copy(RankList, copy, RankList.Length);
        return copy; // 배열 확인용 복사본 반환
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
           //족보 미획득
        }
        RankList[i] += 1;
    }

    public void GetDice4Ranks ()
    {
        for (int i = (int)Ranks.TwoPair; i < (int)Ranks.FullHouse; i++)
        {
            RankList[i] = 1; // 투페어, 카인드4, 4스트레이트 1레벨 등록
        }
        currentDiceCount = DiceCount.four;
    }
    public void GetDice5Ranks()
    {
        for (int i = (int)Ranks.FullHouse; i < (int)Ranks.count; i++)
        {
            RankList[i] = 1; // 풀하우스 5스트레이트, 카인드5 1레벨 등록
        }
        currentDiceCount = DiceCount.five;
    }

}
