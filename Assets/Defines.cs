using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;

public static class DataTableIds
{
    public static readonly string Text = "TextTable";

    public static readonly string SpellBook = "SpellTable";

    public static readonly string Debuff = "DebuffTable";

    public static readonly string Monster = "MonsterTable";

}

public static class Vars
{
    public static readonly string Version = "1.0.0";
    public static readonly int BuildVersion = 1;

}



public static class Tags
{
    public static readonly string Player = "Player";
    public static readonly string GameController = "GameController";
}

public static class SortingLayers
{
    public static readonly string Default = "Default";
}

public static class Layers
{
    public static readonly string UI = "UI";
}


public enum Ranks
{
    OnePair = 1111,
    Triple = 1121,
    Straight3 = 1131,
    TwoPair = 1141,
    KindOf4 = 1151,
    Straight4 = 1161,
    FullHouse = 1171,
    Straight5 = 1181,
    KindOf5 = 1191,
}