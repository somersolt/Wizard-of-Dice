using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : Panel
{
    public List<SpellData> rewardList = new List<SpellData>();
    [SerializeField]
    private Button[] rewards = new Button[3];
    private TextMeshProUGUI[] spellNames = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] spellInfos = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] spellLevels = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] newTexts = new TextMeshProUGUI[2];
    private Image[] stars = new Image[2];

    private SpellData[] rewardSpells = new SpellData[2];
    private Image[] examples = new Image[2];
    private SpellData empty = new SpellData();


    public override void Init()
    {
        base.Init();
        for (int i = 0; i < rewards.Length - 1; i++)
        {
            spellNames[i] = rewards[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            examples[i] = rewards[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("ex").GetComponentInChildren<Image>();
            spellInfos[i] = rewards[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            spellLevels[i] = rewards[i].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
            newTexts[i] = rewards[i].transform.Find("new").GetComponentInChildren<TextMeshProUGUI>();
            stars[i] = rewards[i].transform.Find("star").GetComponentInChildren<Image>();
        } // 족보 보상 1~2칸

        spellNames[2] = rewards[2].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
        spellInfos[2] = rewards[2].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        spellLevels[2] = rewards[2].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
        // 족보보상 3번째 칸
    }


    public void OnReward(RewardMode mode = RewardMode.Normal, int count = 0)
    {
        RewardSound(0);
        RewardClear();
        for (int i = 0; i < 2; i++)
        {
            int index = i;
            rewards[i].onClick.AddListener(() => { GetSpell(rewardSpells[index], index, mode, count); });
            int a = Random.Range(0, rewardList.Count);
            if (rewardList.Count != 0)
            {
                rewardSpells[i] = rewardList[a];
                spellNames[i].text = rewardList[a].GetName;
                spellInfos[i].text = rewardList[a].GetDesc;
                var path = (rewardList[a].ID % 100 / 10).ToString();
                examples[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", path));
                switch (rewardList[a].LEVEL)
                {
                    case 1:
                        spellLevels[i].text = "일반";
                        spellLevels[i].color = Color.white;
                        if (i == 0)
                        { newTexts[0].gameObject.SetActive(true); }
                        if (i == 1)
                        { newTexts[1].gameObject.SetActive(true); }
                        stars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_1"));
                        break;
                    case 2:
                        spellLevels[i].text = "강화";
                        spellLevels[i].color = Color.green;
                        stars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_2"));
                        break;
                    case 3:
                        spellLevels[i].text = "숙련";
                        spellLevels[i].color = Color.yellow;
                        stars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_3"));
                        break;
                }
                rewardList.Remove(rewardList[a]);
            }
            else if (rewardList.Count == 0)
            {
                rewardSpells[i] = empty;
                spellNames[i].text = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(1110).GetName;
                spellInfos[i].text = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(1110).GetDesc;
                spellLevels[i].text = " ";
                examples[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "null_image"));
                stars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "null_image"));
            }
        }

        spellNames[2].text = "신체 강화";
        spellInfos[2].text = $"기본 공격력 + <color=red>{mediator.artifacts.valueData.Stat1}</color> \n 주사위 개수 x {mediator.artifacts.valueData.Stat3} (<color=green>{(int)mediator.gameMgr.currentDiceCount * mediator.artifacts.valueData.Stat3}</color>) 만큼 회복";
        spellLevels[2].text = " ";
        if (mode == RewardMode.Artifact && count > 0)
        {
            rewards[2].onClick.AddListener(() => GetStatus(mediator.artifacts.valueData.Stat1, RewardMode.Artifact, count));
        }
        else
        {
            rewards[2].onClick.AddListener(() => GetStatus(mediator.artifacts.valueData.Stat1));
        }
        SlideOpenPanel();
    }


    public void GetSpell(SpellData spellData, int index, RewardMode mode = RewardMode.Normal, int count = 0)
    {
        if (spellData == empty) { return; }
        for (int i = 0; i < rewardSpells.Length; i++)
        {
            if (i == index)
            {
                if (spellData.LEVEL == 1 || spellData.LEVEL == 2)
                {
                    rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(spellData.ID + 1));
                    //if (spellData.LEVEL == 1)
                    //{
                    //    NextRanks(spellData); // 스킬트리 방식, 폐기
                    //}
                }
                continue;
            }
            if (rewardSpells[i] != empty)
            {
                rewardList.Add(rewardSpells[i]);
            }
        }//돌려놓기 

        if (rewardSpells[index].LEVEL != 0)
        {
            mediator.gameMgr.SetRankList((spellData.ID % 100) / 10 - 1);
            if (rewardSpells[index].LEVEL == 3)
            {
                mediator.ui.maxSpellRewardPanel.maxSpells[(spellData.ID % 100) / 10 - 1].gameObject.SetActive(true);
            }
        }

        mediator.gameMgr.RanksListUpdate();
        mediator.gameMgr.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
        ClosePanel();
        mediator.gameMgr.audioSource.PlayOneShot(mediator.gameMgr.audioClips[8]);

        if (mode == RewardMode.Artifact && count > 0)
        {
            OnReward(RewardMode.Artifact, count - 1);
        }
        else
        {
            mediator.stageMgr.NextStage();
        }
    }

    public void GetStatus(int value, RewardMode mode = RewardMode.Normal, int count = 0)
    {
        if (mode != RewardMode.Event)
        {
            for (int i = 0; i < 2; i++)
            {
                if (rewardSpells[i] != empty)
                {
                    rewardList.Add(rewardSpells[i]);
                }
            }

            mediator.gameMgr.PlayerHeal((int)mediator.gameMgr.currentDiceCount * mediator.artifacts.valueData.Stat3, 0);
        }

        mediator.gameMgr.curruntBonusStat += value;
        foreach (var artifact in mediator.artifacts.artifactList)
        {
            if (artifact.ID == 0)
            {
                artifact.Set(0, "방화광", $"매턴 모든 적에게 '기본 공격력'(<color=purple>{mediator.gameMgr.curruntBonusStat}</color>) 만큼의 데미지");
            }
        }
        mediator.gameMgr.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
        mediator.ui.backGroundPanel.gameObject.SetActive(false);

        if (mode != RewardMode.Event)
        {
            gameObject.SetActive(false);
        }
        else
        {
            mediator.ui.diceRewardPanel.gameObject.SetActive(false);
        }
        mediator.gameMgr.audioSource.PlayOneShot(mediator.gameMgr.audioClips[8]);
        if (mode == RewardMode.Artifact)
        {
            OnReward(RewardMode.Artifact, count - 1);
            return;
        }
        mediator.stageMgr.NextStage();
    }

    private void RewardClear()
    {
        rewards[0].onClick.RemoveAllListeners();
        rewards[1].onClick.RemoveAllListeners();
        rewards[2].onClick.RemoveAllListeners();
        newTexts[0].gameObject.SetActive(false);
        newTexts[1].gameObject.SetActive(false);
    }


    private void NextRanks(SpellData spellData) // 성장형 족보 습득, 변경되어서 안쓰는 코드
    {
        if (spellData.ID == (int)RanksIds.OnePair)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.Straight3));
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.Triple));
        }
        if (spellData.ID == (int)RanksIds.Straight3)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.Straight4));
        }
        if (spellData.ID == (int)RanksIds.Straight4)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.Straight5));
        }
        if (spellData.ID == (int)RanksIds.Triple)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.TwoPair));
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.KindOf4));
        }
        if (spellData.ID == (int)RanksIds.TwoPair)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.FullHouse));
        }
        if (spellData.ID == (int)RanksIds.KindOf4)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.KindOf5));
        }
    }
}
