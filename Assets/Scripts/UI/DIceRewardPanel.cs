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
        } // 상점 보상
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

                diceRewardNames[0].text = "주사위 개수 추가";
                diceRewardInfos[0].text = "매 턴 굴릴 수 있는 주사위를 4개로 증가 \n 상점에 4주사위 마법서가 등장합니다.";
                break;
            case GameMgr.DiceCount.four:
                diceRewards[0].onClick.AddListener(() =>
                {
                    mediator.gameMgr.currentDiceCount = GameMgr.DiceCount.five;
                    mediator.gameMgr.GetDice5Ranks();
                    gameObject.SetActive(false);
                    mediator.ui.getDicePanel.GetDice();
                });

                diceRewardNames[0].text = "주사위 개수 추가";
                diceRewardInfos[0].text = "매 턴 굴릴 수 있는 주사위를 5개로 증가 \n 상점에 5주사위 마법서가 등장합니다.";
                break;
            case GameMgr.DiceCount.five:

                diceRewardNames[0].text = "한계 도달";
                diceRewardInfos[0].text = "더 이상 주사위를 늘릴 수 없습니다.";
                break;
        }

        diceRewards[1].onClick.AddListener(() =>
        {
            mediator.ui.rewardPanel.GetStatus(mediator.gameMgr.artifact.valueData.Stat2, RewardMode.Event);
        });

        diceRewardNames[1].text = "마나 증량";
        diceRewardInfos[1].text = $"주사위 개수를 늘리지 않고 공격력 <color=red>{mediator.gameMgr.artifact.valueData.Stat2}</color> 증가 \n 주사위 눈금 총합에 <color=red>{mediator.gameMgr.artifact.valueData.Stat2}</color>을 더합니다.";

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

                diceRewardNames[2].text = "초월 마법";
                diceRewardInfos[2].text = "보유한 마법 중 하나를 초월 등급으로 변경합니다. \n 초월 마법은 강한 위력과 특수 효과가 추가됩니다.";
                break;
            }
            else
            {
                diceRewardNames[2].text = "초월 마법";
                diceRewardInfos[2].text = "보유한 마법 중 하나를 초월 등급으로 변경합니다. \n <color=red>숙련된 마법이 없어 초월할 수 없습니다.";
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
