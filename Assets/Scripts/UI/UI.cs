using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private Button titleButton;

    [SerializeField]
    private GameObject victoryPanel;
    [SerializeField]
    private Button titleButton2;



    public RewardPanel rewardPanel;
    public MaxSpellPanel maxSpellRewardPanel;
    public DIceRewardPanel diceRewardPanel;
    public ArtifactRewardPanel artifactRewardPanel;
    public MagicInfoPanel magicInfoPanel;
    public DamageInfoPanel damageInfoPanel;
    public GetDiceRewardPanel getDicePanel;
    public ArtifactInfoPanel playerArtifactPanel;
    public EventPanel evenetPanel;
    public EventTextPanel evenetTextPanel;
    public PausePanel pausePanel;
    public TitlePanel titlePanel;

    public GameObject backGroundPanel;

    public Button currentMagicInfo;
    public Button getMagicInfo;

    public AudioSource audioSource;
    public List<AudioClip> uiAudioClips = new List<AudioClip>();

    private void Awake()
    {
        rewardPanel.Init();
        maxSpellRewardPanel.Init();
        diceRewardPanel.Init();
        artifactRewardPanel.Init();
        magicInfoPanel.Init();
        damageInfoPanel.Init();
        getDicePanel.Init();
        playerArtifactPanel.Init();
        evenetPanel.Init();
        evenetTextPanel.Init();
        pausePanel.Init();
        titlePanel.Init(); 

        titleButton.onClick.AddListener(() => ReturnTitle());
        titleButton2.onClick.AddListener(() => ReturnTitle());

        currentMagicInfo.onClick.AddListener(() => { magicInfoPanel.OpenPanel(); magicInfoPanel.SetToggleMagicInfo(false); magicInfoPanel.magicInfoToggle.isOn = false; });
        getMagicInfo.onClick.AddListener(() => { magicInfoPanel.OpenPanel(); magicInfoPanel.SetToggleMagicInfo(true); magicInfoPanel.magicInfoToggle.isOn = true; });

    }

    public void SetInfomationButton(bool ison)
    {
        currentMagicInfo.interactable = ison;
        magicInfoPanel.magicInfoToggle.interactable = ison;
    }

    public void InfomationClear()
    {
        for (int i = 0; i < 5; i++)
        {
            damageInfoPanel.damages[i].text = string.Empty;
        }
        for (int i = 0; i < 9; i++)
        {
           magicInfoPanel.infoMagics[i].gameObject.SetActive(false);
        }
    }


    public void GameOver()
    {
        backGroundPanel.gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(true);
    }

    public void Victory()
    {
        backGroundPanel.gameObject.SetActive(true);
        victoryPanel.gameObject.SetActive(true);
    }
    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }

}
