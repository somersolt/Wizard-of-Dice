using CsvHelper;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemData
{
    public static readonly string FormatIconPath = "Icon/Item/{0}";

    public string Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public int Value { get; set; }
    public int Cost { get; set; }
    public string Icon { get; set; }

    public string GetName
    {
        get
        {
            return DataTableMgr.GetStringTable().Get(Name);
        }
    }

    public string GetDesc
    {
        get
        {
            return DataTableMgr.GetStringTable().Get(Desc);
        }
    }
    public Sprite GetSprite
    {
        get
        {
            return Resources.Load<Sprite>(string.Format(FormatIconPath, Icon));
        }
    }

    public override string ToString()
    {
        return $"{Id}: {Type} / {GetName} / {GetDesc} / {Value} / {Cost} / {GetSprite}";
    }
}

public class ItemTable : DataTable
{
    private Dictionary<string, ItemData> table = new Dictionary<string, ItemData>();

    public List<string> AllItemIds
    {
        get
        {
            return table.Keys.ToList();
        }
    }

    public ItemData Get(string id)
    {
        if (!table.ContainsKey(id))
            return null;

        return table[id];
    }

    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);

        var textAsset = Resources.Load<TextAsset>(path);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<ItemData>();
            foreach (var record in records)
            {
                table.Add(record.Id, record);
                Debug.Log(record);
            }
        }
    }
}
