using CsvHelper;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PassiveData
{
    //public static readonly string FormatIconPath = "Icon/Item/{0}";
    //������ ������ ������ ������ �ּ� (Resources ���� �� ����)

    public int ID { get; set; }
    public int NAME_ID { get; set; }
    public int DESC_ID { get; set; }
    public int VALUE { get; set; }

    //public string Icon { get; set; } ������ path

    public string GetName
    {
        get
        {
            return DataTableMgr.Get<TextTable>(DataTableIds.Passive).Get(NAME_ID);
        }
    }

    public string GetDesc
    {
        get
        {
            return DataTableMgr.Get<TextTable>(DataTableIds.Passive).Get(DESC_ID);
        }
    }

    //public Sprite GetSprite
    //{
    //    get
    //    {
    //        return Resources.Load<Sprite>(string.Format(FormatIconPath, Icon));
    //    }
    //} icon �ּ� ��ȯ ������Ƽ

    public override string ToString()
    {
        return $"{ID}: {GetName} / {GetDesc} / {VALUE} ";
    }
}

public class PassiveTable : DataTable
{
    private Dictionary<int, PassiveData> table = new Dictionary<int, PassiveData>();

    public List<int> AllItemIds
    {
        get
        {
            return table.Keys.ToList();
        }
    }

    public PassiveData Get(int id)
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
            var records = csvReader.GetRecords<PassiveData>();
            foreach (var record in records)
            {
                table.Add(record.ID, record);
            }
        }
    }
}
