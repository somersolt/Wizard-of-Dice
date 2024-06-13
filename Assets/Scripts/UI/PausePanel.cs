using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : Panel
{
    [SerializeField]
    private Button ReturnButton;
    [SerializeField]
    private Button QuitGame;

    public override void Init()
    {
        base.Init();
        ReturnButton.onClick.AddListener(() => { ClosePanel(); Time.timeScale = 1; });
        QuitGame.onClick.AddListener(() => { mediator.ui.titlePanel.OpenPanel(); });
    }
}
