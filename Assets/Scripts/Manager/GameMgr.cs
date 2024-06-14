using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class GameMgr : MonoBehaviour
{
    public Mediator mediator;
    private DiceMgr diceMgr;
    private StageMgr stageMgr;

    public void mediatorCaching()
    {
        diceMgr = mediator.diceMgr;
        stageMgr = mediator.stageMgr;
    }

    public Tutorial tutorial;
    [HideInInspector]
    public bool tutorialMode;

    private bool onTicWait;
    private int dodgeNumber;
    public enum TurnStatus
    {
        PlayerDice = 0,
        PlayerAttack = 1,
        GetRewards = 2,
        MonsterDice = 3,
        MonsterAttack = 4,
        PlayerLose = 5,
    }

    private TurnStatus currentStatus = 0;
    public TurnStatus CurrentStatus
    {
        get { return currentStatus; }
        set { currentStatus = value; }
    }
    [HideInInspector]
    public int currentTurn = 1;
    [SerializeField]
    private TextMeshProUGUI turnInfo;
    private bool enemyDiceRoll;
    /// </summary>
    /// 주사위 , 마법서 관련 필드

    public Canvas canvas;
    public PlayerMessage messagePrefab;
    public GameObject messagePos;
    public ParticleSystem playerHitParticleS;
    public ParticleSystem playerHitParticleL;
    public enum DiceCount
    {
        two = 2,
        three = 3,
        four = 4,
        five = 5,
    }

    [HideInInspector]
    public DiceCount currentDiceCount;
    int[] rankList = new int[(int)Ranks.count]; // 플레이어의 족보 리스트
    private RanksFlag currentRanks; //현재 주사위로 나온 족보

    [HideInInspector]
    public int curruntBonusStat; // 추가 스탯 . 이거까지 저장해야됨

    [SerializeField]
    private TextMeshProUGUI[] ranksInfo = new TextMeshProUGUI[9];

    private int currentValue;
    [SerializeField]
    private TextMeshProUGUI damageInfo;
    [SerializeField]
    private TextMeshProUGUI targetInfo;

    [HideInInspector]
    public int currentDamage;
    [HideInInspector]
    public int currentBarrier;
    [HideInInspector]
    public int currentRecovery;
    [HideInInspector]
    public int currentTarget;

    public TextMeshProUGUI HpUpText;
    public TextMeshProUGUI BarrierUpText;

    [HideInInspector]
    public int enemyValue;
    [SerializeField]
    private Image PlayerHpBarInfo;
    [SerializeField]
    private TextMeshProUGUI PlayerHpInfo;
    [SerializeField]
    private Image PlayerBarrierBarInfo;
    [SerializeField]
    private TextMeshProUGUI PlayerBarrierInfo;

    private int PlayerHp;
    private int PlayerHpMax = 100;
    private int PlayerBarrier;
    private int PlayerBarrierStart = 0;
    ///  
    /// </summary>
    private int livingMonster = 0;
    private int ticCount = 0;
    [HideInInspector]
    public int monsterSignal = 0;
    [HideInInspector]
    public int bossSignal = 0;

    [HideInInspector]
    public int attackCount;
    [HideInInspector]
    public bool bossDoubleAttack;
    [HideInInspector]
    public int enemyDiceCount;

    [SerializeField]
    private Button option;

    [SerializeField]
    private GameObject QuitPanel;
    [SerializeField]
    private Button quit;
    [SerializeField]
    private Button cancle;
    private bool onBackButton;

    /// <summary>
    /// 소리 관련
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    [HideInInspector]
    public int scrollsound = 0;
    /// </summary>
    private void Awake()
    {

        for (int i = 0; i < ranksInfo.Length; i++)
        {
            ranksInfo[i].text = string.Empty;
        }
        PlayerHp = PlayerHpMax;
        PlayerBarrier = PlayerBarrierStart;
        LifeUpdate();


        EventBus.Subscribe(EventType.PlayerAttack, PlayerAttack);
        EventBus.Subscribe(EventType.MonsterAttack, MonsterAttack);

        option.onClick.AddListener(() => pauseGame());
        quit.onClick.AddListener(() => QuitGame());
        cancle.onClick.AddListener(() => { onBackButton = false; QuitPanel.gameObject.SetActive(false); });
        enemyDiceCount = 1;
    }


    private void Start()
    {
        mediatorCaching();

        TurnUpdate(10);
        currentDiceCount = DiceCount.two;
        diceMgr.DiceSet();
        ScrollsClear();
        RanksListUpdate();

        rankList[0] = 1; // 원페어 등록

        var onepair2 = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(RankIdsToInt.rankids[0] + 1);
        mediator.ui.rewardPanel.rewardList.Add(onepair2);
        // 상점 원페어 2레벨 등록

        for (int i = 1; i < (int)Ranks.TwoPair; i++)
        {
            var spells = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(RankIdsToInt.rankids[i]);
            // 상점 트리플, 3스트레이트 1레벨 등록
            mediator.ui.rewardPanel.rewardList.Add(spells);
        }


        if (LoadData())
        {
            return;
        }

        stageMgr.StartGameSet();
        if (PlayerPrefs.GetInt("Tutorial", 0) == 1)
        {
            tutorial.TutorialSkip();
            return;
        }
        mediator.bgm.currentAudio.Play();
    }
    private void Update()
    {
        audioSource.volume = mediator.bgm.SFXsound;
        mediator.ui.audioSource.volume = mediator.bgm.SFXsound;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            currentBarrier = 100;
            LifeUpdate();
        }
#endif
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onBackButton)
            {
                onBackButton = false;
                QuitPanel.gameObject.SetActive(false);
            }
            else
            {
                onBackButton = true;
                QuitPanel.gameObject.SetActive(true);
            }
        }

        switch (currentStatus)
        {
            case TurnStatus.PlayerDice:
                PlayerDiceUpdate();
                break;
            case TurnStatus.PlayerAttack:
                PlayerAttackUpdate();
                break;
            case TurnStatus.GetRewards:
                GetRewardsUpdate();
                break;
            case TurnStatus.MonsterDice:
                MonsterDiceUpdate();
                break;
            case TurnStatus.MonsterAttack:
                MonsterAttackUpdate();
                break;
            case TurnStatus.PlayerLose:
                PlayerLoseUpdate();
                break;
        }

    }

    private void PlayerDiceUpdate()
    {
#if UNITY_EDITOR
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    currentDiceCount = DiceCount.three;
        //    DiceMgr.Instance.DiceThree();
        //    DiceMgr.Instance.DiceRoll(true);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    //GetDice4Ranks();
        //    currentDiceCount = DiceCount.four;
        //    DiceMgr.Instance.DiceFour();
        //    DiceMgr.Instance.DiceRoll(true);

        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    //GetDice4Ranks();
        //    //GetDice5Ranks();
        //    currentDiceCount = DiceCount.five;
        //    DiceMgr.Instance.DiceFive();
        //    DiceMgr.Instance.DiceRoll(true);
        //}
        if (Input.GetKeyDown(KeyCode.X))
        {
            mediator.ui.artifactRewardPanel.OnArtifactReward();
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentTarget = 1;
            targetInfo.text = currentTarget.ToString();

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentTarget = 2;
            targetInfo.text = currentTarget.ToString();

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentTarget = 3;
            targetInfo.text = currentTarget.ToString();

        }


        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentDamage = 300;
            damageInfo.text = currentDamage.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            curruntBonusStat = 300;
            Debug.Log("공격력 300");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            mediator.artifacts.playersArtifactsLevel[0] = 1;
            Debug.Log("1번유물");

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            diceMgr.Artifact2();
            Debug.Log("2번유물");

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            diceMgr.manipulList[0] = 1;
            diceMgr.manipulList[1] = 2;
            diceMgr.manipulList[2] = 3;
            Debug.Log("3번유물");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            mediator.artifacts.playersArtifactsLevel[4] = 1;
            Debug.Log("5번유물");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            mediator.artifacts.playersArtifactsLevel[5] = 1;
            Debug.Log("6번유물");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            mediator.artifacts.playersArtifactsLevel[6] = 1;
            Debug.Log("7번유물");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            mediator.artifacts.playersArtifactsLevel[7] = 1;
            Debug.Log("8번유물");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            mediator.artifacts.playersArtifactsLevel[8] = 1;
            Debug.Log("9번유물");
        }
        // 테스트 코드
#endif
    }
    private void PlayerAttackUpdate()
    {
        if (bossSignal == 1)
        {
            MonsterCheck();
            return;
        }
        if (livingMonster == monsterSignal)
        {
            MonsterCheck();
        }
    }
    private void GetRewardsUpdate()
    {
        return;
    }
    private void MonsterDiceUpdate()
    {
        if (!enemyDiceRoll)
        {
            if (stageMgr.currentField < 3)
            {
                foreach (var enemy in stageMgr.enemies)
                {
                    if (enemy.isBoss)
                    {
                        enemy.OnHeal(10);
                    }
                }
            }
            enemyDiceRoll = true;
            diceMgr.EnemyDiceRoll();
        }
    }

    private void MonsterAttackUpdate()
    {
        if (PlayerHp <= 0)
        {
            if (mediator.artifacts.playersArtifactsLevel[7] == 1) //8번 유물
            {
                UseArtifact8();
                PlayerHp = mediator.artifacts.valueData.Value7;

                var DamageMessage = Instantiate(messagePrefab, canvas.transform);
                DamageMessage.Setup("유물 사용!", Color.blue);
                audioSource.PlayOneShot(audioClips[1]);
                DamageMessage.GetComponent<RectTransform>().anchoredPosition =
                    messagePos.GetComponent<RectTransform>().anchoredPosition;
            }
            else
            {
                SaveLoadSystem.DeleteSaveData();
                currentStatus = TurnStatus.PlayerLose;
                mediator.bgm.currentAudio.Stop();
                audioSource.PlayOneShot(audioClips[4]);

                return;
            }
            //플레이어 사망 체크
        }


        if (monsterSignal == stageMgr.enemies.Count && attackCount > 1)
        {
            EventBus.Publish(EventType.MonsterAttack);
            attackCount--;
            return;
        }


        if (mediator.artifacts.playersArtifactsLevel[0] == 1)
        {
            if (monsterSignal == stageMgr.enemies.Count && !onTicWait)
            {
                monsterSignal = 0;
                ticCount = stageMgr.enemies.Count;
                foreach (var enemy in stageMgr.enemies)
                {
                    enemy.OnTicDamage(curruntBonusStat); //유물 1번
                }
                audioSource.PlayOneShot(audioClips[9]);

                onTicWait = true;
            }

            if (bossSignal == 1)
            {
                onTicWait = false;
                MonsterCheck();
                return;
            }

            if (monsterSignal == ticCount && onTicWait)
            {
                onTicWait = false;
                MonsterCheck();

                if (CurrentStatus != TurnStatus.GetRewards)
                {
                    NextTurn();
                }
            }

        }
        else
        {
            if (monsterSignal == stageMgr.enemies.Count)
            {
                NextTurn();
            }
        }

    }
    private void PlayerLoseUpdate()
    {
        mediator.ui.GameOver();
    }

    private void NextTurn()
    {
        monsterSignal = 0;
        TurnUpdate(--currentTurn);
        if (currentTurn < 0)
        {
            currentStatus = TurnStatus.PlayerLose;
            return;
        }

        PlayerBarrier = 0;
        LifeUpdate();

        currentStatus = TurnStatus.PlayerDice;

        if (bossDoubleAttack)
        {
            attackCount = 2;
        }

        if (tutorialMode)
        {
            tutorial.eventCount++;
            return;
        }

        diceMgr.DiceSet();
        diceMgr.DiceRoll(true);
    }

    public int[] GetRankList()
    {
        int[] copy = new int[rankList.Length];
        Array.Copy(rankList, copy, rankList.Length);
        return copy; // 배열 확인용 복사본 반환
    }

    public int GetRank(int i)
    {
        return rankList[i];
    }
    public void SetRank(int i, int j)
    {
        rankList[i] = j;
    }

    public void SetRankList(int i)
    {
        if (rankList[i] == 4)
        {
            return;
            //max level
        }

        rankList[i] += 1;
    }

    public void GetDice4Ranks()
    {
        for (int i = (int)Ranks.TwoPair; i < (int)Ranks.FullHouse; i++)
        {
            // 투페어 4스트레이트, 카인드4 1레벨 등록
            var spells = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(RankIdsToInt.rankids[i]);
            mediator.ui.rewardPanel.rewardList.Add(spells);
        }
    }
    public void GetDice5Ranks()
    {
        for (int i = (int)Ranks.FullHouse; i < (int)Ranks.count; i++)
        {
            // 풀하우스 5스트레이트, 카인드5 1레벨 등록
            var spells = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(RankIdsToInt.rankids[i]);
            mediator.ui.rewardPanel.rewardList.Add(spells);
        }
    }

    public void SetResult(RanksFlag ranks, int value)
    {
        currentRanks = ranks;
        currentValue = value;

        currentDamage = DamageCheckSystem.DamageCheck(currentValue, rankList, currentRanks, this, diceMgr, mediator.ui);
        damageInfo.text = currentDamage.ToString();
        targetInfo.text = currentTarget.ToString();

        if (currentRecovery > 0)
        {
            HpUpText.text = "↑" + currentRecovery;
        }
        else
        {
            HpUpText.text = string.Empty;
        }

        if (currentBarrier > 0)
        {
            BarrierUpText.text = "↑" + currentBarrier;
        }
        else
        {
            BarrierUpText.text = string.Empty;
        }

        mediator.ui.SetInfomationButton(true);

        for (int i = 0; i < ranksInfo.Length; i++)
        {
            RanksFlag currentFlag = (RanksFlag)(1 << i);
            if ((currentRanks & currentFlag) != 0)
            {
                ranksInfo[i].color = Color.red;
                scrollsound++;
            }
        }

        if (scrollsound > 0 && scrollsound < 3)
        {
            audioSource.PlayOneShot(audioClips[0]);
        }
        else if (scrollsound >= 3)
        {
            audioSource.PlayOneShot(audioClips[1]);
        }
    }

    public void RanksListUpdate()
    {
        RankCheckSystem.ranksList = rankList;
        for (int i = 0; i < ranksInfo.Length; i++)
        {
            if (rankList[i] != 0)
            {
                var spell = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(RankIdsToInt.rankids[i] + rankList[i] - 1);
                ranksInfo[i].text = mediator.ui.magicInfoPanel.MagicInfomationUpdate(i, spell, rankList[i]);
            }
        }
    }

    public void TurnUpdate(int n)
    {
        currentTurn = n;
        turnInfo.text = currentTurn.ToString();
        currentStatus = TurnStatus.PlayerDice;

        if (stageMgr.currentField == stageMgr.lastField && currentTurn == 5)
        {
            foreach (var boss in stageMgr.enemies)
            {
                boss.isimmune = false;
                boss.ImmuneEffect(false);
                boss.BloodEffect();
                bossDoubleAttack = true;
                attackCount = 2;
            }
            diceMgr.SetEnemyDiceCount(3);
        }
    }

    public void PlayerAttack()
    {
        livingMonster = Math.Min(stageMgr.enemies.Count, currentTarget);
        CurrentStatus = TurnStatus.PlayerAttack;
        audioSource.PlayOneShot(audioClips[2]);
        if (currentRecovery > 0)
        {
            audioSource.PlayOneShot(audioClips[5]);
        }

        PlayerHeal(currentRecovery, currentBarrier);
        HpUpText.text = string.Empty;
        BarrierUpText.text = string.Empty;
        Enemy[] targets = new Enemy[Math.Min(currentTarget, stageMgr.enemies.Count)];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = stageMgr.enemies[i];
        }

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].OnDamage(currentDamage);
        }

    }

    private void MonsterAttack()
    {
        currentStatus = TurnStatus.MonsterAttack;
        enemyDiceRoll = false;
        monsterSignal = 0;
        StartCoroutine(CoMonsterAttackMotion());
    }


    IEnumerator CoMonsterAttackMotion()
    {
        enemyDiceRoll = false;
        monsterSignal = 0;
        for (int i = 0; i < stageMgr.enemies.Count; i++)
        {
            stageMgr.enemies[i].animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void ScrollsClear()
    {
        for (int i = 0; i < ranksInfo.Length; i++)
        {
            ranksInfo[i].color = Color.gray;
        }
        damageInfo.text = "공격력";
        targetInfo.text = "타겟";
        mediator.ui.SetInfomationButton(false);
        mediator.ui.InfomationClear();
        scrollsound = 0;
    }

    private void MonsterCheck()
    {
        monsterSignal = 0;
        if (bossSignal == 1)
        {
            List<Enemy> indicesToRemove = new List<Enemy>();
            foreach (var e in stageMgr.enemies)
            {
                indicesToRemove.Add(e);
            }
            foreach (var e in indicesToRemove)
            {
                e.Die();
            }

            bossSignal = 0;
            monsterSignal = 0;
        }
        if (stageMgr.enemies.Count == 0)
        {
            foreach (var deadEnemy in stageMgr.deadEnemies)
            {
                Destroy(deadEnemy.gameObject);
            }
            stageMgr.deadEnemies.Clear();

            PlayerBarrier = 0;
            LifeUpdate();
            RewardCheck();
            return;
        }
        else
        {
            currentStatus = TurnStatus.MonsterDice;
        }
    }

    public void RewardCheck()
    {
        if (stageMgr.currentStage == stageMgr.lastStage && stageMgr.currentField == stageMgr.lastField)
        {
            mediator.bgm.PlayBGM(mediator.bgm.bgmList[1], 2);
            SaveLoadSystem.DeleteSaveData();
            mediator.ui.Victory();
            return;
        }

        currentStatus = TurnStatus.GetRewards;
        if (stageMgr.tutorialStage == stageMgr.currentStage)
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            mediator.ui.getDicePanel.GetDice();
            return;
        }
        if (stageMgr.currentStage == stageMgr.lastStage)
        {
            mediator.ui.artifactRewardPanel.OnArtifactReward();
            return;
        }
        mediator.ui.rewardPanel.OnReward();
    }


    public void PlayerOndamage(int enemyDamage)
    {
        dodgeNumber = 100;
        if (mediator.artifacts.playersArtifactsLevel[5] == 1)
        {
            dodgeNumber = UnityEngine.Random.Range(0, 100);
        }


        if (dodgeNumber < mediator.artifacts.valueData.Value5)
        {
            var DamageMessage = Instantiate(messagePrefab, messagePos.transform);
            audioSource.PlayOneShot(audioClips[3]);
            DamageMessage.Setup("Miss!", Color.magenta);
            DamageMessage.GetComponent<RectTransform>().anchoredPosition =
                messagePos.GetComponent<RectTransform>().anchoredPosition;
        }
        else
        {
            if (PlayerBarrier >= enemyDamage)
            {
                PlayerBarrier -= enemyDamage;

                var DamageMessage = Instantiate(messagePrefab, messagePos.transform);
                DamageMessage.Setup(enemyDamage, Color.blue);
                DamageMessage.GetComponent<RectTransform>().anchoredPosition =
                    messagePos.GetComponent<RectTransform>().anchoredPosition;
                var main = playerHitParticleS.main;
                main.loop = false;
                audioSource.Play();
                playerHitParticleS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                playerHitParticleS.Play();
            }
            else
            {
                enemyDamage -= PlayerBarrier;
                PlayerBarrier = 0;
                PlayerHp -= enemyDamage;
                var DamageMessage = Instantiate(messagePrefab, messagePos.transform);
                DamageMessage.Setup(enemyDamage, Color.red);
                DamageMessage.GetComponent<RectTransform>().anchoredPosition =
                     messagePos.GetComponent<RectTransform>().anchoredPosition;
                audioSource.Play();
                playerHitParticleL.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                playerHitParticleL.Play();
            }
        }

        LifeUpdate();
    }

    public void PlayerHeal(int recovery, int barrier)
    {
        PlayerHp += recovery;
        if (PlayerHp > PlayerHpMax)
        {
            PlayerHp = PlayerHpMax;
        }
        PlayerBarrier += barrier;
        LifeUpdate();
    }

    public void LifeMax()
    {
        PlayerHp = PlayerHpMax;
        LifeUpdate();
    }

    public void SetHp(int i)
    {
        PlayerHp = i;
    }
    public void SetMaxHp(int i)
    {
        PlayerHpMax = i;
    }

    public int GetHp()
    {
        return PlayerHp;
    }
    public int GetMaxHp()
    {
        return PlayerHpMax;
    }

    public void LifeUpdate()
    {
        PlayerBarrierInfo.text = PlayerBarrier.ToString();
        PlayerBarrierBarInfo.fillAmount = (float)PlayerBarrier / PlayerHpMax;
        PlayerHpInfo.text = PlayerHp.ToString();
        PlayerHpBarInfo.fillAmount = (float)PlayerHp / PlayerHpMax;
    }

    public void MaxLifeSet(int i)
    {
        PlayerHpMax += i;
    }

    public void StatusEffectsUpdate(int target, int barrier, int recovery)
    {
        currentTarget = target;
        currentBarrier = barrier;
        currentRecovery = recovery;
    }

    public void UseArtifact8()
    {
        mediator.artifacts.playersArtifactsLevel[7] = 2;
        foreach (var a in mediator.ui.playerArtifactPanel.artifactInfo)
        {
            if (a.text == "원코인 추가")
            {
                a.color = Color.red;
                a.text = "원코인 추가(사용)";
            }
        }
    }

    private bool LoadData()
    {
        if (PlayerPrefs.GetInt("Save", 0) == 0)
        {
            return false;
        }

        tutorialMode = false;
        tutorial.tutorialPanel.gameObject.SetActive(false);
        tutorial.skipButton.gameObject.SetActive(false);

        stageMgr.currentField = SaveLoadSystem.CurrSaveData.savePlay.Stage / 10;
        stageMgr.currentStage = SaveLoadSystem.CurrSaveData.savePlay.Stage % 10;
        currentDiceCount = (DiceCount)SaveLoadSystem.CurrSaveData.savePlay.DiceCount;
        curruntBonusStat = SaveLoadSystem.CurrSaveData.savePlay.Damage;
        PlayerHp = SaveLoadSystem.CurrSaveData.savePlay.Hp;
        PlayerHpMax = SaveLoadSystem.CurrSaveData.savePlay.MaxHp;
        rankList = SaveLoadSystem.CurrSaveData.savePlay.RankList;

        LifeUpdate();
        RanksListUpdate();

        mediator.ui.rewardPanel.rewardList.Clear();
        for (int i = 0; i < 9; i++)
        {
            if (SaveLoadSystem.CurrSaveData.savePlay.RankRewardList[i] == 0)
            {
                continue;
            }
            else
            {
                mediator.ui.rewardPanel.rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(SaveLoadSystem.CurrSaveData.savePlay.RankRewardList[i]));
            }
        }

        for (int i = 0; i < 3; i++)
        {
            mediator.artifacts.playersArtifactsNumber[i] = SaveLoadSystem.CurrSaveData.savePlay.ArtifactList[i];
        }

        for (int i = 0; i < 10; i++)
        {
            mediator.artifacts.playersArtifactsLevel[i] = SaveLoadSystem.CurrSaveData.savePlay.ArtifactLevelList[i];
        }



        for (int i = 0; i < rankList.Length; i++)
        {
            if (rankList[i] == 3)
            {
                mediator.ui.maxSpellRewardPanel.maxSpells[i].gameObject.SetActive(true);
            }
        } // 초월마법등록

        foreach (var artifact in mediator.artifacts.artifactList)
        {
            if (artifact.ID == 0)
            {
                artifact.Set(0, "방화광", $"매턴 모든 적에게 '기본 공격력'(<color=purple>{curruntBonusStat}</color>) 만큼의 데미지");
            }
        } //유물상점 업데이트

        if (mediator.artifacts.playersArtifactsNumber[0] != -1)
        {
            foreach (var a in mediator.artifacts.artifactList)
            {
                if (a.ID == mediator.artifacts.playersArtifactsNumber[0])
                {
                    mediator.artifacts.artifactList.Remove(a);
                    mediator.ui.playerArtifactPanel.ArtifactUpdate(a, 0);
                    LoadArtifactCheck(a.ID);
                    break;
                }
            }

            if (mediator.artifacts.playersArtifactsNumber[1] != -1)
            {
                foreach (var a in mediator.artifacts.artifactList)
                {
                    if (a.ID == mediator.artifacts.playersArtifactsNumber[1])
                    {
                        mediator.artifacts.artifactList.Remove(a);
                        mediator.ui.playerArtifactPanel.ArtifactUpdate(a, 1);
                        LoadArtifactCheck(a.ID);
                        break;
                    }
                }
                if (mediator.artifacts.playersArtifactsNumber[2] != -1)
                {
                    foreach (var a in mediator.artifacts.artifactList)
                    {
                        if (a.ID == mediator.artifacts.playersArtifactsNumber[2])
                        {
                            mediator.artifacts.artifactList.Remove(a);
                            mediator.ui.playerArtifactPanel.ArtifactUpdate(a, 2);
                            LoadArtifactCheck(a.ID);
                            break;
                        }
                    }
                }
            }
        }

        foreach (var a in mediator.artifacts.playersArtifactsLevel)
        {
            if (a == 2)
            {
                UseArtifact8();
            }
        }

        stageMgr.NextStage(true);
        return true;
    }

    private void LoadArtifactCheck(int id)
    {
        if (id == 1)
        {
            diceMgr.Artifact2();
        }
        else if (id == 2)
        {
            diceMgr.manipulList[0] = 1;
            diceMgr.manipulList[1] = 2;
            diceMgr.manipulList[2] = 3;
        }
    }


    private void pauseGame()
    {
        Time.timeScale = 0;
        mediator.ui.pausePanel.gameObject.SetActive(true);
        mediator.ui.backGroundPanel.gameObject.SetActive(true);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
