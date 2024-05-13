using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

    }    // �̱��� ����

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

    /// </summary>
    /// �ֻ��� , ������ ���� �ʵ�

    public enum DiceCount
    {
        three = 3,
        four = 4,
        five = 5,
    }

    public DiceCount currentDiceCount;
    int[] RankList = new int[(int)Ranks.count]; // �÷��̾��� ���� ����Ʈ
    private RanksFlag currentRanks; //���� �ֻ����� ���� ����
    private int currentValue;
    [SerializeField]
    private TextMeshProUGUI damageInfo;
    private int currentDamage;
    public int currentTarget;
    [SerializeField]
    private Button[] scrolls = new Button[3];
    [SerializeField]
    private Button totalScrolls;

    ///  
    /// </summary>



    private void Awake()
    {
        for (int i = 0; i < (int)Ranks.TwoPair; i++)
        {
            RankList[i] = 1; // �����, Ʈ����, 3��Ʈ����Ʈ 1���� ���
        }
        currentDiceCount = DiceCount.three;

        // �̺�Ʈ�� �̺�Ʈ �ڵ鷯 �޼��带 �߰�
        publisher.AttackEvent += listener.AttackHandleEvent;

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
            Debug.Log("three");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GetDice4Ranks();
            DiceMgr.Instance.DiceFour();
            DiceMgr.Instance.DiceRoll(true);

            Debug.Log("four");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GetDice4Ranks();
            GetDice5Ranks();
            DiceMgr.Instance.DiceFive();
            DiceMgr.Instance.DiceRoll(true);

            Debug.Log("five");
        }

        // �׽�Ʈ �ڵ�


    }
    private void PlayerAttackUpdate()
    {

    }
    private void GetRewardsUpdate()
    {

    }
    private void MonsterDiceUpdate()
    {

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
        return copy; // �迭 Ȯ�ο� ���纻 ��ȯ
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
            //���� ��ȹ��
        }
        RankList[i] += 1;
    }

    public void GetDice4Ranks()
    {
        for (int i = (int)Ranks.TwoPair; i < (int)Ranks.FullHouse; i++)
        {
            RankList[i] = 1; // �����, ī�ε�4, 4��Ʈ����Ʈ 1���� ���
        }
        currentDiceCount = DiceCount.four;
    }
    public void GetDice5Ranks()
    {
        for (int i = (int)Ranks.FullHouse; i < (int)Ranks.count; i++)
        {
            RankList[i] = 1; // Ǯ�Ͽ콺 5��Ʈ����Ʈ, ī�ε�5 1���� ���
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
            scrolls[i].GetComponentInChildren<TextMeshProUGUI>().text = "�ֹ��� ����";
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

    public void AttackButtonClicked()
    {
        publisher.RaiseEvent(currentDamage, currentTarget);  // �̺�Ʈ �߻�
    }

}
