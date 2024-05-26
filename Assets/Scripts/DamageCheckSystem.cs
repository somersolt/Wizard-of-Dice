using System;
using System.Collections.Generic;

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
        int totalvalue = value;
        int sum = 0;
        int multiple = 0;
        int barrier = 0;
        int recovery = 0;
        int target = 1;

        if (GameMgr.Instance.artifact.playersArtifacts[6] == 1) //7번 유물
        {
            totalvalue += DiceMgr.Instance.numbersCount[0] * GameMgr.Instance.artifact.valueData.Value6;
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
            multiple += GameMgr.Instance.artifact.valueData.Value4; // 유물 5번
        }


        GameMgr.Instance.ui.damages[0].text = totalvalue.ToString();
        GameMgr.Instance.ui.damages[1].text = GameMgr.Instance.curruntBonusStat.ToString();
        GameMgr.Instance.ui.damages[2].text = sum.ToString();
        GameMgr.Instance.ui.damages[3].text = multiple.ToString() + '%';

        sum += GameMgr.Instance.curruntBonusStat;

        GameMgr.Instance.ui.damages[4].text = ((value + sum) * (100 + multiple) / 100).ToString();

        GameMgr.Instance.currentTarget = target;
        GameMgr.Instance.currentBarrier = barrier;
        GameMgr.Instance.currentRecovery = recovery;

        return (totalvalue + sum) * (100 + multiple) / 100;

        //( 주사위 눈금 총합 + 마법서 합연산 추가 값 ) x (100 + 마법서 곱연산 추가 값) / 100
    }

    //결과값에 상시 마법서등의 수치 추가 가능

    // 추후 실드 회복 상태이상 등의 데미지 계산 필요
}
