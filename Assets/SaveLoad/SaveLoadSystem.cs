using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class SaveLoadSystem 
{
    public enum Mode
    {
        Json,
        Binary,
        EncryptedBinary,
    }

    public static Mode FileMode { get; set; } = Mode.Json;

    public static int SaveDataVersion { get; private set; } = 1;

    // 0 (자동), 1, 2, 3 ...
    private static readonly string[] SaveFileName =
    {
        "SaveAuto.sav",
        "Save1.sav",
        "Save2.sav",
        "Save3.sav"
    };

    static SaveLoadSystem()
    {
        if (!Load())
        {
            CurrSaveData = new SaveDataV1();
            Save();
        }
    }

    public static SaveDataV1 CurrSaveData { get; set; }

    private static string SaveDirectory
    {
        get
        {
            return $"{Application.persistentDataPath}/Save";
        }
    }

    public static bool Save(int slot = 0)
    {
        if (CurrSaveData == null || slot < 0 ||  slot >= SaveFileName.Length)
        {
            return false;
        }

        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        var path = Path.Combine(SaveDirectory, SaveFileName[slot]);
        // FileMode 분기

        using (var writer = new JsonTextWriter(new StreamWriter(path)))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.TypeNameHandling = TypeNameHandling.All;
            //serializer.Converters.Add(new SavePlayDataConverter()); int로만 이뤄져서 컨버터 필요없을듯
            serializer.Serialize(writer, CurrSaveData);
        }

        return true;
    }

    public static bool Load(int slot = 0)
    {
        if (slot < 0 ||  slot >= SaveFileName.Length)
        {
            return false;
        }
        var path = Path.Combine(SaveDirectory, SaveFileName[slot]);
        if (!File.Exists(path))
        {
            return false;
        }

        SaveData data = null;
        using (var reader = new JsonTextReader(new StreamReader(path)))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.TypeNameHandling = TypeNameHandling.All;
            //serializer.Converters.Add(new SavePlayDataConverter()); 마찬가지
            data = serializer.Deserialize<SaveData>(reader);
        }

        //while (data.Version < SaveDataVersion)
        //{
        //    data = data.VersionUp(); 버전업 메소드
        //}

        CurrSaveData = data as SaveDataV1;

        return true;
    }

}
