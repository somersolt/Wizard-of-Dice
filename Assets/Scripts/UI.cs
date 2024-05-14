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
    private GameObject rewardPanel;
    [SerializeField]
    private Button[] rewards = new Button[3];
    private TextMeshProUGUI[] spellNames = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] spellInfos = new TextMeshProUGUI[3];

    private SpellData[] rewardSpells = new SpellData[3];

    private void Awake()
    {
        titleButton.onClick.AddListener(() => ReturnTitle());
        for(int i = 0; i < rewards.Length; i++)
        {
            spellNames[i] = rewards[i].transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            spellInfos[i] = rewards[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            int index = i;
            rewards[i].onClick.AddListener(() => { GetSpell(rewardSpells[index], index); });
        }
    }

    public void OnReward()
    {
        for (int i = 0; i < rewardSpells.Length; i++)
        {
            int a = Random.Range(0, rewardList.Count);
            rewardSpells[i] = rewardList[a];
            spellNames[i].text = rewardList[a].GetName;
            spellInfos[i].text = rewardList[a].GetDesc;
            rewardList.Remove(rewardList[a]);
        }
        rewardPanel.gameObject.SetActive(true);

    }
    public void GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void GetSpell(SpellData spellData, int index)
    {
        for (int i = 0; i < rewardSpells.Length; i++)
        {
            if(i == index)
            {
                if (rewardSpells[i].LEVEL != 3 && rewardSpells[i].LEVEL != 0)
                {
                    rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(rewardSpells[i].ID + 1));
                }
                continue;
            }
            rewardList.Add(rewardSpells[i]);
        }//돌려놓기 

        if (rewardSpells[index].LEVEL != 0)
        {
            GameMgr.Instance.SetRankList((rewardSpells[index].ID % 100) / 10);
        }


        // TO-DO 족보가 아니면 상시 마법서 획득 메소드 만들기
        rewardPanel.gameObject.SetActive(false) ;
        StageMgr.Instance.NextStage();
    }
}
