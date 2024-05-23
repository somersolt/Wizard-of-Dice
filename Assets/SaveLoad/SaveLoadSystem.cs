using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEditor.SceneManagement;
using System.Text;

public static class SaveLoadSystem 
{
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

    public static void Save()
    {
        PlayerPrefs.SetInt("Save", 1);
        int stage = StageMgr.Instance.currentField * 10 + StageMgr.Instance.currentStage;
        PlayerPrefs.SetInt("Stage", stage);
        PlayerPrefs.SetInt("Dice", (int)GameMgr.Instance.currentDiceCount);
        PlayerPrefs.SetInt("Damage", GameMgr.Instance.curruntBonusStat);
        PlayerPrefs.SetInt("Hp", GameMgr.Instance.GetHp());
        PlayerPrefs.SetInt("MaxHp", GameMgr.Instance.GetMaxHp());

        for ( int i = 0; i < 9 ; i++ )
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Rank");
            sb.Append((i+1).ToString());
            PlayerPrefs.SetInt(sb.ToString(), GameMgr.Instance.GetRank(i));
        }

        for (int i = 0; i < 9; i++)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("RankReward");
            sb.Append((i+1).ToString());
            if (i >= GameMgr.Instance.ui.rewardList.Count)
            {
                PlayerPrefs.SetInt(sb.ToString(), 0);
            }
            else
            {
                PlayerPrefs.SetInt(sb.ToString(), GameMgr.Instance.ui.rewardList[i].ID);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Artifact");
            sb.Append((i+1).ToString());
            PlayerPrefs.SetInt(sb.ToString(), GameMgr.Instance.artifact.playersArtifactsNumber[i]);
        }

        PlayerPrefs.Save();
    }

    public static int Load()
    {
        if (PlayerPrefs.GetInt("Save", 0) == 1)
        {
            StageMgr.Instance.currentField = PlayerPrefs.GetInt("Stage", 0) / 10;
            StageMgr.Instance.currentStage = PlayerPrefs.GetInt("Stage", 0) % 10;
            GameMgr.Instance.currentDiceCount = (GameMgr.DiceCount)PlayerPrefs.GetInt("Dice", 2);
            GameMgr.Instance.curruntBonusStat = PlayerPrefs.GetInt("Damage", 0);
            GameMgr.Instance.SetHp(PlayerPrefs.GetInt("Hp", 100));
            GameMgr.Instance.SetMaxHp(PlayerPrefs.GetInt("MaxHp", 100));
            for (int i = 0; i < 9; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Rank");
                sb.Append((i+1).ToString());
                GameMgr.Instance.SetRank(i,PlayerPrefs.GetInt(sb.ToString(), 0));
            }

            GameMgr.Instance.ui.rewardList.Clear();
            for (int i = 0; i < 9; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("RankReward");
                sb.Append((i + 1).ToString());
                int rewardID = PlayerPrefs.GetInt(sb.ToString(), 0);
                if (rewardID != 0)
                {
                    GameMgr.Instance.ui.rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(rewardID));
                }
            }

            for (int i = 0; i < 3; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Artifact");
                sb.Append((i + 1).ToString());
                GameMgr.Instance.artifact.playersArtifactsNumber[i] = PlayerPrefs.GetInt(sb.ToString(), -1);
            }
        }
        return PlayerPrefs.GetInt("Save", 0);
    }
    
    public static void DeleteSaveData()
    {
        PlayerPrefs.SetInt("Save", 0);
    }

}
