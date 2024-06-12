using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceMgr : MonoBehaviour
{

    public Mediator mediator;
    private GameMgr gameMgr;
    private StageMgr stageMgr;


    public bool onDiceRoll;
    [SerializeField]
    private SpinControl[] SpinControlers = new SpinControl[5];
    private bool[] buttonToggle = new bool[5];
    [SerializeField]
    private Image[] buttonLock = new Image[5];

    public Button[] dices = new Button[constant.diceMax]; // 주사위들
    private int[] dicesValues = new int[constant.diceMax]; // 주사위 눈 결과값

    public int[] numbersCount = new int[constant.diceNumberMax]; // 랭크 체크 list

    private List<int> diceNumbers = new List<int>(); // 주사위 눈 1~6 리스트 (변동가능)

    public List<int> selectedDice = new List<int>();

    private RanksFlag checkedRanksList; // 랭크 체크 후 9개 족보 활성화 여부 저장
    public RanksFlag CheckedRanksList
    {
        get { return checkedRanksList; }
    }

    public int[] manipulList = new int[constant.diceMax]; // 주사위 조작용 리스트

    public bool tutorialControl;
    public int tutorialControlMode = 0;

    [SerializeField]
    private Button reRoll; // 재굴림
    [SerializeField]
    private Button confirm; // 확정

    public int countToResult;
    private int rerollCount = 1;
    private bool onResult;

    private int totalValue;


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
    public int enemyValue;
    public int enemyValue2;
    public int enemyValue3;
    public int currentEnemyDice;

    public List<Coroutine> activeCoroutines = new List<Coroutine>(); // 코루틴관리자
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
            dices[index].onClick.AddListener(() => ButtonSelect(index));  // 버튼 세팅
        }
        reRoll.onClick.AddListener(() => DiceRoll());
        confirm.onClick.AddListener(() => { EventBus.Publish(EventType.PlayerAttack); onDiceRoll = true; });

        currentEnemyDice = 1;
    }

    private void Start()
    {
        gameMgr = mediator.gameMgr;
        stageMgr = mediator.stageMgr;
    }

    public void InfoClear()
    {
        selectedDice.Clear();
        onResult = false;
        onDiceRoll = false;
        totalValue = 0;
        for (int i = 0; i < (int)gameMgr.currentDiceCount; i++)
        {
            totalValue += dicesValues[i];
            //buttonToggle[i] = false; // 전부 고정 푸는 코드
            if (buttonToggle[i] == false)
            {
                dices[i].GetComponent<Image>().color = new Color(0x214 / 255f, 0x214 / 255f, 0x214 / 255f);
                selectedDice.Add(i);
            }
        }
    }

    private void Update()
    {
        if (countToResult == selectedDice.Count && onResult)
        {
            checkedRanksList = RankCheckSystem.RankCheck(numbersCount);

            selectedDice.Clear();
            onResult = false;
            onDiceRoll = false;
            totalValue = 0;
            for (int i = 0; i < (int)gameMgr.currentDiceCount; i++)
            {
                totalValue += dicesValues[i];
                //buttonToggle[i] = false; // 전부 고정 푸는 코드
                if (buttonToggle[i] == false)
                {
                    dices[i].GetComponent<Image>().color = new Color(0x214 / 255f, 0x214 / 255f, 0x214 / 255f);
                    selectedDice.Add(i);
                }
            }
            gameMgr.SetResult(checkedRanksList, totalValue);
        }

        if (!tutorialControl)
        {
            if (onDiceRoll)
            {
                for (int i = 0; i < constant.diceMax; i++)
                {
                    dices[i].interactable = false;
                }
                confirm.interactable = false;
                reRoll.interactable = false;
            }
            else
            {

                if (selectedDice.Count == 0 || Time.timeScale == 0)
                {
                    reRoll.interactable = false;
                }

                if (selectedDice.Count != 0 && rerollCount > 0 && Time.timeScale != 0)
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

                if (Time.timeScale != 0)
                {
                    confirm.interactable = true;
                }
                else if (Time.timeScale == 0)
                {
                    confirm.interactable = false;
                }
            }
        }
        else if (tutorialControl)
        {
            dices[0].interactable = false;
            dices[1].interactable = false;
            switch (tutorialControlMode)
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
            confirm.onClick.AddListener(() => { IncrementTutorialTextCount(); gameMgr.PlayerAttackEffect(); onDiceRoll = true; });
        }
        else
        {
            reRoll.onClick.AddListener(() => DiceRoll());
            confirm.onClick.AddListener(() => { gameMgr.PlayerAttackEffect(); onDiceRoll = true; });
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
        onDiceRoll = true;
        onResult = true;
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

            if (gameMgr.artifact.playersArtifacts[8] == 1)//9번 유물
            {
                rerollCount = gameMgr.artifact.valueData.Value8 + 2;
            }
            else
            {
                rerollCount = 2;
            }
            reRoll.GetComponentInChildren<TextMeshProUGUI>().text = "재굴림 : " + rerollCount.ToString();
        }
        else
        {
            rerollCount--;
            reRoll.GetComponentInChildren<TextMeshProUGUI>().text = "재굴림 : " + rerollCount.ToString();
        }



        for (int i = 0; i < selectedDiceCount; i++)
        {
            Action<int> spinCallback = (diceIndex) =>
            {
                dices[diceIndex].GetComponentInChildren<TextMeshProUGUI>().text = dicesValues[diceIndex].ToString();
                countToResult++;
            };

            Action<int> tutorialCallback = (diceIndex) =>
            {
                dices[diceIndex].GetComponentInChildren<TextMeshProUGUI>().text = dicesValues[diceIndex].ToString();
                countToResult++;
                gameMgr.tutorial.eventCount++;
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
        if (gameMgr.currentDiceCount == GameMgr.DiceCount.two && (index == 2 || index == 3 || index == 4))
        {
            dicesValues[index] = 0;
            yield break;
        }
        if (gameMgr.currentDiceCount == GameMgr.DiceCount.three && (index == 3 || index == 4))
        {
            dicesValues[index] = 0;
            yield break;
        }
        if (gameMgr.currentDiceCount == GameMgr.DiceCount.four && index == 4)
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
            switch (gameMgr.currentDiceCount)
            {
                case GameMgr.DiceCount.two:
                    SpinControlers[index].DiceSpin(30 + index * 24, RotatePos.posList[dicesValues[index] - 1], this, () => callback(index));
                    break;
                case GameMgr.DiceCount.three:
                    SpinControlers[index].DiceSpin(30 + index * 16, RotatePos.posList[dicesValues[index] - 1], this, () => callback(index));
                    break;
                case GameMgr.DiceCount.four:
                    SpinControlers[index].DiceSpin(30 + index * 12, RotatePos.posList[dicesValues[index] - 1], this, () => callback(index));
                    break;
                case GameMgr.DiceCount.five:
                    SpinControlers[index].DiceSpin(30 + index * 9, RotatePos.posList[dicesValues[index] - 1], this, () => callback(index));
                    break;
            }
        }
        else
        {
            SpinControlers[index].DiceSpin(30, RotatePos.posList[dicesValues[index] - 1], this, () => callback(index));
        }
        yield return null;
    }

    public void ButtonSelect(int i)
    {
        if (!onDiceRoll)
        {
            if (!buttonToggle[i])
            {
                selectedDice.Remove(i);
                dices[i].GetComponent<Image>().color = new Color(0x57 / 255f, 0x57 / 255f, 0x57 / 255f);
                buttonToggle[i] = true;
                buttonLock[i].gameObject.SetActive(true);
            }
            else if (buttonToggle[i])
            {
                selectedDice.Add(i);
                dices[i].GetComponent<Image>().color = new Color(0x214 / 255f, 0x214 / 255f, 0x214 / 255f);
                buttonToggle[i] = false;
                buttonLock[i].gameObject.SetActive(false);
            }
            gameMgr.audioSource.PlayOneShot(gameMgr.audioClips[7]);
        }
    }

    public void DiceTwo()
    {
        dices[2].gameObject.SetActive(false);
        dices[3].gameObject.SetActive(false);
        dices[4].gameObject.SetActive(false);
        selectedDice.Clear();
        for (int i = 0; i < 2; i++)
        {
            selectedDice.Add(i);     // 시작할때 기본 숫자 세팅
        }
    }

    public void DiceThree()
    {
        dices[2].gameObject.SetActive(true);
        dices[3].gameObject.SetActive(false);
        dices[4].gameObject.SetActive(false);
        selectedDice.Clear();
        for (int i = 0; i < 3; i++)
        {
            selectedDice.Add(i);     // 시작할때 기본 숫자 세팅
        }
    }
    public void DiceFour()
    {
        dices[2].gameObject.SetActive(true);
        dices[3].gameObject.SetActive(true);
        dices[4].gameObject.SetActive(false);
        selectedDice.Clear();
        for (int i = 0; i < 4; i++)
        {
            selectedDice.Add(i);     // 시작할때 기본 숫자 세팅
        }
    }
    public void DiceFive()
    {
        dices[2].gameObject.SetActive(true);
        dices[3].gameObject.SetActive(true);
        dices[4].gameObject.SetActive(true);
        selectedDice.Clear();
        for (int i = 0; i < 5; i++)
        {
            selectedDice.Add(i);     // 시작할때 기본 숫자 세팅
        }
    }


    public void EnemyDiceRoll()
    {
        if (stageMgr.currentStage == 7)
        {
            if (stageMgr.currentField == 2 || stageMgr.currentField == 3)
            {
                foreach (var boss in stageMgr.enemies) 
                {
                    if(boss.isBoss)
                    {
                        boss.WindEffect();
                    }
                }
            }
        }
        switch (currentEnemyDice)
        {
            case 1:
                enemyValue = UnityEngine.Random.Range(1, 7);
                gameMgr.enemyValue = enemyValue;
                Action enemySpincallback = () => { gameMgr.CurrentStatus = GameMgr.TurnStatus.MonsterAttack; };
                enemyDice.DiceSpin(30, RotatePos.posList[enemyValue - 1], this, () => enemySpincallback());
                break;
            case 2:
                enemyValue = UnityEngine.Random.Range(1, 7);
                enemyValue2 = UnityEngine.Random.Range(1, 7);   
                gameMgr.enemyValue = enemyValue + enemyValue2;
                Action enemySpincallback2 = () => { gameMgr.CurrentStatus = GameMgr.TurnStatus.MonsterAttack; };
                enemyDiceTwin1.DiceSpin(30, RotatePos.posList[enemyValue - 1], this);
                enemyDiceTwin2.DiceSpin(30, RotatePos.posList[enemyValue2 - 1], this, () => enemySpincallback2());
                break;
            case 3:
                enemyValue = UnityEngine.Random.Range(1, 7);
                enemyValue2 = UnityEngine.Random.Range(1, 7);
                enemyValue3 = UnityEngine.Random.Range(1, 7);
                gameMgr.enemyValue = enemyValue + enemyValue2 + enemyValue3;
                Action enemySpincallback3 = () => { gameMgr.CurrentStatus = GameMgr.TurnStatus.MonsterAttack; };
                enemyDice.DiceSpin(30, RotatePos.posList[enemyValue - 1], this);
                enemyDiceTriple1.DiceSpin(30, RotatePos.posList[enemyValue2 - 1], this);
                enemyDiceTriple2.DiceSpin(30, RotatePos.posList[enemyValue3 - 1], this, () => enemySpincallback3());
                break;
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
        currentEnemyDice = i;
        switch (i)
        {
            case 1:
                enemyDice.gameObject.SetActive(true);
                enemyDiceTwin1.gameObject.SetActive(false);
                enemyDiceTwin2.gameObject.SetActive(false);
                enemyDiceTriple1.gameObject.SetActive(false);
                enemyDiceTriple2.gameObject.SetActive(false);
                break;
            case 2:
                enemyDice.gameObject.SetActive(false);
                enemyDiceTwin1.gameObject.SetActive(true);
                enemyDiceTwin2.gameObject.SetActive(true);
                enemyDiceTriple1.gameObject.SetActive(false);
                enemyDiceTriple2.gameObject.SetActive(false);
                break;
            case 3:
                enemyDice.gameObject.SetActive(true);
                enemyDiceTwin1.gameObject.SetActive(false);
                enemyDiceTwin2.gameObject.SetActive(false);
                enemyDiceTriple1.gameObject.SetActive(true);
                enemyDiceTriple2.gameObject.SetActive(true);
                break;
        }
    }

}
