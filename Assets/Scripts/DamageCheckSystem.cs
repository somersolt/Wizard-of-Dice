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

        //( �ֻ��� ���� ���� + ������ �տ��� �߰� �� ) x (100 + ������ ������ �߰� ��) / 100
    }

    //������� ��� ���������� ��ġ �߰� ����

    // ���� �ǵ� ȸ�� �����̻� ���� ������ ��� �ʿ�
}
