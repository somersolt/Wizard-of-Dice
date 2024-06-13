using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactInfoPanel : Panel
{
    public Image[] playerArtifacts = new Image[3];
    private TextMeshProUGUI[] playerArtifactName = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] playerArtifactInfo = new TextMeshProUGUI[3];
    [SerializeField]
    private Button playerArtifactButton;
    [SerializeField]
    private Button playerArtifactQuitButton;

    public TextMeshProUGUI[] artifactInfo = new TextMeshProUGUI[3];

    public override void Init()
    {
        base.Init();

        for (int i = 0; i < 3; i++)
        {
            playerArtifactName[i] = playerArtifacts[i].transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            playerArtifactInfo[i] = playerArtifacts[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        }
        playerArtifacts[0].gameObject.SetActive(true);
        playerArtifactName[0].text = "���� ���� ����";
        playerArtifactInfo[0].text = string.Empty;
        playerArtifacts[1].gameObject.SetActive(false);
        playerArtifacts[2].gameObject.SetActive(false);//���� ����


        playerArtifactButton.onClick.AddListener(() => { OpenPanel(); });
        playerArtifactQuitButton.onClick.AddListener(() => { ClosePanel(); });

        foreach (var a in artifactInfo)
        {
            a.text = string.Empty;
        }
    }

    public void ArtifactUpdate(ArtifactData artifactData, int index)
    {
        mediator.gameMgr.artifact.playersArtifactsNumber[index] = artifactData.ID;
        playerArtifacts[index].gameObject.SetActive(true);
        playerArtifactName[index].text = artifactData.NAME;
        playerArtifactInfo[index].text = artifactData.DESC;
        artifactInfo[index].text = artifactData.NAME;
    }


    public void ArtifactInfoUpdate()
    {
        for (int i = 0; i < 3; i++)
        {
            if (playerArtifactName[i].text == "��ȭ��")
            {
                playerArtifactInfo[i].text = $"���� ��� ������ '�⺻ ���ݷ�'(<color=purple>{mediator.gameMgr.curruntBonusStat}</color>) ��ŭ�� ������";
            }
        }
    }
}
