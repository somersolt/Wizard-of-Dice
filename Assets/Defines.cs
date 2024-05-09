using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;

public static class DataTableIds
{
    public static readonly string[] String =
    {
        "StringTableKr",
        "StringTableEn",
        "StringTableJp"
    };

    public static readonly string Item = "ItemTable";

    public static readonly string character = "CharacterTable";

    public static readonly string Exp = "ExpTable";

    public static string CurrString
    {
        get
        {
            return String[(int)Vars.currentLang];
        }
    }
}

public static class Vars
{
    public static readonly string Version = "1.0.0";
    public static readonly int BuildVersion = 8;

    public static Languages currentLang = Languages.Korean;

    public static Languages editorLang = Languages.Korean;
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

public enum Languages
{
    Korean,
    English,
    Japanese,
}

public enum ItemType
{
    Weapon,
    Equip,
    Consumable
}