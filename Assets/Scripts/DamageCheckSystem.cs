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
        int sum = GameMgr.Instance.curruntBonusStat;
        int multiple = 0;
        int barrier = 0;
        int recovery = 0;
        int target = 1;

        if (GameMgr.Instance.artifact.playersArtifacts[6] == 1) //7�� ����
        {
            sum += DiceMgr.Instance.numbersCount[0] * GameMgr.Instance.artifact.Value6;
        }
        for (int i = 0; i < ranks.Length; i++)
        {
            if (ranks[i] == 0)
            {
                continue;
            }
            RanksFlag currentFlag = (RanksFlag)(1 << i);
            if ((checkedlist & currentFlag) != 0)
            {
                var spelldata = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(rankids[i] + ranks[i] - 1);
                sum += spelldata.SUM_OPERATION;
                multiple += spelldata.MULTIPLICATION_OPERATION;
                barrier += spelldata.BARRIER;
                recovery += spelldata.RECOVERY;
                target = Math.Max(target, spelldata.TARGET);
            }
        }

        if (GameMgr.Instance.artifact.playersArtifacts[4] == 1 && GameMgr.Instance.currentDiceCount == GameMgr.DiceCount.three)
        {
            multiple += GameMgr.Instance.artifact.Value4; // ���� 5��
        }


        GameMgr.Instance.currentTarget = target;
        GameMgr.Instance.currentBarrier = barrier;
        GameMgr.Instance.currentRecovery = recovery;
        return (value + sum) * (100 + multiple) / 100;

        //( �ֻ��� ���� ���� + ������ �տ��� �߰� �� ) x (100 + ������ ������ �߰� ��) / 100
    }

    //������� ��� ���������� ��ġ �߰� ����

    // ���� �ǵ� ȸ�� �����̻� ���� ������ ��� �ʿ�
}
