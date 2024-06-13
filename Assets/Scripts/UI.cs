using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private Mediator mediator;
    private DiceMgr diceMgr;
    private StageMgr stageMgr;
    private GameMgr gameMgr;


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


    [SerializeField]
    private GameObject getDicePanel;
    [SerializeField]
    private Image getDiceImage;
    [SerializeField]
    private Button diceRewardConfirm;


    public Toggle magicInfoToggle;
    public TextMeshProUGUI magicInfoToggleText;

    public GameObject magicInfoPanel;
    [SerializeField]
    private Button DamageInfoButton;
    public Image[] infoMagics = new Image[9];
    public TextMeshProUGUI[] infoMagicnames = new TextMeshProUGUI[9];
    public TextMeshProUGUI[] infoMagicInfos = new TextMeshProUGUI[9];
    public TextMeshProUGUI[] infoMagicLevels = new TextMeshProUGUI[9];
    private Image[] infoMagicexamples = new Image[9];
    public Image[] infoMagicstars = new Image[9];

    [SerializeField]
    private Button magicExitButton;

    [SerializeField]
    private GameObject damageInfoPanel;
    public TextMeshProUGUI[] damages = new TextMeshProUGUI[5];
    [SerializeField]
    private Button damageExitButton;

    public GameObject evenetPanel;
    public GameObject evenetTextPanel;
    [SerializeField]
    private Button eventPanelButton;
    [SerializeField]
    private Button eventTextPanelButton;
    [SerializeField]
    private Image eventFace;
    [SerializeField]
    private TextMeshProUGUI eventText;
    [SerializeField]
    private TextMeshProUGUI eventName;
    [SerializeField]
    private TextMeshProUGUI rewardText;

    public TextMeshProUGUI[] artifactInfo = new TextMeshProUGUI[3];


    public GameObject playerArtifactPanel;
    public Image[] playerArtifacts = new Image[3];
    private TextMeshProUGUI[] playerArtifactName = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] playerArtifactInfo = new TextMeshProUGUI[3];
    [SerializeField]
    private Button playerArtifactButton;
    [SerializeField]
    private Button playerArtifactQuitButton;


    public GameObject PausePanel;
    [SerializeField]
    private Button ReturnButton;
    [SerializeField]
    private Button QuitGame;
    [SerializeField]
    private GameObject QuitPanel;
    [SerializeField]
    private Button QuitYes;
    [SerializeField]
    private Button QuitNo;

    public GameObject BackGroundPanel;

    public AudioSource audioSource;
    public List<AudioClip> uiAudioClips = new List<AudioClip>();

    private void Awake()
    {
        mediator = FindObjectOfType<Mediator>();

        rewardPanel.Init();
        maxSpellRewardPanel.Init();
        diceRewardPanel.Init();
        artifactRewardPanel.Init();

        titleButton.onClick.AddListener(() => ReturnTitle());
        titleButton2.onClick.AddListener(() => ReturnTitle());
        diceRewardConfirm.onClick.AddListener(() =>
        {
            getDicePanel.gameObject.SetActive(false);
            BackGroundPanel.gameObject.SetActive(false);
            gameMgr.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
            stageMgr.NextStage();
        });
        ReturnButton.onClick.AddListener(() => { PausePanel.gameObject.SetActive(false); BackGroundPanel.gameObject.SetActive(false); Time.timeScale = 1; });
        QuitGame.onClick.AddListener(() => { QuitPanel.gameObject.SetActive(true); });
        QuitYes.onClick.AddListener(() => { Time.timeScale = 1; SceneManager.LoadScene("Title"); });
        QuitNo.onClick.AddListener(() => { QuitPanel.gameObject.SetActive(false); });


        magicExitButton.onClick.AddListener(() => { magicInfoPanel.gameObject.SetActive(false); BackGroundPanel.gameObject.SetActive(false); });
        damageExitButton.onClick.AddListener(() => { damageInfoPanel.gameObject.SetActive(false); BackGroundPanel.gameObject.SetActive(false); });
        DamageInfoButton.onClick.AddListener(() => { magicInfoPanel.gameObject.SetActive(false); ; damageInfoPanel.gameObject.SetActive(true); });

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

        for (int i = 0; i < 3; i++)
        {
            playerArtifactName[i] = playerArtifacts[i].transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            playerArtifactInfo[i] = playerArtifacts[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        }
        playerArtifacts[0].gameObject.SetActive(true);
        playerArtifactName[0].text = "보유 유물 없음";
        playerArtifactInfo[0].text = " ";
        playerArtifacts[1].gameObject.SetActive(false);
        playerArtifacts[2].gameObject.SetActive(false);//적용 유물


        playerArtifactButton.onClick.AddListener(() => { playerArtifactPanel.gameObject.SetActive(true); });
        playerArtifactQuitButton.onClick.AddListener(() => { playerArtifactPanel.gameObject.SetActive(false); });

        eventPanelButton.onClick.AddListener(() => { evenetPanel.gameObject.SetActive(false); evenetTextPanel.gameObject.SetActive(true); });
        eventTextPanelButton.onClick.AddListener(() => { evenetTextPanel.gameObject.SetActive(false); diceRewardPanel.OnDiceReward(); });

        foreach (var a in artifactInfo)
        {
            a.text = " ";
        }

        magicInfoToggle.onValueChanged.AddListener((isOn) => { toggleMagicInfo(isOn); });

    }

    private void Start()
    {
        gameMgr = mediator.gameMgr;
        diceMgr = mediator.diceMgr;
        stageMgr = mediator.stageMgr;
    }

    public void GetDice()
    {
        audioSource.PlayOneShot(uiAudioClips[1]);

        switch (gameMgr.currentDiceCount)
        {
            case GameMgr.DiceCount.two:
                getDiceImage.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Got_Dice_2to3"));
                break;
            case GameMgr.DiceCount.four:
                getDiceImage.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Got_Dice_3to4"));
                break;
            case GameMgr.DiceCount.five:
                getDiceImage.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "Got_Dice_4to5"));
                break;
        }
        BackGroundPanel.gameObject.SetActive(true);
        getDicePanel.gameObject.SetActive(true);
        //StartCoroutine(PanelSlide(getDicePanel));
    }

    public void GameOver()
    {
        BackGroundPanel.gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(true);
    }

    public void Victory()
    {
        BackGroundPanel.gameObject.SetActive(true);
        victoryPanel.gameObject.SetActive(true);
    }
    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void ArtifactUpdate(ArtifactData artifactData, int index)
    {
        gameMgr.artifact.playersArtifactsNumber[index] = artifactData.ID;
        playerArtifacts[index].gameObject.SetActive(true);
        playerArtifactName[index].text = artifactData.NAME;
        playerArtifactInfo[index].text = artifactData.DESC;
        artifactInfo[index].text = artifactData.NAME;
    }

    public void ArtifactInfoUpdate()
    {
        for (int i = 0; i < 3; i++)
        {
            if (playerArtifactName[i].text == "방화광")
            {
                playerArtifactInfo[i].text = $"매턴 모든 적에게 '기본 공격력'(<color=purple>{gameMgr.curruntBonusStat}</color>) 만큼의 데미지";
            }
        }
    }


    public void EventStage()
    {
        BackGroundPanel.gameObject.SetActive(true);
        switch (stageMgr.currentField)
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

        eventName.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(40000 + stageMgr.currentField);
        eventText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(50000 + stageMgr.currentField);
        rewardText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(60000 + stageMgr.currentField);

        evenetPanel.gameObject.SetActive(true);

    }


    public void toggleMagicInfo(bool isOn)
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
                if (gameMgr.GetRank(i) > 0)
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
                if ((diceMgr.CheckedRanksList & currentFlag) != 0)
                {
                    infoMagics[i].gameObject.SetActive(true);
                }
            }
        }

    }

}
