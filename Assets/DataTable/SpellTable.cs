using CsvHelper;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpellData
{
    //public static readonly string FormatIconPath = "Icon/Item/{0}";
    //������ ������ ������ ������ �ּ� (Resources ���� �� ����)

    public int ID { get; set; }
    public int NAME_ID { get; set; }
    public int DESC_ID { get; set; }
    public int LEVEL { get; set; }
    public int SUM_OPERATION { get; set; }
    public int MULTIPLICATION_OPERATION { get; set; }
    public int TARGET { get; set; }
    public int BARRIER { get; set; }
    public int RECOVERY { get; set; }
    public int STATUS_EFFECT { get; set; }
    //public string Icon { get; set; } ������ path

    public string GetName
    {
        get
        {
            return DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(NAME_ID);
        }
    }

    public string GetDesc
    {
        get
        {
            return DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(DESC_ID);
        }
    }

    public DebuffData GetDebuff
    {

        get
        {
            if (STATUS_EFFECT == 0)
            {
                return null;
            }
            else
            {
                return DataTableMgr.Get<DebuffTable>(DataTableIds.Debuff).Get(STATUS_EFFECT);
            }
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
        return $"{ID}: {GetName} / {GetDesc} / {LEVEL} / {SUM_OPERATION} / {MULTIPLICATION_OPERATION} / {TARGET} / {BARRIER} / {RECOVERY} / {STATUS_EFFECT}";
    }
}

public class SpellTable : DataTable
{
    private Dictionary<int, SpellData> table = new Dictionary<int, SpellData>();

    public List<int> AllItemIds
    {
        get
        {
            return table.Keys.ToList();
        }
    }

    public SpellData Get(int id)
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
            var records = csvReader.GetRecords<SpellData>();
            foreach (var record in records)
            {
                table.Add(record.ID, record);
            }
        }
    }
}
