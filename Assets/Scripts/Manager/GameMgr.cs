using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
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
        for (int i = 0; i < StageMgr.Instance.enemies.Count; i++)
        {
            StageMgr.Instance.enemies[i].OnDamage(e.value);
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
        get; set;
    }

    AttackEventPublisher publisher = new AttackEventPublisher();
    AttackEventListener listener = new AttackEventListener();

    private int currentTurn = 1;
    [SerializeField]
    private TextMeshProUGUI turnInfo;
    private bool enemyDiceRoll;
    /// </summary>
    /// 주사위 , 마법서 관련 필드

    public enum DiceCount
    {
        three = 3,
        four = 4,
        five = 5,
    }

    public DiceCount currentDiceCount;
    int[] RankList = new int[(int)Ranks.count]; // 플레이어의 족보 리스트
    private RanksFlag currentRanks; //현재 주사위로 나온 족보
    private int currentValue;
    [SerializeField]
    private TextMeshProUGUI damageInfo;
    private int currentDamage;
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

    private int PlayerHp;
    private int PlayerHpMax = 100;
    ///  
    /// </summary>



    private void Awake()
    {
        PlayerHp = PlayerHpMax;
        for (int i = 0; i < (int)Ranks.TwoPair; i++)
        {
            RankList[i] = 1; // 원페어, 트리플, 3스트레이트 1레벨 등록
        }
        TurnUpdate(1);
        publisher.AttackEvent += listener.AttackHandleEvent;// 이벤트에 이벤트 핸들러 메서드를 추가

    }

    private void Start()
    {
        currentDiceCount = DiceCount.three;
        DiceMgr.Instance.DiceThree();
        DiceMgr.Instance.DiceRoll(true);
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

        // 테스트 코드


    }
    private void PlayerAttackUpdate()
    {
        //뭔가의 애니메이션
        PlayerAttackEffect(); //TO-DO 한번만 작동해야함
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

    }
    private void PlayerLoseUpdate()
    {

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
        else if (RankList[i] == 0)
        {
            return;
            //족보 미획득
        }
        RankList[i] += 1;
    }

    public void GetDice4Ranks()
    {
        for (int i = (int)Ranks.TwoPair; i < (int)Ranks.FullHouse; i++)
        {
            RankList[i] = 1; // 투페어, 카인드4, 4스트레이트 1레벨 등록
        }
        currentDiceCount = DiceCount.four;
    }
    public void GetDice5Ranks()
    {
        for (int i = (int)Ranks.FullHouse; i < (int)Ranks.count; i++)
        {
            RankList[i] = 1; // 풀하우스 5스트레이트, 카인드5 1레벨 등록
        }
        currentDiceCount = DiceCount.five;
    }

    public void SetResult(RanksFlag ranks, int value)
    {
        currentRanks = ranks;
        currentValue = value;

        currentDamage = DamageCheckSystem.DamageCheck(currentValue, RankList, currentRanks);
        damageInfo.text = currentDamage.ToString();

        int r = 0;
        for (int i = 0; i < scrolls.Length; i++)
        {
            scrolls[i].GetComponentInChildren<TextMeshProUGUI>().text = "주문서 없음";
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
    }

    public void TurnUpdate(int n)
    {
        currentTurn = n;
        turnInfo.text = currentTurn.ToString() + " Turn";
        currentStatus = TurnStatus.PlayerDice;
    }

    public void PlayerAttackEffect()
    {
        CurrentStatus = TurnStatus.PlayerAttack;
        publisher.RaiseEvent(currentDamage, currentTarget); // 이벤트 발생 ondamage, 코루틴으로 해야되나
        //끝나면 몬스터 턴 

        //TO-DO 스테이지 mgr 리스트에서 몬스터 받아와서 다 잡았는지 체크,
        currentStatus = TurnStatus.MonsterDice;
    }

    public void MonsterEffect()
    {
        //이펙트 관리자가 필요할까?
        //TO-DO 플레이어 사망했는지 체크,
        enemyDiceRoll = false;
        PlayerHp -= enemyValue;
        PlayerHpInfo.text = PlayerHp.ToString();
        PlayerHpBarInfo.fillAmount = (float)PlayerHp / PlayerHpMax;
        currentStatus = TurnStatus.PlayerDice;
        switch (currentDiceCount) 
        {
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
