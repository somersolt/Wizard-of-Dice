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

    }// 싱글톤 패턴

    private enum PosNum
    {
        Left = 0,
        Center = 1,
        Right = 2,
    }

    [SerializeField]
    private TextMeshProUGUI StageInfo;
    public int currentStage;

    public List<Enemy> enemies = new List<Enemy>();
    public List<Enemy> DeadEnemies = new List<Enemy>();
    public Enemy testPrefab; // TO-DO 테스트 적
    public Enemy bosstestPrefab; // TO-DO 테스트 적

    public EnemySpawner enemySpawner;
    private void Awake()
    {
        currentStage = 1;
        SetEnemy();
    }


    public void NextStage()
    {
        currentStage++;
        GameMgr.Instance.TurnUpdate(10);
        if (currentStage == 4)
        {
            GameMgr.Instance.GetDice4Ranks();
            SetEnemy();

        }
        else if (currentStage == 7)
        {
            GameMgr.Instance.GetDice5Ranks();
            SetEnemy();

        }
        else if (currentStage == 3 || currentStage == 6)
        {
            enemySpawner.Spawn(bosstestPrefab, (int)PosNum.Center);

        }
        else
        {
            SetEnemy();
        }
        StageInfo.text = $"Stage {currentStage}";

        switch (GameMgr.Instance.currentDiceCount)
        {
            case GameMgr.DiceCount.three:
                DiceMgr.Instance.DiceThree();
                DiceMgr.Instance.DiceRoll(true);
                break;
            case GameMgr.DiceCount.four:
                DiceMgr.Instance.DiceFour();
                DiceMgr.Instance.DiceRoll(true);
                break;
            case GameMgr.DiceCount.five:
                DiceMgr.Instance.DiceFive();
                DiceMgr.Instance.DiceRoll(true);
                break;
        }
    }

    private void SetEnemy()
    {
        int enemyCount = UnityEngine.Random.Range(0, 3);
        switch (enemyCount)
        {
            case 0:
                enemySpawner.Spawn(testPrefab, (int)PosNum.Center);
                break;
            case 1:
                enemySpawner.Spawn(testPrefab, (int)PosNum.Left);
                enemySpawner.Spawn(testPrefab, (int)PosNum.Right);
                break;
            case 2:
                enemySpawner.Spawn(testPrefab, (int)PosNum.Left);
                enemySpawner.Spawn(testPrefab, (int)PosNum.Center);
                enemySpawner.Spawn(testPrefab, (int)PosNum.Right);
                break;
        }

    }


}
