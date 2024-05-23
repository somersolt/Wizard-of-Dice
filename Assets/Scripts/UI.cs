using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public List<SpellData> rewardList = new List<SpellData>();

    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private Button titleButton;

    [SerializeField]
    private GameObject victoryPanel;
    [SerializeField]
    private Button titleButton2;

    [SerializeField]
    private GameObject rewardPanel;
    [SerializeField]
    private Button[] rewards = new Button[3];
    private TextMeshProUGUI[] spellNames = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] spellInfos = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] spellLevels = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] newTexts = new TextMeshProUGUI[2];

    private SpellData[] rewardSpells = new SpellData[2];
    private Image[] examples = new Image[2];
    private SpellData empty = new SpellData();

    [SerializeField]
    private GameObject getDicePanel;
    [SerializeField]
    private Image getDiceImage;
    [SerializeField]
    private Button diceRewardConfirm;

    [SerializeField]
    private GameObject maxSpellRewardPanel;
    public Button[] maxSpells = new Button[9];
    private TextMeshProUGUI[] maxSpellNames = new TextMeshProUGUI[9];
    private TextMeshProUGUI[] maxSpellInfos = new TextMeshProUGUI[9];
    private TextMeshProUGUI[] maxSpellLevels = new TextMeshProUGUI[9];
    private Image[] maxSpellexamples = new Image[9];

    [SerializeField]
    private GameObject diceRewardPanel;
    [SerializeField]
    private Button[] diceRewards = new Button[3];
    private TextMeshProUGUI[] diceRewardNames = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] diceRewardInfos = new TextMeshProUGUI[3];

    [SerializeField]
    private GameObject artifactRewardPanel;
    [SerializeField]
    private Button[] artifacts = new Button[3];
    private ArtifactData[] artifactDatas = new ArtifactData[3];
    private TextMeshProUGUI[] artifactsNames = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] artifactsInfos = new TextMeshProUGUI[3];

    public GameObject magicInfoPanel;
    [SerializeField]
    private Button DamageInfoButton;
    public Image[] infoMagics = new Image[9];
    public TextMeshProUGUI[] infoMagicnames = new TextMeshProUGUI[9];
    public TextMeshProUGUI[] infoMagicInfos = new TextMeshProUGUI[9];
    public TextMeshProUGUI[] infoMagicLevels = new TextMeshProUGUI[9];
    private Image[] infoMagicexamples = new Image[9];

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

    public AudioSource audioSource;
    [SerializeField]
    private List<AudioClip> audioClips = new List<AudioClip>();

    private void Awake()
    {
        titleButton.onClick.AddListener(() => ReturnTitle());
        titleButton2.onClick.AddListener(() => ReturnTitle());
        diceRewardConfirm.onClick.AddListener(() =>
        {
            getDicePanel.gameObject.SetActive(false);
            GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
            StageMgr.Instance.NextStage();
        });
        ReturnButton.onClick.AddListener(() => { PausePanel.gameObject.SetActive(false); Time.timeScale = 1; });
        QuitGame.onClick.AddListener(() => { QuitPanel.gameObject.SetActive(true); });
        QuitYes.onClick.AddListener(() => { Time.timeScale = 1; SceneManager.LoadScene("Title"); });
        QuitNo.onClick.AddListener(() => { QuitPanel.gameObject.SetActive(false); });

        for (int i = 0; i < rewards.Length - 1; i++)
        {
            spellNames[i] = rewards[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            examples[i] = rewards[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("ex").GetComponentInChildren<Image>();
            spellInfos[i] = rewards[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            spellLevels[i] = rewards[i].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
            newTexts[i] = rewards[i].transform.Find("new").GetComponentInChildren<TextMeshProUGUI>();
        } // 족보 보상 1~2칸

        spellNames[2] = rewards[2].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
        spellInfos[2] = rewards[2].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        spellLevels[2] = rewards[2].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
        // 족보보상 3번째 칸

        for (int i = 0; i < 9; i++)
        {
            int index = i;
            maxSpellNames[i] = maxSpells[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            maxSpellInfos[i] = maxSpells[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            maxSpellLevels[i] = maxSpells[i].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
            maxSpellexamples[i] = maxSpells[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("ex").GetComponentInChildren<Image>();
            var path = (i + 1).ToString();
            maxSpellexamples[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", path)); ;
            var maxSpell = DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(DamageCheckSystem.rankids[i] + 2);
            maxSpellNames[i].text = maxSpell.GetName;
            maxSpellInfos[i].text = maxSpell.GetDesc;
            maxSpellLevels[i].text = "초월";
            maxSpells[i].onClick.AddListener(() =>
            {
                GameMgr.Instance.SetRankList(index);
                maxSpells[index].gameObject.SetActive(false);
                maxSpellRewardPanel.gameObject.SetActive(false);
                GameMgr.Instance.audioSource.PlayOneShot(GameMgr.Instance.audioClips[8]);
                GameMgr.Instance.RanksListUpdate();
                StageMgr.Instance.NextStage();
            });
            maxSpells[i].gameObject.SetActive(false);
        } //초월 강화

        for (int i = 0; i < diceRewards.Length; i++)
        {
            diceRewardNames[i] = diceRewards[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            diceRewardInfos[i] = diceRewards[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        } // 상점 보상

        for (int i = 0; i < artifacts.Length; i++)
        {
            artifactsNames[i] = artifacts[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            artifactsInfos[i] = artifacts[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        } // 유물 보상

        magicExitButton.onClick.AddListener(() => { magicInfoPanel.gameObject.SetActive(false); });
        damageExitButton.onClick.AddListener(() => { damageInfoPanel.gameObject.SetActive(false); });
        DamageInfoButton.onClick.AddListener(() => { magicInfoPanel.gameObject.SetActive(false); ; damageInfoPanel.gameObject.SetActive(true); });

        for (int i = 0; i < 9; i++)
        {
            infoMagicnames[i] = infoMagics[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            infoMagicInfos[i] = infoMagics[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
            infoMagicLevels[i] = infoMagics[i].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
            infoMagicexamples[i] = infoMagics[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("ex").GetComponentInChildren<Image>();
            var path = (i + 1).ToString();
            infoMagicexamples[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", path)); ;
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


        playerArtifactButton.onClick.AddListener(() => { playerArtifactPanel.gameObject.SetActive(true);});
        playerArtifactQuitButton.onClick.AddListener(() => { playerArtifactPanel.gameObject.SetActive(false);});

        eventPanelButton.onClick.AddListener(() => { evenetPanel.gameObject.SetActive(false); evenetTextPanel.gameObject.SetActive(true); });
        eventTextPanelButton.onClick.AddListener(() => { evenetTextPanel.gameObject.SetActive(false); OnDiceReward(); });

        foreach ( var a in artifactInfo)
        {
            a.text = " ";
        }
    }
    public void OnReward(RewardMode mode = RewardMode.Normal  , int count = 0)
    {
        audioSource.PlayOneShot(audioClips[0]);
        RewardClear();
        for (int i = 0; i < 2; i++)
        {
            int index = i;
            rewards[i].onClick.AddListener(() => { GetSpell(rewardSpells[index], index, mode , count); });
            int a = Random.Range(0, rewardList.Count);
            if (rewardList.Count != 0)
            {
                rewardSpells[i] = rewardList[a];
                spellNames[i].text = rewardList[a].GetName;
                spellInfos[i].text = rewardList[a].GetDesc;
                var path = (rewardList[a].ID % 100 / 10).ToString();
                examples[i].sprite = Resources.Load<Sprite>(string.Format("Image/{0}", path)); ;
                switch (rewardList[a].LEVEL)
                {
                    case 1:
                        spellLevels[i].text = "일반";
                        if (i == 0)
                        { newTexts[0].gameObject.SetActive(true); }
                        if (i == 1)
                        { newTexts[1].gameObject.SetActive(true); }
                        break;
                    case 2:
                        spellLevels[i].text = "강화";
                        break;
                }
                rewardList.Remove(rewardList[a]);
            }
            else if (rewardList.Count == 0)
            {
                rewardSpells[i] = empty;
                spellNames[i].text = "매 진";
                spellInfos[i].text = "SOLD OUT!!";
                spellLevels[i].text = " ";
            }
        }

        spellNames[2].text = "신체 강화";
        spellInfos[2].text = $"기본 공격력 + 3 \n 주사위 개수 x 5 ({(int)GameMgr.Instance.currentDiceCount * 5}) 만큼 회복";
        spellLevels[2].text = " ";
        if (mode == RewardMode.Artifact && count > 0)
        {
            rewards[2].onClick.AddListener(() => GetStatus(3,RewardMode.Artifact, count));
        }
        else
        {
            rewards[2].onClick.AddListener(() => GetStatus(3));
        }
        rewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(rewardPanel));
    }


    public void OnDiceReward()
    {
        audioSource.PlayOneShot(audioClips[0]);
        DiceRewardClear();

        switch (GameMgr.Instance.currentDiceCount)
        {
            case GameMgr.DiceCount.three:
                diceRewards[0].onClick.AddListener(() =>
                {
                    GameMgr.Instance.currentDiceCount = GameMgr.DiceCount.four;
                    GameMgr.Instance.GetDice4Ranks();
                    diceRewardPanel.gameObject.SetActive(false);
                    GetDice();
                });

                diceRewardNames[0].text = "주사위 개수 추가";
                diceRewardInfos[0].text = "매 턴 굴릴 수 있는 주사위를 4개로 증가 \n 상점에 4주사위 마법서가 등장합니다.";
                break;
            case GameMgr.DiceCount.four:
                diceRewards[0].onClick.AddListener(() =>
                {
                    GameMgr.Instance.currentDiceCount = GameMgr.DiceCount.five;
                    GameMgr.Instance.GetDice5Ranks();
                    diceRewardPanel.gameObject.SetActive(false);
                    GetDice();
                });

                diceRewardNames[0].text = "주사위 개수 추가";
                diceRewardInfos[0].text = "매 턴 굴릴 수 있는 주사위를 5개로 증가 \n 상점에 5주사위 마법서가 등장합니다.";
                break;
            case GameMgr.DiceCount.five:

                diceRewardNames[0].text = "한계 도달";
                diceRewardInfos[0].text = "더 이상 주사위를 늘릴 수 없습니다.";
                break;
        }

        diceRewards[1].onClick.AddListener(() =>
        {
            GetStatus(20, RewardMode.Event);
        });

        diceRewardNames[1].text = "마나 증량";
        diceRewardInfos[1].text = "주사위 개수를 늘리지 않고 마법력 20 증가 \n 주사위 눈금 총합에 20을 더합니다.";

        foreach (var ranks in GameMgr.Instance.GetRankList())
        {
            if (ranks == 2)
            {
                diceRewards[2].onClick.AddListener(() =>
                {
                    diceRewardPanel.gameObject.SetActive(false);
                    maxSpellRewardPanel.gameObject.SetActive(true);
                    StartCoroutine(PanelSlide(maxSpellRewardPanel));
                });

                diceRewardNames[2].text = "초월 마법";
                diceRewardInfos[2].text = "보유한 마법 중 하나를 초월 등급으로 변경합니다. \n 초월 마법은 강한 위력과 특수 효과가 추가됩니다.";
                break;
            }
            else
            {
                diceRewardNames[2].text = "초월 마법";
                diceRewardInfos[2].text = "보유한 마법 중 하나를 초월 등급으로 변경합니다. \n 강화된 마법이 없어 초월할 수 없습니다.";
            }
        }

        diceRewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(diceRewardPanel));
    }


    public void OnArtifactReward()
    {
        audioSource.PlayOneShot(audioClips[0]);
        ArtifactRewardClear();
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            artifacts[i].onClick.AddListener(() => { GetArifact(artifactDatas[index], index); });
            int a = Random.Range(0, GameMgr.Instance.artifact.artifacts.Count);
            if (GameMgr.Instance.artifact.artifacts.Count != 0)
            {
                artifactDatas[i] = GameMgr.Instance.artifact.artifacts[a];
                artifactsNames[i].text = artifactDatas[i].NAME;
                artifactsInfos[i].text = artifactDatas[i].DESC;
                GameMgr.Instance.artifact.artifacts.Remove(GameMgr.Instance.artifact.artifacts[a]);
            }
            else if (GameMgr.Instance.artifact.artifacts.Count == 0)
            {
                Debug.Log("오류");
            }
        }

        artifactRewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(artifactRewardPanel));
    }



    public void GetDice()
    {
        audioSource.PlayOneShot(audioClips[1]);

        switch (GameMgr.Instance.currentDiceCount)
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
        getDicePanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(getDicePanel));
    }

    public void GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
    }

    public void Victory()
    {
        victoryPanel.gameObject.SetActive(true);
    }
    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void GetSpell(SpellData spellData, int index, RewardMode mode = RewardMode.Normal, int count = 0)
    {
        if (spellData == empty) { return; }
        for (int i = 0; i < rewardSpells.Length; i++)
        {
            if (i == index)
            {
                if (spellData.LEVEL == 1)
                {
                    rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(spellData.ID + 1));
                    //if (spellData.LEVEL == 1)
                    //{
                    //    NextRanks(spellData); // 스킬트리 방식, 폐기
                    //}
                }
                continue;
            }
            if (rewardSpells[i] != empty)
            {
                rewardList.Add(rewardSpells[i]);
            }
        }//돌려놓기 

        if (rewardSpells[index].LEVEL != 0)
        {
            GameMgr.Instance.SetRankList((spellData.ID % 100) / 10 - 1);
            if (rewardSpells[index].LEVEL == 2)
            {
                maxSpells[(spellData.ID % 100) / 10 - 1].gameObject.SetActive(true);
            }
        }

        GameMgr.Instance.RanksListUpdate();
        GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
        rewardPanel.gameObject.SetActive(false);
        GameMgr.Instance.audioSource.PlayOneShot(GameMgr.Instance.audioClips[8]);

        if (mode == RewardMode.Artifact && count > 0)
        {
            OnReward(RewardMode.Artifact, count - 1);
        }
        else
        {
            StageMgr.Instance.NextStage();
        }
    }

    public void GetStatus(int value, RewardMode mode = RewardMode.Normal, int count = 0)
    {
        if (mode != RewardMode.Event)
        {
            for (int i = 0; i < 2; i++)
            {
                if (rewardSpells[i] != empty)
                {
                    rewardList.Add(rewardSpells[i]);
                }
            }

            GameMgr.Instance.Heal((int)GameMgr.Instance.currentDiceCount * 5, 0);
        }

        GameMgr.Instance.curruntBonusStat += value;
        foreach (var artifact in GameMgr.Instance.artifact.artifacts)
        {
            if(artifact.ID == 0)
            {
                artifact.Set(0, "방화광", $"매턴 모든 적에게 '기본 마법력'({GameMgr.Instance.curruntBonusStat}) 만큼의 데미지");
            }
        }
        GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;

        if (mode != RewardMode.Event)
        {
            rewardPanel.gameObject.SetActive(false);
        }
        else
        {
            diceRewardPanel.gameObject.SetActive(false);
        }
        GameMgr.Instance.audioSource.PlayOneShot(GameMgr.Instance.audioClips[8]);
        if(mode == RewardMode.Artifact)
        {
            OnReward(RewardMode.Artifact, count - 1);
            return;
        }
        StageMgr.Instance.NextStage();
    }


    public void GetArifact(ArtifactData artifactData, int index)
    {
        if (artifactData == null) { return; }
        for (int i = 0; i < artifacts.Length; i++)
        {
            if (i == index)
            {
                artifactData.ONARTIFACT = true;
                GameMgr.Instance.artifact.playersArtifacts[artifactData.ID]++;
                switch (StageMgr.Instance.currentField)
                {
                    case 1:
                        ArtifactUpdate(artifactData, 0);
                        break;
                    case 2:
                        ArtifactUpdate(artifactData, 1);
                        break;
                    case 3:
                        ArtifactUpdate(artifactData, 2);
                        break;
                }
                continue;
            }

            if (i != index)
            {
                GameMgr.Instance.artifact.artifacts.Add(artifactDatas[i]);
            }
        }
        GetArtifactEffect(artifactData.ID);
        GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;
        artifactRewardPanel.gameObject.SetActive(false);
        GameMgr.Instance.audioSource.PlayOneShot(GameMgr.Instance.audioClips[8]);
        if (artifactData.ID == 3)
        {
            OnReward(RewardMode.Artifact
                ,GameMgr.Instance.artifact.Value3 - 1);
            return;
        }

        StageMgr.Instance.NextStage();
    }

    public void ArtifactUpdate(ArtifactData artifactData, int index)
    {
        GameMgr.Instance.artifact.playersArtifactsNumber[index] = artifactData.ID;
        playerArtifacts[index].gameObject.SetActive(true);
        playerArtifactName[index].text = artifactData.NAME;
        playerArtifactInfo[index].text = artifactData.DESC;
        artifactInfo[index].text = artifactData.NAME;
    }

    public void ArtifactInfoUpdate()
    {
        for(int i = 0; i < 3; i++)
        {
            if (playerArtifactName[i].text == "방화광")
            {
                playerArtifactInfo[i].text = $"매턴 모든 적에게 '기본 마법력'({GameMgr.Instance.curruntBonusStat}) 만큼의 데미지";
            }
        }
    }

    private IEnumerator PanelSlide(GameObject panel)
    {
        float duration = 0.3f;
        float time = 0f;
        Vector3 startpos = panel.transform.position;

        panel.transform.position = new Vector3(1000, startpos.y, startpos.z);

        while (time < duration)
        {
            panel.transform.position = Vector3.Lerp(panel.transform.position, startpos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        panel.transform.position = startpos;
    }

    private void GetArtifactEffect(int Id)
    {
        switch (Id)
        {
            case 0:
                break;
            case 1:
                DiceMgr.Instance.Artifact2();
                break;
            case 2:
                DiceMgr.Instance.manipulList[0] = 1;
                DiceMgr.Instance.manipulList[1] = 2;
                DiceMgr.Instance.manipulList[2] = 3;
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                GameMgr.Instance.MaxLifeSet(GameMgr.Instance.artifact.Value9);
                GameMgr.Instance.audioSource.PlayOneShot(GameMgr.Instance.audioClips[5]);
                GameMgr.Instance.LifeMax();
                break;
        }
    }

    private void RewardClear()
    {
        rewards[0].onClick.RemoveAllListeners();
        rewards[1].onClick.RemoveAllListeners();
        rewards[2].onClick.RemoveAllListeners();
        newTexts[0].gameObject.SetActive(false);
        newTexts[1].gameObject.SetActive(false);
    }

    private void DiceRewardClear()
    {
        diceRewards[0].onClick.RemoveAllListeners();
        diceRewards[1].onClick.RemoveAllListeners();
        diceRewards[2].onClick.RemoveAllListeners();
    }
    private void ArtifactRewardClear()
    {
        artifacts[0].onClick.RemoveAllListeners();
        artifacts[1].onClick.RemoveAllListeners();
        artifacts[2].onClick.RemoveAllListeners();
    }
    private void NextRanks(SpellData spellData)
    {
        if (spellData.ID == (int)RanksIds.OnePair)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.Straight3));
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.Triple));
        }
        if (spellData.ID == (int)RanksIds.Straight3)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.Straight4));
        }
        if (spellData.ID == (int)RanksIds.Straight4)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.Straight5));
        }
        if (spellData.ID == (int)RanksIds.Triple)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.TwoPair));
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.KindOf4));
        }
        if (spellData.ID == (int)RanksIds.TwoPair)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.FullHouse));
        }
        if (spellData.ID == (int)RanksIds.KindOf4)
        {
            rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get((int)RanksIds.KindOf5));
        }
    }

    public void EventStage()
    {

        switch (StageMgr.Instance.currentField)
        {
            case 1:
                eventFace.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "merchant"));
                eventName.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(40001);
                eventText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(50001);
                rewardText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(60001);
                break;
            case 2:
                eventFace.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "devil"));
                eventName.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(40002);
                eventText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(50002);
                rewardText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(60002);
                break;
            case 3:
                eventFace.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "stone"));
                eventName.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(40003);
                eventText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(50003);
                rewardText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(60003);
                break;
            case 4:
                eventFace.sprite = Resources.Load<Sprite>(string.Format("Image/{0}", "merchant"));
                eventName.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(40004);
                eventText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(50004);
                rewardText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(60004);
                break;
        }

        evenetPanel.gameObject.SetActive(true);

    }

}
