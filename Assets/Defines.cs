using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTableIds
{
    public static readonly string Text = "TextTable";

    public static readonly string SpellBook = "MagicBookTable";

    public static readonly string Debuff = "StatusEffectTable";

    public static readonly string Monster = "MonsterTable";

    public static readonly string Passive = "PassiveTable";

}

public static class Vars
{
    public static readonly string Version = "1.0.0";
    public static readonly int BuildVersion = 1;

}
public static class UsedColor
{
    public static readonly Color usedColor = new Color(214/255f, 214/255f, 214/255f , 1);
    public static readonly Color whiteColor = new Color(255/255f, 255/255f, 255/255f , 1);
}

public static class constant
{
    public static readonly int diceNumberMax = 6;
    public static readonly int diceMax = 5;
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
    OnePair = 0,
    Triple = 1,
    Straight3 = 2,
    TwoPair = 3,
    KindOf4 = 4,
    Straight4 = 5,
    FullHouse = 6,
    Straight5 = 7,
    KindOf5 = 8,
    count = 9,
}

public enum RanksIds
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

[Flags]
public enum RanksFlag : short
{
    OnePair = 1 << 0,
    Triple = 1 << 1,
    Straight3 = 1 << 2,
    TwoPair = 1 << 3,
    KindOf4 = 1 << 4,
    Straight4 = 1 << 5,
    FullHouse = 1 << 6,
    Straight5 = 1 << 7,
    KindOf5 = 1 << 8,
}

public enum GameMode
{
    Tutorial2 = -1,
    Tutorial = 0,
    Default = 1,
}