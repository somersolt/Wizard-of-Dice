using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class DIceRewardPanel : Panel
{
    [SerializeField]
    private Button[] diceRewards = new Button[3];
    private TextMeshProUGUI[] diceRewardNames = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] diceRewardInfos = new TextMeshProUGUI[3];
    public TextMeshProUGUI rewardText;

    public override void Init()
    {
        base.Init();
        for (int i = 0; i < diceRewards.Length; i++)
        {
            diceRewardNames[i] = diceRewards[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            diceRewardInfos[i] = diceRewards[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        } // ���� ����
    }


    public void OnDiceReward()
    {
        RewardSound(0);
        DiceRewardClear();

        switch (mediator.gameMgr.currentDiceCount)
        {
            case GameMgr.DiceCount.three:
                diceRewards[0].onClick.AddListener(() =>
                {
                    mediator.gameMgr.currentDiceCount = GameMgr.DiceCount.four;
                    mediator.gameMgr.GetDice4Ranks();
                    gameObject.SetActive(false);
                    mediator.ui.getDicePanel.GetDice();
                });

                diceRewardNames[0].text = "�ֻ��� ���� �߰�";
                diceRewardInfos[0].text = "�� �� ���� �� �ִ� �ֻ����� 4���� ���� \n ������ 4�ֻ��� �������� �����մϴ�.";
                break;
            case GameMgr.DiceCount.four:
                diceRewards[0].onClick.AddListener(() =>
                {
                    mediator.gameMgr.currentDiceCount = GameMgr.DiceCount.five;
                    mediator.gameMgr.GetDice5Ranks();
                    gameObject.SetActive(false);
                    mediator.ui.getDicePanel.GetDice();
                });

                diceRewardNames[0].text = "�ֻ��� ���� �߰�";
                diceRewardInfos[0].text = "�� �� ���� �� �ִ� �ֻ����� 5���� ���� \n ������ 5�ֻ��� �������� �����մϴ�.";
                break;
            case GameMgr.DiceCount.five:

                diceRewardNames[0].text = "�Ѱ� ����";
                diceRewardInfos[0].text = "�� �̻� �ֻ����� �ø� �� �����ϴ�.";
                break;
        }

        diceRewards[1].onClick.AddListener(() =>
        {
            mediator.ui.rewardPanel.GetStatus(mediator.gameMgr.artifact.valueData.Stat2, RewardMode.Event);
        });

        diceRewardNames[1].text = "���� ����";
        diceRewardInfos[1].text = $"�ֻ��� ������ �ø��� �ʰ� ���ݷ� <color=red>{mediator.gameMgr.artifact.valueData.Stat2}</color> ���� \n �ֻ��� ���� ���տ� <color=red>{mediator.gameMgr.artifact.valueData.Stat2}</color>�� ���մϴ�.";

        foreach (var ranks in mediator.gameMgr.GetRankList())
        {
            if (ranks == 3)
            {
                diceRewards[2].onClick.AddListener(() =>
                {
                    gameObject.SetActive(false);
                    mediator.ui.maxSpellRewardPanel.gameObject.SetActive(true);
                    //StartCoroutine(PanelSlide(maxSpellRewardPanel));
                });

                diceRewardNames[2].text = "�ʿ� ����";
                diceRewardInfos[2].text = "������ ���� �� �ϳ��� �ʿ� ������� �����մϴ�. \n �ʿ� ������ ���� ���°� Ư�� ȿ���� �߰��˴ϴ�.";
                break;
            }
            else
            {
                diceRewardNames[2].text = "�ʿ� ����";
                diceRewardInfos[2].text = "������ ���� �� �ϳ��� �ʿ� ������� �����մϴ�. \n <color=red>���õ� ������ ���� �ʿ��� �� �����ϴ�.";
            }
        }

        SlideOpenPanel();
    }

    private void DiceRewardClear()
    {
        diceRewards[0].onClick.RemoveAllListeners();
        diceRewards[1].onClick.RemoveAllListeners();
        diceRewards[2].onClick.RemoveAllListeners();
    }
}
