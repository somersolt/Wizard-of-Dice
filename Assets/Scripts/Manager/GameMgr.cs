using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class AttackEventArgs : EventArgs
{
    public int value { get; }
    public int target { get; }

    public AttackEventArgs(int param1, int param2)
    {
        value = param1;
        target = param2;
    }

}

public delegate void AttackEventHandler(object sender, AttackEventArgs e);

public class AttackEventPublisher
{
    public event AttackEventHandler AttackEvent;

    public void RaiseEvent(int param1, int param2)
    {
        AttackEvent?.Invoke(this, new AttackEventArgs(param1, param2));
    }
}



public class AttackEventListener
{
    public void AttackHandleEvent(object sender, AttackEventArgs e)
    {
        GameMgr.Instance.Heal(GameMgr.Instance.currentRecovery, GameMgr.Instance.currentBarrier);
        GameMgr.Instance.HpUpText.text = " ";
        GameMgr.Instance.BarrierUpText.text = " ";
        Enemy[] targets = new Enemy[Math.Min(GameMgr.Instance.currentTarget, StageMgr.Instance.enemies.Count)];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = StageMgr.Instance.enemies[i];
        }

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].OnDamage(e.value);
        }

    }
}

public class GameMgr : MonoBehaviour
{
    private static GameMgr instance;

    public static GameMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameMgr>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameMgr");
                    instance = obj.AddComponent<GameMgr>();
                }
            }
            return instance;
        }

    }    // 싱글톤 패턴

    public Tutorial tutorial;
    public bool tutorialMode;
    public UI ui;
    public Artifact artifact;

    private bool onTicWait;
    private int dodgeNuber;
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

    AttackEventPublisher publisher = new AttackEventPublisher();
    AttackEventListener listener = new AttackEventListener();

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

    public DiceCount currentDiceCount;
    int[] RankList = new int[(int)Ranks.count]; // 플레이어의 족보 리스트
    private RanksFlag currentRanks; //현재 주사위로 나온 족보

    public int curruntBonusStat; // 추가 스탯 . 이거까지 저장해야됨

    [SerializeField]
    private TextMeshProUGUI[] ranksInfo = new TextMeshProUGUI[9];

    private int currentValue;
    [SerializeField]
    private TextMeshProUGUI damageInfo;
    [SerializeField]
    private TextMeshProUGUI targetInfo;
    public Button currentMagicInfo;
    public Button getMagicInfo;
    public int currentDamage;
    public int currentBarrier;
    public int currentRecovery;
    public int currentTarget;

    public TextMeshProUGUI HpUpText;
    public TextMeshProUGUI BarrierUpText;


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
    public int monsterSignal = 0;
    public int bossSignal = 0;

    private bool onMonsterAttack = false;

    public int AttackCount;
    public bool bossDoubleAttack;
    private int BossShieldCount;
    public int EnemyDiceCount;

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
    public int scrollsound = 0;
    /// </summary>
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

        for (int i = 0; i < ranksInfo.Length; i++)
        {
            ranksInfo[i].text = string.Empty;
        }
        PlayerHp = PlayerHpMax;
        PlayerBarrier = PlayerBarrierStart;
        LifeUpdate();
        RankList[0] = 1; // 원페어 등록
        RanksListUpdate();

        var onepair2 = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[0] + 1);
        ui.rewardList.Add(onepair2);

        // 상점 원페어 2레벨 등록
        for (int i = 1; i < (int)Ranks.TwoPair; i++)
        {
            var spells = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[i]);
            // 상점 트리플, 3스트레이트 1레벨 등록
            ui.rewardList.Add(spells);
        }

        TurnUpdate(10);
        publisher.AttackEvent += listener.AttackHandleEvent;// 이벤트에 이벤트 핸들러 메서드를 추가
        option.onClick.AddListener(() => pauseGame());
        currentMagicInfo.onClick.AddListener(() => { ui.magicInfoPanel.SetActive(true); ui.BackGroundPanel.SetActive(true); ui.toggleMagicInfo(false); ui.magicInfoToggle.isOn = false; });
        getMagicInfo.onClick.AddListener(() => { ui.magicInfoPanel.SetActive(true); ui.BackGroundPanel.SetActive(true); ui.toggleMagicInfo(true); ui.magicInfoToggle.isOn = true; });
        quit.onClick.AddListener(() => QuitGame());
        cancle.onClick.AddListener(() => { onBackButton = false; QuitPanel.gameObject.SetActive(false); });
        EnemyDiceCount = 1;
    }


    private void Start()
    {
        currentDiceCount = DiceCount.two;
        DiceMgr.Instance.DiceTwo();
        ScrollsClear();
        RanksListUpdate();

        if (LoadData())
        {
            return;
        }

        StageMgr.Instance.GameStart();
        if (PlayerPrefs.GetInt("Tutorial", 0) == 1)
        {
            tutorial.TutorialSkip();
            return;
        }
        BGM.Instance.currentAudio.Play();
    }
    private void Update()
    {
        audioSource.volume = BGM.Instance.SFXsound;
        ui.audioSource.volume = BGM.Instance.SFXsound;
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
            ui.OnArtifactReward();
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
            artifact.playersArtifacts[0] = 1;
            Debug.Log("1번유물");

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            DiceMgr.Instance.Artifact2();
            Debug.Log("2번유물");

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            DiceMgr.Instance.manipulList[0] = 1;
            DiceMgr.Instance.manipulList[1] = 2;
            DiceMgr.Instance.manipulList[2] = 3;
            Debug.Log("3번유물");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            artifact.playersArtifacts[4] = 1;
            Debug.Log("5번유물");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            artifact.playersArtifacts[5] = 1;
            Debug.Log("6번유물");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            artifact.playersArtifacts[6] = 1;
            Debug.Log("7번유물");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            artifact.playersArtifacts[7] = 1;
            Debug.Log("8번유물");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            artifact.playersArtifacts[8] = 1;
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

    }
    private void MonsterDiceUpdate()
    {
        if (!enemyDiceRoll)
        {
            if (StageMgr.Instance.currentField == 1 || StageMgr.Instance.currentField == 2)
            {
                foreach (var enemy in StageMgr.Instance.enemies)
                {
                    if (enemy.isBoss)
                    {
                        enemy.OnHeal(10);
                    }
                }
            }
            enemyDiceRoll = true;
            DiceMgr.Instance.EnemyDiceRoll();
        }
    }

    private void MonsterAttackUpdate()
    {
        if (!onMonsterAttack)
        {
            onMonsterAttack = true;
            StartCoroutine(MonsterEffect());
        }
        if (PlayerHp <= 0)
        {
            if (artifact.playersArtifacts[7] == 1) //8번 유물
            {
                UseArtifact8();
                PlayerHp = artifact.valueData.Value7;

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
                BGM.Instance.currentAudio.Stop();
                audioSource.PlayOneShot(audioClips[4]);

                return;
            }
            //플레이어 사망 체크
        }


        if (monsterSignal == StageMgr.Instance.enemies.Count && AttackCount > 1)
        {
            onMonsterAttack = false;
            monsterSignal = 0;
            AttackCount--;
            return;
        }


        if (artifact.playersArtifacts[0] == 1)
        {
            if (monsterSignal == StageMgr.Instance.enemies.Count && !onTicWait)
            {
                monsterSignal = 0;
                ticCount = StageMgr.Instance.enemies.Count;
                foreach (var enemy in StageMgr.Instance.enemies)
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

                    onMonsterAttack = false;
                    if (bossDoubleAttack)
                    {
                        AttackCount = 2;
                    }

                    if (tutorialMode)
                    {
                        tutorial.eventCount++;
                        return;
                    }

                    switch (currentDiceCount)
                    {
                        case DiceCount.two:
                            DiceMgr.Instance.DiceTwo();
                            DiceMgr.Instance.DiceRoll(true);
                            break;
                        case DiceCount.three:
                            DiceMgr.Instance.DiceThree();
                            DiceMgr.Instance.DiceRoll(true);
                            break;
                        case DiceCount.four:
                            DiceMgr.Instance.DiceFour();
                            DiceMgr.Instance.DiceRoll(true);
                            break;
                        case DiceCount.five:
                            DiceMgr.Instance.DiceFive();
                            DiceMgr.Instance.DiceRoll(true);
                            break;
                    }
                }
            }

        }
        else
        {
            if (monsterSignal == StageMgr.Instance.enemies.Count)
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
                onMonsterAttack = false;
                if (bossDoubleAttack)
                {
                    AttackCount = 2;
                }

                if (tutorialMode)
                {
                    tutorial.eventCount++;
                    return;
                }

                switch (currentDiceCount)
                {
                    case DiceCount.two:
                        DiceMgr.Instance.DiceTwo();
                        DiceMgr.Instance.DiceRoll(true);
                        break;
                    case DiceCount.three:
                        DiceMgr.Instance.DiceThree();
                        DiceMgr.Instance.DiceRoll(true);
                        break;
                    case DiceCount.four:
                        DiceMgr.Instance.DiceFour();
                        DiceMgr.Instance.DiceRoll(true);
                        break;
                    case DiceCount.five:
                        DiceMgr.Instance.DiceFive();
                        DiceMgr.Instance.DiceRoll(true);
                        break;
                }
            }
        }

    }
    private void PlayerLoseUpdate()
    {
        ui.GameOver();
    }



    public int[] GetRankList()
    {
        int[] copy = new int[RankList.Length];
        Array.Copy(RankList, copy, RankList.Length);
        return copy; // 배열 확인용 복사본 반환
    }

    public int GetRank(int i)
    {
        return RankList[i];
    }
    public void SetRank(int i, int j)
    {
        RankList[i] = j;
    }

    public void SetRankList(int i)
    {
        if (RankList[i] == 4)
        {
            return;
            //max level
        }

        RankList[i] += 1;
    }

    public void GetDice4Ranks()
    {
        for (int i = (int)Ranks.TwoPair; i < (int)Ranks.FullHouse; i++)
        {
            // 투페어 4스트레이트, 카인드4 1레벨 등록
            var spells = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[i]);
            ui.rewardList.Add(spells);
        }
    }
    public void GetDice5Ranks()
    {
        for (int i = (int)Ranks.FullHouse; i < (int)Ranks.count; i++)
        {
            // 풀하우스 5스트레이트, 카인드5 1레벨 등록
            var spells = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[i]);
            ui.rewardList.Add(spells);
        }
    }

    public void SetResult(RanksFlag ranks, int value)
    {
        currentRanks = ranks;
        currentValue = value;

        currentDamage = DamageCheckSystem.DamageCheck(currentValue, RankList, currentRanks);
        damageInfo.text = currentDamage.ToString();
        targetInfo.text = currentTarget.ToString();

        if (currentRecovery > 0)
        {
            HpUpText.text = "↑" + currentRecovery;
        }
        else
        {
            HpUpText.text = " ";
        }

        if (currentBarrier > 0)
        {
            BarrierUpText.text = "↑" + currentBarrier;
        }
        else
        {
            BarrierUpText.text = " ";
        }

        currentMagicInfo.interactable = true;
        ui.magicInfoToggle.interactable = true;

        /*
        int r = 0;
        for (int i = 0; i < scrolls.Length; i++)
        {
            scrolls[i].GetComponentInChildren<TextMeshProUGUI>().text = " ";
            for (int j = 8 - r; j >= 0; j--)
            {
                RanksFlag currentFlag = (RanksFlag)(1 << j);

                r++;

                if ((currentRanks & currentFlag) != 0)
                {
                    scrolls[i].GetComponentInChildren<TextMeshProUGUI>().text =
                        DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[j]).GetName;
                    break;
                }
            }
        }
        */ // 스크롤 순서대로 띄워주던 코드


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
        RankCheckSystem.ranksList = RankList;
        for (int i = 0; i < ranksInfo.Length; i++)
        {
            if (RankList[i] != 0)
            {
                var magic = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[i] + RankList[i] - 1);
                ui.infoMagicnames[i].text = magic.GetName;
                ui.infoMagicInfos[i].text = magic.GetDesc;
                StringBuilder newText = new StringBuilder();
                newText.Append(magic.GetName);

                switch (RankList[i])
                {
                    case 1:
                        newText.Append("- 일반");
                        ui.infoMagicLevels[i].text = "일반";
                        ui.infoMagicLevels[i].color = Color.white;
                        ui.infoMagicstars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_1"));
                        break;
                    case 2:
                        newText.Append("- 강화");
                        ui.infoMagicLevels[i].text = "강화";
                        ui.infoMagicLevels[i].color = Color.green;
                        ui.infoMagicstars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_2"));
                        break;
                    case 3:
                        newText.Append("- 숙련");
                        ui.infoMagicLevels[i].text = "숙련";
                        ui.infoMagicLevels[i].color = Color.yellow;
                        ui.infoMagicstars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_3"));
                        break;
                    case 4:
                        newText.Append("- 초월");
                        ui.infoMagicLevels[i].text = "초월";
                        ui.infoMagicLevels[i].color = Color.red;
                        ui.infoMagicstars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_4"));
                        break;
                }
                ranksInfo[i].text = newText.ToString();
            }
        }
    }

    public void TurnUpdate(int n)
    {
        currentTurn = n;
        turnInfo.text = currentTurn.ToString();
        currentStatus = TurnStatus.PlayerDice;

        if (StageMgr.Instance.currentField == 4 && currentTurn == 5)
        {
            foreach (var boss in StageMgr.Instance.enemies)
            {
                boss.isimmune = false;
                boss.ImmuneEffect(false);
                boss.BloodEffect();
                bossDoubleAttack = true;
                AttackCount = 2;
            }
            DiceMgr.Instance.SetEnemyDiceCount(3);

        }

    }

    public void PlayerAttackEffect()
    {
        livingMonster = Math.Min(StageMgr.Instance.enemies.Count, currentTarget);
        CurrentStatus = TurnStatus.PlayerAttack;
        audioSource.PlayOneShot(audioClips[2]);
        if (currentRecovery > 0)
        {
            audioSource.PlayOneShot(audioClips[5]);
        }
        publisher.RaiseEvent(currentDamage, currentTarget);
    }

    IEnumerator MonsterEffect()
    {
        //이펙트 관리자가 필요할까?
        enemyDiceRoll = false;
        monsterSignal = 0;
        for (int i = 0; i < StageMgr.Instance.enemies.Count; i++)
        {
            StageMgr.Instance.enemies[i].animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void ScrollsClear()
    {
        /*
        for (int i = 0; i < 3; i++)
        {
            scrolls[i].GetComponentInChildren<TextMeshProUGUI>().text = " ";
        }
        damageInfo.text = " ";
        */ // 마법서 스크롤 띄우던거
        for (int i = 0; i < ranksInfo.Length; i++)
        {
            ranksInfo[i].color = Color.gray;
        }
        damageInfo.text = "공격력";
        targetInfo.text = "타겟";
        currentMagicInfo.interactable = false;
        ui.magicInfoToggle.interactable = false;

        for (int i = 0; i < 5; i++)
        {
            ui.damages[i].text = " ";
        }
        for (int i = 0; i < 9; i++)
        {
            ui.infoMagics[i].gameObject.SetActive(false);
        }
        scrollsound = 0;
    }

    private void MonsterCheck()
    {
        monsterSignal = 0;
        if (bossSignal == 1)
        {
            List<Enemy> indicesToRemove = new List<Enemy>();
            foreach (var e in StageMgr.Instance.enemies)
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
        if (StageMgr.Instance.enemies.Count == 0)
        {
            onMonsterAttack = false;
            foreach (var deadEnemy in StageMgr.Instance.DeadEnemies)
            {
                Destroy(deadEnemy.gameObject);
            }
            StageMgr.Instance.DeadEnemies.Clear();

            PlayerBarrier = 0;
            LifeUpdate();

            if (StageMgr.Instance.currentStage == StageMgr.Instance.latStage && StageMgr.Instance.currentField == StageMgr.Instance.lastField)
            {
                BGM.Instance.PlayBGM(BGM.Instance.bgm[1], 2);
                SaveLoadSystem.DeleteSaveData();
                ui.Victory();
                return;
            }
            currentStatus = TurnStatus.GetRewards;
            if (StageMgr.Instance.TutorialStage == StageMgr.Instance.currentStage)
            {
                PlayerPrefs.SetInt("Tutorial", 1);
                ui.GetDice();
                return;
            }
            if (StageMgr.Instance.currentStage == StageMgr.Instance.latStage)
            {
                ui.OnArtifactReward();
                return;
            }
            ui.OnReward();
            return;
        }
        else
        {
            currentStatus = TurnStatus.MonsterDice;
        }
    }

    public void PlayerOndamage(int enemyDamage)
    {
        if (artifact.playersArtifacts[5] == 1)
        {
            dodgeNuber = UnityEngine.Random.Range(0, 100);
            if (dodgeNuber < artifact.valueData.Value5)
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
                //playerHitParticleL.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                playerHitParticleL.Play();
            }
        }
        LifeUpdate();
    }

    public void Heal(int recovery, int barrier)
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

    public void UseArtifact8()
    {
        artifact.playersArtifacts[7] = 2;
        foreach (var a in ui.artifactInfo)
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

        Instance.tutorialMode = false;
        tutorial.tutorialPanel.gameObject.SetActive(false);
        tutorial.skipButton.gameObject.SetActive(false);

        StageMgr.Instance.currentField = SaveLoadSystem.CurrSaveData.savePlay.Stage / 10;
        StageMgr.Instance.currentStage = SaveLoadSystem.CurrSaveData.savePlay.Stage % 10;
        currentDiceCount = (DiceCount)SaveLoadSystem.CurrSaveData.savePlay.DiceCount;
        curruntBonusStat = SaveLoadSystem.CurrSaveData.savePlay.Damage;
        PlayerHp = SaveLoadSystem.CurrSaveData.savePlay.Hp;
        PlayerHpMax = SaveLoadSystem.CurrSaveData.savePlay.MaxHp;
        RankList = SaveLoadSystem.CurrSaveData.savePlay.RankList;

        LifeUpdate();
        RanksListUpdate();

        ui.rewardList.Clear();
        for (int i = 0; i < 9; i++)
        {
            if (SaveLoadSystem.CurrSaveData.savePlay.RankRewardList[i] == 0)
            {
                continue;
            }
            else
            {
                ui.rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(SaveLoadSystem.CurrSaveData.savePlay.RankRewardList[i]));
            }
        }

        for (int i = 0; i < 3; i++)
        {
            artifact.playersArtifactsNumber[i] = SaveLoadSystem.CurrSaveData.savePlay.ArtifactList[i];
        }

        for (int i = 0; i < 10; i++)
        {
            artifact.playersArtifacts[i] = SaveLoadSystem.CurrSaveData.savePlay.ArtifactLevelList[i];
        }



        for (int i = 0; i < RankList.Length; i++)
        {
            if (RankList[i] == 3)
            {
                ui.maxSpells[i].gameObject.SetActive(true);
            }
        } // 초월마법등록

        foreach (var artifact in GameMgr.Instance.artifact.artifacts)
        {
            if (artifact.ID == 0)
            {
                artifact.Set(0, "방화광", $"매턴 모든 적에게 '기본 공격력'(<color=purple>{GameMgr.Instance.curruntBonusStat}</color>) 만큼의 데미지");
            }
        } //유물상점 업데이트

        if (artifact.playersArtifactsNumber[0] != -1)
        {
            foreach (var a in artifact.artifacts)
            {
                if (a.ID == artifact.playersArtifactsNumber[0])
                {
                    artifact.artifacts.Remove(a);
                    ui.ArtifactUpdate(a, 0);
                    LoadArtifactCheck(a.ID);
                    break;
                }
            }

            if (artifact.playersArtifactsNumber[1] != -1)
            {
                foreach (var a in artifact.artifacts)
                {
                    if (a.ID == artifact.playersArtifactsNumber[1])
                    {
                        artifact.artifacts.Remove(a);
                        ui.ArtifactUpdate(a, 1);
                        LoadArtifactCheck(a.ID);
                        break;
                    }
                }
                if (artifact.playersArtifactsNumber[2] != -1)
                {
                    foreach (var a in artifact.artifacts)
                    {
                        if (a.ID == artifact.playersArtifactsNumber[2])
                        {
                            artifact.artifacts.Remove(a);
                            ui.ArtifactUpdate(a, 2);
                            LoadArtifactCheck(a.ID);
                            break;
                        }
                    }
                }
            }
        }

        foreach (var a in artifact.playersArtifacts)
        {
            if (a == 2)
            {
                UseArtifact8();
            }
        }

        StageMgr.Instance.NextStage(true);
        return true;
    }

    private void LoadArtifactCheck(int id)
    {
        if (id == 1)
        {
            DiceMgr.Instance.Artifact2();
        }
        else if (id == 2)
        {
            DiceMgr.Instance.manipulList[0] = 1;
            DiceMgr.Instance.manipulList[1] = 2;
            DiceMgr.Instance.manipulList[2] = 3;
        }
    }


    private void pauseGame()
    {
        Time.timeScale = 0;
        ui.PausePanel.gameObject.SetActive(true);
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
