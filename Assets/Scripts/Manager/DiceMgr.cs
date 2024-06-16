using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DiceMgr : MonoBehaviour
{
    public Mediator mediator;
    private GameMgr gameMgr;
    private StageMgr stageMgr;

    public void mediatorCaching()
    {
        gameMgr = mediator.gameMgr;
        stageMgr = mediator.stageMgr;
    }

    [SerializeField]
    private SpinControl[] spinControlers = new SpinControl[5];
    private bool[] buttonToggle = new bool[5];
    [SerializeField]
    private Image[] buttonLock = new Image[5];
    public Button[] dices = new Button[constant.diceMax]; // 주사위들
    private int[] dicesValues = new int[constant.diceMax]; // 주사위 눈 결과값
    [HideInInspector]
    public int[] numbersCount = new int[constant.diceNumberMax]; // 랭크 체크 list
    private List<int> diceNumbers = new List<int>(); // 주사위 눈 1~6 리스트 (변동가능)
    [HideInInspector]
    public List<int> selectedDice = new List<int>();

    private RanksFlag checkedRanksList; // 랭크 체크 후 9개 족보 활성화 여부 저장
    public RanksFlag CheckedRanksList
    {
        get { return checkedRanksList; }
    }
    [HideInInspector]
    public int[] manipulList = new int[constant.diceMax]; // 주사위 조작용 리스트

    [HideInInspector]
    public bool tutorialMode;

    [SerializeField]
    private Button reRoll; // 재굴림
    [SerializeField]
    private Button confirm; // 확정

    [HideInInspector]
    public int countToResult;
    private int rerollCount;
    private int maxRerollCount = 2;
    private bool showResult;

    private int totalPlayerDamageValue;

    private List<SpinControl> enemyDiceList = new List<SpinControl>();
    private List<SpinControl> currentEnemyDiceList = new List<SpinControl>();
    [SerializeField]
    private SpinControl enemyDice;
    [SerializeField]
    private SpinControl enemyDiceTwin1;
    [SerializeField]
    private SpinControl enemyDiceTwin2;
    [SerializeField]
    private SpinControl enemyDiceTriple1;
    [SerializeField]
    private SpinControl enemyDiceTriple2;
    private int[] enemyValue = new int[3];
    [HideInInspector]
    public int currentEnemyDice;

    [HideInInspector]
    public List<Coroutine> activeCoroutines = new List<Coroutine>(); // 코루틴관리자
    [HideInInspector]
    public List<SpinControl> coroutineList = new List<SpinControl>(); // 코루틴관리자2


    private void Awake()
    {
        for (int i = 0; i < constant.diceNumberMax; i++)
        {
            diceNumbers.Add(i + 1);  // 1부터 6까지 추가
        }

        for (int i = 0; i < constant.diceMax; i++)
        {
            int index = i;
            dices[index].onClick.AddListener(() => OnDiceButtonClick(index));  // 버튼 세팅
        }
        reRoll.onClick.AddListener(() => DiceRoll());
        confirm.onClick.AddListener(() => { EventBus.Publish(EventType.PlayerAttack); AllDIceAndButtonLock(); });

        enemyDiceList.Add(enemyDice);
        enemyDiceList.Add(enemyDiceTwin1);
        enemyDiceList.Add(enemyDiceTwin2);
        enemyDiceList.Add(enemyDiceTriple1);
        enemyDiceList.Add(enemyDiceTriple2);

        currentEnemyDice = 1;
    }

    private void Start()
    {
        mediatorCaching();
    }

    public void InfoClear()
    {
        selectedDice.Clear();
        showResult = false;
        totalPlayerDamageValue = 0;
        for (int i = 0; i < (int)gameMgr.currentDiceCount; i++)
        {
            totalPlayerDamageValue += dicesValues[i];
            //buttonToggle[i] = false; // 전부 고정 푸는 코드
            if (buttonToggle[i] == false)
            {
                dices[i].GetComponent<Image>().color = UsedColor.buttonSelectedColor;
                selectedDice.Add(i);
            }
        }
    }

    public void DiceResult()
    {
        checkedRanksList = RankCheckSystem.RankCheck(numbersCount);

        selectedDice.Clear();
        showResult = false;
        totalPlayerDamageValue = 0;
        for (int i = 0; i < (int)gameMgr.currentDiceCount; i++)
        {
            totalPlayerDamageValue += dicesValues[i];
            //buttonToggle[i] = false; // 전부 고정 푸는 코드
            if (buttonToggle[i] == false)
            {
                dices[i].GetComponent<Image>().color = UsedColor.buttonUnSelectedColor;
                selectedDice.Add(i);
            }
        }
        gameMgr.SetResult(checkedRanksList, totalPlayerDamageValue);
        confirm.interactable = true;

        if (!tutorialMode)
        {
            if (rerollCount > 0)
            {
                for (int i = 0; i < constant.diceMax; i++)
                {
                    if (buttonToggle[i] == false)
                    {
                        dices[i].interactable = true;
                    }
                }
                reRoll.interactable = true;
            }
            else
            {
                reRoll.interactable = false;
            }
        }
    }

    public void AllDIceAndButtonLock()
    {
        for (int i = 0; i < constant.diceMax; i++)
        {
            dices[i].interactable = false;
        }
        confirm.interactable = false;
        reRoll.interactable = false;
    }

    public void TutorialModeControl(int Mode)
    {
        dices[0].interactable = false;
        dices[1].interactable = false;
        switch (Mode)
        {
            case 0:
                confirm.interactable = false;
                reRoll.interactable = false;
                break;
            case 1:
                confirm.interactable = false;
                reRoll.interactable = true;
                break;
            case 2:
                confirm.interactable = true;
                reRoll.interactable = false;
                break;
        }
    }


    public void TutorialButtonControl(bool control)
    {
        reRoll.onClick.RemoveAllListeners();
        confirm.onClick.RemoveAllListeners();
        if (control)
        {
            reRoll.onClick.AddListener(() =>
            {
                IncrementTutorialTextCount();
                DiceRoll(false, GameMode.Tutorial2);
            });
            confirm.onClick.AddListener(() => { IncrementTutorialTextCount(); gameMgr.PlayerAttack(); AllDIceAndButtonLock(); });
        }
        else
        {
            reRoll.onClick.AddListener(() => DiceRoll());
            confirm.onClick.AddListener(() => { gameMgr.PlayerAttack(); AllDIceAndButtonLock(); });
        }
    }
    private void IncrementTutorialTextCount()
    {
        gameMgr.tutorial.NextStep();
    }
    public void DiceRoll(bool starting = false, GameMode mode = GameMode.Default)
    {
        gameMgr.ScrollsClear();
        gameMgr.audioSource.PlayOneShot(gameMgr.audioClips[6]);
        AllDIceAndButtonLock();
        showResult = true;
        countToResult = 0;
        for (int i = 0; i < numbersCount.Length; i++)
        {
            numbersCount[i] = 0;
        }
        int selectedDiceCount = selectedDice.Count;
        if (starting)
        {
            selectedDiceCount = (int)gameMgr.currentDiceCount;
            for (int i = 0; i < selectedDiceCount; i++)
            {
                buttonLock[i].gameObject.SetActive(false);
            }

            if (mediator.artifacts.playersArtifactsLevel[8] == 1)//9번 유물
            {
                rerollCount = mediator.artifacts.valueData.Value8 + maxRerollCount;
            }
            else
            {
                rerollCount = maxRerollCount;
            }
        }
        else
        {
            rerollCount--;
        }

        reRoll.GetComponentInChildren<TextMeshProUGUI>().text = "재굴림 : " + rerollCount.ToString();

        for (int i = 0; i < selectedDiceCount; i++)
        {
            Action<int> spinCallback = (diceIndex) =>
            {
                dices[diceIndex].GetComponentInChildren<TextMeshProUGUI>().text = dicesValues[diceIndex].ToString();
                countToResult++;
                if (countToResult == selectedDice.Count && showResult)
                {
                    DiceResult();
                }
            };

            Action<int> tutorialCallback = (diceIndex) =>
            {
                dices[diceIndex].GetComponentInChildren<TextMeshProUGUI>().text = dicesValues[diceIndex].ToString();
                countToResult++;
                gameMgr.tutorial.eventCount++;
                if (countToResult == selectedDice.Count && showResult)
                {
                    DiceResult();
                }
            };

            if (mode == GameMode.Default)
            {
                if (starting)
                {
                    if (manipulList[selectedDice[i]] == 0)
                    {
                        StartCoroutine(SelectDiceRoll(selectedDice[i], starting, spinCallback));
                    }
                    else
                    {
                        StartCoroutine(SelectDiceRoll(selectedDice[i], starting, spinCallback, manipulList[selectedDice[i]]));
                    }
                    // 주사위 조작 코드
                }
                else
                {
                    StartCoroutine(SelectDiceRoll(selectedDice[i], starting, spinCallback));
                }
            }
            else if (mode == GameMode.Tutorial)
            {
                switch (i)
                {
                    case 0:
                        Coroutine newcoroutine = StartCoroutine(SelectDiceRoll(selectedDice[i], starting, tutorialCallback, 4));
                        activeCoroutines.Add(newcoroutine);
                        break;
                    case 1:
                        Coroutine newcoroutine2 = StartCoroutine(SelectDiceRoll(selectedDice[i], starting, tutorialCallback, 6));
                        activeCoroutines.Add(newcoroutine2);

                        break;
                }
            }
            else if (mode == GameMode.Tutorial2)
            {
                Coroutine newcoroutine = StartCoroutine(SelectDiceRoll(selectedDice[i], starting, tutorialCallback, 4));
                activeCoroutines.Add(newcoroutine);
            }

            dices[selectedDice[i]].GetComponent<Image>().color = new Color(0x214 / 255f, 0x214 / 255f, 0x214 / 255f);
            buttonToggle[selectedDice[i]] = false;
        }

        for (int i = 0; i < (int)gameMgr.currentDiceCount; i++)
        {
            numbersCount[dicesValues[i] - 1]++;
        }

    }

    private IEnumerator SelectDiceRoll(int index, bool starting, Action<int> callback, int manipul = 0)
    {
        if ((int)gameMgr.currentDiceCount <= index)
        {
            dicesValues[index] = 0;
            yield break;
        }

        dices[index].GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
        dices[index].interactable = false;
        if (manipul == 0)
        {
            dicesValues[index] = diceNumbers[UnityEngine.Random.Range(0, diceNumbers.Count)];
        }
        else
        {
            dicesValues[index] = manipul; // 주사위 조작 코드
        }

        if (starting)
        {
            spinControlers[index].DiceSpin(25 + index * 48 / (int)gameMgr.currentDiceCount, RotatePos.dicesPosList[dicesValues[index] - 1], this, () => callback(index));
        }
        else
        {
            spinControlers[index].DiceSpin(25, RotatePos.dicesPosList[dicesValues[index] - 1], this, () => callback(index));
        }
        yield return null;
    }

    public void OnDiceButtonClick(int i)
    {
        if (!buttonToggle[i])
        {
            selectedDice.Remove(i);
            if (selectedDice.Count == 0)
            {
                reRoll.interactable = false;
            }
            dices[i].GetComponent<Image>().color = UsedColor.diceUnSelectedColor;
            buttonToggle[i] = true;
            buttonLock[i].gameObject.SetActive(true);
        }
        else if (buttonToggle[i])
        {
            selectedDice.Add(i);
            if (selectedDice.Count != 0)
            {
                reRoll.interactable = true;
            }
            dices[i].GetComponent<Image>().color = UsedColor.diceSelectedColor;
            buttonToggle[i] = false;
            buttonLock[i].gameObject.SetActive(false);
        }
        gameMgr.audioSource.PlayOneShot(gameMgr.audioClips[7]);
    }


    public void DiceSet()
    {
        foreach (var dice in dices)
        {
            dice.gameObject.SetActive(false);
        }

        selectedDice.Clear();
        for (int i = 0; i < (int)mediator.gameMgr.currentDiceCount; i++)
        {
            dices[i].gameObject.SetActive(true);
            selectedDice.Add(i);
        }

    }

    public void EnemyDiceRoll()
    {
        if (stageMgr.currentStage == stageMgr.lastStage)
        {
            if (gameMgr.bossDoubleAttack)
            {
                foreach (var boss in stageMgr.enemies)
                {
                    if (boss.isBoss)
                    {
                        boss.WindEffect();
                    }
                }
            }
        }
        Action enemySpincallback = () => { EventBus.Publish(EventType.MonsterAttack); };
        gameMgr.enemyValue = 0;
        for (int i = 0; i < currentEnemyDice; i++)
        {
            enemyValue[i] = UnityEngine.Random.Range(1, 7);
            gameMgr.enemyValue += enemyValue[i];

            if (i+1 == currentEnemyDice)
            {
                currentEnemyDiceList[i].DiceSpin(30, RotatePos.dicesPosList[enemyValue[i] - 1], this, () => enemySpincallback());
            }
            else
            {
                currentEnemyDiceList[i].DiceSpin(30, RotatePos.dicesPosList[enemyValue[i] - 1], this);
            }
        }
    }
    public void Artifact2()
    {
        diceNumbers.Remove(3);
        diceNumbers.Remove(4);
        diceNumbers.Remove(5);
    }

    public void StopAllActiveCoroutines()
    {
        foreach (SpinControl coroutine in coroutineList)
        {
            coroutine.StopCoroutine(coroutine.coroutine);
        }
        coroutineList.Clear();
        foreach (Coroutine coroutine in activeCoroutines)
        {
            StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();
    }

    public void SetEnemyDiceCount(int i)
    { 
        foreach(SpinControl dice in enemyDiceList)
        {
            dice.gameObject.SetActive(false);
        }
        currentEnemyDiceList.Clear();
        currentEnemyDice = i;
        switch (i)
        {
            case 1:
                currentEnemyDiceList.Add(enemyDice);
                break;
            case 2:
                currentEnemyDiceList.Add(enemyDiceTwin1);
                currentEnemyDiceList.Add(enemyDiceTwin2);
                break;
            case 3:
                currentEnemyDiceList.Add(enemyDice);
                currentEnemyDiceList.Add(enemyDiceTriple1);
                currentEnemyDiceList.Add(enemyDiceTriple2);
                break;
        }
        foreach (SpinControl dice in currentEnemyDiceList)
        {
            dice.gameObject.SetActive(true);
        }
    }

}

