using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageMgr : MonoBehaviour
{
    public Mediator mediator;
    private GameMgr gameMgr;
    private DiceMgr diceMgr;


    public void mediatorCaching()
    {
        gameMgr = mediator.gameMgr;
        diceMgr = mediator.diceMgr;
    }
    private enum EnemySideIndex
    {
        Left = 0,
        Center = 1,
        Right = 2,
    }

    [SerializeField]
    private TextMeshProUGUI stageInfo;
    [HideInInspector]
    public int currentStage;
    [HideInInspector]
    public int currentField;
    [HideInInspector]
    public List<Enemy> enemies = new List<Enemy>();
    [HideInInspector]
    public List<Enemy> deadEnemies = new List<Enemy>();
    [HideInInspector]
    public List<int> keyList = new List<int>();
    public Enemy tutorialEnemy;

    public Toggle bossGimic;
    [SerializeField]
    private GameObject bossGimicPanel;
    [SerializeField]
    private TextMeshProUGUI bossGimicText;

    [SerializeField]
    private SpriteRenderer backGround;

    [HideInInspector]
    public int tutorialStage = 0;
    [HideInInspector]
    public int lastStage;
    [HideInInspector]
    public int lastField;
    [HideInInspector]
    public int eventStage;

    public EnemySpawner enemySpawner;
    private void Awake()
    {
        lastStage = 7;
        lastField = 4;
        eventStage = 6;
        bossGimic.onValueChanged.AddListener((isOn) => OnToggleValueChanged(isOn));
    }

    private void Start()
    {
        mediatorCaching();
    }
    public void StartGameSet()
    {
        currentStage = 0;
        currentField = 1;
        enemySpawner.Spawn(tutorialEnemy, (int)EnemySideIndex.Center);
        stageInfo.text = "Tutorial";
    }

    private void OnToggleValueChanged(bool isOn)
    {
        bossGimicPanel.gameObject.SetActive(isOn);
    }

    public void NextStage(bool LoadData = false)
    {

        if (LoadData)
        {
            mediator.Caching();
            mediator.bgm.PlayBGM(mediator.bgm.GetBgmOnGameStart(currentField, currentStage), 3);
        }
        else
        {
            currentStage++;
            if (currentStage > lastStage)
            {
                currentStage = 1;
                currentField++;
            }
            mediator.bgm.PlayBGM(mediator.bgm.GetBgmOnNextStage(currentField, currentStage), 3);
            SaveLoadSystem.Save(gameMgr, this, mediator.ui);
        }

        if (currentField > 2)
        {
            gameMgr.artifact.valueData.Stat1 = 5;
        }

        mediator.ui.ArtifactInfoUpdate();

        stageInfo.text = $"Stage \n {currentField} - {currentStage}";
        var path = currentField.ToString() + currentStage.ToString();
        backGround.sprite = Resources.Load<Sprite>(string.Format("Field/{0}", path));

        gameMgr.TurnUpdate(10);

        if (currentStage == 1 && currentField == 1)
        {
            gameMgr.tutorial.skipButton.gameObject.SetActive(false);
            gameMgr.LifeMax();
            gameMgr.currentDiceCount = GameMgr.DiceCount.three;
            SetEnemy();
        }
        else if (currentStage == eventStage)
        {
            mediator.ui.EventStage();
            return;
        }
        else
        {
            SetEnemy();
        }
        diceMgr.DiceSet();
        diceMgr.DiceRoll(true);
    }

    private void SetEnemy()
    {
        MonsterData enemyData = new MonsterData();
        keyList = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).AllItemIds;
        if (currentStage == lastStage)
        {
            BossGimicTextSet();
        }
        else
        {
            bossGimic.isOn = false;
            bossGimic.gameObject.SetActive(false);
            diceMgr.currentEnemyDice = 1;
            gameMgr.bossDoubleAttack = false;
            gameMgr.attackCount = 1;
        }
        diceMgr.SetEnemyDiceCount(diceMgr.currentEnemyDice);
        foreach (int i in keyList)
        {
            if (i / 1000 == 2)
            {
                if (i % 100 / 10 == currentField && i % 10 == currentStage && currentStage != lastStage)
                {
                    enemyData = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).Get(i);

                    var enemy = Resources.Load<Enemy>(string.Format("Prefabs/Monsters/{0}", enemyData.ID)).GetComponent<Enemy>();
                    var count = enemyData.MONSTER_COUNT;
                    switch (count)
                    {
                        case 1:
                            enemySpawner.Spawn(enemy, (int)EnemySideIndex.Center, enemyData);
                            break;
                        case 2:
                            enemySpawner.Spawn(enemy, (int)EnemySideIndex.Left, enemyData);
                            enemySpawner.Spawn(enemy, (int)EnemySideIndex.Right, enemyData);
                            break;
                        case 3:
                            enemySpawner.Spawn(enemy, (int)EnemySideIndex.Left, enemyData);
                            enemySpawner.Spawn(enemy, (int)EnemySideIndex.Center, enemyData);
                            enemySpawner.Spawn(enemy, (int)EnemySideIndex.Right, enemyData);
                            break;
                    }

                    return;
                }
            }
            else if (i / 1000 == 3 && currentStage == lastStage)
            {
                if (i % 100 / 10 == currentField)
                {
                    var BossEnemyData = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).Get(i);
                    enemyData = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).Get(i - 1000);

                    var Boss = Resources.Load<Enemy>(string.Format("Prefabs/Monsters/{0}", BossEnemyData.ID));
                    Boss.isBoss = true;
                    if (currentField == lastField)
                    {
                        Boss.isimmune = true;
                    }
                    var enemy = Resources.Load<Enemy>(string.Format("Prefabs/Monsters/{0}", enemyData.ID));
                    enemySpawner.Spawn(enemy, (int)EnemySideIndex.Left, enemyData);
                    enemySpawner.Spawn(Boss, (int)EnemySideIndex.Center, BossEnemyData);
                    enemySpawner.Spawn(enemy, (int)EnemySideIndex.Right, enemyData);

                    return;
                }
            }
        }
    }

    private void BossGimicTextSet()
    {
        bossGimic.gameObject.SetActive(true);
        switch (currentField)
        {
            case 1:
                bossGimicText.text = "<size=50> 킹 슬라임\n <size=35><color=red> -매 턴 체력 10 회복";
                diceMgr.currentEnemyDice = 1;
                break;
            case 2:
                bossGimicText.text = "<size=50> 사악한 마법사 \n <size=35><color=red> -매 턴 체력 10 회복 \n-2회 공격";
                diceMgr.currentEnemyDice = 2;
                gameMgr.bossDoubleAttack = true;
                gameMgr.attackCount = 2;
                break;
            case 3:
                bossGimicText.text = "<size=50> 데스 \n <size=35><color=red> -주사위 개수 +1 \n-2회 공격";
                diceMgr.currentEnemyDice = 2;
                gameMgr.bossDoubleAttack = true;
                gameMgr.attackCount = 2;
                break;
            case 4:
                bossGimicText.text = "<size=50> 데스 메이지 \n <size=35><color=red> -5턴 동안 데미지 면역 \n-주사위 개수 +1\n-5턴 이후 주사위 개수 +2, 2회 공격";
                diceMgr.currentEnemyDice = 2;
                gameMgr.bossDoubleAttack = false;
                gameMgr.attackCount = 1;
                break;
        }
    }

}
