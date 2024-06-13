using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventTextPanel : Panel
{
    [SerializeField]
    private Button eventTextPanelButton;

    [SerializeField]
    private Image eventFace;
    [SerializeField]
    private TextMeshProUGUI eventText;
    [SerializeField]
    private TextMeshProUGUI eventName;

    public override void Init()
    {
        base.Init();
        eventTextPanelButton.onClick.AddListener(() => { ClosePanel(); mediator.ui.diceRewardPanel.OnDiceReward();});
    }

    public void EventStage()
    {
        switch (mediator.stageMgr.currentField)
        {
            case 1:
                eventFace.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "merchant"));
                break;
            case 2:
                eventFace.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "devil"));
                break;
            case 3:
                eventFace.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "stone"));
                break;
            case 4:
                eventFace.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "merchant"));
                break;

        }

        eventName.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(40000 + mediator.stageMgr.currentField);
        eventText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(50000 + mediator.stageMgr.currentField);
        mediator.ui.diceRewardPanel.rewardText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(60000 + mediator.stageMgr.currentField);

        OpenPanel();
    }

}
