using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MagicInfoPanel : Panel
{
    public Toggle magicInfoToggle;
    public TextMeshProUGUI magicInfoToggleText;

    [SerializeField]
    private Button DamageInfoButton;
    public Image[] infoMagics = new Image[9];
    private TextMeshProUGUI[] infoMagicnames = new TextMeshProUGUI[9];
    private TextMeshProUGUI[] infoMagicInfos = new TextMeshProUGUI[9];
    private TextMeshProUGUI[] infoMagicLevels = new TextMeshProUGUI[9];
    private Image[] infoMagicexamples = new Image[9];
    public Image[] infoMagicstars = new Image[9];


    [SerializeField]
    private Button magicExitButton;

    public override void Init()
    {
        base.Init();

        magicExitButton.onClick.AddListener(() => { ClosePanel(); });
        DamageInfoButton.onClick.AddListener(() => { ClosePanel(); mediator.ui.damageInfoPanel.OpenPanel(); });

        for (int i = 0; i < 9; i++)
        {
            infoMagicnames[i] = infoMagics[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            infoMagicInfos[i] = infoMagics[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            infoMagicLevels[i] = infoMagics[i].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
            infoMagicexamples[i] = infoMagics[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("ex").GetComponentInChildren<Image>();
            var path = (i + 1).ToString();
            infoMagicexamples[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", path)); ;
            infoMagicstars[i] = infoMagics[i].transform.Find("star").GetComponentInChildren<Image>();
        } //적용 마법



        magicInfoToggle.onValueChanged.AddListener((isOn) => { SetToggleMagicInfo(isOn); });
    }

    public string MagicInfomationUpdate(int i, SpellData spell , int rankList)
    {
        infoMagicnames[i].text = spell.GetName;
        infoMagicInfos[i].text = spell.GetDesc;
        StringBuilder newText = new StringBuilder();
        newText.Append(spell.GetName);

        switch (rankList)
        {
            case 1:
                newText.Append("- 일반");
                infoMagicLevels[i].text = "일반";
                infoMagicLevels[i].color = Color.white;
                infoMagicstars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_1"));
                break;
            case 2:
                newText.Append("- 강화");
                infoMagicLevels[i].text = "강화";
                infoMagicLevels[i].color = Color.green;
                infoMagicstars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_2"));
                break;
            case 3:
                newText.Append("- 숙련");
                infoMagicLevels[i].text = "숙련";
                infoMagicLevels[i].color = Color.yellow;
                infoMagicstars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_3"));
                break;
            case 4:
                newText.Append("- 초월");
                infoMagicLevels[i].text = "초월";
                infoMagicLevels[i].color = Color.red;
                infoMagicstars[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Level_4"));
                break;
        }

        return newText.ToString();
    }

    public void SetToggleMagicInfo(bool isOn)
    {
        foreach (var magic in infoMagics)
        {
            magic.gameObject.SetActive(false);
        }

        magicInfoToggleText.gameObject.SetActive(isOn);
        DamageInfoButton.gameObject.SetActive(!isOn);

        if (magicInfoToggle.isOn)
        {
            for (int i = 0; i < infoMagics.Length; i++)
            {
                if (mediator.gameMgr.GetRank(i) > 0)
                {
                    infoMagics[i].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < infoMagics.Length; i++)
            {
                RanksFlag currentFlag = (RanksFlag)(1 << i);
                if ((mediator.diceMgr.CheckedRanksList & currentFlag) != 0)
                {
                    infoMagics[i].gameObject.SetActive(true);
                }
            }
        }

    }

}
