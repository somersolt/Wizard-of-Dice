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
    public int currentField;

    public List<Enemy> enemies = new List<Enemy>();
    public List<Enemy> DeadEnemies = new List<Enemy>();
    public List<int> keyList = new List<int>();
    public Enemy tutorialEnemy; // TO-DO 테스트 적

    [SerializeField]
    private SpriteRenderer backGround;

    public int TutorialStage = 0;
    public int latStage = 5;
    public int lastField = 4;

    public EnemySpawner enemySpawner;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        currentStage = 0;
        currentField = 1;
        enemySpawner.Spawn(tutorialEnemy, (int)PosNum.Center);
        StageInfo.text = $"Tutorial";

    }


    public void NextStage()
    {

        currentStage++;
        if (currentStage == 6)
        {
            currentStage = 1;
            currentField++;
        }

        var path = currentField.ToString() + currentStage.ToString();
        backGround.sprite = Resources.Load<Sprite>(string.Format("Field/{0}", path));

        GameMgr.Instance.TurnUpdate(10);
        if (currentStage == 1 && currentField == 1)
        {
            GameMgr.Instance.tutorial.skipButton.gameObject.SetActive(false);
            GameMgr.Instance.LifeMax();
            GameMgr.Instance.currentDiceCount = GameMgr.DiceCount.three;
            SetEnemy();
        }
        else if (currentStage == 4)
        {
            GameMgr.Instance.ui.OnDiceReward();
            return;
        }

        else
        {
            SetEnemy();
        }
        StageInfo.text = $"Stage {currentField} - {currentStage}";

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
        MonsterData enemyData = new MonsterData();
        keyList = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).AllItemIds;
        foreach (int i in keyList)
        {
            if (i / 1000 == 2)
            {
                if (i % 100 / 10 == currentField && i % 10 == currentStage && currentStage != 5)
                {
                    enemyData = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).Get(i);

                    var enemy = Resources.Load<Enemy>(string.Format("Prefabs/Monsters/{0}", enemyData.ID)).GetComponent<Enemy>();
                    var count = enemyData.MONSTER_COUNT;
                    switch (count)
                    {
                        case 1:
                            enemySpawner.Spawn(enemy, (int)PosNum.Center, enemyData);
                            break;
                        case 2:
                            enemySpawner.Spawn(enemy, (int)PosNum.Left, enemyData);
                            enemySpawner.Spawn(enemy, (int)PosNum.Right, enemyData);
                            break;
                        case 3:
                            enemySpawner.Spawn(enemy, (int)PosNum.Left, enemyData);
                            enemySpawner.Spawn(enemy, (int)PosNum.Center, enemyData);
                            enemySpawner.Spawn(enemy, (int)PosNum.Right, enemyData);
                            break;
                    }

                    return;
                }
            }
            else if (i / 1000 == 3 && currentStage == 5)
            {
                if (i % 100 / 10 == currentField)
                {
                    var BossEnemyData = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).Get(i);
                    enemyData = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).Get(i - 1000);

                    var Boss = Resources.Load<Enemy>(string.Format("Prefabs/Monsters/{0}", BossEnemyData.ID));
                    var enemy = Resources.Load<Enemy>(string.Format("Prefabs/Monsters/{0}", enemyData.ID));
                    enemySpawner.Spawn(enemy, (int)PosNum.Left, enemyData);
                    enemySpawner.Spawn(Boss, (int)PosNum.Center, BossEnemyData);
                    enemySpawner.Spawn(enemy, (int)PosNum.Right, enemyData);

                    return;
                }
            }
        }
        Debug.Log("적 데이터 없음");
    }


}
