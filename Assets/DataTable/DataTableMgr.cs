using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataTableMgr
{
    private static Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableMgr()
    {

        TextTable textTable = new TextTable();
        textTable.Load(DataTableIds.Text);
        tables.Add(DataTableIds.Text, textTable);

        SpellTable spellTable = new SpellTable();
        spellTable.Load(DataTableIds.SpellBook);
        tables.Add(DataTableIds.SpellBook, spellTable);

        MonsterTable monsterTable = new MonsterTable();
        monsterTable.Load(DataTableIds.Monster);
        tables.Add(DataTableIds.Monster, monsterTable);

        DebuffTable debuffTable = new DebuffTable();
        debuffTable.Load(DataTableIds.Debuff);
        tables.Add(DataTableIds.Debuff, debuffTable);

        PassiveTable passiveTable = new PassiveTable();
        passiveTable.Load(DataTableIds.Passive);
        tables.Add(DataTableIds.Passive, passiveTable);
    }


    public static T Get<T>(string id) where T : DataTable
    {
        if (!tables.ContainsKey(id))
            return null;
        return tables[id] as T;
    }
}
