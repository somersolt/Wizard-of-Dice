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

    private SpellData[] rewardSpells = new SpellData[3];

    private SpellData empty = new SpellData();
    private void Awake()
    {
        titleButton.onClick.AddListener(() => ReturnTitle());
        titleButton2.onClick.AddListener(() => ReturnTitle());
        for(int i = 0; i < rewards.Length; i++)
        {
            spellNames[i] = rewards[i].transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            spellInfos[i] = rewards[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            spellLevels[i] = rewards[i].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
            int index = i;
            rewards[i].onClick.AddListener(() => { GetSpell(rewardSpells[index], index); });
        }
    }

    public void OnReward()
    {
        for (int i = 0; i < 3; i++)
        {
            int a = Random.Range(0, rewardList.Count);
            if (rewardList.Count != 0)
            {
                rewardSpells[i] = rewardList[a];
                spellNames[i].text = rewardList[a].GetName;
                spellInfos[i].text = rewardList[a].GetDesc;
                spellLevels[i].text = rewardList[a].LEVEL.ToString() + " 레벨";
                rewardList.Remove(rewardList[a]);
            }
            else if (rewardList.Count == 0)
            {
                rewardSpells[i] = empty;
                spellNames[i].text = "매 진";
                spellInfos[i].text = "SOLD OUT!!";
                spellLevels[i].text = " ";
            }
        }
        rewardPanel.gameObject.SetActive(true);

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
        if( spellData == empty) { return; }
        for (int i = 0; i < rewardSpells.Length; i++)
        {
            if(i == index)
            {
                if (spellData.LEVEL != 3 && spellData.LEVEL != 0)
                {
                    rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(spellData.ID + 1));
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
            GameMgr.Instance.SetRankList((spellData.ID % 100) / 10 - 1);
        }

        GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
        // TO-DO 족보가 아니면 상시 마법서 획득 메소드 만들기
        rewardPanel.gameObject.SetActive(false) ;
        StageMgr.Instance.NextStage();
    }
}
