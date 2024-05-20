using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameMgr;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialPanel;
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
    public int textCount = 0;

    [SerializeField]
    private Image dices;
    [SerializeField]
    private Image damage;
    [SerializeField]
    private Image ranks;

    private List<Coroutine> tutoCorutines = new List<Coroutine>(); // 코루틴관리자

    bool eventTrigger;
    public int eventCount = 0;
    private TutorialStep currentStep;

    private enum TutorialStep
    {
        None = -1,
        Welcome = 0,
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
        FinalAttackPrompt,
        EndTutorial
    }

    private void Awake()
    {
        skipButton.onClick.AddListener(() => { skipPanel.gameObject.SetActive(true); });
        skipYes.onClick.AddListener(() =>  TutorialSkip());
        skipNo.onClick.AddListener(() => { skipPanel.gameObject.SetActive(false); });
        nextButton.onClick.AddListener(() => NextStep());
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
        switch (currentStep)
        {
            case TutorialStep.Welcome:
                if (!eventTrigger)
                {
                    DiceMgr.Instance.onDiceRoll = true;
                    eventTrigger = true;
                }
                tutorialText.text = "Wizard Of Dice에 오신것을 \n 환영합니다!";
                break;
            case TutorialStep.Introduction:
                tutorialText.text = "Wizard Of Dice는 주사위를 굴려서 \n마법력을 모으고 조합된 마법을 사용해\n 던전을 탐험하는 게임입니다.";
                break;
            case TutorialStep.ShowDice:
                tutorialText.text = "이것이 당신의 주사위입니다";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    Coroutine coroutine = StartCoroutine(DiceColorCycle());
                    tutoCorutines.Add(coroutine);
                }
                break;
            case TutorialStep.RollDicePrompt:
                tutorialText.text = "주사위를 굴려볼까요?";
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
                    tutorialText.text = "4와 6이 나왔네요!";
                    nextButton.interactable = true;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case TutorialStep.ShowMagicPower:
                tutorialText.text = "당신의 마법력은 주사위 눈금의 \n 총합으로 결정되어 이곳에 표시됩니다! \n 4와 6의 합계인 10이 보이시나요?";
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
                tutorialText.text = "주사위 숫자가 마음에 안드신다고요? \n 욕심쟁이네요! 주사위는 \'재굴림\' \n버튼으로 다시 굴릴 수 있습니다.";
                break;
            case TutorialStep.LockDicePrompt:
                tutorialText.text = "또한 굴리기 전에 주사위를 누르시면 \n 주사위를 \'고정\' 할 수 있답니다! 다만 \n 한번 고정한 주사위는 다신 굴리지 못해요.";
                break;
            case TutorialStep.LockAndRerollDice:

                DiceMgr.Instance.onDiceRoll = false;
                tutorialText.text = "그럼 4를 고정하고 재굴림을 해볼까요?";
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
                    tutorialText.text = "4가 두개가 나왔습니다!";
                    nextButton.interactable = true;
                    DiceMgr.Instance.tutorialControlMode = 0;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case TutorialStep.ShowMagicDescription:
                tutorialText.text = "왼쪽 아래에 \'원 페어\'가 보이시나요? \n 그것이 당신이 가진 \'마법\'입니다!";
                if (!eventTrigger)
                {
                    eventTrigger = true;
                    Coroutine coroutine = StartCoroutine(RanksColorCycle());
                    tutoCorutines.Add(coroutine);
                }
                break;
            case TutorialStep.ShowMagicCombination:
                tutorialText.text = "주사위 눈이 특정 마법의 조건에 맞으면 \n 당신의 마법력이 강화됩니다!";
                if (!eventTrigger)
                {
                    ranks.CrossFadeColor(Color.white, 1, true, true);
                }
                break;
            case TutorialStep.OnePairDescription:
                tutorialText.text = "'원 페어' 는 같은 주사위 눈이 \n 두 개일때 활성화 되는 마법입니다.";
                break;
            case TutorialStep.MagicUpgradeDescription:
                tutorialText.text = "마법은 스테이지를 클리어 할때마다\n 추가로 획득할 수도, 기존 마법을 \n 업그레이드 할 수도 있습니다.";
                break;
            case TutorialStep.ShowPlayerAttack:
                nextButton.interactable = false;
                tutorialText.text = "그럼 이제 공격을 해볼까요? \n '공격' 버튼을 눌러보세요";
                DiceMgr.Instance.tutorialControl = true;
                DiceMgr.Instance.tutorialControlMode = 2;
                break;
            case TutorialStep.ShowEnemyAttack:
                if (!eventTrigger)
                {
                    tutorialText.text = " ";
                    eventTrigger = true;
                    tutorialPanel.gameObject.transform.position += new Vector3(0, -1000, 0);
                    GameMgr.Instance.tutorialMode = true;
                }
                if (eventCount == 1)
                {
                    tutorialText.text = "놀라셨나요? 여러분의 공격이 끝나면 \n 적들도 주사위를 굴려서 공격해온답니다.";
                    nextButton.interactable = true;
                    DiceMgr.Instance.tutorialControlMode = 0;
                    DiceMgr.Instance.onDiceRoll = true;
                }
                break;
            case TutorialStep.ShowEnemyDamageCalculation:
                tutorialText.text = "적의 머리 위에 있는 주사위 값에, \n 체력바 왼쪽의 공격력을 더한것이 \n 적의 데미지 입니다.";
                break;
            case TutorialStep.ShowAttackOrder:
                tutorialText.text = "여러분의 마법이 적중하는 순서도, \n 적들이 공격하는 순서도 항상 \n '왼쪽'부터 계산됩니다. \n 이 점 꼭 주의해주세요!";
                break;
            case TutorialStep.ExplainMagicVariations:
                tutorialText.text = "'원 페어'는 하나의 적밖에 \n 공격할 수 없지만, \n 스테이지를 클리어 해 나가면 얻을 수 \n 있는 광역 마법들도 존재합니다";
                break;
            case TutorialStep.ShowMagicCombinationExplanation:
                tutorialText.text = "여러분이 가진 마법들과 주사위의 \n 조건이 맞다면, 마법이 전부 합쳐집니다! \n 마법력이 전부 합쳐지는것은 물론이고, \n 광역 마법이 될 수도 있지요.";
                break;
            case TutorialStep.FinalAttackPrompt:
                tutorialText.text = "자, 그럼 이제 눈앞의 적을 쓰러트려 보세요! \n 멋진 보상이 기다리고 있답니다.";
                break;
            case TutorialStep.EndTutorial:
                if (!eventTrigger)
                {
                    eventTrigger = true;
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

    private void TutorialSkip()
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
        //DiceMgr.Instance.InfoClear();
        //Instance.currentDiceCount = DiceCount.three;
        //DiceMgr.Instance.DiceThree();
        //DiceMgr.Instance.DiceRoll();
        Instance.ui.GetDice();

    }

}
