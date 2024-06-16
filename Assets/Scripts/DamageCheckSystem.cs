using System;
using System.Collections.Generic;

public static class DamageCheckSystem
{

    public static int DamageCheck(int value, int[] ranks, RanksFlag checkedlist, Mediator mediator)
    {
        int totalvalue = value;
        int sum = 0;
        int multiple = 0;
        int barrier = 0;
        int recovery = 0;
        int target = 1;
        int result = 0;

        if (mediator.artifacts.playersArtifactsLevel[6] == 1) //7번 유물
        {
            totalvalue += mediator.diceMgr.numbersCount[0] * mediator.artifacts.valueData.Value6;
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
                var spelldata = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(RankIdsToInt.rankids[i] + ranks[i] - 1);
                sum += spelldata.SUM_OPERATION;
                multiple += spelldata.MULTIPLICATION_OPERATION;
                barrier += spelldata.BARRIER;
                recovery += spelldata.RECOVERY;
                target = Math.Max(target, spelldata.TARGET);
            }
        }

        if (mediator.artifacts.playersArtifactsLevel[4] == 1 && mediator.gameMgr.currentDiceCount == GameMgr.DiceCount.three)
        {
            multiple += mediator.artifacts.valueData.Value4; // 유물 5번
        }

        result = (totalvalue + sum + mediator.gameMgr.curruntBonusStat) * (100 + multiple) / 100;
        mediator.ui.damageInfoPanel.DamageInfoUpdate(totalvalue, mediator.gameMgr.curruntBonusStat, sum, multiple, result);
        mediator.gameMgr.StatusEffectsUpdate(target, barrier, recovery);

        return result;

        //( 주사위 눈금 총합 + 마법서 합연산 추가 값 ) x (100 + 마법서 곱연산 추가 값) / 100
    }

    //결과값에 상시 마법서등의 수치 추가 가능

    // 추후 실드 회복 상태이상 등의 데미지 계산 필요
}
