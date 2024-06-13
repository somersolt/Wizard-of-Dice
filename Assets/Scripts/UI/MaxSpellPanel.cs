using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaxSpellPanel: Panel
{
    public Button[] maxSpells = new Button[9];
    private TextMeshProUGUI[] maxSpellNames = new TextMeshProUGUI[9];
    private TextMeshProUGUI[] maxSpellInfos = new TextMeshProUGUI[9];
    private TextMeshProUGUI[] maxSpellLevels = new TextMeshProUGUI[9];
    private Image[] maxSpellexamples = new Image[9];


    public override void Init()
    {
        base.Init();
        for (int i = 0; i < 9; i++)
        {
            int index = i;
            maxSpellNames[i] = maxSpells[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            maxSpellInfos[i] = maxSpells[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            maxSpellLevels[i] = maxSpells[i].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
            maxSpellexamples[i] = maxSpells[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("ex").GetComponentInChildren<Image>();
            var path = (i + 1).ToString();
            maxSpellexamples[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", path)); ;
            var maxSpell = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(RankIdsToInt.rankids[i] + 3);
            maxSpellNames[i].text = maxSpell.GetName;
            maxSpellInfos[i].text = maxSpell.GetDesc;
            maxSpellLevels[i].text = "초월";
            maxSpells[i].onClick.AddListener(() =>
            {
                mediator.gameMgr.SetRankList(index);
                maxSpells[index].gameObject.SetActive(false);
                SlideOpenPanel();
                RewardSound(2);
                mediator.gameMgr.RanksListUpdate();
                mediator.stageMgr.NextStage();
            });
            maxSpells[i].gameObject.SetActive(false);
        } //초월 강화
    }



}
