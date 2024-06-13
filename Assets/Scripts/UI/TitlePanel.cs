using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitlePanel : Panel
{
    [SerializeField]
    private Button QuitYes;
    [SerializeField]
    private Button QuitNo;

    public override void Init()
    {
        base.Init();
        QuitYes.onClick.AddListener(() => { Time.timeScale = 1; SceneManager.LoadScene("Title"); });
        QuitNo.onClick.AddListener(() => { ClosePanel(); });
    }
}
