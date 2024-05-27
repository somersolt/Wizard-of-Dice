using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Reflection;
using Unity.VisualScripting;

public class DiceMgr : MonoBehaviour
{
    private static DiceMgr instance;

    public static DiceMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DiceMgr>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("DiceMgr");
                    instance = obj.AddComponent<DiceMgr>();
                }
            }
            return instance;
        }

    }    // 싱글톤 패턴


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
        get {return checkedRanksList; }
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
    public int enemyValue;


    public List<Coroutine> activeCoroutines = new List<Coroutine>(); // 코루틴관리자
    public List<SpinControl> coroutineList = new List<SpinControl>(); // 코루틴관리자2


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

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
        confirm.onClick.AddListener(() => { GameMgr.Instance.PlayerAttackEffect(); onDiceRoll = true; });
    }

    public void InfoClear()
    {
        selectedDice.Clear();
        onResult = false;
        onDiceRoll = false;
        totalValue = 0;
        for (int i = 0; i < (int)GameMgr.Instance.currentDiceCount; i++)
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
            for (int i = 0; i < (int)GameMgr.Instance.currentDiceCount; i++)
            {
                totalValue += dicesValues[i];
                //buttonToggle[i] = false; // 전부 고정 푸는 코드
                if (buttonToggle[i] == false)
                {
                    dices[i].GetComponent<Image>().color = new Color(0x214 / 255f, 0x214 / 255f, 0x214 / 255f);
                    selectedDice.Add(i);
                }
            }
            GameMgr.Instance.SetResult(checkedRanksList, totalValue);
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
            confirm.onClick.AddListener(() => { IncrementTutorialTextCount(); GameMgr.Instance.PlayerAttackEffect(); onDiceRoll = true; });
        }
        else
        {
            reRoll.onClick.AddListener(() => DiceRoll());
            confirm.onClick.AddListener(() => { GameMgr.Instance.PlayerAttackEffect(); onDiceRoll = true; });
        }
    }
    private void IncrementTutorialTextCount()
    {
        GameMgr.Instance.tutorial.NextStep();
    }
    public void DiceRoll(bool starting = false, GameMode mode = GameMode.Default)
    {
        GameMgr.Instance.ScrollsClear();
        GameMgr.Instance.audioSource.PlayOneShot(GameMgr.Instance.audioClips[6]);
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
            selectedDiceCount = (int)GameMgr.Instance.currentDiceCount;
            for (int i = 0; i < selectedDiceCount; i++)
            {
                buttonLock[i].gameObject.SetActive(false);
            }

            if (GameMgr.Instance.artifact.playersArtifacts[8] == 1)//9번 유물
            {
                rerollCount = GameMgr.Instance.artifact.valueData.Value8 + 2;
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
                GameMgr.Instance.tutorial.eventCount++;
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
                        Coroutine newcoroutine= StartCoroutine(SelectDiceRoll(selectedDice[i], starting, tutorialCallback, 4));
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

        for (int i = 0; i < (int)GameMgr.Instance.currentDiceCount; i++)
        {
            numbersCount[dicesValues[i] - 1]++;
        }

    }

    private IEnumerator SelectDiceRoll(int index, bool starting, Action<int> callback, int manipul = 0)
    {
        if (GameMgr.Instance.currentDiceCount == GameMgr.DiceCount.two && (index == 2 || index == 3 || index == 4))
        {
            dicesValues[index] = 0;
            yield break;
        }
        if (GameMgr.Instance.currentDiceCount == GameMgr.DiceCount.three && (index == 3 || index == 4))
        {
            dicesValues[index] = 0;
            yield break;
        }
        if (GameMgr.Instance.currentDiceCount == GameMgr.DiceCount.four && index == 4)
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
            switch (GameMgr.Instance.currentDiceCount)
            {
                case GameMgr.DiceCount.two:
                    SpinControlers[index].DiceSpin(30 + index * 24, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
                    break;
                case GameMgr.DiceCount.three:
                    SpinControlers[index].DiceSpin(30 + index * 16, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
                    break;
                case GameMgr.DiceCount.four:
                    SpinControlers[index].DiceSpin(30 + index * 12, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
                    break;
                case GameMgr.DiceCount.five:
                    SpinControlers[index].DiceSpin(30 + index * 9, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
                    break;
            }
        }
        else
        {
            SpinControlers[index].DiceSpin(30, RotatePos.posList[dicesValues[index] - 1], () => callback(index));
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
            GameMgr.Instance.audioSource.PlayOneShot(GameMgr.Instance.audioClips[7]);
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
        enemyValue = UnityEngine.Random.Range(1, 7);
        GameMgr.Instance.enemyValue = enemyValue;
        Action enemySpincallback = () => { GameMgr.Instance.CurrentStatus = GameMgr.TurnStatus.MonsterAttack; };
        enemyDice.DiceSpin(30, RotatePos.posList[enemyValue - 1], () => enemySpincallback());
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

}
