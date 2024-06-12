using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mediator : MonoBehaviour
{

    public GameMgr gameMgr;
    public StageMgr stageMgr;
    public DiceMgr diceMgr;


    private void Awake()
    {
        gameMgr = FindObjectOfType<GameMgr>();
        stageMgr = FindObjectOfType<StageMgr>();
        diceMgr = FindObjectOfType<DiceMgr>();
    }

}
