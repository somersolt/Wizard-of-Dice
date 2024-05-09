using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataTableMgr
{
    private static Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableMgr()
    {
        foreach (var id in DataTableIds.String)
        {
            DataTable table = new StringTable();
            table.Load(id);
            tables.Add(id, table);
        }

        ItemTable itemTable = new ItemTable();
        itemTable.Load(DataTableIds.Item);
        tables.Add(DataTableIds.Item, itemTable);


        //CharacterTable characterTable = new CharacterTable();
        //characterTable.Load(DataTableIds.character);
        //tables.Add(DataTableIds.character, characterTable);

        //ExpTable expTable = new ExpTable();
        //expTable.Load(DataTableIds.Exp);
        //tables.Add(DataTableIds.Exp, expTable);
    }

    public static StringTable GetStringTable()
    {
        return Get<StringTable>(DataTableIds.String[(int)Vars.currentLang]);
    }

    public static T Get<T>(string id) where T : DataTable
    {
        if (!tables.ContainsKey(id))
            return null;
        return tables[id] as T;
    }
}
