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
    private Button diceRewardConfirm;

    [SerializeField]
    private GameObject maxSpellRewardPanel;
    [SerializeField]
    private Button[] maxSpells = new Button[9];
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
        } // ���� ���� 1~2ĭ

        spellNames[2] = rewards[2].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
        spellInfos[2] = rewards[2].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        spellLevels[2] = rewards[2].transform.Find("level").GetComponentInChildren<TextMeshProUGUI>();
        // �������� 3��° ĭ

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
            maxSpellLevels[i].text = "�ʿ�";
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
        } //�ʿ� ��ȭ

        for (int i = 0; i < diceRewards.Length; i++)
        {
            diceRewardNames[i] = diceRewards[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            diceRewardInfos[i] = diceRewards[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        } // ���� ����

        for (int i = 0; i < artifacts.Length; i++)
        {
            artifactsNames[i] = artifacts[i].transform.Find("namePanel").GetComponentInChildren<LayoutElement>().transform.Find("name").GetComponentInChildren<TextMeshProUGUI>();
            artifactsInfos[i] = artifacts[i].transform.Find("Info").GetComponentInChildren<TextMeshProUGUI>();
        } // ���� ����

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
        } //���� ����
    }
    public void OnReward(int mode = 0)
    {
        audioSource.Play();
        RewardClear();
        for (int i = 0; i < 2; i++)
        {
            int index = i;
            rewards[i].onClick.AddListener(() => { GetSpell(rewardSpells[index], index, mode); });
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
                        spellLevels[i].text = "�Ϲ�";
                        if (i == 0)
                        { newTexts[0].gameObject.SetActive(true); }
                        if (i == 1)
                        { newTexts[1].gameObject.SetActive(true); }
                        break;
                    case 2:
                        spellLevels[i].text = "��ȭ";
                        break;
                }
                rewardList.Remove(rewardList[a]);
            }
            else if (rewardList.Count == 0)
            {
                rewardSpells[i] = empty;
                spellNames[i].text = "�� ��";
                spellInfos[i].text = "SOLD OUT!!";
                spellLevels[i].text = " ";
            }
        }

        spellNames[2].text = "���ݷ� up";
        spellInfos[2].text = "�ֻ��� ���� ���� + 3";
        spellLevels[2].text = " ";
        rewards[2].onClick.AddListener(() => GetStatus(3));

        rewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(rewardPanel));
    }


    public void OnDiceReward()
    {

        audioSource.Play();
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

                diceRewardNames[0].text = "�ֻ��� ���� �߰�";
                diceRewardInfos[0].text = "�� �� ���� �� �ִ� �ֻ����� 4���� ���� \n ������ 4�ֻ��� �������� �����մϴ�.";
                break;
            case GameMgr.DiceCount.four:
                diceRewards[0].onClick.AddListener(() =>
                {
                    GameMgr.Instance.currentDiceCount = GameMgr.DiceCount.five;
                    GameMgr.Instance.GetDice5Ranks();
                    diceRewardPanel.gameObject.SetActive(false);
                    GetDice();
                });

                diceRewardNames[0].text = "�ֻ��� ���� �߰�";
                diceRewardInfos[0].text = "�� �� ���� �� �ִ� �ֻ����� 5���� ���� \n ������ 5�ֻ��� �������� �����մϴ�.";
                break;
            case GameMgr.DiceCount.five:

                diceRewardNames[0].text = "�Ѱ� ����";
                diceRewardInfos[0].text = "�� �̻� �ֻ����� �ø� �� �����ϴ�.";
                break;
        }

        diceRewards[1].onClick.AddListener(() =>
        {
            GetStatus(20, "DiceReward");
        });

        diceRewardNames[1].text = "���� ����";
        diceRewardInfos[1].text = "�ֻ��� ������ �ø��� �ʰ� ������ 20 ���� \n �ֻ��� ���� ���տ� 20�� ���մϴ�.";

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

                diceRewardNames[2].text = "�ʿ� ����";
                diceRewardInfos[2].text = "������ ���� �� �ϳ��� �ʿ� ������� �����մϴ�. \n �ʿ� ������ ���� ���°� Ư�� ȿ���� �߰��˴ϴ�.";
                break;
            }
            else
            {
                diceRewardNames[2].text = "�ʿ� ����";
                diceRewardInfos[2].text = "������ ���� �� �ϳ��� �ʿ� ������� �����մϴ�. \n ��ȭ�� ������ ���� �ʿ��� �� �����ϴ�.";
            }
        }

        diceRewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(diceRewardPanel));
    }


    public void OnArtifactReward()
    {
        audioSource.Play();

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
                Debug.Log("����");
            }
        }

        artifactRewardPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(artifactRewardPanel));
    }



    public void GetDice()
    {
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

    public void GetSpell(SpellData spellData, int index, int mode = 0)
    {
        if (spellData == empty) { return; }
        for (int i = 0; i < rewardSpells.Length; i++)
        {
            if (i == index)
            {
                if (spellData.LEVEL != 2 && spellData.LEVEL != 0)
                {
                    rewardList.Add(DataTableMgr.Get<SpellTable>(DataTableIds.SpellBook).Get(spellData.ID + 1));
                    //if (spellData.LEVEL == 1)
                    //{
                    //    NextRanks(spellData); // ��ųƮ�� ���, ���
                    //}
                }
                continue;
            }
            if (rewardSpells[i] != empty)
            {
                rewardList.Add(rewardSpells[i]);
            }
        }//�������� 

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

        if (mode == 0)
        {
            StageMgr.Instance.NextStage();
        }
        else
        {
            OnReward(mode - 1);
        }
    }

    public void GetStatus(int value, string mode = default)
    {
        if (mode == default)
        {
            for (int i = 0; i < 2; i++)
            {
                if (rewardSpells[i] != empty)
                {
                    rewardList.Add(rewardSpells[i]);
                }
            }
        }

        GameMgr.Instance.curruntBonusStat += value;
        GameMgr.Instance.artifact.artifacts[0].Set(0, "��ȭ��", $"���� ��� ������ '�⺻ ������'({GameMgr.Instance.curruntBonusStat}) ��ŭ�� ������");
        GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.PlayerDice;

        if (mode == default)
        { 
            rewardPanel.gameObject.SetActive(false); 
        }
        else
        {
            diceRewardPanel.gameObject.SetActive(false);
        }
        GameMgr.Instance.audioSource.PlayOneShot(GameMgr.Instance.audioClips[8]);
        StageMgr.Instance.NextStage();
    }


    private void GetArifact(ArtifactData artifactData, int index)
    {
        if (artifactData == null) { return; }
        for (int i = 0; i < artifacts.Length; i++)
        {
            if (i == index)
            {
                artifactData.ONARTIFACT = true;
                GameMgr.Instance.artifact.playersArtifacts[artifactData.ID]++;
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
        if (artifactData.ID != 3)
        {
            StageMgr.Instance.NextStage();
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
                OnReward(GameMgr.Instance.artifact.Value3 - 1);
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


}
