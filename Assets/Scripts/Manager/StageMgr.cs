using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Reflection;

public class StageMgr : MonoBehaviour
{
    private static StageMgr instance;
    public static StageMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StageMgr>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("StageMgr");
                    instance = obj.AddComponent<StageMgr>();
                }
            }
            return instance;
        }

    }// ½Ì±ÛÅæ ÆÐÅÏ

    private enum PosNum
    {
        Left = 0,
        Center = 1,
        Right = 2,
    }

    [SerializeField]
    private TextMeshProUGUI StageInfo;
    private int currentStage;

    public List<Enemy> enemies = new List<Enemy>();
    public Enemy testPrefab; // TO-DO Å×½ºÆ® Àû

    public EnemySpawner enemySpawner;
    private void Awake()
    {
        currentStage = 1;
        enemySpawner.Spawn(testPrefab, (int)PosNum.Center);
    }


    public void NextStage()
    {
        currentStage++;
        if (currentStage == 11)
        {
            //TO-DO ½Â¸®!
        }

        enemySpawner.Spawn(testPrefab, (int)PosNum.Center);

        StageInfo.text = $"Stage {currentStage}";
    }
}   
