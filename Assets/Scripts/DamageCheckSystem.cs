using System;
using System.Collections.Generic;

public static class DamageCheckSystem
{

    public static int DamageCheck(int value, int[] ranks, RanksFlag checkedlist, GameMgr gameMgr, DiceMgr diceMgr, UI ui)
    {
        int totalvalue = value;
        int sum = 0;
        int multiple = 0;
        int barrier = 0;
        int recovery = 0;
        int target = 1;
        int result = 0;

        if (gameMgr.artifact.playersArtifacts[6] == 1) //7�� ����
        {
            totalvalue += diceMgr.numbersCount[0] * gameMgr.artifact.valueData.Value6;
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

        if (gameMgr.artifact.playersArtifacts[4] == 1 && gameMgr.currentDiceCount == GameMgr.DiceCount.three)
        {
            multiple += gameMgr.artifact.valueData.Value4; // ���� 5��
        }

        result = (totalvalue + sum + gameMgr.curruntBonusStat) * (100 + multiple) / 100;
        ui.damageInfoPanel.DamageInfoUpdate(totalvalue, gameMgr.curruntBonusStat, sum, multiple, result);
        gameMgr.StatusEffectsUpdate(target, barrier, recovery);

        return result;

        //( �ֻ��� ���� ���� + ������ �տ��� �߰� �� ) x (100 + ������ ������ �߰� ��) / 100
    }

    //������� ��� ���������� ��ġ �߰� ����

    // ���� �ǵ� ȸ�� �����̻� ���� ������ ��� �ʿ�
}
