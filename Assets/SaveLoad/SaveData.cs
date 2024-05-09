using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveData
{
    public int Version { get; protected set; }

    public abstract SaveData VersionUp();

}

public class SaveDataV1 : SaveData
{
    public int itemInvenSorting = 0;
    public int itemFilteringSorting = 0;

    public List<SaveItemData> saveItems = new List<SaveItemData>();
    //public List<SaveCharacterData> saveCharacters = new List<SaveCharacterData>();

    public SaveDataV1()
    {
        Version = 1;
    }

    public override SaveData VersionUp()
    {
        return null;
    }
}

public class SaveDataV2 : SaveData
{
    public int Gold { get; set; } = 100;
    public string Name { get; set; } = "Empty";

    public  SaveDataV2()
    {
        Version = 2;
    }

    public override SaveData VersionUp()
    {
        return null;
    }
}





/*
public class SaveDataV3 : SaveData
{
    public int Gold { get; set; } = 100;
    //public string Name { get; set; } = "Empty";

    public Vector3 Position { get; set; } = Vector3.zero;
    public Quaternion Rotation { get; set; }= Quaternion.identity;
    public Vector3 Scale { get; set; } = Vector3.one;

    public Color color { get; set; } = Color.white;
}
*/
