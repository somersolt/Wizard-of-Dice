using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mediator : MonoBehaviour
{
    public GameMgr gameMgr;
    public StageMgr stageMgr;
    public DiceMgr diceMgr;

    public ArtifactCollection artifacts;
    public BGM bgm;
    public UI ui;
    private void Awake()
    {
        gameMgr = FindObjectOfType<GameMgr>();
        stageMgr = FindObjectOfType<StageMgr>();
        diceMgr = FindObjectOfType<DiceMgr>();
        bgm = FindObjectOfType<BGM>();
        ui = FindObjectOfType<UI>();
        artifacts = FindObjectOfType<ArtifactCollection>();
        artifacts.init(this);
    }

    public void Caching()
    {
        gameMgr.mediatorCaching();
        stageMgr.mediatorCaching();
        diceMgr.mediatorCaching();
    }
}
