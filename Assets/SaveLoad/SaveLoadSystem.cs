using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;


public class SaveLoadSystem
{

    private static readonly byte[] Key = Convert.FromBase64String("ZgjtMN69XqjbfyS22uDnrlohWL9O6rwQSM/ykpSsVko="); // 32 bytes for AES-256
    private static readonly byte[] IV = Convert.FromBase64String("7Tyf5sE7xYNCPBRY3A82Cg==");   // 16 bytes for AES

    public enum Mode
    {
        Json,
        Binary,
        EncryptedBinary,
    }

    public static Mode FileMode { get; set; } = Mode.Json;

    public static int SaveDataVersion { get; private set; } = 1;

    // 0 (자동), 1, 2, 3 ...
    private static readonly string SaveFileName = "SaveAuto.sav";


    static SaveLoadSystem()
    {
        if (!Load())
        {
            CurrSaveData = new SaveDataV1();
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

    public static bool Save(Mediator mediator)
    {

        PlayerPrefs.SetInt("Save", 1);

        CurrSaveData.savePlay = new SavePlayData();
        CurrSaveData.savePlay.Stage = mediator.stageMgr.currentField * 10 + mediator.stageMgr.currentStage;
        CurrSaveData.savePlay.DiceCount = (int)mediator.gameMgr.currentDiceCount;
        CurrSaveData.savePlay.Damage = mediator.gameMgr.curruntBonusStat;
        CurrSaveData.savePlay.Hp = mediator.gameMgr.GetHp();
        CurrSaveData.savePlay.MaxHp = mediator.gameMgr.GetMaxHp();
        CurrSaveData.savePlay.RankList = mediator.gameMgr.GetRankList();
        CurrSaveData.savePlay.RankRewardList = new List<int>();
        CurrSaveData.savePlay.RankRewardList.Clear();
        for (int i = 0; i < 9; i++)
        {
            if (i >= mediator.ui.rewardPanel.rewardList.Count)
            {
                CurrSaveData.savePlay.RankRewardList.Add(0);
            }
            else
            {
                CurrSaveData.savePlay.RankRewardList.Add(mediator.ui.rewardPanel.rewardList[i].ID);
            }
        }
        CurrSaveData.savePlay.ArtifactList = new List<int>();
        CurrSaveData.savePlay.ArtifactList.Clear();
        for (int i = 0; i < 3; i++)
        {
            CurrSaveData.savePlay.ArtifactList.Add(mediator.artifacts.playersArtifactsNumber[i]);
        }

        CurrSaveData.savePlay.ArtifactLevelList = new List<int>();
        CurrSaveData.savePlay.ArtifactLevelList.Clear();
        for (int i = 0; i < 10; i++)
        {
            CurrSaveData.savePlay.ArtifactLevelList.Add(mediator.artifacts.playersArtifactsLevel[i]);
        }


        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        var path = Path.Combine(SaveDirectory, SaveFileName);
        // FileMode 분기

        //using (var writer = new JsonTextWriter(new StreamWriter(path)))
        //{
        //    var serializer = new JsonSerializer();
        //    serializer.Formatting = Formatting.Indented;
        //    serializer.TypeNameHandling = TypeNameHandling.All;
        //    serializer.Serialize(writer, CurrSaveData);
        //}

        string jsonData = JsonConvert.SerializeObject(CurrSaveData, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        });
        byte[] encryptedData = EncryptStringToBytes_Aes(jsonData, Key, IV);
        File.WriteAllBytes(path, encryptedData);

        return true;
    }

    private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException(nameof(plainText));
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException(nameof(Key));
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException(nameof(IV));

        byte[] encrypted;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return encrypted;
    }

    private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException(nameof(cipherText));
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException(nameof(Key));
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException(nameof(IV));

        string plaintext;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }

    public static object LoadData()
    {
        var path = Path.Combine(SaveDirectory, SaveFileName);
        if (!File.Exists(path))
        {
            return null;
        }

        // 파일에서 암호화된 데이터를 읽음
        byte[] encryptedData = File.ReadAllBytes(path);

        // 데이터를 복호화
        string jsonData = DecryptStringFromBytes_Aes(encryptedData, Key, IV);

        // JSON 데이터를 객체로 역직렬화하여 반환
        return JsonConvert.DeserializeObject(jsonData, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
    }


    public static bool Load()
    {
        var path = Path.Combine(SaveDirectory, SaveFileName);
        if (!File.Exists(path) || PlayerPrefs.GetInt("Save", 0) == 0)
        {
            return false;
        }

        CurrSaveData = (SaveDataV1)LoadData();

        //SaveData data = null;
        //using (var reader = new JsonTextReader(new StreamReader(path)))
        //{
        //    var serializer = new JsonSerializer();
        //    serializer.Formatting = Formatting.Indented;
        //    serializer.TypeNameHandling = TypeNameHandling.All;
        //    data = serializer.Deserialize<SaveData>(reader);
        //}

        //while (data.Version < SaveDataVersion)
        //{
        //    data = data.VersionUp();
        //}

        //CurrSaveData = data as SaveDataV1;

        return true;
    }
    public static void DeleteSaveData()
    {
        PlayerPrefs.SetInt("Save", 0);
        File.Delete(Path.Combine(SaveDirectory, SaveFileName));
    }
}









    //저장할거
    // 볼륨 - 따로함
    // 저장 했는지 여부
    // 현재 필드, 스테이지
    // 보유 주사위
    // 보유 공격력
    // 현재 체력
    // 최대 체력 (유물때문에)
    // 보유 족보
    // 보유 유물

//    public static void Save()
//    {
//        PlayerPrefs.SetInt("Save", 1);


//        int stage = StageMgr.Instance.currentField * 10 + StageMgr.Instance.currentStage;
//        PlayerPrefs.SetInt("Stage", stage);
//        PlayerPrefs.SetInt("Dice", (int)GameMgr.Instance.currentDiceCount);
//        PlayerPrefs.SetInt("Damage", GameMgr.Instance.curruntBonusStat);
//        PlayerPrefs.SetInt("Hp", GameMgr.Instance.GetHp());
//        PlayerPrefs.SetInt("MaxHp", GameMgr.Instance.GetMaxHp());

//        for (int i = 0; i < 9; i++)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append("Rank");
//            sb.Append((i + 1).ToString());
//            PlayerPrefs.SetInt(sb.ToString(), GameMgr.Instance.GetRank(i));
//        }

//        for (int i = 0; i < 9; i++)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append("RankReward");
//            sb.Append((i + 1).ToString());
//            if (i >= GameMgr.Instance.ui.rewardList.Count)
//            {
//                PlayerPrefs.SetInt(sb.ToString(), 0);
//            }
//            else
//            {
//                PlayerPrefs.SetInt(sb.ToString(), GameMgr.Instance.ui.rewardList[i].ID);
//            }
//        }

//        for (int i = 0; i < 3; i++)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append("Artifact");
//            sb.Append((i + 1).ToString());
//            PlayerPrefs.SetInt(sb.ToString(), GameMgr.Instance.artifact.playersArtifactsNumber[i]);
//        }

//        for (int i = 0; i < 10; i++)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append("ArtifactLevel");
//            sb.Append((i + 1).ToString());
//            PlayerPrefs.SetInt(sb.ToString(), GameMgr.Instance.artifact.playersArtifacts[i]);
//        }

//        PlayerPrefs.Save();
//    }

//    public static int Load()
//    {
//        if (PlayerPrefs.GetInt("Save", 0) == 1)
//        {
//            StageMgr.Instance.currentField = PlayerPrefs.GetInt("Stage", 0) / 10;
//            StageMgr.Instance.currentStage = PlayerPrefs.GetInt("Stage", 0) % 10;
//            GameMgr.Instance.currentDiceCount = (GameMgr.DiceCount)PlayerPrefs.GetInt("Dice", 2);
//            GameMgr.Instance.curruntBonusStat = PlayerPrefs.GetInt("Damage", 0);
//            GameMgr.Instance.SetHp(PlayerPrefs.GetInt("Hp", 100));
//            GameMgr.Instance.SetMaxHp(PlayerPrefs.GetInt("MaxHp", 100));
//            for (int i = 0; i < 9; i++)
//            {
//                StringBuilder sb = new StringBuilder();
//                sb.Append("Rank");
//                sb.Append((i + 1).ToString());
//                GameMgr.Instance.SetRank(i, PlayerPrefs.GetInt(sb.ToString(), 0));
//            }

//            GameMgr.Instance.ui.rewardList.Clear();
//            for (int i = 0; i < 9; i++)
//            {
//                StringBuilder sb = new StringBuilder();
//                sb.Append("RankReward");
//                sb.Append((i + 1).ToString());
//                int rewardID = PlayerPrefs.GetInt(sb.ToString(), 0);
//                if (rewardID != 0)
//                {
//                    GameMgr.Instance.ui.rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(rewardID));
//                }
//            }

//            for (int i = 0; i < 3; i++)
//            {
//                StringBuilder sb = new StringBuilder();
//                sb.Append("Artifact");
//                sb.Append((i + 1).ToString());
//                GameMgr.Instance.artifact.playersArtifactsNumber[i] = PlayerPrefs.GetInt(sb.ToString(), -1);
//            }

//            for (int i = 0; i < 10; i++)
//            {
//                StringBuilder sb = new StringBuilder();
//                sb.Append("ArtifactLevel");
//                sb.Append((i + 1).ToString());
//                GameMgr.Instance.artifact.playersArtifacts[i] = PlayerPrefs.GetInt(sb.ToString(), 0);
//            }

//        }
//        return PlayerPrefs.GetInt("Save", 0);
//    }

//}
