using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
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
        GameMgr.Instance.Heal();
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
    public Message messagePrefab;
    public GameObject messagePos;
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

    [SerializeField]
    private TextMeshProUGUI[] ranksInfo = new TextMeshProUGUI[9];

    private int currentValue;
    [SerializeField]
    private TextMeshProUGUI damageInfo;
    public int currentDamage;
    public int currentBarrier;
    public int currentRecovery;
    public int currentTarget;
    [SerializeField]
    private Button[] scrolls = new Button[3];
    [SerializeField]
    private Button totalScrolls;

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
    public int monsterSignal = 0;

    private bool onMonsterAttack = false;

    [SerializeField]
    private Button quit;


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
        quit.onClick.AddListener(() => pauseGame());
    }


    private void Start()
    {
        currentDiceCount = DiceCount.two;
        DiceMgr.Instance.DiceTwo();
    }
    private void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentDiceCount = DiceCount.three;
            DiceMgr.Instance.DiceThree();
            DiceMgr.Instance.DiceRoll(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GetDice4Ranks();
            DiceMgr.Instance.DiceFour();
            DiceMgr.Instance.DiceRoll(true);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GetDice4Ranks();
            GetDice5Ranks();
            DiceMgr.Instance.DiceFive();
            DiceMgr.Instance.DiceRoll(true);
        }
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    DiceMgr.Instance.manipulList[0] = 1;
        //    DiceMgr.Instance.manipulList[1] = 2;
        //    DiceMgr.Instance.manipulList[2] = 3;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    currentTarget = 1;
        //    damageInfo.text = currentDamage.ToString() + " / " + currentTarget.ToString();

        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    currentTarget = 2;
        //    damageInfo.text = currentDamage.ToString() + " / " + currentTarget.ToString();

        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    currentTarget = 3;
        //    damageInfo.text = currentDamage.ToString() + " / " + currentTarget.ToString();

        //}


        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentDamage = 100;
            damageInfo.text = currentDamage.ToString() + " / " + currentTarget.ToString();
        }
        // 테스트 코드


    }
    private void PlayerAttackUpdate()
    {
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
            currentStatus = TurnStatus.PlayerLose;
            return;
            //플레이어 사망 체크
        }

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

    public void SetRankList(int i)
    {
        if (RankList[i] == 3)
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
            var spells = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[i] + 1);
            ui.rewardList.Add(spells);
        }
    }
    public void GetDice5Ranks()
    {
        for (int i = (int)Ranks.FullHouse; i < (int)Ranks.count; i++)
        {
            RankList[i] = 1; // 풀하우스 5스트레이트, 카인드5 1레벨 등록
            var spells = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[i] + 1);
            ui.rewardList.Add(spells);
        }
    }

    public void SetResult(RanksFlag ranks, int value)
    {
        currentRanks = ranks;
        currentValue = value;

        currentDamage = DamageCheckSystem.DamageCheck(currentValue, RankList, currentRanks);
        damageInfo.text = currentDamage.ToString() + " / " + currentTarget.ToString();
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
            }
        }
    }

    public void RanksListUpdate()
    {
        RankCheckSystem.ranksList = RankList;
        for(int i = 0; i < ranksInfo.Length; i++)
        {
            if (RankList[i] != 0)
            {
            StringBuilder newText = new StringBuilder();
            newText.Append(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[i]).GetName);
            newText.Append("- Lv ");
            newText.Append(RankList[i]);
            ranksInfo[i].text = newText.ToString(); 
            }
        }
    }

    public void TurnUpdate(int n)
    {
        currentTurn = n;
        turnInfo.text = currentTurn.ToString() + " Turn";
        currentStatus = TurnStatus.PlayerDice;
    }

    public void PlayerAttackEffect()
    {
        livingMonster = Math.Min(StageMgr.Instance.enemies.Count, currentTarget);
        CurrentStatus = TurnStatus.PlayerAttack;
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
    }

    private void MonsterCheck()
    {
        monsterSignal = 0;
        if (StageMgr.Instance.enemies.Count == 0)
        {
            foreach (var deadEnemy in StageMgr.Instance.DeadEnemies)
            {
                Destroy(deadEnemy.gameObject);
            }
            StageMgr.Instance.DeadEnemies.Clear();

            if (StageMgr.Instance.currentStage == 15)
            {
                ui.Victory();
                return;
            }
            currentStatus = TurnStatus.GetRewards;
            if (StageMgr.Instance.TutorialStage == StageMgr.Instance.currentStage)
            {
                ui.GetDice();
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
        if (PlayerBarrier >= enemyDamage)
        {
            PlayerBarrier -= enemyDamage;

            var DamageMessage = Instantiate(messagePrefab, canvas.transform);
            DamageMessage.Setup(enemyDamage, Color.blue);
            DamageMessage.GetComponent<RectTransform>().anchoredPosition =
                messagePos.GetComponent<RectTransform>().anchoredPosition;


        }
        else
        {
            enemyDamage -= PlayerBarrier;
            PlayerBarrier = 0;
            PlayerHp -= enemyDamage;
            var DamageMessage = Instantiate(messagePrefab, canvas.transform);
            DamageMessage.Setup(enemyDamage, Color.red);
            DamageMessage.GetComponent<RectTransform>().anchoredPosition =
                 messagePos.GetComponent<RectTransform>().anchoredPosition;
        }
        LifeUpdate();
    }

    public void Heal()
    {
        PlayerHp += currentRecovery;
        if (PlayerHp > PlayerHpMax)
        {
            PlayerHp = PlayerHpMax;
        }
        PlayerBarrier += currentBarrier;
        LifeUpdate();
    }

    public void LifeMax()
    {
        PlayerHp = PlayerHpMax;
        LifeUpdate();
    }

    public void LifeUpdate()
    {
        PlayerBarrierInfo.text = PlayerBarrier.ToString();
        PlayerBarrierBarInfo.fillAmount = (float)PlayerBarrier / PlayerHpMax;
        PlayerHpInfo.text = PlayerHp.ToString();
        PlayerHpBarInfo.fillAmount = (float)PlayerHp / PlayerHpMax;
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
