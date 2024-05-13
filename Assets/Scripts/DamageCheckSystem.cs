using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class DamageCheckSystem
{

    public static List<int> rankids = new List<int>();

    static DamageCheckSystem()
    {
        rankids.Add((int)RanksIds.OnePair);
        rankids.Add((int)RanksIds.Triple);
        rankids.Add((int)RanksIds.Straight3);
        rankids.Add((int)RanksIds.TwoPair);
        rankids.Add((int)RanksIds.KindOf4);
        rankids.Add((int)RanksIds.Straight4);
        rankids.Add((int)RanksIds.FullHouse);
        rankids.Add((int)RanksIds.Straight5);
        rankids.Add((int)RanksIds.KindOf5);
    }

    public static int DamageCheck(int value, int[] ranks, RanksFlag checkedlist)
    {
        int sum = 0;
        int multiple = 0;
        int target = 0;
        for (int i = 0; i < ranks.Length; i++)
        {
            RanksFlag currentFlag = (RanksFlag)(1 << i);
            if ((checkedlist & currentFlag) != 0)
            {
                var spelldata = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(rankids[i] + ranks[i] - 1);
                sum += spelldata.SUM_OPERATION;
                multiple += spelldata.MULTIPLICATION_OPERATION;
                target = Math.Max(target, spelldata.TARGET);
            }
        }

        GameMgr.Instance.currentTarget = target;
        return (value + sum) * (100 + multiple) / 100;

        //( 주사위 눈금 총합 + 마법서 합연산 추가 값 ) x (100 + 마법서 곱연산 추가 값) / 100
    }

    //결과값에 상시 마법서등의 수치 추가 가능

    // 추후 실드 회복 상태이상 등의 데미지 계산 필요
}
