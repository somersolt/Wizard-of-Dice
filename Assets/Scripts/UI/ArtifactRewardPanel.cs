using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactRewardPanel : Panel
{
    [SerializeField]
    private Button[] artifacts = new Button[3];
    private ArtifactData[] artifactDatas = new ArtifactData[3];
    private TextMeshProUGUI[] artifactsNames = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] artifactsInfos = new TextMeshProUGUI[3];
    public override void Init()
    {
        base.Init();
        for (int i = 0; i < artifacts.Length; i++)
        {
            artifactsNames[i] = artifacts[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            artifactsInfos[i] = artifacts[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        } // 유물 보상
    }


    public void OnArtifactReward()
    {
        RewardSound(0);
        ArtifactRewardClear();
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            artifacts[i].onClick.AddListener(() => { GetArifact(artifactDatas[index], index); });
            int a = Random.Range(0, mediator.artifacts.artifactList.Count);
            if (mediator.artifacts.artifactList.Count != 0)
            {
                artifactDatas[i] = mediator.artifacts.artifactList[a];
                artifactsNames[i].text = artifactDatas[i].NAME;
                artifactsInfos[i].text = artifactDatas[i].DESC;
                mediator.artifacts.artifactList.Remove(mediator.artifacts.artifactList[a]);
            }
            else if (mediator.artifacts.artifactList.Count == 0)
            {
                Debug.Log("오류");
            }
        }

        SlideOpenPanel();
    }


    public void GetArifact(ArtifactData artifactData, int index)
    {
        if (artifactData == null) { return; }
        for (int i = 0; i < artifacts.Length; i++)
        {
            if (i == index)
            {
                artifactData.ONARTIFACT = true;
                mediator.artifacts.playersArtifactsLevel[artifactData.ID]++;
                mediator.ui.playerArtifactPanel.ArtifactUpdate(artifactData, mediator.stageMgr.currentField - 1);
                continue;
            }

            if (i != index)
            {
                mediator.artifacts.artifactList.Add(artifactDatas[i]);
            }
        }
        GetArtifactEffect(artifactData.ID);
        mediator.gameMgr.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
        ClosePanel();
        RewardSound(2);
        if (artifactData.ID == 3)
        {
            mediator.ui.rewardPanel.OnReward(RewardMode.Artifact
                , mediator.artifacts.valueData.Value3 - 1);
            return;
        }

        mediator.stageMgr.NextStage();
    }


    private void ArtifactRewardClear()
    {
        artifacts[0].onClick.RemoveAllListeners();
        artifacts[1].onClick.RemoveAllListeners();
        artifacts[2].onClick.RemoveAllListeners();
    }

    private void GetArtifactEffect(int Id)
    {
        switch (Id)
        {
            case 0:
                break;
            case 1:
                mediator.diceMgr.Artifact2();
                break;
            case 2:
                mediator.diceMgr.manipulList[0] = 1;
                mediator.diceMgr.manipulList[1] = 2;
                mediator.diceMgr.manipulList[2] = 3;
                break;
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                break;
            case 9:
                mediator.gameMgr.MaxLifeSet(mediator.artifacts.valueData.Value9);
                RewardSound(3);
                mediator.gameMgr.LifeMax();
                break;
        }
    }
}
