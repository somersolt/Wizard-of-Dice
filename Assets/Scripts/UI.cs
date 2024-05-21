using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public List<SpellData> rewardList = new List<SpellData>();

    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private Button titleButton;

    [SerializeField]
    private GameObject victoryPanel;
    [SerializeField]
    private Button titleButton2;

    [SerializeField]
    private GameObject rewardPanel;
    [SerializeField]
    private Button[] rewards = new Button[3];
    private TextMeshProUGUI[] spellNames = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] spellInfos = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] spellLevels = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] newTexts = new TextMeshProUGUI[2];

    private SpellData[] rewardSpells = new SpellData[2];
    private SpellData empty = new SpellData();

    [SerializeField]
    private GameObject diceRewardPanel;
    [SerializeField]
    private Button diceRewardConfirm;

    public GameObject PausePanel;
    [SerializeField]
    private Button ReturnButton;
    [SerializeField]
    private Button QuitGame;
    [SerializeField]
    private GameObject QuitPanel;
    [SerializeField]
    private Button QuitYes;
    [SerializeField]
    private Button QuitNo;

    private void Awake()
    {
        titleButton.onClick.AddListener(() => ReturnTitle());
        titleButton2.onClick.AddListener(() => ReturnTitle());
        diceRewardConfirm.onClick.AddListener(() =>
        {
            diceRewardPanel.gameObject.SetActive(false);
            GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
            StageMgr.Instance.NextStage();
        });
        ReturnButton.onClick.AddListener(() => { PausePanel.gameObject.SetActive(false); Time.timeScale = 1; });
        QuitGame.onClick.AddListener(() => { QuitPanel.gameObject.SetActive(true); });
        QuitYes.onClick.AddListener(() => { Time.timeScale = 1; SceneManager.LoadScene("Title"); });
        QuitNo.onClick.AddListener(() => { QuitPanel.gameObject.SetActive(false); });

        for (int i = 0; i < rewards.Length - 1; i++)
        {
            spellNames[i] = rewards[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            spellInfos[i] = rewards[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            spellLevels[i] = rewards[i].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
            newTexts[i] = rewards[i].transform.Find("new").GetComponentInChildren<TextMeshProUGUI>();
        } // ���� ���� 1~2ĭ

        spellNames[2] = rewards[2].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
        spellInfos[2] = rewards[2].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        spellLevels[2] = rewards[2].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
        // �������� 3��° ĭ

    }

    public void OnReward()
    {
        RewardClear();
        for (int i = 0; i < 2; i++)
        {
            int index = i;
            rewards[i].onClick.AddListener(() => { GetSpell(rewardSpells[index], index); });
            int a = Random.Range(0, rewardList.Count);
            if (rewardList.Count != 0)
            {
                rewardSpells[i] = rewardList[a];
                spellNames[i].text = rewardList[a].GetName;
                spellInfos[i].text = rewardList[a].GetDesc;
                switch (rewardList[a].LEVEL)
                {
                    case 1:
                        spellLevels[i].text = "�Ϲ�";
                        if (i == 0)
                        { newTexts[0].gameObject.SetActive(true); }
                        if (i == 1)
                        { newTexts[1].gameObject.SetActive(true); }
                        break;
                    case 2:
                        spellLevels[i].text = "��ȭ";
                        break;
                }
                rewardList.Remove(rewardList[a]);
            }
            else if (rewardList.Count == 0)
            {
                rewardSpells[i] = empty;
                spellNames[i].text = "�� ��";
                spellInfos[i].text = "SOLD OUT!!";
                spellLevels[i].text = " ";
            }
        }

        spellNames[2].text = "���ݷ� up";
        spellInfos[2].text = "�ֻ��� ���� ���� + 3";
        spellLevels[2].text = " ";
        rewards[2].onClick.AddListener(() => GetStatus(3));

        rewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(rewardPanel));
    }


    public void OnDiceReward()
    {
        RewardClear();

        switch (GameMgr.Instance.currentDiceCount)
        {
            case GameMgr.DiceCount.three:
                rewards[0].onClick.AddListener(() =>
                {
                    GameMgr.Instance.currentDiceCount = GameMgr.DiceCount.four;
                    GameMgr.Instance.GetDice4Ranks();
                    rewardPanel.gameObject.SetActive(false);
                    GetDice();
                });

                spellNames[0].text = "�ֻ��� ���� �߰�";
                spellInfos[0].text = "�� �� ���� �� �ִ� �ֻ����� 4���� ���� \n ������ 4�ֻ��� �������� �����մϴ�.";
                spellLevels[0].text = " ";
                break;
            case GameMgr.DiceCount.four:
                rewards[0].onClick.AddListener(() =>
                {
                    GameMgr.Instance.currentDiceCount = GameMgr.DiceCount.five;
                    GameMgr.Instance.GetDice5Ranks();
                    rewardPanel.gameObject.SetActive(false);
                    GetDice();
                });

                spellNames[0].text = "�ֻ��� ���� �߰�";
                spellInfos[0].text = "�� �� ���� �� �ִ� �ֻ����� 5���� ���� \n ������ 5�ֻ��� �������� �����մϴ�.";
                spellLevels[0].text = " ";
                break;
            case GameMgr.DiceCount.five:

                spellNames[0].text = "�Ѱ� ����";
                spellInfos[0].text = "�� �̻� �ֻ����� �ø� �� �����ϴ�.";
                spellLevels[0].text = " ";
                break;
        }

        rewards[1].onClick.AddListener(() =>
        {
            GetStatus(20, "DiceReward");
        });

        spellNames[1].text = "�⺻�� ����";
        spellInfos[1].text = "�ֻ��� ������ �ø��� �ʰ� ������ 20 ���� \n �ֻ��� ���� ���տ� 20�� ���մϴ�.";
        spellLevels[1].text = " + 20 ";


        rewards[1].onClick.AddListener(() =>
        {
            //TO-DO ������ �ʿ�
        });

        spellNames[2].text = "�ʿ� ����";
        spellInfos[2].text = "������ ���� �� �ϳ��� �ʿ� ������� �����մϴ�. \n �ʿ� ������ ���� ���°� Ư�� ȿ���� �߰��˴ϴ�.";
        spellLevels[2].text = " ";

        rewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(rewardPanel));
    }


    public void GetDice()
    {
        diceRewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(diceRewardPanel));
    }

    public void GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
    }

    public void Victory()
    {
        victoryPanel.gameObject.SetActive(true);
    }
    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void GetSpell(SpellData spellData, int index)
    {
        if (spellData == empty) { return; }
        for (int i = 0; i < rewardSpells.Length; i++)
        {
            if (i == index)
            {
                if (spellData.LEVEL != 2 && spellData.LEVEL != 0)
                {
                    rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(spellData.ID + 1));
                    //if (spellData.LEVEL == 1)
                    //{
                    //    NextRanks(spellData); // ��ųƮ�� ���, ���
                    //}
                }
                continue;
            }
            if (rewardSpells[i] != empty)
            {
                rewardList.Add(rewardSpells[i]);
            }
        }//�������� 

        if (rewardSpells[index].LEVEL != 0)
        {
            GameMgr.Instance.SetRankList((spellData.ID % 100) / 10 - 1);
        }

        GameMgr.Instance.RanksListUpdate();
        GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
        rewardPanel.gameObject.SetActive(false);
        StageMgr.Instance.NextStage();
    }

    public void GetStatus(int value, string mode = default)
    {
        if (mode == default)
        {
            for (int i = 0; i < 2; i++)
            {
                if (rewardSpells[i] != empty)
                {
                    rewardList.Add(rewardSpells[i]);
                }
            }
        }

        GameMgr.Instance.curruntBonusStat += value;
        GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
        rewardPanel.gameObject.SetActive(false);
        StageMgr.Instance.NextStage();
    }


    private IEnumerator PanelSlide(GameObject panel)
    {
        float duration = 0.3f;
        float time = 0f;
        Vector3 startpos = panel.transform.position;

        panel.transform.position = new Vector3(1000, startpos.y, startpos.z);

        while (time < duration)
        {
            panel.transform.position = Vector3.Lerp(panel.transform.position, startpos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        panel.transform.position = startpos;
    }

    private void RewardClear()
    {
        rewards[0].onClick.RemoveAllListeners();
        rewards[1].onClick.RemoveAllListeners();
        rewards[2].onClick.RemoveAllListeners();
        newTexts[0].gameObject.SetActive(false);
        newTexts[1].gameObject.SetActive(false);
    }
    private void NextRanks(SpellData spellData)
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
