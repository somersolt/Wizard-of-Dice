using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class TextTable : DataTable
{
    private class TextData
    {
        public int ID { get; set; }
        public string TEXT { get; set; }
    }

    private Dictionary<int, string> table = new Dictionary<int, string>();

    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);

        table.Clear();

        var textAsset = Resources.Load<TextAsset>(path);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<TextData>();
            foreach (var record in records)
            {
                table.Add(record.ID, record.TEXT);
            }
        }
    }

    public string Get(int id)
    {
        if (!table.ContainsKey(id))
            return "Null ID";
        return table[id];
    }

}
