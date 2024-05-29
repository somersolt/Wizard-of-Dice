using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SavePlayData
{
    public int instanceId;
    public int Stage;
    public int DiceCount;
    public int Damage;
    public int Hp;
    public int MaxHp;
    public int[] RankList = new int[9];
    public List<int> RankRewardList;
    public List<int> ArtifactList;
    public List<int> ArtifactLevelList;

}


public abstract class SaveData
{
    public int Version { get; protected set; }

    public abstract SaveData VersionUp();

}

public class SaveDataV1 : SaveData
{

    public SavePlayData savePlay;

    public SaveDataV1()
    {
        Version = 1;
    }

    // 현재 버전

    public override SaveData VersionUp()
    {
        return null;
    }
}

public class SaveDataV2 : SaveData
{
    public  SaveDataV2()
    {
        Version = 2;
    }

    // 패치 시 업데이트 버전

    public override SaveData VersionUp()
    {
        return null;
    }
}

/*
public class SaveDataV3 : SaveData
{
*/
