using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : Panel
{
    [SerializeField]
    private Button eventPanelButton;

    public override void Init()
    {
        base.Init();
        eventPanelButton.onClick.AddListener(() => { ClosePanel(); mediator.ui.evenetTextPanel.EventStage(); });
    }

}
