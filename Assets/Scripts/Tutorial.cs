using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameMgr;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialPanel;
    [SerializeField]
    private GameObject skipPanel;
    [SerializeField]
    private TextMeshProUGUI tutorialText;

    [SerializeField]
    private Button nextButton;
    public Button skipButton;
    [SerializeField]
    private Button skipYes;
    [SerializeField]
    private Button skipNo;
    public int textCount;

    [SerializeField]
    private Image dices;
    [SerializeField]
    private Image damage;
    [SerializeField]
    private Image ranks;

    private List<Coroutine> tutoCorutines = new List<Coroutine>(); // 内风凭包府磊

    bool eventTrigger;
    public int eventCount = 0;
    private TutorialStep currentStep;

    [SerializeField]
    private GameObject panelPos;

    private enum TutorialStep
    {
        None = -1,
        Welcome = 30005,
        Introduction,
        ShowDice,
        RollDicePrompt,
        RollDice,
        ShowMagicPower,
        RerollDicePrompt,
        LockDicePrompt,
        LockAndRerollDice,
        ShowTwoPairs,
        ShowMagicDescription,
        ShowMagicCombination,
        OnePairDescription,
        MagicUpgradeDescription,
        ShowPlayerAttack,
        ShowEnemyAttack,
        ShowEnemyDamageCalculation,
        ShowAttackOrder,
        ExplainMagicVariations,
        ShowMagicCombinationExplanation,
        TurnExplanation,
        FinalAttackPrompt,
        EndTutorial
    }

    private void Awake()
    {
        skipButton.onClick.AddListener(() => { skipPanel.gameObject.SetActive(true); });
        skipYes.onClick.AddListener(() => TutorialSkip());
        skipNo.onClick.AddListener(() => { skipPanel.gameObject.SetActive(false); });
        nextButton.onClick.AddListener(() => NextStep());
        textCount = 30005;
    }

    public void NextStep()
    {
        textCount++;
        currentStep = (TutorialStep)textCount;
        eventTrigger = false;
        eventCount = 0;
    }

    private void Update()
    {
        tutorialText.text = DataTableMgr.Get<TextTable>(DataTableIds.Text).Get(textCount);
        switch (currentStep)
        {
            case TutorialStep.Welcome:
                if (!eventTrigger)
                {
                    DiceMgr.Instance.onDiceRoll = true;
                    eventTrigger = true;
                }
                break;
            case TutorialStep.Introduction:
                break;
            case TutorialStep.ShowDice:
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    Coroutine coroutine = StartCoroutine(DiceColorCycle());
                    tutoCorutines.Add(coroutine);
                }
                break;
            case TutorialStep.RollDicePrompt:
                DiceMgr.Instance.onDiceRoll = true;
                break;
            case TutorialStep.RollDice:
                if (!eventTrigger)
                {
                    dices.CrossFadeColor(UsedColor.usedColor, 1, true, true);
                    nextButton.interactable = false;
                    eventTrigger = true;
                    DiceMgr.Instance.DiceRoll(true, GameMode.Tutorial);
                }
                if (eventCount == 2)
                {
                    nextButton.interactable = true;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case TutorialStep.ShowMagicPower:
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    Coroutine coroutine = StartCoroutine(DamageColorCycle());
                    tutoCorutines.Add(coroutine);

                }
                break;
            case TutorialStep.RerollDicePrompt:
                if (!eventTrigger)
                {
                    damage.CrossFadeColor(Color.white, 1, true, true);
                }
                break;
            case TutorialStep.LockDicePrompt:
                break;
            case TutorialStep.LockAndRerollDice:

                DiceMgr.Instance.onDiceRoll = false;
                if (!eventTrigger)
                {
                    eventTrigger = true;

                    DiceMgr.Instance.ButtonSelect(0);
                    DiceMgr.Instance.tutorialControl = true;
                    DiceMgr.Instance.tutorialControlMode = 1;
                    DiceMgr.Instance.TutorialButtonControl(true);
                }
                nextButton.interactable = false;
                break;
            case TutorialStep.ShowTwoPairs:
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    DiceMgr.Instance.onDiceRoll = true;
                    //DiceMgr.Instance.TutorialButtonControl(false);
                    DiceMgr.Instance.manipulList[1] = 4;
                }

                if (eventCount == 1)
                {
                    nextButton.interactable = true;
                    DiceMgr.Instance.tutorialControlMode = 0;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case TutorialStep.ShowMagicDescription:
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    Coroutine coroutine = StartCoroutine(RanksColorCycle());
                    tutoCorutines.Add(coroutine);
                }
                break;
            case TutorialStep.ShowMagicCombination:
                if (!eventTrigger)
                {
                    ranks.CrossFadeColor(Color.white, 1, true, true);
                }
                break;
            case TutorialStep.OnePairDescription:
                break;
            case TutorialStep.MagicUpgradeDescription:
                break;
            case TutorialStep.ShowPlayerAttack:
                nextButton.interactable = false;
                DiceMgr.Instance.tutorialControl = true;
                DiceMgr.Instance.tutorialControlMode = 2;
                break;
            case TutorialStep.ShowEnemyAttack:
                if (!eventTrigger)
                {
                    tutorialText.gameObject.SetActive(false);
                    eventTrigger = true;
                    tutorialPanel.gameObject.transform.position = panelPos.gameObject.transform.position;
                    GameMgr.Instance.tutorialMode = true;
                }
                if (eventCount == 1)
                {
                    tutorialText.gameObject.SetActive(true);
                    nextButton.interactable = true;
                    DiceMgr.Instance.tutorialControlMode = 0;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case TutorialStep.ShowEnemyDamageCalculation:
                break;
            case TutorialStep.ShowAttackOrder:
                break;
            case TutorialStep.ExplainMagicVariations:
                break;
            case TutorialStep.ShowMagicCombinationExplanation:
                break;
            case TutorialStep.TurnExplanation:
                break;
            case TutorialStep.FinalAttackPrompt:
                break;
            case TutorialStep.EndTutorial:
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    DiceMgr.Instance.manipulList[1] = 0;
                    DiceMgr.Instance.TutorialButtonControl(false);
                    DiceMgr.Instance.tutorialControl = false;
                    GameMgr.Instance.tutorialMode = false;
                    tutorialPanel.gameObject.SetActive(false);
                    DiceMgr.Instance.DiceTwo();
                    DiceMgr.Instance.DiceRoll(true);
                }
                break;

        }

    }

    public IEnumerator DiceColorCycle()
    {
        while (currentStep == TutorialStep.ShowDice)
        {
            dices.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            dices.CrossFadeColor(UsedColor.usedColor, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public IEnumerator DamageColorCycle()
    {
        while (currentStep == TutorialStep.ShowMagicPower)
        {
            damage.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            damage.CrossFadeColor(Color.white, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }
    public IEnumerator RanksColorCycle()
    {
        while (currentStep == TutorialStep.ShowMagicDescription)
        {
            ranks.CrossFadeColor(new Color(113 / 255f, 255 / 255f, 0 / 255f), 1, true, true);
            yield return new WaitForSecondsRealtime(1);

            ranks.CrossFadeColor(Color.white, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public void TutorialSkip()
    {
        currentStep = TutorialStep.None;
        textCount = -1;
        eventTrigger = false;
        eventCount = 0;
        DiceMgr.Instance.StopAllActiveCoroutines();

        dices.color = UsedColor.usedColor;
        damage.color = UsedColor.whiteColor;
        ranks.color = UsedColor.whiteColor;

        DiceMgr.Instance.tutorialControl = false;
        Instance.tutorialMode = false;
        //DiceMgr.Instance.onDiceRoll = false;
        tutorialPanel.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        skipPanel.gameObject.SetActive(false);

        DiceMgr.Instance.TutorialButtonControl(false);
        DiceMgr.Instance.manipulList[1] = 0;
        //DiceMgr.Instance.selectedDice.Clear();

        Destroy(StageMgr.Instance.enemies[0].gameObject);
        StageMgr.Instance.enemies.Clear();
        PlayerPrefs.SetInt("Tutorial", 1);
        //DiceMgr.Instance.InfoClear();
        //Instance.currentDiceCount = DiceCount.three;
        //DiceMgr.Instance.DiceThree();
        //DiceMgr.Instance.DiceRoll();
        Instance.ui.GetDice();
    }

}
