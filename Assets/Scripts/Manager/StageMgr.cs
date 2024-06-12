using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageMgr : MonoBehaviour
{
    public Mediator mediator;
    private GameMgr gameMgr;
    private DiceMgr diceMgr;

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

    public Toggle bossGimic;
    [SerializeField]
    private GameObject bossGimicPanel;
    [SerializeField]
    private TextMeshProUGUI bossGimicText;

    [SerializeField]
    private SpriteRenderer backGround;

    public int TutorialStage = 0;
    public int latStage;
    public int lastField;

    public EnemySpawner enemySpawner;
    private void Awake()
    {
        latStage = 7;
        lastField = 4;
        bossGimic.onValueChanged.AddListener((isOn) => OnToggleValueChanged(isOn));
    }

    private void Start()
    {
        gameMgr = mediator.gameMgr;
        diceMgr = mediator.diceMgr;
    }
    public void GameStart()
    {
        currentStage = 0;
        currentField = 1;
        enemySpawner.Spawn(tutorialEnemy, (int)PosNum.Center);
        StageInfo.text = $"Tutorial";
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            bossGimicPanel.gameObject.SetActive(true);
        }
        else
        {
            bossGimicPanel.gameObject.SetActive(false);
        }
    }

    public void NextStage(bool LoadData = false)
    {

        if (!LoadData)
        {
            currentStage++;
            if (currentStage == 8)
            {
                currentStage = 1;
                currentField++;
            }

            SaveLoadSystem.Save(gameMgr, this);
        }

        if (currentField == 3 || currentField == 4)
        {
            gameMgr.artifact.valueData.Stat1 = 5;
        }

        gameMgr.ui.ArtifactInfoUpdate();

        StageInfo.text = $"Stage \n {currentField} - {currentStage}";
        var path = currentField.ToString() + currentStage.ToString();
        backGround.sprite = Resources.Load<Sprite>(string.Format("Field/{0}", path));
        if (LoadData)
        {
            BGM.Instance.PlayBGM(BGM.Instance.StartBgm(currentField, currentStage), 3);
        }
        else
        {
            BGM.Instance.PlayBGM(BGM.Instance.ChangeBgm(currentField, currentStage), 3);
        }
        gameMgr.TurnUpdate(10);
        if (currentStage == 1 && currentField == 1)
        {
            gameMgr.tutorial.skipButton.gameObject.SetActive(false);
            gameMgr.LifeMax();
            gameMgr.currentDiceCount = GameMgr.DiceCount.three;
            SetEnemy();
        }
        else if (currentStage == 6)
        {
            gameMgr.ui.EventStage();
            return;
        }
        else
        {
            SetEnemy();
        }


        switch (gameMgr.currentDiceCount)
        {
            case GameMgr.DiceCount.three:
                diceMgr.DiceThree();
                diceMgr.DiceRoll(true);
                break;
            case GameMgr.DiceCount.four:
                diceMgr.DiceFour();
                diceMgr.DiceRoll(true);
                break;
            case GameMgr.DiceCount.five:
                diceMgr.DiceFive();
                diceMgr.DiceRoll(true);
                break;
        }
    }

    private void SetEnemy()
    {
        MonsterData enemyData = new MonsterData();
        keyList = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).AllItemIds;
        if (currentStage == 7)
        {
            bossGimic.gameObject.SetActive(true);
            if (currentField == 1)
            {
                bossGimicText.text = "<size=50> 킹 슬라임\n <size=35><color=red> -매 턴 체력 10 회복";
            }

            if (currentField == 2)
            {
                bossGimicText.text = "<size=50> 사악한 마법사 \n <size=35><color=red> -매 턴 체력 10 회복 \n-2회 공격";
                diceMgr.SetEnemyDiceCount(1);
                gameMgr.bossDoubleAttack = true;
                gameMgr.AttackCount = 2;

            }

            if (currentField == 3)
            {
                bossGimicText.text = "<size=50> 데스 \n <size=35><color=red> -주사위 개수 +1 \n-2회 공격";
                diceMgr.SetEnemyDiceCount(2);
                gameMgr.bossDoubleAttack = true;
                gameMgr.AttackCount = 2;

            }
            else if (currentField == 4)
            {
                bossGimicText.text = "<size=50> 데스 메이지 \n <size=35><color=red> -5턴 동안 데미지 면역 \n-주사위 개수 +1\n-5턴 이후 주사위 개수 +2, 2회 공격";
                diceMgr.SetEnemyDiceCount(2);
                gameMgr.bossDoubleAttack = false;
                gameMgr.AttackCount = 1;
            }
        }
        else
        {
            bossGimic.isOn = false;
            bossGimic.gameObject.SetActive(false);
            diceMgr.SetEnemyDiceCount(1);
            gameMgr.bossDoubleAttack = false;
            gameMgr.AttackCount = 1;
        }

        foreach (int i in keyList)
        {
            if (i / 1000 == 2)
            {
                if (i % 100 / 10 == currentField && i % 10 == currentStage && currentStage != 7)
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
            else if (i / 1000 == 3 && currentStage == 7)
            {
                if (i % 100 / 10 == currentField)
                {
                    var BossEnemyData = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).Get(i);
                    enemyData = DataTableMgr.Get<MonsterTable>(DataTableIds.Monster).Get(i - 1000);

                    var Boss = Resources.Load<Enemy>(string.Format("Prefabs/Monsters/{0}", BossEnemyData.ID));
                    Boss.isBoss = true;
                    if (currentField == 4)
                    {
                        Boss.isimmune = true;
                    }
                    var enemy = Resources.Load<Enemy>(string.Format("Prefabs/Monsters/{0}", enemyData.ID));
                    enemySpawner.Spawn(enemy, (int)PosNum.Left, enemyData);
                    enemySpawner.Spawn(Boss, (int)PosNum.Center, BossEnemyData);
                    enemySpawner.Spawn(enemy, (int)PosNum.Right, enemyData);

                    return;
                }
            }
        }
    }


}
